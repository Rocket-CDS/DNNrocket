using DNNrocketAPI;
using RocketSettings;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketMod
{

    public class ArticleDataList : BaseHeaderData
    {
        private int _moduleid;
        private string _langRequired;
        private List<ArticleData> _articleList;
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "ROCKETMOD";
        private DNNrocketController _objCtrl;
        private string _headerCacheKey;
        private string _sortOrderCacheKey;

        public ArticleDataList(int moduleid, string langRequired, bool populate)
        {
            _moduleid = moduleid;
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetCurrentCulture();
            _objCtrl = new DNNrocketController();
            Header = new SimplisityInfo();
            _headerCacheKey = "rocketmodArtcleHEADER" + DNNrocketUtils.GetCurrentUserId();
            _sortOrderCacheKey = _moduleid + "*activatedsortorder";
            if (populate) Populate();
        }
        public void Populate()
        {
            var searchFilter = " and R1.ModuleId = " + _moduleid + " ";
            if (Header.GetXmlProperty("genxml/textbox/searchtext") != "")
            {
                searchFilter += " and ( [XMLData].value('(genxml/textbox/ref)[1]', 'nvarchar(100)') like '%" + Header.GetXmlProperty("genxml/textbox/searchtext") + "%' ";
                searchFilter += " or [XMLData].value('(genxml/lang/genxml/textbox/name)[1]', 'nvarchar(100)') like '%" + Header.GetXmlProperty("genxml/textbox/searchtext") + "%' ) ";
            }
            var searchOrderBy = " " + Header.GetXmlProperty("genxml/hidden/orderby");
            if (String.IsNullOrWhiteSpace(searchOrderBy)) searchOrderBy = " order by R1.[SortOrder] ";

            var rowCount = _objCtrl.GetListCount(-1, -1, _entityTypeCode, searchFilter, _langRequired, _tableName);
            DataList = _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, _langRequired, searchOrderBy, 0, Page, PageSize, rowCount, _tableName);
            RowCount = rowCount;

        }
        public void DeleteAll()
        {
            var l = GetAllArticlesForModule();
            foreach (var r in l)
            {
                _objCtrl.Delete(r.ItemID);
            }
        }

        public List<SimplisityInfo> DataList { get; private set; }

        public List<ArticleData> GetArticleList()
        {
            _articleList = new List<ArticleData>();
            foreach (var o in DataList)
            {
                var articleData = new ArticleData(o.ItemID, _moduleid, _langRequired);
                _articleList.Add(articleData);
            }
            return _articleList;
        }
        public List<ArticleData> GetAllArticleListForModule()
        {
            var l = GetAllArticlesForModule();
            _articleList = new List<ArticleData>();
            foreach (var o in l)
            {
                var articleData = new ArticleData(o.ItemID, _moduleid, _langRequired);
                _articleList.Add(articleData);
            }
            return _articleList;
        }


        public void SortOrderActivate(int ItemId)
        {
            if (ItemId > 0) CacheUtils.SetCache(_sortOrderCacheKey, ItemId);
        }
        public int SortOrderActiveItemId()
        {
            var rtn = CacheUtils.GetCache(_sortOrderCacheKey);
            if (rtn == null) return 0;
            return Convert.ToInt32(rtn);
        }
        public void SortOrderCancel()
        {
            CacheUtils.RemoveCache(_sortOrderCacheKey);
        }

        public void SortOrderMove(int toItemId)
        {
            SortOrderMove(SortOrderActiveItemId(), toItemId);
        }
        public void SortOrderMove(int fromItemId, int toItemId)
        {
            if (fromItemId > 0 && toItemId > 0)
            {
                var moveData = new ArticleData(fromItemId, _moduleid, _langRequired);
                var toData = new ArticleData(toItemId, _moduleid, _langRequired);
                var newSortOrder = toData.SortOrder + 1;
                if (moveData.SortOrder < toData.SortOrder) newSortOrder = toData.SortOrder -1;

                moveData.SortOrder = toData.SortOrder;
                moveData.Update();
                toData.SortOrder = newSortOrder;
                toData.Update();
                SortOrderCancel();
            }
        }

        public List<SimplisityInfo> GetAllArticlesForModule()
        {
            var searchFilter = " and R1.ModuleId = " + _moduleid + " ";
            var searchOrderBy = " order by R1.[SortOrder] ";
            return _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, _langRequired, searchOrderBy, 0, 0, 0, 0, _tableName);
        }

        public void SortOrderReIndex()
        {
            var sortOrderList = GetAllArticlesForModule();
            var lp = 1;
            foreach (var s in sortOrderList)
            {
                s.SortOrder = lp * 100;
                _objCtrl.Update(s);
                lp += 1;
            }
        }

        public string Export()
        {
            // Export DB
            var exportData = "<root>";
            foreach (var s in DataList)
            {
                exportData += s.ToXmlItem();
            }
            exportData += "</root>";

            var exportDirMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\export_" + _moduleid;
            Directory.Delete(exportDirMapPath, true);
            Directory.CreateDirectory(exportDirMapPath);

            var exportFileMapPath = exportDirMapPath + "\\export.xml";
            if (File.Exists(exportFileMapPath)) File.Delete(exportFileMapPath);
            FileUtils.SaveFile(exportFileMapPath, exportData);

            //TODO: Add files needed for export Images, docs, etc.


            // Create zip
            var exportZipMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\export_" + _moduleid + ".zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            ZipFile.CreateFromDirectory(exportDirMapPath, exportZipMapPath);

            Directory.Delete(exportDirMapPath, true);

            return exportZipMapPath;
        }



        #region "HEADER"

        public void CacheHeaderDelete()
        {
            CacheUtils.RemoveCache(_headerCacheKey);
        }

        public void SaveCacheHeader()
        {
            CacheUtils.SetCache(_headerCacheKey, Header);
        }
        public void LoadCacheHeader()
        {
            Header = (SimplisityInfo)CacheUtils.GetCache(_headerCacheKey);
            if (Header == null) Header = new SimplisityInfo();
        }
        public void LoadViewHeader(SimplisityInfo header)
        {
            Header = header;
            if (Header == null) Header = new SimplisityInfo();
        }

        #endregion


    }

}
