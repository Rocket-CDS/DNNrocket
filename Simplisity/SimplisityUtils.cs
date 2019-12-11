using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using Newtonsoft.Json;

namespace Simplisity
{
    public static class SimplisityUtils
    {        

        public static string PostData(string url, string systemkey, string simplisity_cmd, string inputjson = "", string paramjson = "", NameValueCollection reqparm = null)
        {
            if (reqparm == null) reqparm = new NameValueCollection();
            reqparm.Add("simplisity_cmd", simplisity_cmd);
            reqparm.Add("inputjson", inputjson);
            reqparm.Add("paramjson", paramjson);
            if (!url.ToLower().StartsWith("http") && !url.ToLower().StartsWith("https")) url = "http://" + url;
            return PostData(url + "?cmd=" + simplisity_cmd + "&systemkey=" + systemkey, reqparm);
        }
        public static string PostData(string url, NameValueCollection reqparm)
        {
            var responsebody = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] responsebytes = client.UploadValues(url, "POST", reqparm);
                    responsebody = Encoding.UTF8.GetString(responsebytes);
                }
            }
            catch (Exception ex)
            {
                responsebody = ex.ToString();
            }
            return responsebody;
        }

    }
}
