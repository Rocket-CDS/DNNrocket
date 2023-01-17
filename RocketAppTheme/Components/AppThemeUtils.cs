using DNNrocketAPI;
using DNNrocketAPI.Components;
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
        public static AppThemeRocketApiLimpet AppThemeRocketApi(int portalId)
        {
            var cacheKey = "AppThemeRocketApi" + portalId;
            var appTheme = (AppThemeRocketApiLimpet)CacheUtils.GetCache(cacheKey);
            lock (_cacheLock1)
            {
                if (appTheme == null)
                {
                    appTheme = new AppThemeRocketApiLimpet(portalId);
                    CacheUtils.SetCache(cacheKey, appTheme);
                }
            }
            return appTheme;
        }
        public static AppThemeDNNrocketLimpet AppThemeDNNrocket(int portalId, string systemKey)
        {
            var cacheKey = "AppThemeDNNrocket" + portalId + "*" + systemKey;
            var appTheme = (AppThemeDNNrocketLimpet)CacheUtils.GetCache(cacheKey);
            lock (_cacheLock1)
            {
                if (appTheme == null)
                {
                    appTheme = new AppThemeDNNrocketLimpet(portalId, systemKey);
                    CacheUtils.SetCache(cacheKey, appTheme);
                }
            }
            return appTheme;
        }

        public static AppThemeModule AppThemeModule(int moduleId, string systemKey)
        {
            var cacheKey = "AppThemeModule" + moduleId + "*" + systemKey;
            var appTheme = (AppThemeModule)CacheUtils.GetCache(cacheKey);
            lock (_cacheLock1)
            {
                if (appTheme == null)
                {
                    appTheme = new AppThemeModule(moduleId, systemKey);
                    CacheUtils.SetCache(cacheKey, appTheme);
                }
            }
            return appTheme;
        }

        public static AppThemeSystemLimpet AppThemeSystem(int portalId, string systemKey)
        {
            var cacheKey = "AppThemeSystem" + portalId + "*" + systemKey;
            var appTheme = (AppThemeSystemLimpet)CacheUtils.GetCache(cacheKey);
            lock (_cacheLock1)
            {
                if (appTheme == null)
                {
                    appTheme = new AppThemeSystemLimpet(portalId, systemKey);
                    CacheUtils.SetCache(cacheKey, appTheme);
                }
            }
            return appTheme;
        }

        public static AppThemeLimpet AppThemeDefault(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder)
        {
            var cKey = "AppThemeLimpet*" + appThemeFolder + "*" + versionFolder + "*" + portalId + "-" + systemData.SystemKey;
            var appTheme = (AppThemeLimpet)CacheUtils.GetCache(cKey);
            lock (_cacheLock1)
            {
                if (appTheme == null)
                {
                    appTheme = new AppThemeLimpet(portalId, systemData, appThemeFolder, versionFolder);
                    CacheUtils.SetCache(cKey, appTheme);
                }
            }
            return appTheme;
        }

        public static AppThemeLimpet AppTheme(int portalId, string appThemeFolder, string versionFolder, string projectName, bool refresh = false)
        {
            var cKey = "AppThemeLimpet*" + appThemeFolder + "*" + versionFolder + "*" + portalId + "-" + projectName;
            var appTheme = (AppThemeLimpet)CacheUtils.GetCache(cKey);
            lock (_cacheLock1)
            {
                if (appTheme == null || refresh)
                {
                    appTheme = new AppThemeLimpet(portalId, appThemeFolder, versionFolder, projectName);
                    CacheUtils.SetCache(cKey, appTheme);
                }
            }
            return appTheme;
        }

        public static AppThemeDataList AppThemeDataList(string projectName, string systemKey, bool refresh = false)
        {
            var cKey = "AppThemeDataList*" + systemKey + "-" + projectName;
            var appthemedatalistview = (AppThemeDataList)CacheUtils.GetCache(cKey);
            lock (_cacheLock1)
            {
                if (appthemedatalistview == null || refresh)
                {
                    appthemedatalistview = new AppThemeDataList(projectName, systemKey);
                    CacheUtils.SetCache(cKey, appthemedatalistview);
                }
            }
            return appthemedatalistview;
        }
        public static AppThemeProjectLimpet AppThemeProjects(bool refresh = false)
        {
            var cKey = "AppThemeProjectLimpet";
            var appThemeProjectLimpet = (AppThemeProjectLimpet)CacheUtils.GetCache(cKey);
            lock (_cacheLock1)
            {
                if (appThemeProjectLimpet == null || refresh)
                {
                    appThemeProjectLimpet = new AppThemeProjectLimpet();
                    CacheUtils.SetCache(cKey, appThemeProjectLimpet);
                }
            }
            return appThemeProjectLimpet;
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

        #region "GitHub"

        private static readonly object _cacheLock2 = new object();
        //public static List<SimplisityRecord> GetGitHubAppThemes(string projectThemeName)
        //{
        //    var cKey = "GetGitHubAppThemes*" + projectThemeName;
        //    var rtnList = (List<SimplisityRecord>)CacheUtils.GetCache(cKey);
        //    if (rtnList != null) return rtnList;
        //    lock (_cacheLock2)
        //    {
        //        rtnList = new List<SimplisityRecord>();
        //        try
        //        {
        //            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 128 };
        //            var contentsJson = HttpGet($"https://api.github.com/orgs/{org}/repos");
        //            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
        //            foreach (var file in contents)
        //            {
        //                var themename = (string)file["name"];
        //                if (!String.IsNullOrEmpty(themename))
        //                {
        //                    XmlDocument doc = JsonConvert.DeserializeXmlNode("{\"genxml\":" + file.ToString() + "}");
        //                    var sRec = new SimplisityRecord();
        //                    sRec.XMLData = doc.OuterXml;
        //                    rtnList.Add(sRec);
        //                }
        //            }
        //            CacheUtils.SetCache(cKey, rtnList);
        //        }
        //        catch (Exception)
        //        {
        //            // Organisation does not exist, ignore and return empty list
        //        }
        //        return rtnList;
        //    }
        //}
        //public static void DownloadAllGitHubAppTheme(string org)
        //{
        //    var newFolder = false;
        //    var l = GetGitHubAppThemes(org);
        //    foreach (var a in l)
        //    {
        //        if (a.GetXmlNode("genxml/name") != "")
        //        {
        //            var appTheme = AppThemeUtils.AppTheme(PortalUtils.GetPortalId(), a.GetXmlNode("genxml/name"), "", org);
        //            if (!Directory.Exists(appTheme.AppThemeFolderMapPath)) newFolder = true;
        //            DownloadRepoFromGitHub(a.GetXmlProperty("genxml/html_url") + "/archive/refs/heads/main.zip", appTheme.AppThemeFolderMapPath);
        //        }
        //    }
        //    if (newFolder) DNNrocketUtils.RecycleApplicationPool();// recycle so we pickup new AppTheme Folders.
        //    CacheUtils.ClearAllCache();
        //}
        public static void DownloadGitHubAppTheme(string projectThemeUrl, string downloadFolderMapPath)
        {
            DownloadRepoFromGitHub(projectThemeUrl + "/archive/refs/heads/main.zip", downloadFolderMapPath);
            CacheUtils.ClearAllCache();
        }

        private static void DownloadRepoFromGitHub(string projectThemeUrl, string downloadFolderMapPath)
        {
            var zFile = downloadFolderMapPath + "\\main.zip";
            WebClient client = new WebClient();
            client.DownloadFile(projectThemeUrl, zFile);
            // unzip
            var extractDir = downloadFolderMapPath.TrimEnd('\\') + "\\Temp";
            if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
            Directory.CreateDirectory(extractDir);
            DNNrocketUtils.ExtractZipFolder(downloadFolderMapPath + "\\main.zip", extractDir, true);
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
                            if (Directory.Exists(d2)) Directory.Move(d2, rDir);
                        }
                    }
                }
            }
            //delete temp files.
            if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
            if (File.Exists(zFile)) File.Delete(zFile);
        }

        #endregion


    }
}
