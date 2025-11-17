using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class SystemGlobalData
    {
        private static string _cacheKey; 
        public SystemGlobalData(bool cache = true)
        {
            _cacheKey = "rocketGLOBALSETTINGS";
            if (cache) Info = (SimplisityInfo)CacheUtils.GetCache(_cacheKey);
            if (Info == null) LoadData();
            if (AccessCode == "") RegenerateAccessCodes();
            if (cache) ConfigInfo = (SimplisityRecord)CacheUtils.GetCache(_cacheKey + "ConfigInfo");
            if (ConfigInfo == null) LoadConfig();
        }
        public void Save(SimplisityInfo postInfo)
        {
            var ac = AccessCode;
            var ap = AccessPassword;
             var objCtrl = new DNNrocketController();
            Info.XMLData = postInfo.XMLData;
            AccessCode = ac;
            AccessPassword = ap;
            Update();
        }
        public void Update()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Update(Info);
            CacheUtils.ClearAllCache();
            CacheUtils.SetCache(_cacheKey, Info);
        }
        private void LoadConfig()
        {
            ConfigInfo = new SimplisityRecord();
            //import the config data from XML file.
            var fullFileName = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket").TrimEnd('\\') + "\\globalconfig.xml";
            var xmlData = FileUtils.ReadFile(fullFileName);
            if (xmlData != "") ConfigInfo.XMLData = xmlData;
            CacheUtils.SetCache(_cacheKey + "ConfigInfo", ConfigInfo);
        }

        private void LoadData()
        {
            var objCtrl = new DNNrocketController();
            Info = objCtrl.GetByType(0, -1, "GLOBALSETTINGS");
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ItemID = -1;
                Info.PortalId = 0;
                Info.TypeCode = "GLOBALSETTINGS";

                Info.ItemID = objCtrl.Update(Info);
            }
            CacheUtils.SetCache(_cacheKey, Info);
        }
        public void RegenerateAccessCodes()
        {
            AccessCode = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString();
            AccessPassword = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString();
        }
        public bool ValidClient(string clientCode)
        {
            return ClientCodes.Contains(clientCode);
        }

        public SimplisityInfo Info { get; set; }
        public SimplisityRecord ConfigInfo { get; set; }

        public bool SchedulerIsInstalled { get { return Info.GetXmlPropertyBool("genxml/checkbox/schedulerinstalled"); } set { Info.SetXmlProperty("genxml/checkbox/schedulerinstalled", value.ToString()); } }
        public bool SchedulerIsEnabled { get { return Info.GetXmlPropertyBool("genxml/checkbox/schedulerenabled"); } set { Info.SetXmlProperty("genxml/checkbox/schedulerenabled", value.ToString()); } }        
        public string Name { get { return Info.GetXmlProperty("genxml/textbox/name"); } set { Info.SetXmlProperty("genxml/textbox/name", value); } }
        public string RootDomain { get { return Info.GetXmlProperty("genxml/textbox/rootdomain"); } set { Info.SetXmlProperty("genxml/textbox/rootdomain", value); } }
        public bool PreCompileRazor { get { return Info.GetXmlPropertyBool("genxml/checkbox/precompilerazor"); } set { Info.SetXmlProperty("genxml/checkbox/precompilerazor", value.ToString()); } }
        public bool PreCompileRazorAdmin { get { return Info.GetXmlPropertyBool("genxml/checkbox/precompilerazoradmin"); } set { Info.SetXmlProperty("genxml/checkbox/precompilerazoradmin", value.ToString()); } }
        public string AccessCode { get { return Info.GetXmlProperty("genxml/config/accesscode"); } set { Info.SetXmlProperty("genxml/config/accesscode", value); } }
        public string AccessPassword { get { return Info.GetXmlProperty("genxml/config/accesspassword"); } set { Info.SetXmlProperty("genxml/config/accesspassword", value); } }
        public string LogoTemplate { get { if (Info.GetXmlProperty("genxml/textbox/logotemplate") != "") return Info.GetXmlProperty("genxml/textbox/logotemplate"); else return "Logo.cshtml"; } }
        public string[] ClientCodes { get { return Info.GetXmlProperty("genxml/textbox/clientcodes").Split(','); } }
        public string DeepLurl { get { return Info.GetXmlProperty("genxml/textbox/deeplurl"); } set { Info.SetXmlProperty("genxml/textbox/deeplurl", value); } }
        public string DeepLauthKey { get { return Info.GetXmlProperty("genxml/textbox/deeplauthkey"); } set { Info.SetXmlProperty("genxml/textbox/deeplauthkey", value); } }
        public bool Log { get { return Info.GetXmlPropertyBool("genxml/checkbox/log"); } set { Info.SetXmlProperty("genxml/checkbox/log", value.ToString()); } }        
        public int MaxLogFiles { get { return Info.GetXmlPropertyInt("genxml/textbox/maxlogfiles"); } set { Info.SetXmlProperty("genxml/textbox/maxlogfiles", value.ToString()); } }
        public string ChatGptKey { get { return Info.GetXmlProperty("genxml/textbox/openaichatgptkey"); } set { Info.SetXmlProperty("genxml/textbox/openaichatgptkey", value); } }
        public string AmbeeKey { get { return Info.GetXmlProperty("genxml/textbox/ambeeapikey"); } set { Info.SetXmlProperty("genxml/textbox/ambeeapikey", value); } }
    }
}
