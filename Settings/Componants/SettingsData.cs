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
        private string _langRequired;
        private string _entityTypeCode;

        public SimplisityInfo Info;


        public SettingsData(int tabId, int moduleId, string langRequired, string entityTypeCode = "ROCKETSETTINGS")
        {
            _entityTypeCode = entityTypeCode;
            _langRequired = langRequired;
            _tabid = tabId;
            _moduleid = moduleId;
            _dataList = new List<SimplisityInfo>();

            Populate();
            PopulateList();
        }

        public void Populate()
        {
            var objCtrl = new DNNrocketController();

            // Load ALL langauge records, so we can update lists correctly
            var langList = DNNrocketUtils.GetCultureCodeList();
            foreach (var cultureCode in langList)
            {
                var s = objCtrl.GetData("moduleid" + _moduleid, _entityTypeCode, cultureCode, -1, _moduleid, false);
                if (s != null)
                {
                    AddSimplisityInfo(s, cultureCode);
                    if (_langRequired == cultureCode)
                    {
                        Info = s;
                    }
                }
            }
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ModuleId = _moduleid;
            }

        }

        public void PopulateList()
        {
            _dataList = new List<SimplisityInfo>();
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("moduleid" + _moduleid, _entityTypeCode, _langRequired, -1, _moduleid, true);
            if (info != null)
            {
                _dataList = info.GetList("settingsdata");
            }
        }

        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("moduleid" + _moduleid, _entityTypeCode, DNNrocketUtils.GetEditCulture(), -1, _moduleid, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                ClearCache();
                Populate();
                PopulateList();
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");


            // get current data
            var dbInfo = objCtrl.GetData("moduleid" + _moduleid, _entityTypeCode, DNNrocketUtils.GetEditCulture(), -1, _moduleid, true);

            // check against new data and find removed list items.
            var removeList = new Dictionary<string, string>();
            var keepList = new List<string>();
            var listNames = dbInfo.GetLists();
            foreach (var listName in listNames)
            {
                var list = dbInfo.GetList(listName);
                foreach (var s in list)
                {
                    var keyref = s.GetXmlProperty("genxml/recordkey");
                    if (postInfo.GetListItem(listName, "/genxml/recordkey", keyref) == null)
                    {
                        if (!removeList.ContainsKey(keyref))
                        {
                            removeList.Add(keyref, listName);
                        }
                    }
                }
            }

            foreach (var r in removeList)
            {
                // delete removed list items from all langauges
                RemoveListRowByKey(r.Value, r.Key);
            }


            foreach (var listName in listNames)
            {
                SortListByCultureCode(listName, postInfo.Lang);
            }

            // Update ALL langauge records.
            foreach (var listItem in SimplisityInfoList)
            {
                var saveInfo = (SimplisityInfo)postInfo.Clone();
                if (saveInfo.Lang != listItem.Value.Lang)
                {
                    // If it's not the same langauge, update the data with the listItem.
                    saveInfo.RemoveLangRecord();
                    saveInfo.SetLangRecord(listItem.Value.GetLangRecord());
                }
                objCtrl.SaveData("moduleid" + _moduleid, _entityTypeCode, saveInfo, -1, _moduleid);
            }

            ClearCache();
            Populate();
            PopulateList();
        }


        public void AddRow()
        {
            AddListRow("settingsdata");

            // Update ALL langauge records.
            var objCtrl = new DNNrocketController();
            foreach (var listItem in SimplisityInfoList)
            {
                objCtrl.SaveData("moduleid" + _moduleid, _entityTypeCode, listItem.Value, -1, _moduleid);
            }

            ClearCache();
            Populate();
            PopulateList();
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


        public void ClearCache()
        {
            CacheUtils.ClearCache("rocketmod" + _moduleid);
        }




    }

}
