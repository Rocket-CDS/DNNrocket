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

            var controlRelPath = rocketInterface.TemplateRelPath;
            if (controlRelPath == "") controlRelPath = ControlRelPath;
            var moduleid = postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");

            var strOut = "";

            switch (paramCmd)
            {
                case "rocketmod_getconfig":
                    strOut = ConfigUtils.GetConfig(moduleid, rocketInterface);
                    break;
                case "rocketmod_getdata":
                    if (ConfigUtils.HasConfig(moduleid))
                    {
                        strOut = GetData(rocketInterface, postInfo);
                    }
                    else
                    {
                        strOut = ConfigUtils.GetSetup(rocketInterface);
                    }
                    break;
                case "rocketmod_saveconfig":
                    strOut = ConfigUtils.SaveConfig(moduleid, postInfo);
                    if (strOut == "")
                    {
                        // not error returned , so return details.
                        strOut = GetData(rocketInterface, postInfo);
                    }
                    break;
                case "rocketmod_getsetupmenu":
                    strOut = ConfigUtils.GetSetup(rocketInterface);
                    break;
                default:
                    strOut = "Comamnd Not Found";
                    break;
            }


            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static String GetData(DNNrocketInterface rocketInterface, SimplisityInfo postInfo)
        {
            try
            {
                var strOut = "";

                strOut = "<h1> DATA to be returned</h1>";
                strOut += "<p> lkwde fpowe pofk ok wefopk wepof kpowe fop weop fd po we odkopwed opkweopd weopkdpowefodpk wpoefk</p>";

                return strOut;


                //var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                //var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                //var moduleid = postInfo.ModuleId;

                //var passSettings = postInfo.ToDictionary();
                //var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                //var objCtrl = new DNNrocketController();

                //var info = objCtrl.GetData("moduleconfig", "MDATA", DNNrocketUtils.GetEditCulture(), moduleid);
                //strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
