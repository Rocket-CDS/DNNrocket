using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DNNrocketAPI.Componants
{

    public class AppTheme
    {
        private Dictionary<string, string> _templates;
        private Dictionary<string, string> _cssPath;
        private Dictionary<string, string> _imgPath;
        private Dictionary<string, string> _resxPath;

        private SimplisityInfo _fields;

        public AppTheme(string appName, string versionFolder)
        {
            InitClass(appName, "/DesktopModules/DNNrocket/AppThemes", "", versionFolder);
        }

        public AppTheme(string appName, string langRequired, string versionFolder)
        {
            InitClass(appName, "/DesktopModules/DNNrocket/AppThemes", langRequired, versionFolder);
        }

        public AppTheme(string appName, string appFolder = "/DesktopModules/DNNrocket/AppThemes", string langRequired = "", string versionFolder = "1.0")
        {
            InitClass(appName, appFolder, langRequired, versionFolder);
        }

        private void InitClass(string appName, string appRelFolder, string langRequired, string versionFolder)
        {
            AppVersionFolder = versionFolder;
            if (langRequired == "") langRequired = DNNrocketUtils.GetCurrentCulture();
            AppName = appName;
            AppFolder = appRelFolder;
            AppThemeFolder = appRelFolder + "/Themes/" + appName;
            AppCultureCode = langRequired;
            AppFolderMapPath = DNNrocketUtils.MapPath(AppFolder);
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolder);
            AppThemeVersionFolder = AppThemeFolder + "/" + AppVersionFolder;
            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolder);
            if (AppName != "") Populate();
        }

        public void Populate()
        {
            var themeFolderPath = "Themes\\" + AppName + "\\" + AppVersionFolder;
            var templCtrl = new Simplisity.TemplateEngine.TemplateGetter(DNNrocketUtils.HomeDirectory(), themeFolderPath, AppFolderMapPath);
            ActiveViewTemplate = templCtrl.GetTemplateData("view.cshtml", AppCultureCode);
            ActiveEditTemplate = templCtrl.GetTemplateData("edit.cshtml", AppCultureCode);
            ActivePageHeaderTemplate = templCtrl.GetTemplateData("pageheader.cshtml", AppCultureCode);
            var logoMapPath = AppThemeFolderMapPath + "\\Logo.png";
            Logo = AppThemeFolder + "/Logo.png";
            if (!File.Exists(logoMapPath)) Logo = "";

            var xmlMeta = FileUtils.ReadFile(AppThemeFolderMapPath + "\\Meta.xml");            
            Info = new SimplisityInfo(AppCultureCode);
            Info.XMLData = xmlMeta;

        }

        public List<SimplisityInfo> ListFields()
        {
            return Info.GetList("settingsdata");
        }


        public void SaveTheme()
        {
            if (AppName != "")
            {
                var xmlMeta = FileUtils.ReadFile(AppThemeFolderMapPath + "\\Meta.xml");
                var mInfo = new SimplisityInfo();
                if (xmlMeta != "") mInfo.XMLData = xmlMeta;

                // Update detail fields
                mInfo.SetXmlProperty("genxml/textbox/appname", AppName);
                mInfo.SetXmlProperty("genxml/hidden/logo", Logo);
                mInfo.Lang = AppCultureCode;

                mInfo.SetXmlProperty("genxml/lang-" + AppCultureCode + "/genxml/textbox/displayname", DisplayName);
                mInfo.SetXmlProperty("genxml/lang-" + AppCultureCode + "/genxml/textbox/summary", Summary);

                // Save to XML file
                if (!Directory.Exists(AppThemeFolderMapPath)) Directory.CreateDirectory(AppThemeFolderMapPath);
                FileUtils.SaveFile(AppThemeFolderMapPath + "\\Meta.xml", mInfo.XMLData);
            }
        }

        public void DeleteTheme()
        {
            if (Directory.Exists(AppThemeFolderMapPath))
            {
                Directory.Delete(AppThemeFolderMapPath, true);
            }
        }
        public void DeleteVersion(string versionFolder)
        {
            if (Directory.Exists(AppThemeFolderMapPath + "\\" + versionFolder))
            {
                Directory.Delete(AppThemeFolderMapPath + "\\" + versionFolder, true);
            }
        }

        public void CreateVersion(string versionFolder)
        {
            if (!Directory.Exists(AppThemeFolderMapPath + "\\" + versionFolder))
            {
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder);
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\css");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\default");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\resx");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\js");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\img");
            }
        }

        public void CopyVersion(string sourceVersionFolder, string destVersionFolder)
        {
            sourceVersionFolder = AppThemeFolderMapPath + "\\" + sourceVersionFolder;
            destVersionFolder = AppThemeFolderMapPath + "\\" + destVersionFolder;

            if (Directory.Exists(sourceVersionFolder))
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourceVersionFolder, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceVersionFolder, destVersionFolder));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourceVersionFolder, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourceVersionFolder, destVersionFolder), true);
            }
        }


        #region "properties"

        public string AppName { get; private set; }
        public SimplisityInfo Info { get; private set; }
        public string Logo { get; private set; }
        public string AppCultureCode { get; set; }
        public string AppFolder { get; set; }
        public string AppFolderMapPath { get; set; }
        public string AppThemeFolder { get; set; }
        public string AppThemeFolderMapPath { get; set; }
        public string AppVersionFolder { get; set; }
        public string AppThemeVersionFolder { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
        public string ActiveViewTemplate { get; set; }
        public string ActiveEditTemplate { get; set; }
        public string ActivePageHeaderTemplate { get; set; }


        public string DisplayName
        {
            get
            {
                var appCultureCode = AppCultureCode;
                if (appCultureCode == "") appCultureCode = "en-US";
                return Info.GetXmlProperty("genxml/lang-" + appCultureCode + "/genxml/textbox/displayname");
            }
            set
            {
                Info.SetXmlProperty("genxml/lang-" + AppCultureCode + "/genxml/textbox/displayname", value);
            }
        }
        public string Summary
        {
            get
            {
                var appCultureCode = AppCultureCode;
                if (appCultureCode == "") appCultureCode = "en-US";
                return Info.GetXmlProperty("genxml/lang-" + appCultureCode + "/genxml/textbox/summary");
            }
            set
            {
                Info.SetXmlProperty("genxml/lang-" + AppCultureCode + "/genxml/textbox/summary", value);
            }
        }

        public SimplisityInfo Fields { get; set; }
        public SimplisityInfo Images { get; set; }
        public SimplisityInfo Resx { get; set; }
        public Dictionary<string,string> ResxDict {
            get {
                var rtnDict = new Dictionary<string, string>();
                foreach (var r in Resx.GetList("settingsdata"))
                {
                    rtnDict.Add(r.GetXmlProperty("genxml/textbox/name"), r.GetXmlProperty("genxml/textbox/value"));
                }
                return rtnDict;
            }
        }

        #endregion


    }

}
