﻿using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class AppThemeBase
    {

        public AppThemeBase()
        {
        }

        public AppThemeBase(string appThemeFolderRel, string versionFolder = "")
        {
            AppThemeFolderRel = appThemeFolderRel;
            FileNameList = new Dictionary<string, string>();
            PortalFileNameList = new Dictionary<string, string>();
            ImageFileNameList = new Dictionary<string, string>();
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);
            AppThemeFolder = Path.GetFileName(AppThemeFolderMapPath); // the format of the directory <systemkey>.<AppThemName>, make the directory look like a file.
            if (!Directory.Exists(AppThemeFolderMapPath)) Directory.CreateDirectory(AppThemeFolderMapPath);
            PopulateVersionList();
            if (versionFolder == "") versionFolder = LatestVersionFolder;
            AppVersionFolder = versionFolder;
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;
            AppVersion = Convert.ToDouble(AppVersionFolder, CultureInfo.GetCultureInfo("en-US"));
            LatestVersion = Convert.ToDouble(LatestVersionFolder, CultureInfo.GetCultureInfo("en-US"));
            PortalId = PortalUtils.GetPortalId();
            PortalFileDirectoryMapPath = PortalUtils.DNNrocketThemesDirectoryMapPath().TrimEnd('\\') + "\\" + AppThemeFolder + "\\" + AppVersionFolder + "\\";
            AssignVersionFolders();

            Exists = false;
            if (File.Exists(RazorFolderMapPath + "\\view.cshtml")) Exists = true;

            Populate();

        }

        private void AssignVersionFolders()
        {
            AppThemeVersionFolderRel = AppThemeFolderRel + "/" + AppVersionFolder;

            ImageFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/img";
            DocumentFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/doc";
            CssFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/css";
            TempFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/temp";
            JsFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/js";
            ResxFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/resx";
            RazorFolderRel = AppThemeFolderRel + "/" + AppVersionFolder + "/default";

            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolderRel);

            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);
            ImageFolderMapPath = DNNrocketUtils.MapPath(ImageFolderRel);
            DocumentFolderMapPath = DNNrocketUtils.MapPath(DocumentFolderRel);
            CssFolderMapPath = DNNrocketUtils.MapPath(CssFolderRel);
            TempFolderMapPath = DNNrocketUtils.MapPath(TempFolderRel);
            JsFolderMapPath = DNNrocketUtils.MapPath(JsFolderRel);
            ResxFolderMapPath = DNNrocketUtils.MapPath(ResxFolderRel);
            RazorFolderMapPath = DNNrocketUtils.MapPath(RazorFolderRel);

            CreateNewAppTheme();

        }

        public void Populate()
        {
            // sync filesystem
            SyncFiles();
        }

        private void SyncFiles()
        {
            // sync filesystem
            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\default")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\default");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\default", "*.*", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath).ToLower();
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\css")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\css");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\css", "*.css", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath).ToLower();
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\js")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\js");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\js", "*.js", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath).ToLower();
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\resx")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\resx");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\resx", "*.resx", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath).ToLower();
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(ImageFolderMapPath)) Directory.CreateDirectory(ImageFolderMapPath);
            foreach (string newPath in Directory.GetFiles(ImageFolderMapPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath).ToLower();
                if (ImageFileNameList.ContainsKey(fname)) ImageFileNameList.Remove(fname);
                ImageFileNameList.Add(fname, newPath);
            }

            // portal level files.
            if (Directory.Exists(PortalFileDirectoryMapPath + "\\default"))
            {
                foreach (string newPath in Directory.GetFiles(PortalFileDirectoryMapPath + "\\default", "*.*", SearchOption.TopDirectoryOnly))
                {
                    var fname = Path.GetFileName(newPath).ToLower();
                    if (PortalFileNameList.ContainsKey(fname)) PortalFileNameList.Remove(fname);
                    PortalFileNameList.Add(fname, newPath);
                }
            }
            if (Directory.Exists(PortalFileDirectoryMapPath + "\\css"))
            {
                foreach (string newPath in Directory.GetFiles(PortalFileDirectoryMapPath + "\\css", "*.css", SearchOption.TopDirectoryOnly))
                {
                    var fname = Path.GetFileName(newPath).ToLower();
                    if (PortalFileNameList.ContainsKey(fname)) PortalFileNameList.Remove(fname);
                    PortalFileNameList.Add(fname, newPath);
                }
            }
            if (Directory.Exists(PortalFileDirectoryMapPath + "\\js"))
            {
                foreach (string newPath in Directory.GetFiles(PortalFileDirectoryMapPath + "\\js", "*.js", SearchOption.TopDirectoryOnly))
                {
                    var fname = Path.GetFileName(newPath).ToLower();
                    if (PortalFileNameList.ContainsKey(fname)) PortalFileNameList.Remove(fname);
                    PortalFileNameList.Add(fname, newPath);
                }
            }
            if (Directory.Exists(PortalFileDirectoryMapPath + "\\resx"))
            {
                foreach (string newPath in Directory.GetFiles(PortalFileDirectoryMapPath + "\\resx", "*.resx", SearchOption.TopDirectoryOnly))
                {
                    var fname = Path.GetFileName(newPath).ToLower();
                    if (PortalFileNameList.ContainsKey(fname)) PortalFileNameList.Remove(fname);
                    PortalFileNameList.Add(fname, newPath);
                }
            }

        }
        private void CreateVersionFolders(double dblVersionFolder)
        {
            var versionFolder = dblVersionFolder.ToString("0.0", CultureInfo.GetCultureInfo("en-US"));

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

        public string GetFileMapPath(string fileName)
        {
            if (FileNameList.ContainsKey(fileName.ToLower()))
            {
                return FileNameList[fileName.ToLower()];
            }
            return "";
        }
        public string GetTemplate(string templateFileName, string moduleref = "")
        {
            if (FileNameList.ContainsKey(templateFileName.ToLower()))
            {
                var fileMapPath = FileNameList[templateFileName.ToLower()];
                if (PortalUtils.GetPortalId() != 0)
                {
                    var fileMP = "";
                    if (moduleref != "")
                        fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                    else
                        fileMP = GetPortalFileMapPath(fileMapPath);
                    if (File.Exists(fileMP)) fileMapPath = fileMP;
                }
                return FileUtils.ReadFile(fileMapPath);
            }
            return "";
        }
        public void SaveEditor(string filename, string editorcode, string moduleref = "")
        {
            if (FileNameList.ContainsKey(filename))
            {
                var fileMapPath = FileNameList[filename];
                var formHtml = GeneralUtils.DeCode(editorcode);
                if (PortalUtils.GetPortalId() != 0)
                {
                    var fileMP = "";
                    if (moduleref != "")
                        fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                    else
                        fileMP = GetPortalFileMapPath(fileMapPath);
                    if (fileMP != "") fileMapPath = fileMP;
                }
                new FileInfo(fileMapPath).Directory.Create();
                FileUtils.SaveFile(fileMapPath, formHtml);
            }
        }
        public void DeleteFile(string filename, string moduleref)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var fileMapPath = FileNameList[filename.ToLower()];
            if (PortalUtils.GetPortalId() != 0)
            {
                var fileMP = "";
                if (moduleref != "")
                    fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                else
                    fileMP = GetPortalFileMapPath(fileMapPath);
                if (File.Exists(fileMP)) File.Delete(fileMP);
            }
            else
            {
                File.Delete(fileMapPath);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        public bool IsModuleLevel(string fileName, string moduleref)
        {
            if (String.IsNullOrEmpty(moduleref)) return false;
            if (PortalFileNameList.ContainsKey(moduleref + "_" + fileName.ToLower())) return true;
            return false;
        }
        public bool IsPortalLevel(string fileName)
        {
            if (PortalFileNameList.ContainsKey(fileName.ToLower())) return true;
            return false;
        }
        private string GetModuleFileMapPath(string fileMapPath, string moduleref)
        {
            return Path.GetDirectoryName(fileMapPath).TrimEnd('\\') + "\\" + moduleref  + "_" + Path.GetFileName(fileMapPath);
        }
        private string GetPortalFileMapPath(string filename)
        {
            var fn = Path.GetFileName(filename);
            var ext = Path.GetExtension(filename);
            var subfolder = "default";
            if (ext == ".css") subfolder = "css";
            if (ext == ".js") subfolder = "js";
            if (ext == ".resx") subfolder = "resx";
            return PortalFileDirectoryMapPath + subfolder + "\\" + fn;
        }

        public void DeleteTheme()
        {
            if (Directory.Exists(AppThemeFolderMapPath))
            {
                foreach (var v in VersionList)
                {
                    var folderToDelete = AppThemeFolderMapPath + "\\" + v;
                    if (Directory.Exists(folderToDelete))
                    {
                        string currentDirectory = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(currentDirectory);
                        Directory.Delete(folderToDelete, true);
                    }
                }
                if (Directory.Exists(AppThemeFolderMapPath)) Directory.Delete(AppThemeFolderMapPath, true);
            }
        }
        public void DeleteVersion()
        {
            DeleteVersion(AppVersionFolder);
        }
        public void DeleteVersion(string versionFolder)
        {

            try
            {
                if (Directory.Exists(AppThemeVersionFolderMapPath))
                {
                    Directory.Delete(AppThemeVersionFolderMapPath, true);
                }

                PopulateVersionList();
                AppVersionFolder = LatestVersionFolder;
                AssignVersionFolders();
                Populate();
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }

        }

        public void CreateNewAppTheme()
        {
            // create new appTheme
            if (!Directory.Exists(AppThemeFolderMapPath))
            {
                Directory.CreateDirectory(AppThemeFolderMapPath);
                if (!Directory.Exists(AppThemeVersionFolderMapPath)) CreateVersionFolders(AppVersion);
            }
        }

        public string CopyVersion(double sourceVersionFolder, double destVersionFolder)
        {
            var strdestVersionFolder = destVersionFolder.ToString("0.0", CultureInfo.GetCultureInfo("en-US"));
            var strsourceVersionFolder = sourceVersionFolder.ToString("0.0", CultureInfo.GetCultureInfo("en-US"));

            var sourceVersionFolderMapPath = AppThemeFolderMapPath + "\\" + strsourceVersionFolder;
            var destVersionFolderMapPath = AppThemeFolderMapPath + "\\" + strdestVersionFolder;
            if (!Directory.Exists(destVersionFolderMapPath)) Directory.CreateDirectory(destVersionFolderMapPath);
            if (Directory.Exists(sourceVersionFolderMapPath) && !Directory.EnumerateFileSystemEntries(destVersionFolderMapPath).Any())
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourceVersionFolderMapPath, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceVersionFolderMapPath, destVersionFolderMapPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourceVersionFolderMapPath, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourceVersionFolderMapPath, destVersionFolderMapPath), true);

                // repopulate
                AssignVersionFolders();

                PopulateVersionList();
                Populate();
            }
            else
            {
                if (!Directory.Exists(sourceVersionFolderMapPath))
                {
                    return "<h1>ERROR: Invalid Source Directory</h1>";
                }
                if (Directory.EnumerateFileSystemEntries(destVersionFolderMapPath).Any())
                {
                    return "<h1>ERROR: Destination Directory not empty</h1>";
                }
            }
            return "";
        }

        public void PopulateVersionList()
        {
            LatestVersionFolder = "1.0";
            if (AppThemeFolder != null && AppThemeFolder != "")
            {
                VersionList = new List<string>();
                if (System.IO.Directory.Exists(AppThemeFolderMapPath))
                {
                    var dirlist = System.IO.Directory.GetDirectories(AppThemeFolderMapPath);
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        if (GeneralUtils.IsNumeric(dr.Name)) VersionList.Add(dr.Name); // only load version numbers.
                    }
                }
                if (VersionList.Count == 0) VersionList.Add("1.0");
                VersionList.Reverse();
                LatestVersionFolder = (string)VersionList.First();
            }
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;
        }

        public string ExportZipFile()
        {
            // Create zip
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\" + AppThemeFolder + ".zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            ZipFile.CreateFromDirectory(AppThemeFolderMapPath, exportZipMapPath);

            return exportZipMapPath;
        }

        public void Copy(string destFolderMapPath)
        {
            var sourceDirName = AppThemeFolderMapPath;
            // we need to make sure the filesystem is not doing anything before we continue.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destFolderMapPath))
            {
                Directory.CreateDirectory(destFolderMapPath);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            GeneralUtils.CopyAll(sourceDirName, destFolderMapPath);

            // we need to make sure the filesystem is not doing anything before we continue.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public Dictionary<string, string> GetTemplatesRazor()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in FileNameList)
            {
                if (t.Key.ToLower().EndsWith(".cshtml")) rtnDict.Add(t.Key, t.Value);
            }
            return rtnDict;
        }
        public Dictionary<string, string> GetTemplatesJS()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in FileNameList)
            {
                if (t.Key.ToLower().EndsWith(".js")) rtnDict.Add(t.Key, t.Value);
            }
            return rtnDict;
        }
        public Dictionary<string, string> GetTemplatesCSS()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in FileNameList)
            {
                if (t.Key.ToLower().EndsWith(".css")) rtnDict.Add(t.Key, t.Value);
            }
            return rtnDict;
        }

        public Dictionary<string, string> GetTemplatesResx()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in FileNameList)
            {
                if (t.Key.ToLower().EndsWith(".resx")) rtnDict.Add(t.Key, t.Value);
            }
            return rtnDict;
        }


        #region "properties"

        public string AppThemeFolder { get; set; }
        public string AppThemeFolderRel { get; set; }
        public string AppThemeFolderMapPath { get; set; }
        public string AppVersionFolder { get; set; }
        public double AppVersion { get; set; }
        public string AppThemeVersionFolderRel { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
        public List<string> VersionList { get; set; }
        public double LatestVersion { get; set; }
        public string LatestVersionFolder { get; set; }
        public string ImageFolderMapPath { get; set; }
        public string DocumentFolderMapPath { get; set; }
        public string CssFolderMapPath { get; set; }
        public string TempFolderMapPath { get; set; }
        public string JsFolderMapPath { get; set; }
        public string ResxFolderMapPath { get; set; }
        public string RazorFolderMapPath { get; set; }
        public string ImageFolderRel { get; set; }
        public string DocumentFolderRel { get; set; }
        public string CssFolderRel { get; set; }
        public string TempFolderRel { get; set; }
        public string JsFolderRel { get; set; }
        public string ResxFolderRel { get; set; }
        public string RazorFolderRel { get; set; }
        public string PortalFileDirectoryMapPath { get; set; }        
        public int PortalId { get; set; }        
        public bool Exists { get; set; }        
        public Dictionary<string, string> ImageFileNameList { get; set; }
        public Dictionary<string, string> FileNameList { get; set; }
        public Dictionary<string, string> PortalFileNameList { get; set; }
        #endregion



    }
}