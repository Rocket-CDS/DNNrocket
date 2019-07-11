using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocketAPI.Componants
{

    public class ConfigData
    {

        private bool _configExists;
        private int _tabid;
        private int _moduleid;
        private int _dataModuleid;
        private int _systemid;
        private int _portalid;
        private string _moduleref;
        private string _moduletype;
        private SimplisityInfo _systemInfo;
        private string _requiredCultureCode;

        public SimplisityInfo ConfigInfo;

        public ConfigData(int portalId, SimplisityInfo systemInfo, int tabId, int moduleId, string requiredCultureCode = "")
        {
            _portalid = portalId;
            _tabid = tabId;
            _moduleid = moduleId;
            _systemid = systemInfo.ItemID;
            _systemInfo = systemInfo;
            _requiredCultureCode = requiredCultureCode;
            if (_requiredCultureCode == "") _requiredCultureCode = DNNrocketUtils.GetCurrentCulture();

            PopulateConfig();
        }

        #region "CONFIG"

        public void PopulateConfig()
        {
            var objCtrl = new DNNrocketController();

            _moduletype = _systemInfo.GUIDKey;

            ConfigInfo = objCtrl.GetByType(_portalid, _moduleid, "CONFIG" + _moduletype, "", _requiredCultureCode);
            if (ConfigInfo == null)
            {
                _configExists = false;
                ConfigInfo = new SimplisityInfo();
                ConfigInfo.ModuleId = _moduleid;
                ConfigInfo.Lang = _requiredCultureCode;
                _dataModuleid = _moduleid;
                _moduleref = GeneralUtils.GetUniqueKey();
            }
            else
            {
                _moduleref = ConfigInfo.GUIDKey;
                _dataModuleid = ConfigInfo.GetXmlPropertyInt("genxml/select/datamoduleid");
                if (_dataModuleid == 0) _dataModuleid = _moduleid;
                if (AppTheme == "")
                {
                    _configExists = false;
                }
                else
                {
                    _configExists = true;
                }
            }

        }

        public void DeleteConfig()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetByType(_portalid, _moduleid, "CONFIG" + _moduletype, "", _requiredCultureCode);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                CacheUtils.ClearAllCache();
                PopulateConfig();
            }
        }

        public void SaveAppTheme(string appTheme)
        {
            if (appTheme != "")
            {
                ConfigInfo.SetXmlProperty("genxml/hidden/apptheme", appTheme);
                var objCtrl = new DNNrocketController();
                var info = objCtrl.SaveData(_moduleref, "CONFIG" + _moduletype, ConfigInfo, _systemid, _moduleid);
                CacheUtils.ClearAllCache();
                PopulateConfig();
            }
        }

        public void SaveConfig(SimplisityInfo postInfo, string templateRelPath = "")
        {
            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            if (postInfo.GetXmlProperty("genxml/hidden/apptheme") != "")
            {
                ConfigInfo.SetXmlProperty("genxml/dropdownlist/paymentprovider", postInfo.GetXmlProperty("genxml/dropdownlist/paymentprovider"));
            }
            else
            {
                postInfo.SetXmlProperty("genxml/dropdownlist/paymentprovider", ConfigInfo.GetXmlProperty("genxml/dropdownlist/paymentprovider"));
            }
            postInfo.SetXmlProperty("genxml/checkbox/noiframeedit", "False"); // iframe edit
            postInfo.SetXmlProperty("genxml/hidden/templaterelpath", templateRelPath);

            ConfigInfo.XMLData = postInfo.XMLData;

            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData(_moduleref, "CONFIG" + _moduletype, ConfigInfo, _systemid, _moduleid);
            CacheUtils.ClearAllCache();
            PopulateConfig();
        }

        #endregion

        public string ProviderAssembly { get { return ConfigInfo.GetXmlProperty("genxml/textbox/assembly"); } }
        public string ProviderClass { get { return ConfigInfo.GetXmlProperty("genxml/textbox/namespaceclass"); } }
        public string ManagerEmail { get { return ConfigInfo.GetXmlProperty("genxml/textbox/manageremail"); } }
        public string AppTheme { get { return ConfigInfo.GetXmlProperty("genxml/hidden/apptheme"); } }
        public string AppThemeVersion { get { return ConfigInfo.GetXmlProperty("genxml/select/versionfolder"); } }
        public string ImageFolderRel { get{ return DNNrocketUtils.HomeRelDirectory() + "/" + ImageFolder; } }
        public string DocumentFolderRel { get{ return DNNrocketUtils.HomeRelDirectory() + "/" + DocumentFolder;} }

        public string SystemKey { get { return _systemInfo.GetXmlProperty("genxml/textbox/ctrlkey"); } }

        public string DocumentFolder
        {
            get
            {
                if (ConfigInfo.GetXmlProperty("genxml/textbox/documentfolder") == "")
                {
                    return "docs";
                }
                else
                {
                    return ConfigInfo.GetXmlProperty("genxml/textbox/documentfolder");
                }
            }
        }
        public string ImageFolder
        {
            get
            {
                if (ConfigInfo.GetXmlProperty("genxml/textbox/imagefolder") == "")
                {
                    return "images";
                }
                else
                {
                    return ConfigInfo.GetXmlProperty("genxml/textbox/imagefolder");
                }
            }
        }
        public string DocumentFolderMapPath { get { return DNNrocketUtils.MapPath(DocumentFolderRel); } }
        public string ImageFolderMapPath { get { return DNNrocketUtils.MapPath(ImageFolderRel); } }

        public bool Exists { get { return _configExists; } }
        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
        public int SystemId { get { return _systemid; } }
        public int DataModuleId { get { return _dataModuleid; } }
        public string ModuleRef { get { return _moduleref; } }

    }

}
