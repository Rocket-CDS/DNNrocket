using DNNrocketAPI;
using DotNetNuke.Entities.Modules;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocketAPI.Componants
{
    /// <summary>
    /// </summary>
    public class RemoteData
    {

        private SimplisityRecord _remoteModuleRec;
        private string _cacheKey;
        private int _moduleid;

        public RemoteData(int moduleId, bool useCache = true)
        {
            _remoteModuleRec = new SimplisityRecord();
            _moduleid = moduleId;
            _cacheKey = "moduleparams*" + moduleId;
            _remoteModuleRec = (SimplisityRecord)CacheUtilsDNN.GetCache(_cacheKey);
            var objCtrl = new DNNrocketController();
            if ((_remoteModuleRec == null || !useCache))
            {
                if (moduleId <= 0)
                {
                    _remoteModuleRec = new SimplisityRecord();
                    Exists = false;
                }
                else
                {
                    _remoteModuleRec = objCtrl.GetRecordByGuidKey(-1, _moduleid, "REMOTEMODULE", _cacheKey);
                    if (_remoteModuleRec == null)
                    {
                        _remoteModuleRec = new SimplisityRecord();
                        _remoteModuleRec.PortalId = -1;
                        _remoteModuleRec.TypeCode = "MODULEPARAMS";
                        _remoteModuleRec.GUIDKey = _cacheKey;
                        _remoteModuleRec.ModuleId = _moduleid;
                        _remoteModuleRec = objCtrl.SaveRecord(_remoteModuleRec);
                        Exists = false;
                    }
                    CacheUtilsDNN.SetCache(_cacheKey, _remoteModuleRec);
                }
            }
            DataSourceExternal = false;
            if (systemKey != "") SystemKey = systemKey;
            if (ModuleIdDataSource != _moduleid) DataSourceExternal = true;
            if (ModuleRef == "") ModuleRef = GeneralUtils.GetUniqueString(3);
            if (ModuleIdDataSource <= 0) ModuleIdDataSource = _moduleid;
            if (ModuleRefDataSource == "") ModuleRefDataSource = ModuleRef;

            // Get module settings
            ModuleSettings = new Dictionary<string, string>();
            var moduleSettings = objCtrl.GetByGuidKey(-1, moduleId, ModuleType.ToUpper() +  "SETTINGS", "moduleid" + _moduleid, "",_tableName);
            if (moduleSettings != null)
            {
                moduleSettings = objCtrl.GetInfo(moduleSettings.ItemID, DNNrocketUtils.GetCurrentCulture());
                if (moduleSettings != null)
                {
                    var settingsDict = moduleSettings.ToDictionary();
                    var settingList = moduleSettings.GetList(ModuleType.ToLower() + "settings");
                    if (settingList != null)
                    {
                        foreach (var s in settingList)
                        {
                            var settingvalue = s.GetXmlProperty("genxml/textbox/value");
                            if (s.GetXmlPropertyBool("genxml/checkbox/localized")) settingvalue = s.GetXmlProperty("genxml/lang/genxml/textbox/valuelang");
                            if (!settingsDict.ContainsKey(s.GetXmlProperty("genxml/textbox/name"))) settingsDict.Add(s.GetXmlProperty("genxml/textbox/name"), settingvalue);
                        }
                        ModuleSettings = settingsDict;
                    }
                }
            }

        }
        public void Save()
        {
            var objCtrl = new DNNrocketController();

            DataSourceExternal = false;
            if (ModuleIdDataSource != _moduleid) DataSourceExternal = true;

            Exists = true;

            _remoteModuleRec = objCtrl.SaveRecord(_cacheKey, "REMOTEMODULE", _remoteModuleRec, _moduleid);
            CacheUtilsDNN.SetCache(_cacheKey, _remoteModuleRec);
        }
        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(Record.ItemID);
            Exists = false;
            CacheUtilsDNN.RemoveCache(_cacheKey);
        }
        public string GetValue(string key, string defaultValue = "")
        {
            var rtn = _remoteModuleRec.GetXmlProperty("genxml/hidden/" + key.ToLower());
            if (rtn == "") return defaultValue;
            return rtn;
        }
        public int GetValueInt(string key)
        {
            return _remoteModuleRec.GetXmlPropertyInt("genxml/hidden/" + key.ToLower());
        }
        public double GetValueDouble(string key)
        {
            return _remoteModuleRec.GetXmlPropertyDouble("genxml/hidden/" + key.ToLower());
        }
        public bool GetValueBool(string key)
        {
            if (_remoteModuleRec == null) return false;
            return _remoteModuleRec.GetXmlPropertyBool("genxml/hidden/" + key.ToLower());
        }
        public void SetValue(string key, string value)
        {
            _remoteModuleRec.SetXmlProperty("genxml/hidden/" + key.ToLower(), value);
        }

        public SimplisityRecord Record { get { return _remoteModuleRec; } }


        public string ProviderAssembly { get { return _remoteModuleRec.GetXmlProperty("genxml/textbox/assembly"); } }
        public string ProviderClass { get { return _remoteModuleRec.GetXmlProperty("genxml/textbox/namespaceclass"); } }
        public string DetailUrlParam { get { return GetValue("DetailUrlParam"); } set { SetValue("DetailUrlParam", value); } }
        public bool DetailView { get { return GetValueBool("detailview"); } set { SetValue("detailview", value.ToString()); } }
        public string AppThemeNotes { get { return GetValue("AppThemeNotes"); } set { SetValue("AppThemeNotes", value); } }
        public string AppThemeLogo { get { return GetValue("AppThemeLogo"); } set { SetValue("AppThemeLogo", value); } }
        public string AppThemeFolder { get { return GetValue("AppThemeFolder"); } set { SetValue("AppThemeFolder", value); } } 
        public string AppThemeFolderRel { get { return GetValue("AppThemeFolderRel"); } set { SetValue("AppThemeFolderRel", value); } }
        public string AppThemeFolderMapPath { get { return DNNrocketUtils.MapPath(AppThemeFolderRel); } } 
        public string AppThemeVersionFolderRel { get { return GetValue("AppThemeVersionFolderRel"); } set { SetValue("AppThemeVersionFolderRel", value); } } 
        public string AppThemeVersionFolderMapPath { get { return DNNrocketUtils.MapPath(AppThemeVersionFolderRel); } }
        public string AppProjectFolderRel { get { return GetValue("AppProjectFolderRel"); } set { SetValue("AppProjectFolderRel", value); } }
        public string AppSystemThemeFolderRel { get { return GetValue("AppSystemThemeFolderRel"); } set { SetValue("AppSystemThemeFolderRel", value); } }
        public string AppSystemThemeFolderMapPath { get { return DNNrocketUtils.MapPath(AppSystemThemeFolderRel); } }
        public string AppThemeVersion { get { return GetValue("AppThemeVersion"); } set { SetValue("AppThemeVersion", value); } }
        public int LatestRev { get { return GetValueInt("LatestRev"); } set { SetValue("LatestRev", value.ToString()); } }
        public string ImageFolderRel
        {
            get
            {
                if (!Exists)
                {
                    // we do not have any module data, so assume this is a system upload.
                    if (SystemKey != "")
                    {
                        var systemData = new SystemData(SystemKey);
                        if (systemData.Exists) return systemData.ImageFolderRelPath;
                    }
                    return "/DesktopModules/DNNrocket/SystemData/images";
                }
                return PortalUtils.HomeDNNrocketDirectoryRel().TrimEnd('/') + "/" + ImageFolder;
            }
        }
        public string DocumentFolderRel { get{ return PortalUtils.HomeDNNrocketDirectoryRel().TrimEnd('/') + "/" + DocumentFolder;} }
        public string DocumentFolder { get { return GetValue("documentfolder", "");} set { SetValue("DocumentFolder", value); } }
        public string ImageFolder { get { return GetValue("imagefolder", ""); } set { SetValue("ImageFolder", value); } }
        public string DocumentFolderMapPath { get { return DNNrocketUtils.MapPath(DocumentFolderRel); } }
        public string ImageFolderMapPath { get { return DNNrocketUtils.MapPath(ImageFolderRel); } }
        public bool Exists { get { return GetValueBool("Exists"); } set { SetValue("Exists", value.ToString()); } }
        public string Name { get { return GetValue("Name", ""); } set { SetValue("Name", value); } }
        public string ModuleType { get { return GetValue("ModuleType", ""); } set { SetValue("ModuleType", value); } }
        public int ModuleId { get {return _moduleid; } }
        public string SystemKey { get { return GetValue("SystemKey", ""); } set { SetValue("SystemKey", value); } }
        public int TabId { get { return GetValueInt("TabId"); } set { SetValue("TabId", value.ToString()); } }
        public string ShareData { get { return GetValue("sharedata", ""); } set { SetValue("sharedata", value); } }
        public string ModuleRefDataSource { get { return GetValue("ModuleRefDataSource"); } set { SetValue("ModuleRefDataSource", value.ToString()); } }
        public int ModuleIdDataSource
        {
            get
            {
                var dv = GetValueInt("ModuleIdDataSource");
                if (dv == 0) dv = _moduleid;
                return dv;
            }
            set { SetValue("ModuleIdDataSource", value.ToString()); }
        }
        public bool DataSourceExternal { get; set; }
        public string ModuleRef { get { return GetValue("ModuleRef"); } set { SetValue("ModuleRef", value); } }
        public bool CacheDisbaled { get { return GetValueBool("disablecache"); } set { SetValue("disablecache", value.ToString()); } }
        public bool CacheEnabled { get { return !GetValueBool("disablecache"); } }
        public bool ExportResourceFiles { get { return GetValueBool("exportresourcefiles"); } set { SetValue("exportresourcefiles", value.ToString()); } }
        public Dictionary<string,string> ModuleSettings { get; private set; }
        public string CacheGroupId { get { return "datamoduleid:" + ModuleIdDataSource; } }
        public bool AutoBackUp { get { return GetValueBool("autobackup"); } set { SetValue("autobackup", value.ToString()); } }

        public string OrderBySQL(string orderbyref = "")
        { 
            var orderbysql = "";
            if (ModuleSettings.ContainsKey(orderbyref))
            {
                orderbysql = ModuleSettings[orderbyref];
            }
            if (orderbysql == null || orderbysql == "") return " order by R1.[SortOrder] ";
            return orderbysql;
        }

        public string GetFilterSQL(SimplisityInfo paramInfo, int index = 0)
        {
            var key = "filter" + index;
            var filtersql = "";
            if (ModuleSettings.ContainsKey(key))
            {
                filtersql = ModuleSettings[key];
                FastReplacer fr = new FastReplacer("{", "}", false);
                fr.Append(filtersql);
                var tokenList = fr.GetTokenStrings();
                foreach (var token in tokenList)
                {
                    if (paramInfo.GetXmlProperty(token) == "") return ""; // no data so ignore filter
                    fr.Replace("{" + token + "}", paramInfo.GetXmlProperty(token));
                }
                filtersql = " " + fr.ToString() + " ";
            }
            return filtersql;
        }


    }

}
