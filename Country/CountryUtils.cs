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

        public static string[] CountryListCSV(DNNrocketInterface rocketInterface, bool allowempty = true)
        {
            var rtn = new string[2];
            var countryData = new CountryData(PortalUtils.GetPortalId(), rocketInterface);
            var countrycode_csv = "";
            var countryname_csv = "";
            if (allowempty)
            {
                countrycode_csv = "'',";
                countryname_csv = "'',";
            }

            foreach (var i in countryData.GetSelectedCountries())
            {
                countrycode_csv += "'" + i.GetXmlProperty("genxml/hidden/countrycode").Replace("'", "") + "',";
                countryname_csv += "'" + DNNrocketUtils.GetCountryName(i.GetXmlProperty("genxml/hidden/countrycode")).Replace("'", "") + "',";
            }

            countryname_csv = countryname_csv.TrimEnd(',');
            countrycode_csv = countrycode_csv.TrimEnd(',');

            rtn[1] = countryname_csv;
            rtn[0] = countrycode_csv;
            return rtn;
        }

        public static string[] RegionListCSV(string countrycode, bool allowempty = true)
        {
            var rtn = new string[2];
            var code_csv = "";
            var name_csv = "";
            if (allowempty)
            {
                code_csv = "'',";
                name_csv = "'',";
            }
            foreach (var i in DNNrocketUtils.GetRegionList(countrycode))
                {
                    code_csv += "'" + i.Key.Replace("'","") + "',";
                    name_csv += "'" + i.Value.Replace("'", "") + "',";
                }
                name_csv = name_csv.TrimEnd(',');
                code_csv = code_csv.TrimEnd(',');

                rtn[0] = code_csv;
                rtn[1] = name_csv;
            return rtn;
        }

    }
}
