using System;
using System.Collections.Generic;
using System.Linq;
using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;

namespace RocketMod
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static ModuleData _moduleData;
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        private static string _rocketModRelPath;
        private static string _rocketModMapPath;
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static SimplisityInfo _systemInfo;
        private static ConfigData _configData;
        private static int _tabid;
        private static int _moduleid;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _rocketModRelPath = "/DesktopModules/DNNrocket/RocketMod";
            _rocketModMapPath = DNNrocketUtils.MapPath(_rocketModRelPath);
            _postInfo = postInfo;
            _systemInfo = systemInfo;

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            _moduleid = _postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _postInfo.GetXmlPropertyInt("genxml/urlparams/moduleid"); // IPN           
            if (_moduleid == 0) _moduleid = _postInfo.ModuleId;
            _tabid = _postInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _postInfo.GetXmlPropertyInt("genxml/urlparams/tabid"); // IPN

            var selecteditemid = _postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

            _configData = new ConfigData(postInfo.PortalId, _systemInfo.ItemID, _tabid, _moduleid);
            _moduleData = new ModuleData(_configData, selecteditemid, langRequired);
            _postInfo.ModuleId = _moduleData.ModuleId; // make sure we have correct moduleid.

            _commandSecurity = new CommandSecurity(_moduleData.TabId, _moduleData.ModuleId, _rocketInterface);
            _commandSecurity.AddCommand("rocketmod_edit", true);
            _commandSecurity.AddCommand("rocketmod_savedata", true);
            _commandSecurity.AddCommand("rocketmod_delete", true);
            _commandSecurity.AddCommand("rocketmod_saveconfig", true);
            _commandSecurity.AddCommand("rocketmod_saveheader", true);
            _commandSecurity.AddCommand("rocketmod_getsetupmenu", true);
            _commandSecurity.AddCommand("rocketmod_dashboard", true);
            _commandSecurity.AddCommand("rocketmod_reset", true);
            _commandSecurity.AddCommand("rocketmod_resetdata", true);
            _commandSecurity.AddCommand("rocketmod_add", true);

            _commandSecurity.AddCommand("rocketmod_getdata", false);
            _commandSecurity.AddCommand("rocketmod_login", false);

            if (!_commandSecurity.HasSecurityAccess(paramCmd))
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                return DNNrocketUtils.ReturnString(strOut);
            }

            switch (paramCmd)
            {
                case "rocketmod_getdata":
                    strOut = GetDisplay();
                    break;
                case "rocketmod_edit":
                    strOut = EditData();
                    break;
                case "rocketmod_savedata":
                    _moduleData.SaveData(postInfo);
                    strOut = EditData();
                    break;
                case "rocketmod_saveheader":
                    _moduleData.SaveHeader(postInfo);
                    strOut = EditData();
                    break;
                case "rocketmod_add":
                    //_moduleData.AddNew();
                    strOut = EditData();
                    break;
                case "rocketmod_delete":
                    _moduleData.DeleteData();
                    strOut = EditData();
                    break;
                case "rocketmod_saveconfig":
                    _moduleData.configData.SaveConfig(postInfo);
                    strOut = GetDashBoard();
                    break;
                case "rocketmod_getsetupmenu":
                    strOut = GetSetup();
                    break;
                case "rocketmod_dashboard":
                    strOut = GetDashBoard();
                    break;
                case "rocketmod_reset":
                    strOut = ResetRocketMod();
                    break;
                case "rocketmod_resetdata":
                    strOut = ResetDataRocketMod();
                    break;
                case "rocketmod_login":
                    strOut = LoginUtils.DoLogin(systemInfo, postInfo, userHostAddress);
                    break;
            }

            return DNNrocketUtils.ReturnString(strOut);
        }


        public static String EditData()
        {
            try
            {
                var strOut = "";
                //var themeFolder = "";
                //var razortemplate = "";

                //if (!_moduleData.configData.Exists)
                //{
                //    // no display type set, return dashboard.
                //    return GetDashBoard();
                //}

                //if (_moduleData.IsList)
                //{
                //    if (_moduleData.SelectedItemId > 0)
                //    {
                //        razortemplate = "editdetail.cshtml";
                //    }
                //    else
                //    {
                //        razortemplate = "editlist.cshtml";
                //    }
                //}
                //else
                //{
                //    razortemplate = "edit.cshtml";
                //}

                //themeFolder = _moduleData.configData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");

                //var passSettings = _postInfo.ToDictionary();
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, themeFolder, DNNrocketUtils.GetEditCulture());
                //strOut = DNNrocketUtils.RazorList(razorTempl, _moduleData.List, passSettings,_moduleData.HeaderInfo);

                //if (strOut == "") strOut = "ERROR: No data returned for " + _appthemeMapPath + "\\" + themeFolder + "\\default\\" + razortemplate;
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
                if (controlRelPath == "") controlRelPath = ControlRelPath;

                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _moduleData, passSettings);
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
                    var themeFolder = _moduleData.configData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                    var passSettings = _postInfo.ToDictionary();
                    
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());

                    strOut = DNNrocketUtils.RazorList(razorTempl, _moduleData.List, passSettings, _moduleData.HeaderInfo);

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
                _rocketInterface.ModuleId = _moduleData.ModuleId;
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
