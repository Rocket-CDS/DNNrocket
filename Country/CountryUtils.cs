using DNNrocketAPI;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Componants;
using Simplisity;
using DNNrocket.Country.Componants;

namespace DNNrocket.Country
{
    public static class CountryUtils
    {

        #region "API"

        public static String CultureSelect(SimplisityInfo sInfo, string langRequired, DNNrocketInterface rocketInterface)
        {
            try
            {
                var strOut = "";
                var objCtrl = new DNNrocketController();
                var passSettings = sInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("CultureCodeSelect.cshtml", rocketInterface.TemplateRelPath, "config-w3", langRequired);

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


        public static String CountryDetail(DNNrocketInterface rocketInterface, bool saved)
        {
            try
            {
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(rocketInterface.DefaultTemplate, rocketInterface.TemplateRelPath, rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                var countryData = new CountryData(PortalUtils.GetPortalId(), rocketInterface);

                var passSettings = new Dictionary<string, string>();
                if (saved) passSettings.Add("saved", "true");

                return DNNrocketUtils.RazorDetail(razorTempl, countryData.Info, passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void CountrySave(SimplisityInfo postInfo, DNNrocketInterface rocketInterface)
        {
            var countryData = new CountryData(PortalUtils.GetPortalId(), rocketInterface);
            countryData.Save(postInfo);
        }

        #endregion

        public static string CountryListJson(DNNrocketInterface rocketInterface, bool allowempty = true)
        {
            var countryData = new CountryData(PortalUtils.GetPortalId(), rocketInterface);
            var rtn = "";
            if (allowempty)
            {
                rtn += "{'':'','':''},";
            }
            foreach (var i in countryData.GetSelectedDictCountries())
            {
                rtn += "{\"key\":\"" + i.Key.Replace("\"", "") + "\",\"name\":\"" + i.Value.Replace("\"", "") + "\"},";
            }
            rtn = rtn.TrimEnd(',');

            return rtn;
        }

        public static string RegionListJson(string countrycode, bool allowempty = true)
        {
            var rtn = "";
            if (allowempty)
            {
                rtn += "{\"\":\"\",\"\":\"\"},";
            }
            foreach (var i in DNNrocketUtils.GetRegionList(countrycode))
            {
                rtn += "{\"key\":\"" + i.Key.Replace("\"", "") + "\",\"name\":\"" + i.Value.Replace("\"", "") + "\"},";
            }
            rtn = rtn.TrimEnd(',');

            return rtn;
        }

    }
}
