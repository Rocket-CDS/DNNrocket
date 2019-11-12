using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Componants;

namespace DNNrocketAPI
{
    public class SystemInfoData
    {
        public SystemInfoData(string systemKey)
        {
            var objCtrl = new DNNrocketController();
            var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemKey);
            InitSystem(systemInfo);
        }
        public SystemInfoData(int systemId)
        {
            var objCtrl = new DNNrocketController();
            var systemInfo = objCtrl.GetInfo(systemId);
            InitSystem(systemInfo);
        }
        public SystemInfoData(SimplisityInfo systemInfo)
        {
            InitSystem(systemInfo);
        }
        private void InitSystem(SimplisityInfo systemInfo)
        {
            if (systemInfo == null)
            {
                systemInfo = new SimplisityInfo();
                Exists = false;
            }
            else
            {
                Exists = true;
            }
            Info = systemInfo;
            EventList = new List<DNNrocketInterface>();
            InterfaceList = new Dictionary<string, DNNrocketInterface>();
            Settings = new Dictionary<string, string>();
            var l = Info.GetList("interfacedata");
            foreach (var r in l)
            {
                var rocketInterface = new DNNrocketInterface(r);
                if (rocketInterface.IsProvider("eventprovider") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    EventList.Add(rocketInterface);
                }
                InterfaceList.Add(rocketInterface.InterfaceKey, rocketInterface);
            }
            var l2 = Info.GetList("settingsdata");
            foreach (var s in l2)
            {
                var key = s.GetXmlProperty("genxml/textbox/name");
                if (key != "" && !Settings.ContainsKey(key)) Settings.Add(key, s.GetXmlProperty("genxml/textbox/value"));
            }

            var licenseData = new LicenseData(SystemKey, LicenseKey, DNNrocketUtils.GetDefaultWebsiteDomainUrl());
            IsLicensed = licenseData.IsLicensed;
            
        }

        public SimplisityInfo SystemInfo { get { return Info; } }
        public SimplisityInfo Info { get; set; }
        public List<DNNrocketInterface> EventList { get; set;}
        public bool Exists { get; set; }
        public bool IsLicensed { get; private set; }
        public Dictionary<string, DNNrocketInterface> InterfaceList { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public string GetSetting(string key)
        {
            if (Settings.ContainsKey(key)) return Settings[key];
            return "";
        }

        public bool HasInterface(string interfaceKey)
        {
            return InterfaceList.ContainsKey(interfaceKey);
        }

        public DNNrocketInterface GetInterface(string interfaceKey)
        {
            var s = Info.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfaceKey);
            if (s == null) return null;
            return new DNNrocketInterface(s);
        }

        public string SystemKey
        {
            get { return Info.GetXmlProperty("genxml/textbox/ctrlkey"); }
            set { Info.SetXmlProperty("genxml/textbox/ctrlkey", value); }
        }
        public string SystemName
        {
            get { return Info.GetXmlProperty("genxml/textbox/systemname"); }
            set { Info.SetXmlProperty("genxml/textbox/systemname", value); }
        }
        public string ApiUrl
        {
            get { return Info.GetXmlProperty("genxml/textbox/apiurl"); }
            set { Info.SetXmlProperty("genxml/textbox/apiurl", value); }
        }
        public string AdminUrl
        {
            get { return Info.GetXmlProperty("genxml/textbox/adminurl"); }
            set { Info.SetXmlProperty("genxml/textbox/adminurl", value); }
        }
        public bool DebugMode
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/debugmode"); }
            set { Info.SetXmlProperty("genxml/checkbox/debugmode", value.ToString()); }
        }
        public string LicenseKey
        {
            get { return Info.GetXmlProperty("genxml/textbox/licensekey"); }
            set { Info.SetXmlProperty("genxml/textbox/licensekey", value); }
        }
        public string EncryptKey
        {
            get { return Info.GetXmlProperty("genxml/textbox/encryptkey"); }
            set { Info.SetXmlProperty("genxml/textbox/encryptkey", value); }
        }
        public int SystemId
        {
            get { return Info.ItemID; }
        }
        public string SystemRelPath
        {
            get { return Info.GetXmlProperty("genxml/textbox/systemrelpath"); }
        }
        public string SystemMapPath
        {
            get { return DNNrocketUtils.MapPath(Info.GetXmlProperty("genxml/textbox/systemrelpath")); }
        }
        public string DefaultInterface
        {
            get { return Info.GetXmlProperty("genxml/textbox/defaultinterface"); }
        }
        public string FtpRoot
        {
            get { return Info.GetXmlProperty("genxml/textbox/ftproot"); }
        }        

    }
}
