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
        private string _guidKey;        
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "APPTHEME";
        private DNNrocketController _objCtrl;
        private List<string> _templateFileName;
        private List<string> _cssFileName;
        private List<string> _jsFileName;
        private List<string> _resxFileName;
        private bool _debugMode;

        public AppTheme(string systemKey, string appThemeFolder, string versionFolder, string langRequired = "", bool debugMode = false)
        {
            _debugMode = debugMode;
            _objCtrl = new DNNrocketController();
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            SystemKey = systemKey;

            _templateFileName = new List<string>();
            _cssFileName = new List<string>();
            _jsFileName = new List<string>();
            _resxFileName = new List<string>();

            AppThemeFolder = appThemeFolder;
            AppVersionFolder = versionFolder;

            AppSystemThemeFolderRel = AppProjectFolderRel + "/SystemThemes/" + SystemKey;

            AppThemeFolderRel = AppSystemThemeFolderRel + "/" + AppThemeFolder;

            AppCultureCode = langRequired;
            if (AppCultureCode == "") AppCultureCode = DNNrocketUtils.GetEditCulture();

            AssignVersionFolders();

            PopulateVersionList();
            if (AppVersionFolder == "") AppVersionFolder = LatestVersionFolder; ;
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

        }

        public void Populate()
        {
            Info = _objCtrl.GetData(_guidKey, _entityTypeCode, AppCultureCode, -1, -1, false, _tableName);

            AppName = Info.GetXmlProperty("genxml/textbox/appname");
            AppDisplayName = Info.GetXmlProperty("genxml/lang/genxml/textbox/displayname");
            AppSummary = Info.GetXmlProperty("genxml/lang/genxml/textbox/summary");

            // sync filesystem
            SyncFiles();

            // logo, take first image
            var imageList = Info.GetList("imagelist");
            Logo = "";
            if (imageList.Count > 0)
            {
                var i = Info.GetListItem("imagelist", 0);
                var logoMapPath = DNNrocketUtils.MapPath(i.GetXmlProperty("genxml/hidden/imagepath"));
                Logo = i.GetXmlProperty("genxml/hidden/imagepath");
                if (!File.Exists(logoMapPath)) Logo = "";
            }

            // RESX Default
            var resxList = Info.GetList("resxlist");
            if (resxList.Count == 0) // create default
            {
                AddListResx("");
                resxList = Info.GetList("resxlist");
            }

            foreach (var r in resxList)
            {
                _resxFileName.Add(r.GetXmlProperty("genxml/hidden/fullfilename"));
            }

            TemplateDefaults();

        }

        private void TemplateDefaults()
        {
            // Tempalte Defaults
            var tempMapPath = AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var formHtml = GenerateEditForm(0);
                FileUtils.SaveFile(tempMapPath, formHtml);
                _templateFileName.Add(Path.GetFileName(tempMapPath));
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var listHtml = GenerateEditList(0);
                FileUtils.SaveFile(tempMapPath, listHtml);
                _templateFileName.Add(Path.GetFileName(tempMapPath));
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                var viewHtml = GenerateView(0);
                FileUtils.SaveFile(tempMapPath, "");
                _templateFileName.Add(Path.GetFileName(tempMapPath));
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\pageheader.cshtml";
            if (!_templateFileName.Contains(Path.GetFileName(tempMapPath)))
            {
                //var formHtml = AppThemeUtils.GeneraateEditForm(Info.GetList("fielddata"), 0);
                FileUtils.SaveFile(tempMapPath, "");
                _templateFileName.Add(Path.GetFileName(tempMapPath));
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

        public string GetTemplate(string templateName)
        {
            if (Path.GetExtension(templateName) != "") templateName += Path.GetFileNameWithoutExtension(templateName);
            if (!_templateFileName.Contains(templateName + ".cshtml")) return "";
            var rtnItem = Info.GetListItem("templatelist", "genxml/hidden/filename", templateName);
            if (rtnItem == null) return "";
            return GeneralUtils.DeCode(rtnItem.GetXmlProperty("genxml/hidden/editorcodehtmlmixed"));
        }

        public void DeleteTheme()
        {
            if (Directory.Exists(AppThemeFolderMapPath))
            {
                Directory.Delete(AppThemeFolderMapPath, true);
            }
        }
        public void DeleteVersion()
        {

            if (Directory.Exists(AppThemeVersionFolderMapPath))
            {
                Directory.Delete(AppThemeVersionFolderMapPath, true);
            }

            var versionRecord = _objCtrl.GetRecordByGuidKey(Info.PortalId, -1, _entityTypeCode, _guidKey, "", _tableName);
            _objCtrl.Delete(versionRecord.ItemID, _tableName); // cascade delete

            PopulateVersionList();
            AppVersionFolder = LatestVersionFolder;
        }
        public void Save(SimplisityInfo postInfo)
        {
            ActionListTemplateFiles(postInfo);
            ActionListCssFiles(postInfo);
            ActionListJsFiles(postInfo);
            ActionListResxFiles(postInfo);


            var dbInfo = _objCtrl.GetData(_entityTypeCode, Info.ItemID, AppCultureCode, -1, -1, true, _tableName);
            if (dbInfo != null)
            {

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
                _objCtrl.SaveData(dbInfo, Info.ItemID, _tableName);

                Populate();

                // output generated template.
                var formHtml = GetTemplate("edit");
                if (RegenerateEdit) formHtml = GenerateEditForm(0);
                var tempMapPath = AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";
                if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

                var listHtml = GetTemplate("editlist");
                if (RegenerateEditList) listHtml = GenerateEditList(0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
                if (listHtml != "") FileUtils.SaveFile(tempMapPath, listHtml);

                var viewHtml = GetTemplate("view");
                if (RegenerateView) viewHtml = GenerateView(0);
                tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
                if (viewHtml != "") FileUtils.SaveFile(tempMapPath, viewHtml);

            }
        }
        private void ActionListTemplateFiles(SimplisityInfo postInfo)
        {

            foreach (var t in _templateFileName)
            {
                var filename = Path.GetFileNameWithoutExtension(t);
                var delItem = postInfo.GetListItem("templatelist", "genxml/hidden/filename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\default\\" + t)) File.Delete(AppThemeVersionFolderMapPath + "\\default\\" + t);
            }

            //create any new files. (will be added to template list when populate syncs files)
            var tList = postInfo.GetList("templatelist");
            foreach (SimplisityInfo templateInfo in tList)
            {
                var fname = templateInfo.GetXmlProperty("genxml/hidden/filename");
                var filename = Path.GetFileNameWithoutExtension(fname);
                fname = filename + ".cshtml";
                FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\default\\" + fname, GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodehtmlmixed")));
            }
        }
        private void ActionListCssFiles(SimplisityInfo postInfo)
        {
            foreach (var t in _cssFileName)
            {
                var filename = Path.GetFileNameWithoutExtension(t);
                var delItem = postInfo.GetListItem("csslist", "genxml/hidden/filename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\css\\" + t)) File.Delete(AppThemeVersionFolderMapPath + "\\css\\" + t);
            }

            //create any new files. (will be added to template list when populate syncs files)
            var tList = postInfo.GetList("csslist");
            foreach (SimplisityInfo templateInfo in tList)
            {
                var fname = templateInfo.GetXmlProperty("genxml/hidden/filename");
                var filename = Path.GetFileNameWithoutExtension(fname);
                fname = filename + ".css";
                FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\css\\" + fname, GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodecss")));
            }
        }
        private void ActionListJsFiles(SimplisityInfo postInfo)
        {
            foreach (var t in _jsFileName)
            {
                var filename = Path.GetFileNameWithoutExtension(t);
                var delItem = postInfo.GetListItem("jslist", "genxml/hidden/filename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\js\\" + t)) File.Delete(AppThemeVersionFolderMapPath + "\\js\\" + t);
            }

            //create any new files. (will be added to template list when populate syncs files)
            var tList = postInfo.GetList("jslist");
            foreach (SimplisityInfo templateInfo in tList)
            {
                var fname = templateInfo.GetXmlProperty("genxml/hidden/filename");
                var filename = Path.GetFileNameWithoutExtension(fname);
                fname = filename + ".js";
                FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\js\\" + fname, GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/editorcodejavascript")));
            }
        }
        private void ActionListResxFiles(SimplisityInfo postInfo)
        {
            foreach (var t in _resxFileName)
            {
                var filename = t;
                var delItem = postInfo.GetListItem("resxlist", "genxml/hidden/fullfilename", filename);
                if (delItem == null && File.Exists(AppThemeVersionFolderMapPath + "\\resx\\" + filename)) File.Delete(AppThemeVersionFolderMapPath + "\\resx\\" + filename);
            }

            //Write files.
            var tList = postInfo.GetList("resxlist");
            foreach (SimplisityInfo templateInfo in tList)
            {
                var fileFields = "";
                var jsonDict = new Dictionary<string, string>();
                var fname = templateInfo.GetXmlProperty("genxml/hidden/fullfilename");
                var jsondata = GeneralUtils.DeCode(templateInfo.GetXmlProperty("genxml/hidden/jsonresx"));
                if (jsondata != "")
                {
                    var jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, AppCultureCode);
                    var row = 1;
                    foreach (var i in jasonInfo.GetList("resxlistvalues"))
                    {
                        if (i.GetXmlProperty("genxml/text/name" + row) != "" && !jsonDict.ContainsKey(i.GetXmlProperty("genxml/text/name" + row)))
                        {
                            jsonDict.Add(i.GetXmlProperty("genxml/text/name" + row), i.GetXmlProperty("genxml/text/value" + row));
                        }
                        row += 1;
                    }
                    foreach (var i in jsonDict)
                    {
                        fileFields += "  <data name=\"" + i.Key + "\" xml:space=\"preserve\"><value>" + i.Value + "</value></data>";
                    }
                }

                var resxtemplate = FileUtils.ReadFile(AppProjectFolderMapPath + @"\Themes\config-w3\1.0\default\resxtemplate.xml");
                if (resxtemplate != "")
                {
                    resxtemplate = resxtemplate.Replace("<injectdatanodes/>", fileFields);
                    FileUtils.SaveFile(AppThemeVersionFolderMapPath + "\\resx\\" + fname, resxtemplate);
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
        public void AddListTemplate(string filename = "", string templateText = "")
        {
            AddListFile("templatelist", filename, templateText,"htmlmixed");
        }
        public void AddListCss(string filename = "", string templateText = "")
        {
            AddListFile("csslist", filename, templateText, "css");
        }
        public void AddListJs(string filename = "", string templateText = "")
        {
            AddListFile("jslist", filename, templateText,"javascript");
        }
        private void AddListFile(string listname, string filename = "", string templateText = "", string modeType = "")
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            if (filename != "")
            {
                var rtnItem = Info.GetListItem(listname, "genxml/hidden/filename", filename);
                if (rtnItem != null)
                {
                    // update
                    var idx = rtnItem.GetXmlPropertyInt("genxml/index");
                    Info.SetXmlProperty("genxml/" + listname + "/genxml[" + idx + "]/hidden/filename", filename);
                    Info.SetXmlProperty("genxml/" + listname + "/genxml[" + idx + "]/hidden/editorcode" + modeType, GeneralUtils.EnCode(templateText));
                }
                else
                {
                    // add
                    var nbi = new SimplisityRecord();
                    nbi.SetXmlProperty("genxml/hidden/filename", filename);
                    nbi.SetXmlProperty("genxml/hidden/editorcode" + modeType, GeneralUtils.EnCode(templateText));
                    Info.AddListItem(listname, nbi.XMLData);
                }
            }
            else
            {
                Info.AddListItem(listname);
            }
            Update();
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
            nbi.SetXmlProperty("genxml/hidden/jsonresx", jsonresx);
            Info.AddListItem(listname, nbi.XMLData);
            Update();
        }

        public void AddListField()
        {
            var listname = "fielddata";
            Info.AddListItem(listname);
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
                var versionCopyRecord = _objCtrl.GetRecordByGuidKey(Info.PortalId,-1, _entityTypeCode, _guidKey,"", _tableName);
                _guidKey = "appTheme*" + SystemKey + "*" + AppThemeFolder + "*" + AppVersionFolder;
                versionCopyRecord.GUIDKey = _guidKey;
                versionCopyRecord.ItemID = -1;
                versionCopyRecord.SetXmlProperty("genxml/select/versionfolder", AppVersionFolder);

                versionCopyRecord = _objCtrl.SaveRecord(versionCopyRecord, -1, _tableName);

                var l = _objCtrl.GetList(Info.PortalId, -1, _entityTypeCode + "LANG", " and R1.ParentItemId = " + Info.ItemID + " ", "", "", 0, 0, 0, 0, -1, _tableName);
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
            if (AppThemeFolder != "")
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
        }

        private string GenerateEditForm(int row)
        {
            var systemInfoData = new SystemInfoData(SystemKey);

            List<SimplisityInfo> fieldList = Info.GetList("fielddata");
            var resxItem = Info.GetListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = GeneralUtils.DeCode(resxItem.GetXmlProperty("genxml/hidden/jsonresx"));
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, AppCultureCode);
            }

            var strFieldList = "";
            // calc rows
            var frows = new List<List<SimplisityInfo>>();
            var fline = new List<SimplisityInfo>();
            var col = 0;
            foreach (var f in fieldList)
            {
                var size = f.GetXmlPropertyInt("genxml/select/size");
                if (size == 0 || size > 12) size = 12;
                col += size;
                if (col > 12)
                {
                    frows.Add(fline);
                    fline = new List<SimplisityInfo>();
                    fline.Add(f);
                    col = size;
                }
                else
                {
                    fline.Add(f);
                }
            }
            frows.Add(fline);

            var imageselect = false;
            var imagegallery = false;
            var docselect = false;
            var docgallery = false;

            foreach (var flines in frows)
            {
                strFieldList += "<div class='w3-row'>" + Environment.NewLine;
                foreach (var f in flines)
                {
                    var localized = f.GetXmlProperty("genxml/checkbox/localized").ToLower();
                    var localizedbool = f.GetXmlPropertyBool("genxml/checkbox/localized");
                    var xpath = "";
                    var size = f.GetXmlProperty("genxml/select/size");
                    var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                    var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                    var defaultBool = f.GetXmlProperty("genxml/textbox/defaultvalue").ToLower();
                    if (defaultBool == "") defaultBool = "false";
                    var attributes = f.GetXmlProperty("genxml/textbox/attributes");

                    strFieldList += "\t<div class='w3-col m" + size + " w3-padding'>" + Environment.NewLine;

                    if (label != "")
                    {
                        if (Path.GetExtension(label) == "") label = label + ".Text";
                        var resxLabelItem = jasonInfo.GetListItem("resxlistvalues", "genxml/text/name", label);
                        if (resxLabelItem != null)
                        {
                            strFieldList += "\t\t<label>@ResourceKey(\"" + AppThemeFolder + "." + label + "\")</label>";
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
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        strFieldList += "\t\t@TextBox(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                    {
                        xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        strFieldList += "\t\t@CheckBox(info,\"" + xpath + "\", \"\", \"" + attributes + "\", \"" + defaultBool + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strFieldList += "\t\t@DropDownList(info, \"" + xpath + "\",\"" + datavalue + "\",\"" + datatext + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + ",\"" + row + "\")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strFieldList += "\t\t@RadioButtonList(info,\"" + xpath + "\",\"" + datavalue + "\",\"" + datatext + "\",\"" + attributes + "\",\"" + defaultValue + "\", \"\"," + localized + ",\"" + row + "\")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strFieldList += "\t\t@CheckBoxList(info,\"" + xpath + "\",\"" + datavalue + "\",\"" + datatext + "\",\"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        imageselect = true;
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
                        imageselect = true;
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
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t\t@TabSelectListOnTabId(info,\"" + xpath + "\",\"" + attributes + "\"," + allowEmpty + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "document")
                    {
                        docselect = true;
                        var fieldid = f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t\t@DocumentEdit(info,\"" + fieldid + "\",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        strFieldList += "\t\t<div class='w3-col m12'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@CKEditor(info,\"" + xpath + "\",\"\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                        strFieldList += "\t\t<div class='w3-col' style='width:0px;height:300px;'></div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        imagegallery = true;
                        strFieldList += "\t\t<div id='imagelistcontainer'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editimages.cshtml\", \"" + systemInfoData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        docgallery = true;
                        strFieldList += "\t\t<div id='documentlistcontainer'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editdocuments.cshtml\", \"" + systemInfoData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strFieldList += "\t\t<div id='linklistcontainer'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editlinks.cshtml\", \"" + systemInfoData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }

                    strFieldList += "\t</div>" + Environment.NewLine;
                }
                strFieldList += "</div>" + Environment.NewLine;
            }

            if (imagegallery || imageselect)
            {
                strFieldList += "<div id=\"dnnrocket_imageselectwrapper\">@RenderImageSelect(100, true, false, articleData.ImageFolder)</div>";
            }
            if (docgallery || docselect)
            {
                strFieldList += "<div id=\"dnnrocket_documentselectwrapper\">@RenderDocumentSelect(true, false, articleData.DocumentFolder)</div>";
            }


            // merge to template            
            var strOut = FileUtils.ReadFile(systemInfoData.SystemMapPath + "\\AppThemeBase\\edit.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(AppProjectFolderMapPath + "\\AppThemeBase\\edit.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = strOut.Replace("[Token:AppThemeFields]", strFieldList);
                strOut = strOut.Replace("[Token:DisplayName]", AppDisplayName);
                strOut = strOut.Replace("[Token:SystemKey]", SystemKey);
                strOut = strOut.Replace("[Token:DefaultInterface]", systemInfoData.DefaultInterface);
                strOut = strOut.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

        }

        private string GenerateEditList(int row)
        {

            List<SimplisityInfo> fieldList = Info.GetList("fielddata");
            var resxItem = Info.GetListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = GeneralUtils.DeCode(resxItem.GetXmlProperty("genxml/hidden/jsonresx"));
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, AppCultureCode);
            }

            var strFieldList = "";
            var sortedList = new List<SimplisityInfo>();
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
                var xpath = "";
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");

                if (isonlist)
                {
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
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

                    if (localized) xpath = "genxml/lang/" + xpath;

                    if (xpath != "")
                    {
                        strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                        strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }


                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "@if (info.GetXmlProperty(\"" + xpath + "\") != \"\")" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "else" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=60&h=60' imageheight='60' imagewidth='60'>" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "@if (info.GetListItem(\"imagelist\", 0).GetXmlProperty(\"genxml/hidden/imagepath\") != \"\")" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist\", 0).GetXmlProperty(\"genxml/hidden/imagepath\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "else" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=60&h=60' imageheight='60' imagewidth='60'>" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
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
                strOut = strOut.Replace("[Token:DisplayName]", AppDisplayName);
                strOut = strOut.Replace("[Token:SystemKey]", SystemKey);
                strOut = strOut.Replace("[Token:DefaultInterface]", systemInfoData.DefaultInterface);
                strOut = strOut.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

        }

        private string GenerateView(int row)
        {

            List<SimplisityInfo> fieldList = Info.GetList("fielddata");
            var resxItem = Info.GetListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = GeneralUtils.DeCode(resxItem.GetXmlProperty("genxml/hidden/jsonresx"));
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, AppCultureCode);
            }

            var strFieldList = "";
            foreach (var f in fieldList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = "";
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");

                if (isonlist)
                {
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
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

                    if (localized) xpath = "genxml/lang/" + xpath;

                    if (xpath != "")
                    {
                        strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                        strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }


                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "@if (info.GetXmlProperty(\"" + xpath + "\") != \"\")" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "else" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=60&h=60' imageheight='60' imagewidth='60'>" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "@if (info.GetListItem(\"imagelist\", 0).GetXmlProperty(\"genxml/hidden/imagepath\") != \"\")" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist\", 0).GetXmlProperty(\"genxml/hidden/imagepath\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "else" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=60&h=60' imageheight='60' imagewidth='60'>" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
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
                strOut = strOut.Replace("[Token:DisplayName]", AppDisplayName);
                strOut = strOut.Replace("[Token:SystemKey]", SystemKey);
                strOut = strOut.Replace("[Token:DefaultInterface]", systemInfoData.DefaultInterface);
                strOut = strOut.Replace("[Token:appthemeresx]", AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

        }

        #region "properties"

        public string AppProjectFolderRel { get; set; }
        public string AppProjectFolderMapPath { get; set; }
        public string AppSystemThemeFolderRel { get; set; }
        public string AppSystemThemeFolderMapPath { get; set; }
        public string AppName { get; private set; }
        public string AppDisplayName { get; private set; }
        public string AppSummary { get; private set; }
        public string Logo { get; private set; }
        public string AppCultureCode { get; set; }
        public string AppThemeFolder { get; set; }
        public string AppThemeFolderRel { get; set; }
        public string AppThemeFolderMapPath { get; set; }
        public string AppVersionFolder { get; set; }
        public string AppThemeVersionFolderRel { get; set; }
        public string AppThemeVersionFolderMapPath { get; set; }
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
        public bool RegenerateEditList { get { return Info.GetXmlPropertyBool("genxml/checkbox/regenerateeditlist"); } }
        public bool RegenerateEdit { get { return Info.GetXmlPropertyBool("genxml/checkbox/regenerateedit"); } }
        public bool RegenerateView { get { return Info.GetXmlPropertyBool("genxml/checkbox/regenerateview"); } }

        public int DataType { get { return Info.GetXmlPropertyInt("genxml/radio/themetype"); } }

        public string SystemKey { get; set; }

        #endregion

    }

}
