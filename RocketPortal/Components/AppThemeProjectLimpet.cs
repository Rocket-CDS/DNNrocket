﻿using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Components
{

    public class AppThemeProjectLimpet
    {
        private const string _entityTypeCode = "APPTHEMEPROJECTS";
        private const string _listName = "appthemeprojects";
        private const string _tableName = "DNNrocket";
        private const string _rocketThemesPath = "\\DesktopModules\\RocketThemes";
        private DNNrocketController _objCtrl;

        public SimplisityRecord Record;

        public AppThemeProjectLimpet()
        {
            _objCtrl = new DNNrocketController();
            Record = _objCtrl.GetRecordByGuidKey(-1, -1, _entityTypeCode, _entityTypeCode, "", _tableName);
            if (Record == null)
            {
                Record = new SimplisityRecord();
                Record.GUIDKey = _entityTypeCode;
                Record.TypeCode = _entityTypeCode;
                Record.ModuleId = -1;
                Record.PortalId = -1;
            }
            if (List.Count == 0)
            {
                // setup defaults
                var defaultFileMapPath = "/DesktopModules/DNNrocket/RocketPortal/Installation/SystemDefaults.rules";
                var filenamepath = DNNrocketUtils.MapPath(defaultFileMapPath);
                var xmlString = FileUtils.ReadFile(filenamepath);
                var s = new SimplisityInfo();
                s.XMLData = xmlString;
                foreach (XmlNode orgNod in s.XMLDoc.SelectNodes("root/appthemeprojects/*"))
                {
                    Record.AddListItem(_listName, orgNod.OuterXml);
                }
                Update();
            }
            var themeRoot = DNNrocketUtils.MapPath(_rocketThemesPath);
            if (!Directory.Exists(themeRoot)) Directory.CreateDirectory(themeRoot);

        }
        public void Delete()
        {
            if (Record.ItemID > 0)
            {
                _objCtrl.Delete(Record.ItemID);
                Record = null;
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
            Record.XMLData = postInfo.XMLData;
            Update();
        }

        public void Update()
        {
            Record.ItemID = _objCtrl.Update(Record, _tableName);
        }
        public void AddRow()
        {
            var s = new SimplisityRecord();
            s.SetXmlProperty("genxml/textbox/githubrepourl", "");
            s.SetXmlProperty("genxml/textbox/githubusername", "");
            s.SetXmlProperty("genxml/textbox/githubtoken", "");
            s.SetXmlProperty("genxml/textbox/name", "");
            s.SetXmlProperty("genxml/checkbox/active", "");
            s.SetXmlProperty("genxml/checkbox/default", "");
            Record.AddRecordListItem(_listName, s);
            Update();
        }
        public void DeleteRow(int idx)
        {
            Record.RemoveRecordListItem(_listName, idx);
            Update();
        }
        public bool Active(int idx)
        {
            return Record.GetXmlPropertyBool("genxml/" + _listName + "/genxml[" + idx + 1 + "]/checkbox/active");
        }
        public void Active(int idx, bool value)
        {
            Record.SetXmlProperty("genxml/" + _listName + "/genxml[" + idx + 1 + "]/checkbox/active", value.ToString());
            Update();
        }
        public string ExportData(bool withTextData = false)
        {
            var xmlOut = "<root>";
            xmlOut += Record.ToXmlItem(withTextData);
            xmlOut += "</root>";

            return xmlOut;
        }

        public void ImportData(string XmlIn)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlIn);

            var nodList = xmlDoc.SelectNodes("root/item");
            foreach (XmlNode nod in nodList)
            {
                Record.FromXmlItem(nod.OuterXml);
            }
        }
        public string DefaultProjectUrl()
        {
            var dl = Record.GetRecordList(_listName);
            foreach (var d in dl)
            {
                if (d.GetXmlPropertyBool("genxml/checkbox/default")) return d.GetXmlProperty("genxml/textbox/githubrepourl");
            }
            if (dl.Count == 0) return "";
            return dl.First().GetXmlProperty("genxml/textbox/githubrepourl");
        }
        public string DefaultProjectName()
        {
            var dl = Record.GetRecordList(_listName);
            foreach (var d in dl)
            {
                if (d.GetXmlPropertyBool("genxml/checkbox/default")) return d.GetXmlProperty("genxml/textbox/name");
            }
            if (dl.Count == 0) return "";
            return dl.First().GetXmlProperty("genxml/textbox/name");
        }
        public string ProjectUrl(string projectName)
        {
            var dl = Record.GetRecordList(_listName);
            foreach (var d in dl)
            {
                if (d.GetXmlProperty("genxml/textbox/name") == projectName) return d.GetXmlProperty("genxml/textbox/githubrepourl");
            }
            return DefaultProjectUrl();
        }
        public string GitHubToken(string projectName)
        {
            var dl = Record.GetRecordList(_listName);
            foreach (var d in dl)
            {
                if (d.GetXmlProperty("genxml/textbox/name") == projectName) return d.GetXmlProperty("genxml/textbox/githubtoken");
            }
            return "";
        }

        public Dictionary<string, string> ActiveList()
        {
            var orgDict = new Dictionary<string, string>();
            foreach (SimplisityRecord o in List)
            {
                if (o.GetXmlPropertyBool("genxml/checkbox/active"))
                {
                    var org = o.GetXmlProperty("genxml/textbox/name");
                    if (!orgDict.ContainsKey(org)) orgDict.Add(org, o.GetXmlProperty("genxml/textbox/name"));
                }
            }
            return orgDict;
        }
        public List<AppThemeLimpet> GetAppThemeList(string projectName)
        {
            var rtn = new List<AppThemeLimpet>();
            foreach (var app in List)
            {
                var name = app.GetXmlProperty("genxml/textbox/name");
                if (name == projectName)
                {
                    foreach (var d2 in Directory.GetDirectories(DNNrocketUtils.MapPath(_rocketThemesPath + "\\" + projectName)))
                    {
                        string dirName = new DirectoryInfo(d2).Name;
                        if (dirName != "")
                        {
                            var a = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), dirName, "", projectName);
                            if (a.Exists) rtn.Add(a);
                        }
                    }


                }
            }
            return rtn;
        }

        public void DownloadGitHubProject(string projectName)
        {
            LogUtils.LogSystem("DownloadGitHubProject: " + projectName + " - START");

            var githubToken = GitHubToken(projectName);
            var projectThemeUrl = ProjectUrl(projectName);
            projectThemeUrl = projectThemeUrl.ToLower().Replace(".git", "") + "/archive/refs/heads/main.zip";

            using (var client = new System.Net.Http.HttpClient())
            {
                var zFile = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\" + projectName + "_main.zip";
                var credentials = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:", githubToken);
                credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", githubToken);
                var contents = client.GetByteArrayAsync(projectThemeUrl).Result;
                System.IO.File.WriteAllBytes(zFile, contents);
                LogUtils.LogSystem("Download: " + zFile);

                // unzip
                var extractDir = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\TempExtractDir";
                if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);
                DNNrocketUtils.ExtractZipFolder(zFile, extractDir, true);

                //check for project folder.
                var destRoot = DNNrocketUtils.MapPath(_rocketThemesPath + "\\" + projectName);
                if (!Directory.Exists(destRoot)) Directory.CreateDirectory(destRoot);

                // check for correct folder and move to AppTheme. (3rd level)
                foreach (var d in Directory.GetDirectories(extractDir))
                {
                    foreach (var d2 in Directory.GetDirectories(d))
                    {
                        string dirName = new DirectoryInfo(d2).Name;
                        if (dirName != "")
                        {
                            try
                            {
                                var rDir = d + "\\" + dirName;
                                var dest = DNNrocketUtils.MapPath(_rocketThemesPath + "\\" + projectName + "\\" + dirName);
                                if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
                                Directory.Delete(dest, true);
                                Directory.Move(rDir, dest);
                                LogUtils.LogSystem("MOVE: "  + rDir + " --> " + dest);
                            }
                            catch (Exception ex)
                            {
                                LogUtils.LogException(ex);
                                LogUtils.LogSystem("ERROR: " + ex.ToString());
                            }
                        }
                    }
                }
                LogUtils.LogSystem("DownloadGitHubProject: " + projectName + " - END");

                //delete temp files.
                CacheUtils.ClearAllCache();
                if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                if (File.Exists(zFile))
                {
                    LogUtils.LogSystem("RecycleApplicationPool");
                    File.Delete(zFile);
                    DNNrocketUtils.RecycleApplicationPool();// recycle so we pickup new AppTheme Folders.
                }

            }

        }

        public List<SimplisityRecord> List { get { return Record.GetRecordList(_listName); } }


    }

}