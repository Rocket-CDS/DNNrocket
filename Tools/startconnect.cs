using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace Rocket.Tools
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private static DNNrocketInterface _rocketInterface;
        private static SystemInfoData _systemInfoData;
        private static Dictionary<string, string> _passSettings;
        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.
            var rtnDic = new Dictionary<string, string>();

            _systemInfoData = new SystemInfoData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _passSettings = new Dictionary<string, string>();

            switch (paramCmd)
            {
                case "rockettools_login":
                    strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    break;
                case "rockettools_dashboard":
                    strOut = GetDashBoard();
                    break;

            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;

        }
        public static String GetDashBoard()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return DNNrocketUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


    }
}
