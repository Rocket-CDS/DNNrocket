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
            var controlRelPath = "/DesktopModules/DNNrocket/DNNrocketModule";
            var strOut = "";

            switch (paramCmd)
            {
                case "dnnrocketmodule_getconfig":
                    strOut = GetConfig(postInfo, ControlRelPath);
                    break;
                case "dnnrocketmodule_getdata":
                    strOut = "<h1> DATA to be returned</h1>";
                    strOut += "<p> lkwde fpowe pofk ok wefopk wepof kpowe fop weop fd po we odkopwed opkweopd weopkdpowefodpk wpoefk</p>";
                    break;
                default:
                    strOut = GetSetup(postInfo, interfaceInfo, ControlRelPath);
                    break;
            }


            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static String GetConfig(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = postInfo.ModuleId;

                var passSettings = postInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();

                var info = objCtrl.GetData("moduleconfig", "MDATA", DNNrocketUtils.GetEditCulture(),moduleid);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetSetup(SimplisityInfo postInfo, SimplisityInfo iInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var themeFolder = iInfo.GetXmlProperty("genxml/hidden/theme");
                if (themeFolder == "") themeFolder = "config-w3";
                var razortemplate = iInfo.GetXmlProperty("genxml/hidden/template");
                if (razortemplate == "") razortemplate = "setup.cshtml";
                iInfo.ModuleId = postInfo.ModuleId;


                var passSettings = iInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();
                strOut = DNNrocketUtils.RazorDetail(razorTempl, new SimplisityInfo(), passSettings, iInfo, true);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
