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

        public AppTheme(string systemKey, string appThemeFolder, string langRequired = "", string versionFolder = "")
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";

            _systemKey = systemKey;

            PopulateVersionList();

            AppThemeFolder = appThemeFolder;

            AppVersionFolder = versionFolder;
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;

            AppSystemThemeFolderRel = AppProjectFolderRel + "/SystemThemes/" + _systemKey;
            AppSystemThemeFolderMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRel);

            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);

            AppCultureCode = langRequired;

            AppThemeVersionFolderRel = AppThemeFolderRel + "/" + AppVersionFolder;
            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolderRel);

            if (AppThemeFolder != "") Populate();
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
            Info = new SimplisityInfo();
            Load();
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
        public void Save(SimplisityInfo _postInfo)
        {
            if (AppThemeFolder != "")
            {
                var fileMapPath = AppThemeFolderMapPath + "\\meta.xml";
                File.WriteAllText(fileMapPath, _postInfo.ToXmlItem());
            }
        }

        public void Load()
        {
            var fileMapPath = AppThemeFolderMapPath + "\\" + AppThemeFolder + "\\meta.xml";
            if (File.Exists(fileMapPath))
            {
                var xmlImport = File.ReadAllText(fileMapPath);
                Info.FromXmlItem(xmlImport);
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
        public SimplisityInfo Info { get; set; }

        #endregion

    }

}
