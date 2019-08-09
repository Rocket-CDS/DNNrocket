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

    public class AppThemeDataList
    {
        private static SystemInfoData _systemInfoData;

        public AppThemeDataList(SystemInfoData systemInfoData)
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            _systemInfoData = systemInfoData;

            AssignFolders();

            if (List.Count == 0) Populate();
            if (SystemFolderList.Count == 1) SelectedSystemKey = SystemFolderList.First().SystemKey;
        }
        private void AssignFolders()
        {
            AppProjectFolderMapPath = DNNrocketUtils.MapPath(AppProjectFolderRel);

            AppProjectThemesFolderRel = AppProjectFolderRel + "/Themes";
            AppProjectThemesFolderMapPath = DNNrocketUtils.MapPath(AppProjectThemesFolderRel);

            AppSystemThemeFolderRootRel = AppProjectFolderRel + "/SystemThemes";
            AppSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRootRel);
        }
        public void Populate()
        {
            List = new List<AppTheme>();
            var dirlist = System.IO.Directory.GetDirectories(AppSystemThemeFolderRootMapPath + "\\" + SelectedSystemKey);
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var appTheme = new AppTheme(_systemInfoData, dr.Name);
                List.Add(appTheme);
            }

            SystemFolderList = new List<SystemInfoData>();
            var dirlist2 = System.IO.Directory.GetDirectories(AppSystemThemeFolderRootMapPath);
            foreach (var d in dirlist2)
            {
                var dr = new System.IO.DirectoryInfo(d);
                var systemInfoData = new SystemInfoData(dr.Name);
                if (systemInfoData.Exists) SystemFolderList.Add(systemInfoData);
            }

        }

        public void ClearCache()
        {
            var cachekey = "SelectedSystemKey*" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey);
            cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
            CacheUtils.RemoveCache(cachekey);
            cachekey = "AppThemeDataList*" + AppSystemThemeFolderRootMapPath;
            CacheUtils.RemoveCache(cachekey);
            Populate();
        }

        public void RemoveSelectedSystemKey()
        {
            var cachekey = "SelectedSystemKey*" + DNNrocketUtils.GetCurrentUserId();
            CacheUtils.RemoveCache(cachekey);
        }

        public string AppProjectFolderRel { get; set; }
        public string AppProjectFolderMapPath { get; set; }
        public string AppSystemThemeFolderRootRel { get; set; }
        public string AppSystemThemeFolderRootMapPath { get; set; }
        public string AppProjectThemesFolderRel { get; set; }
        public string AppProjectThemesFolderMapPath { get; set; }
        public string SelectedSystemKey {
            get {
                var cachekey = "SelectedSystemKey*" + DNNrocketUtils.GetCurrentUserId();
                if (CacheUtils.GetCache(cachekey) == null) return "";
                return CacheUtils.GetCache(cachekey).ToString(); ;
            }
            set {
                if (value != "")
                {
                    var cachekey = "SelectedSystemKey*" + DNNrocketUtils.GetCurrentUserId();
                    CacheUtils.SetCache(cachekey, value);
                    Populate();
                }
            }
        }
        public List<AppTheme> List {
            get
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
                if (CacheUtils.GetCache(cachekey) == null) return new List<AppTheme>();
                return (List<AppTheme>)CacheUtils.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
                CacheUtils.SetCache(cachekey, value);
            }
        }
        public List<SystemInfoData> SystemFolderList {
            get
            {
                var cachekey = "AppThemeDataList*" + AppSystemThemeFolderRootMapPath;
                if (CacheUtils.GetCache(cachekey) == null) return new List<SystemInfoData>();
                return (List<SystemInfoData>)CacheUtils.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppSystemThemeFolderRootMapPath;
                CacheUtils.SetCache(cachekey, value);
            }
        }

    }

}
