using DNNrocketAPI;
using DNNrocketAPI.Componants;
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
        private Dictionary<string, string> _templatesRazor;
        private Dictionary<string, string> _templatesCSS;
        private Dictionary<string, string> _templatesJS;

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

            ModuleTemplateFolderMapPath = DNNrocketUtils.HomeDirectoryMapPath() + "\\DNNrocketThemes";
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + AppTheme.AppThemeFolder;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + AppTheme.AppVersionFolder;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            if (!Directory.Exists(ModuleTemplateFolderRazorMapPath)) Directory.CreateDirectory(ModuleTemplateFolderRazorMapPath);
            if (!Directory.Exists(ModuleTemplateFolderCSSMapPath)) Directory.CreateDirectory(ModuleTemplateFolderCSSMapPath);
            if (!Directory.Exists(ModuleTemplateFolderJSMapPath)) Directory.CreateDirectory(ModuleTemplateFolderJSMapPath);

            AppTheme = new AppTheme(SystemInfoData.SystemKey, ModuleParams.AppThemeFolder, ModuleParams.AppThemeVersion, _debugMode);
            
        }

        private void LoadTemplates()
        {
            _templatesRazor = new Dictionary<string, string>();
            _templatesCSS = new Dictionary<string, string>();
            _templatesJS = new Dictionary<string, string>();

            // get razor
            var flist = Directory.GetFiles(ModuleTemplateFolderRazorMapPath, ModuleParams.ModuleRef + "_*.cshtml");
            foreach (var f in flist)
            {
                var fname = Path.GetFileName(f);
                var ftext = FileUtils.ReadFile(f);
                _templatesRazor.Add(fname, ftext);
            }
            // get css
            flist = Directory.GetFiles(ModuleTemplateFolderCSSMapPath, ModuleParams.ModuleRef + "_*.css");
            foreach (var f in flist)
            {
                var fname = Path.GetFileName(f);
                var ftext = FileUtils.ReadFile(f);
                _templatesRazor.Add(fname, ftext);
            }
            // get js
            flist = Directory.GetFiles(ModuleTemplateFolderJSMapPath, ModuleParams.ModuleRef + "_*.js");
            foreach (var f in flist)
            {
                var fname = Path.GetFileName(f);
                var ftext = FileUtils.ReadFile(f);
                _templatesRazor.Add(fname, ftext);
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


        public ModuleParams ModuleParams { get; private set; }
        public AppTheme AppTheme { get; private set; }
        public SystemInfoData SystemInfoData { get; private set; }
        public int ModuleId { get; private set; }
        public int SystemId { get; private set; }
        public string ModuleTemplateFolderMapPath { get; private set; }
        public string ModuleTemplateFolderRazorMapPath { get { return ModuleTemplateFolderMapPath + "\\default"; } }
        public string ModuleTemplateFolderCSSMapPath { get { return ModuleTemplateFolderMapPath + "\\css"; } }
        public string ModuleTemplateFolderJSMapPath { get { return ModuleTemplateFolderMapPath + "\\js"; } }

    }
}
