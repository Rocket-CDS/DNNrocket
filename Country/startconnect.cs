using DNNrocketAPI;
using Simplisity;
using System;

namespace DNNrocket.Country
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _systemprovider;

        public override string ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo sInfo, string userHostAddress, string editlang = "")
        {

            //CacheUtils.ClearAllCache();
            _systemprovider = sInfo.GetXmlProperty("genxml/systemprovider");

            var controlRelPath = "/DesktopModules/DNNrocket/Country";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [settingcountry]";

            switch (paramCmd)
            {
                case "settingcountry_get":
                    strOut = CountryDetail(sInfo, controlRelPath, editlang);
                    break;
                case "settingcountry_save":
                    CountrySave(sInfo);
                    strOut = CountryDetail(sInfo, controlRelPath, editlang);
                    break;
            }
            return strOut;

        }

        public static String CountryDetail(SimplisityInfo sInfo, string templateControlRelPath, string editlang)
        {
            try
            {
                var strOut = "";
                var objCtrl = new DNNrocketController();
                var smi = objCtrl.GetData("countrysettings", _systemprovider + "_SETTINGS", editlang);
                if (smi != null)
                {

                    var selecteditemid = sInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                    var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                    var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                    var passSettings = sInfo.ToDictionary();

                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                    strOut = DNNrocketUtils.RazorDetail(razorTempl, smi, passSettings);
                }
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void CountrySave(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            objCtrl.SaveData("countrysettings", _systemprovider + "_SETTINGS", postInfo);
        }

    }
}
