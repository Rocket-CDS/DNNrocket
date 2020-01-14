using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI
{
    /// <summary>
    /// This is a simple helper class, to wrapper the settings for the razor templates.
    /// It can also be used to display what settings exist
    /// -------------------------------------------------------------------------------------------
    /// @{
    ///     var articleDataList = (ArticleDataList)Model.List.First();
    ///     var passSettings = new PassSettings();
    ///     passSettings.AddDictionary(Model.Settings);
    /// }
    /// 
    ///  @HtmlOf(passSettings.HtmlOut())
    /// -------------------------------------------------------------------------------------------
    /// 
    /// The above method is for display and debug, the settings are in the SimplisityRazor class, which is passed as the Model.
    /// 
    /// </summary>
    public class PassSettings
    {
        public PassSettings()
        {
            DictionaryData = new Dictionary<string,string>();
        }
        public PassSettings(SimplisityInfo simplisityInfo)
        {
            DictionaryData = simplisityInfo.ToDictionary();
        }


        public void AddDictionary(Dictionary<string, string> extraDictionary)
        {
            if (extraDictionary != null)
            {
                foreach (var d in extraDictionary)
                {
                    Add(d.Key, d.Value);
                }
            }
        }

        public void Add(string key, string value)
        {
            Remove(key);
            DictionaryData.Add(key, value);
        }
        public void Remove(string key)
        {
            if (DictionaryData.ContainsKey(key)) DictionaryData.Remove(key);
        }

        public string JsonOut()
        {
            var rtn = "{";
            foreach (var d in DictionaryData)
            {
                rtn += "\"" + d.Key + "\":\"" + d.Value + "\","; 
            }
            rtn = rtn.TrimEnd(',') + "}";
            return rtn;
        }
        public string HtmlOut()
        {
            var rtn = "";
            foreach (var d in DictionaryData)
            {
                rtn += "<b>" + d.Key + "</b>:&nbsp;" + d.Value + "<br/>";
            }
            return rtn;
        }


        public Dictionary<string, string> DictionaryData { get; set; }

    }
}
