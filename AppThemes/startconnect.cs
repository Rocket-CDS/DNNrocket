using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.AppThemes
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static AppThemeData _appThemeData;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string langRequired = "")
        {
            var strOut = "ERROR - Must be SuperUser"; // return ERROR if not matching commands.

            if (DNNrocketUtils.IsSuperUser())
            {

                paramCmd = paramCmd.ToLower();

                _rocketInterface = new DNNrocketInterface(interfaceInfo);
                _postInfo = postInfo;
                _appThemeData = new AppThemeData(DNNrocketUtils.GetCurrentUserId(), "/DesktopModules/DNNrocket/AppThemes", langRequired);


                switch (paramCmd)
                {
                    case "rocketapptheme_dashboard":
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_editor":
                        strOut = GetEditor();
                        break;
                    case "rocketapptheme_gettemplate":
                        strOut = GetTemplate();
                        break;
                    case "rocketapptheme_save":
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
                    case "rocketapptheme_appversions":
                        strOut = GetAppVersions();
                        break;
                    case "rocketapptheme_saveversions":
                        var version = _postInfo.GetXmlProperty("genxml/hidden/version");
                        _appThemeData.SelectedVersion = version;
                        if (_appThemeData.ActionType == "new")
                        {
                            strOut = GetAppNewName();
                        }
                        else
                        {
                            strOut = GetDisplay();
                        }
                        break;
                    case "rocketapptheme_savename":
                        var newname = _postInfo.GetXmlProperty("genxml/textbox/name");
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_deleteconfig":
                        _appThemeData.Delete();
                        strOut = GetDisplay();
                        break;
                }
            }
            else
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }

            return ReturnString(strOut);
        }

        public static Dictionary<string, string> ReturnString(string strOut, string jsonOut = "")
        {
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            rtnDic.Add("outputjson", jsonOut);            
            return rtnDic;
        }

        public static String SaveTemplate()
        {
            try
            {
                var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                var appthemeRelPath = _postInfo.GetXmlProperty("genxml/hidden/systemrelpath");
                var appthemeversion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");
                var apptheme = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                var themelevel = _postInfo.GetXmlProperty("genxml/hidden/themelevel"); // system, portal, module
                var moduleref = _postInfo.GetXmlProperty("genxml/hidden/moduleref");

                var editorContent = GeneralUtils.DeCode(_postInfo.GetXmlProperty("genxml/hidden/editorcode"));

                var themeFolderPath = "Themes\\" + apptheme + "\\" + appthemeversion + "\\default";
                var controlMapPath = (DNNrocketUtils.DNNrocketThemesDirectory() + "\\" + themeFolderPath).TrimEnd('\\'); 
                if (!Directory.Exists(controlMapPath)) Directory.CreateDirectory(controlMapPath);

                if (themelevel.ToLower() == "system")
                {
                    controlMapPath = (appthemeRelPath.TrimEnd('\\') + "\\" + themeFolderPath).TrimEnd('\\');
                }

                if (themelevel.ToLower() == "portal")
                {
                    controlMapPath = DNNrocketUtils.DNNrocketThemesDirectory() + "\\" + themeFolderPath.TrimEnd('\\');
                }
                var fileMapPath = controlMapPath + "\\" + templateName;

                if (themelevel.ToLower() == "module")
                {
                    fileMapPath = controlMapPath + "\\" + moduleref + "_" + templateName;
                }

                if (!Directory.Exists(controlMapPath)) Directory.CreateDirectory(controlMapPath);

                File.WriteAllText(fileMapPath,editorContent);

                return "OK";
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
                var appthemeRelPath = _postInfo.GetXmlProperty("genxml/hidden/systemrelpath");
                var appthemeversion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");
                var apptheme = _postInfo.GetXmlProperty("genxml/hidden/apptheme");

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(templateName, appthemeRelPath, apptheme, DNNrocketUtils.GetCurrentCulture(), appthemeversion);

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
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editor.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData.Info, passSettings);
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
                _appThemeData.Save();

                var objCtrl = new DNNrocketController();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeSelect.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                strOut = DNNrocketUtils.RazorList(razorTempl, _appThemeData.List, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String GetAppVersions()
        {
            try
            {
                var strOut = "";
                var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                _appThemeData.SelectedTheme = appThemeName;
                _appThemeData.PopulateVersionList();
                _appThemeData.Save();

                var objCtrl = new DNNrocketController();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppVersionSelect.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                strOut = DNNrocketUtils.RazorList(razorTempl, _appThemeData.VersionList, passSettings);

                return strOut;
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
                var strOut = "";
                var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                _appThemeData.SelectedTheme = appThemeName;
                _appThemeData.PopulateVersionList();
                _appThemeData.Save();

                var objCtrl = new DNNrocketController();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeName.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var passSettings = _postInfo.ToDictionary();
                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


    }
}
