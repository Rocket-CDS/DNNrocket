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
        public AppThemeDataPrivateList(string selectedsystemkey)
        {
            try
            {
                ErrorMsg = "";
                Error = false;
                SelectedSystemKey = selectedsystemkey;

                var cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
                SystemFolderList = (List<SystemInfoData>)CacheUtils.GetCache(cachekey);
                if (SystemFolderList == null) PopulateSystemFolderList();

                cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
                List = (List<SimplisityInfo>)CacheUtils.GetCache(cachekey);
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
            List = new List<SimplisityInfo>();
            if (SelectedSystemKey != "")
            {
                var ftpConnect = new FtpConnect(SelectedSystemKey);
                var l = ftpConnect.DownloadAppThemeXmlList();

                // check verison
                foreach (SimplisityInfo a in l)
                {
                    var appTheme = new AppTheme(SelectedSystemKey, a.GetXmlProperty("genxml/hidden/appthemefolder"));
                    a.SetXmlProperty("genxml/hidden/localversion", appTheme.LatestVersionFolder);
                    a.SetXmlProperty("genxml/hidden/localrev", appTheme.LatestRev.ToString());
                    a.SetXmlProperty("genxml/hidden/islatestversion", "False");
                    if (a.GetXmlPropertyDouble("genxml/hidden/latestversion") == Convert.ToDouble(appTheme.LatestVersionFolder))
                    {
                        a.SetXmlProperty("genxml/hidden/islatestversion", "True");
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

                    List.Add(a);
                }
                var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
                CacheUtils.SetCache(cachekey, List);
            }
        }
        public void PopulateSystemFolderList()
        {
            var appSystemThemeFolderRootRel = "/DesktopModules/DNNrocket/AppThemes/SystemThemes";
            var appSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(appSystemThemeFolderRootRel);

            SystemFolderList = new List<SystemInfoData>();
            var dirlist2 = System.IO.Directory.GetDirectories(appSystemThemeFolderRootMapPath);
            foreach (var d in dirlist2)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var systemInfoData = new SystemInfoData(dr.Name);
                if (systemInfoData.Exists) SystemFolderList.Add(systemInfoData);
            }

            var cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.SetCache(cachekey, SystemFolderList);

        }
        public void ClearCache()
        {
            SelectedSystemKey = "";
            var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey);
            cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey);
        }
        public string SelectedSystemKey { get; set; }
        public List<SimplisityInfo> List { get; set; }
        public List<SystemInfoData> SystemFolderList { get; set; }
        public bool Error { get; set; }
        public string ErrorMsg { get; set; }

    }

}
