using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Componants
{

    public class AppThemeDataPrivateList
    {
        private const string AppThemeListType = "AppThemeDataPrivateList";
        private const string AppSystemFolderRel = "/DesktopModules/RocketThemes";
        public AppThemeDataPrivateList(string selectedsystemkey, bool useCache)
        {
            try
            {
                ErrorMsg = "";
                Error = false;
                SelectedSystemKey = selectedsystemkey;

                var cachekey = AppThemeListType + "*SystemFolders" + UserUtils.GetCurrentUserId();
                if (useCache) SystemFolderList = (List<SystemLimpet>)CacheUtilsDNN.GetCache(cachekey);
                if (SystemFolderList == null) PopulateSystemFolderList();

                cachekey = AppThemeListType + "*" + UserUtils.GetCurrentUserId();
                if (useCache)
                {
                    List = (List<SimplisityRecord>)CacheUtilsDNN.GetCache(cachekey);
                    var cacheRtn = CacheUtilsDNN.GetCache(cachekey + "ERROR");
                    if (cacheRtn == null)
                    {
                        Error = false;
                    }
                    else
                    {
                        Error = Convert.ToBoolean(cacheRtn);
                    }
                }
                if (List == null) PopulateAppThemeList();
            }
            catch (Exception exc)
            {
                ErrorMsg = exc.ToString();
                Error = true; 
            }
        }
        public void PopulateAppThemeList()
        {
            List = new List<SimplisityRecord>();
            if (SelectedSystemKey != "")
            {
                var ftpConnect = new FtpConnect(SelectedSystemKey);
                if (ftpConnect.IsValid)
                {
                    var l = ftpConnect.DownloadAppThemeXmlIndexList();
                    List<SimplisityRecord> SortedList = l.OrderBy(o => o.GetXmlProperty("genxml/hidden/appthemefolder")).ToList();
                    foreach (SimplisityRecord a in SortedList)
                    {
                        //get local directory and check if exists
                        var localdir = AppSystemFolderRel + "/" + SelectedSystemKey + "/" + a.GetXmlProperty("genxml/hidden/appthemefolder");
                        var localdirMapPath = DNNrocketUtils.MapPath(localdir);
                        if (Directory.Exists(localdirMapPath))
                        {
                            var appTheme = new AppTheme(SelectedSystemKey, a.GetXmlProperty("genxml/hidden/appthemefolder"));

                            // update list data
                            a.SetXmlProperty("genxml/hidden/localversion", appTheme.LatestVersionFolder);
                            a.SetXmlProperty("genxml/hidden/islatestversion", "False");
                            a.SetXmlProperty("genxml/hidden/exists", "True");

                            if (a.GetXmlPropertyDouble("genxml/hidden/latestversion") == a.GetXmlPropertyDouble("genxml/hidden/localversion"))
                            {
                                a.SetXmlProperty("genxml/hidden/islatestversion", "True");
                            }

                            a.SetXmlProperty("genxml/hidden/localupdated", "False");
                            if (a.GetXmlPropertyDouble("genxml/hidden/latestversion") < a.GetXmlPropertyDouble("genxml/hidden/localversion"))
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
                        ////var onlineIndex = (OnlineAppThemeIndex)CacheUtilsDNN.GetCache(SelectedSystemKey + "_privateIndex");
                        ////if (onlineIndex == null)
                        ////{
                        ////    onlineIndex = new OnlineAppThemeIndex(SelectedSystemKey, "private");
                        ////    CacheUtilsDNN.SetCache(SelectedSystemKey + "_privateIndex", onlineIndex);
                        ////}

                        //a.SetXmlProperty("genxml/hidden/logobase64", onlineIndex.GetLogoBase64String(appTheme.AppThemeFolder));

                        List.Add(a);
                    }
                }
                else
                {
                    Error = true;
                    ErrorMsg = "";
                }
                var cachekey = AppThemeListType + "*" + UserUtils.GetCurrentUserId();
                CacheUtilsDNN.SetCache(cachekey, List);
                CacheUtilsDNN.SetCache(cachekey + "ERROR", Error);
            }
        }
        public void PopulateSystemFolderList()
        {
            var appSystemThemeFolderRootRel = "/DesktopModules/RocketThemes";
            var appSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(appSystemThemeFolderRootRel);

            SystemFolderList = new List<SystemLimpet>();
            var dirlist2 = System.IO.Directory.GetDirectories(appSystemThemeFolderRootMapPath);
            foreach (var d in dirlist2)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var systemData = new SystemLimpet(dr.Name);
                if (systemData.Exists) SystemFolderList.Add(systemData);
            }

            var cachekey = AppThemeListType + "*SystemFolders" + UserUtils.GetCurrentUserId();
            CacheUtilsDNN.SetCache(cachekey, SystemFolderList);

        }
        public void ClearCache()
        {
            SelectedSystemKey = "";
            var cachekey = AppThemeListType + "*" + UserUtils.GetCurrentUserId();
            CacheUtilsDNN.RemoveCache(cachekey);
            cachekey = AppThemeListType + "*SystemFolders" + UserUtils.GetCurrentUserId();
            CacheUtilsDNN.RemoveCache(cachekey);
        }
        public string SelectedSystemKey { get; set; }
        public List<SimplisityRecord> List { get; set; }
        public List<SystemLimpet> SystemFolderList { get; set; }
        public bool Error { get; set; }
        public string ErrorMsg { get; set; }

    }

}
