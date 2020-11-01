using DNNrocketAPI;
using DotNetNuke.Entities.Modules;
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

namespace DNNrocketAPI.Componants
{
    /// <summary>
    /// Used to call remote engine and returns data
    /// </summary>
    public class RemoteLimpet
    {
        private string _cacheKey;
        private const string _tableName = "DNNrocket";

        public RemoteLimpet(string systemKey)
        {
            Record = new SimplisityRecord();
            SystemKey = systemKey;
        }
        /// <summary>
        /// Convert input SimplsityInfo to a remoteLimpet.  (No Cache)
        /// </summary>
        /// <param name="paramInfo"></param>
        /// <param name="systemKey"></param>
        public RemoteLimpet(SimplisityInfo paramInfo, string systemKey)
        {
            Record = new SimplisityRecord();
            var remoteparams = paramInfo.GetXmlProperty("genxml/settings/remoteparams");
            if (remoteparams == "") remoteparams = paramInfo.GetXmlProperty("genxml/hidden/remoteparams");
            var remoteParamItem = StringCompress.DecompressString(remoteparams);
            if (remoteParamItem != "") Record.FromXmlItem(remoteParamItem);
            if (systemKey != "") SystemKey = systemKey;
            if (ModuleRef == "") ModuleRef = GeneralUtils.GetUniqueString(3);
            // add any params that have been past
            var nodList = paramInfo.XMLDoc.SelectNodes("genxml/urlparams/*");
            if (nodList != null)
            {
                foreach (XmlNode nod in nodList)
                {
                    AddUrlParam(nod.Name, nod.InnerText);
                }
            }
            var nodList2 = paramInfo.XMLDoc.SelectNodes("genxml/form/*");
            if (nodList2 != null)
            {
                foreach (XmlNode nod in nodList2)
                {
                    AddFormParam(nod.Name, nod.InnerText);
                }
            }

            Record.SetXmlProperty("genxml/requestcontent", paramInfo.GetXmlPropertyRaw("genxml/requestcontent"));

        }
        /// <summary>
        /// Link remoteData to as module, this can be saved in the DB and cached.
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="systemKey"></param>
        /// <param name="useCache"></param>
        public RemoteLimpet(int moduleId, string systemKey = "", bool useCache = true)
        {
            Record = new SimplisityRecord();
            _cacheKey = "remoteparams*" + moduleId;
            Record = (SimplisityRecord)CacheUtilsDNN.GetCache(_cacheKey);
            var objCtrl = new DNNrocketController();
            if ((Record == null || !useCache))
            {
                Record = new SimplisityRecord();
                if (moduleId > 0)
                {
                    Record = objCtrl.GetRecordByGuidKey(-1, moduleId, "REMOTEPARAMS", _cacheKey, "", _tableName);
                    if (Record == null)
                    {
                        Record = new SimplisityRecord();
                        Record.PortalId = -1;
                        Record.TypeCode = "REMOTEPARAMS";
                        Record.GUIDKey = _cacheKey;
                        Record.ModuleId = moduleId;
                        Record = objCtrl.SaveRecord(Record);
                    }
                    CacheUtilsDNN.SetCache(_cacheKey, Record);
                }
            }

            // if we have a Security Code, use that for the params passed
            try
            {
                var sc2 = Record.GetXmlProperty("genxml/settings/securitycode");
                if (sc2 != "")
                {
                    var sRemote = new SimplisityInfo();
                    sRemote.FromXmlItem(StringCompress.DecompressString(sc2));
                    var l = sRemote.ToDictionary();
                    foreach (var d in l)
                    {
                        Record.SetXmlProperty("genxml/settings/" + d.Key, d.Value);
                    }
                }
            }
            catch (Exception)
            {
                // Ignore
            }

            if (systemKey != "") SystemKey = systemKey;
            if (ModuleRef == "") ModuleRef = GeneralUtils.GetUniqueString();
        }
        public void Save(SimplisityInfo postInfo)
        {
            Record.XMLData = postInfo.XMLData;
            Update();
        }
        public void Update()
        {
            var objCtrl = new DNNrocketController();
            Record = objCtrl.SaveRecord(_cacheKey, "REMOTEPARAMS", Record, ModuleId, _tableName);
            CacheUtilsDNN.SetCache(_cacheKey, Record);
        }
        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(Record.ItemID, _tableName);
            Record = new SimplisityRecord();
            CacheUtilsDNN.RemoveCache(_cacheKey);
        }
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
                if (key.ToLower() == UrlKey.ToLower() && UrlKeyActive) RemoteKey = value;
                // get current url for remoteParam
                if (key.ToLower() == "tabid") ClientUrl = DNNrocketUtils.NavigateURL(Convert.ToInt32(value), new string[0]);
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
                Update();
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

