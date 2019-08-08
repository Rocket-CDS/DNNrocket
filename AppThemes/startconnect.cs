using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.AppThemes
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static AppThemeData _appThemeData;
        private static SimplisityInfo _systemInfo;
        private static string _editLang;
        private static SystemInfoData _systemInfoData;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR - Must be SuperUser"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _systemInfoData = new SystemInfoData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _postInfo = postInfo;

            _editLang = langRequired;
            if (_editLang == "") _editLang = DNNrocketUtils.GetEditCulture();

            var cacheKey = "appthemedata_" + DNNrocketUtils.GetCurrentUserId();
            _appThemeData = (AppThemeData)CacheUtils.GetCache(cacheKey);
            if (_appThemeData == null)
            {
                _appThemeData = new AppThemeData(DNNrocketUtils.GetCurrentUserId(), "/DesktopModules/DNNrocket/AppThemes", _editLang);
                _appThemeData.VersionFolder = "1.0";
            }

            if (DNNrocketUtils.IsSuperUser())
            {

                switch (paramCmd)
                {
                    case "rocketapptheme_dashboard":
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_selecttheme":
                        strOut = SelectAppTheme();
                        break;

                    case "rocketapptheme_addfield":
                        _appThemeData.AddFieldRow();
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_resxfield":
                        _appThemeData.AddResxRow();
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_save":                        
                        _appThemeData.AddDataInfo(_postInfo, _editLang);
                        _appThemeData.Save();
                        strOut = GetDisplay();
                        break;



                    case "rocketapptheme_editor":
                        strOut = GetEditor();
                        break;
                    case "rocketapptheme_editorcss":
                        strOut = GetEditorCSS();
                        break;
                    case "rocketapptheme_editorjs":
                        strOut = GetEditorJS();
                        break;
                    case "rocketapptheme_gettemplate":
                        strOut = GetEditorFile("default");
                        break;
                    case "rocketapptheme_getcss":
                        strOut = GetEditorFile("css");
                        break;
                    case "rocketapptheme_getjs":
                        strOut = GetEditorFile("js");
                        break;
                    case "rocketapptheme_savetemplate":
                        strOut = SaveTemplate();
                        break;
                    case "rocketapptheme_upload":
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_download":
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_actiontype":
                        strOut = GetActionType();
                        break;
                    case "rocketapptheme_savedetails":
                        SaveDetails();
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_deletetheme":
                        _appThemeData.DeleteTheme();
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_deleteversion":
                        _appThemeData.VersionFolder = _postInfo.GetXmlProperty("genxml/select/versionfolder");
                        _appThemeData.DeleteVersion();
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_selectculturecode":
                        strOut = CultureSelect();
                        break;
                    case "rocketapptheme_culturecodeselected":
                        strOut = CultureCodeSelected();
                        break;
                }
            }
            else
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }

            CacheUtils.SetCache(cacheKey, _appThemeData);

            return DNNrocketUtils.ReturnString(strOut);
        }


        public static String CultureSelect()
        {

            try
            {
                var template = _postInfo.GetXmlProperty("genxml/hidden/template");
                if (template == "") template = _rocketInterface.DefaultTemplate;

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(template, _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                
                var l = DNNrocketUtils.GetAllCultureCodeList();
                return DNNrocketUtils.RazorList(razorTempl, l, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String SaveTemplate()
        {
            try
            {
                var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                var filefolder = _postInfo.GetXmlProperty("genxml/hidden/filefolder");
                var editorContent = GeneralUtils.DeCode(_postInfo.GetXmlProperty("genxml/hidden/editorcode"));

                var fileMapPath = _appThemeData.AppThemeVersionFolderMapPath + "\\" + filefolder + "\\" + templateName;
                if (!Directory.Exists(_appThemeData.AppThemeVersionFolderMapPath + "\\" + filefolder))
                {
                    File.WriteAllText(fileMapPath, editorContent);
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetEditorFile(string subFolder)
        {
            try
            {
                var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                var filePath = _appThemeData.AppThemeVersionFolderMapPath + "\\" + subFolder + "\\" + templateName;
                if (!File.Exists(filePath)) return "";
                var rtnFile = FileUtils.ReadFile(filePath); 
                return GeneralUtils.EnCode(rtnFile);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetTemplate()
        {
            try
            {
                var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(templateName, _appThemeData.AppThemesRelPath, _appThemeData.AppName, DNNrocketUtils.GetCurrentCulture(), _appThemeData.VersionFolder);
                return GeneralUtils.EnCode(razorTempl);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetEditor()
        {
            try
            {
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editor.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), _appThemeData.VersionFolder,_systemInfoData.DebugMode);

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings,null, _systemInfoData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static String GetEditorCSS()
        {
            try
            {
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("EditorCSS.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static String GetEditorJS()
        {
            try
            {
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("EditorJS.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDisplay()
        {
            try
            {
                var template = _postInfo.GetXmlProperty("genxml/hidden/template");
                if (template == "") template = _rocketInterface.DefaultTemplate;
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(template, _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);
                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetActionType()
        {
            try
            {
                var strOut = "";
                _appThemeData.ActionType = _postInfo.GetXmlProperty("genxml/hidden/actiontype").ToLower();

                var objCtrl = new DNNrocketController();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeSelect.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                strOut = DNNrocketUtils.RazorList(razorTempl, _appThemeData.AppThemesList, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String SelectAppTheme()
        {
            try
            {
                /// When we have the AppName we can populate the theme.
                var strOut = "";
                var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                _appThemeData.AppName = appThemeName;
                _appThemeData.Populate();

                if (_appThemeData.ActionType == "version")
                {
                    var versionincrement = _postInfo.GetXmlPropertyDouble("genxml/hidden/versionincrement");
                    if (versionincrement <= 0) versionincrement = 1;
                    _appThemeData.CreateNewVersion(versionincrement);
                }

                strOut = GetDisplay();

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String CultureCodeSelected()
        {
            try
            {
                var cultureCode = _postInfo.GetXmlProperty("genxml/hidden/culturecode");
                _appThemeData.CultureCode = cultureCode;
                return ""; // reload to return to correct page. (s-reload='true')
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public static String GetAppNewName()
        {
            try
            {
                var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                _appThemeData.AppName = appThemeName;
                _appThemeData.PopulateVersionList();

                var objCtrl = new DNNrocketController();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static void SaveDetails()
        {
            _appThemeData.CultureCode = _postInfo.GetXmlProperty("genxml/hidden/appculturecode");
            _appThemeData.AppName  = _postInfo.GetXmlProperty("genxml/textbox/appname");
            _appThemeData.VersionFolder = _postInfo.GetXmlProperty("genxml/select/versionfolder");
        }

    }
}
