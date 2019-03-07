using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace RocketMod
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            var rocketInterface = new DNNrocketInterface(interfaceInfo);

            var cookieModuleId = DNNrocketUtils.GetCookieValue("rocketmod_moduleid");
            var moduleid = 0;
            if (!GeneralUtils.IsNumeric(cookieModuleId))
            {
                moduleid = postInfo.ModuleId;
            }
            else
            {
                moduleid = Convert.ToInt32(cookieModuleId);
            }


            var strOut = "";

            switch (paramCmd)
            {
                case "rocketmod_getconfig":
                    strOut = ConfigUtils.GetConfig(moduleid, rocketInterface);
                    break;
                case "rocketmod_getdata":
                        strOut = GetData(moduleid, rocketInterface, postInfo);
                    break;
                case "rocketmod_edit":
                    strOut = "EDIT THE DATA";
                    break;
                case "rocketmod_saveconfig":
                    strOut = ConfigUtils.SaveConfig(moduleid, postInfo);
                    if (strOut == "")
                    {
                        // not error returned , so return details.
                        strOut = GetData(moduleid, rocketInterface, postInfo);
                    }
                    break;
                case "rocketmod_getsetupmenu":
                    strOut = ConfigUtils.GetSetup(moduleid, rocketInterface);
                    break;
                case "rocketmod_adminurl":
                    strOut = "/desktopmodules/dnnrocket/RocketMod/admin.html";
                    break;
                case "rocketmod_dashboard":
                    strOut = GetDashBoard(moduleid, rocketInterface);
                    break;
                case "rocketmod_reset":
                    strOut = ResetRocketMod(moduleid, rocketInterface);
                    break;
                default:
                    strOut = "Comamnd Not Found";
                    break;
            }

            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static String ResetRocketMod(int moduleid, DNNrocketInterface rocketInterface)
        {
            try
            {
                ConfigUtils.DeleteConfig(moduleid);
                return ConfigUtils.GetSetup(moduleid, rocketInterface);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetDashBoard(int moduleid, DNNrocketInterface rocketInterface)
        {
            try
            {
                var controlRelPath = rocketInterface.TemplateRelPath;
                if (controlRelPath == "") controlRelPath = ControlRelPath;

                var themeFolder = rocketInterface.DefaultTheme;
                var razortemplate = rocketInterface.DefaultTemplate;

                var passSettings = rocketInterface.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();

                var info = objCtrl.GetData("moduleconfig", "MDATA", DNNrocketUtils.GetEditCulture(), moduleid);
                return DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetData(int moduleid, DNNrocketInterface rocketInterface, SimplisityInfo postInfo)
        {
            try
            {
                var strOut = "";

                if (ConfigUtils.HasConfig(moduleid))
                {
                    strOut = "<h1> DATA to be returned</h1>";
                    strOut += "<p> lkwde fpowe pofk ok wefopk wepof kpowe fop weop fd po we odkopwed opkweopd weopkdpowefodpk wpoefk</p>";

                    var controlRelPath = rocketInterface.TemplateRelPath;
                    if (controlRelPath == "") controlRelPath = ControlRelPath;

                    var themeFolder = rocketInterface.DefaultTheme;
                    var razortemplate = rocketInterface.DefaultTemplate;

                    var passSettings = rocketInterface.ToDictionary();
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                    var objCtrl = new DNNrocketController();

                    var info = objCtrl.GetData("moduleconfig", "MDATA", DNNrocketUtils.GetEditCulture(), moduleid);
                    //return DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);


                }
                else
                {
                    strOut = ConfigUtils.GetSetup(moduleid, rocketInterface);
                }

                return strOut;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
