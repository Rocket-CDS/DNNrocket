using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketSettings
{

    public class SettingsData
    {
        private List<SimplisityInfo> _dataList;
        private int _tabid;
        private int _moduleid;
        private string _langRequired;

        public SimplisityInfo Info;


        public SettingsData(int tabId, int moduleId, string langRequired)
        {
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
            Info = objCtrl.GetData("rocketsettings_" + _moduleid, "ROCKETSETTINGS", _langRequired, -1, _moduleid, true);
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ModuleId = _moduleid;
            }
        }

        public void PopulateList()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("rocketsettings_" + _moduleid, "ROCKETSETTINGS", _langRequired, -1, _moduleid, true);
            if (info != null)
            {
                _dataList = info.GetList("rocketsettings");
            }
        }

        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("rocketsettings_" + _moduleid, "ROCKETSETTINGS", DNNrocketUtils.GetEditCulture(), -1, _moduleid, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                ClearCache();
                Populate();
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData("rocketsettings_" + _moduleid, "ROCKETSETTINGS", postInfo, -1, _moduleid);
            ClearCache();
            Populate();
        }


        public void Add()
        {
            Info.AddListRow("rocketsettings");
            Save(Info);
        }



        #region "properties"

        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
      
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
