using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocket.AppThemes
{

    public class AppThemeData
    {
        private List<Object> _dataList;
        private string _langRequired;
        private int _userId;
        public SimplisityInfo Info;


        public AppThemeData(int userId, string appThemesRelPath, string langRequired)
        {
            _langRequired = langRequired;
            _userId = userId;
            AppThemesRelPath = appThemesRelPath;
            AppThemesMapPath = DNNrocketUtils.MapPath(AppThemesRelPath);
            AdminAppThemesRelPath = appThemesRelPath + "/Admin";
            AdminAppThemesMapPath = DNNrocketUtils.MapPath(AdminAppThemesRelPath);            

            Populate();
            PopulateList();
        }

        public void Populate()
        {
            var objCtrl = new DNNrocketController();
            Info = objCtrl.GetData("apptheme_" + _userId, "APPTHEMECONFIG", _langRequired, -1,-1, true);
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.UserId = _userId;
                Info.SetXmlProperty("genxml/appthemesmappath", AppThemesMapPath);
                Info.SetXmlProperty("genxml/appthemesrelpath", AppThemesRelPath);
            }

        }

        public void PopulateList()
        {
            _dataList = new List<Object>();

            var dirlist = System.IO.Directory.GetDirectories(AppThemesMapPath + "\\Themes");
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var appTheme = new AppTheme(dr.Name);
                _dataList.Add(appTheme);
            }


        }

        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("apptheme_" + _userId, "APPTHEMECONFIG", _langRequired, -1, -1, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                Populate();
                PopulateList();
            }
        }

        public void Save()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData("apptheme_" + _userId, "APPTHEMECONFIG", Info, -1, -1);
            Populate();
            PopulateList();
        }


        #region "properties"

        public string AppThemesRelPath { get; }
        public string AppThemesMapPath { get; }
        public string AdminAppThemesRelPath { get; }
        public string AdminAppThemesMapPath { get; }

        public List<Object> List
        {
            get { return _dataList; }
        }

        #endregion

    }

}
