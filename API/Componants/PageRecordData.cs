using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using DNNrocketAPI.Componants;
using DotNetNuke.Entities.Tabs;

namespace DNNrocketAPI
{
    public class PageRecordData
    {
        public PageRecordData(int portalId, int tabId, bool ignoreCache = false)
        {
            PortalId = portalId;
            PageId = tabId;

            var objTabs = new TabController();
            var tabInfo = objTabs.GetTab(tabId, portalId, ignoreCache);

            Record = new SimplisityRecord();
            if (tabInfo != null)
            {
                Record.ItemID = tabId;
                Record.ParentItemId = tabInfo.ParentId;
                Record.SetXmlProperty("genxml/name", tabInfo.TabName);
                Record.SetXmlProperty("genxml/title", tabInfo.Title);
                Record.SetXmlProperty("genxml/description", tabInfo.Description);
                Record.SetXmlProperty("genxml/keywords", tabInfo.KeyWords);
                Record.SetXmlProperty("genxml/url", tabInfo.Url);
                Record.SetXmlProperty("genxml/fullurl", tabInfo.FullUrl);
            }
        }

        public void Update()
        {
            var objTabs = new TabController();
            var tabInfo = objTabs.GetTab(PageId, PortalId, true);

            tabInfo.TabName = Name;
            tabInfo.Title = Title;
            tabInfo.Description = Description;
            tabInfo.KeyWords = KeyWords;

            objTabs.UpdateTab(tabInfo);
        }

        public int PortalId { get; set; }
        public int PageId { get; set; }
        public SimplisityRecord Record { get; set; }
        public string Name
        {
            get { return Record.GetXmlProperty("genxml/name"); }
            set { Record.SetXmlProperty("genxml/name", value); }
        }
        public string Title
        {
            get { return Record.GetXmlProperty("genxml/title"); }
            set { Record.SetXmlProperty("genxml/title", value); }
        }
        public string Description
        {
            get { return Record.GetXmlProperty("genxml/description"); }
            set { Record.SetXmlProperty("genxml/description", value); }
        }
        public string KeyWords
        {
            get { return Record.GetXmlProperty("genxml/keywords"); }
            set { Record.SetXmlProperty("genxml/keywords", value); }
        }
        public string Url
        {
            get { return Record.GetXmlProperty("genxml/url"); }
            set { Record.SetXmlProperty("genxml/url", value); }
        }
        public string FullUrl
        {
            get { return Record.GetXmlProperty("genxml/fullurl"); }
            set { Record.SetXmlProperty("genxml/fullurl", value); }
        }

    }
}
