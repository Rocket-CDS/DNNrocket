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
        private SimplisityInfo _currentAppTheme; 

        private int _userId;

        public AppThemeData(int userId, string appThemesRelPath, string langRequired = "")
        {
            _currentAppTheme = new SimplisityInfo();

            if (langRequired == "") langRequired = DNNrocketUtils.GetEditCulture();
            CultureCode = langRequired;
            _userId = userId;
            AppThemesRelPath = appThemesRelPath;
            AppThemesMapPath = DNNrocketUtils.MapPath(AppThemesRelPath);
            AdminAppThemesRelPath = appThemesRelPath + "/Admin";
            AdminAppThemesMapPath = DNNrocketUtils.MapPath(AdminAppThemesRelPath);

            AssignFolders();

            PopulateAppThemeList();  // we have no AppName selected, just get appTheme List.
        }

        private void AssignFolders()
        {
            PopulateVersionList();
            AppThemeFolder = AppThemesRelPath + "/Themes/" + AppName;
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolder);
            AppThemeVersionFolder = AppThemeFolder + "/" + VersionFolder;
            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolder);
        }


        public void Populate()
        {            
            PopulateAppThemeList();
            PopulateVersionList();
            PopulateDataList(); //Can only be populated when we have the AppName and version.
        }

        public void PopulateDataList()
        {
            AssignFolders();

            _dataList = new SimplisityData();

            var xmlIn = FileUtils.ReadFile(AppThemeVersionFolderMapPath + "\\meta.xml");
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
                    if (sInfo2.Lang == CultureCode && AppName != "")
                    {
                        // App Theme needs to have been selected.
                        var v = VersionFolder;
                        var n = AppName;
                        _currentAppTheme = sInfo2;
                        VersionFolder = v;
                        AppName = n;
                    }
                }
            }
            if (_dataList.GetInfo(CultureCode) == null )
            {
                _dataList.AddSimplisityInfo(new SimplisityInfo(), CultureCode);
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
            _dataList.AddListItem("fielddata");
        }

        public void AddResxRow()
        {
            _dataList.AddListItem("resxdata");
        }

        public void AddDataInfo(SimplisityInfo sInfo, string cultureCode)
        {
            //remove any params
            sInfo.RemoveXmlNode("genxml/postform");
            sInfo.RemoveXmlNode("genxml/urlparams");

            _dataList.AddSimplisityInfo(sInfo, cultureCode);

            var v = VersionFolder;
            var n = AppName;
            _currentAppTheme.XMLData = sInfo.XMLData;
            VersionFolder = v;
            AppName = n;

            AssignFolders();

        }

        public void Save()
        {
            AssignFolders();

            var xmlOut = "<genxml>";
            xmlOut += "<data>";

            foreach (var sDic in _dataList.SimplisityInfoList )
            {
                xmlOut += sDic.Value.ToXmlItem();
            }

            xmlOut += "</data>";
            xmlOut += "</genxml>";

            FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\meta.xml", xmlOut);

            Populate();
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
                if (VersionFolder == "") VersionFolder = LatestVersionFolder;
            }
        }

        public void DeleteTheme()
        {
            var appTheme = new AppTheme(AppName, CultureCode, VersionFolder);
            appTheme.DeleteTheme();
        }

        public void DeleteVersion()
        {
            var appTheme = new AppTheme(AppName,CultureCode,VersionFolder);
            appTheme.DeleteVersion(VersionFolder);
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
                var appTheme = new AppTheme(AppName, CultureCode, VersionFolder);
                appTheme.CopyVersion(currentLatestVersionFolder, LatestVersionFolder);
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
            if (info != null) return info.GetList("fielddata");
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

        public string AppThemesRelPath { get; set; }
        public string AppThemesMapPath { get; set; }
        public string AdminAppThemesRelPath { get; set; }
        public string AdminAppThemesMapPath { get; set; }
        public string AppThemeFolder { get; set; }
        public string AppThemeFolderMapPath { get; set; }
        public string AppThemeVersionFolder { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
        public string ActionType { get; set; }
        public string VersionFolder
        {
            get
            {
                return _currentAppTheme.GetXmlProperty("genxml/select/versionfolder");
            }
            set
            {
                _currentAppTheme.SetXmlProperty("genxml/select/versionfolder", value);
            }
        }
        public string AppName
        {
            get
            {
                return _currentAppTheme.GetXmlProperty("genxml/textbox/appname");
            }
            set
            {
                _currentAppTheme.SetXmlProperty("genxml/textbox/appname", value);
            }
        }
        public string LatestVersionFolder { get; set; }

        public SimplisityInfo CurrentInfo
        {
            get
            {
                return _currentAppTheme;
            }
        }

        public string CultureCode
        {
            get
            {
                return _currentAppTheme.Lang;
            }
            set
            {
                _currentAppTheme.Lang = value;
            }
        }
        public List<Object> AppThemesList
        {
            get { return _appThemesList; }
        }

        public List<Object> VersionList
        {
            get { return _versionList; }
        }

        #endregion

    }

}
