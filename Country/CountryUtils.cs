using DNNrocketAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocket.Country
{
    public static class CountryUtils
    {
        public static string[] CountryListCSV(string systemprovider)
        {
            var rtn = new string[2];
            var objCtrl = new DNNrocketController();
            var smi = objCtrl.GetData("countrysettings", systemprovider + "_SETTINGS", DNNrocketUtils.GetEditCulture());
            var countrycode_csv = "";
            var countryname_csv = "";
            if (smi != null)
            {
                foreach (var i in smi.GetList("countrylist"))
                {
                    countrycode_csv += "'" + i.GetXmlProperty("genxml/hidden/countrycode").Replace("'", "") + "',";
                    countryname_csv += "'" + DNNrocketUtils.GetCountryName(i.GetXmlProperty("genxml/hidden/countrycode")).Replace("'", "") + "',";
                }
                countryname_csv = countryname_csv.TrimEnd(',');
                countrycode_csv = countrycode_csv.TrimEnd(',');

                rtn[1] = countryname_csv;
                rtn[0] = countrycode_csv;
            }
            return rtn;
        }

        public static string[] RegionListCSV(string countrycode)
        {
            var rtn = new string[2];
            var code_csv = "";
            var name_csv = "";
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
