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

namespace DNNrocketAPI.Componants
{
    /// <summary>
    /// Used to call remote engine and returns data
    /// </summary>
    public class RemoteLimpet
    {
        private string _cacheKey;
        private const string _tableName = "DNNrocket";

        /// <summary>
        /// Convert input SimplsityInfo to a remoteLimpet.  (No Cache)
        /// </summary>
        /// <param name="paramInfo"></param>
        /// <param name="systemKey"></param>
        public RemoteLimpet(SimplisityInfo paramInfo, string systemKey)
        {
            Record = new SimplisityRecord();
            var remoteParamItem = GeneralUtils.DeCode(paramInfo.GetXmlProperty("genxml/hidden/remoteparams"));
            if (remoteParamItem != "") Record.FromXmlItem(remoteParamItem);
            if (systemKey != "") SystemKey = systemKey;
            if (ModuleRef == "") ModuleRef = GeneralUtils.GetUniqueString(3);
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

        #region "API call"

        public string headerAPI(string httpMethod = "POST")
        {
            var remoteHeaderCmd = RemoteCmd + "header";
            return CallAPI(remoteHeaderCmd,httpMethod, "text/html");
        }
        public string htmlAPI(string httpMethod = "POST")
        {
            return CallAPI(RemoteCmd, httpMethod, "text/html");
        }
        public string jsonAPI(string httpMethod = "POST")
        {
            return CallAPI(RemoteCmd, httpMethod, "application/json");
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
                    paramInfo.SetXmlProperty("genxml/hidden/language", DNNrocketUtils.GetCurrentCulture());
                    paramInfo.SetXmlProperty("genxml/hidden/remoteparams", GeneralUtils.EnCode(Record.ToXmlItem()));

                    var body = "<items>" + paramInfo.ToXmlItem() + "</items>";

                    // build weburl
                    var weburl = $"{RemoteAPI}?cmd={cmd}&systemkey={RemoteSystemKey}";
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

                    if (webResp.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        LogUtils.LogDebug("CallAPI() Login expired. Please start over.");
                        return "";
                    }

                    var readStream = new StreamReader(webResp.GetResponseStream(), System.Text.Encoding.UTF8);
                    rtnStr = readStream.ReadToEnd();
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
        public string Name { get { return Record.GetXmlProperty("genxml/hidden/name");  } set { Record.SetXmlProperty("genxml/hidden/name", value); } }
        public int ModuleId { get { return Record.ModuleId; } private set { Record.ModuleId = value; } }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/hidden/systemkey"); } set { Record.SetXmlProperty("genxml/hidden/systemkey", value); } }
        public int TabId { get { return Record.GetXmlPropertyInt("genxml/hidden/tabid"); } set { Record.SetXmlProperty("genxml/hidden/tabid", value.ToString()); } }
        public string ModuleRef { get { return Record.GetXmlProperty("genxml/hidden/moduleref"); } set { Record.SetXmlProperty("genxml/hidden/moduleref", value); } }
        public bool CacheDisbaled { get { return Record.GetXmlPropertyBool("genxml/hidden/disablecache"); } set { Record.SetXmlProperty("genxml/hidden/cachedisbaled", value.ToString()); } }
        public bool CacheEnabled { get { return !Record.GetXmlPropertyBool("genxml/hidden/disablecache"); } }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/hidden/securitykey"); } set { Record.SetXmlProperty("genxml/hidden/securitykey", value); } }
        public string RemoteSystemKey { get { return Record.GetXmlProperty("genxml/hidden/remotesystemkey"); } set { Record.SetXmlProperty("genxml/hidden/remotesystemkey", value); } }
        public string RemoteAPI { get { return EngineURL.TrimEnd('/') + "/Desktopmodules/dnnrocket/api/rocket/actionremote"; } }
        public string EngineURL { get { return Record.GetXmlProperty("genxml/hidden/engineurl"); } set { Record.SetXmlProperty("genxml/hidden/engineurl", value); } }
        public string RemoteCmd { get { return Record.GetXmlProperty("genxml/hidden/remotecmd"); } set { Record.SetXmlProperty("genxml/hidden/remotecmd", value); } }
        public string Template { get { return Record.GetXmlProperty("genxml/hidden/template"); } set { Record.SetXmlProperty("genxml/hidden/template", value); } }
        public string RemoteTemplate
        {
            get
            {
                if (Record.GetXmlProperty("genxml/hidden/remotetemplate") == "")
                    return Template;
                else
                    return Record.GetXmlProperty("genxml/hidden/remotetemplate");
            }
            set { Record.SetXmlProperty("genxml/hidden/remotetemplate", value); }
        }
        public string RemoteKey { get
            {
                if (Record.GetXmlProperty("genxml/hidden/remotekey") == "")
                    return SystemKey;
                else
                    return Record.GetXmlProperty("genxml/hidden/remotekey");
            } set { Record.SetXmlProperty("genxml/hidden/remotekey", value); } }
        public string AppThemeFolder { get { return Record.GetXmlProperty("genxml/hidden/appthemefolder"); } set { Record.SetXmlProperty("genxml/hidden/appthemefolder", value); } }
        public string AppThemeVersion { get { return Record.GetXmlProperty("genxml/hidden/appthemeversion"); } set { Record.SetXmlProperty("genxml/hidden/appthemeversion", value); } }

        #endregion
    }



}
