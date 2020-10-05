using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class SystemGlobalData
    {
        private static string _cacheKey; 
        public SystemGlobalData(bool cache = true)
        {
            _cacheKey = "rocketGLOBALSETTINGS";
            if (cache) Info = (SimplisityInfo)CacheUtilsDNN.GetCache(_cacheKey);
            if (Info == null) LoadData();

            if (cache) ConfigInfo = (SimplisityRecord)CacheUtilsDNN.GetCache(_cacheKey + "ConfigInfo");
            if (ConfigInfo == null) LoadConfig();
        }
        public void Save(SimplisityInfo postInfo)
        {
             var objCtrl = new DNNrocketController();
            Info.XMLData = postInfo.XMLData;
            Update();
        }
        public void Update()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Update(Info);
            CacheUtilsDNN.ClearAllCache();
            CacheUtilsDNN.SetCache(_cacheKey, Info);
        }
        private void LoadConfig()
        {
            ConfigInfo = new SimplisityRecord();
            //import the config data from XML file.
            var fullFileName = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket").TrimEnd('\\') + "\\globalconfig.xml";
            var xmlData = FileUtils.ReadFile(fullFileName);
            if (xmlData != "") ConfigInfo.XMLData = xmlData;
            CacheUtilsDNN.SetCache(_cacheKey + "ConfigInfo", ConfigInfo);
        }

        private void LoadData()
        {
            var objCtrl = new DNNrocketController();
            Info = objCtrl.GetByType(PortalUtils.GetPortalId(), -1, "GLOBALSETTINGS");
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.ItemID = -1;
                Info.PortalId = PortalUtils.GetPortalId();
                Info.TypeCode = "GLOBALSETTINGS";

                // add any MUST have settings.
                Info.SetXmlProperty("genxml/textbox/globalheading", "<link rel='stylesheet' href='/DesktopModules/DNNrocket/css/w3.css'>");

                Info.ItemID = objCtrl.Update(Info);
            }
            CacheUtilsDNN.SetCache(_cacheKey, Info);
        }

        public SimplisityInfo Info { get; set; }
        public SimplisityRecord ConfigInfo { get; set; }

        public string FtpUserName { get { return Info.GetXmlProperty("genxml/textbox/ftpuser"); } set { Info.SetXmlProperty("genxml/textbox/ftpuser",value); } }
        public string FtpPassword { get { return Info.GetXmlProperty("genxml/textbox/ftppassword"); } set { Info.SetXmlProperty("genxml/textbox/ftppassword", value); } }
        public string FtpServer { get { return Info.GetXmlProperty("genxml/textbox/ftpserver"); } set { Info.SetXmlProperty("genxml/textbox/ftpserver", value); } }
        public string ImageType { get { return Info.GetXmlProperty("genxml/select/imagetype"); } set { Info.SetXmlProperty("genxml/select/imagetype", value); } }
        public bool PngImage { get { if (Info.GetXmlProperty("genxml/select/imagetype") != "jpg") return true; else return false; } }
        public string CKEditorCssList { get { return Info.GetXmlProperty("genxml/textbox/ckeditorcsslist"); } set { Info.SetXmlProperty("genxml/textbox/ckeditorcsslist", value); } }
        public bool SchedulerIsInstalled { get { return Info.GetXmlPropertyBool("genxml/checkbox/schedulerinstalled"); } set { Info.SetXmlProperty("genxml/checkbox/schedulerinstalled", value.ToString()); } }
        public bool SchedulerIsEnabled { get { return Info.GetXmlPropertyBool("genxml/checkbox/schedulerenabled"); } set { Info.SetXmlProperty("genxml/checkbox/schedulerenabled", value.ToString()); } }

        // globalconfig.xml - Config XML file data
        public string LicenseUrl { get { return ConfigInfo.GetXmlProperty("genxml/hidden/licenseurl"); } }
        public string PublicAppThemeURI { get { return ConfigInfo.GetXmlProperty("genxml/hidden/publicappthemeuri"); } }

        public string GlobalPageHeading
        {
            get { return Info.GetXmlProperty("genxml/textbox/globalheading"); }
            set { Info.SetXmlProperty("genxml/textbox/globalheading", value); }
        }
        public DateTime LastForcedSchedulerTime
        {
            get
            {
                if (GeneralUtils.IsDateInvariantCulture(Info.GetXmlPropertyDate("genxml/hidden/lastschedulertime")))
                    return Info.GetXmlPropertyDate("genxml/hidden/lastschedulertime");
                else
                    return DateTime.Now.AddDays(-10);
            }
            set { Info.SetXmlProperty("genxml/hidden/lastschedulertime", value.ToString(), TypeCode.DateTime); }
        }


    }
}
