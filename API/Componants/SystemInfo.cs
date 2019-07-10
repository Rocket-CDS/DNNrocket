using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI
{
    public class SystemInfo
    {
        public SystemInfo(SimplisityInfo systemInfo)
        {
            if (systemInfo == null)
            {
                Exists = false;
            }
            else
            {
                Exists = true;
            }
            Info = systemInfo;
        }

        public SimplisityInfo Info { get; }

        public bool Exists { get; }

        public bool SystemKey
        {
            get { return Info.GetXmlPropertyBool("genxml/textbox/ctrlkey"); }
            set { Info.SetXmlProperty("genxml/textbox/ctrlkey", value.ToString()); }
        }
        public bool SystemName
        {
            get { return Info.GetXmlPropertyBool("genxml/textbox/systemname"); }
            set { Info.SetXmlProperty("genxml/textbox/systemname", value.ToString()); }
        }
        public bool ApiUrl
        {
            get { return Info.GetXmlPropertyBool("genxml/textbox/apiurl"); }
            set { Info.SetXmlProperty("genxml/textbox/apiurl", value.ToString()); }
        }
        public bool AdminUrl
        {
            get { return Info.GetXmlPropertyBool("genxml/textbox/adminurl"); }
            set { Info.SetXmlProperty("genxml/textbox/adminurl", value.ToString()); }
        }
        public bool DebugMode
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/debugmode"); }
            set { Info.SetXmlProperty("genxml/checkbox/debugmode", value.ToString()); }
        }
        public bool LicenseKey
        {
            get { return Info.GetXmlPropertyBool("genxml/textbox/licensekey"); }
            set { Info.SetXmlProperty("genxml/textbox/licensekey", value.ToString()); }
        }
        public bool EncryptKey
        {
            get { return Info.GetXmlPropertyBool("genxml/textbox/encryptkey"); }
            set { Info.SetXmlProperty("genxml/textbox/encryptkey", value.ToString()); }
        }



    }
}
