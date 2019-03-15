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
        private static SimplisityInfo _postInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static AppThemeData _appThemeData;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string langRequired = "")
        {
            var strOut = "ERROR"; // return ERROR if not matching commands.

            paramCmd = paramCmd.ToLower();

            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppTheme";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _postInfo = postInfo;

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            var moduleid = _postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (moduleid == 0) moduleid = _postInfo.ModuleId;
            var tabid = _postInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.

            _appThemeData = new AppThemeData(tabid, moduleid, langRequired);

            _commandSecurity = new CommandSecurity(tabid, moduleid, _rocketInterface);
            _commandSecurity.AddCommand("rocketapptheme_edit", true);
            _commandSecurity.AddCommand("rocketapptheme_add", true);
            _commandSecurity.AddCommand("rocketapptheme_save", true);
            _commandSecurity.AddCommand("rocketapptheme_delete", true);
            _commandSecurity.AddCommand("rocketapptheme_dashboard", true);
            _commandSecurity.AddCommand("rocketapptheme_builder", true);

            _commandSecurity.AddCommand("rocketapptheme_getdata", false);
            _commandSecurity.AddCommand("rocketapptheme_login", false);

            if (_commandSecurity.SecurityCommandCheck(paramCmd))
            {
                switch (paramCmd)
                {
                    case "rocketapptheme_dashboard":
                        strOut = GetDashBoard();
                        break;
                    case "rocketapptheme_builder":
                        strOut = GetDashBoard();
                        break;

                }
            }
            else
            {
                if (systemInfo.GetXmlPropertyBool("genxml/checkbox/debugmode"))
                {
                    strOut = "<h1>ERROR</h1> <p><b>Invalid Command - check commandSecurity() class</b></p> <p>" + paramCmd + "  ModuleID:" + _appThemeData.ModuleId + "  TabID:" + _appThemeData.TabId + "</p>";
                    strOut += "<div class='w3-card-4 w3-padding w3-codespan'>" + DNNrocketUtils.HtmlOf(postInfo.XMLData) + "</div>";
                }

                if (_commandSecurity.ValidCommand(paramCmd))
                {
                    strOut = LoginUtils.LoginForm(postInfo, _rocketInterface.InterfaceKey);
                }
            }

            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;



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

                return DNNrocketUtils.RazorDetail(razorTempl, _appThemeData.Info, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }




    }
}
