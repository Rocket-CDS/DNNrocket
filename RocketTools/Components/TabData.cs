using System;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using Simplisity;

namespace RocketTools.Components
{
    public class TabData
    {
        public SimplisityRecord DataRecord;
        public TabInfo TabInfo;

        /// <summary>
        /// Populate the class
        /// </summary>
        /// <param name="tabId">categoryId (use -1 to create new record)</param>
        /// <param name="lang"> </param>
        public TabData(SimplisityRecord dataRecord)
        {
            DataRecord = dataRecord;
        }

        #region "public functions/interface"

        public String PageName
        {
            get { return DataRecord.GetXmlProperty("genxml/textbox/pagename"); }
        }

        public String PageTitle
        {
            get { return DataRecord.GetXmlProperty("genxml/textbox/pagetitle"); }
        }

        public String TagWords
        {
            get { return DataRecord.GetXmlProperty("genxml/textbox/tagwords"); }
        }

        public String PageDescription
        {
            get { return DataRecord.GetXmlProperty("genxml/textbox/pagedescription"); }
        }
        public bool EditPageUrl
        {
            get { return DataRecord.GetXmlPropertyBool("genxml/checkbox/editpageurl"); }
        }
        public String PageUrl
        {
            get { return DataRecord.GetXmlProperty("genxml/textbox/pageurl"); }
            set { DataRecord.SetXmlProperty("genxml/textbox/pageurl", value); }
        }
        public int TabId
        {
            get { if (GeneralUtils.IsNumeric(DataRecord.SortOrder)) return Convert.ToInt32(DataRecord.SortOrder); return -1; }
        }

        public void Save()
        {
            // Ensure the pageurl is correct format.
            if (EditPageUrl)
                PageUrl = PageUrl;
            else
                PageUrl = GetPageUrl();

            var objCtrl = new DNNrocketController();
            objCtrl.Update(DataRecord);
            var menucachekey = "RocketTools*" + PortalSettings.Current.PortalId + "*" + DataRecord.Lang;
            CacheUtils.RemoveCache(menucachekey);
            var cachekey = "RocketTools*hreflang*" + PortalSettings.Current.PortalId + "*" + DataRecord.Lang + "*" + DataRecord.GUIDKey;
            CacheUtils.RemoveCache(cachekey);
        }
        public int Validate()
        {
            var errorcount = 0;
          
            return errorcount;
        }

        #endregion



        #region " private functions"
        private string GetPageUrl()
        {
            if (TabId <= 0) return "";
            var portalid = PortalSettings.Current.PortalId;
            var objTabCtrl = new TabController();
            var dnnTab = objTabCtrl.GetTab(TabId, portalid);
            var objCtrl = new DNNrocketController();
            var rtnUrl = "";
            var lp = 0;
            while (dnnTab != null && dnnTab.TabID > 0 && lp < 20)
            {
                var guidkey = "PL_" + DataRecord.Lang + "_" + dnnTab.TabID;
                var plTabData = objCtrl.GetRecordByGuidKey(PortalUtils.GetPortalId(), -1, "PL", guidkey);
                var pagename = dnnTab.TabName;
                if (plTabData != null)
                {
                    var pname = plTabData.GetXmlProperty("genxml/textbox/pagename");
                    if (!String.IsNullOrWhiteSpace(pname)) pagename = pname;
                }
                rtnUrl = "/" + GeneralUtils.UrlFriendly(pagename) + rtnUrl;
                if (dnnTab.ParentId > 0)
                    dnnTab = objTabCtrl.GetTab(dnnTab.ParentId, portalid);
                else
                    dnnTab = null;
                lp += 1;
            }
            return rtnUrl;
        }

        #endregion
    }
}
