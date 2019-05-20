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
        private string _langRequired;
        private string _themeFolderMapPath;
        private string _themeFolder;
        private string _controlMapPath;
        private Dictionary<string, string> _templates;
        private Dictionary<string, string> _cssPath;
        private Dictionary<string, string> _imgPath;
        private Dictionary<string, string> _resxPath;

        public AppTheme(string appName, string versionFolder)
        {
            InitClass(appName, "/DesktopModules/DNNrocket/AppThemes", "", versionFolder);
        }

        public AppTheme(string appName, string langRequired, string versionFolder)
        {
            InitClass(appName, "/DesktopModules/DNNrocket/AppThemes", langRequired, versionFolder);
        }

        public AppTheme(string appName, string appFolder = "/DesktopModules/DNNrocket/AppThemes", string langRequired = "", string versionFolder = "v1")
        {
            InitClass(appName, appFolder, langRequired, versionFolder);
        }

        private void InitClass(string appName, string appFolder, string langRequired, string versionFolder)
        {
            AppVersionFolder = versionFolder;
            if (langRequired == "") langRequired = DNNrocketUtils.GetCurrentCulture();
            _langRequired = langRequired;
            AppName = appName;
            AppFolder = appFolder;
            _controlMapPath = DNNrocketUtils.MapPath(AppFolder);
            _themeFolderMapPath = _controlMapPath.TrimEnd('\\') + "\\Themes\\" + AppName;
            _themeFolder = AppFolder.TrimEnd('/') + "/Themes/" + AppName;
            Populate();
        }

        public void Populate()
        {
            var themeFolderPath = "Themes\\" + AppName + "\\" + AppVersionFolder;
            var templCtrl = new Simplisity.TemplateEngine.TemplateGetter(DNNrocketUtils.HomeDirectory(), themeFolderPath, _controlMapPath);
            ActiveViewTemplate = templCtrl.GetTemplateData("view.cshtml", _langRequired);
            ActiveEditTemplate = templCtrl.GetTemplateData("edit.cshtml", _langRequired);
            ActivePageHeaderTemplate = templCtrl.GetTemplateData("pageheader.cshtml", _langRequired);
            var logoMapPath = _themeFolderMapPath + "\\Logo.png";
            Logo = _themeFolder + "/Logo.png";
            if (!File.Exists(logoMapPath)) Logo = "";
            var summaryTxt = FileUtils.ReadFile(_themeFolderMapPath + "\\summary-" + _langRequired + ".txt");
            if (summaryTxt != null || summaryTxt == "")
            {
                summaryTxt = FileUtils.ReadFile(_themeFolderMapPath + "\\summary.txt");
            }
            Summary = summaryTxt;
        }

        public void SaveMetaFile()
        {
            var sRec = new SimplisityRecord();
            sRec.SetXmlProperty("genxml/appname", AppName);
            sRec.SetXmlProperty("genxml/summary", Summary);
            sRec.SetXmlProperty("genxml/logo", Logo);
        }


        #region "properties"

        public string AppName { get; private set; }
        public string Summary { get; private set; }
        public string Logo { get; private set; }

        public string AppFolder { get; set; }
        public string AppVersionFolder { get; set; }
        public string ActiveViewTemplate { get; set; }
        public string ActiveEditTemplate { get; set; }
        public string ActivePageHeaderTemplate { get; set; }

        #endregion


    }

}
