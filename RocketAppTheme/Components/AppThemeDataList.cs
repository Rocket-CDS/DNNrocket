using DNNrocketAPI;
using DNNrocketAPI.Components;
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
        private int _portalId;
        public AppThemeDataList(int portalId, string projectName)
        {
            _portalId = portalId;
            ProjectName = projectName; // assign first.
            SelectedSystemKey = "";

            AssignFolders();
            PopulateAppThemeList();
        }
        public AppThemeDataList(int portalId, string projectName, string selectedsystemkey)
        {
            _portalId = portalId;
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
                    var appTheme = AppThemeUtils.AppTheme(_portalId, dr.Name, "", org);
                    if (appTheme.FileNameList.Count > 0) list.Add(appTheme);
                }
            }
            List = list;
        }
        public void ClearCacheLists()
        {
            var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath + "*" + SelectedSystemKey;
            CacheUtils.RemoveCache(cachekey);
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
                    var n = a.AppThemeFolder.Replace(SelectedSystemKey + ".","");
                    rtn.Add(a.AppThemeFolder, n);
                }
                return rtn;
            }
        }
        public Dictionary<string, string> NameListDict(bool addEmpty = true)
        {
            var rtn = new Dictionary<string, string>();
            if (addEmpty) rtn.Add("", "");
            foreach (var a in List)
            {
                var n = a.AppThemeFolder.Replace(SelectedSystemKey + ".", "");
                rtn.Add(a.AppThemeFolder, n);
            }
            return rtn;
        }
        public object NameListJson(bool addEmpty = true)
        {
            var jsonList = new List<ValuePair>();
            if (NameList != null)
            {
                var valuePair = new ValuePair();
                valuePair.Key = "";
                valuePair.Value = "";
                if (addEmpty) jsonList.Add(valuePair);
                foreach (var i in NameList)
                {
                    valuePair = new ValuePair();
                    valuePair.Key = i.Key;
                    valuePair.Value = i.Value;
                    jsonList.Add(valuePair);
                }
            }
            return jsonList;
        }

        public List<AppThemeLimpet> List {
            get
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath + "*" + SelectedSystemKey;
                if (CacheUtils.GetCache(cachekey) == null) return new List<AppThemeLimpet>();
                return (List<AppThemeLimpet>)CacheUtils.GetCache(cachekey);
            }
            set
            {
                var cachekey = "AppThemeDataList*" + AppProjectThemesFolderMapPath + "*" + SelectedSystemKey;
                CacheUtils.SetCache(cachekey, value);
            }
        }

    }

}
