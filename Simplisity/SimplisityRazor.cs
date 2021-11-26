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
        public List<object> List { get; set; }
        public SessionParams SessionParamsData { get; set; } 
        public SimplisityRazor()
        {
            Settings = new Dictionary<string, string>();
            SessionParamsData = null;
            var obj = new SimplisityInfo();
            var l = new List<object>();
            l.Add(obj);
            List = l;
            Init();
        }
        public SimplisityRazor(List<object> list, Dictionary<String, String> settings = null)
        {
            if (settings == null)
                Settings = new Dictionary<String, String>();
            else
                Settings = settings;
            List = list;
            Init();
        }
        public SimplisityRazor(object listobj, Dictionary<String, String> settings = null)
        {
            var list = new List<object>();
            list.Add(listobj);
            if (settings == null)
                Settings = new Dictionary<String, String>();
            else
                Settings = settings;
            List = list;
            Init();
        }
        private void Init()
        {
            DataObjects = new Dictionary<string, object>();
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

    }

}
