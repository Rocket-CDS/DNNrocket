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

    }
}
