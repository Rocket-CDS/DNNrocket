using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Rocket.AppThemes.Componants
{

    public class AppThemeDataPublicList
    {
        private const string AppThemeListType = "AppThemeDataPublicList";
        private const string AppSystemFolderRel = "/DesktopModules/DNNrocket/SystemThemes";
        public AppThemeDataPublicList(string selectedsystemkey, bool useCache)
        {
            try
            {
                ErrorMsg = "";
                Error = false;
                SelectedSystemKey = selectedsystemkey;

                var cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
                if (useCache) SystemFolderList = (List<SystemData>)CacheUtils.GetCache(cachekey, "apptheme");
                if (SystemFolderList == null) PopulateSystemFolderList();

                cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
                if (useCache) List = (List<SimplisityRecord>)CacheUtils.GetCache(cachekey, "apptheme");
                if (List == null) PopulateAppThemeList();
            }
            catch (Exception exc)
            {
                ErrorMsg = exc.ToString();
                Error = true; 
            }
        }
        private List<SimplisityRecord> DownloadAppThemeXmlList()
        {
            var httpConnect = new HttpConnect(SelectedSystemKey);
            var rtnList = httpConnect.DownloadAppThemeXmlList();
            return rtnList;
        }

        public void PopulateAppThemeList()
        {
            List = new List<SimplisityRecord>();
            if (SelectedSystemKey != "")
            {

                var l = DownloadAppThemeXmlList();

                List<SimplisityRecord> SortedList = l.OrderBy(o => o.GetXmlProperty("genxml/hidden/appthemefolder")).ToList();

                foreach (SimplisityRecord a in SortedList)
                {
                    //get local directory and check if exists
                    var appTheme = new AppTheme(SelectedSystemKey, a.GetXmlProperty("genxml/hidden/appthemefolder"));
                    var localdir = AppSystemFolderRel + "/" + SelectedSystemKey + "/" + a.GetXmlProperty("genxml/hidden/appthemefolder");
                    var localdirMapPath = DNNrocketUtils.MapPath(localdir);
                    if (Directory.Exists(localdirMapPath))
                    {

                        // update list data
                        a.SetXmlProperty("genxml/hidden/localversion", appTheme.LatestVersionFolder);
                        a.SetXmlProperty("genxml/hidden/localrev", appTheme.LatestRev.ToString());
                        a.SetXmlProperty("genxml/hidden/islatestversion", "False");
                        a.SetXmlProperty("genxml/hidden/exists", "True");
                        if (a.GetXmlPropertyDouble("genxml/hidden/latestversion") == Convert.ToDouble(appTheme.LatestVersionFolder))
                        {
                            if (a.GetXmlPropertyDouble("genxml/hidden/latestrev") == Convert.ToDouble(appTheme.LatestRev))
                            {
                                a.SetXmlProperty("genxml/hidden/islatestversion", "True");
                            }
                        }

                        a.SetXmlProperty("genxml/hidden/localupdated", "False");
                        if (a.GetXmlPropertyDouble("genxml/hidden/latestversion") < Convert.ToDouble(appTheme.LatestVersionFolder))
                        {
                            a.SetXmlProperty("genxml/hidden/localupdated", "True");
                        }
                        if (a.GetXmlPropertyInt("genxml/hidden/latestrev") < appTheme.LatestRev)
                        {
                            a.SetXmlProperty("genxml/hidden/localupdated", "True");
                        }
                    }
                    else
                    {
                        a.SetXmlProperty("genxml/hidden/islatestversion", "False");
                        a.SetXmlProperty("genxml/hidden/exists", "False");
                    }

                    // ensure we have the image from the xml file
                    var onlineIndex = (OnlineAppThemeIndex)CacheUtils.GetCache(appTheme.SystemKey + "_publicIndex");
                    if (onlineIndex == null)
                    {
                        onlineIndex = new OnlineAppThemeIndex(appTheme.SystemKey, "public");
                        CacheUtils.SetCache(appTheme.SystemKey + "_publicIndex", onlineIndex);
                    }
                    a.SetXmlProperty("genxml/hidden/logobase64", onlineIndex.GetLogoBase64String(appTheme.AppThemeFolder));

                    List.Add(a);
                }
                var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
                CacheUtils.SetCache(cachekey, List, "apptheme");
            }
        }
        public void PopulateSystemFolderList()
        {
            var appSystemThemeFolderRootRel = "/DesktopModules/DNNrocket/SystemThemes";
            var appSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(appSystemThemeFolderRootRel);

            SystemFolderList = new List<SystemData>();
            var dirlist2 = System.IO.Directory.GetDirectories(appSystemThemeFolderRootMapPath);
            foreach (var d in dirlist2)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var systemData = new SystemData(dr.Name);
                if (systemData.Exists) SystemFolderList.Add(systemData);
            }

            var cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.SetCache(cachekey, SystemFolderList, "apptheme");

        }
        public void ClearCache()
        {
            SelectedSystemKey = "";
            var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey, "apptheme");
            cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey, "apptheme");
        }
        public string SelectedSystemKey { get; set; }
        public List<SimplisityRecord> List { get; set; }
        public List<SystemData> SystemFolderList { get; set; }
        public bool Error { get; set; }
        public string ErrorMsg { get; set; }

    }

}
