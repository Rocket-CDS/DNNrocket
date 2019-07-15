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
        private static ModuleData _moduleData;
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


            // Ensure we have a valid edit data cultueCode.  It may have been changed in a localization admin pages.
            if (_langRequired == "" && DNNrocketUtils.GetCultureCodeList().Count() > 0 && (!DNNrocketUtils.GetCultureCodeList().Contains(DNNrocketUtils.GetEditCulture())))
            {
                DNNrocketUtils.SetEditCulture(DNNrocketUtils.GetCultureCodeList().First());
            }

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid"); // IPN           
            if (_moduleid == 0) _moduleid = _paramInfo.ModuleId;
            if (_moduleid == 0)
            {
                var cookie_moduleid = DNNrocketUtils.GetCookieValue("rocketmod_moduleid");
                if (GeneralUtils.IsNumeric(cookie_moduleid)) _moduleid =  Convert.ToInt32(cookie_moduleid);
            }

            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid"); // IPN

            var selecteditemid = _paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

            _configData = new ConfigData(paramInfo.PortalId, _systemInfo, _tabid, _moduleid);
            _moduleData = new ModuleData(_configData, langRequired);
            _postInfo.ModuleId = _moduleData.ModuleId; // make sure we have correct moduleid.

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

                case "rocketmod_getsidemenu":
                    strOut = GetSideMenu(paramInfo, systemInfo);
                    break;
                case "rocketmod_selectapptheme":
                    strOut = GetSelectApp();
                    break;
                case "rocketmod_saveapptheme":                    
                    _moduleData.configData.SaveAppTheme(paramInfo.GetXmlProperty("genxml/hidden/apptheme"));
                    _moduleData.Populate();
                    strOut = GetDashBoard();
                    break;
                case "rocketmod_getdata":
                    strOut = GetDisplay();
                    break;
                case "rocketmod_edit":
                    strOut = EditData();
                    break;
                case "rocketmod_save":
                    _moduleData.Data.Save(postInfo);
                    strOut = EditData();
                    break;
                case "rocketmod_saveheader":
                    _moduleData.SaveHeader(postInfo);
                    strOut = EditData();
                    break;
                case "rocketmod_add":
                    _moduleData.Data.AddRow();
                    strOut = EditData();
                    break;
                case "rocketmod_delete":
                    strOut = EditData();
                    break;
                case "rocketmod_saveconfig":
                    var appTheme = new AppTheme(_moduleData.configData.AppTheme, DNNrocketUtils.GetEditCulture(), _configData.AppThemeVersion);
                    _moduleData.configData.SaveConfig(postInfo, _appthemeRelPath.TrimEnd('/') + "/" + postInfo.GetXmlProperty("genxml/hidden/apptheme") + "/" + postInfo.GetXmlProperty("genxml/select/versionfolder"), _appthemeRelPath.TrimEnd('/'));
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

            if (strOut == "" && !_moduleData.configData.Exists)
            {
                return DNNrocketUtils.ReturnString(GetSetup());
            }


            return DNNrocketUtils.ReturnString(strOut);
        }

        public static bool CheckSecurity(string paramCmd)
        {
            _commandSecurity = new CommandSecurity(_moduleData.TabId, _moduleData.ModuleId, _rocketInterface);

            _commandSecurity.AddCommand("dashboard_get", true);

            _commandSecurity.AddCommand("rocketmod_edit", true);
            _commandSecurity.AddCommand("rocketmod_save", true);
            _commandSecurity.AddCommand("rocketmod_delete", true);
            _commandSecurity.AddCommand("rocketmod_saveconfig", true);
            _commandSecurity.AddCommand("rocketmod_saveheader", true);
            _commandSecurity.AddCommand("rocketmod_getsetupmenu", true);
            _commandSecurity.AddCommand("rocketmod_reset", true);
            _commandSecurity.AddCommand("rocketmod_resetdata", true);
            _commandSecurity.AddCommand("rocketmod_add", true);
            _commandSecurity.AddCommand("rocketmod_selectapptheme", true);
            _commandSecurity.AddCommand("rocketmod_saveapptheme", true);
            _commandSecurity.AddCommand("rocketmod_getsidemenu", true);

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
            return new SettingsData(_tabid, _moduleid, _langRequired, _rocketInterface.EntityTypeCode, "rocketmodsettings", false, _rocketInterface.DatabaseTable);
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
            if (settingsData.InvalidKeyValues)
            {
                return "Invalid Key Values in Template.  '@HiddenField(i, \"genxml/key1\", \"\", \"\", false, lp3)' and '@HiddenField(i, \"genxml/lang/genxml/key2\", \"\", \"\", true, lp3)' must be in the template for each setting row.";
            }
            else
            {
                return EditSettingsData();
            }

        }

        #endregion


        public static string GetSideMenu(SimplisityInfo sInfo, SimplisityInfo systemInfo)
        {
            try
            {
                var strOut = "";
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = sInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                if (moduleid == 0) moduleid = -1;

                var passSettings = sInfo.ToDictionary();

                var systemData = new SystemData();
                var sidemenu = new SideMenu(systemInfo);
                var templateControlRelPath = _rocketModRelPath;
                sidemenu.ModuleId = moduleid;

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                if (razorTempl == "")
                {
                    // no razor template for sidemenu, so use default.
                    razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                }

                strOut = DNNrocketUtils.RazorDetail(razorTempl, sidemenu, passSettings, _configData.ConfigInfo);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }



        public static String EditData()
        {
            try
            {
                if (_moduleData.Data.List.Count == 0)
                {
                    // always have 1 row when editing.
                    _moduleData.Data.AddRow();
                }

                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var appTheme = new AppTheme(_moduleData.configData.AppTheme, DNNrocketUtils.GetEditCulture(), _configData.AppThemeVersion);
                strOut = DNNrocketUtils.RazorDetail(appTheme.ActiveEditTemplate, _moduleData, passSettings, _moduleData.HeaderInfo);

                if (strOut == "") strOut = "ERROR: No data returned for EditData() : " + _rocketModMapPath;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String ResetRocketMod()
        {
            try
            {
                _moduleData.configData.DeleteConfig();

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleid" + _moduleData.ModuleId, "ROCKETMODFIELDS", "", -1, _moduleData.ModuleId, true);
                if (info != null) objCtrl.Delete(info.ItemID);
                info = objCtrl.GetData("moduleid" + _moduleData.ModuleId, "ROCKETSETTINGS", "", -1, _moduleData.ModuleId, true);
                if (info != null) objCtrl.Delete(info.ItemID);

                CacheUtils.ClearAllCache();
                _moduleData.configData.PopulateConfig();

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
                _moduleData.DeleteData();
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
                var controlRelPath = _rocketInterface.TemplateRelPath;

                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                var passSettings = _paramInfo.ToDictionary();
                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _moduleData, passSettings);
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

                return DNNrocketUtils.RazorDetail(razorTempl, _moduleData, passSettings);
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
                if (!_moduleData.configData.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    var razortemplate = "selectapp.cshtml";
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                    var passSettings = _paramInfo.ToDictionary();

                    var appList = new List<Object>();
                    var dirlist = System.IO.Directory.GetDirectories(_appthemeMapPath + "\\Themes");
                    foreach (var d in dirlist)
                    {
                        var dr = new System.IO.DirectoryInfo(d);
                        var appTheme = new AppTheme(dr.Name);
                        appList.Add(appTheme);
                    }

                    strOut = DNNrocketUtils.RazorList(razorTempl, appList, passSettings, _moduleData.HeaderInfo);

                }
                else
                {
                    strOut = EditData();
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
                if (_moduleData.configData.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    var razortemplate = "view.cshtml";
                    var apptheme = _moduleData.configData.ConfigInfo.GetXmlProperty("genxml/hidden/apptheme");
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, apptheme, DNNrocketUtils.GetCurrentCulture());

                    var passSettings = _paramInfo.ToDictionary();
                    
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());

                    var adminurl = "/DesktopModules/DNNrocket/RocketMod/admin.aspx?moduleid=" + _moduleid + "&tabid=" + _tabid;
                    passSettings.Add("adminurl", adminurl);

                    var appTheme = new AppTheme(apptheme,_configData.AppThemeVersion);

                    strOut = DNNrocketUtils.RazorDetail(appTheme.ActiveViewTemplate, _moduleData.CurrentRecord, passSettings, _moduleData.HeaderInfo);

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
                _rocketInterface.Info.ModuleId = _moduleData.ModuleId;
                var strOut = "";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                return DNNrocketUtils.RazorDetail(razorTempl, _rocketInterface.Info,_postInfo.ToDictionary());
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
                if (_moduleData.configData.Exists)
                {
                    var themeFolder = _moduleData.configData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");
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


    }
}
