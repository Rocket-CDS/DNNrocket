using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketSettings
{

    public class SettingsData : SimplisityData
    {
        private List<SimplisityInfo> _dataList;
        private int _tabid;
        private int _moduleid;
        private string _guidKey;
        private string _langRequired;
        private string _entityTypeCode;
        private string _listName;
        private List<string> _cultureList;
        private bool _onlyRead;
        private string _tableName;
        private string _cacheKey;
        private bool _debugMode;

        public SimplisityInfo Info;

        public SettingsData(string guidKey, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata", bool onlyRead = false, string tableName = "DNNrocket", bool debugMode = false)
        {
            InitSettingsData(-1, -1, guidKey, langRequired, entityTypeCode, listname, onlyRead, tableName, debugMode);
        }


        public SettingsData(int tabId, int moduleId, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata", bool onlyRead = false, string tableName = "DNNrocket", bool debugMode = false)
        {
            var guidKey = "moduleid" + moduleId;
            InitSettingsData(tabId, moduleId, guidKey, langRequired, entityTypeCode, listname, onlyRead, tableName, debugMode);

        }


        private void InitSettingsData(int tabId, int moduleId, string guidKey, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata", bool onlyRead = false, string tableName = "DNNrocket", bool debugMode = false)
        {
            _debugMode = debugMode;
            _tableName = tableName;
            _onlyRead = onlyRead;
            _entityTypeCode = entityTypeCode;
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();
            _tabid = tabId;
            _moduleid = moduleId;
            _guidKey = guidKey;
            _dataList = new List<SimplisityInfo>();
            _listName = listname;
            _cultureList = GetCultureList();

            _cacheKey = guidKey + "*" + langRequired + "*" + entityTypeCode + "*" + tableName + "*" + DNNrocketUtils.GetPortalId() + "*" + _moduleid + "*" + tabId;

            if (_moduleid > 0)
            {
                Info = GetSettingData(guidKey, _langRequired);
                Populate();
                PopulateList();
            }
            else
            {
                Info = new SimplisityInfo();
            }

        }


        public List<string> GetCultureList()
        {
            var objCtrl = new DNNrocketController();

            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ModuleId = _moduleid;
            }

            // Load ALL language records, so we can update lists correctly
            var cultureList = new List<string>();
            if (Info.ParentItemId > 0)
            {
                var l = objCtrl.GetList(-1, -1, _entityTypeCode + "LANG", " and ParentItemId = " + Info.ItemID);
                foreach (var s in l)
                {
                    if (!cultureList.Contains(s.Lang)) cultureList.Add(s.Lang);
                }
            }

            var ls = DNNrocketUtils.GetCultureCodeList();
            foreach (var s in ls)
            {
                if (!cultureList.Contains(s)) cultureList.Add(s);
            }

            return cultureList;
        }

        public void Populate()
        {
            foreach (var cultureCode in _cultureList)
            {
                var s = GetSettingData(_guidKey, cultureCode);
                if (s != null)
                {
                    AddSimplisityInfo(s, cultureCode);
                }
            }
        }

        public void PopulateList()
        {
            _dataList = new List<SimplisityInfo>();
            var objCtrl = new DNNrocketController();
            if (!SimplisityInfoList.ContainsKey(_langRequired))
            {
                AddSimplisityInfo(new SimplisityInfo(_langRequired), _langRequired);  // new edit lang
            }
            var info = SimplisityInfoList[_langRequired];
            if (info != null)
            {
                _dataList = info.GetList(_listName);
            }
        }

        public void Delete()
        {
            var info = GetSettingData(_guidKey, DNNrocketUtils.GetEditCulture());
            if (info != null)
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(info.ItemID);
                ClearCache();
                Populate();
                PopulateList();
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            var editlang = DNNrocketUtils.GetEditCulture();

            var dbInfo = GetSettingData(_guidKey, editlang);

            if (!SimplisityInfoList.ContainsKey(editlang))
            {
                // new lang record, so add it to list
                AddSimplisityInfo(dbInfo, editlang);
            }

            RemovedDeletedListRecords(_listName, dbInfo, postInfo);

            SortListRecordsOnSave(_listName, postInfo, editlang);

            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                SaveSettingData(_guidKey, listItem.Value);
            }

            ClearCache();
            Populate();
            PopulateList();
        }


        public void AddRow()
        {
            AddListItem(_listName);

            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                SaveSettingData(_guidKey, listItem.Value);
            }

            ClearCache();
            Populate();
            PopulateList();
        }

        public void ClearCache()
        {
            CacheUtils.RemoveCache(_cacheKey);
        }


        public string ExportData(bool withTextData = false)
        {
            var xmlOut = "<root>";
            foreach (var listItem in SimplisityInfoList)
            {
                xmlOut += listItem.Value.ToXmlItem(withTextData);
            }
            xmlOut += "</root>";

            return xmlOut;
        }

        public void ImportData(string XmlIn)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlIn);

            var nodList = xmlDoc.SelectNodes("root/item");
            foreach (XmlNode nod in nodList)
            {
                var s = new SimplisityInfo();
                s.FromXmlItem(nod.OuterXml);
                AddSimplisityInfo(s, s.Lang);
                if (_langRequired == s.Lang)
                {
                    Info = s;
                }
            }
            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                SaveSettingData(_guidKey, listItem.Value);
            }

        }


        #region "properties"

        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
        public string EntityTypeCode { get { return _entityTypeCode; } set { _entityTypeCode = value; } }        

        public List<SimplisityInfo> List
        {
            get { return _dataList; }
        }

        #endregion

        #region "private methods"

        private SimplisityInfo GetSettingData(string guidKey, string cultureCode)
        {
            var info = (SimplisityInfo)CacheUtils.GetCache(_cacheKey);
            if (info == null || _debugMode)
            {
                var objCtrl = new DNNrocketController();
                info = objCtrl.GetData(guidKey, _entityTypeCode, cultureCode, -1, _moduleid, _onlyRead, _tableName);
                CacheUtils.SetCache(_cacheKey, info);
            }
            return info;
        }

        private void SaveSettingData(string guidKey, SimplisityInfo sInfo)
        {
            var objCtrl = new DNNrocketController();
            sInfo.ModuleId = _moduleid;
            objCtrl.SaveData(guidKey, _entityTypeCode, sInfo, -1, _moduleid, _tableName);
            CacheUtils.SetCache(_cacheKey, sInfo);
        }

        #endregion


    }

}
