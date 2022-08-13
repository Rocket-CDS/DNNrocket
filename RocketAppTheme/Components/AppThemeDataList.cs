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
        public AppThemeDataList(string projectName)
        {
            ProjectName = projectName; // assign first.
            SelectedSystemKey = "";

            AssignFolders();
            PopulateAppThemeList();
        }
        public AppThemeDataList(string projectName, string selectedsystemkey)
        {
            ProjectName = projectName; // assign first.
            SelectedSystemKey = selectedsystemkey;

            AssignFolders();
            PopulateAppThemeList();
        }
        private void AssignFolders()
        {

            AppSystemThemeFolderRootRel = "/DesktopModules/RocketThemes";
            AppSystemThemeFolderRootMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRootRel);
            if (!Directory.Exists(AppSystemThemeFolderRootMapPath)) Directory.CreateDirectory(AppSystemThemeFolderRootMapPath);

            AppThemeProjectNameFolderRootRel = AppSystemThemeFolderRootRel + "/" + ProjectName;
            AppThemeProjectNameFolderRootMapPath = DNNrocketUtils.MapPath(AppThemeProjectNameFolderRootRel);
            if (!Directory.Exists(AppThemeProjectNameFolderRootMapPath)) Directory.CreateDirectory(AppThemeProjectNameFolderRootMapPath);

        }

        public void PopulateAppThemeList()
        {
            var list = new List<AppThemeLimpet>();
            var dirlist1 = System.IO.Directory.GetDirectories(AppThemeProjectNameFolderRootMapPath);
            foreach (var d1 in dirlist1)
            {
                var dirlist = System.IO.Directory.GetDirectories(d1);
                var org = new System.IO.DirectoryInfo(AppThemeProjectNameFolderRootMapPath).Name;
                var dr = new System.IO.DirectoryInfo(d1);
                if (dr.Name.StartsWith(SelectedSystemKey) || SelectedSystemKey == "")
                {
                    var appTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), dr.Name, "", org);
                    if (appTheme.FileNameList.Count > 0) list.Add(appTheme);
                }
            }
            List = list;
        }
        public void ClearCacheLists()
        {
            var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath + "*" + SelectedSystemKey;
            CacheUtilsDNN.RemoveCache(cachekey);
            PopulateAppThemeList();
        }

        public void ClearCache()
        {
            SelectedSystemKey = "";
            ClearCacheLists();
        }

        public string AppThemeProjectNameFolderRootRel { get; set; }
        public string AppThemeProjectNameFolderRootMapPath { get; set; }
        public string AppSystemThemeFolderRootRel { get; set; }
        public string AppSystemThemeFolderRootMapPath { get; set; }
        public string AppProjectThemesFolderRel { get; set; }
        public string AppProjectThemesFolderMapPath { get; set; }
        public string SelectedSystemKey { get; set; }
        public string ProjectName { get; set; }        
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
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath + "*" + SelectedSystemKey;
                if (CacheUtilsDNN.GetCache(cachekey) == null) return new List<AppThemeLimpet>();
                return (List<AppThemeLimpet>)CacheUtilsDNN.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath + "*" + SelectedSystemKey;
                CacheUtilsDNN.SetCache(cachekey, value);
            }
        }

    }

}
