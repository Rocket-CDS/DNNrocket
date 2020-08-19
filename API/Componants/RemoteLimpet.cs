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

        public RemoteLimpet(int moduleId, string systemKey = "", bool useCache = true)
        {
            Record = new SimplisityRecord();
            ModuleId = moduleId;
            _cacheKey = "remoteparams*" + ModuleId;
            Record = (SimplisityRecord)CacheUtilsDNN.GetCache(_cacheKey);
            var objCtrl = new DNNrocketController();
            if ((Record == null || !useCache))
            {
                if (ModuleId <= 0)
                {
                    Record = new SimplisityRecord();
                }
                else
                {
                    Record = objCtrl.GetRecordByGuidKey(-1, ModuleId, "REMOTEPARAMS", _cacheKey, "", _tableName);
                    if (Record == null)
                    {
                        Record = new SimplisityRecord();
                        Record.PortalId = -1;
                        Record.TypeCode = "REMOTEPARAMS";
                        Record.GUIDKey = _cacheKey;
                        Record.ModuleId = ModuleId;
                        Record = objCtrl.SaveRecord(Record);
                    }
                    CacheUtilsDNN.SetCache(_cacheKey, Record);
                }
            }
            if (systemKey != "") SystemKey = systemKey;
            if (ModuleRef == "") ModuleRef = GeneralUtils.GetUniqueString(3);

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
        public string Name { get { return Record.GetXmlProperty("genxml/name");  } set { Record.SetXmlProperty("genxml/name", value); } }
        public int ModuleId { get; private set; }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/systemkey"); } set { Record.SetXmlProperty("genxml/systemkey", value); } }
        public int TabId { get { return Record.GetXmlPropertyInt("genxml/tabid"); } set { Record.SetXmlProperty("genxml/tabid", value.ToString()); } }
        public string ModuleRef { get { return Record.GetXmlProperty("genxml/moduleref"); } set { Record.SetXmlProperty("genxml/moduleref", value); } }
        public bool CacheDisbaled { get { return Record.GetXmlPropertyBool("genxml/disablecache"); } set { Record.SetXmlProperty("genxml/cachedisbaled", value.ToString()); } }
        public bool CacheEnabled { get { return !Record.GetXmlPropertyBool("genxml/disablecache"); } }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/securitykey"); } set { Record.SetXmlProperty("genxml/securitykey", value); } }
        public string RemoteSystemKey { get { return Record.GetXmlProperty("genxml/remotesystemkey"); } set { Record.SetXmlProperty("genxml/remotesystemkey", value); } }
        public string RemoteAPI { get { return EngineURL.TrimEnd('/') + "/Desktopmodules/dnnrocket/api/rocket/actionremote"; } }
        public string EngineURL { get { return Record.GetXmlProperty("genxml/engineurl"); } set { Record.SetXmlProperty("genxml/engineurl", value); } }
        public string RemoteCmd { get { return Record.GetXmlProperty("genxml/remotecmd"); } set { Record.SetXmlProperty("genxml/remotecmd", value); } }
        public string RemoteTemplate { get { return Record.GetXmlProperty("genxml/remotetemplate"); } set { Record.SetXmlProperty("genxml/remotetemplate", value); } }
        public string RemoteKey { get { return Record.GetXmlProperty("genxml/remotekey"); } set { Record.SetXmlProperty("genxml/remotekey", value); } }
        public string AppThemeFolder { get { return Record.GetXmlProperty("genxml/appthemefolder"); } set { Record.SetXmlProperty("genxml/appthemefolder", value); } }
        public string AppThemeVersion { get { return Record.GetXmlProperty("genxml/appthemeversion"); } set { Record.SetXmlProperty("genxml/appthemeversion", value); } }

        #endregion
    }



}
