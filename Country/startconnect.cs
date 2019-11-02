using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;

namespace DNNrocket.Country
{
    public class StartConnect : APInterface
    {
        private static SimplisityInfo _systemInfo;
        private static DNNrocketInterface _rocketInterface;
        private static Dictionary<string,string> _passSettings; 
        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            var commandSecurity = new CommandSecurity(-1,-1, _rocketInterface);
            commandSecurity.AddCommand("settingcountry_save", true);
            commandSecurity.AddCommand("settingcountry_get", true);
            commandSecurity.AddCommand("settingcountry_getregion", false);

            //CacheUtils.ClearAllCache();
            _systemInfo = systemInfo;
            var controlRelPath = "/DesktopModules/DNNrocket/Country";

            var rtnDic = new Dictionary<string, string>();

            if (commandSecurity.HasSecurityAccess(paramCmd))
            {
                _passSettings = paramInfo.ToDictionary();
                switch (paramCmd)
                {
                    case "settingcountry_save":
                        CountrySave(postInfo);
                        rtnDic.Add("outputhtml", CountryDetail(postInfo, controlRelPath, langRequired));
                        break;
                    case "settingcountry_get":
                        rtnDic.Add("outputhtml", CountryDetail(postInfo, controlRelPath, langRequired));
                        break;
                    case "settingcountry_getregion":
                        rtnDic.Add("outputhtml", "");
                        var regionlist = CountryUtils.RegionListCSV(paramInfo.GetXmlProperty("genxml/hidden/activevalue"), paramInfo.GetXmlPropertyBool("genxml/hidden/allowempty"));
                        rtnDic.Add("outputjson", "{listkey: [" + regionlist[0] + "], listvalue: [" + regionlist[1] + "] }");
                        break;
                    case "settingcountry_selectculturecode":
                        rtnDic.Add("outputhtml", CultureSelect(postInfo, controlRelPath, langRequired));
                        break;                        
                }
            }
            return rtnDic;
        }

        public static String CultureSelect(SimplisityInfo sInfo, string templateControlRelPath, string editlang)
        {
            try
            {
                var strOut = "";
                var objCtrl = new DNNrocketController();
                var passSettings = sInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("CultureCodeSelect.cshtml", templateControlRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture());

                var l = DNNrocketUtils.GetCultureCodeList();
                var objl = new List<object>();
                foreach (var s in l)
                {
                    objl.Add(s);
                }
                strOut = DNNrocketUtils.RazorList(razorTempl, objl, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String CountryDetail(SimplisityInfo sInfo, string templateControlRelPath, string editlang)
        {
            try
            {
                var strOut = "";
                var objCtrl = new DNNrocketController();
                var smi = objCtrl.GetData("countrysettings", "SETTINGS", editlang, -1, false, _rocketInterface.DatabaseTable);
                if (smi != null)
                {
                    var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                    if (themeFolder == "") themeFolder = _rocketInterface.DefaultTheme;
                    var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");
                    if (razortemplate == "") razortemplate = _rocketInterface.DefaultTemplate;


                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                    strOut = DNNrocketUtils.RazorDetail(razorTempl, smi, _passSettings);
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
            _passSettings.Add("saved", "true");
            var objCtrl = new DNNrocketController();
            objCtrl.SaveData("countrysettings", "SETTINGS", postInfo, -1, _rocketInterface.DatabaseTable);
        }

    }
}
