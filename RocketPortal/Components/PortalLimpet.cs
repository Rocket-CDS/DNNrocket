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

namespace RocketPortal.Components
{
    public class PortalLimpet
    {
        private const string _entityTypeCode = "PORTAL";
        private const string _systemkey = "rocketportal";
        private DNNrocketController _objCtrl;
        private string _guidKey;
        private int _portalId;
        private string _cacheKey;
        public PortalLimpet(int portalId)
        {
            Record = new SimplisityRecord();
            _portalId = portalId;
            _guidKey = PortalUtils.SiteGuid(portalId);
            _objCtrl = new DNNrocketController();

            _cacheKey = "Portal" + portalId;

            Record = (SimplisityRecord)CacheUtils.GetCache(_cacheKey);
            if (Record == null)
            {
                var uInfo = _objCtrl.GetByGuidKey(portalId, -1, _entityTypeCode, _guidKey, "");
                if (uInfo != null) Record = _objCtrl.GetRecord(uInfo.ItemID);
                if (Record == null || Record.ItemID <= 0)
                {
                    Record = new SimplisityInfo();
                    Record.PortalId = _portalId;
                    Record.ModuleId = -1;
                    Record.TypeCode = _entityTypeCode;
                    Record.GUIDKey = _guidKey;
                }
                else
                {
                    CacheUtils.SetCache(_cacheKey, Record);
                }
            }

            // add systems
            SystemDataList = new SystemLimpetList();
        }

        public void Save(SimplisityInfo info)
        { 
            Record.XMLData = info.XMLData;
            if (EngineUrl != "") PortalUtils.AddPortalAlias(_portalId, EngineUrl);
            Update();
        }
        public void Update()
        {
            Validate();
            _objCtrl.SaveRecord(Record);
            if (UserId <= 0) UserId = UserUtils.GetCurrentUserId();
            CacheUtils.SetCache(_cacheKey,Record);
        }
        public void Delete()
        {
            _objCtrl.Delete(Record.ItemID);

            // remove all portal records.
            var l = _objCtrl.GetList(_portalId, -1, "","","","",0,0,0,0);
            foreach (var r in l)
            {
                _objCtrl.Delete(r.ItemID);
            }
            CacheUtils.RemoveCache(_cacheKey);
        }

        public void Validate()
        {
            var dpa  = PortalUtils.DefaultPortalAlias(PortalId);
            if (dpa != EngineUrl && dpa != "")
            {
                EngineUrl = dpa;
                Update();
            }
        }
        public string EntityTypeCode { get { return _entityTypeCode; } }


        #region "setting"
        public string GetPortalSetting(int idx)
        {
            var rtnInfo = Record.GetRecordListItem("settingsdata", idx);
            if (rtnInfo == null) return "";
            return rtnInfo.GetXmlProperty("genxml/textbox/value");
        }
        public string GetPortalSettingByKey(string key)
        {
            var rtnInfo = Record.GetRecordListItem("settingsdata", "genxml/textbox/key", key);
            if (rtnInfo == null) return "";
            return rtnInfo.GetXmlProperty("genxml/textbox/value");
        }
        public List<SimplisityRecord> GetPortalSettings()
        {
            return Record.GetRecordList("settingsdata");
        }
        

        #endregion

        public SimplisityRecord Record { get; set; }
        public int PortalId { get { return Record.PortalId; } }
        public string Name { get { return Record.GetXmlProperty("genxml/textbox/name"); } }
        public string Protocol { get { var rtn = Record.GetXmlProperty("genxml/select/protocol"); if (rtn == "") rtn = "https://"; return rtn; } }
        public string EngineUrl { get { return Record.GetXmlProperty("genxml/textbox/engineurl"); } set { Record.SetXmlProperty("genxml/textbox/engineurl", value); } }
        public bool Exists { get { if (Record.ItemID > 0) return true; else return false; } }
        public DateTime LastSchedulerTime
        {
            get
            {
                if (Record.GetXmlProperty("genxml/lastschedulertime") != "")
                    return Record.GetXmlPropertyDate("genxml/lastschedulertime");
                else
                    return DateTime.Now.AddDays(-10);
            }
            set { Record.SetXmlProperty("genxml/lastschedulertime", value.ToString(), TypeCode.DateTime); }
        }
        public int SchedulerRunHours
        {
            get
            {
                var rtn = Record.GetXmlPropertyInt("genxml/schedulerrunhours");
                if (Record.GetXmlProperty("genxml/schedulerrunhours") == "") rtn = 24;
                return rtn;
            }
        }
        public string SiteKey { get { return Record.GUIDKey; } set { Record.GUIDKey = value; } }
        public Dictionary<string, string> Managers { get; private set; }
        public SystemLimpetList SystemDataList { get; private set; }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/securitykey"); } }
        public bool EmailActive { get { return Record.GetXmlPropertyBool("genxml/emailon"); } }
        public bool DebugMode { get { return Record.GetXmlPropertyBool("genxml/debugmode"); } }
        public int UserId { get { return Record.UserId; } private set { Record.UserId = value; } }

    }
}
