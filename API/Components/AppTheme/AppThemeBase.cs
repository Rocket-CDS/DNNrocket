using Simplisity;
using Simplisity.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace DNNrocketAPI.Components
{
    public class AppThemeBase
    {

        public AppThemeBase()
        {
        }
        public AppThemeBase(int portalid, string appThemeFolderRel, string versionFolder = "", string projectName = "")
        {
            Init(portalid, appThemeFolderRel, versionFolder, projectName);
        }
        private void Init(int portalid, string appThemeFolderRel, string versionFolder = "", string projectName = "")
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
            if (portalid < 0)
                PortalId = PortalUtils.GetPortalId();
            else
                PortalId = portalid;
            AppThemeFolderPortalMapPath = PortalUtils.DNNrocketThemesDirectoryMapPath(portalid).TrimEnd('\\') + "\\" + projectName + "\\" + AppThemeFolder;
            PortalFileDirectoryMapPath = PortalUtils.DNNrocketThemesDirectoryMapPath(portalid).TrimEnd('\\') + "\\" + projectName + "\\" + AppThemeFolder + "\\" + AppVersionFolder + "\\";
            PortalFileDirectoryRel = "/" + PortalUtils.DNNrocketThemesDirectoryRel(portalid).TrimEnd('/') + "/" + projectName + "/" + AppThemeFolder + "/" + AppVersionFolder + "/";
            AssignVersionFolders();
            ImportConfig();

            LastUpdated = Directory.GetLastWriteTime(AppThemeFolderMapPath);

            Exists = false;
            if (File.Exists(RazorFolderMapPath + "\\view.cshtml")) Exists = true;

            Populate();

        }

        private void ImportConfig()
        {
            ViewXml = new List<SimplisityRecord>();
            SettingsXml = new List<SimplisityRecord>();
            var themeConfigMapPath = RazorFolderMapPath + "\\theme.rules";
            if (File.Exists(themeConfigMapPath))
            {
                var configXml = FileUtils.ReadFile(themeConfigMapPath);
                var sRec = new SimplisityRecord();
                sRec.XMLData = configXml;
                foreach (var v in sRec.GetRecordList("views"))
                {
                    ViewXml.Add(v);
                }
                foreach (var v in sRec.GetRecordList("settingsdata"))
                {
                    SettingsXml.Add(v);
                }
            }
        }

        private void AssignVersionFolders()
        {
            AppThemeVersionFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder;

            ImageFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/img";
            DocumentFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/doc";
            CssFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/css";
            TempFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/temp";
            JsFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/js";
            ResxFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/resx";
            RazorFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/default";
            DepFolderRel = AppThemeFolderRel.TrimEnd('/') + "/" + AppVersionFolder + "/dep";

            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolderRel);

            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);
            ImageFolderMapPath = DNNrocketUtils.MapPath(ImageFolderRel);
            DocumentFolderMapPath = DNNrocketUtils.MapPath(DocumentFolderRel);
            CssFolderMapPath = DNNrocketUtils.MapPath(CssFolderRel);
            TempFolderMapPath = DNNrocketUtils.MapPath(TempFolderRel);
            JsFolderMapPath = DNNrocketUtils.MapPath(JsFolderRel);
            ResxFolderMapPath = DNNrocketUtils.MapPath(ResxFolderRel);
            RazorFolderMapPath = DNNrocketUtils.MapPath(RazorFolderRel);
            DepFolderMapPath = DNNrocketUtils.MapPath(DepFolderRel);

            CreateNewAppTheme();

        }

        public void Populate()
        {
            // sync filesystem
            SyncFiles();
        }

        private void SyncSystemLevel(string folder, string filter = "")
        {
            if (filter == "") filter = "*." + folder;
            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\" + folder)) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\" + folder);
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\" + folder, filter, SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath).ToLower();
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }
        }
        private void SyncPortalLevel(string folder, string filter = "")
        {
            if (filter == "") filter = "*." + folder;
            if (Directory.Exists(PortalFileDirectoryMapPath + "\\" + folder))
            {
                foreach (string newPath in Directory.GetFiles(PortalFileDirectoryMapPath + "\\" + folder, filter, SearchOption.TopDirectoryOnly))
                {
                    var fname = Path.GetFileName(newPath).ToLower();
                    if (PortalFileNameList.ContainsKey(fname)) PortalFileNameList.Remove(fname);
                    PortalFileNameList.Add(fname, newPath);
                }
            }
        }
        private void SyncFiles()
        {
            if (AppThemeFolder != "")
            {
                // sync filesystem
                SyncSystemLevel("default", "*.*");
                SyncSystemLevel("css");
                SyncSystemLevel("js");
                SyncSystemLevel("resx");
                SyncSystemLevel("dep");

                if (!Directory.Exists(ImageFolderMapPath)) Directory.CreateDirectory(ImageFolderMapPath);
                foreach (string newPath in Directory.GetFiles(ImageFolderMapPath, "*.*", SearchOption.TopDirectoryOnly))
                {
                    var fname = Path.GetFileName(newPath).ToLower();
                    if (ImageFileNameList.ContainsKey(fname)) ImageFileNameList.Remove(fname);
                    ImageFileNameList.Add(fname, newPath);
                }

                // portal level files.
                SyncPortalLevel("default", "*.*");
                SyncPortalLevel("css");
                SyncPortalLevel("js");
                SyncPortalLevel("resx");
                SyncPortalLevel("dep");
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
                Directory.CreateDirectory(AppThemeFolderMapPath + "\\" + versionFolder + "\\dep");
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
                var fileMP = "";
                if (moduleref != "") fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                if (!File.Exists(fileMP)) fileMP = GetPortalFileMapPath(fileMapPath);
                if (File.Exists(fileMP)) fileMapPath = fileMP;
                return FileUtils.ReadFile(fileMapPath);
            }
            else
            {
                // we might only have the file at portallevel
                if (PortalFileNameList.ContainsKey(templateFileName.ToLower()))
                {
                    var fileMapPath = PortalFileNameList[templateFileName.ToLower()];
                    var fileMP = "";
                    if (moduleref != "") fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                    if (!File.Exists(fileMP)) fileMP = GetPortalFileMapPath(fileMapPath);
                    if (File.Exists(fileMP)) fileMapPath = fileMP;
                    return FileUtils.ReadFile(fileMapPath);
                }
                
            }
            return "";
        }
        public ResxData GetResx(string templateFileName, string moduleref = "")
        {
            if (FileNameList.ContainsKey(templateFileName.ToLower()))
            {
                var fileMapPath = FileNameList[templateFileName.ToLower()];
                var fileMP = "";
                if (moduleref != "") fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                if (!File.Exists(fileMP)) fileMP = GetPortalFileMapPath(fileMapPath);
                if (File.Exists(fileMP)) fileMapPath = fileMP;
                return new ResxData(fileMapPath);
            }
            else
            {
                // we might only have the file at portallevel
                if (PortalFileNameList.ContainsKey(templateFileName.ToLower()))
                {
                    var fileMapPath = PortalFileNameList[templateFileName.ToLower()];
                    var fileMP = "";
                    if (moduleref != "") fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                    if (!File.Exists(fileMP)) fileMP = GetPortalFileMapPath(fileMapPath);
                    if (File.Exists(fileMP)) fileMapPath = fileMP;
                    return new ResxData(fileMapPath);
                }
            }
            return null;
        }
        public SimplisityRecord GetSeachIndexFieldNames(string moduleref = "")
        {
            var depList = GetTemplatesDep();
            foreach (var depfile in depList)
            {
                var dep = GetDep(depfile.Key, moduleref);
                if (dep != null)
                {
                    var searchIdxList = dep.GetRecordList("searchindex");
                    if (searchIdxList != null && searchIdxList.Count > 0) return searchIdxList.First();
                }
            }
            return new SimplisityRecord();
        }
        public SimplisityRecord GetDep(string templateFileName, string moduleref = "")
        {
            var fileMapPath = "";
            var rtn = new SimplisityRecord();
            if (FileNameList.ContainsKey(templateFileName.ToLower()))
            {
                fileMapPath = FileNameList[templateFileName.ToLower()];
                var fileMP = "";
                if (moduleref != "") fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                if (!File.Exists(fileMP)) fileMP = GetPortalFileMapPath(fileMapPath);
                if (File.Exists(fileMP)) fileMapPath = fileMP;
            }
            else
            {
                // we might only have the file at portallevel
                if (PortalFileNameList.ContainsKey(templateFileName.ToLower()))
                {
                    fileMapPath = PortalFileNameList[templateFileName.ToLower()];
                    var fileMP = "";
                    if (moduleref != "") fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                    if (!File.Exists(fileMP)) fileMP = GetPortalFileMapPath(fileMapPath);
                    if (File.Exists(fileMP)) fileMapPath = fileMP;
                }
            }
            if (fileMapPath == "") return rtn;
            rtn.XMLData = FileUtils.ReadFile(fileMapPath);
            return rtn;
        }
        public SimplisityRecord GetModT(string templateFileName, string moduleref = "")
        {
            return GetDep(templateFileName, moduleref);
        }
        public void SaveResx(string filename, ResxData resxData, string moduleref = "")
        {
            var fileMapPath = "";
            filename = filename.ToLower();
            if (FileNameList.ContainsKey(filename))
                fileMapPath = FileNameList[filename];
            else
                if (PortalFileNameList.ContainsKey(filename)) fileMapPath = PortalFileNameList[filename];
            if (fileMapPath != "")
            {
                var fileMP = "";
                if (moduleref != "")
                    fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                else
                    fileMP = GetPortalFileMapPath(fileMapPath);
                if (fileMP != "") fileMapPath = fileMP;
                var dir = Path.GetDirectoryName(fileMapPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                FileUtils.SaveFile(fileMapPath, resxData.ResxXmlData.OuterXml);
            }
        }
        public void SaveDep(string filename, SimplisityRecord sRecord, string moduleref = "")
        {
            var fileMapPath = "";
            filename = filename.ToLower();
            if (FileNameList.ContainsKey(filename))
                fileMapPath = FileNameList[filename];
            else
                if (PortalFileNameList.ContainsKey(filename)) fileMapPath = PortalFileNameList[filename];
            if (fileMapPath != "")
            {
                var fileMP = "";
                if (moduleref != "")
                    fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                else
                    fileMP = GetPortalFileMapPath(fileMapPath);
                if (fileMP != "") fileMapPath = fileMP;
                var dir = Path.GetDirectoryName(fileMapPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var rtnRec = new SimplisityRecord();
                foreach (var r in sRecord.GetRecordList("deps"))
                {
                    rtnRec.AddRecordListItem("deps", r);
                }
                FileUtils.SaveFile(fileMapPath, rtnRec.XMLData);
            }
        }
        public void AddDep(string templateFileName, string moduleref = "")
        {
            var sRec = GetDep(templateFileName, moduleref);
            sRec.AddRecordListItem("deps", new SimplisityRecord());
            SaveDep(templateFileName, sRec, moduleref);
        }

        public void SaveEditor(string filename, string editorcode, string moduleref = "")
        {
            filename = filename.ToLower();
            if (FileNameList.ContainsKey(filename))
            {
                // NOTE: ONLY SUPERUSER CAN CHANGE RAZOR TEMPLATES.
                // Razor templates are executed on the host machine and can hack the database of the install, across ALL portals. 
                var allowedit = true;
                if (Path.GetExtension(filename).ToLower() == ".cshtml" && !UserUtils.IsSuperUser()) allowedit = false;
                if (allowedit)
                {
                    var fileMapPath = FileNameList[filename];
                    var formHtml = GeneralUtils.DeCode(editorcode);

                    var fileMP = "";
                    if (moduleref != "")
                        fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
                    else
                        fileMP = GetPortalFileMapPath(fileMapPath);
                    if (fileMP != "") fileMapPath = fileMP;

                    var dir = Path.GetDirectoryName(fileMapPath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    FileUtils.SaveFile(fileMapPath, formHtml);
                }
            }
        }
        /// <summary>
        /// Delete AppTheme File.  Note: never delete system level.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="moduleref"></param>
        public void DeleteFile(string filename, string moduleref)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var fileMapPath = FileNameList[filename.ToLower()];
            var fileMP = "";
            if (moduleref != "") 
                fileMP = GetModuleFileMapPath(fileMapPath, moduleref);
            else
                fileMP = GetPortalFileMapPath(fileMapPath);
                
            if (File.Exists(fileMP)) File.Delete(fileMP);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public bool HasModuleLevel(string moduleref)
        {
            if (String.IsNullOrEmpty(moduleref)) return false;
            foreach (var f in PortalFileNameList)
            {
                if (f.Key.ToLower().StartsWith(moduleref.ToLower())) return true;
            }
            return false;
        }
        public bool IsModuleLevel(string fileName, string moduleref)
        {
            if (String.IsNullOrEmpty(moduleref)) return false;
            if (PortalFileNameList.ContainsKey(moduleref.ToLower() + "_" + fileName.ToLower())) return true;
            return false;
        }
        public bool HasPortalLevel()
        {
            foreach (var f in PortalFileNameList)
            {
                if (!f.Key.Contains("_")) return true;
            }
            return false;
        }
        public bool IsPortalLevel(string fileName)
        {
            if (PortalFileNameList.ContainsKey(fileName.ToLower())) return true;
            return false;
        }
        private string GetModuleFileMapPath(string filename, string moduleref)
        {
            var fn = Path.GetFileName(filename);
            var ext = Path.GetExtension(filename);
            var subfolder = "default";
            if (ext == ".css") subfolder = "css";
            if (ext == ".js") subfolder = "js";
            if (ext == ".resx") subfolder = "resx";
            if (ext == ".dep") subfolder = "dep";
            return PortalFileDirectoryMapPath + subfolder + "\\" + moduleref + "_" + fn;
        }
        private string GetPortalFileMapPath(string filename)
        {
            var fn = Path.GetFileName(filename);
            var ext = Path.GetExtension(filename);
            var subfolder = "default";
            if (ext == ".css") subfolder = "css";
            if (ext == ".js") subfolder = "js";
            if (ext == ".resx") subfolder = "resx";
            if (ext == ".dep") subfolder = "dep";
            return PortalFileDirectoryMapPath + "\\"  + subfolder + "\\" + fn;
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
            VersionList = new List<string>();
            VersionListDict = new Dictionary<string, string>();
            LatestVersionFolder = "1.0";
            VersionList = new List<string>();
            VersionListDict = new Dictionary<string, string>();
            if (AppThemeFolder != null && AppThemeFolder != "")
            {
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
                foreach (var v in VersionList)
                {
                    VersionListDict.Add(v, v);
                }
                LatestVersionFolder = (string)VersionList.First();
            }
            if (AppVersionFolder == "" || AppVersionFolder == null) AppVersionFolder = LatestVersionFolder;
        }
        public object VersionListJson()
        {
            var jsonList = new List<ValuePair>();
            if (VersionList != null)
            {
                var valuePair = new ValuePair();
                foreach (var i in VersionList)
                {
                    valuePair = new ValuePair();
                    valuePair.Key = i;
                    valuePair.Value = i;
                    jsonList.Add(valuePair);
                }
            }
            return jsonList;
        }
        public string ExportZipFile(int portalId, string prefix = "")
        {
            // Create zip
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath(portalId) + "\\" + prefix + AppThemeFolder + ".zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            if (Directory.Exists(AppThemeFolderMapPath)) ZipFile.CreateFromDirectory(AppThemeFolderMapPath, exportZipMapPath);

            return exportZipMapPath;
        }
        public void ImportZipFile(int portalId, string zipFileMapPath)
        {
            if (File.Exists(zipFileMapPath)) // if exists, assume correct AppTheme from GitHub system project
            {
                string[] filesTest = System.IO.Directory.GetFiles(AppThemeVersionFolderMapPath + "\\default");
                if (filesTest.Length == 0)
                {
                    var importTemp = PortalUtils.TempDirectoryMapPath(portalId) + "\\" + Path.GetFileNameWithoutExtension(zipFileMapPath);
                    if (Directory.Exists(importTemp)) Directory.Delete(importTemp, true);
                    ZipFile.ExtractToDirectory(zipFileMapPath, importTemp);

                    // copy missing files 
                    string[] files = Directory.GetFiles(importTemp, "*.*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        FileInfo f = new FileInfo(file);
                        var fileMapPath = f.FullName.Replace(importTemp, AppThemeFolderMapPath);
                        if (!File.Exists(fileMapPath))
                        {
                            new FileInfo(fileMapPath).Directory.Create();
                            File.Move(f.FullName, fileMapPath);
                        }
                    }
                    Directory.Delete(importTemp, true);
                }
            }
        }
        public string ExportPortalZipFile(int portalId, string prefix = "")
        {
            // Create zip
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath(portalId) + "\\" + prefix + AppThemeFolder + "_portal.zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            if (Directory.Exists(AppThemeFolderPortalMapPath)) ZipFile.CreateFromDirectory(AppThemeFolderPortalMapPath, exportZipMapPath);

            return exportZipMapPath;
        }
        public void ImportPortalZipFile(int portalId, string zipFileMapPath, string oldModuleRef = "", string newModuleRef = "")
        {
            if (File.Exists(zipFileMapPath))
            {
                var importTemp = PortalUtils.TempDirectoryMapPath(portalId) + "\\" + Path.GetFileNameWithoutExtension(zipFileMapPath);
                if (Directory.Exists(importTemp)) Directory.Delete(importTemp, true);
                ZipFile.ExtractToDirectory(zipFileMapPath, importTemp);

                // copy missing files 
                string[] files = Directory.GetFiles(importTemp,"*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    FileInfo f = new FileInfo(file);
                    var fileMapPath = f.FullName.Replace(importTemp, AppThemeFolderPortalMapPath).Replace(oldModuleRef,newModuleRef);
                    if (!File.Exists(fileMapPath))
                    {
                        new FileInfo(fileMapPath).Directory.Create();
                        File.Move(f.FullName, fileMapPath);
                    }
                }
                Directory.Delete(importTemp, true);
            }
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
        public Dictionary<string, string> GetTemplateSelectName()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in GetTemplatesRazor())
            {
                if (!t.Key.ToLower().EndsWith("-settings.cshtml") && !t.Key.ToLower().EndsWith(".hbs"))
                {
                    if (!rtnDict.ContainsKey(Path.GetFileName(t.Value))) rtnDict.Add(Path.GetFileName(t.Value), Path.GetFileName(t.Value));
                }
            }
            return rtnDict;
        }

        public Dictionary<string, string> GetTemplatesRazor()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in FileNameList)
            {
                if (t.Key.ToLower().EndsWith(".cshtml") || t.Key.ToLower().EndsWith(".hbs")) rtnDict.Add(t.Key, t.Value);
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
        public Dictionary<string, string> GetTemplatesDep()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var t in FileNameList)
            {
                if (t.Key.ToLower().EndsWith(".dep")) rtnDict.Add(t.Key, t.Value);
            }
            return rtnDict;
        }
        [Obsolete("The .modt file data is now included in the .dep file.")]
        public Dictionary<string, string> GetModuleTemples()
        {
            return GetTemplatesDep();
        }

        public List<string> ModuleTemplateList(string moduleRef)
        {
            var rtn = new List<string>();
            if (Directory.Exists(PortalFileDirectoryMapPath))
            {
                foreach (var f in Directory.GetFiles(PortalFileDirectoryMapPath, "*.*", SearchOption.AllDirectories))
                {
                    if (f.Contains(moduleRef))
                    {
                        rtn.Add(Path.GetFileName(f));
                    }
                }
            }
            return rtn;
        }
        public List<string> PortalTemplateList()
        {
            var rtn = new List<string>();
            if (Directory.Exists(PortalFileDirectoryMapPath))
            {
                foreach (var f in Directory.GetFiles(PortalFileDirectoryMapPath, "*.*", SearchOption.AllDirectories))
                {
                    if (!f.Contains("ModuleID"))
                    {
                        rtn.Add(Path.GetFileName(f));
                    }
                }
            }
            return rtn;
        }

        #region "properties"

        public List<SimplisityRecord> SettingsXml { get; set; }
        public List<SimplisityRecord> ViewXml { get; set; }
        public string AppThemeFolder { get; set; }
        public string Name { get { var l = AppThemeFolder.Split('.'); if (l.Length >= 2) return l[1]; else return AppThemeFolder; } }
        public string AppThemeFolderRel { get; set; }
        public string AppThemeFolderMapPath { get; set; }
        public string AppVersionFolder { get; set; }
        public double AppVersion { get; set; }
        public string AppThemeVersionFolderRel { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
        public List<string> VersionList { get; set; }
        public Dictionary<string, string> VersionListDict { get; set; }
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
        public string DepFolderRel { get; set; }
        public string DepFolderMapPath { get; set; }
        public string PortalFileDirectoryMapPath { get; set; }
        public string AppThemeFolderPortalMapPath { get; set; }        
        public string PortalFileDirectoryRel { get; set; }
        public int PortalId { get; set; }        
        public bool Exists { get; set; }        
        public Dictionary<string, string> ImageFileNameList { get; set; }
        public Dictionary<string, string> FileNameList { get; set; }
        public Dictionary<string, string> PortalFileNameList { get; set; }
        public DateTime LastUpdated { get; set; }
        #endregion



    }
}
