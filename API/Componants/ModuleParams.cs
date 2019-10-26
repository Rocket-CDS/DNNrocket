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
    /// This class is used to pass data from modules in the system to the API, where any normal usage would cause a circular reference.
    /// The moduel code should save the dtaa as a simplisityInfo class, using "portalId, "systemkey", "tabId", "moduleId", "requiredCultureCode".
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
        private int _systemid;

        public ModuleParams(int moduleId, int systemid, bool useCache = true, string tableName = "DNNrocket")
        {
            _tableName = tableName;
            _systemid = systemid;
            _moduleid = moduleId;
            _moduleParamsRec = new SimplisityRecord();
            _cacheKey = "moduleparams*" + moduleId;
            _moduleParamsRec = (SimplisityRecord)CacheUtils.GetCache(_cacheKey);

            if ((_moduleParamsRec == null || !useCache))
            {
                if (moduleId <= 0)
                {
                    _moduleParamsRec = new SimplisityRecord(); // sto error when no moduleid
                }
                else
                {
                    var objCtrl = new DNNrocketController();
                    _moduleParamsRec = objCtrl.GetRecordByGuidKey(-1, _moduleid, "MODULEPARAMS", _cacheKey, "", _tableName);
                    if (_moduleParamsRec == null)
                    {
                        _moduleParamsRec = new SimplisityRecord();
                        _moduleParamsRec.PortalId = -1;
                        _moduleParamsRec.TypeCode = "MODULEPARAMS";
                        _moduleParamsRec.GUIDKey = _cacheKey;
                        _moduleParamsRec.ModuleId = _moduleid;
                        objCtrl.SaveRecord(_moduleParamsRec);
                    }
                    CacheUtils.SetCache(_cacheKey, _moduleParamsRec);
                }
            }
        }
        public void Save()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.SaveRecord(_cacheKey, "MODULEPARAMS", _moduleParamsRec, _systemid, _moduleid, _tableName);
        }
        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(Record.ItemID, _tableName);
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

        public string ImageFolderRel { get{ return DNNrocketUtils.HomeDNNrocketRelDirectory().TrimEnd('/') + "/" + ImageFolder; } }
        public string DocumentFolderRel { get{ return DNNrocketUtils.HomeDNNrocketRelDirectory().TrimEnd('/') + "/" + DocumentFolder;} }

        public string DocumentFolder { get { return GetValue("DocumentFolder","docs"); } set { SetValue("DocumentFolder", value); } }
        public string ImageFolder { get { return GetValue("ImageFolder", "images"); } set { SetValue("ImageFolder", value); } }

        public string DocumentFolderMapPath { get { return DNNrocketUtils.MapPath(DocumentFolderRel); } }
        public string ImageFolderMapPath { get { return DNNrocketUtils.MapPath(ImageFolderRel); } }

        public bool Exists { get { return GetValueBool("Exists"); } set { SetValue("Exists", value.ToString()); } }
        public string Name { get { return GetValue("Name", ""); } set { SetValue("Name", value); } }
        public string ModuleType { get { return GetValue("ModuleType", ""); } set { SetValue("ModuleType", value); } }
        public int ModuleId { get {return _moduleid; } }
        public int SystemId { get { return _systemid; } }
        public int DataModuleId
        {
            get
            {
                var dv = GetValueInt("DataModuleId");
                if (dv == 0) dv = _moduleid;
                return dv;
            }
            set { SetValue("DataModuleId", value.ToString()); }
        }
        public string ModuleRef { get { return GetValue("ModuleRef"); } set { SetValue("ModuleRef", value); } }
        public bool CacheDisbaled { get { return GetValueBool("disbalecache"); } set { SetValue("disbalecache", value.ToString()); } }
        public bool CacheEnabled { get { return !GetValueBool("disbalecache"); } }
    }

}
