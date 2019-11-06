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
            SelectedSystemKey = selectedsystemkey;
            if (List.Count == 0) PopulateAppThemeList();
        }
        public void PopulateAppThemeList()
        {
            List = new List<SimplisityInfo>();
            if (SelectedSystemKey != "")
            {
                var ftpConnect = new FtpConnect(SelectedSystemKey);
                List = ftpConnect.DownloadAppThemeXmlList();
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
        }
        public void ClearCache()
        {
            SelectedSystemKey = "";
            var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey);
            PopulateAppThemeList();
        }
        public string SelectedSystemKey { get; set; }
        public List<SimplisityInfo> List {
            get
            {
                var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
                if (CacheUtils.GetCache(cachekey) == null) return new List<SimplisityInfo>();
                return (List<SimplisityInfo>)CacheUtils.GetCache(cachekey);
            }
            set
            {
                var cachekey = AppThemeListType + "*" + DNNrocketUtils.GetCurrentUserId();
                CacheUtils.SetCache(cachekey, value);
            }
        }

        public List<SystemInfoData> SystemFolderList
        {
            get
            {
                var cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
                if (CacheUtils.GetCache(cachekey) == null) return new List<SystemInfoData>();
                return (List<SystemInfoData>)CacheUtils.GetCache(cachekey);
            }
            set
            {
                var cachekey = AppThemeListType + "*SystemFolders" + DNNrocketUtils.GetCurrentUserId();
                CacheUtils.SetCache(cachekey, value);
            }
        }


    }

}
