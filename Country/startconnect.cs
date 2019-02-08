using DNNrocketAPI;
using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;

namespace DNNrocket.Country
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _systemprovider;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo sInfo, string userHostAddress, string editlang = "")
        {

            //CacheUtils.ClearAllCache();
            _systemprovider = sInfo.GetXmlProperty("genxml/systemprovider");

            var controlRelPath = "/DesktopModules/DNNrocket/Country";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [settingcountry]";

            var rtnDic = new Dictionary<string, string>();

            switch (paramCmd)
            {
                case "settingcountry_get":
                    rtnDic.Add("outputhtml", CountryDetail(sInfo, controlRelPath, editlang));
                    break;
                case "settingcountry_save":
                    CountrySave(sInfo);
                    rtnDic.Add("outputhtml", CountryDetail(sInfo, controlRelPath, editlang));
                    break;
                case "settingcountry_getregion":
                    rtnDic.Add("outputhtml", "");
                    var regionlist = CountryUtils.RegionListCSV(GeneralUtils.DeCode(sInfo.GetXmlProperty("genxml/hidden/activevalue")));
                    rtnDic.Add("outputjson", "{listkey: [" + regionlist[0] + "], listvalue: [" + regionlist[1] + "] }");
                    break;
            }

            return rtnDic;
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
