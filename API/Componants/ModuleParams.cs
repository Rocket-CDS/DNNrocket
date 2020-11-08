using DNNrocketAPI;
using DotNetNuke.Entities.Modules;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace DNNrocketAPI.Componants
{
    /// <summary>
    /// This class is used to pass data from modules in the system to the API, where any normal usage would cause a circular reference.
    /// The module code should save the data as a simplisityInfo class, using "portalId, "systemkey", "tabId", "moduleId", "requiredCultureCode".
    /// This class will then use the database record to create the paramaters.
    /// Some params are standard and have a property to help identify data, others use normal xpath.
    /// Data can also be read using hte "GetValue" method, or Written using the "SetValue" method.
    /// </summary>
    public class ModuleParams
    {

        private SimplisityRecord _moduleParamsRec;
        private string _cacheKey;
        private string _tableName;
        private int _moduleid;

        public ModuleParams(int moduleId, string systemKey = "", bool useCache = true, string tableName = "DNNrocket")
        {
            _moduleParamsRec = new SimplisityRecord();
            _tableName = tableName;
            _moduleid = moduleId;
            _cacheKey = "moduleparams*" + moduleId;
            _moduleParamsRec = (SimplisityRecord)CacheUtilsDNN.GetCache(_cacheKey);
            var objCtrl = new DNNrocketController();
            if ((_moduleParamsRec == null || !useCache))
            {
                if (moduleId <= 0)
                {
                    _moduleParamsRec = new SimplisityRecord();
                }
                else
                {
                    _moduleParamsRec = objCtrl.GetRecordByGuidKey(-1, _moduleid, "MODULEPARAMS", _cacheKey, "", _tableName);
                    if (_moduleParamsRec == null)
                    {
                        _moduleParamsRec = new SimplisityRecord();
                        _moduleParamsRec.PortalId = -1;
                        _moduleParamsRec.TypeCode = "MODULEPARAMS";
                        _moduleParamsRec.GUIDKey = _cacheKey;
                        _moduleParamsRec.ModuleId = _moduleid;
                        _moduleParamsRec = objCtrl.SaveRecord(_moduleParamsRec);
                    }
                    CacheUtilsDNN.SetCache(_cacheKey, _moduleParamsRec);
                }
            }
            DataSourceExternal = false;
            if (systemKey != "") SystemKey = systemKey;
            if (ModuleIdDataSource != _moduleid) DataSourceExternal = true;
            if (ModuleRef == "") ModuleRef = GeneralUtils.GetGuidKey();
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

            LoadAppTheme(_moduleParamsRec.GetXmlProperty("genxml/hidden/appthemefolder"));

        }
        public void LoadAppTheme(string appthemefolder)
        {
            var appThemeCacheKey = "apptheme*moduleparams*" + _moduleid;
            AppTheme = (AppThemeLimpet)CacheUtils.GetCache(appThemeCacheKey);
            if (AppTheme == null || AppTheme.AppThemeFolder != appthemefolder)
            {
                AppTheme = new AppThemeLimpet("dnnrocketmodule", appthemefolder, "");
                CacheUtils.SetCache(appThemeCacheKey, AppTheme);
            }
        }
        public void Save()
        {
            var objCtrl = new DNNrocketController();

            DataSourceExternal = false;
            if (ModuleIdDataSource != _moduleid) DataSourceExternal = true;
            _moduleParamsRec = objCtrl.SaveRecord(_cacheKey, "MODULEPARAMS", _moduleParamsRec, _moduleid, _tableName);
            CacheUtilsDNN.SetCache(_cacheKey, _moduleParamsRec);
        }
        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(Record.ItemID, _tableName);
            _moduleParamsRec = new SimplisityRecord();
            CacheUtilsDNN.RemoveCache(_cacheKey);
        }

        public string GetValue(string key, string defaultValue = "")
        {
            var rtn = _moduleParamsRec.GetXmlProperty("genxml/hidden/" + key.ToLower());
            if (rtn == "") return defaultValue;
            return rtn;
        }
        public int GetValueInt(string key)
        {
            return _moduleParamsRec.GetXmlPropertyInt("genxml/hidden/" + key.ToLower());
        }
        public double GetValueDouble(string key)
        {
            return _moduleParamsRec.GetXmlPropertyDouble("genxml/hidden/" + key.ToLower());
        }
        public bool GetValueBool(string key)
        {
            if (_moduleParamsRec == null) return false;
            return _moduleParamsRec.GetXmlPropertyBool("genxml/hidden/" + key.ToLower());
        }
        public void SetValue(string key, string value)
        {
            _moduleParamsRec.SetXmlProperty("genxml/hidden/" + key.ToLower(), value);
        }

        public SimplisityRecord Record { get { return _moduleParamsRec; } }

        public string ProviderAssembly { get { return _moduleParamsRec.GetXmlProperty("genxml/textbox/assembly"); } }
        public string ProviderClass { get { return _moduleParamsRec.GetXmlProperty("genxml/textbox/namespaceclass"); } }
        public string DetailUrlParam { get { return GetValue("DetailUrlParam"); } set { SetValue("DetailUrlParam", value); } }
        public bool DetailView { get { return GetValueBool("detailview"); } set { SetValue("detailview", value.ToString()); } }
        public AppThemeLimpet AppTheme { get; private set; }

        public string ImageFolderRel
        {
            get
            {
                if (!Exists)
                {
                    // we do not have any module data, so assume this is a system upload.
                    if (SystemKey != "")
                    {
                        var systemData = new SystemLimpet(SystemKey);
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
        public bool Exists
        {
            get
            {
                if (Record.ItemID <= 0 || AppTheme.AppThemeFolder == "")
                {
                    return false;
                }
                return true;
            }
        }
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
