using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
        private const string _tableName = "DNNRocket";
        private static AppTheme _appTheme;

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
                if (_paramInfo.GetXmlPropertyBool("genxml/hidden/clearlistcache")) _appThemeDataList.PopulateAppThemeList();

                if (paramCmd != "rocketapptheme_selectsystemkey" && _appThemeDataList.SelectedSystemKey == "") paramCmd = "rocketapptheme_getlist";

                _appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, _appThemeFolder, _appVersionFolder, true);
                AssignEditLang();

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
                    case "rocketapptheme_export":
                        return ExportAppTheme();
                    case "rocketapptheme_import":
                        strOut = ImportAppTheme();
                        break;
                    case "rocketapptheme_saveeditor":
                        strOut = SaveEditor();
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
                var rtn = _appTheme.CopyVersion(_appTheme.AppVersionFolder, (Convert.ToDouble(_appTheme.LatestVersionFolder) + 1).ToString("0.0"));
                _appVersionFolder = _appTheme.AppVersionFolder;
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
                _appTheme.DeleteVersion();
                _appVersionFolder = _appTheme.AppVersionFolder;
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
                _appTheme.DeleteTheme();
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

                // crearte new apptheme.
                var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, newAppThemeName, "1.0");

                _appThemeDataList.PopulateAppThemeList();
                return "";
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
                return GetEditTemplate(_appTheme);
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

private static Dictionary<string, string> ExportAppTheme()
{
    var appThemeFolder = _paramInfo.GetXmlProperty("genxml/urlparams/appthemefolder");
    var appVersionFolder = _paramInfo.GetXmlProperty("genxml/urlparams/appversionfolder");
    var appTheme = new AppTheme(_appThemeDataList.SelectedSystemKey, appThemeFolder, appVersionFolder, true);

    var rtnDic = new Dictionary<string, string>();

    rtnDic.Add("downloadfiledata", appTheme.Export());
    rtnDic.Add("downloadname", appTheme.AppThemeFolder + ".xml");

    return rtnDic;
}

        private static string ImportAppTheme()
        {
            var fileuploadlist = _paramInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                var userid = DNNrocketUtils.GetCurrentUserId();
                var userFolder = DNNrocketUtils.TempDirectory() + "\\user" + userid;
                var objCtrl = new DNNrocketController();

                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);

                        // get import data 
                        var xmlData = FileUtils.ReadFile(userFolder + "\\" + friendlyname);
                        var _appTheme = new AppTheme(xmlData, true, _systemInfoData.DebugMode);
                        // delete import file
                        //File.Delete(userFolder + "\\" + friendlyname);
                    }
                }

            }

            return GetList();
        }

        public static void SaveData()
        {
            _appTheme.Save(_postInfo);
        }

        public static string SaveEditor()
        {
            var editorcode = _postInfo.GetXmlProperty("genxml/hidden/editorcodesave");
            var filename = _postInfo.GetXmlProperty("genxml/hidden/editorfilenamesave");
            var listname = _postInfo.GetXmlProperty("genxml/hidden/editorlistnamesave");
            _appTheme.SaveEditor(listname, filename, editorcode);
            return "OK";
        }

        public static void AddListImage()
        {
            _appTheme.AddListImage();
        }

        public static string AddListTemplate()
        {
            _appTheme.AddListTemplate();
            return GetEditTemplate(_appTheme);
        }
        public static string AddListCss()
        {
            _appTheme.AddListCss();
            return GetEditTemplate(_appTheme);
        }
        public static string AddListJs()
        {
            _appTheme.AddListJs();
            return GetEditTemplate(_appTheme);
        }
        public static string AddListResx()
        {
            var culturecoderesx = _paramInfo.GetXmlProperty("genxml/hidden/culturecoderesx");

            // get default data
            var jsonResx = "";
            var resxItem = _appTheme.Record.GetRecordListItem("resxlist", "genxml/hidden/culturecode", "");
            if (resxItem != null) jsonResx = resxItem.GetXmlProperty("genxml/hidden/jsonresx");

            _appTheme.AddListResx(culturecoderesx, jsonResx);
            return GetEditTemplate(_appTheme);
        }
        private static string AddListField()
        {
            _appTheme.AddListField();
            return GetEditTemplate(_appTheme);
        }
        private static void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }
        private static string GetEditTemplate(AppTheme appTheme)
        {
            var defaultjsonresxItem = appTheme.Record.GetRecordListItem("resxlist", "genxml/resxlist/genxml/hidden/culturecode", "");
            var defaultjsonresx = defaultjsonresxItem.GetXmlProperty("genxml/hidden/jsonresx");
            if (!_passSettings.ContainsKey("defaultjsonresx")) _passSettings.Add("defaultjsonresx", defaultjsonresx);

            var razorTempl = DNNrocketUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            return DNNrocketUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }


    }
}
