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
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            var rocketInterface = new DNNrocketInterface(interfaceInfo);
            var strOut = ""; // return nothing if not matching commands.
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _postInfo = postInfo;

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            var moduleid = _postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (moduleid == 0) moduleid = _postInfo.ModuleId;
            var tabid = _postInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.

            _moduleData = new ModuleData(tabid, moduleid);
            _postInfo.ModuleId = _moduleData.ModuleId; // make sure we have correct moduleid.

            _commandSecurity = new CommandSecurity(_moduleData.TabId, _moduleData.ModuleId, rocketInterface);
            _commandSecurity.AddCommand("rocketmod_edit", true);
            _commandSecurity.AddCommand("rocketmod_savedata", true);
            _commandSecurity.AddCommand("rocketmod_delete", true);
            _commandSecurity.AddCommand("rocketmod_saveconfig", true);
            _commandSecurity.AddCommand("rocketmod_getsetupmenu", true);
            _commandSecurity.AddCommand("rocketmod_dashboard", true);
            _commandSecurity.AddCommand("rocketmod_reset", true);
            _commandSecurity.AddCommand("rocketmod_resetdata", true);
            _commandSecurity.AddCommand("rocketmod_getdata", false);
            _commandSecurity.AddCommand("rocketmod_login", false);


            // use command form cookie if we have set it.
            var cookieCmd = DNNrocketUtils.GetCookieValue("rocketmod_cmd");
            if (cookieCmd != "")
            {
                paramCmd = cookieCmd;
                DNNrocketUtils.DeleteCookieValue("rocketmod_cmd");
            }

            if (_commandSecurity.SecurityCommandCheck(paramCmd))
            {
                switch (paramCmd)
                {
                    case "rocketmod_edit":
                        strOut = EditData(rocketInterface);
                        break;
                    case "rocketmod_savedata":
                        strOut = SaveData(moduleid, rocketInterface);
                        break;
                    case "rocketmod_delete":
                        DeleteData(moduleid, postInfo);
                        _moduleData.PopulateList();  // we need to clear the data, now it's deleted.
                        strOut = EditData(rocketInterface);
                        break;
                    case "rocketmod_saveconfig":
                        _moduleData.SaveConfig(postInfo, CheckIfList());
                        _moduleData.PopulateConfig();
                        strOut = GetDashBoard(rocketInterface);
                        break;
                    case "rocketmod_getsetupmenu":
                        strOut = GetSetup(rocketInterface);
                        break;
                    case "rocketmod_dashboard":
                        strOut = GetDashBoard(rocketInterface);
                        break;
                    case "rocketmod_reset":
                        strOut = ResetRocketMod(rocketInterface);
                        break;
                    case "rocketmod_resetdata":
                        strOut = ResetDataRocketMod(rocketInterface);
                        break;
                    case "rocketmod_getdata":
                        strOut = GetDisplay(rocketInterface);
                        break;
                    case "rocketmod_login":
                        strOut = LoginUtils.DoLogin(postInfo, userHostAddress);
                        break;
                }
            }
            else
            {
                if (_commandSecurity.ValidCommand(paramCmd))
                {
                    strOut = LoginUtils.LoginForm(postInfo, rocketInterface.InterfaceKey);
                }
            }

            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static void DeleteData(int moduleid, SimplisityInfo postInfo)
        {
            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            var objCtrl = new DNNrocketController();
            objCtrl.Delete(selecteditemid);
        }

        public static String EditData(DNNrocketInterface rocketInterface)
        {
            try
            {
                var strOut = "";
                var themeFolder = "";
                var razortemplate = "";

                if (!_moduleData.ConfigExists)
                {
                    // no display type set, return dashboard.
                    return GetDashBoard(rocketInterface);
                }

                if (_moduleData.IsList)
                {
                    razortemplate = "editlist.cshtml";
                }
                else
                {
                    razortemplate = "edit.cshtml";
                }

                themeFolder = _moduleData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");

                var passSettings = _postInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, themeFolder, DNNrocketUtils.GetEditCulture());
                strOut = DNNrocketUtils.RazorList(razorTempl, _moduleData.List, passSettings,_moduleData.HeaderInfo);

                if (strOut == "") strOut = "ERROR: No data returned for " + _appthemeMapPath + "\\" + themeFolder + "\\default\\" + razortemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String SaveData(int moduleid, DNNrocketInterface rocketInterface)
        {
            try
            {
                var objCtrl = new DNNrocketController();
                var info = _postInfo;
                if (_moduleData.List.Count() > 0)
                {
                    info = _moduleData.List.First();
                    info.XMLData = _postInfo.XMLData;
                }
                info.ModuleId = moduleid;
                objCtrl.SaveData(moduleid.ToString(), rocketInterface.EntityTypeCode, info, -1, moduleid);
                _moduleData.PopulateList();
                return EditData(rocketInterface);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String ResetRocketMod(DNNrocketInterface rocketInterface)
        {
            try
            {
                _moduleData.DeleteConfig();
                return GetDashBoard(rocketInterface);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String ResetDataRocketMod(DNNrocketInterface rocketInterface)
        {
            try
            {
                _moduleData.DeleteData();
                return GetDashBoard(rocketInterface);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDashBoard(DNNrocketInterface  rocketInterface)
        {
            try
            {
                var controlRelPath = rocketInterface.TemplateRelPath;
                if (controlRelPath == "") controlRelPath = ControlRelPath;

                var themeFolder = rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _moduleData.ConfigInfo, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDisplay(DNNrocketInterface rocketInterface)
        {

            try
            {
                var strOut = "";
                if (_moduleData.ConfigExists)
                {
                    var objCtrl = new DNNrocketController();

                    var razortemplate = "view.cshtml";
                    var themeFolder = _moduleData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _appthemeRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                    var passSettings = _postInfo.ToDictionary();
                    
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());

                    strOut = DNNrocketUtils.RazorList(razorTempl, _moduleData.List, passSettings, _moduleData.HeaderInfo);

                }
                else
                {
                    strOut = GetSetup(rocketInterface);
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        private static String GetSetup(DNNrocketInterface interfaceInfo)
        {
            try
            {
                interfaceInfo.ModuleId = _moduleData.ModuleId;
                var strOut = "";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", interfaceInfo.TemplateRelPath, interfaceInfo.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                return DNNrocketUtils.RazorDetail(razorTempl, interfaceInfo.Info,_postInfo.ToDictionary());
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
                if (_moduleData.ConfigExists)
                {
                    var themeFolder = _moduleData.ConfigInfo.GetXmlProperty("genxml/select/apptheme");
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
