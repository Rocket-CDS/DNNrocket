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
        private DNNrocketController _objCtrl;
        private List<string> _templateFileName;
        private List<string> _cssFileName;
        private List<string> _jsFileName;
        private List<string> _resxFileName;
        private bool _debugMode;          

        public AppTheme(string zipMapPath)
        {
            _objCtrl = new DNNrocketController();
            ImportXmlFile(zipMapPath);
        }
        public AppTheme(string systemKey, string appThemeFolder, string versionFolder = "", bool debugMode = false)
        {
            _objCtrl = new DNNrocketController();
            InitAppTheme(systemKey, appThemeFolder, versionFolder, debugMode);
        }

        private void InitAppTheme(string systemKey, string appThemeFolder, string versionFolder = "", bool debugMode = false)
        {
            _debugMode = debugMode;
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            SystemKey = systemKey;
            var systemInfodata = new SystemInfoData(systemKey);
            SystemId = systemInfodata.SystemId;
            _templateFileName = new List<string>();
            _cssFileName = new List<string>();
            _jsFileName = new List<string>();
            _resxFileName = new List<string>();



            AppSummary = "";
            AppThemeFolder = appThemeFolder;
            AppSystemThemeFolderRel = AppProjectFolderRel + "/SystemThemes/" + SystemKey;
            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;
            AppThemeFolderMapPath = DNNrocketUtils.MapPath(AppThemeFolderRel);

            PopulateVersionList();
            if (versionFolder == "") versionFolder = LatestVersionFolder;

            AppVersionFolder = versionFolder;
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder;

            _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;

            AssignVersionFolders();

            if (AppThemeFolder != "" && SystemKey != "") Populate();
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
            AppSystemThemeFolderMapPath = DNNrocketUtils.MapPath(AppSystemThemeFolderRel);

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

            // RESX Default
            var resxList = Record.GetRecordList("resxlist");
            if (resxList.Count == 0) // create default
            {
                AddListResx("");
                resxList = Record.GetRecordList("resxlist");
            }

            foreach (var r in resxList)
            {
                _resxFileName.Add(r.GetXmlProperty("genxml/hidden/fullfilename"));
            }

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
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var formHtml = GenerateEditForm("fielddata", "edit",0);
                FileUtils.SaveFile(tempMapPath, formHtml);
                _templateFileName.Add(Path.GetFileName(tempMapPath));
                AddListTemplate(Path.GetFileNameWithoutExtension(tempMapPath), formHtml);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var listHtml = GenerateEditList(0);
                FileUtils.SaveFile(tempMapPath, listHtml);
                _templateFileName.Add(Path.GetFileName(tempMapPath));
                AddListTemplate(Path.GetFileNameWithoutExtension(tempMapPath), listHtml);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var viewHtml = GenerateView(0);
                FileUtils.SaveFile(tempMapPath, viewHtml);
                _templateFileName.Add(Path.GetFileName(tempMapPath));
                AddListTemplate(Path.GetFileNameWithoutExtension(tempMapPath), viewHtml);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\pageheader.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                //var formHtml = AppThemeUtils.GeneraateEditForm(Record.GetList("fielddata"), 0);
                FileUtils.SaveFile(tempMapPath, "");
                _templateFileName.Add(Path.GetFileName(tempMapPath));
                AddListTemplate(Path.GetFileNameWithoutExtension(tempMapPath), "");
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\settings.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var formHtml = GenerateEditForm("settingfielddata", "settings",0);
                FileUtils.SaveFile(tempMapPath, formHtml);
                _templateFileName.Add(Path.GetFileName(tempMapPath));
                AddListTemplate(Path.GetFileNameWithoutExtension(tempMapPath), formHtml);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\css\\" + AppThemeFolder + ".css";
            if (!_cssFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                FileUtils.SaveFile(tempMapPath, "");
                _cssFileName.Add(Path.GetFileName(tempMapPath));
                AddListCss(Path.GetFileNameWithoutExtension(tempMapPath), "");
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\js\\" + AppThemeFolder + ".js";
            if (!_jsFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                FileUtils.SaveFile(tempMapPath, "");
                _jsFileName.Add(Path.GetFileName(tempMapPath));
                AddListJs(Path.GetFileNameWithoutExtension(tempMapPath), "");
            }
        }

        private void SyncFiles()
        {
            // sync filesystem
            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\default")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\default");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\default", "*.cshtml", SearchOption.TopDirectoryOnly))
            {
                var templateName = Path.GetFileName(newPath);
                var templateText = FileUtils.ReadFile(newPath);
                AddListTemplate(Path.GetFileNameWithoutExtension(templateName), templateText);
                _templateFileName.Add(templateName);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\css")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\css");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\css", "*.css", SearchOption.TopDirectoryOnly))
            {
                var templateName = Path.GetFileName(newPath);
                var templateText = FileUtils.ReadFile(newPath);
                AddListCss(Path.GetFileNameWithoutExtension(templateName), templateText);
                _cssFileName.Add(templateName);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\js")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\js");
            foreach (string newPath in Directory.GetFiles(AppThemeVersionFolderMapPath + "\\js", "*.js", SearchOption.TopDirectoryOnly))
            {
                var templateName = Path.GetFileName(newPath);
                var templateText = FileUtils.ReadFile(newPath);
                AddListJs(Path.GetFileNameWithoutExtension(templateName), templateText);
                _jsFileName.Add(templateName);
            }

            if (!Directory.Exists(AppThemeVersionFolderMapPath + "\\resx")) Directory.CreateDirectory(AppThemeVersionFolderMapPath + "\\resx");
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

        public Dictionary<string,string> GetTemplateDictionaryRazor()
        {
            if (Record == null) return null;
            var tList = Record.GetRecordList("templatelist");
            // get defaultDict
            var defaultDict = new Dictionary<string, string>();
            foreach (SimplisityRecord templateInfo in tList)
            {
                defaultDict.Add(templateInfo.GetXmlProperty("genxml/hidden/filename"),GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodehtmlmixed")));
            }
            return defaultDict;
        }
        public Dictionary<string, string> GetTemplateDictionaryCSS()
        {
            if (Record == null) return null;
            var tList = Record.GetRecordList("csslist");
            // get defaultDict
            var defaultDict = new Dictionary<string, string>();
            foreach (SimplisityRecord templateInfo in tList)
            {
                defaultDict.Add(templateInfo.GetXmlProperty("genxml/hidden/filename"), GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodecss")));
            }
            return defaultDict;
        }
        public Dictionary<string, string> GetTemplateDictionaryJS()
        {
            if (Record == null) return null;
            var tList = Record.GetRecordList("jslist");
            // get defaultDict
            var defaultDict = new Dictionary<string, string>();
            foreach (SimplisityRecord templateInfo in tList)
            {
                defaultDict.Add(templateInfo.GetXmlProperty("genxml/hidden/filename"), GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodejavascript")));
            }
            return defaultDict;
        }

        public string GetTemplate(string templateName)
        {
            if (Path.GetExtension(templateName) != "") templateName += Path.GetFileNameWithoutExtension(templateName);
            if (!_templateFileName.Contains(templateName + ".cshtml")) return "";
            var rtnItem = Record.GetRecordListItem("templatelist", "genxml/hidden/filename", templateName);
            if (rtnItem == null) return "";
            return GeneralUtils.DeCode(rtnItem.GetXmlProperty("genxml/hidden/editorcodehtmlmixed"));
        }

        public void DeleteTheme()
        {
            if (Directory.Exists(AppThemeFolderMapPath))
            {
                foreach (var v in VersionList)
                {
                    if (Directory.Exists(AppThemeFolderMapPath + "\\" + v)) Directory.Delete(AppThemeFolderMapPath + "\\" + v, true);
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

            postInfo = ActionListTemplateFiles(postInfo);
            postInfo = ActionListCssFiles(postInfo);
            postInfo = ActionListJsFiles(postInfo);
            postInfo = ActionListResxFiles(postInfo);

            var dbInfo = _objCtrl.GetRecord(_entityTypeCode, Record.ItemID, -1, true, _tableName);
            if (dbInfo != null)
            {

                dbInfo.XMLData = postInfo.XMLData;
                dbInfo.SetXmlProperty("genxml/hidden/latestrev", (LatestRev + 1).ToString());
                dbInfo.RemoveXmlNode("genxml/lang");
                _objCtrl.SaveRecord(dbInfo, _tableName);

                Populate();

                UpdateFieldXpath("fielddata");
                UpdateFieldXpath("settingfielddata");

                // output generated template.
                var formHtml = GetTemplate("edit");
                if (RegenerateEdit) formHtml = GenerateEditForm("fielddata", "edit",0);
                var tempMapPath = AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";
                if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

                formHtml = GetTemplate("settings");
                if (RegenerateSettings) formHtml = GenerateEditForm("settingfielddata", "settings", 0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\settings.cshtml";
                if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

                var listHtml = GetTemplate("editlist");
                if (RegenerateEditList) listHtml = GenerateEditList(0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
                if (listHtml != "") FileUtils.SaveFile(tempMapPath, listHtml);

                var viewHtml = GetTemplate("view");
                if (RegenerateView) viewHtml = GenerateView(0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
                if (viewHtml != "") FileUtils.SaveFile(tempMapPath, viewHtml);

                SyncFiles();


            }
        }

        public void SaveEditor(string listname, string filename, string editorcode)
        {
            var editortype = "htmlmixed";
            var fileext = ".cshtml";
            var folder = "default";
            if (listname == "csslist")
            {
                editortype = "css";
                fileext = ".css";
                folder = "css";
            }
            if (listname == "jslist")
            {
                editortype = "javascript";
                fileext = ".js";
                folder = "js";
            }

            var sr = new SimplisityRecord();
            sr.SetXmlProperty("genxml/hidden/filename", filename);
            sr.SetXmlProperty("genxml/hidden/editorcode" + editortype, editorcode);

            Record.RemoveRecordListItem(listname, "genxml/hidden/filename", filename);
            Record.AddRecordListItem(listname, sr);

            var formHtml = GeneralUtils.DeCode(editorcode);
            var tempMapPath = AppThemeVersionFolderMapPath + "\\" + folder + "\\" + filename + fileext;
            FileUtils.SaveFile(tempMapPath, formHtml);

        }


        private SimplisityInfo ActionListTemplateFiles(SimplisityInfo postInfo)
        {
            foreach (var t in _templateFileName)
            {
                var filename = Path.GetFileNameWithoutExtension(t);
                var delItem = postInfo.GetListItem("templatelist", "genxml/hidden/filename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\default\\" + t)) File.Delete(AppThemeVersionFolderMapPath + "\\default\\" + t);
            }
            //create any new files. (will be added to template list when populate syncs files)
            var tList = postInfo.GetList("templatelist"); // save list
            postInfo.RemoveList("templatelist"); // remove list
             
            // repopulate list with valid filenames
            var idx = 1;
            foreach (SimplisityInfo templateInfo in tList)
            {
                var postFileName = templateInfo.GetXmlProperty("genxml/hidden/filename");
                var fname = FileUtils.RemoveInvalidFileChars(postFileName).Replace(".", "");
                var filename = Path.GetFileNameWithoutExtension(fname);                
                templateInfo.SetXmlProperty("genxml/hidden/filename", filename);
                postInfo.AddListItem("templatelist", templateInfo);
                fname = filename + ".cshtml";
                FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\default\\" + fname, GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodehtmlmixed")));
                idx += 1;
            }
            return postInfo;
        }
        private SimplisityInfo ActionListCssFiles(SimplisityInfo postInfo)
        {
            foreach (var t in _cssFileName)
            {
                var filename = Path.GetFileNameWithoutExtension(t);
                var delItem = postInfo.GetListItem("csslist", "genxml/hidden/filename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\css\\" + t)) File.Delete(AppThemeVersionFolderMapPath + "\\css\\" + t);
            }

            //create any new files. (will be added to template list when populate syncs files)
            var tList = postInfo.GetList("csslist");
            postInfo.RemoveList("csslist"); // remove list

            // repopulate list with valid filenames
            var idx = 1;
            foreach (SimplisityInfo templateInfo in tList)
            {
                var postFileName = templateInfo.GetXmlProperty("genxml/hidden/filename");
                var fname = FileUtils.RemoveInvalidFileChars(postFileName).Replace(".", "");
                var filename = Path.GetFileNameWithoutExtension(fname);
                templateInfo.SetXmlProperty("genxml/hidden/filename", filename);
                postInfo.AddListItem("csslist", templateInfo);
                fname = filename + ".css";
                FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\css\\" + fname, GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodecss")));
                idx += 1;
            }
            return postInfo;
        }
        private SimplisityInfo ActionListJsFiles(SimplisityInfo postInfo)
        {
            foreach (var t in _jsFileName)
            {
                var filename = Path.GetFileNameWithoutExtension(t);
                var delItem = postInfo.GetListItem("jslist", "genxml/hidden/filename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\js\\" + t)) File.Delete(AppThemeVersionFolderMapPath + "\\js\\" + t);
            }

            //create any new files. (will be added to template list when populate syncs files)
            var tList = postInfo.GetList("jslist");
            postInfo.RemoveList("jslist"); // remove list

            // repopulate list with valid filenames
            var idx = 1;
            foreach (SimplisityInfo templateInfo in tList)
            {
                var postFileName = templateInfo.GetXmlProperty("genxml/hidden/filename");
                var fname = FileUtils.RemoveInvalidFileChars(postFileName).Replace(".", "");
                var filename = Path.GetFileNameWithoutExtension(fname);
                templateInfo.SetXmlProperty("genxml/hidden/filename", filename);
                postInfo.AddListItem("jslist", templateInfo);
                fname = filename + ".js";
                FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\js\\" + fname, GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodejavascript")));
                idx += 1;
            }
            return postInfo;
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
                    k = k + flp;
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

        private SimplisityInfo ActionListResxFiles(SimplisityInfo postInfo)
        {
            var fileList = Directory.GetFiles(AppThemeVersionFolderMapPath + "\\resx", "*.resx");
            foreach (var filenamepath in fileList)
            {
                if (File.Exists(filenamepath)) File.Delete(filenamepath);
            }

            // get fields
            var fieldNames = GetFieldDictionaryAll(new SimplisityRecord(postInfo));

            // get defaultDict
            var defaultDict = GetResxDictionary(new SimplisityRecord(postInfo));
            if (defaultDict.Count() == 0)
            {
                foreach (var f in fieldNames)
                {
                    defaultDict.Add(f.Key,f.Value);
                }
            }

            // write localized fieldname list, so we can disable default resx fiedls if they exist as a field.
            var fieldlocalizedlist = "";
            foreach (var f in fieldNames)
            {
                fieldlocalizedlist += f.Key + ",";
            }
            postInfo.SetXmlProperty("genxml/hidden/fieldlocalizedlist", fieldlocalizedlist);

            var tList = postInfo.GetList("resxlist");

            //Write files.
            var idx = 1;
            foreach (SimplisityInfo templateInfo in tList)
            {
                var fileFields = "";
                var fname = templateInfo.GetXmlProperty("genxml/hidden/fullfilename");
                var culturecode = templateInfo.GetXmlProperty("genxml/hidden/culturecode");
                var jsonDict = GetResxDictionary(new SimplisityRecord(postInfo), culturecode);
                if (culturecode == "")
                {
                    // build default with current field names.
                    foreach (var f in fieldNames)
                    {
                        var keyname = f.Key;
                        if (defaultDict.ContainsKey(keyname))
                        {
                            defaultDict[f.Key] = f.Value;
                        }
                        else
                        {
                            defaultDict.Add(f.Key, f.Value);
                        }
                    }
                    jsonDict = new Dictionary<string, string>();
                    foreach (var d in defaultDict)
                    {
                        var keyname = d.Key;
                        jsonDict.Add(keyname, defaultDict[keyname].Replace("]]>", "").Replace("\"", ""));
                        fileFields += "  <data name=\"" + keyname + "\" xml:space=\"preserve\"><value><![CDATA[" + defaultDict[keyname].Replace("]]>", "").Replace("\"", "") + "]]></value></data>";
                    }
                }
                else
                {
                    if (jsonDict.Count() == 0)
                    {
                        // build empty resx from default.
                        foreach (var d in defaultDict)
                        {
                            var keyname = d.Key;
                            jsonDict.Add(keyname, defaultDict[keyname].Replace("]]>", "").Replace("\"", ""));
                            fileFields += "  <data name=\"" + keyname + "\" xml:space=\"preserve\"><value><![CDATA[" + defaultDict[keyname].Replace("]]>", "").Replace("\"", "") + "]]></value></data>";
                        }
                    }
                    else
                    {
                        // put default values in any empty values.
                        foreach (var f in jsonDict)
                        {
                            var keyname = f.Key;
                            if (jsonDict.ContainsKey(keyname))
                            {
                                var valueString = jsonDict[keyname].Replace("]]>", "");
                                if (valueString == "") valueString = defaultDict[keyname].Replace("]]>", "").Replace("\"", "");
                                fileFields += "  <data name=\"" + keyname + "\" xml:space=\"preserve\"><value><![CDATA[" + valueString + "]]></value></data>";
                            }
                        }
                    }
                }

                // build json and save.
                var jsonStr = BuildJsonResx(jsonDict);
                postInfo.SetXmlProperty("genxml/resxlist/genxml[" + idx + "]/hidden/jsonresx", GeneralUtils.EnCode(jsonStr));

                // Save to file
                var resxtemplate = FileUtils.ReadFile(AppProjectFolderMapPath + @"\AppThemeBase\resxtemplate.xml");
                if (resxtemplate != "")
                {
                    resxtemplate = resxtemplate.Replace("<injectdatanodes/>", fileFields);
                    FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\resx\\" + fname, resxtemplate);
                }
                idx += 1;
            }
            return postInfo;
        }

        private string BuildJsonResx(Dictionary<string,string> jsonDict)
        {
            var jsonStr = "{\"listdata\":[";
            var lp = 1;
            foreach (var j in jsonDict)
            {
                jsonStr += "{\"id\":\"name_" + lp + "\",\"value\":\"" + j.Key.Replace("\"", "") + "\",\"row\":\"" + lp + "\",\"listname\":\".resxlistvalues\",\"type\":\"text\"},";
                jsonStr += "{\"id\":\"value_" + lp + "\",\"value\":\"" + j.Value.Replace("\"", "") + "\",\"row\":\"" + lp + "\",\"listname\":\".resxlistvalues\",\"type\":\"text\"},";
                lp += 1;
            }
            jsonStr = jsonStr.TrimEnd(',') + "]}";
            return jsonStr;
        }

        private Dictionary<string,string> GetResxDictionary(SimplisityRecord appThemeRecord, string culturecode = "")
        {
            var tList = appThemeRecord.GetRecordList("resxlist");
            // get defaultDict
            var defaultDict = new Dictionary<string, string>();
            foreach (SimplisityRecord templateInfo in tList)
            {
                var culturecode1 = templateInfo.GetXmlProperty("genxml/hidden/culturecode");
                if (culturecode1 == culturecode)
                {
                    var jsondata = GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/jsonresx"));
                    if (jsondata != "")
                    {
                        var jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, DNNrocketUtils.GetEditCulture());
                        var row = 1;
                        foreach (var i in jasonInfo.GetList("resxlistvalues"))
                        {
                            var keyname = i.GetXmlProperty("genxml/text/*[1]").Replace("\"", "");
                            if (keyname != "")
                            {
                                if (!defaultDict.ContainsKey(keyname))
                                {
                                    defaultDict.Add(keyname, i.GetXmlProperty("genxml/text/*[2]").Replace("\"",""));
                                }
                            }
                            row += 1;
                        }
                    }
                }
            }
            return defaultDict;
        }

        public void Update()
        {
            _objCtrl.SaveRecord(Record, _tableName);
        }
        public void AddListImage()
        {
            Record.AddListItem("imagelist");
            Update();
        }
        public void AddListTemplate(string filename = "", string templateText = "")
        {
            AddListFile("templatelist", filename, templateText, "htmlmixed");
        }
        public void AddListCss(string filename = "", string templateText = "")
        {
            AddListFile("csslist", filename, templateText, "css");
        }
        public void AddListJs(string filename = "", string templateText = "")

        {
            AddListFile("jslist", filename, templateText, "javascript");
        }
        private void AddListFile(string listname, string filename = "", string templateText = "", string modeType = "")
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            if (filename != "")
            {
                Record.RemoveRecordListItem(listname, "genxml/hidden/filename", filename);
                var nbi = new SimplisityRecord();
                nbi.SetXmlProperty("genxml/hidden/filename", filename);
                nbi.SetXmlProperty("genxml/hidden/editorcode" + modeType, GeneralUtils.EnCode(templateText));
                Record.AddListItem(listname, nbi.XMLData);
            }
            else
            {
                Record.AddListItem(listname);
            }
            Update();
        }

        public void ReBuildResx(string culturecode)
        {
            var listname = "resxlist";
            var nbi = new SimplisityRecord();
            if (culturecode == "")
            {
                nbi.SetXmlProperty("genxml/hidden/fullfilename", AppThemeFolder + ".resx");
            }
            else
            {
                nbi.SetXmlProperty("genxml/hidden/fullfilename", AppThemeFolder + "." + culturecode + ".resx");
            }
            nbi.SetXmlProperty("genxml/hidden/culturecode", culturecode);
            nbi.SetXmlProperty("genxml/hidden/filefolder", "resx");

            var jsonDict = new Dictionary<string, string>();
            var rtnDict = GetFieldDictionaryAll();
            var resxDict = GetResxDictionary(Record, culturecode);
            foreach (var d in rtnDict)
            {
                jsonDict.Add(d.Key, d.Value);
            }
            foreach (var d in resxDict)
            {
                if (jsonDict.ContainsKey(d.Key)) jsonDict.Remove(d.Key);
                jsonDict.Add(d.Key, d.Value);
            }
            var jsonStr = BuildJsonResx(jsonDict);
            nbi.SetXmlProperty("genxml/hidden/jsonresx", GeneralUtils.EnCode(jsonStr));

            var l = Record.GetRecordList(listname);
            var idx = 1;
            var rtnidx = -1;
            foreach (var r in l)
            {
                if (r.GetXmlProperty("genxml/hidden/culturecode") == culturecode) rtnidx = idx;
                idx += 1;
            }
            if (rtnidx >= 1)
            {
                Record.RemoveRecordListItem(listname, rtnidx);
                Record.AddListItem(listname, nbi.XMLData);
                Update();
            }
        }


        public void AddListResx(string culturecode, string jsonresx = "")
        {
            var listname = "resxlist";
            var nbi = new SimplisityRecord();
            if (culturecode == "")
            {
                nbi.SetXmlProperty("genxml/hidden/fullfilename", AppThemeFolder + ".resx");
            }
            else
            {
                nbi.SetXmlProperty("genxml/hidden/fullfilename", AppThemeFolder + "." + culturecode + ".resx");
            }
            nbi.SetXmlProperty("genxml/hidden/culturecode", culturecode);
            nbi.SetXmlProperty("genxml/hidden/filefolder", "resx");

            var jsonDict = new Dictionary<string, string>();
            var rtnDict = GetResxDictionary(Record);
            foreach (var d in rtnDict)
            {
                jsonDict.Add(d.Key,d.Value);
            }
            var jsonStr = BuildJsonResx(jsonDict);
            nbi.SetXmlProperty("genxml/hidden/jsonresx", GeneralUtils.EnCode(jsonStr));

            Record.AddListItem(listname, nbi.XMLData);
            Update();
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
                if (!Directory.Exists(AppThemeVersionFolderMapPath)) CreateVersionFolders(AppVersionFolder);
            }
        }

        public string CopyVersion(string sourceVersionFolder, string destVersionFolder)
        {
            var sourceVersionFolderMapPath = AppThemeFolderMapPath + "\\" + sourceVersionFolder;
            var destVersionFolderMapPath = AppThemeFolderMapPath + "\\" + destVersionFolder;
            if (!Directory.Exists(destVersionFolderMapPath)) Directory.CreateDirectory(destVersionFolderMapPath);
            if (Directory.Exists(sourceVersionFolderMapPath) && !Directory.EnumerateFileSystemEntries(destVersionFolderMapPath).Any())
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourceVersionFolderMapPath, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceVersionFolderMapPath, destVersionFolderMapPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourceVersionFolderMapPath, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourceVersionFolderMapPath, destVersionFolderMapPath), true);

                AppVersionFolder = destVersionFolder;

                // copy DB record
                var versionCopyRecord = _objCtrl.GetRecordByGuidKey(Record.PortalId, -1, _entityTypeCode, _guidKey, "", _tableName);
                _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;
                versionCopyRecord.GUIDKey = _guidKey;
                versionCopyRecord.ItemID = -1;
                versionCopyRecord.SetXmlProperty("genxml/select/versionfolder", AppVersionFolder);

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
            var systemInfoData = new SystemInfoData(SystemKey);

            List<SimplisityRecord> fieldList = Record.GetRecordList(listname);
            var resxItem = Record.GetRecordListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = GeneralUtils.DeCode(resxItem.GetXmlProperty("genxml/hidden/jsonresx"));
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
                        var datatext = "ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString()";
                        strFieldList += "\t\t@DropDownList(info, \"" + xpath + "\",\"" + datavalue + "\"," + datatext + ",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        var datatext = "ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString()";
                        strFieldList += "\t\t@RadioButtonList(info,\"" + xpath + "\",\"" + datavalue.Replace("\"", "\\\"") + "\"," + datatext + ",\"" + attributes + "\",\"" + defaultValue + "\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        var datatext = "ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString()";
                        strFieldList += "\t\t@CheckBoxList(info,\"" + xpath + "\",\"" + datavalue + "\"," + datatext + ",\"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
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
                        strFieldList += "\t\t\t@RenderTemplate(\"editimages.cshtml\", \"" + systemInfoData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        strFieldList += "\t\t<div id='documentlistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"docfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editdocuments.cshtml\", \"" + systemInfoData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strFieldList += "\t\t<div id='linklistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"linkfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editlinks.cshtml\", \"" + systemInfoData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }

                    strFieldList += "\t</div>" + Environment.NewLine;
                }
                strFieldList += "</div>" + Environment.NewLine;
            }


            // merge to template            
            var strOut = FileUtils.ReadFile(systemInfoData.SystemMapPath + "\\AppThemeBase\\" + basefile + ".cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\" + basefile + ".cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = strOut.Replace("[Token:AppThemeFields]", strFieldList);
                strOut = strOut.Replace("[Token:SystemKey]", SystemKey);
                strOut = strOut.Replace("[Token:DefaultInterface]", systemInfoData.DefaultInterface);
                strOut = strOut.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

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
            var systemInfoData = new SystemInfoData(SystemKey);
            var strOut = FileUtils.ReadFile(systemInfoData.SystemMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = strOut.Replace("[Token:List]", strFieldList);
                strOut = strOut.Replace("[Token:SystemKey]", SystemKey);
                strOut = strOut.Replace("[Token:DefaultInterface]", systemInfoData.DefaultInterface);
                strOut = strOut.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

        }

        private string GenerateView(int row)
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
            var systemInfoData = new SystemInfoData(SystemKey);
            var strOut = FileUtils.ReadFile(systemInfoData.SystemMapPath + "\\AppThemeBase\\view.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\view.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = strOut.Replace("[Token:List]", strFieldList);
                strOut = strOut.Replace("[Token:SystemKey]", SystemKey);
                strOut = strOut.Replace("[Token:DefaultInterface]", systemInfoData.DefaultInterface);
                strOut = strOut.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

        }

        private void UpdateFieldXpath(string listname)
        {
            List<SimplisityRecord> fieldList = Record.GetRecordList(listname);
            var lp = 1;
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

                if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "textarea")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                {
                    xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                {
                    xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                {
                    xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                {
                    xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }

                if (localized) xpath = "genxml/lang/" + xpath;

                if (xpath != "")
                {
                    Record.SetXmlProperty("genxml/" + listname + "/genxml[" + lp + "]/hidden/xpath",xpath);
                }
                lp += 1;
            }
            _objCtrl.Update(Record);
        }

        public string ExportXML()
        {
            // Export DB
            var exportData = "<root>";
            var l = _objCtrl.GetList(-1, -1, "APPTHEME", "and guidkey like 'appTheme*" + SystemKey + "*" + AppThemeFolder + "*%' ");
            foreach (SimplisityInfo i in l)
            {
                var r = new SimplisityRecord(i);
                if (r.GetXmlProperty("genxml/hidden/selectedsystemkey") == SystemKey) // only export if we have a systemkey.
                {
                    var appVersionFolder = r.GetXmlProperty("genxml/select/versionfolder");
                    r.ItemID = -1;
                    r.SetXmlProperty("genxml/hidden/appthemefolder", AppThemeFolder);
                    exportData += r.ToXmlItem();
                }
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
            if (Directory.Exists(tempZipFolder)) Directory.Delete(tempZipFolder, true);
            ZipFile.ExtractToDirectory(zipMapPath, tempZipFolder);

            var xmlImport = FileUtils.ReadFile(tempZipFolder + "\\export.xml");

            var iRec = new SimplisityRecord();
            iRec.XMLData = xmlImport;

            var appThemeFolder = iRec.GetXmlProperty("root/item[1]/genxml/hidden/appthemefolder");

            if (appThemeFolder != "")
            {

                // import DB records.
                var systemKey = "";
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
                    var appProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
                    var appSystemThemeFolderRel = appProjectFolderRel + "/SystemThemes/" + systemKey;
                    var appSystemThemeFolderMapPath = DNNrocketUtils.MapPath(appSystemThemeFolderRel);

                    var destinationFolder = appSystemThemeFolderMapPath + "\\" + appThemeFolder;
                    if (Directory.Exists(destinationFolder)) Directory.Delete(destinationFolder, true);
                    DirectoryCopy(tempZipFolder, destinationFolder, true);
                    if (Directory.Exists(tempZipFolder)) Directory.Delete(tempZipFolder, true);

                    InitAppTheme(systemKey, appThemeFolder, lastversion.ToString("F1", CultureInfo.InvariantCulture), false);
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
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
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
        public string AppThemeVersionFolderRel { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
        public List<string> VersionList { get; set; }
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
        public bool RegenerateEditList { get { return Record.GetXmlPropertyBool("genxml/checkbox/regenerateeditlist"); } }
        public bool RegenerateEdit { get { return Record.GetXmlPropertyBool("genxml/checkbox/regenerateedit"); } }
        public bool RegenerateSettings { get { return Record.GetXmlPropertyBool("genxml/checkbox/regeneratesettings"); } }
        public bool RegenerateView { get { return Record.GetXmlPropertyBool("genxml/checkbox/regenerateview"); } }

        public int DataType { get { return Record.GetXmlPropertyInt("genxml/radio/themetype"); } }
        public bool EnableSettings { get { return Record.GetXmlPropertyBool("genxml/checkbox/enablesettings"); } }
        public int SystemId { get; set; }
        public string SystemKey { get; set; }
        public Dictionary<string, SimplisityRecord> SnippetText { get; set; }
        public Dictionary<string, SimplisityRecord> RazorTokenText { get; set; }
        public DNNrocketController RocketController { get { return _objCtrl; } }

        #endregion

    }

}
