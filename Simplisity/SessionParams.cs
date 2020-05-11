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

        public string ViewStateOut()
        {
            string jsonText = "{";
            var nodList = Info.XMLDoc.SelectNodes("r/*");
            if (nodList != null)
            {
                foreach (XmlNode nod in nodList)
                {
                    jsonText += "'" + nod.Name + "':'" + nod.InnerText + "',";
                }
                jsonText = jsonText.TrimEnd(',');
            }
            jsonText += "}";
            return GeneralUtils.EnCode(jsonText);
        }

        #region "properties"
        //Order by
        public int OrderByIndex { get { return Info.GetXmlPropertyInt("r/orderbyindex"); } set { Info.SetXmlProperty("r/orderbyindex", value.ToString()); } }
        public int SortActivate { get { return Info.GetXmlPropertyInt("r/sortorderactivate"); } set { Info.SetXmlProperty("r/sortorderactivate", value.ToString()); } }
        // Paging
        public int PageSize { get { return Info.GetXmlPropertyInt("r/pagesize"); } set { Info.SetXmlProperty("r/pagesize", value.ToString()); } }
        public int Page { get { return Info.GetXmlPropertyInt("r/page"); } set { Info.SetXmlProperty("r/page", value.ToString()); } }
        public int RowCount { get { return Info.GetXmlPropertyInt("r/rowcount"); } set { Info.SetXmlProperty("r/rowcount", value.ToString()); } }
        //Filter SQL
        public int FilterIndex { get { return Info.GetXmlPropertyInt("r/filterindex"); } set { Info.SetXmlProperty("r/filterindex", value.ToString()); } }

        #endregion

    }

}
