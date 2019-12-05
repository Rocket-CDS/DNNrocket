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
        private const string _tableName = "DNNrocket";
        private DNNrocketController _objCtrl;
        private int _licenseId;
        private string _guidkey;

        public LicenseData(int licenseId)
        {
            _guidkey = "";
            _licenseId = licenseId;
            LoadData();
        }
        public LicenseData(string systemkey, string sitekey)
        {
            _guidkey = systemkey + "*" + sitekey;
            _licenseId = -1;
            LoadData();
            SystemKey = systemkey;
            SiteKey = sitekey;
        }

        public void Save(SimplisityInfo _postInfo)
        {
            if (Exists)
            {
                Record.GUIDKey = SystemKey + "*" + SiteKey;

                Record.SetXmlProperty("genxml/textbox/expiredate", _postInfo.GetXmlPropertyRaw("genxml/textbox/expiredate"), TypeCode.DateTime);
                DomainUrl = _postInfo.GetXmlProperty("genxml/textbox/domainurl");
                SystemKey = _postInfo.GetXmlProperty("genxml/select/systemkey");
                SiteKey = _postInfo.GetXmlProperty("genxml/textbox/sitekey");

                Update();

            }
        }
        public void Update()
        {
            if (Exists) _objCtrl.Update(Record, _tableName);
        }
        public void Delete()
        {
            if (Exists) _objCtrl.Delete(LicenseId, _tableName);
        }

        private void LoadData()
        {
            Exists = false;
            _objCtrl = new DNNrocketController();

            if (_guidkey == "")
            {
                Record = _objCtrl.GetRecord(_licenseId, _tableName);
            }
            else
            {
                Record = _objCtrl.GetByGuidKey(-1, -1, "LICENSE", _guidkey, "", _tableName);
            }

            if (Record != null)
            {
                Exists = true;

                DomainUrl = Record.GetXmlProperty("genxml/textbox/domainurl");
                SystemKey = Record.GetXmlProperty("genxml/select/systemkey");
                SiteKey = Record.GetXmlProperty("genxml/textbox/sitekey");
                var expireDate = Record.GetXmlPropertyRaw("genxml/textbox/expiredate");
                if (GeneralUtils.IsDate(expireDate, "en-US"))
                {
                    ExpireDate = Convert.ToDateTime(Record.GetXmlPropertyRaw("genxml/textbox/expiredate"));
                }
                else
                {
                    ExpireDate = DateTime.Now.Date.AddDays(-1);
                }
                ClientId = Record.UserId;
            }
            else
            {
                Record = new SimplisityRecord();
                ExpireDate = DateTime.Now.Date.AddDays(-1);
            }
        }

        public void CreateNew(int clientid, int licenseDays = 90, bool isTrial = false)
        {
            if (!Exists)
            {
                Exists = true;
                IsTrial = isTrial;
                ClientId = clientid;
                ExpireDate = DateTime.Now.Date.AddDays(licenseDays);
                Record.GUIDKey = _guidkey;
                Record.TypeCode = "LICENSE";
                Record.Lang = "";
                GenerateCertificateKey(); // updates also.
            }
        }

        public bool ValidateCertificateKey(string sitekey)
        {
            if (CertificateKey.Length > 15)
            {
                if (SystemKey == "") return false;
                if (SiteKey == "") return false;
                if (sitekey == "") return false;
                var licenseKey = CertificateKey.Substring(0, 12);
                var encryptedKey = CertificateKey.Substring(12);
                var decryptedKey = GeneralUtils.Decrypt(licenseKey, encryptedKey);
                if (decryptedKey.StartsWith(SystemKey) && decryptedKey.EndsWith(sitekey)) return true;
            }
            return false;
        }

        public void GenerateCertificateKey()
        {
            CertificateKey = "";
            if (SiteKey != "")
            {
                var licenseKey = GeneralUtils.GetUniqueKey(12);
                CertificateKey = licenseKey + GeneralUtils.Encrypt(licenseKey, SystemKey + SiteKey);
            }
            else
            {
                CertificateKey = "ERROR: Invalid SiteKey";
            }
            Update();
        }
        public string CertificateKey { get { return Record.GetXmlProperty("genxml/hidden/certificatekey"); } set { Record.SetXmlProperty("genxml/hidden/certificatekey", value); } }
        public string DomainUrl { get { return Record.GetXmlProperty("genxml/textbox/domainurl"); } set { Record.SetXmlProperty("genxml/textbox/domainurl", value); } }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/select/systemkey"); } set { Record.SetXmlProperty("genxml/select/systemkey", value); } }
        public string SiteKey { get { return Record.GetXmlProperty("genxml/textbox/sitekey"); } set { Record.SetXmlProperty("genxml/textbox/sitekey", value); } }
        public DateTime ExpireDate
        {
            get
            {
                var expireDate = Record.GetXmlPropertyRaw("genxml/textbox/expiredate");
                if (GeneralUtils.IsDate(expireDate, "en-US"))
                    return Convert.ToDateTime(expireDate);
                else
                    return DateTime.Now.Date.AddDays(-1);
            }
            set { Record.SetXmlProperty("genxml/textbox/expiredate", value.ToString("O"), TypeCode.DateTime); }
        }
        public bool Exists { get; set; }
        public bool IsTrial { get; set; }
        public int LicenseId { get { return Record.ItemID; } }
        public int ClientId { get { return Record.UserId; } set { Record.UserId = value; } }
        public SimplisityRecord Record { get; set; }

    }
}
