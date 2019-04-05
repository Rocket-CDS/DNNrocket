using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Simplisity
{
    public class SimplisityRazor
    {
        public Dictionary<String, String> Settings { get; set; }
        public NameValueCollection UrlParams { get; set; }
        public List<object> List { get; set; }
        public int ModuleId { get; set; }
        public String ModuleRef { get; set; }
        public int ModuleIdDataSource { get; set; }

        public String FullTemplateName { get; set; }
        public String TemplateName { get; set; }
        public String ThemeFolder { get; set; }

        public String Lang { get; set; }

        public SimplisityInfo HeaderData { get; set; }

        public SimplisityRazor(List<object> list, Dictionary<String, String> settings, NameValueCollection urlParams)
        {
            Settings = settings;
            UrlParams = urlParams;
            List = list;

            ModuleRef = "";
            ModuleId = 0;
            ModuleIdDataSource = 0;

            if (settings.ContainsKey("modref")) ModuleRef = settings["modref"];
            if (settings.ContainsKey("moduleid") && GeneralUtils.IsNumeric(settings["moduleid"]))
            {
                ModuleId = Convert.ToInt32(settings["moduleid"]);
                ModuleIdDataSource = ModuleId;
            }
            if (settings.ContainsKey("moduleiddatasource") && !String.IsNullOrWhiteSpace(settings["moduleiddatasource"]))
            {
                ModuleIdDataSource = Convert.ToInt32(settings["moduleiddatasource"]);
            }

        }

        public SimplisityRazor()
        {
            Settings = new Dictionary<string, string>();
            HeaderData = new SimplisityInfo();
            var obj = new SimplisityInfo();
            var l = new List<object>();
            l.Add(obj);
            List = l;
        }

        public SimplisityRazor(List<object> list, Dictionary<String, String> settings)
        {
            Settings = settings;
            UrlParams = new NameValueCollection();
            List = list;
        }
        public SimplisityRazor(List<object> list, NameValueCollection urlParams)
        {
            Settings = new Dictionary<string, string>();
            UrlParams = urlParams;
            List = list;
        }

        public String GetSetting(String key, String defaultValue = "")
        {
            if (Settings.ContainsKey(key)) return Settings[key];
            return defaultValue;
        }

        public Boolean GetSettingBool(String key, Boolean defaultValue = false)
        {
            try
            {
                if (Settings.ContainsKey(key))
                {
                    var x = Settings[key];
                    // bool usually stored as "True" "False"
                    if (x.ToLower() == "true") return true;
                    // Test for 1 as true also.
                    if (GeneralUtils.IsNumeric(x))
                    {
                        if (Convert.ToInt32(x) > 0) return true;
                    }
                    return false;
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }


        public int GetSettingInt(String key, int defaultValue = -1)
        {
            if (Settings.ContainsKey(key))
            {
                var s = Settings[key];
                if (GeneralUtils.IsNumeric(s)) return Convert.ToInt32(s);
            }
            return defaultValue;
        }

        public String GetUrlParam(String key, String defaultValue = "")
        {
            var result = defaultValue;
            if (UrlParams.Count != 0)
            {
                result = Convert.ToString(UrlParams[key]);
            }
            return (result == null) ? defaultValue : result.Trim();
        }

    }

}
