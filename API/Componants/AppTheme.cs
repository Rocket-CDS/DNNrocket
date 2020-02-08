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

    public class AppTheme
    {
        private string _guidKey;
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "APPTHEME";
        private readonly DNNrocketController _objCtrl;
 
        public AppTheme(string systemKey, string zipMapPath, bool isImport)
        {
            SystemKey = systemKey;
            _objCtrl = new DNNrocketController();
            ImportXmlFile(zipMapPath);
        }
        public AppTheme(string systemKey, string appThemeFolder, string versionFolder = "")
        {
            _objCtrl = new DNNrocketController();
            InitAppTheme(systemKey, appThemeFolder, versionFolder);
        }

        private void InitAppTheme(string systemKey, string appThemeFolder, string versionFolder = "")
        {
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            SystemKey = systemKey;
            var systemData = new SystemData(systemKey);
            SystemId = systemData.SystemId;
            FileNameList = new Dictionary<string, string>();

            AppSummary = "";
            AppThemeFolder = appThemeFolder;

            var systemThemesDirRel = "/DesktopModules/DNNrocket/SystemThemes";

            AppSystemThemeFolderRel = systemThemesDirRel + "/" + SystemKey;
            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);
            AppSystemThemeFolderMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRel);
            if (!Directory.Exists(AppSystemThemeFolderMapPath)) Directory.CreateDirectory(AppSystemThemeFolderMapPath);

            if (AppThemeFolder != "") // don't create anything if we dont; have a apptheme (first entry)
            {
                PopulateVersionList();
                if (versionFolder == "") versionFolder = LatestVersionFolder;

                AppVersionFolder = versionFolder;
                if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;

                AppVersion = Convert.ToDouble(AppVersionFolder, CultureInfo.GetCultureInfo("en-US"));
                LatestVersion = Convert.ToDouble(LatestVersionFolder, CultureInfo.GetCultureInfo("en-US"));


                _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;

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
            Record = _objCtrl.GetRecord(_guidKey, _entityTypeCode, -1, false, _tableName);
            Record.SetXmlProperty("genxml/hidden/appthemefolder", AppThemeFolder);
            AppSummary = Record.GetXmlProperty("genxml/textbox/summary");

            // sync filesystem
            SyncFiles();

            // logo
            Logo = Record.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/*[1]");

            TemplateDefaults();

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

        private void TemplateDefaults()
        {
            // Tempalte Defaults
            var tempMapPath = AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";

            if (!File.Exists(tempMapPath))
            {
                var formHtml = GenerateEditForm("fielddata", "edit.cshtml", 0);
                FileUtils.SaveFile(tempMapPath, formHtml);
            }

            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var listHtml = GenerateEditList(0);
                FileUtils.SaveFile(tempMapPath, listHtml);
            }

            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var viewHtml = GenerateView(0, "view.cshtml");
                FileUtils.SaveFile(tempMapPath, viewHtml);
            }

            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\detail.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var viewHtml = GenerateView(0, "detail.cshtml");
                FileUtils.SaveFile(tempMapPath, viewHtml);
            }

            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\pageheader.cshtml";
            if (!File.Exists(tempMapPath)) FileUtils.SaveFile(tempMapPath, "");

            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\settings.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var formHtml = GenerateEditForm("settingfielddata", "settings.cshtml", 0);
                FileUtils.SaveFile(tempMapPath, formHtml);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\css\\" + AppThemeFolder + ".css";
            if (!File.Exists(tempMapPath)) FileUtils.SaveFile(tempMapPath, "");

            tempMapPath = AppThemeVersionFolderMapPath + "\\js\\" + AppThemeFolder + ".js";
            if (!File.Exists(tempMapPath)) FileUtils.SaveFile(tempMapPath, "");

            tempMapPath = AppThemeVersionFolderMapPath + "\\resx\\" + AppThemeFolder + ".resx";
            if (!File.Exists(tempMapPath))
            {
                // we need to use the base resx file, so format is easy.
                var baserexFileMapPath = AppProjectFolderMapPath + "\\AppThemeBase\\resxtemplate.xml";
                var resxFileData = FileUtils.ReadFile(baserexFileMapPath);
                FileUtils.SaveFile(tempMapPath, resxFileData);
            }

        }

        private void SyncFiles()
        {
            // sync filesystem
            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\default")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\default");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\default", "*.cshtml", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath);
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\css")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\css");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\css", "*.css", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath);
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\js")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\js");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\js", "*.js", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath);
                if (FileNameList.ContainsKey(fname)) FileNameList.Remove(fname);
                FileNameList.Add(fname, newPath);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\resx")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\resx");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\resx", "*.resx", SearchOption.TopDirectoryOnly))
            {
                var fname = Path.GetFileName(newPath);
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
            if (FileNameList.ContainsKey(fileName))
            {
                return FileNameList[fileName];
            }
            return "";
        }

        public string GetTemplate(string templateFileName)
        {
            if (FileNameList.ContainsKey(templateFileName))
            {
                return FileUtils.ReadFile(FileNameList[templateFileName]);
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

                    _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + v;
                    var versionRecord = _objCtrl.GetRecordByGuidKey(Record.PortalId, -1, _entityTypeCode, _guidKey, "", _tableName);
                    _objCtrl.Delete(versionRecord.ItemID, _tableName); // cascade delete
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

                _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + versionFolder;

                var versionRecord = _objCtrl.GetRecordByGuidKey(Record.PortalId, -1, _entityTypeCode, _guidKey, "", _tableName);
                _objCtrl.Delete(versionRecord.ItemID, _tableName); // cascade delete

                PopulateVersionList();
                AppVersionFolder = LatestVersionFolder;
                _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;
                AssignVersionFolders();
                Populate();
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
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

            var dbInfo = _objCtrl.GetRecord(_entityTypeCode, Record.ItemID, -1, true, _tableName);
            if (dbInfo != null)
            {

                dbInfo.XMLData = postInfo.XMLData;
                dbInfo.SetXmlProperty("genxml/hidden/latestrev", (LatestRev + 1).ToString());
                dbInfo.RemoveXmlNode("genxml/lang");
                _objCtrl.SaveRecord(dbInfo, _tableName);

                Populate();

                Record = DNNrocketUtils.UpdateFieldXpath(Record, "fielddata");
                Record = DNNrocketUtils.UpdateFieldXpath(Record, "settingfielddata");
                _objCtrl.Update(Record, _tableName);

                // output generated template.
                var formHtml = "";
                if (RegenerateEdit) formHtml = GenerateEditForm("fielddata", "edit.cshtml", 0);
                var tempMapPath = AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";
                if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

                formHtml = "";
                if (RegenerateSettings) formHtml = GenerateEditForm("settingfielddata", "settings.cshtml", 0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\settings.cshtml";
                if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

                var listHtml = "";
                if (RegenerateEditList) listHtml = GenerateEditList(0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
                if (listHtml != "") FileUtils.SaveFile(tempMapPath, listHtml);

                var viewHtml = "";
                if (RegenerateView) viewHtml = GenerateView(0, "view.cshtml");
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
                if (viewHtml != "") FileUtils.SaveFile(tempMapPath, viewHtml);

                var detailHtml = "";
                if (RegenerateDetail) detailHtml = GenerateView(0, "detail.cshtml");
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\detail.cshtml";
                if (detailHtml != "") FileUtils.SaveFile(tempMapPath, detailHtml);

            }

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
            Update();


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
            if (Record != null) _objCtrl.SaveRecord(Record, _tableName);
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
                var versionCopyRecord = _objCtrl.GetRecordByGuidKey(Record.PortalId, -1, _entityTypeCode, _guidKey, "", _tableName);
                _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + strdestVersionFolder;
                versionCopyRecord.GUIDKey = _guidKey;
                versionCopyRecord.ItemID = -1;
                versionCopyRecord.SetXmlProperty("genxml/select/versionfolder", strdestVersionFolder);

                versionCopyRecord = _objCtrl.SaveRecord(versionCopyRecord, _tableName);

                var l = _objCtrl.GetList(Record.PortalId, -1, _entityTypeCode + "LANG", " and R1.ParentItemId = " + Record.ItemID + " ", "", "", 0, 0, 0, 0, _tableName);
                foreach (var i in l)
                {
                    i.ParentItemId = versionCopyRecord.ItemID;
                    i.ItemID = -1;
                    _objCtrl.Update(i, _tableName);
                }

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

        private string GenerateEditForm(string listname, string basefile, int row)
        {
            var systemData = new SystemData(SystemKey);

            List<SimplisityRecord> fieldList = Record.GetRecordList(listname);
            var resxItem = Record.GetRecordListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = "";
            if (resxItem != null)
            {
                var jsonresx = resxItem.GetXmlProperty("genxml/hidden/jsonresx");
                jsondata = GeneralUtils.DeCode(jsonresx);
            }
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, DNNrocketUtils.GetEditCulture());
            }

            var strFieldList = "";
            // calc rows
            var frows = new List<List<SimplisityRecord>>();
            var fline = new List<SimplisityRecord>();
            var col = 0;
            foreach (var f in fieldList)
            {
                var size = f.GetXmlPropertyInt("genxml/select/size");
                if (size == 0 || size > 12) size = 12;
                col += size;
                if (col > 12)
                {
                    frows.Add(fline);
                    fline = new List<SimplisityRecord>();
                    fline.Add(f);
                    col = size;
                }
                else
                {
                    fline.Add(f);
                }
            }
            frows.Add(fline);

            foreach (var flines in frows)
            {
                strFieldList += "<div class='w3-row'>" + Environment.NewLine;
                foreach (var f in flines)
                {
                    var localized = f.GetXmlProperty("genxml/checkbox/localized").ToLower();
                    var localizedbool = f.GetXmlPropertyBool("genxml/checkbox/localized");
                    var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                    var size = f.GetXmlProperty("genxml/select/size");
                    var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                    var labelname = f.GetXmlProperty("genxml/textbox/name");
                    var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                    var defaultBool = f.GetXmlProperty("genxml/textbox/defaultvalue").ToLower();
                    if (defaultBool == "") defaultBool = "false";
                    var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                    var fieldname = f.GetXmlProperty("genxml/textbox/name");
                    var resxKeyName = AppThemeFolder + "." + labelname.Replace(".Text", "");

                    strFieldList += "\t<div class='w3-col m" + size + " w3-padding'>" + Environment.NewLine;

                    if (labelname != "")
                    {
                        var resxLabelItem = jasonInfo.GetRecordListItem("resxlistvalues", "genxml/text/*[starts-with(name(), 'name')]", labelname + ".Text");
                        if (resxLabelItem != null)
                        {
                            strFieldList += "\t\t<label>@ResourceKey(\"" + resxKeyName + "\")</label>";
                        }
                        else
                        {
                            strFieldList += "\t\t<label>" + label + "</label>";
                        }
                    }
                    else
                    {
                        strFieldList += "\t\t<label>&nbsp;</label>";
                    }
                    if (!localizedbool)
                    {
                        strFieldList += Environment.NewLine;
                    }
                    else
                    {
                        strFieldList += "\t\t&nbsp;@EditFlag()" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                    {
                        strFieldList += "\t\t@TextBox(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textboxdate")
                    {
                        strFieldList += "\t\t@TextBoxDate(info,\"" + xpath + "\",\"" + attributes + " s-datatype='date' \",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textarea")
                    {
                        strFieldList += "\t\t@TextArea(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                    {
                        strFieldList += "\t\t@CheckBox(info,\"" + xpath + "\", \"\", \"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        strFieldList += "@{ var datatext = ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString();}";
                        strFieldList += "@if (datatext == \"\") ";
                        strFieldList += "{ ";
                        strFieldList += " datatext = \"" + datavalue + "\"; ";
                        strFieldList += "} ";
                        strFieldList += "\t\t@DropDownList(info, \"" + xpath + "\",\"" + datavalue + "\",datatext,\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        strFieldList += "@{ var datatext = ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString();}";
                        strFieldList += "@if (datatext == \"\") ";
                        strFieldList += "{ ";
                        strFieldList += " datatext = \"" + datavalue + "\"; ";
                        strFieldList += "} ";
                        strFieldList += "\t\t@RadioButtonList(info,\"" + xpath + "\",\"" + datavalue.Replace("\"", "\\\"") + "\",datatext,\"" + attributes + "\",\"" + defaultValue + "\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        strFieldList += "@{ var datatext = ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString();}";
                        strFieldList += "@if (datatext == \"\") ";
                        strFieldList += "{ ";
                        strFieldList += " datatext = \"" + datavalue + "\"; ";
                        strFieldList += "} ";
                        strFieldList += "\t\t@CheckBoxList(info,\"" + xpath + "\",\"" + datavalue + "\",datatext,\"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        var width = f.GetXmlPropertyInt("genxml/textbox/width");
                        var height = f.GetXmlPropertyInt("genxml/textbox/height");
                        if (width == 0 && height == 0)
                        {
                            width = 140;
                            height = 140;
                        }
                        strFieldList += "\t\t@ImageEdit(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull")
                    {
                        var width = f.GetXmlPropertyInt("genxml/hidden/width");
                        var height = f.GetXmlPropertyInt("genxml/hidden/height");
                        if (width == 0 && height == 0)
                        {
                            width = 140;
                            height = 140;
                        }
                        strFieldList += "\t\t@ImageEditFull(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "internalpage")
                    {
                        var allowEmpty = f.GetXmlPropertyBool("genxml/checkbox/allowempty");
                        strFieldList += "\t\t@TabSelectListOnTabId(info,\"" + xpath + "\",\"" + attributes + "\"," + allowEmpty + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "document")
                    {
                        var fieldid = f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t\t@DocumentEdit(info,\"" + fieldid + "\",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        strFieldList += "\t\t<div class='w3-col m12'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@CKEditor(info,\"" + xpath + "\",\"\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                        strFieldList += "\t\t<div class='w3-col' style='width:0px;height:300px;'></div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        strFieldList += "\t\t<div id='imagelistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"imgfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editimages.cshtml\", \"" + systemData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        strFieldList += "\t\t<div id='documentlistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"docfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editdocuments.cshtml\", \"" + systemData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strFieldList += "\t\t<div id='linklistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"linkfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editlinks.cshtml\", \"" + systemData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }

                    strFieldList += "\t</div>" + Environment.NewLine;
                }
                strFieldList += "</div>" + Environment.NewLine;
            }


            // merge to template            
            var strOut = FileUtils.ReadFile(systemData.SystemMapPath + "\\AppThemeBase\\" + basefile);
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\" + basefile);
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = ReplaceTemplateTokens(strOut, strFieldList, systemData);
                return strOut;
            }

        }

        private string ReplaceTemplateTokens(string templateText, string strFieldList, SystemData systemData)
        {
            templateText = templateText.Replace("[Token:AppThemeFields]", strFieldList);
            templateText = templateText.Replace("[Token: AppThemeFields]", strFieldList);
            templateText = templateText.Replace("[Token:List]", strFieldList);
            templateText = templateText.Replace("[Token: List]", strFieldList);
            templateText = templateText.Replace("[Token:SystemKey]", SystemKey);
            templateText = templateText.Replace("[Token: SystemKey]", SystemKey);
            templateText = templateText.Replace("[Token:DefaultInterface]", systemData.DefaultInterface);
            templateText = templateText.Replace("[Token: DefaultInterface]", systemData.DefaultInterface);
            templateText = templateText.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
            templateText = templateText.Replace("[Token: appthemeresx]", AppThemeVersionFolderRel + "/resx/");

            return templateText;
        }

        private string GenerateEditList(int row)
        {

            List<SimplisityRecord> fieldList = Record.GetRecordList("fielddata");

            var strFieldList = "";
            var sortedList = new List<SimplisityRecord>();
            for (int i = 1; i < 12; i++)
            {
                foreach (var f in fieldList)
                {
                    var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                    if (isonlist && f.GetXmlPropertyInt("genxml/select/listcol") == i)
                    {
                        sortedList.Add(f);
                    }
                }
            }

            foreach (var f in sortedList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                var fieldname = f.GetXmlProperty("genxml/textbox/name");

                if (isonlist)
                {
                    if (xpath != "")
                    {
                        strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                        strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;

                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                        {
                            strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                            strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                            strFieldList += "\t</div>" + Environment.NewLine;
                        }

                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                        {
                            strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                            strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist" + fieldname + "\", 0).GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                            strFieldList += "\t</div>" + Environment.NewLine;
                        }
                    }

                }
            }

            // merge to template
            var systemData = new SystemData(SystemKey);
            var strOut = FileUtils.ReadFile(systemData.SystemMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = ReplaceTemplateTokens(strOut, strFieldList, systemData);
                return strOut;
            }

        }

        private string GenerateView(int row, string templatefilename)
        {
            List<SimplisityRecord> fieldList = Record.GetRecordList("fielddata");

            var strFieldList = "";
            foreach (var f in fieldList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                var fieldname = f.GetXmlProperty("genxml/textbox/name");

                if (xpath != "")
                {
                    strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        strFieldList += "\t\t@HtmlOf(info.GetXmlProperty(\"" + xpath + "\"))" + Environment.NewLine;
                    }
                    else
                    {
                        strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                    }
                    strFieldList += "\t</div>" + Environment.NewLine;


                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        xpath = "genxml/hidden/" + fieldname.Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist" + fieldname + "\", 0).GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }
                }

            }

            // merge to template
            var systemData = new SystemData(SystemKey);
            var strOut = FileUtils.ReadFile(systemData.SystemMapPath + "\\AppThemeBase\\" + templatefilename);
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\" + templatefilename);
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = ReplaceTemplateTokens(strOut, strFieldList, systemData);
                return strOut;
            }

        }


        public string ExportXML()
        {
            // Export DB
            var exportData = "<root>";
            var l = _objCtrl.GetList(-1, -1, "APPTHEME", "and guidkey like 'appTheme*" + SystemKey + "*" + AppThemeFolder + "*%' ");
            foreach (SimplisityInfo i in l)
            {
                var r = new SimplisityRecord(i);
                var appVersionFolder = r.GetXmlProperty("genxml/select/versionfolder");
                r.ItemID = -1;
                r.SetXmlProperty("genxml/hidden/appthemefolder", AppThemeFolder);
                exportData += r.ToXmlItem();
            }
            exportData += "</root>";

            return exportData;
        }

        public string ExportZipFile()
        {
            // Export DB
            var exportData = ExportXML();

            var exportMapPath = AppThemeFolderMapPath + "\\export.xml";
            if (File.Exists(exportMapPath)) File.Delete(exportMapPath);
            FileUtils.SaveFile(exportMapPath, exportData);

            // Create zip
            var exportZipMapPath = DNNrocketUtils.TempDirectoryMapPath() + "\\" + AppThemeFolder + ".zip";
            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            ZipFile.CreateFromDirectory(AppThemeFolderMapPath, exportZipMapPath);

            return exportZipMapPath;
        }

        private void ImportXmlFile(string zipMapPath)
        {
            var tempDir = DNNrocketUtils.TempDirectoryMapPath();
            var tempZipFolder = tempDir + "\\TempImport";
            if (Directory.Exists(tempZipFolder))
            {
                Directory.Delete(tempZipFolder, true);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            ZipFile.ExtractToDirectory(zipMapPath, tempZipFolder);

            var xmlImport = FileUtils.ReadFile(tempZipFolder + "\\export.xml");

            var iRec = new SimplisityRecord();
            iRec.XMLData = xmlImport;

            var appThemeFolder = iRec.GetXmlProperty("root/item[1]/genxml/hidden/appthemefolder");

            if (appThemeFolder != "")
            {

                // import DB records.
                var systemKey = SystemKey;
                var lastversion = 1.0;
                var nodList = iRec.XMLDoc.SelectNodes("root/item");
                foreach (XmlNode nod in nodList)
                {
                    var s = new SimplisityInfo();
                    if (nod != null) s.FromXmlItem(nod.OuterXml);
                    if (s.GetXmlProperty("genxml/hidden/selectedsystemkey") != "") systemKey = s.GetXmlProperty("genxml/hidden/selectedsystemkey");
                    if (s.GetXmlPropertyDouble("genxml/select/versionfolder") > lastversion) lastversion = s.GetXmlPropertyDouble("genxml/select/versionfolder");
                    s.ItemID = -1;
                    var dbRec = _objCtrl.GetRecordByGuidKey(-1, -1, "APPTHEME", s.GUIDKey, "", _tableName);
                    if (dbRec != null) s.ItemID = dbRec.ItemID;
                    if (systemKey != "")  _objCtrl.Update(s, _tableName);
                }

                if (systemKey != "")
                {
                    var appSystemThemeFolderRel = "/DesktopModules/DNNrocket/SystemThemes/" + systemKey;
                    var appSystemThemeFolderMapPath = DNNrocketUtils.MapPath(appSystemThemeFolderRel);

                    var destinationFolder = appSystemThemeFolderMapPath + "\\" + appThemeFolder;
                    if (Directory.Exists(destinationFolder))
                    {
                        Directory.Delete(destinationFolder, true);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    DirectoryCopy(tempZipFolder, destinationFolder, true);
                    if (Directory.Exists(tempZipFolder))
                    {
                        Directory.Delete(tempZipFolder, true);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    InitAppTheme(systemKey, appThemeFolder, lastversion.ToString("F1", CultureInfo.InvariantCulture));
                }
            }
        }

        public void Copy(string appThemeName)
        {
            // copy folder
            var newAppThemeFolder = AppSystemThemeFolderMapPath + "\\" + appThemeName;
            DirectoryCopy(AppThemeFolderMapPath, newAppThemeFolder, true);
            // rename
            var l = _objCtrl.GetList(-1, -1, "APPTHEME", "and guidkey like 'appTheme*" + SystemKey + "*" + AppThemeFolder + "*%' ");
            foreach (SimplisityInfo i in l)
            {
                var iclone = (SimplisityInfo)i.Clone();
                var r = new SimplisityRecord(iclone);
                r.ItemID = -1; // create new record
                var appVersionFolder = r.GetXmlProperty("genxml/select/versionfolder");
                var newguidkey = "appTheme*" + SystemKey + "*" + appThemeName + "*" + appVersionFolder;
                r.GUIDKey = newguidkey;

                var s = appThemeName.Split('_');
                if (s.Length > 1)
                {
                    r.SetXmlProperty("genxml/hidden/appthemeprefix", s[0]);
                    r.SetXmlProperty("genxml/hidden/appthemename", s[1]);
                }

                var newAppThemeVersionFolderRel = AppSystemThemeFolderRel + "/" + appThemeName + "/" + appVersionFolder;
                var newAppThemeVersionFolderMapPath = AppSystemThemeFolderMapPath + "\\" + appThemeName + "\\" + appVersionFolder;
                var oldAppThemeVersionFolderMapPath = AppSystemThemeFolderMapPath + "\\" + AppThemeFolder + "\\" + appVersionFolder;

                // rename rex files
                var rlist = r.GetRecordList("resxlist");
                var lp = 1;
                foreach (var ri1 in rlist)
                {
                    var oldname = ri1.GetXmlProperty("genxml/hidden/fullfilename");
                    var culturecode = ri1.GetXmlProperty("genxml/hidden/culturecode");
                    var newName = appThemeName + "." + culturecode + ".resx";
                    if (culturecode == "") newName = appThemeName + ".resx";
                    r.SetXmlProperty("genxml/resxlist/genxml[" + lp + "]/hidden/fullfilename", newName);
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
                rlist = r.GetRecordList("templatelist");
                lp = 1;
                foreach (var ri2 in rlist)
                {
                    var editorcode = ri2.GetXmlProperty("genxml/hidden/editorcodehtmlmixed");
                    var editorText = GeneralUtils.DeCode(editorcode);
                    editorText.Replace("(\"" + AppThemeFolder + ".", "(\"" + appThemeName + ".");
                    r.SetXmlProperty("genxml/templatelist/genxml[" + lp + "]/hidden/editorcodehtmlmixed", GeneralUtils.EnCode(editorText));
                    lp += 1;
                }

                // convert image paths
                rlist = r.GetRecordList("imagelist");
                lp = 1;
                foreach (var ri3 in rlist)
                {
                    var imagepath = ri3.GetXmlProperty("genxml/hidden/imagepathimg");
                    var fname = Path.GetFileName(imagepath);
                    imagepath = newAppThemeVersionFolderRel + "/img/" + fname;
                    r.SetXmlProperty("genxml/imagelist/genxml[" + lp + "]/hidden/imagepathimg", imagepath);
                    lp += 1;
                }

                _objCtrl.Update(r);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

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
        public int LatestRev { get { return Record.GetXmlPropertyInt("genxml/hidden/latestrev"); } }
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
        public bool RegenerateEditList { get { return Record.GetXmlPropertyBool("genxml/checkbox/regenerateeditlist"); } set { Record.SetXmlProperty("genxml/checkbox/regenerateeditlist", value.ToString()); } }
        public bool RegenerateEdit { get { return Record.GetXmlPropertyBool("genxml/checkbox/regenerateedit"); } set { Record.SetXmlProperty("genxml/checkbox/regenerateedit", value.ToString()); } }
        public bool RegenerateSettings { get { return Record.GetXmlPropertyBool("genxml/checkbox/regeneratesettings"); } set { Record.SetXmlProperty("genxml/checkbox/regeneratesettings", value.ToString()); } }
        public bool RegenerateView { get { return Record.GetXmlPropertyBool("genxml/checkbox/regenerateview"); } set { Record.SetXmlProperty("genxml/checkbox/regenerateview", value.ToString()); } }
        public bool RegenerateDetail { get { return Record.GetXmlPropertyBool("genxml/checkbox/regeneratedetail"); } set { Record.SetXmlProperty("genxml/checkbox/regeneratedetail", value.ToString()); } }
        public Dictionary<string, string> FileNameList { get; set; }

        public int DataType { get { return Record.GetXmlPropertyInt("genxml/radio/themetype"); } }
        public bool EnableSettings { get { return Record.GetXmlPropertyBool("genxml/checkbox/enablesettings"); } }
        public int SystemId { get; set; }
        public string SystemKey { get; set; }
        public Dictionary<string, SimplisityRecord> SnippetText { get; set; }
        public Dictionary<string, SimplisityRecord> RazorTokenText { get; set; }
        public DNNrocketController RocketController { get { return _objCtrl; } }

        public string AppThemeName { get { return Record.GetXmlProperty("genxml/hidden/appthemename"); } set { Record.SetXmlProperty("genxml/hidden/appthemename",value); } }
        public string AppThemePrefix { get { return Record.GetXmlProperty("genxml/hidden/appthemeprefix"); } set { Record.SetXmlProperty("genxml/hidden/appthemeprefix", value); } }

        #endregion

    }

}
