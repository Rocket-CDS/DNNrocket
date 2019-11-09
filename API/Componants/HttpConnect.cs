using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DNNrocketAPI.Componants
{
    public class HttpConnect
    {
        private SystemInfoData _systemInfoData;
        private SystemGlobalData _systemGlobalData;
        private string _baseuri;
        public HttpConnect(string selectedSystemKey)
        {
            _systemInfoData = new SystemInfoData(selectedSystemKey);
            _systemGlobalData = new SystemGlobalData();
            _baseuri = _systemGlobalData.PublicAppThemeURI.TrimStart('/').TrimEnd('/') + "/" + _systemInfoData.SystemKey;
        }
        public string DownloadAppThemeToFile(string appThemeFolder, string destinationMapPath)
        {
            try
            {
                var uri = _baseuri + "/zip/" + appThemeFolder + ".zip";
                DownloadZip(uri, destinationMapPath);
            }
            catch (Exception exc)
            {
                DNNrocketUtils.LogException(exc);
                return "FAIL";
            }
            return "OK";
        }

        public void DownloadZip(string uri, string destinationmapPath)
        {
            WebClient client = new WebClient();
            client.DownloadFile(uri, destinationmapPath);
        }

        public string Download(string uri)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential(_systemGlobalData.FtpUserName, _systemGlobalData.FtpPassword);
                var rtnString = client.DownloadString(uri);
                return rtnString;
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }

        public List<SimplisityInfo> DownloadAppThemeXmlList()
        {
            var rtnList = new List<SimplisityInfo>();
            var namelist = ListXmlFiles();
            foreach (var n in namelist)
            {
                var uri = _baseuri + "/xml/" + n;
                var xmlDownload = Download(uri);
                if (xmlDownload != "FAIL")
                {
                    var sInfo = new SimplisityInfo();
                    sInfo.FromXmlItem(xmlDownload);
                    rtnList.Add(sInfo);
                }
            }
            return rtnList;
        }
        private List<string> ListXmlFiles()
        {
            try
            {
                WebClient client = new WebClient();
                var rtnList = new List<string>();
                var uri = _baseuri + "/xml";
                WebRequest request = WebRequest.Create(uri);
                WebResponse response = request.GetResponse();
                Regex regex = new Regex("<a href=\".*(.xml)");

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    MatchCollection matches = regex.Matches(result);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (!match.Success) { continue; }
                            foreach (Group n in match.Groups)
                            {
                                var hrefLink = XElement.Parse(n.Value + "</a>")
                                               .Descendants("a")
                                               .Select(x => x.Attribute("href").Value)
                                               .FirstOrDefault();
                                rtnList.Add(hrefLink);
                            }
                        }
                    }
                }
                return rtnList;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


    }
}
