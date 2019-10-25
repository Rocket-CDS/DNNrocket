using DNNrocketAPI;
using RocketSettings;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public ArticleDataList(int moduleid, string langRequired)
        {
            _moduleid = moduleid;
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetCurrentCulture();
            _objCtrl = new DNNrocketController();
            Header = new SimplisityInfo();
            _headerCacheKey = "rocketmodArtcleHEADER" + DNNrocketUtils.GetCurrentUserId();
        }
        public void Populate(int dataType = 2)
        {
            var returnLimit = 0; // form
            if (dataType == 1) returnLimit = 1; // list

            var searchFilter = " and R1.ModuleId = " + _moduleid + " ";
            if (Header.GetXmlProperty("genxml/textbox/searchtext") != "")
            {
                searchFilter += " and ( [XMLData].value('(genxml/textbox/ref)[1]', 'nvarchar(100)') like '%" + Header.GetXmlProperty("genxml/textbox/searchtext") + "%' ";
                searchFilter += " or [XMLData].value('(genxml/lang/genxml/textbox/name)[1]', 'nvarchar(100)') like '%" + Header.GetXmlProperty("genxml/textbox/searchtext") + "%' ) ";
            }
            var searchOrderBy = " order by R1.[SortOrder] ";

            var rowCount = _objCtrl.GetListCount(-1, -1, _entityTypeCode, searchFilter, _langRequired, -1, _tableName);
            DataList = _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, _langRequired, searchOrderBy, returnLimit, Page, PageSize, rowCount, -1, _tableName);
            RowCount = rowCount;

            ModuleSettings = new SettingsData(-1, _moduleid, "", "ROCKETMODSETTINGS", "rocketmodsettings", true);

        }
        public void DeleteAll()
        {
            var returnLimit = 0; // form
            var searchFilter = " and R1.ModuleId = " + _moduleid + " ";

            var rowCount = _objCtrl.GetListCount(-1, -1, _entityTypeCode, searchFilter, _langRequired, -1, _tableName);
            var l = _objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _entityTypeCode, searchFilter, _langRequired, "", returnLimit, Page, PageSize, rowCount, -1, _tableName);
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

        public SettingsData ModuleSettings { get; private set; }


    }

}
