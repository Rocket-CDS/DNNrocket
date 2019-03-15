using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocket.AppThemes
{

    public class AppThemeData
    {
        private List<SimplisityInfo> _dataList;
        private int _tabid;
        private int _moduleid;
        private string _langRequired;

        public SimplisityInfo Info;


        public AppThemeData(int tabId, int moduleId, string langRequired)
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
            Info = objCtrl.GetData("rocketsettings_" + _moduleid, "APPTHEME", _langRequired, -1, _moduleid, true);
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
            var info = objCtrl.GetData("rocketsettings_" + _moduleid, "APPTHEME", _langRequired, -1, _moduleid, true);
            if (info != null)
            {
                _dataList = info.GetList("settingsdata");
            }
        }

        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("rocketsettings_" + _moduleid, "APPTHEME", DNNrocketUtils.GetEditCulture(), -1, _moduleid, true);
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
            var info = objCtrl.SaveData("rocketsettings_" + _moduleid, "APPTHEME", postInfo, -1, _moduleid);
            ClearCache();
            Populate();
            PopulateList();
        }


        public void AddRow()
        {
            Info.AddListRow("settingsdata");
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
            CacheUtils.ClearCache("apptheme" + _moduleid);
        }




    }

}
