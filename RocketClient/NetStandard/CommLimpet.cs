using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RocketComm
{
    /// <summary>
    /// Processing Limpet for POST and return of data from RocketCDS.
    /// </summary>
    public class CommLimpet
    {
        public CommLimpet(SimplisityRecord  remoteParams)
        {
            RemoteParams = remoteParams;
        }

        public CommData CallRedirect(string cmd, string jsonPost = "", string jsonParams = "", string systemKey = "")
        {
            var RocketClientData = new CommData();
            if (systemKey == "") systemKey = RemoteParams.GetXmlProperty("genxml/remote/systemkey");
            if (EngineURL != "" && cmd != "" && systemKey != "")
            {
                // build weburl
                var weburl = $"{RemoteAPI}?cmd={cmd}&systemkey={systemKey}&language=" + RemoteParams.GetXmlProperty("genxml/remote/culturecode");

                try
                {
                    RocketClientData.CacheFlag = false;
                    var webReq = WebRequest.Create(weburl);
                    webReq.Method = "POST";
                    webReq.ContentType = "application/x-www-form-urlencoded";

                    Dictionary<string, string> formField = new Dictionary<string, string>();

                    formField.Add("inputjson", jsonPost);
                    formField.Add("paramjson", jsonParams);
                    formField.Add("remote", GeneralUtils.EnCode(GeneralUtils.Base64Encode(RemoteParams.ToXmlItem())));
                    formField.Add("simplisity_cmd", cmd);

                    string body = GetBodyStringFromDictionary(formField);

                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] byte1 = encoding.GetBytes(body);
                    // Set the content length of the string being posted.
                    webReq.ContentLength = byte1.Length;
                    // get the request stream
                    Stream newStream = webReq.GetRequestStream();
                    // write the content to the stream
                    newStream.Write(byte1, 0, byte1.Length);

                    var webResp = (HttpWebResponse)webReq.GetResponse();

                    if (webResp.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        var readStream = new StreamReader(webResp.GetResponseStream(), System.Text.Encoding.UTF8);

                        RocketClientData.StatusCode = webResp.Headers["razor-statuscode"];
                        RocketClientData.ErrorMsg = GeneralUtils.Base64Decode(webResp.Headers["razor-errormsg"] ?? "");
                        RocketClientData.FirstHeader = GeneralUtils.Base64Decode(webResp.Headers["remote-firstheader"] ?? "");
                        RocketClientData.LastHeader = GeneralUtils.Base64Decode(webResp.Headers["remote-lastheader"] ?? "");
                        RocketClientData.SeoHeaderXml = GeneralUtils.Base64Decode(webResp.Headers["remote-seoheader"] ?? "");
                        RocketClientData.JsonReturn = GeneralUtils.Base64Decode(webResp.Headers["remote-json"] ?? "");
                        RocketClientData.SettingsXml = GeneralUtils.Base64Decode(webResp.Headers["remote-settingsxml"] ?? "");
                        RocketClientData.Body = readStream.ReadToEnd();
                        RocketClientData.CacheFlag = false;
                        if (webResp.Headers["remote-cache"] != null) RocketClientData.CacheFlag = bool.Parse(GeneralUtils.Base64Decode(webResp.Headers["remote-cache"]));
                    }
                    else
                    {
                        RocketClientData.ErrorMsg = "Unauthorized Code:" + HttpStatusCode.Unauthorized;
                        RocketClientData.StatusCode = webResp.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    RocketClientData.ErrorMsg = ex.Message + " : " + ex.ToString();
                    RocketClientData.StatusCode = "10";
                }
            }
            return RocketClientData;
        }
        private string GetBodyStringFromDictionary(Dictionary<string, string> formField)
        {
            string body = string.Empty;
            foreach (var pair in formField)
            {
                body += $"{pair.Key}={pair.Value}&";
            }
            // delete last "&"
            body = body.Substring(0, body.Length - 1);
            return body;
        }

        public SimplisityRecord RemoteParams { set; get; }
        public string EngineURL { get { return RemoteParams.GetXmlProperty("genxml/remote/engineurl"); } set { RemoteParams.SetXmlProperty("genxml/remote/engineurl", value); } }
        public string RemoteAPI { get { return EngineURL.TrimEnd('/') + "/Desktopmodules/DNNrocket/api/rocket/action"; } }

    }
}
