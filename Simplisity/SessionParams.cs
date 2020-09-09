using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Simplisity
{
    public class SessionParams
    {
        /// <summary>
        /// Contruct the SessionParams to control things like, page, pagesize, sort order, etc.
        /// </summary>
        public SessionParams(SimplisityInfo paramInfo)
        {
            Info = new SimplisityInfo();
            Page = 1;
            //Populate any values and overwrites
            foreach (var d in paramInfo.ToDictionary())
            {
                Set(d.Key, d.Value);
            }

            BrowserSessionId = Get("browsersessionid");
            BrowserId = Get("browserid");

        }
        public string GetCommand()
        {
            return Get("s-cmd");
        }
        public void Set(string key, string value)
        {
            Info.SetXmlProperty("r/" + key, value);
        }
        public string Get(string key)
        {
            return Info.GetXmlProperty("r/" + key);
        }
        public int GetInt(string key)
        {
            return Info.GetXmlPropertyInt("r/" + key);
        }
        public SimplisityInfo Info { get; set; }

        public void ActivateItemSort(int itemid)
        {
            SortActivate = itemid;
        }
        public void CancelItemSort()
        {
            SortActivate = 0;
        }

        #region "properties"
        //Order by
        public string OrderByRef { get { return Info.GetXmlProperty("r/orderbyref"); } set { Info.SetXmlProperty("r/orderbyref", value.ToString()); } }
        public int SortActivate { get { return Info.GetXmlPropertyInt("r/sortorderactivate"); } set { Info.SetXmlProperty("r/sortorderactivate", value.ToString()); } }
        // Paging
        public int PageSize { get { return Info.GetXmlPropertyInt("r/pagesize"); } set { Info.SetXmlProperty("r/pagesize", value.ToString()); } }
        public int Page { get { return Info.GetXmlPropertyInt("r/page"); } set { Info.SetXmlProperty("r/page", value.ToString()); } }
        public int RowCount { get { return Info.GetXmlPropertyInt("r/rowcount"); } set { Info.SetXmlProperty("r/rowcount", value.ToString()); } }
        //Filter SQL
        public int FilterIndex { get { return Info.GetXmlPropertyInt("r/filterindex"); } set { Info.SetXmlProperty("r/filterindex", value.ToString()); } }
        /// <summary>
        /// Return a session data value, with an element "id" of "searchtext"
        /// </summary>
        public string SearchText { get { return Info.GetXmlProperty("r/searchtext"); } set { Info.SetXmlProperty("r/searchtext", value.ToString()); } }
        public string BrowserSessionId { get; set; }
        public string BrowserId { get; set; }
        public string Language { get { return Info.GetXmlProperty("r/language"); } set { Info.SetXmlProperty("r/language", value.ToString()); } }

        #endregion

    }

}
