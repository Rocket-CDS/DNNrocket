using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class LicenseData
    {
        private string _encrypteddomain;
        public LicenseData(SimplisityRecord sRec)
        {
            PortalId = sRec.PortalId;
            SystemKey = sRec.GetXmlProperty("genxml/hidden/systemkey");
            LoadLicense();
        }
        public LicenseData(int portalId, string systemKey)
        {
            PortalId = portalId;
            SystemKey = systemKey;
            LoadLicense();
        }

        public bool IsLicensed { get; private set; }
        public SimplisityRecord Record { get; set; }
        public string LicenseKey { get; private set; }
        public string DomainUrl { get; private set; }
        public DateTime ExpireDate { get; private set; }
        public string SystemKey { get; private set; }
        public int PortalId { get; private set; }

        public void CheckOnLineLicense()
        {
            //var uri = "https://license.rocket-templates.ovh/";
            //WebClient client = new WebClient();
            //var rtnString = client.DownloadString(uri);
            //return Convert.ToBoolean(rtnString);

            // return the SimplsistyRecord for the license.
            // set "genxml/encryptedkey" and LicenseKey server side and save to local DB on return.

            IsLicensed = true;

        }
        public void Update()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Update(Record);
        }
        private void LoadLicense()
        {
            var guidKey = SystemKey + "*" + PortalId;
            var objCtrl = new DNNrocketController();
            Record = objCtrl.GetRecordByGuidKey(PortalId, -1, "LICENCE", guidKey);
            if (Record == null)
            {
                Record = new SimplisityRecord();
                Record.ItemID = -1;
                Record.PortalId = PortalId;
                Record.TypeCode = "LICENSE";

                Record.SetXmlProperty("genxml/hidden/license", LicenseKey);
                Record.SetXmlProperty("genxml/hidden/domainurl", DomainUrl);
                Record.SetXmlProperty("genxml/hidden/encryptedkey", "FREE");
                Record.SetXmlProperty("genxml/hidden/fail", "0");
                ExpireDate = DateTime.Now.Date.AddDays(-1);
                Record.SetXmlProperty("genxml/textbox/expiredate", ExpireDate.ToString("O"), TypeCode.DateTime);
                var checkdate = DateTime.Now.Date.AddDays(-1);
                Record.SetXmlProperty("genxml/hidden/checkdate", checkdate.ToString("O"), TypeCode.DateTime);
                Record.SetXmlProperty("genxml/hidden/validate", "30");
                Record.GUIDKey = guidKey;
                Record.ItemID = objCtrl.Update(Record);

            }

            if (GeneralUtils.IsDate(Record.GetXmlPropertyRaw("genxml/hidden/checkdate"), "en-US") && Convert.ToDateTime(Record.GetXmlPropertyRaw("genxml/hidden/checkdate")) < DateTime.Now.Date)
            {
                CheckOnLineLicense();
            }

            IsLicensed = false;
            if (GeneralUtils.IsDate(Record.GetXmlPropertyRaw("genxml/hidden/checkdate"), "en-US") && !String.IsNullOrEmpty(LicenseKey) && !String.IsNullOrEmpty(DomainUrl))
            {
                var encryptdata = DomainUrl + "&" + Record.GetXmlPropertyRaw("genxml/textbox/expiredate");
                _encrypteddomain = DNNrocketUtils.Encrypt(encryptdata, LicenseKey);
                if (_encrypteddomain == Record.GetXmlProperty("genxml/hidden/encryptedkey") && Convert.ToDateTime(Record.GetXmlPropertyRaw("genxml/textbox/expiredate")) >= DateTime.Now.Date) IsLicensed = true;
            }

        }

    }
}
