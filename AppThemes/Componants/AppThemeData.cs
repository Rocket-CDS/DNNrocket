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
        private List<Object> _versionList;
        private int _userId;
        public SimplisityInfo Info;
        private AppTheme _appTheme;


        public AppThemeData(int userId, string appThemesRelPath, string langRequired)
        {
            CultureCode = langRequired;
            AppCultureCode = CultureCode;
            _userId = userId;
            AppThemesRelPath = appThemesRelPath;
            AppThemesMapPath = DNNrocketUtils.MapPath(AppThemesRelPath);
            AdminAppThemesRelPath = appThemesRelPath + "/Admin";
            AdminAppThemesMapPath = DNNrocketUtils.MapPath(AdminAppThemesRelPath);            

            Populate();
            PopulateList();
            PopulateVersionList();
        }

        public void Populate()
        {
            var objCtrl = new DNNrocketController();
            Info = objCtrl.GetData("apptheme_" + _userId, "APPTHEMECONFIG", CultureCode, -1,-1, true);
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.UserId = _userId;
                Info.SetXmlProperty("genxml/appthemesmappath", AppThemesMapPath);
                Info.SetXmlProperty("genxml/appthemesrelpath", AppThemesRelPath);
            }
            _appTheme = new AppTheme(AppName, CultureCode, VersionFolder);
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

        public void PopulateVersionList()
        {
            if (AppName != "")
            {
                _versionList = new List<object>();
                if (System.IO.Directory.Exists(AppThemesMapPath + "\\Themes\\" + AppName))
                {
                    var dirlist = System.IO.Directory.GetDirectories(AppThemesMapPath + "\\Themes\\" + AppName);
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        _versionList.Add(dr.Name);
                    }
                }
                if (_versionList.Count == 0) _versionList.Add("1.0");
                _versionList.Reverse();
                LatestVersionFolder = (string)_versionList.First();
            }
        }

        public void DeleteConfig()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("apptheme_" + _userId, "APPTHEMECONFIG", CultureCode, -1, -1, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                Populate();
                PopulateList();
                PopulateVersionList();
            }
        }

        public void Save()
        {
            var objCtrl = new DNNrocketController();

            AppName = GeneralUtils.AlphaNumeric(AppName);

            Info = objCtrl.SaveData("apptheme_" + _userId, "APPTHEMECONFIG", Info, -1, -1);
            
            Populate();
            PopulateList();
            PopulateVersionList();

            if (AppName != "")
            {
                // save the theme xml to tthe theme folder
                _appTheme = new AppTheme(AppName, VersionFolder);
                _appTheme.DisplayName = DisplayName;
                _appTheme.Summary = Summary;
                _appTheme.AppCultureCode = AppCultureCode;
                _appTheme.SaveTheme();
            }

        }

        public void DeleteTheme()
        {
            _appTheme.DeleteTheme();
            DeleteConfig();
        }

        public void DeleteVersion()
        {
            _appTheme.DeleteVersion(VersionFolder);
            PopulateVersionList();
            VersionFolder = "v1";
            if (VersionList.Count() > 0)
            {
                VersionFolder = (string) VersionList.First();
            }
            Populate();
            PopulateList();
            PopulateVersionList();
        }

        public void CreateNewVersion(double increment = 1)
        {
            PopulateVersionList();
            if (GeneralUtils.IsNumeric(LatestVersionFolder))
            {
                var currentLatestVersionFolder = LatestVersionFolder;
                LatestVersionFolder = (Convert.ToDouble(LatestVersionFolder) + increment).ToString("0.0");
                _appTheme.CopyVersion(currentLatestVersionFolder, LatestVersionFolder);
                PopulateVersionList();
                VersionFolder = "1.0";
                if (VersionList.Count() > 0)
                {
                    VersionFolder = (string)VersionList.First();
                }
            }
            Populate();
            PopulateList();
        }

        public void SaveToDisk()
        {

            Populate();
            PopulateList();
            PopulateVersionList();
        }
        public void SaveAppTheme()
        {

            Populate();
            PopulateList();
            PopulateVersionList();
        }

        #region "properties"

        public string AppThemesRelPath { get; }
        public string AppThemesMapPath { get; }
        public string AdminAppThemesRelPath { get; }
        public string AdminAppThemesMapPath { get; }

        public string VersionFolder
        {
            get
            {
                var rtnV = Info.GetXmlProperty("genxml/selected/versionfolder");
                if (rtnV == "") rtnV = "1.0";
                return rtnV;
            }
            set
            {
                if (value != "")
                {
                    Info.SetXmlProperty("genxml/selected/versionfolder", value);
                }
            }
        }
        public string ActionType
        {
            get
            {
                return Info.GetXmlProperty("genxml/hidden/ationtype");
            }
            set
            {
                Info.SetXmlProperty("genxml/hidden/ationtype", value);
            }
        }
        public string AppName
        {
            get
            {
                return Info.GetXmlProperty("genxml/textbox/appname");
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/appname", value);
            }
        }
        public string DisplayName {
            get {
                return Info.GetXmlProperty("genxml/lang/genxml/textbox/displayname");
            }
            set {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/displayname", value);
            }
        }
        public string Summary
        {
            get
            {
                return Info.GetXmlProperty("genxml/lang/genxml/textbox/summary");
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/summary", value);
            }
        }
        public string LatestVersionFolder { get; set; }
        public string CultureCode { get; set; }
        public string AppCultureCode { get; set; }
        public List<Object> List
        {
            get { return _dataList; }
        }

        public List<Object> VersionList
        {
            get { return _versionList; }
        }

        #endregion

    }

}
