using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace RocketUtils
{
    public static class RocketUtils
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

        /// <summary>
        /// Calls the DNNrocket Action API endpoint as if it were a normal simplisity.js JSON call.
        /// Converts <paramref name="postInfo"/> to <c>inputjson</c> postdata format and <paramref name="paramInfo"/>
        /// to <c>paramjson</c> format, then HTTP-POSTs to <paramref name="url"/> with <c>?cmd=<paramref name="sCmd"/></c>.
        /// Cookies from the current request are forwarded so the call executes in the same authenticated session.
        /// </summary>
        /// <param name="domainurl">Domain URL  (e.g. https://my.site.com).</param>
        /// <param name="sCmd">The command to execute (appended as ?cmd= to the URL).</param>
        /// <param name="postInfo">The data info posted as inputjson.</param>
        /// <param name="paramInfo">Optional parameter info posted as paramjson; pass null for an empty paramjson.</param>
        /// <returns>The response body as a string, or an error message prefixed with "ERROR".</returns>
        public static async Task<string> CallAction(string domainurl, string sCmd, SimplisityInfo postInfo, SimplisityInfo paramInfo = null, Dictionary<string, string> queryString = null)
        {
            try
            {
                var fullUrl = domainurl.TrimEnd('/') + "/Desktopmodules/dnnrocket/api/rocket/action";
                var separator = fullUrl.Contains("?") ? "&" : "?";
                fullUrl += separator + "cmd=" + Uri.EscapeDataString(sCmd);

                if (queryString != null)
                {
                    foreach (var q in queryString)
                    {
                        fullUrl += "&" + Uri.EscapeDataString(q.Key) + "=" + Uri.EscapeDataString(q.Value ?? string.Empty);
                    }
                }

                var inputJson = postInfo?.XMLData ?? string.Empty;
                var paramJson = paramInfo?.XMLData ?? string.Empty;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["inputjson"] = inputJson,
                    ["paramjson"] = paramJson
                });

                var response = await _httpClient.PostAsync(fullUrl, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return "ERROR - CallAction url=" + domainurl + " cmd=" + sCmd + " ex=" + ex.ToString();
            }
        }

        public static string GetUniqueFileName(string fileName, string folderMapPath, int idx = 1, string originalFileName = "")
        {
            if (originalFileName == "") originalFileName = fileName;

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            if (File.Exists(folderMapPath + "\\" + fileName))
            {
                fileName = GetUniqueFileName(Path.GetFileNameWithoutExtension(originalFileName) + idx + Path.GetExtension(originalFileName), folderMapPath, idx + 1, originalFileName);
            }
            return fileName;
        }

    }
}
