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
        private SimplisityData _dataList;
        private List<Object> _appThemesList;
        private List<Object> _versionList;

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

            PopulateDataList();
            PopulateAppThemeList();
            PopulateVersionList();

        }

        public void PopulateDataList()
        {
            _dataList = new SimplisityData();

            var xmlIn = FileUtils.ReadFile(_appTheme.AppThemeVersionFolderMapPath + "\\meta.xml");
            var sInfo = new SimplisityInfo();
            sInfo.XMLData = xmlIn;
            if (sInfo.XMLDoc != null)
            {
                var nodList = sInfo.XMLDoc.SelectNodes("genxml/data/*");
                foreach (XmlNode nod in nodList)
                {
                    var sInfo2 = new SimplisityInfo();
                    sInfo2.FromXmlItem(nod.OuterXml);
                    _dataList.AddSimplisityInfo(sInfo2, sInfo2.Lang);
                }
            }
            if (_dataList.GetInfo(AppCultureCode) == null )
            {
                _dataList.AddSimplisityInfo(new SimplisityInfo(), AppCultureCode);
            }
        }

        public void PopulateAppThemeList()
        {
            _appThemesList = new List<Object>();

            var dirlist = System.IO.Directory.GetDirectories(AppThemesMapPath + "\\Themes");
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var appTheme = new AppTheme(dr.Name);
                _appThemesList.Add(appTheme);
            }

        }

        public void AddFieldRow()
        {
            _dataList.AddListRow("fields");
        }

        public void AddResxRow()
        {
            _dataList.AddListRow("resx");
        }

        public void AddDataInfo(SimplisityInfo sInfo, string cultureCode)
        {
            //remove any params
            sInfo.RemoveXmlNode("genxml/postform");
            sInfo.RemoveXmlNode("genxml/urlparams");

            _dataList.AddSimplisityInfo(sInfo, cultureCode);
        }

        public void Save()
        {
            var xmlOut = "<genxml>";
            xmlOut += "<data>";

            foreach (var sDic in _dataList.SimplisityInfoList )
            {
                xmlOut += sDic.Value.ToXmlItem();
            }

            xmlOut += "</data>";
            xmlOut += "</genxml>";

            FileUtils.SaveFile(_appTheme.AppThemeVersionFolderMapPath + "\\meta.xml", xmlOut);
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

        public List<SimplisityInfo> GetFields(string cultureCode)
        {
            var info = _dataList.GetInfo(cultureCode);
            if (info != null) return info.GetList("fields");
            return new List<SimplisityInfo>();
        }
        public List<SimplisityInfo> GetResxList(string cultureCode)
        {
            var info = _dataList.GetInfo(cultureCode);
            if (info != null) return info.GetList("resx");
            return new List<SimplisityInfo>();
        }
        public List<SimplisityInfo> GetImageList(string cultureCode)
        {
            var info = _dataList.GetInfo(cultureCode);
            if (info != null) return info.GetList("images");
            return new List<SimplisityInfo>();
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
        public List<Object> AppThemesList
        {
            get { return _appThemesList; }
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
