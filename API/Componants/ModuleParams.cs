using DNNrocketAPI;
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

        private SimplisityInfo _moduleParamsInfo;
        private string _cacheKey;
        private int _moduleid;
        private int _systemid;

        public ModuleParams(int moduleId,int systemid, string requiredCultureCode, bool useCache = true)
        {
            _systemid = systemid;
            _moduleid = moduleId;
            _moduleParamsInfo = new SimplisityInfo();
            _cacheKey = "moduleparams*" + moduleId + "*" + requiredCultureCode;
            _moduleParamsInfo = (SimplisityInfo)CacheUtils.GetCache(_cacheKey);
            if ((_moduleParamsInfo == null || !useCache) && moduleId > 0)
            {
                var objCtrl = new DNNrocketController();
                _moduleParamsInfo = objCtrl.GetData(_cacheKey, "MODULEPARAMS", requiredCultureCode, _systemid, _moduleid);
                CacheUtils.SetCache(_cacheKey, _moduleParamsInfo);
            }

        }

        public void Save()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.SaveData(_cacheKey, "MODULEPARAMS", _moduleParamsInfo, _systemid, _moduleid);
        }

        public string GetValue(string key, string defaultValue = "")
        {
            var rtn = _moduleParamsInfo.GetXmlProperty("genxml/hidden/" + key);
            if (rtn == "") return defaultValue;
            return rtn;
        }
        public int GetValueInt(string key)
        {
            return _moduleParamsInfo.GetXmlPropertyInt("genxml/hidden/" + key);
        }
        public double GetValueDouble(string key)
        {
            return _moduleParamsInfo.GetXmlPropertyDouble("genxml/hidden/" + key);
        }
        public bool GetValueBool(string key)
        {
            return _moduleParamsInfo.GetXmlPropertyBool("genxml/hidden/" + key);
        }
        public void SetValue(string key, string value)
        {
            _moduleParamsInfo.SetXmlProperty("genxml/hidden/" + key, value);
        }

        public SimplisityInfo Info { get { return _moduleParamsInfo; } }

        public string ProviderAssembly { get { return _moduleParamsInfo.GetXmlProperty("genxml/textbox/assembly"); } }
        public string ProviderClass { get { return _moduleParamsInfo.GetXmlProperty("genxml/textbox/namespaceclass"); } }


        public string AppThemeFolder { get { return GetValue("AppThemeFolder"); } set { SetValue("AppThemeFolder", value); } } 
        public string AppThemeFolderRel { get { return GetValue("AppThemeFolderRel"); } set { SetValue("AppThemeFolderRel", value); } }
        public string AppThemeFolderMapPath { get { return GetValue("AppThemeFolderMapPath"); } set { SetValue("AppThemeFolderMapPath", value); } } 
        public string AppVersionFolder { get { return GetValue("AppVersionFolder"); } set { SetValue("AppVersionFolder", value); } } 
        public string AppThemeVersionFolderRel { get { return GetValue("AppThemeVersionFolderRel"); } set { SetValue("AppThemeVersionFolderRel", value); } } 
        public string AppThemeVersionFolderMapPath { get { return GetValue("AppThemeVersionFolderMapPath"); } set { SetValue("AppThemeVersionFolderMapPath", value); } }
        public string AppProjectFolderRel { get { return GetValue("AppProjectFolderRel"); } set { SetValue("AppProjectFolderRel", value); } }
        public string AppSystemThemeFolderRel { get { return GetValue("AppSystemThemeFolderRel"); } set { SetValue("AppSystemThemeFolderRel", value); } }
        public string AppSystemThemeFolderMapPath { get { return GetValue("AppSystemThemeFolderMapPath"); } set { SetValue("AppSystemThemeFolderMapPath", value); } }
        public string AppThemeVersion { get { return GetValue("AppThemeVersion"); } set { SetValue("AppThemeVersion", value); } }

        public string ImageFolderRel { get{ return DNNrocketUtils.HomeRelDirectory() + "/" + ImageFolder; } }
        public string DocumentFolderRel { get{ return DNNrocketUtils.HomeRelDirectory() + "/" + DocumentFolder;} }


        public string DocumentFolder { get { return GetValue("DocumentFolder","docs"); } set { SetValue("DocumentFolder", value); } }
        public string ImageFolder { get { return GetValue("ImageFolder", "images"); } set { SetValue("ImageFolder", value); } }

        public string DocumentFolderMapPath { get { return DNNrocketUtils.MapPath(DocumentFolderRel); } }
        public string ImageFolderMapPath { get { return DNNrocketUtils.MapPath(ImageFolderRel); } }

        public bool Exists { get; set; } 

        public int ModuleId { get {return _moduleid; } }
        public int SystemId { get { return _systemid; } }

        public int DataModuleId { get { return GetValueInt("DataModuleId"); } set { SetValue("DataModuleId", value.ToString()); } }
        public string ModuleRef { get { return GetValue("ModuleRef"); } set { SetValue("ModuleRef", value); } }
    }

}
