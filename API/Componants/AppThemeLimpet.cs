using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace DNNrocketAPI.Componants
{

    public class AppThemeLimpet
    {
        private const string _entityTypeCode = "APPTHEME";
 
        public AppThemeLimpet(string systemKey, string zipMapPath, bool isImport)
        {
            SystemKey = systemKey;
            ImportXmlFile(zipMapPath);
        }
        public AppThemeLimpet(string systemKey, string appThemeFolder, string versionFolder = "")
        {
            InitAppTheme(systemKey, appThemeFolder, versionFolder);
        }

        private void InitAppTheme(string systemKey, string appThemeFolder, string versionFolder = "")
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            SystemKey = systemKey;
            var systemData = new SystemLimpet(systemKey);
            SystemId = systemData.SystemId;
            FileNameList = new Dictionary<string, string>();

            AppSummary = "";
            AppThemeFolder = appThemeFolder;

            var systemThemesDirRel = "/DesktopModules/RocketThemes";

            AppSystemThemeFolderRel = systemThemesDirRel + "/" + SystemKey;
            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);
            AppSystemThemeFolderMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRel);
            if (!Directory.Exists(AppSystemThemeFolderMapPath)) Directory.CreateDirectory(AppSystemThemeFolderMapPath);

            if (AppThemeFolder != "") // don't create anything if we dont; have a apptheme (first entry)
            {
                var sourceDirectory = systemData.SystemMapPath.TrimEnd('\\') + "\\AppThemeBase";
                if (Directory.Exists(sourceDirectory) && !Directory.Exists(AppThemeFolderMapPath))
                {
                    // copy the base AppThemeLimpet for this system.
                    GeneralUtils.CopyAll(sourceDirectory, AppThemeFolderMapPath);
                }

                PopulateVersionList();
                if (versionFolder == "") versionFolder = LatestVersionFolder;

                AppVersionFolder = versionFolder;
                if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;

                AppVersion = Convert.ToDouble(AppVersionFolder, CultureInfo.GetCultureInfo("en-US"));
                LatestVersion = Convert.ToDouble(LatestVersionFolder, CultureInfo.GetCultureInfo("en-US"));

                AssignVersionFolders();

                if (AppThemeFolder != "" && SystemKey != "") Populate();
            }

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

            AppThemeVersionFolderMapPath = DNNrocketUtils.MapPath(AppThemeVersionFolderRel);

            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);
            ImageFolderMapPath = DNNrocketUtils.MapPath(ImageFolderRel);
            DocumentFolderMapPath = DNNrocketUtils.MapPath(DocumentFolderRel);
            CssFolderMapPath = DNNrocketUtils.MapPath(CssFolderRel);
            TempFolderMapPath = DNNrocketUtils.MapPath(TempFolderRel);
            JsFolderMapPath = DNNrocketUtils.MapPath(JsFolderRel);
            ResxFolderMapPath = DNNrocketUtils.MapPath(ResxFolderRel);
            AppProjectFolderMapPath = DNNrocketUtils.MapPath(AppProjectFolderRel);

            CreateNewAppTheme();

        }

        public void Populate()
        {
            Record = new SimplisityInfo();
            Record = ReadAppTheme(AppThemeFolderMapPath);

            Record.SetXmlProperty("genxml/hidden/appthemefolder", AppThemeFolder);
            AppSummary = Record.GetXmlProperty("genxml/textbox/summary");

            // sync filesystem
            SyncFiles();

            // logo
            Logo = Record.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/*[1]");

            // add snippetText
            SnippetText = new Dictionary<string, SimplisityRecord>();
            var snippetInfo = new SimplisityRecord();
            var snippetXML = File.ReadAllText(AppProjectFolderMapPath + "\\Themes\\config-w3\\1.0\\EditorTokens\\snippets.resx");
            snippetInfo.XMLData = snippetXML;
            foreach (XmlNode snip in snippetInfo.XMLDoc.SelectNodes("root/data"))
            {
                var snipRecord = new SimplisityRecord();
                snipRecord.SetXmlProperty("genxml/name", snip.Attributes["name"].Value);
                snipRecord.SetXmlProperty("genxml/value", snip.SelectSingleNode("value").InnerText);
                if (snip.SelectSingleNode("comment") == null)
                {
                    snipRecord.SetXmlProperty("genxml/comment", "");
                }
                else
                {
                    snipRecord.SetXmlProperty("genxml/comment", snip.SelectSingleNode("comment").InnerText);
                }
                AddSnippetText(snip.Attributes["name"].Value, snipRecord);
            }
            // add RazorTokens
            RazorTokenText = new Dictionary<string, SimplisityRecord>();
            var razortokensInfo = new SimplisityRecord();
            var razortokensXML = File.ReadAllText(AppProjectFolderMapPath + "\\Themes\\config-w3\\1.0\\EditorTokens\\razortokens.resx");
            razortokensInfo.XMLData = razortokensXML;
            foreach (XmlNode snip in razortokensInfo.XMLDoc.SelectNodes("root/data"))
            {
                var snipRecord = new SimplisityRecord();
                snipRecord.SetXmlProperty("genxml/name", snip.Attributes["name"].Value);
                snipRecord.SetXmlProperty("genxml/value", snip.SelectSingleNode("value").InnerText);
                if (snip.SelectSingleNode("comment") == null)
                {
                    snipRecord.SetXmlProperty("genxml/comment", "");
                }
                else
                {
                    snipRecord.SetXmlProperty("genxml/comment", snip.SelectSingleNode("comment").InnerText);
                }
                AddRazorTokenText(snip.Attributes["name"].Value, snipRecord);
            }
        }

        private void SyncFiles()
        {
            // sync filesystem
            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\default")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\default");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\default", "*.cshtml", SearchOption.TopDirectoryOnly))
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

        public string GetTemplate(string templateFileName)
        {
            if (FileNameList.ContainsKey(templateFileName.ToLower()))
            {
                return FileUtils.ReadFile(FileNameList[templateFileName.ToLower()]);
            }
            return "";
        }

        public void DeleteTheme()
        {
            if (Directory.Exists(AppThemeFolderMapPath))
            {
                Save(new SimplisityInfo());

                foreach (var v in VersionList)
                {
                    var folderToDelete = AppThemeFolderMapPath + "\\" + v;
                    if (Directory.Exists(folderToDelete))
                    {
                        string currentDirectory = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(currentDirectory);
                        Directory.Delete(folderToDelete,true);

                        // remove index xml
                        var privateonlineIndex = new OnlineAppThemeIndex(SystemKey, "private");
                        privateonlineIndex.DeleteIndex(AppThemeFolder);
                        var publiconlineIndex = new OnlineAppThemeIndex(SystemKey, "public");
                        publiconlineIndex.DeleteIndex(AppThemeFolder);

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
        public void Save(SimplisityInfo postInfo)
        {

            // ensure we have validate field names.
            var lp = 1;
            foreach (var f in postInfo.GetList("fielddata"))
            {
                var newfieldname = f.GetXmlProperty("genxml/textbox/name");
                if (GeneralUtils.IsNumeric(newfieldname) || newfieldname == "")
                {
                    newfieldname = "field" + newfieldname + lp;
                    postInfo.SetXmlProperty("genxml/fielddata/genxml[" + lp + "]/textbox/name", newfieldname);
                }
                lp += 1;
            }
            lp = 1;
            foreach (var f in postInfo.GetList("settingfielddata"))
            {
                var newfieldname = f.GetXmlProperty("genxml/textbox/name");
                if (GeneralUtils.IsNumeric(newfieldname) || newfieldname == "")
                {
                    newfieldname = "field" + newfieldname + lp;
                    postInfo.SetXmlProperty("genxml/settingfielddata/genxml[" + lp + "]/textbox/name", newfieldname);
                }
                lp += 1;
            }


            Record = DNNrocketUtils.UpdateFieldXpath(postInfo, Record, "fielddata");
            Record = DNNrocketUtils.UpdateFieldXpath(postInfo, Record, "settingfielddata");


            var appthemeprefix = "";
            var newAppThemeName = AppThemeFolder;
            var s = AppThemeFolder.Split('_');
            if (s.Length > 1)
            {
                appthemeprefix = s[0];
                newAppThemeName = s[1];
            }
            AppThemePrefix = appthemeprefix;
            AppThemeName = newAppThemeName;

            ReplaceInfoFields(postInfo, "genxml/textbox/*");
            ReplaceInfoFields(postInfo, "genxml/select/*");
            ReplaceInfoFields(postInfo, "genxml/radio/*");
            ReplaceInfoFields(postInfo, "genxml/checkbox/*");

            Update();
        }
        private void ReplaceInfoFields(SimplisityInfo postInfo, string xpathListSelect)
        {
            var textList = Record.XMLDoc.SelectNodes(xpathListSelect);
            if (textList != null)
            {
                foreach (XmlNode nod in textList)
                {
                    Record.RemoveXmlNode(xpathListSelect.Replace("*", "") + nod.Name);
                }
            }
            textList = postInfo.XMLDoc.SelectNodes(xpathListSelect);
            if (textList != null)
            {
                foreach (XmlNode nod in textList)
                {
                    Record.SetXmlProperty(xpathListSelect.Replace("*", "") + nod.Name, nod.InnerText);
                }
            }
        }


        public void SaveEditor(string filename, string editorcode)
        {
            if (FileNameList.ContainsKey(filename))
            {
                var fileMapPath = FileNameList[filename];

                var formHtml = GeneralUtils.DeCode(editorcode);
                FileUtils.SaveFile(fileMapPath, formHtml);
            }

        }

        public Dictionary<string, string> GetFieldDictionaryFields(bool withExt = true)
        {
            return GetFieldDictionary("fielddata", Record, withExt);
        }
        public Dictionary<string, string> GetFieldDictionaryFields(SimplisityRecord record, bool withExt = true)
        {
            return GetFieldDictionary("fielddata", record, withExt);
        }
        public Dictionary<string, string> GetFieldDictionarySettings(bool withExt = true)
        {
            return GetFieldDictionary("settingfielddata", Record, withExt);
        }
        public Dictionary<string, string> GetFieldDictionarySettings(SimplisityRecord record, bool withExt = true)
        {
            return GetFieldDictionary("settingfielddata", record, withExt);
        }
        public Dictionary<string, string> GetFieldDictionaryAll(bool withExt = true)
        {
            var rtnlist = GetFieldDictionaryFields(withExt);
            var sl = GetFieldDictionarySettings(withExt);
            foreach (var s in sl)
            {
                rtnlist.Add(s.Key, s.Value);
            }
            return rtnlist;
        }
        public Dictionary<string, string> GetFieldDictionaryAll(SimplisityRecord record, bool withExt = true)
        {
            var rtnlist = GetFieldDictionaryFields(record, withExt);
            var sl = GetFieldDictionarySettings(record, withExt);
            foreach (var s in sl)
            {
                var flp = 1;
                var k = s.Key;
                while (rtnlist.ContainsKey(k))
                {
                    k += flp;
                    flp += 1;
                }
                rtnlist.Add(k, s.Value);
            }
            return rtnlist;
        }
        private Dictionary<string,string> GetFieldDictionary(string listname, SimplisityRecord record, bool withExt = true)
        {
            // get fields
            var fieldNames = new Dictionary<string, string>();
            foreach (var f in record.GetRecordList(listname))
            {
                var fieldname = f.GetXmlProperty("genxml/textbox/name");
                if (fieldname != "")
                {
                    var newfieldname = fieldname.Split('_')[0];                   
                    var labelvalue = f.GetXmlProperty("genxml/textbox/label");
                    if (withExt)
                    {
                        fieldNames.Add(newfieldname + ".Text", labelvalue);
                    }
                    else
                    {
                        fieldNames.Add(newfieldname, labelvalue);
                    }

                    // Add dictionary values for dropdownlist and radiobuttons.
                    var keylist = f.GetXmlProperty("genxml/hidden/dictionarykey");
                    var valuelist = f.GetXmlProperty("genxml/hidden/dictionaryvalue");
                    if (keylist != "" && valuelist != "")
                    {
                        var keyarray = keylist.Split(',');
                        var valuearray = valuelist.Split(',');
                        if (keyarray.Length == valuearray.Length)
                        {
                            var lp = 0;
                            foreach (var k in keyarray)
                            {
                                var resxfieldname = newfieldname + "-" + GeneralUtils.DeCode(k);
                                if (resxfieldname != "")
                                {
                                    if (withExt)
                                    {
                                        fieldNames.Add(resxfieldname + ".Text", GeneralUtils.DeCode(valuearray[lp]));
                                    }
                                    else
                                    {
                                        fieldNames.Add(resxfieldname, GeneralUtils.DeCode(valuearray[lp]));
                                    }
                                }
                                lp += 1;
                            }

                        }
                    }
                }
            }
            return fieldNames;
        }


        public void Update()
        {
            if (Record != null) UpdateAppTheme();
        }
        public void AddListImage()
        {
            if (Record != null) Record.AddListItem("imagelist");
            Update();
        }
        public void UpdateListFileName(string filename, string mapPath)
        {
            if (FileNameList.ContainsKey(filename)) FileNameList.Remove(filename);
            FileNameList.Add(filename, mapPath);
        }
        public void AddListField()
        {
            var listname = "fielddata";
            Record.AddListItem(listname);
            Update();
        }
        public void AddSettingListField()
        {
            var listname = "settingfielddata";
            Record.AddListItem(listname);
            Update();
        }

        public void AddSnippetText(string key, SimplisityRecord value)
        {
            SnippetText.Remove(key);
            SnippetText.Add(key, value);
        }
        public string GetSnippetText(string key, Dictionary<string,string> replaceData)
        {
            if (SnippetText.ContainsKey(key))
            {
                var rtntext = SnippetText[key].GetXmlProperty("genxml/value");
                foreach (var t in replaceData)
                {
                    rtntext = rtntext.Replace(t.Key, t.Value);
                }
                return rtntext;
            }
            return "";
        }

        public void AddRazorTokenText(string key, SimplisityRecord value)
        {
            RazorTokenText.Remove(key);
            RazorTokenText.Add(key, value);
        }
        public string GetRazorTokenText(string key, Dictionary<string, string> replaceData)
        {
            if (RazorTokenText.ContainsKey(key))
            {
                var rtntext = RazorTokenText[key].GetXmlProperty("genxml/value");
                foreach (var t in replaceData)
                {
                    rtntext = rtntext.Replace(t.Key, t.Value);
                }
                return rtntext;
            }
            return "";
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

                // copy DB record
                Record.SetXmlProperty("genxml/select/versionfolder", strdestVersionFolder);

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
            Update();

            // Create zip
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\" + AppThemeFolder + ".zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            ZipFile.CreateFromDirectory(AppThemeFolderMapPath, exportZipMapPath);

            return exportZipMapPath;
        }

        private void ImportXmlFile(string zipMapPath)
        {
            var tempDir = PortalUtils.TempDirectoryMapPath();
            var tempZipFolder = tempDir + "\\TempImport";
            if (Directory.Exists(tempZipFolder))
            {
                Directory.Delete(tempZipFolder, true);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            ZipFile.ExtractToDirectory(zipMapPath, tempZipFolder);

            var xmlImport = FileUtils.ReadFile(tempZipFolder + "\\export.xml");

            Record.FromXmlItem(xmlImport);

            if (AppThemeFolder != "")
            {

                // import DB records.
                var systemKey = SystemKey;
                var lastversion = 1.0;

                var appSystemThemeFolderRel = "/DesktopModules/RocketThemes/" + systemKey;
                var appSystemThemeFolderMapPath = DNNrocketUtils.MapPath(appSystemThemeFolderRel);

                var destinationFolder = appSystemThemeFolderMapPath + "\\" + AppThemeFolder;
                if (Directory.Exists(destinationFolder))
                {
                    Directory.Delete(destinationFolder, true);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                DirectoryCopy(tempZipFolder, destinationFolder);
                if (Directory.Exists(tempZipFolder))
                {
                    Directory.Delete(tempZipFolder, true);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                InitAppTheme(systemKey, AppThemeFolder, lastversion.ToString("F1", CultureInfo.InvariantCulture));
            }
        }

        public void Copy(string appThemeName)
        {
            // copy folder
            var newAppThemeFolder = AppSystemThemeFolderMapPath + "\\" + appThemeName;
            DirectoryCopy(AppThemeFolderMapPath, newAppThemeFolder);

            Record = ReadAppTheme(newAppThemeFolder);

            // change any AppTheme name in the Record.
            var oldAppThemeName = AppThemeFolder;
            var xmlItem = Record.ToXmlItem();
            xmlItem = xmlItem.Replace(oldAppThemeName, appThemeName);
            Record.FromXmlItem(xmlItem);
            AppThemeName = appThemeName;

            // rename
            var s = appThemeName.Split('_');
            if (s.Length > 1)
            {
                Record.SetXmlProperty("genxml/hidden/appthemeprefix", s[0]);
                Record.SetXmlProperty("genxml/hidden/appthemename", s[1]);
            }

            foreach (var appVersionFolder in VersionList)
            {
                var newAppThemeVersionFolderRel = AppSystemThemeFolderRel + "/" + appThemeName + "/" + appVersionFolder;
                var newAppThemeVersionFolderMapPath = AppSystemThemeFolderMapPath + "\\" + appThemeName + "\\" + appVersionFolder;
                var oldAppThemeVersionFolderMapPath = AppSystemThemeFolderMapPath + "\\" + AppThemeFolder + "\\" + appVersionFolder;

                // rename rex files
                var rlist = Record.GetRecordList("resxlist");
                var lp = 1;
                foreach (var ri1 in rlist)
                {
                    var oldname = ri1.GetXmlProperty("genxml/hidden/fullfilename");
                    var culturecode = ri1.GetXmlProperty("genxml/hidden/culturecode");
                    var newName = appThemeName + "." + culturecode + ".resx";
                    if (culturecode == "") newName = appThemeName + ".resx";
                    Record.SetXmlProperty("genxml/resxlist/genxml[" + lp + "]/hidden/fullfilename", newName);
                    if (!File.Exists(newAppThemeVersionFolderMapPath + "\\resx\\" + newName))
                    {
                        if (File.Exists(oldAppThemeVersionFolderMapPath + "\\resx\\" + oldname))
                        {
                            File.Move(oldAppThemeVersionFolderMapPath + "\\resx\\" + oldname, newAppThemeVersionFolderMapPath + "\\resx\\" + newName);
                        }
                    }
                    lp += 1;
                }
                // convert resx in templates to new filename
                rlist = Record.GetRecordList("templatelist");
                lp = 1;
                foreach (var ri2 in rlist)
                {
                    var editorcode = ri2.GetXmlProperty("genxml/hidden/editorcodehtmlmixed");
                    var editorText = GeneralUtils.DeCode(editorcode);
                    editorText.Replace("(\"" + AppThemeFolder + ".", "(\"" + appThemeName + ".");
                    Record.SetXmlProperty("genxml/templatelist/genxml[" + lp + "]/hidden/editorcodehtmlmixed", GeneralUtils.EnCode(editorText));
                    lp += 1;
                }

                // convert image paths
                rlist = Record.GetRecordList("imagelist");
                lp = 1;
                foreach (var ri3 in rlist)
                {
                    var imagepath = ri3.GetXmlProperty("genxml/hidden/imagepathimg");
                    var fname = Path.GetFileName(imagepath);
                    imagepath = newAppThemeVersionFolderRel + "/img/" + fname;
                    Record.SetXmlProperty("genxml/imagelist/genxml[" + lp + "]/hidden/imagepathimg", imagepath);
                    lp += 1;
                }
            }
            Update();
            Populate();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
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
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            GeneralUtils.CopyAll(sourceDirName, destDirName);

            // we need to make sure the filesystem is not doing anything before we continue.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public Dictionary<string,string> GetTemplatesRazor()
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

        private SimplisityRecord ReadAppTheme(string appThemeFolder)
        {
            var appThemeFile = appThemeFolder + "\\export.xml";
            var rtnRec = new SimplisityRecord();
            var appThemeXmlFile = rtnRec.ToXmlItem();
            if (File.Exists(appThemeFile))
            {
                appThemeXmlFile = FileUtils.ReadFile(appThemeFile);
                if (appThemeXmlFile.ToLower().StartsWith("<root>"))  // reformat for legacy code
                {
                    var sRec = new SimplisityRecord();
                    sRec.XMLData = appThemeXmlFile;
                    appThemeXmlFile = new SimplisityRecord().ToString();
                    var nod = sRec.XMLDoc.SelectSingleNode("root/item[1]");
                    if (nod != null)
                    {
                        appThemeXmlFile = nod.OuterXml;
                    }
                }
            }
            rtnRec.FromXmlItem(appThemeXmlFile);
            return rtnRec;
        }
        private void UpdateAppTheme()
        {
            var exportMapPath = AppThemeFolderMapPath + "\\export.xml";
            if (File.Exists(exportMapPath)) File.Delete(exportMapPath);
            FileUtils.SaveFile(exportMapPath, Record.ToXmlItem());
        }


        #region "properties"

        public string AppProjectFolderRel { get; set; }
        public string AppProjectFolderMapPath { get; set; }
        public string AppSystemThemeFolderRel { get; set; }
        public string AppSystemThemeFolderMapPath { get; set; }
        public string AppSummary { get; private set; }
        public string Logo { get; private set; }
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
        public string ImageFolderRel { get; set; }
        public string DocumentFolderRel { get; set; }
        public string CssFolderRel { get; set; }
        public string TempFolderRel { get; set; }
        public string JsFolderRel { get; set; }
        public string ResxFolderRel { get; set; }
        public SimplisityRecord Record { get; set; }
        public Dictionary<string, string> FileNameList { get; set; }
        public Dictionary<string, string> GetTemplatesMapPath { get { return FileNameList; } }

        public bool IsSinglePage { get { if (DataType == 1) return true; else return false;  } }
        public bool IsListDetail { get { if (DataType == 2) return true; else return false; } }
        public int DataType { get { return Record.GetXmlPropertyInt("genxml/radio/themetype"); } }
        public bool EnableSettings { get { return Record.GetXmlPropertyBool("genxml/checkbox/enablesettings"); } }
        public int SystemId { get; set; }
        public string SystemKey { get; set; }
        public Dictionary<string, SimplisityRecord> SnippetText { get; set; }
        public Dictionary<string, SimplisityRecord> RazorTokenText { get; set; }

        public string AppThemeName { get { return Record.GetXmlProperty("genxml/hidden/appthemename"); } set { Record.SetXmlProperty("genxml/hidden/appthemename",value); } }
        public string AppThemePrefix { get { return Record.GetXmlProperty("genxml/hidden/appthemeprefix"); } set { Record.SetXmlProperty("genxml/hidden/appthemeprefix", value); } }

        public string DefaultTemplate
        {
            get
            {
                var template = Record.GetXmlProperty("genxml/textbox/defaulttemplate");
                if (template == "") template = "view.cshtml";
                return template;
            }
        }
        public string DefaultCommand { get { return Record.GetXmlProperty("genxml/textbox/defaultcmd"); } }

        #endregion

    }

}
