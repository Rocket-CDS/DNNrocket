using DNNrocketAPI;
using DNNrocketAPI.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Components
{

    public class AppThemeDataList
    {
        public AppThemeDataList()
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            AssignFolders();

            SelectedSystemKey = "";

            PopulateAppThemeList();
        }
        public AppThemeDataList(string selectedsystemkey)
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            AssignFolders();

            SelectedSystemKey = selectedsystemkey;

            PopulateAppThemeList();
        }
        private void AssignFolders()
        {
            AppProjectFolderMapPath = DNNrocketUtils.MapPath(AppProjectFolderRel);

            AppProjectThemesFolderRel = AppProjectFolderRel + "/Themes";
            AppProjectThemesFolderMapPath = DNNrocketUtils.MapPath(AppProjectThemesFolderRel);

            AppSystemThemeFolderRootRel = "/DesktopModules/RocketThemes";
            AppSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRootRel);

        }

        public void PopulateAppThemeList()
        {
            List = new List<AppThemeLimpet>();
            var dirlist = System.IO.Directory.GetDirectories(AppSystemThemeFolderRootMapPath);
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                if (dr.Name.StartsWith(SelectedSystemKey) || SelectedSystemKey == "")
                {
                    var appTheme = new AppThemeLimpet(dr.Name, "");
                    if (appTheme.FileNameList.Count > 0) List.Add(appTheme);
                }
            }
        }
        public void ClearCacheLists()
        {
            var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
            CacheUtilsDNN.RemoveCache(cachekey);
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
        public Dictionary<string,string> NameList
        {
            get
            {
                var rtn = new Dictionary<string, string>();
                foreach (var a in List)
                {
                    var n = a.AppThemeFolder;
                    if (n == "") n = a.AppThemeFolder;
                    rtn.Add(a.AppThemeFolder, n);
                }
                return rtn;
            }
        }
        public List<AppThemeLimpet> List {
            get
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
                if (CacheUtilsDNN.GetCache(cachekey) == null) return new List<AppThemeLimpet>();
                return (List<AppThemeLimpet>)CacheUtilsDNN.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath;
                CacheUtilsDNN.SetCache(cachekey, value);
            }
        }

    }

}
