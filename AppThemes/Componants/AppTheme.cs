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
        private static string _systemKey;
        private static string _guidKey;        
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "APPTHEME";
        private DNNrocketController _objCtrl;

        public AppTheme(string systemKey, string appThemeFolder, string langRequired = "", string versionFolder = "")
        {
            _objCtrl = new DNNrocketController();

            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";

            _systemKey = systemKey;

            PopulateVersionList();

            AppThemeFolder = appThemeFolder;

            AppVersionFolder = versionFolder;
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;

            _guidKey = "appTheme*" + _systemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;

            AppSystemThemeFolderRel = AppProjectFolderRel + "/SystemThemes/" + _systemKey;
            AppSystemThemeFolderMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRel);

            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);

            AppCultureCode = langRequired;
            if (AppCultureCode == "") AppCultureCode = DNNrocketUtils.GetEditCulture();

            AppThemeVersionFolderRel = AppThemeFolderRel + "/" + AppVersionFolder;
            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolderRel);

            ImageFolderMapPath = AppThemeFolderMapPath + "\\" + AppVersionFolder + "\\img";
            DocumentFolderMapPath = AppThemeFolderMapPath + "\\" + AppVersionFolder + "\\doc";
            CssFolderMapPath = AppThemeFolderMapPath + "\\" + AppVersionFolder + "\\css";
            TempFolderMapPath = AppThemeFolderMapPath + "\\" + AppVersionFolder + "\\temp";
            JsFolderMapPath = AppThemeFolderMapPath + "\\" + AppVersionFolder + "\\js";
            ResxFolderMapPath = AppThemeFolderMapPath + "\\" + AppVersionFolder + "\\resx";

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
            var themeFolderPath = "SystemThemes\\" + _systemKey + "\\" + AppThemeFolder + "\\" + AppVersionFolder;

            var templCtrl = new Simplisity.TemplateEngine.TemplateGetter(DNNrocketUtils.HomeDirectory(), themeFolderPath, AppProjectFolderRel);
            ActiveViewTemplate = templCtrl.GetTemplateData("view.cshtml", AppCultureCode);
            if (ActiveViewTemplate == null) ActiveViewTemplate = "";
            ActiveEditTemplate = templCtrl.GetTemplateData("edit.cshtml", AppCultureCode);
            if (ActiveEditTemplate == null) ActiveEditTemplate = "";
            ActivePageHeaderTemplate = templCtrl.GetTemplateData("pageheader.cshtml", AppCultureCode);
            if (ActivePageHeaderTemplate == null) ActivePageHeaderTemplate = "";

            var logoMapPath = AppThemeFolderMapPath + "\\Logo.png";
            Logo = AppThemeFolderRel + "/Logo.png";
            if (!File.Exists(logoMapPath)) Logo = "";

            Info = _objCtrl.GetData(_guidKey, _entityTypeCode, AppCultureCode, -1, -1, false, _tableName);

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
        public string AppSystemThemeFolderRel { get; set; }
        public string AppSystemThemeFolderMapPath { get; set; }
        public string AppName { get; set; }
        public string AppDisplayName { get; set; }
        public string AppSummary { get; set; }
        public string Logo { get; private set; }
        public string AppCultureCode { get; set; }
        public string AppThemeFolder { get; set; }
        public string AppThemeFolderRel { get; set; }
        public string AppThemeFolderMapPath { get; set; }
        public string AppVersionFolder { get; set; }
        public string AppThemeVersionFolderRel { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
        public string ActiveViewTemplate { get; set; }
        public string ActiveEditTemplate { get; set; }
        public string ActivePageHeaderTemplate { get; set; }
        public List<string> VersionList { get; set; }
        public string LatestVersionFolder { get; set; }
        public string ImageFolderMapPath { get; set; }
        public string DocumentFolderMapPath { get; set; }
        public string CssFolderMapPath { get; set; }
        public string TempFolderMapPath { get; set; }
        public string JsFolderMapPath { get; set; }
        public string ResxFolderMapPath { get; set; }
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
