using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Simplisity
{
    public class SimplisityRazor
    {
        public Dictionary<String, object> DataObjects { get; set; }
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

        public int TabId { get; set; }

        public SimplisityInfo Datainfo { get; set; }
        public SimplisityInfo HeaderData { get; set; }

        public SimplisityRazor(List<object> list, Dictionary<String, String> settings, NameValueCollection urlParams)
        {
            Settings = settings;
            UrlParams = urlParams;
            List = list;

            DataObjects = new Dictionary<string, object>();

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

            if (settings.ContainsKey("tabid") && GeneralUtils.IsNumeric(settings["tabid"])) TabId = Convert.ToInt32(settings["tabid"]);

        }

        public SimplisityRazor()
        {
            Settings = new Dictionary<string, string>();
            DataObjects = new Dictionary<string, object>();  
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

        public void SetSetting(String key, String value)
        {
            if (Settings == null) Settings = new Dictionary<String, String>();
            if (Settings.ContainsKey(key)) Settings.Remove(key);
            Settings.Add(key, value);
        }

        public String GetSetting(String key, String defaultValue = "")
        {
            if (Settings != null && Settings.ContainsKey(key)) return Settings[key];
            return defaultValue;
        }
        public void SetDataObject(String key, object value)
        {
            if (DataObjects == null) DataObjects = new Dictionary<string, object>(); // could be null if initiated from within razor template to pass data.
            if (DataObjects.ContainsKey(key)) DataObjects.Remove(key);
            DataObjects.Add(key, value);
        }
        public object GetDataObject(String key)
        {
            if (DataObjects != null && DataObjects.ContainsKey(key)) return DataObjects[key];
            return null;
        }

        public Boolean GetSettingBool(String key, Boolean defaultValue = false)
        {
            try
            {
                if (Settings == null) Settings = new Dictionary<String, String>();
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
                var ms = ex.ToString();
                return defaultValue;
            }
        }


        public int GetSettingInt(String key, int defaultValue = -1)
        {
            if (Settings == null) Settings = new Dictionary<String, String>();
            if (Settings.ContainsKey(key))
            {
                var s = Settings[key];
                if (GeneralUtils.IsNumeric(s)) return Convert.ToInt32(s);
            }
            return defaultValue;
        }

        public String GetUrlParam(String key, String defaultValue = "")
        {
            if (UrlParams == null) return "";
            var result = defaultValue;
            if (UrlParams.Count != 0)
            {
                result = Convert.ToString(UrlParams[key]);
            }
            return (result == null) ? defaultValue : result.Trim();
        }

    }

}
