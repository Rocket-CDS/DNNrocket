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
        public AppThemeDataList(string selectedsystemkey)
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            AssignFolders();

            PopulateSystemFolderList();

            SelectedSystemKey = selectedsystemkey;

            PopulateAppThemeList();
        }
        private void AssignFolders()
        {
            AppProjectFolderMapPath = DNNrocketUtils.MapPath(AppProjectFolderRel);

            AppProjectThemesFolderRel = AppProjectFolderRel + "/Themes";
            AppProjectThemesFolderMapPath = DNNrocketUtils.MapPath(AppProjectThemesFolderRel);

            AppSystemThemeFolderRootRel = "/DesktopModules/DNNrocket/SystemThemes";
            AppSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRootRel);

            if (!Directory.Exists(AppSystemThemeFolderRootMapPath))
            {
                Directory.CreateDirectory(AppSystemThemeFolderRootMapPath);
                Directory.CreateDirectory(AppSystemThemeFolderRootMapPath.TrimEnd('\\') + "\\dnnrocketmodule"); // included in default install
            }

        }
        public void PopulateAppThemeList()
        {
            List = new List<AppTheme>();
            if (SelectedSystemKey != "")
            {
                var themePath = AppSystemThemeFolderRootMapPath + "\\" + SelectedSystemKey;
                if (Directory.Exists(themePath))
                {
                    var dirlist = System.IO.Directory.GetDirectories(themePath);
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        var appTheme = new AppTheme(SelectedSystemKey, dr.Name, "");
                        List.Add(appTheme);
                    }
                }
            }
        }
        public void PopulateSystemFolderList()
        {
            SystemFolderList = new List<SystemData>();

            // Get the system data from the DB SYSTEM records
            var systemDataList = new SystemDataList();
            foreach (var d in systemDataList.GetSystemList())
            {
                var systemData = new SystemData(d);
                if (systemData.Exists)
                {
                    SystemFolderList.Add(systemData);
                    if (!Directory.Exists(AppSystemThemeFolderRootMapPath.TrimEnd('\\') + "\\" + systemData.SystemKey))
                    {
                        Directory.CreateDirectory(AppSystemThemeFolderRootMapPath.TrimEnd('\\') + "\\" + systemData.SystemKey);
                    }
                }
            }

        }

        public void ClearCacheLists()
        {
            var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
            CacheUtilsDNN.RemoveCache(cachekey);
            cachekey = "AppThemeDataList*" + AppSystemThemeFolderRootMapPath;
            CacheUtilsDNN.RemoveCache(cachekey);
            PopulateSystemFolderList();
            PopulateAppThemeList();
        }

        public void ClearCache()
        {
            SelectedSystemKey = "";
            ClearCacheLists();
        }
        public string AppProjectFolderRel { get; set; }
        public string AppProjectFolderMapPath { get; set; }
        public string AppSystemThemeFolderRootRel { get; set; }
        public string AppSystemThemeFolderRootMapPath { get; set; }
        public string AppProjectThemesFolderRel { get; set; }
        public string AppProjectThemesFolderMapPath { get; set; }
        public string SelectedSystemKey { get; set; }
        public List<AppTheme> List {
            get
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
                if (CacheUtilsDNN.GetCache(cachekey) == null) return new List<AppTheme>();
                return (List<AppTheme>)CacheUtilsDNN.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
                CacheUtilsDNN.SetCache(cachekey, value);
            }
        }
        public List<SystemData> SystemFolderList {
            get
            {
                var cachekey = "AppThemeDataList*" + AppSystemThemeFolderRootMapPath;
                if (CacheUtilsDNN.GetCache(cachekey) == null) return new List<SystemData>();
                return (List<SystemData>)CacheUtilsDNN.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppSystemThemeFolderRootMapPath;
                CacheUtilsDNN.SetCache(cachekey, value);
            }
        }

    }

}
