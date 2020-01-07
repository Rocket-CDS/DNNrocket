using Simplisity;
using System.Collections.Generic;
using System.IO;

namespace DNNrocketAPI.Componants
{
    public class AppThemeModule
    {
        private string _systemKey;
        private Dictionary<string,bool> _modellevelTemplates;

        public AppThemeModule(int moduleId, string systemKey)
        {
            ModuleId = moduleId;
            _systemKey = systemKey;

            _modellevelTemplates = new Dictionary<string, bool>();

            InitAppThemeMod();
            LoadTemplates();
        }

        private void InitAppThemeMod() {

            ModuleParams = new ModuleParams(ModuleId, _systemKey);
            systemData = new SystemData(_systemKey);
            AppTheme = new AppTheme(systemData.SystemKey, ModuleParams.AppThemeFolder, ModuleParams.AppThemeVersion);

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
            ModuleTemplateListRazor = AppTheme.GetTemplateDictionaryRazor();
            ModuleTemplateListJS = AppTheme.GetTemplateDictionaryJS();
            ModuleTemplateListCSS = AppTheme.GetTemplateDictionaryCSS();

            // get razor
            if (ModuleTemplateListRazor != null) // does not exist on selection of Module AppTheme.
            {
                var flist = Directory.GetFiles(ModuleTemplateFolderRazorMapPath, ModuleParams.ModuleRef + "_*.cshtml");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    var ftext = FileUtils.ReadFile(f);
                    if (ModuleTemplateListRazor.ContainsKey(fname)) ModuleTemplateListRazor.Remove(fname);
                    ModuleTemplateListRazor.Add(fname, ftext);
                    _modellevelTemplates.Add("razor_" + fname, true);
                }
                // get css
                flist = Directory.GetFiles(ModuleTemplateFolderCSSMapPath, ModuleParams.ModuleRef + "_*.css");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    var ftext = FileUtils.ReadFile(f);
                    if (ModuleTemplateListCSS.ContainsKey(fname)) ModuleTemplateListCSS.Remove(fname);
                    ModuleTemplateListCSS.Add(fname, ftext);
                    _modellevelTemplates.Add("css_" + fname, true);
                }
                // get js
                flist = Directory.GetFiles(ModuleTemplateFolderJSMapPath, ModuleParams.ModuleRef + "_*.js");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    var ftext = FileUtils.ReadFile(f);
                    if (ModuleTemplateListJS.ContainsKey(fname)) ModuleTemplateListJS.Remove(fname);
                    ModuleTemplateListJS.Add(fname, ftext);
                    _modellevelTemplates.Add("js_" + fname, true);
                }

                foreach (var d in ModuleTemplateListRazor)
                {
                    var itemInfo = AppTheme.Record.GetRecordListItem("templatelist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                    if (itemInfo != null)
                    {
                        AppTheme.Record.RemoveRecordListItem("templatelist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                        itemInfo.SetXmlProperty("genxml/hidden/editorcodehtmlmixed", GeneralUtils.EnCode(d.Value));
                        if (_modellevelTemplates.ContainsKey("razor_" + Path.GetFileNameWithoutExtension(d.Key)))
                        {
                            itemInfo.SetXmlProperty("genxml/hidden/moduletemplate", "True");
                        }
                        AppTheme.Record.AddRecordListItem("templatelist", itemInfo);
                    }
                }
                foreach (var d in ModuleTemplateListCSS)
                {
                    var itemInfo = AppTheme.Record.GetRecordListItem("csslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                    if (itemInfo != null)
                    {
                        AppTheme.Record.RemoveRecordListItem("csslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                        itemInfo.SetXmlProperty("genxml/hidden/editorcodecss", GeneralUtils.EnCode(d.Value));
                        if (_modellevelTemplates.ContainsKey("css_" + Path.GetFileNameWithoutExtension(d.Key)))
                        {
                            itemInfo.SetXmlProperty("genxml/hidden/moduletemplate", "True");
                        }
                        AppTheme.Record.AddRecordListItem("csslist", itemInfo);
                    }
                }
                foreach (var d in ModuleTemplateListJS)
                {
                    var itemInfo = AppTheme.Record.GetRecordListItem("jslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                    if (itemInfo != null)
                    {
                        AppTheme.Record.RemoveRecordListItem("jslist", "genxml/hidden/filename", Path.GetFileNameWithoutExtension(d.Key));
                        itemInfo.SetXmlProperty("genxml/hidden/editorcodejavascript", GeneralUtils.EnCode(d.Value));
                        if (_modellevelTemplates.ContainsKey("js_" + Path.GetFileNameWithoutExtension(d.Key)))
                        {
                            itemInfo.SetXmlProperty("genxml/hidden/moduletemplate", "True");
                        }
                        AppTheme.Record.AddRecordListItem("jslist", itemInfo);
                    }
                }
            }
        }

        public bool IsModuleLevelTemplate(string templatename)
        {
            templatename = templatename.ToLower();
            if (_modellevelTemplates.ContainsKey("razor_" + Path.GetFileNameWithoutExtension(templatename))) return true;
            if (_modellevelTemplates.ContainsKey("css_" + Path.GetFileNameWithoutExtension(templatename))) return true;
            if (_modellevelTemplates.ContainsKey("js_" + Path.GetFileNameWithoutExtension(templatename))) return true;
            return false;
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
            if (ModuleTemplateListRazor.ContainsKey(templatename))
            {
                template = ModuleTemplateListRazor[templatename];
            }
            return template;
        }
        public void RemoveModuleTemplate(string filename, string fileExtension)
        {
            var folder = ModuleTemplateFolderRazorMapPath;
            if (fileExtension.Trim('.') == "css") folder = ModuleTemplateFolderCSSMapPath;
            if (fileExtension.Trim('.') == "js") folder = ModuleTemplateFolderJSMapPath;

            var templatefileMapPath = folder + "\\" + ModuleParams.ModuleRef + "_" + filename + "." + fileExtension.Trim('.');
            File.Delete(templatefileMapPath);
            LoadTemplates();
        }
        public string GetTemplateCSS(string templatename)
        {
            var template = "";
            if (ModuleTemplateListCSS.ContainsKey(templatename))
            {
                template = ModuleTemplateListCSS[templatename];
            }
            return template;
        }
        public string GetTemplateJS(string templatename)
        {
            var template = "";
            if (ModuleTemplateListJS.ContainsKey(templatename))
            {
                template = ModuleTemplateListJS[templatename];
            }
            return template;
        }


        public ModuleParams ModuleParams { get; private set; }
        public AppTheme AppTheme { get; private set; }
        public SystemData systemData { get; private set; }
        public int ModuleId { get; private set; }
        public int SystemId { get; private set; }
        public string ModuleTemplateFolderMapPath { get; private set; }
        public string ModuleTemplateFolderRazorMapPath { get { return ModuleTemplateFolderMapPath + "\\default"; } }
        public string ModuleTemplateFolderCSSMapPath { get { return ModuleTemplateFolderMapPath + "\\css"; } }
        public string ModuleTemplateFolderJSMapPath { get { return ModuleTemplateFolderMapPath + "\\js"; } }
        public Dictionary<string, string> ModuleTemplateListRazor { get; private set; }
        public Dictionary<string, string> ModuleTemplateListJS { get; private set; }
        public Dictionary<string, string> ModuleTemplateListCSS { get; private set; }
    }
}
