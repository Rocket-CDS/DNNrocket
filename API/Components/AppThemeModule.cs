using Simplisity;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DNNrocketAPI.Components
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
        }

        private void InitAppThemeMod() {

            ModuleParams = new ModuleParams(ModuleId, _systemKey);
            systemData = new SystemLimpet(_systemKey);

            ModuleTemplateFolderMapPath = PortalUtils.HomeDirectoryMapPath().Trim('\\') + "\\DNNrocketThemes";
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + ModuleParams.AppTheme.SystemKey;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + ModuleParams.AppTheme.AppThemeFolder;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            ModuleTemplateFolderMapPath += "\\" + ModuleParams.AppTheme.AppVersionFolder;
            if (!Directory.Exists(ModuleTemplateFolderMapPath)) Directory.CreateDirectory(ModuleTemplateFolderMapPath);
            if (!Directory.Exists(ModuleTemplateFolderRazorMapPath)) Directory.CreateDirectory(ModuleTemplateFolderRazorMapPath);
            if (!Directory.Exists(ModuleTemplateFolderCSSMapPath)) Directory.CreateDirectory(ModuleTemplateFolderCSSMapPath);
            if (!Directory.Exists(ModuleTemplateFolderJSMapPath)) Directory.CreateDirectory(ModuleTemplateFolderJSMapPath);

            LoadTemplates();

        }

        private void LoadTemplates()
        {

            // get razor
            if (Directory.Exists(ModuleTemplateFolderRazorMapPath)) // does not exist on selection of Module AppThemeLimpet.
            {
                var flist = Directory.GetFiles(ModuleTemplateFolderRazorMapPath, ModuleParams.ModuleRef + "_*.cshtml");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileName(f).Replace(ModuleParams.ModuleRef + "_", "");
                    //ModuleParams.AppTheme.UpdateListFileName(fname, f);
                    _modellevelTemplates.Remove(fname);
                    _modellevelTemplates.Add(fname, true);
                }
                // get css
                flist = Directory.GetFiles(ModuleTemplateFolderCSSMapPath, ModuleParams.ModuleRef + "_*.css");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileName(f).Replace(ModuleParams.ModuleRef + "_", "");
                    //ModuleParams.AppTheme.UpdateListFileName(fname, f);
                    _modellevelTemplates.Remove(fname);
                    _modellevelTemplates.Add(fname, true);
                }
                // get js
                flist = Directory.GetFiles(ModuleTemplateFolderJSMapPath, ModuleParams.ModuleRef + "_*.js");
                foreach (var f in flist)
                {
                    var fname = Path.GetFileName(f).Replace(ModuleParams.ModuleRef + "_", "");
                    //ModuleParams.AppTheme.UpdateListFileName(fname, f);
                    _modellevelTemplates.Remove(fname);
                    _modellevelTemplates.Add(fname, true);
                }
            }
        }

        public string ExportModuleLevelTemplateXml()
        {
            var exportXml = "";
            if (_modellevelTemplates.Count > 0)
            {
                exportXml += "<templates moduleid='" + ModuleId + "'>";
                foreach (var templateDict in ModuleParams.AppTheme.FileNameList)
                {
                    if (IsModuleLevelTemplate(templateDict.Key))
                    {
                        var sRec2 = new SimplisityRecord();
                        var templateText = FileUtils.ReadFile(templateDict.Value);
                        sRec2.SetXmlProperty("template/name", templateDict.Key);
                        sRec2.SetXmlProperty("template/mappath", templateDict.Value);
                        sRec2.SetXmlProperty("template/text", GeneralUtils.EnCode(templateText));
                        exportXml += sRec2.ToXmlItem();
                    }
                }
                exportXml += "</templates>";
            }
            return exportXml;
        }
        public void ImportModuleLevelTemplateXml(string importXml)
        {
            var sRec = new SimplisityRecord();
            sRec.XMLData = importXml;
            var nodList = sRec.XMLDoc.SelectNodes("templates/*");
            foreach (XmlNode nod in nodList)
            {
                var sRec2 = new SimplisityRecord();
                sRec2.FromXmlItem(nod.OuterXml);
                var mappath = sRec2.GetXmlProperty("template/mappath");
                var text = sRec2.GetXmlProperty("template/text");
                if (text != "")
                {
                    FileUtils.SaveFile(mappath, GeneralUtils.DeCode(text));
                }
            }

        }

        public bool IsModuleLevelTemplate(string templatename)
        {
            return _modellevelTemplates.ContainsKey(templatename);
        }

        public void SaveEditor(string filename, string editorcode)
        {
            var folder = ModuleTemplateFolderRazorMapPath;
            if (Path.GetExtension(filename) == ".css") folder = ModuleTemplateFolderCSSMapPath;
            if (Path.GetExtension(filename) == ".js") folder = ModuleTemplateFolderJSMapPath;

            var formHtml = GeneralUtils.DeCode(editorcode);
            var templatefileMapPath = folder + "\\" + ModuleParams.ModuleRef + "_" + filename;

            FileUtils.SaveFile(templatefileMapPath, formHtml);
            InitAppThemeMod();
        }
        public string GetTemplateRazor(string templatename)
        {
            return ModuleParams.AppTheme.GetTemplate(templatename);
        }
        public void RemoveModuleTemplate(string filename)
        {
            var folder = ModuleTemplateFolderRazorMapPath;
            if (Path.GetExtension(filename) == ".css") folder = ModuleTemplateFolderCSSMapPath;
            if (Path.GetExtension(filename) == ".js") folder = ModuleTemplateFolderJSMapPath;

            var templatefileMapPath = folder + "\\" + ModuleParams.ModuleRef + "_" + filename;
            if (File.Exists(templatefileMapPath)) File.Delete(templatefileMapPath);
            InitAppThemeMod();
        }

        public ModuleParams ModuleParams { get; private set; }
        public SystemLimpet systemData { get; private set; }
        public int ModuleId { get; private set; }
        public int SystemId { get; private set; }
        public string ModuleTemplateFolderMapPath { get; private set; }
        public string ModuleTemplateFolderRazorMapPath { get { return ModuleTemplateFolderMapPath + "\\default"; } }
        public string ModuleTemplateFolderCSSMapPath { get { return ModuleTemplateFolderMapPath + "\\css"; } }
        public string ModuleTemplateFolderJSMapPath { get { return ModuleTemplateFolderMapPath + "\\js"; } }
    }
}
