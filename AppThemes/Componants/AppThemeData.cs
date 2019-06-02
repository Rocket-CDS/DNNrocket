using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DNNrocket.AppThemes
{

    public class AppThemeData
    {
        private List<Object> _dataList;
        private List<Object> _versionList;
        private List<Object> _fields;
        private List<Object> _resxList;
        
        private int _userId;
        public SimplisityInfo configInfo;
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

            configInfo = new SimplisityInfo();

            Populate();
        }

        public void Populate()
        {
              
            if (AppCultureCode == "") AppCultureCode = CultureCode;  
            _appTheme = new AppTheme(AppName, AppCultureCode, VersionFolder);

            PopulateFields();
            PopulateResx();
            PopulateAppThemeList();
            PopulateVersionList();

        }

        public void PopulateFields()
        {
            _fields = new List<Object>();
        }
        public void PopulateResx()
        {
            _resxList = new List<Object>();
        }

        public void PopulateAppThemeList()
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

        public void DeleteTheme()
        {
            _appTheme.DeleteTheme();
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
                var rtnV = configInfo.GetXmlProperty("genxml/select/versionfolder");
                if (rtnV == "") rtnV = "1.0";
                return rtnV;
            }
            set
            {
                if (value != "")
                {
                    configInfo.SetXmlProperty("genxml/select/versionfolder", value);
                }
            }
        }
        public string ActionType
        {
            get
            {
                return configInfo.GetXmlProperty("genxml/hidden/ationtype");
            }
            set
            {
                configInfo.SetXmlProperty("genxml/hidden/ationtype", value);
            }
        }
        public string AppName
        {
            get
            {
                return configInfo.GetXmlProperty("genxml/textbox/appname");
            }
            set
            {
                configInfo.SetXmlProperty("genxml/textbox/appname", value);
            }
        }
        public string AppCultureCode
        {
            get
            {
                return configInfo.GetXmlProperty("genxml/hidden/appculturecode");
            }
            set
            {
                configInfo.SetXmlProperty("genxml/hidden/appculturecode", value);
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
        public List<object> Fields
        {
            get { return _fields; }
        }

        public List<object> ResxList
        {
            get { return _resxList; }
        }        

        #endregion

    }

}
