using DNNrocketAPI;
using Simplisity;
using System;

namespace DNNrocket.Country
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        public override string ProcessCommand(string paramCmd, SimplisityInfo sInfo, string userHostAddress, string editlang = "")
        {

            //CacheUtils.ClearAllCache();

            var controlRelPath = "/DesktopModules/DNNrocket/Country";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [settingcountry]";

            switch (paramCmd)
            {
                case "settingcountry_get":
                    strOut = CountryDetail(sInfo, controlRelPath);
                    break;
                case "settingcountry_save":
                    CountrySave(sInfo);
                    strOut = CountryDetail(sInfo, controlRelPath);
                    break;
            }
            return strOut;

        }

        public static String CountryDetail(SimplisityInfo sInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var selecteditemid = sInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");
                
                var passSettings = sInfo.ToDictionary();


                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                strOut = DNNrocketUtils.RazorDetail(razorTempl, new SimplisityInfo(), passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void CountrySave(SimplisityInfo sInfo)
        {
            var objCtrl = new DNNrocketController();
            var nbi = objCtrl.GetSinglePageData("countrysettings", "SETTINGS", "");

            if (nbi == null)
            {
                nbi = new SimplisityInfo();
                nbi.TypeCode = "";
                nbi.GUIDKey = "";
                nbi.ItemID = -1;
            }
            nbi.XMLData = sInfo.XMLData;
            objCtrl.SaveSinglePageData("countrysettings", "SETTINGS", "", nbi);

        }

    }
}