        #region "API call"

        public MetaSEO seoAPI(string httpMethod = "POST")
        {
            var metaSEO = new MetaSEO();
            if (RemoteCmd == "") return metaSEO;
            var cmd = RemoteCmd;
            if (GetUrlParam("cmd") != "") cmd = GetUrlParam("cmd"); // URL param overwrites database setting.
            var remoteHeaderCmd = cmd + "seo";
            var rtnXml =  CallAPI(remoteHeaderCmd, httpMethod, "text/html");
            if (rtnXml != "")
            {
                var sRec = new SimplisityRecord();
                sRec.FromXmlItem(rtnXml);
                metaSEO.Title = GeneralUtils.CleanInput(sRec.GetXmlProperty("genxml/title"));
                metaSEO.Description = GeneralUtils.CleanInput(sRec.GetXmlProperty("genxml/description"));
                metaSEO.KeyWords = GeneralUtils.CleanInput(sRec.GetXmlProperty("genxml/keywords"));
            }
            return metaSEO;
        }
        public string headerAPI(string httpMethod = "POST")
        {
            if (RemoteCmd == "") return "";
            var cmd = RemoteCmd;
            if (GetUrlParam("cmd") != "") cmd = GetUrlParam("cmd"); // URL param overwrites database setting.
            var remoteHeaderCmd = cmd + "header";
            return CallAPI(remoteHeaderCmd,httpMethod, "text/html");
        }
        public string htmlAPI(string httpMethod = "POST")
        {
            if (RemoteCmd == "") return "";
            var cmd = RemoteCmd;
            if (GetUrlParam("cmd") != "") cmd = GetUrlParam("cmd"); // URL param overwrites database setting.
            return CallAPI(cmd, httpMethod, "text/html");
        }
        public string jsonAPI(string httpMethod = "POST")
        {
            if (RemoteCmd == "") return "{\"cmd\":\"\",}";
            var cmd = RemoteCmd;
            if (GetUrlParam("cmd") != "") cmd = GetUrlParam("cmd"); // URL param overwrites database setting.
            var remoteJsonCmd = cmd + "json";
            return CallAPI(remoteJsonCmd, httpMethod, "application/json");
        }
        private string CallAPI(string cmd, string httpMethod = "POST", string contentType = "text/html")
        {
            try
            {
                var rtnStr = "";

                if (EngineURL != "" && cmd != "" && RemoteSystemKey != "")
                {
                    // Build data to be sent.
                    var paramInfo = new SimplisityInfo();
                    paramInfo.TypeCode = "paramInfo";
                    
                    Language = DNNrocketUtils.GetCurrentCulture();
                    paramInfo.SetXmlProperty("genxml/settings/language", DNNrocketUtils.GetCurrentCulture());  // pass the current language to the remote server
                    AddUrlParam("language", DNNrocketUtils.GetCurrentCulture());

                    paramInfo.SetXmlProperty("genxml/settings/remoteparams", CompressedRemoteParam);
                    var urlNode = Record.XMLDoc.SelectSingleNode("genxml/urlparams");
                    if (urlNode != null) paramInfo.AddXmlNode(urlNode.OuterXml, "urlparams", "genxml");
                    var formNode = Record.XMLDoc.SelectSingleNode("genxml/form");
                    if (formNode != null) paramInfo.AddXmlNode(formNode.OuterXml, "form", "genxml");

                    var body = "<items>" + paramInfo.ToXmlItem() + "</items>";

                    // build weburl
                    var weburl = $"{RemoteAPI}?cmd={cmd}&systemkey={RemoteSystemKey}&language=" + DNNrocketUtils.GetCurrentCulture();

                    try
                    {
                        var webReq = WebRequest.Create(weburl);
                        webReq.Method = httpMethod;
                        webReq.ContentType = contentType;

                        if (String.IsNullOrEmpty(body)) body = PortalUtils.SiteGuid().ToString();

                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] byte1 = encoding.GetBytes(body);
                        // Set the content length of the string being posted.
                        webReq.ContentLength = byte1.Length;
                        // get the request stream
                        Stream newStream = webReq.GetRequestStream();
                        // write the content to the stream
                        newStream.Write(byte1, 0, byte1.Length);

                        var webResp = (HttpWebResponse)webReq.GetResponse();

                        if (webResp.StatusCode == HttpStatusCode.Unauthorized) return "";

                        var readStream = new StreamReader(webResp.GetResponseStream(), System.Text.Encoding.UTF8);
                        rtnStr = readStream.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        rtnStr = "** Config Error **";
                        LogUtils.LogException(ex);
                    }
                }
                return rtnStr;
            }
            catch (Exception e)
            {
                return e.ToString();
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
        public string Language { get {
                var rtn = Record.GetXmlProperty("genxml/urlparam/language");  // use language if passed form romote source.
                if (rtn == "") rtn = Record.GetXmlProperty("genxml/settings/language");  // use language if passed form romote source.
                if (rtn == "") rtn = Record.GetXmlProperty("genxml/hidden/language");  // use language if passed form romote source.
                if (rtn == "") rtn = DNNrocketUtils.GetCurrentCulture(); // no remote langauge, use local culture code
                return rtn; 
            } private set { Record.SetXmlProperty("genxml/settings/language", value); } }
        public string Name { get { return Record.GetXmlProperty("genxml/settings/name");  } set { Record.SetXmlProperty("genxml/settings/name", value); } }
        public int ModuleId { get { return Record.ModuleId; } private set { Record.ModuleId = value; } }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/settings/systemkey"); } set { Record.SetXmlProperty("genxml/settings/systemkey", value); } }
        public int TabId { get { return Record.GetXmlPropertyInt("genxml/settings/tabid"); } set { Record.SetXmlProperty("genxml/settings/tabid", value.ToString()); } }
        public string ModuleRef { get { return Record.GetXmlProperty("genxml/settings/moduleref"); } set { Record.SetXmlProperty("genxml/settings/moduleref", value); } }
        public bool CacheDisabled { get { return Record.GetXmlPropertyBool("genxml/settings/disablecache"); } set { Record.SetXmlProperty("genxml/settings/disablecache", value.ToString()); } }
        public bool CacheEnabled { get { return !Record.GetXmlPropertyBool("genxml/settings/disablecache"); } }
        public int CacheTimeout { get { return Record.GetXmlPropertyInt("genxml/settings/cachetime"); } set { Record.SetXmlProperty("genxml/settings/cachetime", value.ToString()); } }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/settings/securitykey"); } set { Record.SetXmlProperty("genxml/settings/securitykey", value); } }
        public string RemoteSystemKey { get { return Record.GetXmlProperty("genxml/settings/remotesystemkey"); } set { Record.SetXmlProperty("genxml/settings/remotesystemkey", value); } }
        public string RemoteAPI { get { return EngineURL.TrimEnd('/') + "/Desktopmodules/dnnrocket/api/rocket/actionremote"; } }
        public string EngineURL { get { return Record.GetXmlProperty("genxml/settings/engineurl"); } set { Record.SetXmlProperty("genxml/settings/engineurl", value); } }
        public string RemoteCmd { get { return Record.GetXmlProperty("genxml/settings/remotecmd"); } set { Record.SetXmlProperty("genxml/settings/remotecmd", value); } }
        public string Template { get { return Record.GetXmlProperty("genxml/settings/template"); } set { Record.SetXmlProperty("genxml/settings/template", value); } }
        public string RemoteTemplate
        {
            get
            {
                if (Record.GetXmlProperty("genxml/settings/remotetemplate") == "")
                    return Template;
                else
                    return Record.GetXmlProperty("genxml/settings/remotetemplate");
            }
            set { Record.SetXmlProperty("genxml/settings/remotetemplate", value); }
        }
        public string RemoteKey { get { return Record.GetXmlProperty("genxml/settings/remotekey"); } set { Record.SetXmlProperty("genxml/settings/remotekey", value); } }
        public string AppThemeFolder { get { return Record.GetXmlProperty("genxml/settings/appthemefolder"); } set { Record.SetXmlProperty("genxml/settings/appthemefolder", value); } }
        public string AppThemeVersion { get { return Record.GetXmlProperty("genxml/settings/appthemeversion"); } set { Record.SetXmlProperty("genxml/settings/appthemeversion", value); } }
        public bool UrlKeyActive { get { return Record.GetXmlPropertyBool("genxml/settings/urlkeyactive"); } set { Record.SetXmlProperty("genxml/settings/urlkeyactive", value.ToString()); } }
        public string UrlKey { get { return Record.GetXmlProperty("genxml/settings/urlkey"); } set { Record.SetXmlProperty("genxml/settings/urlkey", value); } }
        public string ClientUrl { get { return Record.GetXmlProperty("genxml/settings/url"); } set { Record.SetXmlProperty("genxml/settings/url", value); } }
        public string RemoteAdminRelPath { get { return Record.GetXmlProperty("genxml/settings/remoteadminrelpath"); } set { Record.SetXmlProperty("genxml/settings/remoteadminrelpath", value); } }
        public string RemoteAdminUrl { get { return EngineURL.TrimEnd('/') + "/" + RemoteAdminRelPath; } }
        public string CompressedRemoteParam { get { return StringCompress.CompressString(Record.ToXmlItem()); } }

        #endregion
    }



}
