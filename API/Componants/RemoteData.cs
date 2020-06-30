using DNNrocketAPI;
using DotNetNuke.Entities.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DNNrocketAPI.Componants
{
    /// <summary>
    /// </summary>
    public class RemoteData
    {

        private SimplisityRecord _remoteModuleRec;
        private string _cacheKey;
        private int _moduleid;

        public RemoteData(int moduleId, bool useCache = true)
        {
            _remoteModuleRec = new SimplisityRecord();
            _moduleid = moduleId;
            _cacheKey = "remotemodule*" + moduleId;
            _remoteModuleRec = (SimplisityRecord)CacheUtilsDNN.GetCache(_cacheKey);
            var objCtrl = new DNNrocketController();
            if ((_remoteModuleRec == null || !useCache))
            {
                if (moduleId <= 0)
                {
                    _remoteModuleRec = new SimplisityRecord();
                    Exists = false;
                }
                else
                {
                    _remoteModuleRec = objCtrl.GetRecordByGuidKey(-1, _moduleid, "REMOTEMODULE", _cacheKey);
                    if (_remoteModuleRec == null)
                    {
                        _remoteModuleRec = new SimplisityRecord();
                        _remoteModuleRec.PortalId = -1;
                        _remoteModuleRec.TypeCode = "REMOTEMODULE";
                        _remoteModuleRec.GUIDKey = _cacheKey;
                        _remoteModuleRec.ModuleId = _moduleid;
                        _remoteModuleRec = objCtrl.SaveRecord(_remoteModuleRec);
                        Exists = false;
                    }
                    else
                    {
                        Exists = true;
                    }
                    CacheUtilsDNN.SetCache(_cacheKey, _remoteModuleRec);
                }
            }
            else
            {
                Exists = true;
            }
        }
        public void Save()
        {
            var objCtrl = new DNNrocketController();
            Exists = true;
            _remoteModuleRec = objCtrl.SaveRecord(_cacheKey, "REMOTEMODULE", _remoteModuleRec, _moduleid);
            CacheUtilsDNN.SetCache(_cacheKey, _remoteModuleRec);
        }
        public void Delete()
        {
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(Record.ItemID);
            Exists = false;
            CacheUtilsDNN.RemoveCache(_cacheKey);
        }
        public string CallAPI(string hostname, string requestPath, string cmd, string systemkey, SimplisityInfo postInfo, SimplisityInfo paramInfo, string body = "", string httpMethod = "POST", string protocol = "https")
        {
            try
            {
                NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                outgoingQueryString.Add("inputjson", postInfo.ToXmlItem());
                outgoingQueryString.Add("paramjson", paramInfo.ToXmlItem());
                string postdata = outgoingQueryString.ToString();

                var webReq = WebRequest.Create($"{protocol}://{hostname}{requestPath}?cmd={cmd}&systemkey={systemkey}&{postdata}");
                webReq.Method = httpMethod;
                webReq.ContentType = "application/json";

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
                    DNNrocketUtils.LogDebug("CallAPI() Login expired. Please start over.");
                    return "";
                }

                var readStream = new StreamReader(webResp.GetResponseStream(), Encoding.UTF8);
                return readStream.ReadToEnd();
            }
            catch (Exception e)
            {
                return null;
            }
        }


        #region "set, get functions"

        public string GetValue(string key, string defaultValue = "")
        {
            var rtn = _remoteModuleRec.GetXmlProperty("genxml/hidden/" + key.ToLower());
            if (rtn == "") return defaultValue;
            return rtn;
        }
        public int GetValueInt(string key)
        {
            return _remoteModuleRec.GetXmlPropertyInt("genxml/hidden/" + key.ToLower());
        }
        public double GetValueDouble(string key)
        {
            return _remoteModuleRec.GetXmlPropertyDouble("genxml/hidden/" + key.ToLower());
        }
        public bool GetValueBool(string key)
        {
            if (_remoteModuleRec == null) return false;
            return _remoteModuleRec.GetXmlPropertyBool("genxml/hidden/" + key.ToLower());
        }
        public void SetValue(string key, string value)
        {
            _remoteModuleRec.SetXmlProperty("genxml/hidden/" + key.ToLower(), value);
        }

        #endregion

        #region "properties"

        public SimplisityRecord Record { get { return _remoteModuleRec; } }

        public string APIurl { get { return GetValue("APIurl"); } set { SetValue("APIurl", value); } }
        public bool Exists { get { return GetValueBool("Exists"); } set { SetValue("Exists", value.ToString()); } }
        public string ModuleRef { get { return GetValue("ModuleRef"); } set { SetValue("ModuleRef", value); } }

        public string Html { get { return GetValue("html"); } set { SetValue("html", value); } }
        public string Json { get { return GetValue("json"); } set { SetValue("json", value); } }

        #endregion

    }

}
