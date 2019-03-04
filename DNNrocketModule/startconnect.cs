using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace DNNrocketModule
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            var rocketInterface = new DNNrocketInterface(interfaceInfo);

            var controlRelPath = rocketInterface.TemplateRelPath;
            if (controlRelPath == "") controlRelPath = ControlRelPath;

            var strOut = "";

            switch (paramCmd)
            {
                case "dnnrocketconfig_getconfig":
                    strOut = ConfigUtils.GetConfig(postInfo);
                    break;
                case "dnnrocketmodule_getdata":
                    strOut = GetData(postInfo, ControlRelPath);
                    break;
                case "dnnrocketconfig_saveconfig":
                    strOut = ConfigUtils.SaveConfig(postInfo);
                    if (strOut == "")
                    {
                        // not error returned , so return details.
                        strOut = GetData(postInfo, ControlRelPath);
                    }
                    break;
                case "dnnrocketconfig_getsetupmenu":
                    strOut = ConfigUtils.GetSetup(postInfo, interfaceInfo);
                    break;
                case "dnnrocketconfig_iframe":
                    strOut = ConfigUtils.IFrame(postInfo, interfaceInfo);
                    break;
                default:
                    strOut = ConfigUtils.GetSetup(postInfo, interfaceInfo);
                    break;
            }


            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static String GetData(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";

                strOut = "<h1> DATA to be returned</h1>";
                strOut += "<p> lkwde fpowe pofk ok wefopk wepof kpowe fop weop fd po we odkopwed opkweopd weopkdpowefodpk wpoefk</p>";

                return strOut;


                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = postInfo.ModuleId;

                var passSettings = postInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();

                var info = objCtrl.GetData("moduleconfig", "MDATA", DNNrocketUtils.GetEditCulture(), moduleid);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
