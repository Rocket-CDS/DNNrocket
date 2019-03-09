using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace DNNrocketAPI
{
    public static class ConfigUtils
    {

        public static bool HasConfig(int moduleId)
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetByGuidKey(-1,moduleId, "CONFIG", "moduleconfig");
            if (info == null)
            {
                return false;
            }
            else
            {
                if (info.XMLDoc.SelectNodes("genxml/*").Count <= 1) // <lang> node will be created for new record.
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static String DeleteConfig(int moduleId)
        {
            try
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetCurrentCulture(),-1, moduleId);
                objCtrl.Delete(info.ItemID);
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String SaveConfig(int moduleId, SimplisityInfo postInfo)
        {
            try
            {
                var objCtrl = new DNNrocketController();

                // find out is this is a list or single data record, nby looking for the "editlist.cshtml" template.  (editlist.cshtml will exist for lists)
                var appthemerelpath = postInfo.GetXmlProperty("genxml/appthemerelpath");
                var themeFolder = postInfo.GetXmlProperty("genxml/select/apptheme");
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editlist.cshtml", appthemerelpath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                if (razorTempl == "")
                {
                    postInfo.SetXmlProperty("genxml/checkbox/datalist", "False");
                }
                else
                {
                    postInfo.SetXmlProperty("genxml/checkbox/datalist", "True");
                }
                var info = objCtrl.SaveData("moduleconfig", "CONFIG", postInfo, -1, moduleId);

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetConfig(int moduleId, DNNrocketInterface interfaceInfo)
        {
            try
            {
                var strOut = "";
                var passSettings = interfaceInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("config.cshtml", interfaceInfo.TemplateRelPath, interfaceInfo.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();

                var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(),-1, moduleId);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }




    }
}
