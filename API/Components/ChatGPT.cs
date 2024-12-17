using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DNNrocketAPI.Components
{
    public class ChatGPT
    {
        private static MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
        private string _openai_key;
        public ChatGPT()
        {
            var globalData = new SystemGlobalData();
            _openai_key = globalData.ChatGptKey;
        }
        public async Task<string> GenerateImageAsync(string prompt, string dalleversion = "dall-e-2", string imageSize = "1024×1024")
        {
            if (!String.IsNullOrEmpty(prompt) && !String.IsNullOrEmpty(_openai_key))
            {
                if (dalleversion != "dall-e-3" && dalleversion != "dall-e-2")
                {
                    dalleversion = "dall-e-2";
                    imageSize = "1024×1024";
                }
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations");
                    var requestBody = new
                    {
                        model = dalleversion,
                        prompt = PadQuotes(prompt),
                        n = 1,
                        quality = "standard",
                        size = imageSize
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    request.Content = content;

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openai_key}");

                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(responseString, "root");
                    var sUrl = doc.SelectSingleNode("root/data/url").InnerText;
                    return sUrl;
                }
            }
            return "";
        }
        public string SendMsg(string sQuestion)
        {
            if (_openai_key == "") return "ChatGpt API Key missing";

            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Ssl3 |
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls11 |
                System.Net.SecurityProtocolType.Tls;

            string sUrl = "https://api.openai.com/v1/chat/completions";

            var request = WebRequest.Create(sUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + _openai_key);

            var data = "{";
            data += " \"model\":\"gpt-3.5-turbo\",";
            data += " \"messages\": [{\"role\": \"user\", \"content\": \"" + PadQuotes(sQuestion) + "\"}]";
            data += "}";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            string sJson = streamReader.ReadToEnd();
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(sJson,"root");
            var sResponse = doc.SelectSingleNode("root/choices/message/content").InnerText;
            return sResponse;
        }
        private string PadQuotes(string s)
        {
            if (s.IndexOf("\\") != -1)
                s = s.Replace("\\", @"\\");

            if (s.IndexOf("\n\r") != -1)
                s = s.Replace("\n\r", @"\n");

            if (s.IndexOf("\r") != -1)
                s = s.Replace("\r", @"\r");

            if (s.IndexOf("\n") != -1)
                s = s.Replace("\n", @"\n");

            if (s.IndexOf("\t") != -1)
                s = s.Replace("\t", @"\t");

            if (s.IndexOf("\"") != -1)
                return s.Replace("\"", @"""");
            else
                return s;
        }

        public string openai_key { get { return _openai_key; } }

    }
}
