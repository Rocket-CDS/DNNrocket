using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocket.AppThemes
{

    public class AppTheme
    {
        private string _langRequired;
        private string _appName;
        private string _appFolder;
        private Dictionary<string, string> _templates;
        private Dictionary<string, string> _cssPath;
        private Dictionary<string, string> _imgPath;
        private Dictionary<string, string> _resxPath;


        public string ActiveViewTemplate;
        public string ActiveEditTemplate;


        public AppTheme(string appName, string appFolder = "/DesktopModules/DNNrocket/AppThemes", string langRequired = "")
        {
            if (langRequired == "") langRequired = DNNrocketUtils.GetCurrentCulture();
            _langRequired = langRequired;
            _appName = appName;
            _appFolder = appFolder;
            Populate();
        }

        public void Populate()
        {
            var controlMapPath = DNNrocketUtils.MapPath(_appFolder);
            var templCtrl = new Simplisity.TemplateEngine.TemplateGetter(DNNrocketUtils.HomeDirectory(), "Themes\\" + _appName, controlMapPath);
            ActiveViewTemplate = templCtrl.GetTemplateData("view.cshtml", _langRequired);
            ActiveEditTemplate = templCtrl.GetTemplateData("edit.cshtml", _langRequired);

        }


        #region "properties"

        public string AppName { get { return _appName; } }

        #endregion


    }

}
