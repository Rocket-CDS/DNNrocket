using Simplisity;
using System.Collections.Generic;
using System.IO;

namespace DNNrocketAPI.Componants
{
    public class AppThemeModule
    {
        private string _systemKey;
        private Dictionary<string, bool> _modellevelTemplates;

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

            // get razor
            if (Directory.Exists(ModuleTemplateFolderRazorMapPath)) // does not exist on selection of Module AppTheme.
            {
                var flist = Directory.GetFiles(ModuleTemplateFolderRazorMapPath, ModuleParams.ModuleRef + "_*.cshtml");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    AppTheme.UpdateListFileName(fname, f);
                    _modellevelTemplates.Add(fname, true);
                }
                // get css
                flist = Directory.GetFiles(ModuleTemplateFolderCSSMapPath, ModuleParams.ModuleRef + "_*.css");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    AppTheme.UpdateListFileName(fname, f);
                    _modellevelTemplates.Add(fname, true);
                }
                // get js
                flist = Directory.GetFiles(ModuleTemplateFolderJSMapPath, ModuleParams.ModuleRef + "_*.js");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileNameWithoutExtension(f).Replace(ModuleParams.ModuleRef + "_", "");
                    AppTheme.UpdateListFileName(fname, f);
                    _modellevelTemplates.Add(fname, true);
                }
            }
        }

        public bool IsModuleLevelTemplate(string templatename)
        {
            return _modellevelTemplates.ContainsKey(templatename);
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
                var fMapPath = ModuleTemplateListJS[templatename];
                template = FileUtils.ReadFile(fMapPath);
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
