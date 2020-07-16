using DNNrocketAPI;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Componants;
using Simplisity;
using DNNrocket.Country.Componants;
using Newtonsoft.Json;

namespace DNNrocket.Country
{
    public static class CountryUtils
    {

        #region "API"

        public static String CultureSelect(SimplisityInfo sInfo, string langRequired, RocketInterface rocketInterface)
        {
            try
            {
                var strOut = "";
                var objCtrl = new DNNrocketController();
                var passSettings = sInfo.ToDictionary();
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("CultureCodeSelect.cshtml", rocketInterface.TemplateRelPath, "config-w3", langRequired);

                var l = DNNrocketUtils.GetCultureCodeList();
                var objl = new List<object>();
                foreach (var s in l)
                {
                    objl.Add(s);
                }
                strOut = RenderRazorUtils.RazorList(razorTempl, objl, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String CountryDetail(RocketInterface rocketInterface, bool saved)
        {
            try
            {
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(rocketInterface.DefaultTemplate, rocketInterface.TemplateRelPath, rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                var countryData = new CountryLimpet(PortalUtils.GetPortalId(), rocketInterface);

                var passSettings = new Dictionary<string, string>();
                if (saved) passSettings.Add("saved", "true");

                return RenderRazorUtils.RazorDetail(razorTempl, countryData.Info, passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void CountrySave(SimplisityInfo postInfo, RocketInterface rocketInterface)
        {
            var countryData = new CountryLimpet(PortalUtils.GetPortalId(), rocketInterface);
            countryData.Save(postInfo);
        }

        #endregion

        public static string CountryListJson(RocketInterface rocketInterface, bool allowempty = true)
        {
            var countryData = new CountryLimpet(PortalUtils.GetPortalId(), rocketInterface);
            var jsonList = new List<ValuePair>();
            var valuePair = new ValuePair();
            if (allowempty)
            {
                valuePair.Key = "";
                valuePair.Value = "";
                jsonList.Add(valuePair);
            }
            foreach (var i in countryData.GetSelectedDictCountries())
            {
                valuePair.Key = i.Key;
                valuePair.Value = i.Value;
                jsonList.Add(valuePair);
            }
            string result = JsonConvert.SerializeObject(jsonList);
            return result;
        }

        public static object RegionListJson(string countrycode, bool allowempty = true)
        {
            var jsonList = new List<ValuePair>();
            var valuePair = new ValuePair();
            if (allowempty)
            {
                valuePair.Key = "";
                valuePair.Value = "";
                jsonList.Add(valuePair);
            }
            foreach (var i in DNNrocketUtils.GetRegionList(countrycode))
            {
                valuePair = new ValuePair();
                valuePair.Key = i.Key;
                valuePair.Value = i.Value;
                jsonList.Add(valuePair);
            }
            return jsonList;
        }

    }
}
