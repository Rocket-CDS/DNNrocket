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
        private string _appName;
        private string _appFolder;
        private string _appVersionFolder;
        private Dictionary<string, string> _templates;
        private Dictionary<string, string> _cssPath;
        private Dictionary<string, string> _imgPath;
        private Dictionary<string, string> _resxPath;

        public string ActiveViewTemplate;
        public string ActiveEditTemplate;
        public string ActivePageHeaderTemplate;        

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
            _appVersionFolder = versionFolder;
            if (langRequired == "") langRequired = DNNrocketUtils.GetCurrentCulture();
            _langRequired = langRequired;
            _appName = appName;
            _appFolder = appFolder;
            Populate();
        }

        public void Populate()
        {
            var controlMapPath = DNNrocketUtils.MapPath(_appFolder);
            var themeFolderPath = "Themes\\" + _appName + "\\" + _appVersionFolder;
            var templCtrl = new Simplisity.TemplateEngine.TemplateGetter(DNNrocketUtils.HomeDirectory(), themeFolderPath, controlMapPath);
            ActiveViewTemplate = templCtrl.GetTemplateData("view.cshtml", _langRequired);
            ActiveEditTemplate = templCtrl.GetTemplateData("edit.cshtml", _langRequired);
            ActivePageHeaderTemplate = templCtrl.GetTemplateData("pageheader.cshtml", _langRequired);
        }


        #region "properties"

        public string AppName { get { return _appName; } }

        #endregion


    }

}
