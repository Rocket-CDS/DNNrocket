using DNNrocketAPI;
using DNNrocketAPI.Componants;
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

    public class ArticleDataList
    {
        private string _langRequired;
        private List<ArticleData> _articleList;
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "ROCKETMOD";
        private DNNrocketController _objCtrl;
        private ModuleParams _moduleParams;

        public ArticleDataList(SimplisityInfo paramInfo, string langRequired, bool populate)
        {
            ModuleId = paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (ModuleId == 0) ModuleId = paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");

            _moduleParams = new ModuleParams(ModuleId);

            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetCurrentCulture();
            _objCtrl = new DNNrocketController();

            SessionParamData = new SessionParams(paramInfo);

            if (populate) Populate();
        }
        public void Populate()
        {
            var searchFilter = " and R1.ModuleId = " + _moduleParams.ModuleIdDataSource;
            searchFilter += _moduleParams.GetFilterSQL(SessionParamData.Info, SessionParamData.FilterIndex);
            SessionParamData.RowCount = _objCtrl.GetListCount(-1, -1, _entityTypeCode, searchFilter, _langRequired, _tableName);
            DataList = _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, _langRequired, _moduleParams.OrderBySQL(SessionParamData.OrderByIndex), 0, SessionParamData.Page, SessionParamData.PageSize, SessionParamData.RowCount, _tableName);
        }
        public void DeleteAll()
        {
            var l = GetAllArticlesForModule();
            foreach (var r in l)
            {
                _objCtrl.Delete(r.ItemID);
            }
        }

        public int ModuleId { get; set; }
        public SessionParams SessionParamData { get; set; }
        public List<SimplisityInfo> DataList { get; private set; }

        public List<ArticleData> GetArticleList()
        {
            _articleList = new List<ArticleData>();
            foreach (var o in DataList)
            {
                var articleData = new ArticleData(o.ItemID, ModuleId, _langRequired);
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
                var articleData = new ArticleData(o.ItemID, ModuleId, _langRequired);
                _articleList.Add(articleData);
            }
            return _articleList;
        }


        public void SortOrderMove(int toItemId)
        {
            SortOrderMove(SessionParamData.SortActivate, toItemId);
        }
        public void SortOrderMove(int fromItemId, int toItemId)
        {
            if (fromItemId > 0 && toItemId > 0)
            {
                var moveData = new ArticleData(fromItemId, ModuleId, _langRequired);
                var toData = new ArticleData(toItemId, ModuleId, _langRequired);

                var newSortOrder = toData.SortOrder - 1;
                if (moveData.SortOrder < toData.SortOrder) newSortOrder = toData.SortOrder + 1;

                moveData.SortOrder = newSortOrder;
                moveData.Update();
                SessionParamData.CancelItemSort();
            }
        }

        public List<SimplisityInfo> GetAllArticlesForModule()
        {
            var searchFilter = " and R1.ModuleId = " + ModuleId + " ";
            return _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, _langRequired, _moduleParams.OrderBySQL(SessionParamData.OrderByIndex), 0, 0, 0, 0, _tableName);
        }

        public void SortOrderReIndex()
        {
            var searchFilter = " and R1.ModuleId = " + ModuleId + " ";
            var sortOrderList = _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, "", _moduleParams.OrderBySQL(SessionParamData.OrderByIndex), 0, 0, 0, 0, _tableName);
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

            var exportDirMapPath = PortalUtils.TempDirectoryMapPath() + "\\export_" + ModuleId;
            Directory.Delete(exportDirMapPath, true);
            Directory.CreateDirectory(exportDirMapPath);

            var exportFileMapPath = exportDirMapPath + "\\export.xml";
            if (File.Exists(exportFileMapPath)) File.Delete(exportFileMapPath);
            FileUtils.SaveFile(exportFileMapPath, exportData);

            //TODO: Add files needed for export Images, docs, etc.


            // Create zip
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\export_" + ModuleId + ".zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            ZipFile.CreateFromDirectory(exportDirMapPath, exportZipMapPath);

            Directory.Delete(exportDirMapPath, true);

            return exportZipMapPath;
        }

    }

}
