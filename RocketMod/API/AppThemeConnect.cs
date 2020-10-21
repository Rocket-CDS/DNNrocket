using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketMod
{
    public partial class StartConnect
    {


        public String GetSelectApp()
        {

            try
            {
                var strOut = "";
                if (!_moduleParams.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    _appThemeDataList = new AppThemeDataList(_systemData.SystemKey);

                    var razortemplate = "selectapp.cshtml";
                    var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                    var passSettings = _postInfo.ToDictionary();
                    if (!passSettings.ContainsKey("moduleid")) passSettings.Add("moduleid", _moduleParams.ModuleId.ToString());

                    return RenderRazorUtils.RazorDetail(razorTempl, _appThemeDataList, passSettings, null, true);
                }
                else
                {
                    //strOut = EditData();
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        private void SaveAppTheme()
        {
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.ModuleType = "RocketMod";
            _moduleParams.ImageFolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            _moduleParams.DocumentFolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            _moduleParams.ShareData = "1";
            _moduleParams.TabId = _tabid;
            _moduleParams.Save();
            CacheFileUtils.ClearAllCache();
        }

        public String GetAppModTheme()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "apptheme.cshtml";
                var appThemeMod = new AppThemeModule(_moduleid, _systemData.SystemKey);

                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                return RenderRazorUtils.RazorDetail(razorTempl, appThemeMod, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string SaveEditor()
        {
            DoTemplateBackUp(); // backup before we save

            var editorcode = _postInfo.GetXmlProperty("genxml/hidden/editorcodesave");
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            _appThemeMod.SaveEditor(filename, editorcode);
            CacheFileUtils.ClearAllCache();
            return "OK";
        }

        public string RemoveTemplate()
        {
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            _appThemeMod.RemoveModuleTemplate(filename);
            CacheFileUtils.ClearAllCache();
            return GetAppModTheme();
        }
        public String GetEditorDetail()
        {
            try
            {
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var jsonString = GeneralUtils.EnCode(_appThemeMod.ModuleParams.AppTheme.GetTemplate(fname));
                _passSettings.Add("filename", fname);
                _passSettings.Add("jsonFileData", jsonString);

                var editormode = "htmlmixed";
                if (Path.GetExtension(fname) == ".js") editormode = "javascript";
                if (Path.GetExtension(fname) == ".css") editormode = "css";
                _passSettings.Add("editormode", editormode);

                _passSettings.Add("interfacekey", _rocketInterface.InterfaceKey);
                _passSettings.Add("moduleref", _moduleParams.ModuleRef);

                if (_appThemeMod.IsModuleLevelTemplate(fname)) _passSettings.Add("moduletemplatebutton", "True");

                var razorTempl = RenderRazorUtils.GetRazorTemplateData("EditorPopUp.cshtml", "/DesktopModules/DNNrocket/AppThemes", "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorObjectRender(razorTempl, _appThemeMod.ModuleParams.AppTheme, null, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


    }
}
