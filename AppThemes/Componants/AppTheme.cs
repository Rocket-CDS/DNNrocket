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
        private List<string> _templateFileName;
        private List<string> _cssFileName;
        private List<string> _jsFileName;
        private List<string> _resxFileName;
        private bool _debugMode;

        public AppTheme(string systemKey, string appThemeFolder, string langRequired = "", string versionFolder = "", bool debugMode = false)
        {
            _debugMode = debugMode;
            _objCtrl = new DNNrocketController();
            AppProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
            _systemKey = systemKey;

            _templateFileName = new List<string>();
            _cssFileName = new List<string>();
            _jsFileName = new List<string>();
            _resxFileName = new List<string>();

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
            if (!_templateFileName.Contains(tempMapPath) || RegenerateEdit)
            {
                var formHtml = AppThemeUtils.GeneraateEditForm(this,0);
                FileUtils.SaveFile(tempMapPath, formHtml);
                _templateFileName.Add(tempMapPath);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
            if (!_templateFileName.Contains(tempMapPath) || RegenerateEditList)
            {
                //var formHtml = AppThemeUtils.GeneraateEditForm(Info.GetList("fielddata"), 0);
                FileUtils.SaveFile(tempMapPath, "");
                _templateFileName.Add(tempMapPath);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
            if (!_templateFileName.Contains(tempMapPath))
            {
                //var formHtml = AppThemeUtils.GeneraateEditForm(Info.GetList("fielddata"), 0);
                FileUtils.SaveFile(tempMapPath, "");
                _templateFileName.Add(tempMapPath);
            }
            tempMapPath = AppThemeVersionFolderMapPath + "\\default\\pageheader.cshtml";
            if (!_templateFileName.Contains(tempMapPath))
            {
                //var formHtml = AppThemeUtils.GeneraateEditForm(Info.GetList("fielddata"), 0);
                FileUtils.SaveFile(tempMapPath, "");
                _templateFileName.Add(tempMapPath);
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
            if (Path.GetExtension(templateName) == "") templateName += ".cshtml";
            if (!_templateFileName.Contains(templateName)) return "";
            var rtnItem = Info.GetListItem("templatelist", "genxml/hidden/filename", templateName);
            return rtnItem.GetXmlProperty("genxml/hidden/editorcode");
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
            ActionListTemplateFiles(postInfo);
            ActionListCssFiles(postInfo);
            ActionListJsFiles(postInfo);
            ActionListResxFiles(postInfo);


            var dbInfo = _objCtrl.GetData(_entityTypeCode, Info.ItemID, AppCultureCode, -1, -1, true, _tableName);
            if (dbInfo != null)
            {
                // NOTE: Be careful of the order for saving.  The sort list is a PAIN and requires speciifc data.
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

                // sort lists from DB and post data
                var sortLists = new DNNrocketAPI.Componants.SortLists(dbInfo, _tableName, false);
                sortLists.Save();

                Populate();

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

        public void CopyVersion(string sourceVersionFolder, string destVersionFolder)
        {
            sourceVersionFolder = AppThemeFolderMapPath + "\\" + sourceVersionFolder;
            destVersionFolder = AppThemeFolderMapPath + "\\" + destVersionFolder;

            if (Directory.Exists(sourceVersionFolder))
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourceVersionFolder, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceVersionFolder, destVersionFolder));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourceVersionFolder, "*.*", SearchOption.AllDirectories))
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
        public bool RegenerateEditList { get { return Info.GetXmlPropertyBool("genxml/checkbox/regenerateeditlist"); } }
        public bool RegenerateEdit { get { return Info.GetXmlPropertyBool("genxml/checkbox/regenerateedit"); } }


        #endregion

    }

}
