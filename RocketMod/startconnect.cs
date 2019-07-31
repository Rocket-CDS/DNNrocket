using System;
using System.Collections.Generic;
using System.Linq;
using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
using Simplisity;

namespace RocketMod
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        //private static ModuleData _moduleData;
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        private static string _rocketModRelPath;
        private static string _rocketModMapPath;
        private static SimplisityInfo _postInfo;
        private static SimplisityInfo _paramInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static SimplisityInfo _systemInfo;
        private static ConfigData _configData;
        private static int _tabid;
        private static int _moduleid;
        private static string _langRequired;
        private static SystemInfoData _systemInfoData;
        private static Dictionary<string, string> _passSettings;
        private static SettingsData _settingsData;
        private static string _editLang;
        private static int _selectedItemId;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            _systemInfoData = new SystemInfoData(systemInfo);
            _langRequired = langRequired;
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _rocketModRelPath = "/DesktopModules/DNNrocket/RocketMod";
            _rocketModMapPath = DNNrocketUtils.MapPath(_rocketModRelPath);
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemInfo = systemInfo;

            _selectedItemId = _paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

            _editLang = langRequired;
            if (_editLang == "") _editLang = DNNrocketUtils.GetEditCulture();

            _settingsData = GetSettingsData();
            _passSettings = LocalUtils.SettingsToDictionary(_settingsData);
            _passSettings.Add("stopanimate", _paramInfo.GetXmlProperty("genxml/hidden/stopanimate"));

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid"); // IPN           
            _passSettings.Add("moduleid", _moduleid.ToString());
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid"); // IPN
            _passSettings.Add("tabid", _tabid.ToString());

            _configData = new ConfigData(paramInfo.PortalId, _systemInfo, _tabid, _moduleid);
            //_moduleData = new ModuleData(_configData, langRequired);
            //_postInfo.ModuleId = _moduleid; // make sure we have correct moduleid.


            if (!CheckSecurity(paramCmd))
            {
                paramCmd = "rocketmod_login";
            }
            else
            {
                if (_configData.AppTheme == "" && paramCmd != "rocketmod_saveapptheme" && paramCmd != "rocketmod_getsidemenu" && paramCmd != "rocketmod_getdata")
                {
                    paramCmd = "rocketmod_selectapptheme";  //we must have an apptheme to work on.
                }
            }

            switch (paramCmd)
            {
                case "rocketmod_login":
                    strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    break;

                case "dashboard_get":
                    strOut = GetDashBoard();
                    break;

                case "rocketmod_selectapptheme":
                    strOut = GetSelectApp();
                    break;
                case "rocketmod_saveapptheme":
                    SaveAppTheme();
                    strOut = GetDashBoard();
                    break;

                case "rocketmod_getdata":
                    strOut = GetDisplay();
                    break;

                case "edit_editarticlelist":
                    strOut = GetArticleList(true);
                    break;
                case "edit_articlesearch":
                    strOut = GetArticleList(false);
                    break;
                case "edit_editarticle":
                    strOut = GetArticle();
                    break;
                case "edit_addarticle":
                    strOut = AddArticle();
                    break;
                case "edit_savearticle":
                    SaveArticle();
                    strOut = GetArticle();
                    break;
                case "edit_deletearticle":
                    DeleteArticle();
                    strOut = GetArticleList(true);
                    break;

                case "rocketmod_saveconfig":
                    SaveConfig();
                    strOut = GetDashBoard();
                    break;
                case "rocketmod_getsetupmenu":
                    strOut = GetSetup();
                    break;
                case "rocketmod_config":
                    strOut = GetConfig();
                    break;
                case "rocketmod_reset":
                    strOut = ResetRocketMod();
                    break;
                case "rocketmod_resetdata":
                    strOut = ResetDataRocketMod();
                    break;

                case "rocketmodsettings_edit":
                    strOut = EditSettingsData();
                    break;
                case "rocketmodsettings_add":
                    strOut = AddSettingsRow();
                    break;
                case "rocketmodsettings_save":
                    strOut = SettingsSave();
                    break;
                case "rocketmodsettings_delete":
                    strOut = SettingsDelete();
                    break;

                case "rocketmodfields_edit":
                    strOut = EditFieldsData();
                    break;
                case "rocketmodfields_add":
                    strOut = AddFieldsRow();
                    break;
                case "rocketmodfields_save":
                    strOut = FieldsSave();
                    break;
                case "rocketmodfields_delete":
                    strOut = FieldsDelete();
                    break;


            }

            if (strOut == "" && !_configData.Exists)
            {
                return DNNrocketUtils.ReturnString(GetSetup());
            }


            return DNNrocketUtils.ReturnString(strOut);
        }

        public static bool CheckSecurity(string paramCmd)
        {
            _commandSecurity = new CommandSecurity(_tabid, _moduleid, _rocketInterface);

            _commandSecurity.AddCommand("dashboard_get", true);

            _commandSecurity.AddCommand("rocketmod_saveconfig", true);
            _commandSecurity.AddCommand("rocketmod_saveheader", true);
            _commandSecurity.AddCommand("rocketmod_getsetupmenu", true);
            _commandSecurity.AddCommand("rocketmod_reset", true);
            _commandSecurity.AddCommand("rocketmod_resetdata", true);
            _commandSecurity.AddCommand("rocketmod_selectapptheme", true);
            _commandSecurity.AddCommand("rocketmod_saveapptheme", true);
            _commandSecurity.AddCommand("rocketmod_getsidemenu", true);

            _commandSecurity.AddCommand("rocketmod_editarticlelist", true);
            _commandSecurity.AddCommand("rocketmod_articlesearch", true);
            _commandSecurity.AddCommand("rocketmod_editarticle", true);
            _commandSecurity.AddCommand("rocketmod_addarticle", true);
            _commandSecurity.AddCommand("rocketmod_savedata", true);
            _commandSecurity.AddCommand("rocketmod_delete", true);


            _commandSecurity.AddCommand("rocketmod_getdata", false);
            _commandSecurity.AddCommand("rocketmod_login", false);

            _commandSecurity.AddCommand("rocketmodsettings_edit", true);
            _commandSecurity.AddCommand("rocketmodsettings_add", true);
            _commandSecurity.AddCommand("rocketmodsettings_save", true);
            _commandSecurity.AddCommand("rocketmodsettings_delete", true);

            _commandSecurity.AddCommand("rocketmodfields_edit", true);
            _commandSecurity.AddCommand("rocketmodfields_add", true);
            _commandSecurity.AddCommand("rocketmodfields_save", true);
            _commandSecurity.AddCommand("rocketmodfields_delete", true);

            var hasAccess = false;
            hasAccess = _commandSecurity.HasSecurityAccess(paramCmd);

            return hasAccess;
        }


        private static void SaveConfig()
        {
            var appTheme = new AppTheme(_configData.AppTheme, DNNrocketUtils.GetEditCulture(), _configData.AppThemeVersion);
            _configData.SaveConfig(_postInfo, _appthemeRelPath.TrimEnd('/') + "/" + _postInfo.GetXmlProperty("genxml/hidden/apptheme") + "/" + _postInfo.GetXmlProperty("genxml/select/versionfolder"), _appthemeRelPath.TrimEnd('/'));
            _passSettings.Add("saved", "true");
        }

        private static void SaveAppTheme()
        {
            _configData.SaveAppTheme(_paramInfo.GetXmlProperty("genxml/hidden/apptheme"));
            _passSettings.Add("saved", "true");
        }

        #region "Articles"

        public static void SaveArticle()
        {
            var articleData = new ArticleData(_selectedItemId, _moduleid, _editLang);
            _passSettings.Add("saved", "true");
            articleData.Save(_postInfo);
        }

        public static void DeleteArticle()
        {
            var articleData = new ArticleData(_selectedItemId, _moduleid, _editLang);
            articleData.Delete();
        }

        public static String AddArticle()
        {
            try
            {
                var articleData = new ArticleData(-1, _moduleid, _editLang);
                _selectedItemId = articleData.ItemId;
                var strOut = GetArticle();

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String GetArticle()
        {
            try
            {
                AssignEditLang();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("edit.cshtml", _appthemeRelPath, _configData.AppTheme, _editLang, _rocketInterface.ThemeVersion, _systemInfoData.DebugMode);
                var articleData = new ArticleData(_selectedItemId, _moduleid, _editLang);
                articleData.ImageFolder = _configData.ImageFolder;
                articleData.DocumentFolder = _configData.DocumentFolder;
                var strOut = DNNrocketUtils.RazorDetail(razorTempl, articleData, _passSettings, new SimplisityInfo(), _systemInfoData.DebugMode);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String GetArticleList(bool loadCachedHeader)
        {

            try
            {
                AssignEditLang();
                var articleDataList = new ArticleDataList(_moduleid, _editLang);
                if (loadCachedHeader)
                {
                    articleDataList.LoadCacheHeader();
                }
                else
                {
                    articleDataList.PageSize = _postInfo.GetXmlPropertyInt("genxml/hidden/pagesize");
                    articleDataList.Page = _paramInfo.GetXmlPropertyInt("genxml/hidden/page");
                    articleDataList.SearchText = _postInfo.GetXmlProperty("genxml/textbox/searchtext");
                    articleDataList.SaveCacheHeader();
                }
                articleDataList.Populate();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editlist.cshtml", _appthemeRelPath, _configData.AppTheme, _editLang, _rocketInterface.ThemeVersion, _systemInfoData.DebugMode);
                var strOut = DNNrocketUtils.RazorDetail(razorTempl, articleDataList, _passSettings, articleDataList.Header, _systemInfoData.DebugMode);
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        #endregion



        #region "Fields"

        private static SettingsData GetFieldsData()
        {
            return new SettingsData(_tabid, _moduleid, _langRequired, _rocketInterface.EntityTypeCode, "fielddata", false, _rocketInterface.DatabaseTable);
        }

        private static String EditFieldsData()
        {
            try
            {
                var fieldsData = GetFieldsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetEditCulture(), "1.0", _systemInfoData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, fieldsData, passSettings);

                if (strOut == "") strOut = "ERROR: No data returned for EditfieldsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private static string FieldsDelete()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Delete();
            return EditFieldsData();
        }

        private static string AddFieldsRow()
        {
            var fieldsData = GetFieldsData();
            fieldsData.AddRow();
            return EditFieldsData();
        }

        private static String FieldsSave()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Save(_postInfo);
            return EditFieldsData();
        }

        #endregion

        #region "Settings"

        private static SettingsData GetSettingsData()
        {
            return new SettingsData(_tabid, _moduleid, _langRequired, "ROCKETMODSETTINGS", "rocketmodsettings", true, _rocketInterface.DatabaseTable);
        }
        private static String EditSettingsData()
        {
            try
            {
                var settingsData = GetSettingsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetEditCulture(), "1.0", _systemInfoData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, settingsData, passSettings);

                if (strOut == "") strOut = "ERROR: No data returned for EditSettingsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private static string SettingsDelete()
        {
            var settingsData = GetSettingsData();
            settingsData.Delete();
            return EditSettingsData();
        }

        private static string AddSettingsRow()
        {
            var settingsData = GetSettingsData();
            settingsData.AddRow();
            return EditSettingsData();
        }

        private static String SettingsSave()
        {
            var settingsData = GetSettingsData();
            settingsData.Save(_postInfo);
            return EditSettingsData();
        }

        #endregion

        public static String ResetRocketMod()
        {
            try
            {
                _configData.DeleteConfig();

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleid" + _moduleid, "ROCKETMODFIELDS", "", -1, _moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);
                info = objCtrl.GetData("moduleid" + _moduleid, "ROCKETSETTINGS", "", -1, _moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);

                CacheUtils.ClearAllCache();
                _configData.PopulateConfig();

                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String ResetDataRocketMod()
        {
            try
            {
                //_moduleData.DeleteData();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDashBoard()
        {
            try
            {
                AssignEditLang();
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                _passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _configData, _passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetConfig()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;

                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "config.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());


                var passSettings = _paramInfo.ToDictionary();
                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                //return DNNrocketUtils.RazorDetail(razorTempl, _moduleData, passSettings);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetSelectApp()
        {

            try
            {
                var strOut = "";
                if (!_configData.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    var razortemplate = "selectapp.cshtml";
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                    var appList = new List<Object>();
                    var dirlist = System.IO.Directory.GetDirectories(_appthemeMapPath + "\\Themes");
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        var appTheme = new AppTheme(dr.Name);
                        appList.Add(appTheme);
                    }

                   strOut = DNNrocketUtils.RazorList(razorTempl, appList, _passSettings, null, _systemInfoData.DebugMode);

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


        public static String GetDisplay()
        {

            try
            {
                var strOut = "";
                if (_configData.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    var razortemplate = "view.cshtml";
                    var apptheme = _configData.ConfigInfo.GetXmlProperty("genxml/hidden/apptheme");
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, apptheme, DNNrocketUtils.GetCurrentCulture());

                    var passSettings = _paramInfo.ToDictionary();
                    
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());

                    var adminurl = "/DesktopModules/DNNrocket/RocketMod/admin.aspx?moduleid=" + _moduleid + "&tabid=" + _tabid;
                    passSettings.Add("adminurl", adminurl);

                    var appTheme = new AppTheme(apptheme,_configData.AppThemeVersion);

                    //strOut = DNNrocketUtils.RazorDetail(appTheme.ActiveViewTemplate, _moduleData.CurrentRecord, passSettings, _moduleData.HeaderInfo);

                }
                else
                {
                    strOut = GetSetup();
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        private static String GetSetup()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                _passSettings.Add("tabid", _tabid.ToString());
                var strOut = "";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(),"1.0",_systemInfoData.DebugMode);
                return DNNrocketUtils.RazorDetail(razorTempl, _rocketInterface.Info,_passSettings, new SimplisityInfo(), _systemInfoData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static bool CheckIfList()
        {
            try
            {
                if (_configData.Exists)
                {
                    var themeFolder = _configData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData("editlist.cshtml", _appthemeRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                    if (razorTempl != "") return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
                return false;
            }

        }

        private static void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }


    }
}
