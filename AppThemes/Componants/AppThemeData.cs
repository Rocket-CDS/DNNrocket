using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
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
        public SimplisityRecord userRecord;
        private AppTheme _appTheme;


        public AppThemeData(int userId, string appThemesRelPath, string langRequired = "")
        {
            if (langRequired == "") langRequired = DNNrocketUtils.GetEditCulture();
            CultureCode = langRequired;
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
            userRecord = objCtrl.GetRecordByGuidKey(-1,-1, "APPTHEMECONFIG", "apptheme_" + _userId, _userId.ToString());
            if (userRecord == null)
            {
                userRecord = new SimplisityRecord();
                userRecord.UserId = _userId;
                userRecord.TypeCode = "APPTHEMECONFIG";
                userRecord.SetXmlProperty("genxml/appthemesmappath", AppThemesMapPath);
                userRecord.SetXmlProperty("genxml/appthemesrelpath", AppThemesRelPath);
                userRecord = objCtrl.SaveRecord("apptheme_" + _userId, "APPTHEMECONFIG", userRecord);
            }
            AppCultureCode = userRecord.GetXmlProperty("genxml/hidden/appculturecode");
            if (AppCultureCode == "") AppCultureCode = CultureCode;  // default to current culture.
            _appTheme = new AppTheme(AppName, AppCultureCode, VersionFolder);
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
            if (userRecord.UserId > 0)
            {
                var objCtrl = new DNNrocketController();
                AppName = GeneralUtils.AlphaNumeric(AppName);
                userRecord = objCtrl.SaveRecord("apptheme_" + _userId, "APPTHEMECONFIG", userRecord);
                _appTheme.SaveTheme();
            }

            Populate();
            PopulateList();
            PopulateVersionList();

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
                _appTheme .SaveTheme();
            }
        }

        public void SaveToDisk()
        {
            if (AppName != "")
            {
                var objCtrl = new DNNrocketController();

                var xmlMeta = FileUtils.ReadFile(AppTheme.AppThemeFolderMapPath + "\\Meta.xml");
                var mInfo = new SimplisityInfo();
                if (xmlMeta != "") mInfo.XMLData = xmlMeta;

                // Update detail fields
                mInfo.SetXmlProperty("genxml/textbox/appname", AppTheme.AppName);
                mInfo.SetXmlProperty("genxml/hidden/logo", AppTheme.Logo);
                mInfo.Lang = AppTheme.AppCultureCode;

                mInfo.SetXmlProperty("genxml/lang-" + AppTheme.AppCultureCode + "/genxml/textbox/displayname", AppTheme.DisplayName);
                mInfo.SetXmlProperty("genxml/lang-" + AppTheme.AppCultureCode + "/genxml/textbox/summary", AppTheme.Summary);

                // Update Fields
                mInfo.SetXmlProperty("genxml/fields","");
                var settingsData = new SettingsData(AppTheme.Info.ItemID, AppTheme.AppCultureCode, "APPTHEMEFIELDS");
                mInfo.AddXmlNode(settingsData.)

                // Update Image

                // Update RESX


                // Save to XML file
                if (!Directory.Exists(AppThemeFolderMapPath)) Directory.CreateDirectory(AppThemeFolderMapPath);
                FileUtils.SaveFile(AppThemeFolderMapPath + "\\Meta.xml", mInfo.XMLData);
            }

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
                var rtnV = userRecord.GetXmlProperty("genxml/select/versionfolder");
                if (rtnV == "") rtnV = "1.0";
                return rtnV;
            }
            set
            {
                if (value != "")
                {
                    userRecord.SetXmlProperty("genxml/select/versionfolder", value);
                }
            }
        }
        public string ActionType
        {
            get
            {
                return userRecord.GetXmlProperty("genxml/hidden/ationtype");
            }
            set
            {
                userRecord.SetXmlProperty("genxml/hidden/ationtype", value);
            }
        }
        public string AppName
        {
            get
            {
                return userRecord.GetXmlProperty("genxml/textbox/appname");
            }
            set
            {
                userRecord.SetXmlProperty("genxml/textbox/appname", value);
            }
        }
        public string AppCultureCode
        {
            get
            {
                return userRecord.GetXmlProperty("genxml/hidden/appculturecode");
            }
            set
            {
                userRecord.SetXmlProperty("genxml/hidden/appculturecode", value);
            }
        }


        public string LatestVersionFolder { get; set; }
        public string CultureCode { get; set; }
        public List<Object> List
        {
            get { return _dataList; }
        }

        public List<Object> VersionList
        {
            get { return _versionList; }
        }

        public AppTheme AppTheme
        {
            get { return _appTheme; }
        }        

        #endregion

    }

}
