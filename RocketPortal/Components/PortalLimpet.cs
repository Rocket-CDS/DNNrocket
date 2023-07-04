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
    public class PortalLimpet
    {
        private const string _entityTypeCode = "PORTAL";
        private const string _systemkey = "rocketportal";
        private DNNrocketController _objCtrl;
        private int _portalId;
        private string _cacheKey;
        public PortalLimpet(int portalId)
        {
            Record = new SimplisityRecord();
            _portalId = portalId;
            _objCtrl = new DNNrocketController();

            _cacheKey = "Portal" + portalId;

            Record = (SimplisityRecord)CacheUtils.GetCache(_cacheKey, portalId.ToString());
            if (Record == null)
            {
                //var uInfo = _objCtrl.GetByGuidKey(portalId, -1, _entityTypeCode, _guidKey, "");
                var uInfo = _objCtrl.GetByType(portalId, -1, _entityTypeCode);

                if (uInfo != null) Record = _objCtrl.GetRecord(uInfo.ItemID);
                if (Record == null || Record.ItemID <= 0)
                {
                    Record = new SimplisityInfo();
                    Record.PortalId = _portalId;
                    Record.ModuleId = -1;
                    Record.TypeCode = _entityTypeCode;
                    Record.SetXmlProperty("genxml/radio/culturecodes/chk", "");
                    Record.SetXmlProperty("genxml/radio/culturecodes/chk/@value", "true");
                    Record.SetXmlProperty("genxml/radio/culturecodes/chk/@data", DNNrocketUtils.GetCurrentCulture());
                }
            }

            if (SecurityKey == "")
            {
                SecurityKey = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString();
            }
            if (SecurityKeyEdit == "")
            {
                SecurityKeyEdit = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString();
            }
            // add systems
            SystemDataList = new SystemLimpetList();
        }

        private void ReplaceInfoFields(SimplisityInfo postInfo, string xpathListSelect)
        {
            var textList = postInfo.XMLDoc.SelectNodes(xpathListSelect);
            if (textList != null)
            {
                foreach (XmlNode nod in textList)
                {
                    Record.SetXmlProperty(xpathListSelect.Replace("*", "") + nod.Name, nod.InnerText);
                }
            }
        }

        public int Save(SimplisityInfo info)
        {
            ReplaceInfoFields(info, "genxml/config/*");
            ReplaceInfoFields(info, "genxml/select/*");
            ReplaceInfoFields(info, "genxml/textbox/*");
            return Update();
        }
        public string DefaultLanguage()
        {
            return PortalUtils.GetDefaultLanguage(PortalId);
        }
        public void UpdateDefaultLanguage(string cultureCode)
        {
            if (cultureCode != "")
            {
                var validlangauge = false;
                var cultureList = DNNrocketUtils.GetCultureCodeList(PortalId);
                foreach (var cc in cultureList)
                {
                    if (cc == "") PortalUtils.RemoveLanguage(PortalId, ""); // invalid
                    if (cultureCode == cc) validlangauge = true;
                }
                if (validlangauge) 
                    PortalUtils.SetDefaultLanguage(PortalId, cultureCode);
                else
                {
                    if (cultureList.Count > 0) PortalUtils.SetDefaultLanguage(PortalId, cultureList[0]);
                }
            }
        }
        public int Update()
        {
            Validate();
            if (UserId <= 0) UserId = UserUtils.GetCurrentUserId();
            Record = _objCtrl.SaveRecord(Record);
            CacheUtils.SetCache(_cacheKey, Record, _portalId.ToString());
            return Record.ItemID;
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
            CacheUtils.RemoveCache(_cacheKey, _portalId.ToString());
        }

        public void Validate()
        {
            if (EngineUrl == "")
            {
                var dpa = PortalUtils.DefaultPortalAlias(PortalId);
                EngineUrl = dpa;
            }

            EngineUrl = EngineUrl.ToLower(); // only allow lowercase
        }
        private void UpdatePortalAlias()
        {
            if (EngineUrl != "")
            {
                var cultureList = DNNrocketUtils.GetCultureCodeList(PortalId);

                // delete ALL portal Alias with this Engine URL.  (Overkill, but tracking which to delete is awkward.  It needs to be on url and culture.)
                var pAlias = PortalUtils.GetPortalAliasesWithCultureCode(_portalId);
                foreach (var pa in pAlias)
                {
                    if (pa.Key.StartsWith(EngineUrl)) PortalUtils.DeletePortalAlias(_portalId, pa.Key);
                }

                // Add root domain url
                PortalUtils.AddPortalAlias(_portalId, EngineUrl, "");

                if (cultureList.Count > 1) // use root domain for only 1 langauge.
                {
                    foreach (var cultureCode in cultureList)
                    {
                        PortalUtils.AddPortalAlias(_portalId, EngineUrl + "/" + cultureCode.ToLower(), cultureCode);
                    }
                }

                //Always set this EngineUrl to primary
                pAlias = PortalUtils.GetPortalAliasesWithCultureCode(_portalId);
                foreach (var pa in pAlias)
                {
                    if (pa.Key.StartsWith(EngineUrl)) PortalUtils.SetPrimaryPortalAlias(_portalId, pa.Key);
                }
            }
        }
        public void AddLanguage(string cultureCode)
        {
            PortalUtils.AddLanguage(PortalId, cultureCode);
            // no langauge selected, set default language
            XmlElement formData = (XmlElement)Record.XMLDoc.SelectSingleNode("genxml/radio/culturecodes/chk[@data='" + cultureCode + "']");
            if (formData != null) formData.SetAttribute("value", "true");
            Update();
            UpdatePortalAlias();
        }
        public void RemoveLanguage(string cultureCode)
        {
            var cultureList = DNNrocketUtils.GetCultureCodeList(PortalId);
            if (cultureList.Count > 1 && cultureCode != DefaultLanguage())
            {
                PortalUtils.RemoveLanguage(PortalId, cultureCode);
                // no langauge selected, set default language
                XmlElement formData = (XmlElement)Record.XMLDoc.SelectSingleNode("genxml/radio/culturecodes/chk[@data='" + cultureCode + "']");
                if (formData != null) formData.SetAttribute("value", "false");
                Update();
                UpdatePortalAlias();
            }
        }

        /// <summary>
        /// This is used to create a string which is passed to any remote module, to give minimum setting.
        /// </summary>
        /// <returns></returns>
        public string RemoteBase64Params()
        {
            var remoteParams = new RemoteParams();
            remoteParams.EngineURL = EngineUrlWithProtocol;
            remoteParams.SecurityKey = SecurityKey;
            remoteParams.SecurityKeyEdit = SecurityKeyEdit;
            return remoteParams.RecordItemBase64;
        }
        public bool IsValidRemote(string securityKey)
        {
            if (SecurityKey == securityKey) return true;
            return false;
        }
        public bool IsValidRemoteEdit(string securityKeyEdit)
        {
            if (SecurityKeyEdit == securityKeyEdit) return true;
            return false;
        }

        public List<SystemLimpet> GetSystems()
        {
            var rtn = new List<SystemLimpet>();
            var l = SystemDataList.GetSystemActiveList();
            foreach (var s in l)
            {
                if (Record.GetXmlPropertyBool("genxml/systems/" + s.SystemKey)) rtn.Add(s);
            }
            return rtn;
        }
        public bool AccessCodeCheck(string accessCode, string accessPassword, string clientcode)
        {
            var gData = new SystemGlobalData();
            if (gData.AccessCode == accessCode)
            {
                var securityfailcount = (string)CacheUtils.GetCache(accessCode + "count", "security");
                if (securityfailcount == null) securityfailcount = "0";
                if (Convert.ToInt32(securityfailcount) > 9) return false;
                if (gData.ValidClient(clientcode) && gData.AccessPassword == accessPassword) return true;
                CacheUtils.SetCache(accessCode + "count", (Convert.ToInt32(securityfailcount) + 1).ToString(), "security");
                CacheUtils.SetCache("securitylock", "true", "security");
            }
            return false;
        }
        public bool SecurityKeyCheck(string securityKey, string securityKetEdit)
        {
            if (SecurityKey == securityKey)
            {
                var securityfailcount = (string)CacheUtils.GetCache(securityKey + "count", "security");
                if (securityfailcount == null) securityfailcount = "0";
                if (Convert.ToInt32(securityfailcount) > 9) return false;
                if (SecurityKeyEdit == securityKetEdit) return true;
                CacheUtils.SetCache(securityKey + "count", (Convert.ToInt32(securityfailcount) + 1).ToString(), "security");
                CacheUtils.SetCache("securitylock", "true", "security");
            }
            return false;
        }
        public void ResetSecurity()
        {
            CacheUtils.ClearAllCache("security");
        }
        public void ResetCodes()
        {
            SecurityKey = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString();
            SecurityKeyEdit = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString();
            ResetSecurity(); // also updates
        }

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

        public string EntityTypeCode { get { return _entityTypeCode; } }
        public SimplisityRecord Record { get; set; }
        public int PortalId { get { return Record.PortalId; } }
        public string Protocol { get { var rtn = Record.GetXmlProperty("genxml/select/protocol"); if (rtn == "") rtn = "https://"; return rtn; } }
        public string EngineUrl { get { return Record.GetXmlProperty("genxml/textbox/engineurl"); } set { Record.SetXmlProperty("genxml/textbox/engineurl", value); } }
        public string Name { get { return PortalUtils.GetPortalName(PortalId); } }
        public string EngineUrlWithProtocol { get { return Protocol + EngineUrl; } }
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
        public string SiteKey { get { return Record.GUIDKey; } set { Record.GUIDKey = value; } }
        public Dictionary<string, string> Managers { get; private set; }
        public SystemLimpetList SystemDataList { get; private set; }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/config/securitykey"); } set { Record.SetXmlProperty("genxml/config/securitykey", value); } }
        public string SecurityKeyEdit { get { return Record.GetXmlProperty("genxml/config/securitykeyedit"); } set { Record.SetXmlProperty("genxml/config/securitykeyedit", value); } }
        public bool EmailActive { get { return Record.GetXmlPropertyBool("genxml/config/emailon"); } }
        public int UserId { get { return Record.UserId; } private set { Record.UserId = value; } }
        public string ColorAdminTheme { get { var rtn = Record.GetXmlProperty("genxml/select/colortheme"); if (rtn == "") rtn = "grey-theme.css"; return rtn; } set { Record.SetXmlProperty("genxml/select/colortheme", value); } }

    }
}
