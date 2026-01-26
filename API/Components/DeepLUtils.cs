using DNNrocketAPI.ApiControllers;
using DNNrocketAPI.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RocketPortal.Components
{
    public static class DeepLUtils
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> TranslateText(string apiUrl, string authKey, string text, string destinationLanguage, string sourceLanguage = "")
        {
            try
            {
                if (text == "") return "";

                // deal with culturecode if passed by accident.
                if (destinationLanguage.Contains("-")) destinationLanguage = DNNrocketUtils.GetLanguageCode(destinationLanguage);
                if (sourceLanguage.Contains("-")) sourceLanguage = DNNrocketUtils.GetLanguageCode(sourceLanguage);

                if (destinationLanguage == sourceLanguage) return text;

                //client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "DNNrocket v1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", authKey);

                Dictionary<string, string> data = new Dictionary<string, string>();
                if (sourceLanguage != "") data.Add("source_lang", sourceLanguage);
                data.Add("text", text.Trim());
                data.Add("target_lang", destinationLanguage);

                var content = new FormUrlEncodedContent(data);

                var response = await client.PostAsync(apiUrl, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var json = response.Content.ReadAsStringAsync().Result;
                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
                var rtnInfo = new SimplisityInfo();
                rtnInfo.XMLDoc = doc;
                return rtnInfo.GetXmlProperty("translations/text");
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return text;
            }
        }

    }
}
