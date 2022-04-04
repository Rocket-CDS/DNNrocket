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
                SecurityKey = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString(1);
            }
            if (SecurityKeyEdit == "")
            {
                SecurityKeyEdit = GeneralUtils.GetGuidKey() + GeneralUtils.GetUniqueString(1);
            }
            // add systems
            SystemDataList = new SystemLimpetList();
        }

        public int Save(SimplisityInfo info)
        {
            Record.XMLData = info.XMLData;
            return Update();
        }
        private void UpdateDefaultLanguage(string cultureCode)
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

            if (EngineUrl != "")
            {
                // Portal Alias
                var requiredPortalAlias = new Dictionary<string, string>();
                requiredPortalAlias.Add(EngineUrl,"");

                var defaultLanguage = Record.GetXmlProperty("genxml/select/defaultlanguage");
                var defaultBackupLanguage = "";
                var nodList = Record.XMLDoc.SelectNodes("genxml/radio/culturecodes/chk");
                foreach (XmlNode nod in nodList)
                {
                    var cultureCode = nod.Attributes["data"].Value;
                    if (nod.Attributes["value"].Value.ToLower() == "true")
                    {
                        requiredPortalAlias.Add(EngineUrl + "/" + cultureCode.ToLower(), cultureCode);
                        defaultBackupLanguage = cultureCode;
                    }
                    else
                    {
                        PortalUtils.RemoveLanguage(_portalId, cultureCode);
                    }
                }

                if (requiredPortalAlias.Count == 1)
                {
                    // no langauge selected, set default language
                    XmlElement formData = (XmlElement)Record.XMLDoc.SelectSingleNode("genxml/radio/culturecodes/chk[@data='" + defaultLanguage + "']");
                    if (formData != null) formData.SetAttribute("value", "true");
                }

                if (requiredPortalAlias.Count == 2)
                {
                    // we only have 1 language, only set 1 neutral portalalias
                    requiredPortalAlias.Remove(requiredPortalAlias.Last().Key);
                }

                var pAlias = PortalUtils.GetPortalAliases(_portalId);
                foreach (var pa in pAlias)
                {
                    if (!requiredPortalAlias.ContainsKey(pa)) PortalUtils.DeletePortalAlias(_portalId, pa);
                }

                pAlias = PortalUtils.GetPortalAliases(_portalId);
                if (pAlias.Count == 0) PortalUtils.AddPortalAlias(_portalId, EngineUrl, ""); // incorrectly removed.

                foreach (var pa in requiredPortalAlias)
                {
                    if (!pAlias.Contains(pa.Key))
                    {
                        if (pa.Value != "") PortalUtils.AddLanguage(_portalId, pa.Value);
                        PortalUtils.AddPortalAlias(_portalId, pa.Key, pa.Value);
                    }
                    PortalUtils.SetPrimaryPortalAlias(_portalId, pa.Key);
                }
                if (requiredPortalAlias.ContainsValue(defaultLanguage)) 
                    UpdateDefaultLanguage(defaultLanguage);
                else
                    UpdateDefaultLanguage(defaultBackupLanguage);
            }

        }
        public void AddLangauge(string cultureCode)
        {
            PortalUtils.AddLanguage(PortalId, cultureCode);
        }
        public void RemoveLangauge(string cultureCode)
        {
            PortalUtils.RemoveLanguage(PortalId, cultureCode);
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
        public string Name { get { return Record.GetXmlProperty("genxml/textbox/name"); } set { Record.SetXmlProperty("genxml/textbox/name", value); } }
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
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/config/securitykey"); } set { Record.SetXmlProperty("genxml/config/securitykey", value); } }
        public string SecurityKeyEdit { get { return Record.GetXmlProperty("genxml/config/securitykeyedit"); } set { Record.SetXmlProperty("genxml/config/securitykeyedit", value); } }
        public bool EmailActive { get { return Record.GetXmlPropertyBool("genxml/config/emailon"); } }
        public int UserId { get { return Record.UserId; } private set { Record.UserId = value; } }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/radio/systemkey"); } }
        public string ColorAdminTheme { get { var rtn = Record.GetXmlProperty("genxml/select/colortheme"); if (rtn == "") rtn = "grey-theme.css"; return rtn; } set { Record.SetXmlProperty("genxml/select/colortheme", value); } }

    }
}
