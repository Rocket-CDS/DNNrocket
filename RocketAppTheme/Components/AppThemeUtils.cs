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

        public static AppThemeDataList AppThemeDataList(int portalId, string projectName, string systemKey, bool refresh = false)
        {
            var cKey = "AppThemeDataList*" + systemKey + "-" + projectName;
            var appthemedatalistview = (AppThemeDataList)CacheUtils.GetCache(cKey);
            lock (_cacheLock1)
            {
                if (appthemedatalistview == null || refresh)
                {
                    appthemedatalistview = new AppThemeDataList(portalId, projectName, systemKey);
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

        #endregion


    }
}
