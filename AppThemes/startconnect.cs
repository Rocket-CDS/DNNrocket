using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;

namespace DNNrocket.AppThemes
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        private static string _adminAppthemeRelPath;
        private static string _adminAppthemeMapPath;
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static AppThemeData _appThemeData;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string langRequired = "")
        {
            var strOut = "ERROR"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _adminAppthemeRelPath = "/DesktopModules/DNNrocket/AppThemes/Admin";
            _adminAppthemeMapPath = DNNrocketUtils.MapPath(_adminAppthemeRelPath);
            _postInfo = postInfo;

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            var moduleid = _postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (moduleid == 0) moduleid = _postInfo.ModuleId;
            var tabid = _postInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.

            if (moduleid == 0) moduleid = -1;
            if (tabid == 0) tabid = -1;

            _appThemeData = new AppThemeData(tabid, moduleid, langRequired);

            _commandSecurity = new CommandSecurity(tabid, moduleid, _rocketInterface);
            _commandSecurity.AddCommand("rocketapptheme_edit", true);
            _commandSecurity.AddCommand("rocketapptheme_add", true);
            _commandSecurity.AddCommand("rocketapptheme_save", true);
            _commandSecurity.AddCommand("rocketapptheme_delete", true);
            _commandSecurity.AddCommand("rocketapptheme_dashboard", true);
            _commandSecurity.AddCommand("rocketapptheme_builder", true);

            if (!_commandSecurity.HasSecurityAccess(paramCmd))
            {
                strOut = LoginUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                return ReturnString(strOut);
            }

            switch (paramCmd)
            {
                case "rocketapptheme_dashboard":
                    strOut = GetDashBoard();
                    break;
                case "rocketapptheme_builder":
                    strOut = GetDashBoard();
                    break;
                case "rocketapptheme_editor":
                    strOut = GetEditor();
                    break;
                case "rocketapptheme_gettemplate":
                    strOut = GetTemplate();
                    break;
                   
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


        public static String GetTemplate()
        {
            try
            {
                var templateName = _postInfo.GetXmlProperty("genxml/select/templatename");
                var appthemeRelPath = _postInfo.GetXmlProperty("genxml/hidden/appthemerelpath");
                var appthemeversion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(templateName, appthemeRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), appthemeversion);

                return razorTempl;
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
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editor.cshtml", _adminAppthemeRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData.Info, passSettings);
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
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("dashboard.cshtml", _adminAppthemeRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("mappathAppThemeFolder", _appthemeMapPath);

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData.Info, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }




    }
}
