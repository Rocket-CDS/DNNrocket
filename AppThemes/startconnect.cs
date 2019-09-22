using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.AppThemes
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private static SimplisityInfo _postInfo;
        private static SimplisityInfo _paramInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static AppThemeDataList _appThemeDataList;
        private static SimplisityInfo _systemInfo;
        private static string _editLang;
        private static SystemInfoData _systemInfoData;
        private static Dictionary<string, string> _passSettings;
        private static string _appThemeFolder;
        private static string _appVersionFolder;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR - Must be SuperUser"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _passSettings = new Dictionary<string, string>(); 
            _systemInfoData = new SystemInfoData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _editLang = langRequired;
            if (_editLang == "") _editLang = DNNrocketUtils.GetEditCulture();

            if (DNNrocketUtils.IsSuperUser())
            {
                _appThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
                _appVersionFolder = _paramInfo.GetXmlProperty("genxml/hidden/appversionfolder");

                _appThemeDataList = new AppThemeDataList(postInfo.GetXmlProperty("genxml/hidden/selectedsystemkey"));

                if (paramCmd != "rocketapptheme_selectsystemkey" && _appThemeDataList.SelectedSystemKey == "") paramCmd = "rocketapptheme_getlist";

                switch (paramCmd)
                {
                    case "rocketapptheme_getlist":
                        strOut = GetList();
                        break;
                    case "rocketapptheme_getdetail":
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_selectsystemkey":
                        strOut = SetSystemKey();
                        break;
                    case "rocketapptheme_clearsystemkey":
                        strOut = ClearSystemKey();
                        break;
                    case "rocketapptheme_save":
                        SaveData();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addimage":
                        AddListImage();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addtemplate":
                        SaveData();
                        strOut = AddListTemplate();
                        break;
                    case "rocketapptheme_addcss":
                        SaveData();
                        strOut = AddListCss();
                        break;
                    case "rocketapptheme_addjs":
                        SaveData();
                        strOut = AddListJs();
                        break;
                    case "rocketapptheme_addresx":
                        SaveData();
                        strOut = AddListResx();
                        break;
                    case "rocketapptheme_addfield":
                        SaveData();
                        strOut = AddListField();
                        break;
                    case "rocketapptheme_createversion":
                        strOut = CreateNewVersion();
                        break;
                    case "rocketapptheme_changeversion":
                        _appVersionFolder = _postInfo.GetXmlProperty("genxml/select/versionfolder");
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_deleteversion":
                        strOut = DeleteVersion();
                        break;
                    case "rocketapptheme_deletetheme":
                        strOut = DeleteTheme();
                        break;
                    case "rocketapptheme_addapptheme":
                        strOut = AddNewAppTheme();
                        break;
                }
            }
            else
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }

            return DNNrocketUtils.ReturnString(strOut);
        }


        public static string CreateNewVersion()
        {
            try
            {
                AssignEditLang();
                var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, "", true);
                var rtn = appTheme.CopyVersion(appTheme.AppVersionFolder, (Convert.ToDouble(appTheme.LatestVersionFolder) + 1).ToString("0.0"));
                if (rtn != "") return rtn;
                return GetDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static string DeleteVersion()
        {
            try
            {
                AssignEditLang();
                var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, "", true);
                appTheme.DeleteVersion();
                _appVersionFolder = appTheme.AppVersionFolder;
                _appThemeDataList = new AppThemeDataList(_appThemeDataList.SelectedSystemKey); // rebuild list without verison.
                return GetDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static string DeleteTheme()
        {
            try
            {
                AssignEditLang();
                var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, "", true);
                appTheme.DeleteTheme();
                _appThemeDataList = new AppThemeDataList(_appThemeDataList.SelectedSystemKey);
                return GetList();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string AddNewAppTheme()
        {
            try
            {
                var appthemeprefix = _postInfo.GetXmlProperty("genxml/textbox/appthemeprefix");
                var appthemename = _postInfo.GetXmlProperty("genxml/textbox/appthemename");

                var newAppThemeName = appthemename;
                if (appthemeprefix != "") newAppThemeName = appthemeprefix + "_" + newAppThemeName;

                var appProjectFolderRel = "/DesktopModules/DNNrocket/AppThemes";
                var appSystemThemeFolderRel = appProjectFolderRel + "/SystemThemes/" + _appThemeDataList.SelectedSystemKey;
                var appThemeFolderRel = appSystemThemeFolderRel + "/" + newAppThemeName;
                var appThemeFolderMapPath = DNNrocketUtils.MapPath(appThemeFolderRel);

                if (Directory.Exists(appThemeFolderMapPath))
                {
                    return DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/AppThemes/App_LocalResources/", "AppThemes.appthemeexists");
                }

                AssignEditLang();
                var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, newAppThemeName, "1.0", "", true);
                _appVersionFolder = appTheme.AppVersionFolder;
                _appThemeDataList.PopulateAppThemeList();
                return GetList();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDetail()
        {
            try
            {
                AssignEditLang();
                var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, "", true);
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings,null,true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static String SetSystemKey()
        {
            _appThemeDataList.SelectedSystemKey = _paramInfo.GetXmlProperty("genxml/hidden/systemkey");

            return GetList();
        }
        public static String ClearSystemKey()
        {
            _appThemeDataList.ClearCache();
            return GetList();
        }
        public static String GetList()
        {
            try
            {
                AssignEditLang();
                var template = _rocketInterface.DefaultTemplate;
                if (template == "") template = "appthemelist.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(template, _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(),"1.0",true);
                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppProjectThemesFolderMapPath", _appThemeDataList.AppProjectThemesFolderMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeDataList, passSettings,null,true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void SaveData()
        {
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);
            appTheme.Save(_postInfo);
        }

        public static void AddListImage()
        {
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);
            appTheme.AddListImage();
        }

        public static string AddListTemplate()
        {
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);
            appTheme.AddListTemplate();
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), appTheme.AppVersionFolder, true);
            return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }
        public static string AddListCss()
        {
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);
            appTheme.AddListCss();
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), appTheme.AppVersionFolder, true);
            return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }
        public static string AddListJs()
        {
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);
            appTheme.AddListJs();
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), appTheme.AppVersionFolder, true);
            return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }
        public static string AddListResx()
        {
            var culturecode = _paramInfo.GetXmlProperty("genxml/hidden/resxculturecode");
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);

            // get default data
            var jsonResx = "";
            var resxItem = appTheme.Info.GetListItem("resxlist", "genxml/hidden/culturecode", "");
            if (resxItem != null) jsonResx = resxItem.GetXmlProperty("genxml/hidden/jsonresx");

            appTheme.AddListResx(culturecode, jsonResx);
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), appTheme.AppVersionFolder, true);
            return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }
        private static string AddListField()
        {
            var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, _editLang, true);
            appTheme.AddListField();
            var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), appTheme.AppVersionFolder, true);
            return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }
        private static void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------------



        public static String GetEditorFile(string subFolder)
        {
            try
            {
                //var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                //var filePath = _appThemeData.AppThemeVersionFolderMapPath + "\\" + subFolder + "\\" + templateName;
                //if (!File.Exists(filePath)) return "";
                //var rtnFile = FileUtils.ReadFile(filePath); 
                //return GeneralUtils.EnCode(rtnFile);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }


        public static String GetTemplate()
        {
            try
            {
                //var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData(templateName, _appThemeData.AppThemesRelPath, _appThemeData.AppName, DNNrocketUtils.GetCurrentCulture(), _appThemeData.VersionFolder);
                //return GeneralUtils.EnCode(razorTempl);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }


        public static String GetEditor()
        {
            try
            {
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData("editor.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), _appThemeData.VersionFolder,_systemInfoData.DebugMode);

                //var passSettings = _postInfo.ToDictionary();
                //passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                //return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings,null, _systemInfoData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }
        public static String GetEditorCSS()
        {
            try
            {
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData("EditorCSS.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                //var passSettings = _postInfo.ToDictionary();
                //passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                //return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }
        public static String GetEditorJS()
        {
            try
            {
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData("EditorJS.cshtml", _appThemeData.AdminAppThemesRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                //var passSettings = _postInfo.ToDictionary();
                //passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);

                //return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public static String GetDisplay()
        {
            try
            {
                //var template = _postInfo.GetXmlProperty("genxml/hidden/template");
                //if (template == "") template = _rocketInterface.DefaultTemplate;
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData(template, _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                //var passSettings = _postInfo.ToDictionary();
                //passSettings.Add("AppThemesMapPath", _appThemeData.AppThemesMapPath);
                //return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public static String GetActionType()
        {
            try
            {
                var strOut = "";
                //_appThemeData.ActionType = _postInfo.GetXmlProperty("genxml/hidden/actiontype").ToLower();

                //var objCtrl = new DNNrocketController();
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeSelect.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                //var passSettings = _postInfo.ToDictionary();
                //strOut = DNNrocketUtils.RazorList(razorTempl, _appThemeData.AppThemesList, passSettings);

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
                //var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                //_appThemeData.AppName = appThemeName;
                //_appThemeData.Populate();

                //if (_appThemeData.ActionType == "version")
                //{
                //    var versionincrement = _postInfo.GetXmlPropertyDouble("genxml/hidden/versionincrement");
                //    if (versionincrement <= 0) versionincrement = 1;
                //    _appThemeData.CreateNewVersion(versionincrement);
                //}

                //strOut = GetDisplay();

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
                //var cultureCode = _postInfo.GetXmlProperty("genxml/hidden/culturecode");
                //_appThemeData.CultureCode = cultureCode;
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
                //var appThemeName = _postInfo.GetXmlProperty("genxml/hidden/apptheme");
                //_appThemeData.AppName = appThemeName;
                //_appThemeData.PopulateVersionList();

                //var objCtrl = new DNNrocketController();
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _appThemeData.AdminAppThemesRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                //var passSettings = _postInfo.ToDictionary();
                //return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public static void SaveDetails()
        {
            //_appThemeData.CultureCode = _postInfo.GetXmlProperty("genxml/hidden/appculturecode");
            //_appThemeData.AppName  = _postInfo.GetXmlProperty("genxml/textbox/appname");
            //_appThemeData.VersionFolder = _postInfo.GetXmlProperty("genxml/select/versionfolder");
        }

    }
}
