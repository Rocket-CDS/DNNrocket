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
            var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(), moduleId);
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
                var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetCurrentCulture(), moduleId);
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
                var info = objCtrl.SaveData("moduleconfig", "CONFIG", postInfo, moduleId);

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

                var info = objCtrl.GetData("moduleconfig", "CONFIG", DNNrocketUtils.GetEditCulture(), moduleId);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String GetSetup(int moduleId, DNNrocketInterface interfaceInfo)
        {
            try
            {
                interfaceInfo.ModuleId = moduleId;

                var strOut = "";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", interfaceInfo.TemplateRelPath, interfaceInfo.DefaultTheme, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();
                strOut = DNNrocketUtils.RazorDetail(razorTempl, interfaceInfo.Info);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


    }
}
