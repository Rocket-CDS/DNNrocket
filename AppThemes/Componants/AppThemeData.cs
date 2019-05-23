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
            PopulateVersionList();
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

        public void PopulateVersionList()
        {
            var appThemeName = Name;
            if (appThemeName == "") appThemeName = SelectedTheme;
            if (appThemeName != "")
            {
                _versionList = new List<Object>();
                if (System.IO.Directory.Exists(AppThemesMapPath + "\\Themes\\" + appThemeName))
                {
                    var dirlist = System.IO.Directory.GetDirectories(AppThemesMapPath + "\\Themes\\" + appThemeName);
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        _versionList.Add(dr.Name);
                    }
                }
                if (_versionList.Count == 0) _versionList.Add("v1");
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
                PopulateVersionList();
            }
        }

        public void Save()
        {
            var objCtrl = new DNNrocketController();

            if (Name == "" && ActionType != "new") Name = SelectedTheme;
            Name = GeneralUtils.AlphaNumeric(Name);

            var info = objCtrl.SaveData("apptheme_" + _userId, "APPTHEMECONFIG", Info, -1, -1);
            Populate();
            PopulateList();
        }

        #region "properties"

        public string AppThemesRelPath { get; }
        public string AppThemesMapPath { get; }
        public string AdminAppThemesRelPath { get; }
        public string AdminAppThemesMapPath { get; }

        public string SelectedTheme
        {
            get
            {
                return Info.GetXmlProperty("genxml/hidden/selectedtheme");
            }
            set
            {
                Info.SetXmlProperty("genxml/hidden/selectedtheme", value);
            }
        }
        public string SelectedVersion
        {
            get
            {
                return Info.GetXmlProperty("genxml/hidden/selectedversion");
            }
            set
            {
                Info.SetXmlProperty("genxml/hidden/selectedversion", value);
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
        public string Name
        {
            get
            {
                return Info.GetXmlProperty("genxml/textbox/name");
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/name", value);
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
        public string CultureCode
        {
            get
            {
                return Info.Lang;
            }
            set
            {
                Info.Lang = value;
            }
        }

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
