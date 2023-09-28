using DNNrocketAPI;
using DNNrocketAPI.Components;
using RocketPortal.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Rocket.AppThemes.Components
{
    public class AppThemesDOL
    {
        private Dictionary<string, object> _dataObjects;
        private Dictionary<string, string> _passSettings;
        private string _moduleRef;
        public AppThemesDOL(int portalid, SessionParams sessionParams)
        {
            Populate(portalid, sessionParams);
        }
        public void Populate(int portalid, SessionParams sessionParams)
        {
            _passSettings = new Dictionary<string, string>();
            _dataObjects = new Dictionary<string, object>();

            var appThemeProjectData = new AppThemeProjectLimpet();
            _moduleRef = sessionParams.Get("moduleref");            
            var appThemeFolder = sessionParams.Get("appthemefolder");
            var appVersionFolder = sessionParams.Get("appversionfolder");          
            var projectName = sessionParams.Get("selectedproject");
            if (projectName == "") projectName = appThemeProjectData.DefaultProjectName();

            SetDataObject("apptheme", new AppThemeLimpet(portalid, appThemeFolder, appVersionFolder, projectName));
            SetDataObject("appthemesystem", new AppThemeDNNrocketLimpet(SystemKey));
            SetDataObject("systemdata", SystemSingleton.Instance(SystemKey));

            SetDataObject("portaldata", new PortalLimpet(portalid));
            SetDataObject("appthemeprojects", appThemeProjectData);

        }
        public void SetDataObject(String key, object value)
        {
            if (_dataObjects.ContainsKey(key)) _dataObjects.Remove(key);
            _dataObjects.Add(key, value);
        }
        public object GetDataObject(String key)
        {
            if (_dataObjects != null && _dataObjects.ContainsKey(key)) return _dataObjects[key];
            return null;
        }
        public void SetSetting(string key, string value)
        {
            if (_passSettings.ContainsKey(key)) _passSettings.Remove(key);
            _passSettings.Add(key, value);
        }
        public string GetSetting(string key)
        {
            if (!_passSettings.ContainsKey(key)) return "";
            return _passSettings[key];
        }
        public List<SimplisityRecord> GetAppThemeProjects()
        {
            return AppThemeProjects.List;
        }
        public string SystemKey { get { return "rocketapptheme"; } }
        public int PortalId { get { return PortalData.PortalId; } }
        public Dictionary<string, object> DataObjects { get { return _dataObjects; } }
        public AppThemeDNNrocketLimpet AppThemeSystem { get { return (AppThemeDNNrocketLimpet)GetDataObject("appthemesystem"); } }
        public AppThemeLimpet AppTheme { get { return (AppThemeLimpet)GetDataObject("apptheme"); } set { SetDataObject("apptheme", value); } }
        public PortalLimpet PortalData { get { return (PortalLimpet)GetDataObject("portaldata"); } }
        public SystemLimpet SystemData { get { return (SystemLimpet)GetDataObject("systemdata"); } }
        public AppThemeProjectLimpet AppThemeProjects { get { return (AppThemeProjectLimpet)GetDataObject("appthemeprojects"); } }
        public Dictionary<string, string> Settings { get { return _passSettings; } }
        public string ModuleRef { get { return _moduleRef; }  }

    }
}
