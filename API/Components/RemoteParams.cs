using DNNrocketAPI;
using DotNetNuke.Entities.Modules;
using RazorEngine;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// Used to call remote engine and returns data
    /// </summary>
    public class RemoteParams
    {
        public RemoteParams(string systemKey)
        {
            Record = new SimplisityRecord();
            SystemKey = systemKey;
        }

        #region "Form UrlParams"
        public void RemoveAllUrlParam()
        {
            if (Record != null)
            {
                Record.RemoveXmlNode("genxml/urlparams");
            }
        }
        public void AddUrlParam(string key, string value)
        {
            if (Record != null)
            {
                Record.SetXmlProperty("genxml/urlparams/" + key, value);
                // get current url for remoteParam
                if (key.ToLower() == "tabid") PageUrl = DNNrocketUtils.NavigateURL(Convert.ToInt32(value), new string[0]);
            }
        }
        public string GetUrlParam(string key)
        {
            if (Record != null)
            {
                return Record.GetXmlProperty("genxml/urlparams/" + key);
            }
            return "";
        }
        public Dictionary<string, string> GetUrlParams()
        {
            var rtn = new Dictionary<string, string>();
            if (Record != null)
            {
                var l = Record.XMLDoc.SelectNodes("genxml/urlparams/*");
                if (l != null)
                {
                    foreach (XmlNode n in l)
                    {
                        rtn.Add(n.Name, n.InnerText);
                    }
                }
            }
            return rtn;
        }

        #endregion

        #region "Form Fields"
        public Dictionary<string, string> GetFormParams()
        {
            var rtn = new Dictionary<string, string>();
            if (Record != null)
            {
                var l = Record.XMLDoc.SelectNodes("genxml/form/*");
                if (l != null)
                {
                    foreach (XmlNode n in l)
                    {
                        rtn.Add(n.Name, n.InnerText);
                    }
                }
            }
            return rtn;
        }

        public void RemoveAllFormParam()
        {
            if (Record != null)
            {
                Record.RemoveXmlNode("genxml/form");
            }
        }

        public void AddFormParam(string key, string value)
        {
            if (Record != null)
            {
                Record.SetXmlProperty("genxml/form/" + key, value);
            }
        }
        public string GetFormParam(string key)
        {
            if (Record != null)
            {
                return Record.GetXmlProperty("genxml/form/" + key);
            }
            return "";
        }

        #endregion

        #region "Form Settings"
        public void RemoveAllSettings()
        {
            if (Record != null)
            {
                Record.RemoveXmlNode("genxml/settingsdata");
            }
        }
        public void AddSetting()
        {
            if (Record != null)
            {
                Record.AddListItem("settingsdata");
            }
        }
        public string GetSetting(string keyvalue)
        {
            if (Record != null)
            {
                var rec = Record.GetRecordListItem("settingsdata", "genxml/settings/key", keyvalue);
                if (rec == null) return "";
                return rec.GetXmlProperty("genxml/settings/value");
            }
            return "";
        }
        public void RemoveSetting(int idx)
        {
            if (Record != null)
            {
                Record.RemoveRecordListItem("settingsdata", idx);
            }
        }
        public void RemoveSetting(string keyvalue)
        {
            if (Record != null)
            {
                Record.RemoveRecordListItem("settingsdata", "genxml/settings/key", keyvalue);
            }
        }

        #endregion

        #region "properties"
        public SimplisityRecord Record { get; private set; }
        public bool Exists
        {
            get
            {
                if (Record.ItemID <= 0)
                {
                    return false;
                }
                return true;
            }
        }
        public string Name { get { return Record.GetXmlProperty("genxml/settings/name"); } set { Record.SetXmlProperty("genxml/settings/name", value); } }
        public int ModuleId { get { return Record.ModuleId; } private set { Record.ModuleId = value; } }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/settings/systemkey"); } set { Record.SetXmlProperty("genxml/settings/systemkey", value); } }
        public string RemoteSystemKey { get { return Record.GetXmlProperty("genxml/settings/remotesystemkey"); } set { Record.SetXmlProperty("genxml/settings/remotesystemkey", value); } }
        public int TabId { get { return Record.GetXmlPropertyInt("genxml/settings/tabid"); } set { Record.SetXmlProperty("genxml/settings/tabid", value.ToString()); } }
        public string ModuleRef { get { return Record.GetXmlProperty("genxml/settings/moduleref"); } set { Record.SetXmlProperty("genxml/settings/moduleref", value); } }
        public bool CacheDisabled { get { return Record.GetXmlPropertyBool("genxml/settings/disablecache"); } set { Record.SetXmlProperty("genxml/settings/disablecache", value.ToString()); } }
        public bool CacheEnabled { get { return !Record.GetXmlPropertyBool("genxml/settings/disablecache"); } }
        public int CacheTimeout { get { return Record.GetXmlPropertyInt("genxml/settings/cachetime"); } set { Record.SetXmlProperty("genxml/settings/cachetime", value.ToString()); } }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/settings/securitykey"); } set { Record.SetXmlProperty("genxml/settings/securitykey", value); } }
        public string RemoteAPI { get { return EngineURL.TrimEnd('/') + "/Desktopmodules/DNNrocket/api/rocket/actionremote"; } }
        public string EngineURL { get { return Record.GetXmlProperty("genxml/settings/engineurl"); } set { Record.SetXmlProperty("genxml/settings/engineurl", value); } }
        public string RemoteCmd { get { return Record.GetXmlProperty("genxml/settings/remotecmd"); } set { Record.SetXmlProperty("genxml/settings/remotecmd", value); } }
        public string RemoteTemplate { get { return Record.GetXmlProperty("genxml/settings/remotetemplate"); } set { Record.SetXmlProperty("genxml/settings/remotetemplate", value); } }
        public string RemoteKey { get { return Record.GetXmlProperty("genxml/settings/remotekey"); } set { Record.SetXmlProperty("genxml/settings/remotekey", value); } }
        public string AppThemeFolder { get { return Record.GetXmlProperty("genxml/settings/appthemefolder"); } set { Record.SetXmlProperty("genxml/settings/appthemefolder", value); } }
        public string AppThemeVersion { get { return Record.GetXmlProperty("genxml/settings/appthemeversion"); } set { Record.SetXmlProperty("genxml/settings/appthemeversion", value); } }
        public string AdminRelPath { get { return Record.GetXmlProperty("genxml/settings/adminrelpath"); } set { Record.SetXmlProperty("genxml/settings/adminrelpath", value); } }
        public string AdminUrl { get { return EngineURL.TrimEnd('/') + "/" + AdminRelPath.TrimStart('/'); } }
        public string RecordItemBase64 { get { return GeneralUtils.Base64Encode(Record.ToXmlItem()); } }
        public string SiteKey { get { return Record.GetXmlProperty("genxml/form/sitekey"); } set { Record.SetXmlProperty("genxml/form/sitekey", value.ToString()); } }
        public string PageUrl { get { return Record.GetXmlProperty("genxml/form/pageurl"); } set { Record.SetXmlProperty("genxml/form/pageurl", value.ToString()); } }
        public string CultureCode { get { return Record.GetXmlProperty("genxml/form/culturecode"); } set { Record.SetXmlProperty("genxml/form/culturecode", value.ToString()); } }


        #endregion


    }



}
