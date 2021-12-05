using DNNrocketAPI;
using DNNrocketAPI.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.AppThemes.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Components
{
    public static class AppThemeUtils
    {
        private static readonly object _cacheLock1 = new object();
        public static AppThemeLimpet GetAppThemeLimpet(string systemKey, string appThemeFolder, string versionFolder)
        {
            var cKey = "AppThemeLimpet*" + systemKey + "*" + appThemeFolder + "*" + versionFolder + "*" + PortalUtils.GetPortalId();
            var appTheme = (AppThemeLimpet)CacheUtilsDNN.GetCache(cKey);
            lock (_cacheLock1)
            {
                if (appTheme == null)
                {
                    appTheme = new AppThemeLimpet(appThemeFolder, versionFolder);
                    CacheUtilsDNN.SetCache(cKey, appTheme);
                }
            }
            return appTheme;
        }

        public static string HttpGet(string URI)
        {
            using (WebClient client = new WebClient())
            {
                // Add a user agent header in case the requested URI contains a query.
                client.Headers.Add("user-agent", "Toasted-github");
                Stream data = client.OpenRead(URI);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();

                return s;
            }
        }

        #region "Bonoboo git.agence-sesame.fr  - NOT USED"
        public static List<SimplisityRecord> GetBonobooAppThemes(string xmlfilename = "toastedrepos.xml")
        {
            var rtnList = new List<SimplisityRecord>();
            var contentsXml = HttpGet($"http://git.agence-sesame.fr/" + xmlfilename);
            var sRec = new SimplisityRecord();
            sRec.XMLData = contentsXml;
            var contents = sRec.XMLDoc.SelectNodes("root/*");
            if (contents != null)
            {
                foreach (XmlNode nod in contents)
                {
                    var sRec2 = new SimplisityRecord();
                    sRec2.XMLData = nod.OuterXml;
                    rtnList.Add(sRec2);
                }
            }
            return rtnList;
        }


        #endregion


        #region "GitHub"

        private static readonly object _cacheLock2 = new object();
        public static List<SimplisityRecord> GetGitHubAppThemes(string org)
        {
            var cKey = "GetGitHubAppThemes*" + org;
            var rtnList = (List<SimplisityRecord>)CacheUtilsDNN.GetCache(cKey);
            if (rtnList != null) return rtnList;
            lock (_cacheLock2)
            {
                rtnList = new List<SimplisityRecord>();
                var contentsJson = HttpGet($"https://api.github.com/orgs/{org}/repos");
                var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
                foreach (var file in contents)
                {
                    var themename = (string)file["name"];
                    if (!String.IsNullOrEmpty(themename))
                    {
                        XmlDocument doc = JsonConvert.DeserializeXmlNode("{\"genxml\":" + file.ToString() + "}");
                        var sRec = new SimplisityRecord();
                        sRec.XMLData = doc.OuterXml;
                        rtnList.Add(sRec);
                    }
                }
                CacheUtilsDNN.SetCache(cKey, rtnList);
                return rtnList;
            }
        }
        public static void DownloadAllGitHubAppTheme(string org)
        {
            var l = GetGitHubAppThemes(org);
            foreach (var a in l)
            {
                if (a.GetXmlNode("genxml/name") != "")
                {
                    var appTheme = AppThemeUtils.GetAppThemeLimpet("", a.GetXmlNode("genxml/name"), "");
                    DownloadRepoFromGitHub(a.GetXmlProperty("genxml/html_url") + "/archive/refs/heads/main.zip", appTheme.AppThemeFolderMapPath);
                }
            }
            CacheUtilsDNN.ClearAllCache();
        }
        public static void DownloadGitHubAppTheme(string contentsUrl, string downloadFolderMapPath)
        {
            DownloadRepoFromGitHub(contentsUrl + "/archive/refs/heads/main.zip", downloadFolderMapPath);
            CacheUtilsDNN.ClearAllCache();
        }

        private static void DownloadRepoFromGitHub(string contentsUrl, string downloadFolderMapPath)
        {
            var zFile = downloadFolderMapPath + "\\main.zip";
            WebClient client = new WebClient();
            client.DownloadFile(contentsUrl, zFile);
            // unzip
            var extractDir = downloadFolderMapPath.TrimEnd('\\') + "\\Temp";
            if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
            Directory.CreateDirectory(extractDir);
            DNNrocketUtils.ExtractZipFolder(downloadFolderMapPath + "\\main.zip", extractDir);
            // check for correct folder and move to AppTheme. (3rd level)
            foreach ( var d in Directory.GetDirectories(extractDir))
            {
                foreach (var d2 in Directory.GetDirectories(d))
                {
                    string dirName = new DirectoryInfo(d2).Name;
                    if (dirName != "")
                    {
                        var rDir = downloadFolderMapPath.TrimEnd('\\') + "\\" + dirName;
                        var gitFolder = rDir + "\\.git";
                        if (!Directory.Exists(gitFolder)) // if we have a git repo, do not download. (Dev Site)
                        {
                            if (Directory.Exists(rDir)) Directory.Delete(rDir, true);
                            Directory.Move(d2, rDir);
                        }
                    }
                }
            }
            //delete temp files.
            Directory.Delete(extractDir, true);
            File.Delete(zFile);
        }

        #endregion


    }
}
