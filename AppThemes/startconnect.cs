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

            paramCmd = paramCmd.ToLower();
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _postInfo = postInfo;
            _appThemeData = new AppThemeData(DNNrocketUtils.GetCurrentUserId(), "/DesktopModules/DNNrocket/AppThemes", langRequired);

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
                    case "rocketapptheme_savedetails":
                        SaveDetails();
                        strOut = GetDisplay();
                        break;
                    case "rocketapptheme_deleteconfig":
                        _appThemeData.DeleteConfig();
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

            return ReturnString(strOut);
        }

        public static Dictionary<string, string> ReturnString(string strOut, string jsonOut = "")
        {
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            rtnDic.Add("outputjson", jsonOut);            
            return rtnDic;
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
                var fileMapPath = _appThemeData.AppTheme.AppThemeVersionFolderMapPath + "\\" + filefolder + "\\" + templateName;
                if (!Directory.Exists(_appThemeData.AppTheme.AppThemeVersionFolderMapPath + "\\" + filefolder))
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

        public static String GetTemplate()
        {
            try
            {
                var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(templateName, _appThemeData.AppTheme.AppFolder, _appThemeData.AppTheme.AppName, DNNrocketUtils.GetCurrentCulture(), _appThemeData.AppTheme.AppVersionFolder);
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

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData.AppTheme, passSettings);
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

        public static String SelectAppTheme()
        {
            try
            {
                var strOut = "";
                var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                _appThemeData.AppName = appThemeName;
                _appThemeData.PopulateVersionList();
                _appThemeData.VersionFolder = _appThemeData.LatestVersionFolder;
                _appThemeData.Save();

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
                var strOut = "";
                var cultureCode = _postInfo.GetXmlProperty("genxml/hidden/culturecode");
                _appThemeData.AppCultureCode = cultureCode;
                _appThemeData.Save();

                strOut = GetDisplay();

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
                _appThemeData.AppName = appThemeName;
                _appThemeData.PopulateVersionList();
                _appThemeData.Save();

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
            _appThemeData.AppCultureCode = _postInfo.GetXmlProperty("genxml/hidden/appculturecode");
            _appThemeData.AppName  = _postInfo.GetXmlProperty("genxml/textbox/appname");
            _appThemeData.AppTheme.DisplayName = _postInfo.GetXmlProperty("genxml/lang/genxml/textbox/displayname");
            _appThemeData.AppTheme.Summary = _postInfo.GetXmlProperty("genxml/lang/genxml/textbox/summary");
            _appThemeData.VersionFolder = _postInfo.GetXmlProperty("genxml/select/versionfolder");
            _appThemeData.Save();
        }

    }
}
