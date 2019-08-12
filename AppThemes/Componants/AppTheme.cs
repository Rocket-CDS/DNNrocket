using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rocket.AppThemes.Componants
{

    public class AppTheme
    {
        private string _systemKey;
        private string _guidKey;        
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "APPTHEME";
        private DNNrocketController _objCtrl;
        private Dictionary<string, string> _templateDict;
        private bool _debugMode;

        public AppTheme(string systemKey, string appThemeFolder, string langRequired = "", string versionFolder = "", bool debugMode = false)
        {
            _debugMode = debugMode;
            _objCtrl = new DNNrocketController();
            _templateDict = new Dictionary<string, string>();
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            _systemKey = systemKey;

            PopulateVersionList();

            AppThemeFolder = appThemeFolder;
            AppVersionFolder = versionFolder;
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;

            _guidKey = "appTheme*" + _systemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;

            AppSystemThemeFolderRel = AppProjectFolderRel + "/SystemThemes/" + _systemKey;

            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;

            AppCultureCode = langRequired;
            if (AppCultureCode == "") AppCultureCode = DNNrocketUtils.GetEditCulture();

            AppThemeVersionFolderRel = AppThemeFolderRel + "/" + AppVersionFolder;

            ImageFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/img";
            DocumentFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/doc";
            CssFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/css";
            TempFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/temp";
            JsFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/js";
            ResxFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/resx";

            if (AppThemeFolder != "" && _systemKey != "") Populate();
        }

        public void Populate()
        {
            var logoMapPath = AppThemeFolderMapPath + "\\Logo.png";
            Logo = AppThemeFolderRel + "/Logo.png";
            if (!File.Exists(logoMapPath)) Logo = "";

            Info = _objCtrl.GetData(_guidKey, _entityTypeCode, AppCultureCode, -1, -1, false, _tableName);

            AppName = Info.GetXmlProperty("genxml/textbox/appname");
            AppDisplayName = Info.GetXmlProperty("genxml/lang/genxml/textbox/displayname");
            AppSummary = Info.GetXmlProperty("genxml/lang/genxml/textbox/summary");

            // only edit system level template.
            // get file data for each template file
            var templatelist = Info.GetList("templatelist");
            foreach (var t in templatelist)
            {
                var templateName = t.GetXmlProperty("genxml/hidden/filename");
                if (Path.GetExtension(templateName) == "") templateName += ".cshtml";
                var templateText = FileUtils.ReadFile(AppThemeVersionFolderMapPath + "\\default\\" + templateName);
                if (!_templateDict.ContainsKey(templateName)) _templateDict.Add(templateName, templateText);
                Info.SetXmlProperty("genxml/templatelist/genxml[" + t.GetXmlProperty("genxml/index") + "]/hidden/editorcode", GeneralUtils.EnCode(templateText));
            }



        }
        private void CreateVersionFolders(string versionFolder)
        {
            if (!Directory.Exists(AppThemeFolderMapPath + "\\" + versionFolder))
            {
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder);
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\css");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\default");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\resx");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\js");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\img");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\temp");
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\doc");
            }
        }

        public void DeleteTemplateFile(string templateName)
        {
        }

        public string GetTemplate(string templateName)
        {
            if (Path.GetExtension(templateName) == "") templateName += ".cshtml";
            if (!_templateDict.ContainsKey(templateName)) return "";
            return _templateDict[templateName];
        }

        public void DeleteTheme()
        {
            if (Directory.Exists(AppThemeFolderMapPath))
            {
                Directory.Delete(AppThemeFolderMapPath, true);
            }
        }
        public void DeleteVersion(string versionFolder)
        {
            if (Directory.Exists(AppThemeFolderMapPath + "\\" + versionFolder))
            {
                Directory.Delete(AppThemeFolderMapPath + "\\" + versionFolder, true);
            }
        }
        public void Save(SimplisityInfo postInfo)
        {
            //get removed templates (To be deleted)
            var templateDict2 = _templateDict;

            var dbInfo = _objCtrl.GetData(_entityTypeCode, Info.ItemID, AppCultureCode, -1, -1, true, _tableName);
            if (dbInfo != null)
            {
                // NOTE: Be careful of the order for saving.  The sort list is a PAIN and required speciifc data.
                // 1 - Create any empty record. (Must be done, so on first save we get the data in the DB)
                // 2 - Apply new XML strcuture 
                // 3 - Do sort list.
                // 4 - Save the new list data to the DB.

                // update all langauge record which are empty.
                var cc = DNNrocketUtils.GetCultureCodeList();
                foreach (var l in cc)
                {
                    var dbRecord = _objCtrl.GetRecordLang(Info.ItemID, l, false, _tableName);
                    var nodList = dbRecord.XMLDoc.SelectNodes("genxml/*");
                    if (nodList.Count == 0)
                    {
                        var dbInfo2 = _objCtrl.GetData(_entityTypeCode, Info.ItemID, l, -1, -1, true, _tableName);
                        if (dbInfo2 != null)
                        {
                            dbInfo2.XMLData = postInfo.XMLData;
                            _objCtrl.SaveData(dbInfo2, Info.ItemID, _tableName);
                        }
                    }
                }

                dbInfo.XMLData = postInfo.XMLData;
                //_objCtrl.SaveData(dbInfo, Info.ItemID, _tableName); // save before list sort, so we have hte data in DB.

                // sort lists from DB and post data
                var sortLists = new DNNrocketAPI.Componants.SortLists(dbInfo, _tableName, false);
                sortLists.Save();

                Populate();

                // delete removed templates
                var deleteList = new List<string>();
                foreach (var t in templateDict2)
                {
                    if (!_templateDict.ContainsKey(t.Key))
                    {
                        deleteList.Add(t.Key);
                    }
                }

                foreach (var d in deleteList)
                {
                    File.Delete(AppThemeVersionFolderMapPath + "\\default\\" + d);
                }

            }
        }
        public void Update()
        {
            _objCtrl.SaveData(Info, -1, _tableName);
        }
        public void AddListImage()
        {
            Info.AddListItem("imagelist");
            Update();
        }
        public void AddListTemplate()
        {
            Info.AddListItem("templatelist");
            Update();
        }

        public void Export(SimplisityInfo _postInfo)
        {
            if (AppThemeFolder != "")
            {
                var fileMapPath = AppThemeFolderMapPath + "\\meta_" + AppCultureCode + ".xml";
                File.WriteAllText(fileMapPath,  _postInfo.ToXmlItem());
            }
        }
        public void Import()
        {
            var fileMapPath = AppThemeFolderMapPath + "\\meta_" + AppCultureCode + ".xml";
            if (File.Exists(fileMapPath))
            {
                var fileImport = File.ReadAllText(fileMapPath);
                Info.FromXmlItem(fileImport);
            }
        }

        public void CopyVersion(string sourceVersionFolder, string destVersionFolder)
        {
            sourceVersionFolder = AppThemeFolderMapPath + "\\" + sourceVersionFolder;
            destVersionFolder = AppThemeFolderMapPath + "\\" + destVersionFolder;

            if (Directory.Exists(sourceVersionFolder))
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourceVersionFolder, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceVersionFolder, destVersionFolder));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourceVersionFolder, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourceVersionFolder, destVersionFolder), true);
            }
        }

        public void PopulateVersionList()
        {
            LatestVersionFolder = "1.0";
            if (AppThemeFolder != "")
            {
                VersionList = new List<string>();
                if (System.IO.Directory.Exists(AppThemeFolderMapPath))
                {
                    var dirlist = System.IO.Directory.GetDirectories(AppThemeFolderMapPath);
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        VersionList.Add(dr.Name);
                    }
                }
                if (VersionList.Count == 0) VersionList.Add("1.0");
                VersionList.Reverse();
                LatestVersionFolder = (string)VersionList.First();
            }
        }


        #region "properties"

        public string AppProjectFolderRel { get; set; }
        public string AppProjectFolderMapPath { get { return DNNrocketUtils.MapPath(AppProjectFolderRel); }  }
        public string AppSystemThemeFolderRel { get; set; }
        public string AppSystemThemeFolderMapPath { get { return DNNrocketUtils.MapPath(AppSystemThemeFolderRel); } }
        public string AppName { get; private set; }
        public string AppDisplayName { get; private set; }
        public string AppSummary { get; private set; }
        public string Logo { get; private set; }
        public string AppCultureCode { get; set; }
        public string AppThemeFolder { get; set; }
        public string AppThemeFolderRel { get; set; }
        public string AppThemeFolderMapPath { get { return DNNrocketUtils.MapPath(AppThemeFolderRel); } }
        public string AppVersionFolder { get; set; }
        public string AppThemeVersionFolderRel { get; set; }
        public string AppThemeVersionFolderMapPath { get { return DNNrocketUtils.MapPath(AppThemeVersionFolderRel); } }
        public List<string> VersionList { get; set; }
        public string LatestVersionFolder { get; set; }
        public string ImageFolderMapPath { get { return DNNrocketUtils.MapPath(ImageFolderRel); } }
        public string DocumentFolderMapPath { get { return DNNrocketUtils.MapPath(DocumentFolderRel); } }
        public string CssFolderMapPath { get { return DNNrocketUtils.MapPath(CssFolderRel); } }
        public string TempFolderMapPath { get { return DNNrocketUtils.MapPath(TempFolderRel); } }
        public string JsFolderMapPath { get { return DNNrocketUtils.MapPath(JsFolderRel); } }
        public string ResxFolderMapPath { get { return DNNrocketUtils.MapPath(ResxFolderRel); } }
        public string ImageFolderRel { get; set; }
        public string DocumentFolderRel { get; set; }
        public string CssFolderRel { get; set; }
        public string TempFolderRel { get; set; }
        public string JsFolderRel { get; set; }
        public string ResxFolderRel { get; set; }
        public SimplisityInfo Info { get; set; }

        #endregion

    }

}
