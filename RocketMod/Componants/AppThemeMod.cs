using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketMod.Componants
{
    public class AppThemeMod
    {
        private bool _debugMode;
        private string _systemKey;

        public AppThemeMod(int moduleId, string systemKey, bool debugMode = false)
        {
            _debugMode = debugMode;
            ModuleId = moduleId;
            _systemKey = systemKey;

            InitAppThemeMod();
            LoadTemplates();
        }

        private void InitAppThemeMod() {

            ModuleParams = new ModuleParams(ModuleId, SystemId);
            SystemInfoData = new SystemInfoData(_systemKey);
            AppTheme = new AppTheme(SystemInfoData.SystemKey, ModuleParams.AppThemeFolder, ModuleParams.AppThemeVersion, _debugMode);

            ModuleTemplateFolderMapPath = DNNrocketUtils.HomeDirectoryMapPath().Trim('\\') + "\\DNNrocketThemes";
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + AppTheme.SystemKey;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + AppTheme.AppThemeFolder;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + AppTheme.AppVersionFolder;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            if (!Directory.Exists(ModuleTemplateFolderRazorMapPath)) Directory.CreateDirectory(ModuleTemplateFolderRazorMapPath);
            if (!Directory.Exists(ModuleTemplateFolderCSSMapPath)) Directory.CreateDirectory(ModuleTemplateFolderCSSMapPath);
            if (!Directory.Exists(ModuleTemplateFolderJSMapPath)) Directory.CreateDirectory(ModuleTemplateFolderJSMapPath);

            
        }

        private void LoadTemplates()
        {
            ModuelTemplateListRazor = AppTheme.GetTemplateDictionaryRazor();
            ModuelTemplateListJS = AppTheme.GetTemplateDictionaryJS();
            ModuelTemplateListCSS = AppTheme.GetTemplateDictionaryCSS();

            // get razor
            if (ModuelTemplateListRazor != null) // does not exist on selection of Module AppTheme.
            {
                var flist = Directory.GetFiles(ModuleTemplateFolderRazorMapPath, ModuleParams.ModuleRef + "_*.cshtml");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    var ftext = FileUtils.ReadFile(f);
                    if (ModuelTemplateListRazor.ContainsKey(fname)) ModuelTemplateListRazor.Remove(fname);
                    ModuelTemplateListRazor.Add(fname, ftext);
                }
                // get css
                flist = Directory.GetFiles(ModuleTemplateFolderCSSMapPath, ModuleParams.ModuleRef + "_*.css");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    var ftext = FileUtils.ReadFile(f);
                    if (ModuelTemplateListCSS.ContainsKey(fname)) ModuelTemplateListCSS.Remove(fname);
                    ModuelTemplateListCSS.Add(fname, ftext);
                }
                // get js
                flist = Directory.GetFiles(ModuleTemplateFolderJSMapPath, ModuleParams.ModuleRef + "_*.js");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    var ftext = FileUtils.ReadFile(f);
                    if (ModuelTemplateListJS.ContainsKey(fname)) ModuelTemplateListJS.Remove(fname);
                    ModuelTemplateListJS.Add(fname, ftext);
                }

                foreach (var d in ModuelTemplateListRazor)
                {
                    var itemInfo = AppTheme.Record.GetRecordListItem("templatelist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                    if (itemInfo != null)
                    {
                        AppTheme.Record.RemoveRecordListItem("templatelist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                        itemInfo.SetXmlProperty("genxml/hidden/editorcodehtmlmixed", GeneralUtils.EnCode(d.Value));
                        AppTheme.Record.AddRecordListItem("templatelist", itemInfo);
                    }
                }
                foreach (var d in ModuelTemplateListCSS)
                {
                    var itemInfo = AppTheme.Record.GetRecordListItem("csslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                    if (itemInfo != null)
                    {
                        AppTheme.Record.RemoveRecordListItem("csslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                        itemInfo.SetXmlProperty("genxml/hidden/editorcodecss", GeneralUtils.EnCode(d.Value));
                        AppTheme.Record.AddRecordListItem("csslist", itemInfo);
                    }
                }
                foreach (var d in ModuelTemplateListRazor)
                {
                    var itemInfo = AppTheme.Record.GetRecordListItem("jslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                    if (itemInfo != null)
                    {
                        AppTheme.Record.RemoveRecordListItem("jslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                        itemInfo.SetXmlProperty("genxml/hidden/editorcodejavascript", GeneralUtils.EnCode(d.Value));
                        AppTheme.Record.AddRecordListItem("jslist", itemInfo);
                    }
                }
            }
        }
        public void SaveEditor(string filename, string fileExtension, string editorcode)
        {
            var folder = ModuleTemplateFolderRazorMapPath;
            if (fileExtension.Trim('.') == "css") folder = ModuleTemplateFolderCSSMapPath;
            if (fileExtension.Trim('.') == "js") folder = ModuleTemplateFolderJSMapPath;

            var formHtml = GeneralUtils.DeCode(editorcode);
            var templatefileMapPath = folder + "\\" + ModuleParams.ModuleRef + "_" + filename + "." + fileExtension.Trim('.');

            FileUtils.SaveFile(templatefileMapPath, formHtml);
        }
        public string GetTemplateRazor(string templatename)
        {
            var template = "";
            if (ModuelTemplateListRazor.ContainsKey(templatename))
            {
                template = ModuelTemplateListRazor[templatename];
            }
            return template;
        }
        public string GetTemplateCSS(string templatename)
        {
            var template = "";
            if (ModuelTemplateListCSS.ContainsKey(templatename))
            {
                template = ModuelTemplateListCSS[templatename];
            }
            return template;
        }
        public string GetTemplateJS(string templatename)
        {
            var template = "";
            if (ModuelTemplateListJS.ContainsKey(templatename))
            {
                template = ModuelTemplateListJS[templatename];
            }
            return template;
        }


        public ModuleParams ModuleParams { get; private set; }
        public AppTheme AppTheme { get; private set; }
        public SystemInfoData SystemInfoData { get; private set; }
        public int ModuleId { get; private set; }
        public int SystemId { get; private set; }
        public string ModuleTemplateFolderMapPath { get; private set; }
        public string ModuleTemplateFolderRazorMapPath { get { return ModuleTemplateFolderMapPath + "\\default"; } }
        public string ModuleTemplateFolderCSSMapPath { get { return ModuleTemplateFolderMapPath + "\\css"; } }
        public string ModuleTemplateFolderJSMapPath { get { return ModuleTemplateFolderMapPath + "\\js"; } }
        public Dictionary<string, string> ModuelTemplateListRazor { get; private set; }
        public Dictionary<string, string> ModuelTemplateListJS { get; private set; }
        public Dictionary<string, string> ModuelTemplateListCSS { get; private set; }
    }
}
