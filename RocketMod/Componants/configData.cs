using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketMod
{

    public class ConfigData
    {

        private bool _configExists;
        private int _tabid;
        private int _moduleid;
        private int _systemid;
        private int _portalid;


        public SimplisityInfo ConfigInfo;

        public ConfigData(int portalId, int systemId, int tabId, int moduleId)
        {
            _portalid = portalId;
            _tabid = tabId;
            _moduleid = moduleId;
            _systemid = systemId;

            PopulateConfig();
        }

        #region "CONFIG"

        public void PopulateConfig()
        {
            var objCtrl = new DNNrocketController();
            ConfigInfo = objCtrl.GetData("rocketmod_" + _moduleid, "CONFIG",DNNrocketUtils.GetCurrentCulture(), -1, _moduleid, true);
            if (ConfigInfo == null)
            {
                _configExists = false;
                ConfigInfo = new SimplisityInfo();
                ConfigInfo.ModuleId = _moduleid;
            }
            else
            {
                    _configExists = true;

            }
        }

        public void DeleteConfig()
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("rocketmod_" + _moduleid, "CONFIG", DNNrocketUtils.GetCurrentCulture(), -1, _moduleid, true);
            if (info != null)
            {
                objCtrl.Delete(info.ItemID);
                PopulateConfig();
            }
        }

        public void SaveConfig(SimplisityInfo postInfo)
        {
            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            if (postInfo.GetXmlProperty("genxml/dropdownlist/paymentprovider") != "")
            {
                ConfigInfo.SetXmlProperty("genxml/dropdownlist/paymentprovider", postInfo.GetXmlProperty("genxml/dropdownlist/paymentprovider"));
            }
            else
            {
                postInfo.SetXmlProperty("genxml/dropdownlist/paymentprovider", ConfigInfo.GetXmlProperty("genxml/dropdownlist/paymentprovider"));
            }
            postInfo.SetXmlProperty("genxml/checkbox/noiframeedit", "True"); // we do not want iframe edit

            ConfigInfo.XMLData = postInfo.XMLData;

            var objCtrl = new DNNrocketController();
            var info = objCtrl.SaveData("rocketmod_" + _moduleid, "CONFIG", ConfigInfo, _systemid, _moduleid);
            PopulateConfig();
        }

        #endregion

        public string ProviderAssembly { get { return ConfigInfo.GetXmlProperty("genxml/textbox/assembly"); } }
        public string ProviderClass { get { return ConfigInfo.GetXmlProperty("genxml/textbox/namespaceclass"); } }
        public string RazorTemplate { get { return ConfigInfo.GetXmlProperty("genxml/textbox/razortemplate"); } }

        public string ManagerEmail { get { return ConfigInfo.GetXmlProperty("genxml/textbox/manageremail"); } }
        public string WebsiteUrl { get { return ConfigInfo.GetXmlProperty("genxml/textbox/websiteurl"); } }
        public string CompanyName { get { return ConfigInfo.GetXmlProperty("genxml/textbox/companyname"); } }

        public bool Exists { get { return _configExists; } }
        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
        public int SystemId { get { return _systemid; } }


    }

}
