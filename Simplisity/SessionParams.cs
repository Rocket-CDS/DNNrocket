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
        /// NOTE: this is not good for SEO, use url params for friendly SEO
        /// </summary>
        public SessionParams(SimplisityInfo paramInfo)
        {
            Info = new SimplisityInfo();
            Page = 1;

            foreach (var d in paramInfo.ToDictionary("hidden"))
            {
                Set(d.Key, d.Value);
            }
            foreach (var d in paramInfo.ToDictionary("settings"))
            {
                Set(d.Key, d.Value);
            }
            foreach (var d in paramInfo.ToDictionary("form"))
            {
                Set(d.Key, d.Value);
            }
            foreach (var d in paramInfo.ToDictionary("urlparams"))
            {
                Set(d.Key, d.Value);
            }

            // Do remote node last, if present then we are dealing with a remote system. (values will not be overwritten by subsequence values)
            foreach (var d in paramInfo.ToDictionary("remote"))
            {
                if (d.Key == "urlparams" || d.Key == "session")
                {
                    var nods = paramInfo.XMLDoc.SelectNodes("genxml/remote/" + d.Key + "/*");
                    if (nods != null)
                    {
                        foreach (XmlNode nod in nods)
                        {
                            Set(nod.Name, nod.InnerText);
                        }
                    }
                }
                else
                {
                    Set(d.Key, d.Value);
                }

            }

            BrowserSessionId = Get("browsersessionid");
            BrowserId = Get("browserid");

            PageDetailUrl = PageUrl; //Make sure we have detial URL in XML, for hbs.

            // allow for legacy language flag (culturecodeedit & culturecode)
            if (Info.GetXmlProperty("r/simplisity_editlanguage") != "") Info.SetXmlProperty("r/culturecodeedit", Info.GetXmlProperty("r/simplisity_editlanguage"));
            if (Info.GetXmlProperty("r/simplisity_language") != "") Info.SetXmlProperty("r/culturecode", Info.GetXmlProperty("r/simplisity_language"));
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
        public string ModuleRef { get { return Info.GetXmlProperty("r/moduleref"); } set { Info.SetXmlProperty("r/moduleref", value.ToString()); } }
        public int ModuleId { get { return Info.GetXmlPropertyInt("r/moduleid"); } set { Info.SetXmlProperty("r/moduleid", value.ToString()); } }
        public int TabId { get { return Info.GetXmlPropertyInt("r/tabid"); } set { Info.SetXmlProperty("r/tabid", value.ToString()); } }
        /// <summary>
        /// Return a session data value, with an element "id" of "searchtext"
        /// </summary>
        public string SearchText { get { return Info.GetXmlProperty("r/searchtext"); } set { Info.SetXmlProperty("r/searchtext", value.ToString()); } }
        public string BrowserSessionId { get; set; }
        public string BrowserId { get; set; }
        public string CultureCode { get { return Info.GetXmlProperty("r/culturecode"); } set { Info.SetXmlProperty("r/culturecode", value.ToString()); } }
        public string CultureCodeEdit { get { if (Info.GetXmlProperty("r/culturecodeedit") == "") return CultureCode; else return Info.GetXmlProperty("r/culturecodeedit"); } set { Info.SetXmlProperty("r/culturecodeedit", value.ToString()); } }
        public string SiteKey { get { return Info.GetXmlProperty("r/sitekey"); } set { Info.SetXmlProperty("r/sitekey", value.ToString()); } }
        public string PageUrl { get { return Info.GetXmlProperty("r/pageurl").TrimEnd('/'); } set { Info.SetXmlProperty("r/pageurl", value.ToString()); } }
        public string PageDetailUrl { get { return Info.GetXmlProperty("r/pagedetailurl").TrimEnd('/'); } set { Info.SetXmlProperty("r/pagedetailurl", value.ToString()); } }
        public string PageListUrl { get { if (Info.GetXmlProperty("r/pagelisturl") == "") return PageUrl; else return Info.GetXmlProperty("r/pagelisturl").TrimEnd('/'); } set { Info.SetXmlProperty("r/pagelisturl", value.ToString()); } }
        public string EngineURL { get { return Info.GetXmlProperty("r/engineurl").TrimEnd('/'); } set { Info.SetXmlProperty("r/engineurl", value.ToString()); } }
        public string UserHostAddress { get { return Info.GetXmlProperty("r/userhostaddress"); } set { Info.SetXmlProperty("r/userhostaddress", value.ToString()); } }
        public string ApiUrl { get { return Info.GetXmlProperty("r/apiurl"); } set { Info.SetXmlProperty("r/apiurl", value.ToString()); } }
        public bool IsMobile { get { return Info.GetXmlPropertyBool("r/ismobile"); } set { Info.SetXmlProperty("r/ismobile", value.ToString()); } }
        public string ReturnHtmlElementId { get { return Info.GetXmlProperty("r/simplisity_return"); } }
        public string Url { get { return Info.GetXmlProperty("r/url"); } set { Info.SetXmlProperty("r/url", value.ToString()); } }
        public string UrlFriendly { get { return Info.GetXmlProperty("r/urlfriendly"); } set { Info.SetXmlProperty("r/urlfriendly", value.ToString()); } }
        public string UrlReferrer { get { return Info.GetXmlProperty("r/urlreferrer"); } set { Info.SetXmlProperty("r/urlreferrer", value.ToString()); } }

        #endregion

    }

}
