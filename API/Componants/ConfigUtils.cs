using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace DNNrocketAPI
{
    public static class ConfigUtils
    {
        public static String SaveConfig(SimplisityInfo postInfo, string templateControlRelPath = "/DesktopModules/DNNrocket/API")
        {
            try
            {
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");

                var objCtrl = new DNNrocketController();
                var info = objCtrl.SaveData("moduleconfig", "MDATA", postInfo, moduleid);

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetConfig(SimplisityInfo postInfo, string templateControlRelPath = "/DesktopModules/DNNrocket/API")
        {
            try
            {
                var strOut = "";
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                if (themeFolder == "") themeFolder = "config";
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                if (razortemplate == "") razortemplate = "config.cshtml";
                var moduleid = postInfo.GetXmlPropertyInt("genxml/hidden/moduleid");

                var passSettings = postInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();

                var info = objCtrl.GetData("moduleconfig", "MDATA", DNNrocketUtils.GetEditCulture(), moduleid);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetSetup(SimplisityInfo postInfo, SimplisityInfo iInfo, string templateControlRelPath = "/DesktopModules/DNNrocket/API")
        {
            try
            {
                var strOut = "";
                var themeFolder = iInfo.GetXmlProperty("genxml/textbox/defaulttheme");
                if (themeFolder == "") themeFolder = "config";
                var razortemplate = iInfo.GetXmlProperty("genxml/textbox/defaulttemplate");
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
