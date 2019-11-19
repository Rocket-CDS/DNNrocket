using RocketSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketMod
{
    public static class LocalUtils
    {

        public static Dictionary<string,string> SettingsToDictionary(SettingsData settingsData)
        {
            var rtn = new Dictionary<string, string>();
            if (settingsData == null) return rtn;

            foreach (var s in settingsData.List)
            {
                var name = s.GetXmlProperty("genxml/textbox/name");
                if (!rtn.ContainsKey(name))
                {
                    var value = s.GetXmlProperty("genxml/lang/genxml/textbox/value");
                    if (value == "") value = s.GetXmlProperty("genxml/textbox/value");
                    if (!rtn.ContainsKey(name))  rtn.Add(name, value);
                }
            }
            return rtn;
        }


    }
}
