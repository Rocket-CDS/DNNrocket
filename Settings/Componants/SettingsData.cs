using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketSettings
{

    public class SettingsData : SimplisityData
    {
        private List<SimplisityInfo> _dataList;
        private int _tabid;
        private int _moduleid;
        private int _parentitemid;
        private string _langRequired;
        private string _entityTypeCode;
        private string _listName;

        public SimplisityInfo Info;

        public SettingsData(int parentitemid, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata")
        {
            _entityTypeCode = entityTypeCode;
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();
            _tabid = -1;
            _moduleid = -1;
            _parentitemid = parentitemid;
            _dataList = new List<SimplisityInfo>();
            _listName = listname;
            if (_parentitemid > 0)
            {
                Populate();
                PopulateList();
            }
        }


        public SettingsData(int tabId, int moduleId, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata")
        {
            _entityTypeCode = entityTypeCode;
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();
            _tabid = tabId;
            _moduleid = moduleId;
            _parentitemid = -1;
            _dataList = new List<SimplisityInfo>();            
            _listName = listname;

            if (_moduleid > 0)
            {
                Populate();
                PopulateList();
            }
        }

        public void Populate()
        {
            var objCtrl = new DNNrocketController();

            // Load ALL language records, so we can update lists correctly
            var langList = DNNrocketUtils.GetCultureCodeList();
            foreach (var cultureCode in langList)
            {
                var guidkey = "moduleid" + _moduleid;
                if (_moduleid <= 0) guidkey = "parentitemid" + _parentitemid;
                var s = GetSettingData(cultureCode);
                if (s != null)
                {
                    AddSimplisityInfo(s, cultureCode);
                    if (_langRequired == cultureCode)
                    {
                        Info = s;
                        Info.ParentItemId = _parentitemid;
                    }
                }
            }
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ModuleId = _moduleid;
                Info.ParentItemId = _parentitemid;
            }

        }

        public void PopulateList()
        {
            _dataList = new List<SimplisityInfo>();
            var objCtrl = new DNNrocketController();
            var info = SimplisityInfoList[_langRequired];
            if (info != null)
            {
                _dataList = info.GetList(_listName);
            }
        }

        public void Delete()
        {
            var info = GetSettingData(DNNrocketUtils.GetEditCulture());
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

            // get current data
            var dbInfo = GetSettingData(DNNrocketUtils.GetEditCulture());

            RemovedDeletedListRecords(_listName, dbInfo, postInfo);

            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                var saveInfo = (SimplisityInfo)postInfo.Clone();
                if (saveInfo.Lang != listItem.Value.Lang)
                {
                    // If it's not the same langauge, update the data with the listItem.
                    saveInfo.RemoveLangRecord();
                    saveInfo.SetLangRecord(listItem.Value.GetLangRecord());

                    // resequance the other language list, by rebuilding from sorted GetList.
                    var l = saveInfo.GetList(_listName);
                    saveInfo.RemoveList(_listName);
                    foreach (var s in l)
                    {
                        saveInfo.AddListRow(_listName, s);
                    }
                }

                SaveSettingData(saveInfo);
            }

            ClearCache();
            Populate();
            PopulateList();
        }


        public void AddRow()
        {
            AddListRow(_listName);

            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                SaveSettingData(listItem.Value);
            }

            ClearCache();
            Populate();
            PopulateList();
        }

        public void ClearCache()
        {
            CacheUtils.ClearCache("rocketmod" + _moduleid);
        }


        public string ExportData()
        {
            var expInfo = (SimplisityInfo)Info.Clone();
            expInfo.RemoveLangRecord();
            var xmlOut = expInfo.ToXmlItem();

            return "";
        }

        public string ImportData()
        {
            return "";
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

        private SimplisityInfo GetSettingData(string cultureCode)
        {
            var guidkey = "moduleid" + _moduleid;
            if (_moduleid <= 0) guidkey = "parentitemid" + _parentitemid;
            var objCtrl = new DNNrocketController();
            return objCtrl.GetData(guidkey, _entityTypeCode, cultureCode, -1, _moduleid, false);
        }

        private void SaveSettingData(SimplisityInfo sInfo)
        {
            var guidkey = "moduleid" + _moduleid;
            if (_moduleid <= 0) guidkey = "parentitemid" + _parentitemid;
            var objCtrl = new DNNrocketController();
            sInfo.ParentItemId = _parentitemid;
            sInfo.ModuleId = _moduleid;
            objCtrl.SaveData(guidkey, _entityTypeCode, sInfo, -1, _moduleid);
        }

        #endregion


    }

}
