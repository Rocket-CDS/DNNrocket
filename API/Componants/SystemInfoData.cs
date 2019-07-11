using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI
{
    public class SystemInfoData
    {
        public SystemInfoData(SimplisityInfo systemInfo)
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
            var l = Info.GetList("interfacedata");
            foreach (var r in l)
            {
                var rocketInterface = new DNNrocketInterface(r);
                if (rocketInterface.IsProvider && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    EventList.Add(rocketInterface);
                }
            }
        }

        public SimplisityInfo Info { get; }
        public List<DNNrocketInterface> EventList { get; }
        public bool Exists { get; }

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



    }
}
