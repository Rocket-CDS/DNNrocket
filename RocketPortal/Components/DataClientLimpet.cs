using DNNrocketAPI;
using DNNrocketAPI.Components;
using Rocket.AppThemes.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RocketPortal.Components
{
    public class DataClientLimpet
    {
        private const string _entityTypeCode = "DATACLIENT";
        private const string _systemkey = "rocketportal";
        private DNNrocketController _objCtrl;
        private int _portalId;
        private string _cacheKey;
        public DataClientLimpet(int portalId, string siteKey)
        {
            ServiceData = new PortalLimpet(portalId);
            Record = new SimplisityRecord();
            SiteKey = siteKey;
            _portalId = portalId;
            _objCtrl = new DNNrocketController();

            _cacheKey = _entityTypeCode + siteKey;

            Record = (SimplisityRecord)CacheUtils.GetCache(_cacheKey, portalId.ToString());
            if (Record == null)
            {
                var dcInfo = _objCtrl.GetByGuidKey(portalId, -1, _entityTypeCode, siteKey, "");
                if (dcInfo != null) Record = _objCtrl.GetRecord(dcInfo.ItemID);
                if (Record == null || Record.ItemID <= 0)
                {
                    Record = new SimplisityInfo();
                    Record.PortalId = _portalId;
                    Record.ModuleId = -1;
                    Record.TypeCode = _entityTypeCode;
                    Record.GUIDKey = siteKey;
                    Record.ParentItemId = ServiceData.Record.ItemID;
                    Record.UserId = ServiceData.UserId;
                    Active = true;
                }
            }
        }
        public int Update()
        {
            Record = _objCtrl.SaveRecord(Record);
            CacheUtils.SetCache(_cacheKey, Record, _portalId.ToString());
            return Record.ItemID;
        }
        public void Delete()
        {
            _objCtrl.Delete(Record.ItemID);
            CacheUtils.RemoveCache(_cacheKey, _portalId.ToString());
        }
        public bool AccessCodeCheck(string accessCode, string accessPassword)
        {
            var accessfailcountDate = Record.GetXmlPropertyDate("genxml/accessfaildatetime");
            if (accessfailcountDate < DateTime.Now)
            {
                Record.GetXmlProperty("genxml/accessfailcount", "0");
                Update();
            }
            var accessfailCount = Record.GetXmlPropertyInt("genxml/accessfailcount");
            if (accessfailCount > 3) return false;

            var gData = new SystemGlobalData();
            if (gData.AccessCode == accessCode)
            {
                if (gData.AccessPassword == accessPassword) return true;
                Record.SetXmlProperty("genxml/accessfailcount", (accessfailCount + 1).ToString());
                Record.SetXmlProperty("genxml/accessfaildatetime", DateTime.Now.AddMinutes(10).ToString("O"), TypeCode.DateTime);
                Update();
            }
            return false;
        }
        public bool SecurityKeyCheck(int portalId, string securityKey, string securityKetEdit)
        {
            var accessfailcountDate = Record.GetXmlPropertyDate("genxml/securityfaildatetime");
            if (accessfailcountDate < DateTime.Now)
            {
                Record.GetXmlProperty("genxml/securityfailcount", "0");
                Update();
            }
            var accessfailCount = Record.GetXmlPropertyInt("genxml/securityfailcount");
            if (accessfailCount > 3) return false;

            var portalData = new PortalLimpet(portalId);
            if (portalData.SecurityKey == securityKey)
            {
                if (portalData.SecurityKeyEdit == securityKetEdit) return true;
                Record.SetXmlProperty("genxml/securityfailcount", (accessfailCount + 1).ToString());
                Record.SetXmlProperty("genxml/securityfaildatetime", DateTime.Now.AddMinutes(10).ToString("O"), TypeCode.DateTime);
                Update();
            }

            return false;
        }
        public void ResetSecurity()
        {
            Record.SetXmlProperty("genxml/accessfailcount", "0");
            Record.SetXmlProperty("genxml/accessfaildatetime", DateTime.Now.AddMinutes(-1).ToString("O"), TypeCode.DateTime);
            Record.SetXmlProperty("genxml/securityfailcount", "0");
            Record.SetXmlProperty("genxml/securityfaildatetime", DateTime.Now.AddMinutes(-1).ToString("O"), TypeCode.DateTime);
            Update();
        }
        public string EntityTypeCode { get { return _entityTypeCode; } }
        public SimplisityRecord Record { get; set; }
        public int PortalId { get { return Record.PortalId; } }
        public int ServiceId { get { return Record.ParentItemId; } set { Record.ParentItemId = value; } }
        public bool Active { get { return Record.GetXmlPropertyBool("genxml/checkbox/active"); } set { Record.SetXmlProperty("genxml/checkbox/active", value.ToString()); } }
        public bool Exists { get { if (Record.ItemID > 0) return true; else return false; } }
        public string SiteKey { get { return Record.GUIDKey; } set { Record.GUIDKey = value; } }
        public int UserId { get { return Record.UserId; } private set { Record.UserId = value; } }
        public PortalLimpet ServiceData { set; get; }
        public string UrlWithProtocol { get { return Record.GetXmlProperty("genxml/textbox/url"); } set { Record.SetXmlProperty("genxml/textbox/url", value); } }
        public string UrlApi { get { return Record.GetXmlProperty("genxml/textbox/urlapi"); } set { Record.SetXmlProperty("genxml/textbox/urlapi", value); } }
        public DateTime LastAccessDate { get { return Record.GetXmlPropertyDate("genxml/remote/lastaccessdate"); } set { Record.SetXmlProperty("genxml/remote/lastaccessdate", value.ToString("O"), TypeCode.DateTime); } }

    }
}
