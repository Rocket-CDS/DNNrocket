using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using System.Xml;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using System.IO.Compression;
using DataProvider = DotNetNuke.Data.DataProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common;

namespace DNNrocketAPI
{
    public class PagesUtils
    {

        public static void SaveTabUrls(int tabId, string pageUrl, int portalId, string Lang)
        {
            try
            {
                //save data

                // update tab url table.
                if (pageUrl != "")
                {
                    var objTabs = new TabController();




                    if (!pageUrl.StartsWith("/")) pageUrl = "/" + pageUrl;
                    var tabInfo = objTabs.GetTab(tabId, portalId);
                    if (tabInfo != null)
                    {

                        ValidateTabUrls(tabId, pageUrl);

                        var tabUrlList = objTabs.GetTabUrls(tabId, tabInfo.PortalID);
                        TabUrlInfo tabUrlInfo = null;
                        foreach (var t in tabUrlList)
                        {
                            // Get matching langauge.
                            if (t.CultureCode == Lang && t.HttpStatus == "200")
                            {
                                tabUrlInfo = t;
                            }
                        }
                        if (tabUrlInfo == null)
                        {
                            tabUrlInfo = new TabUrlInfo();
                            tabUrlInfo.SeqNum = tabUrlList.Count + 1;
                        }
                        if (tabUrlInfo.Url != pageUrl)
                        {
                            // create to 301.
                            // Who decided that saving a taburl would trigger the creation of a 301????
                            // This is complicated and confusing for people using it.  
                            // If you add an existing 301 url, it causes a duplicate for 200 and 301.
                            tabUrlInfo.SeqNum = tabUrlList.Count + 1;
                        }

                        tabUrlInfo.TabId = tabId;
                        tabUrlInfo.HttpStatus = "200";
                        tabUrlInfo.Url = pageUrl;
                        tabUrlInfo.QueryString = "";
                        tabUrlInfo.CultureCode = Lang;
                        tabUrlInfo.IsSystem = true;
                        tabUrlInfo.PortalAliasUsage = 0;
                        objTabs.SaveTabUrl(tabUrlInfo, PortalSettings.Current.PortalId, true);

                        
                    }

                }



            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException("SaveTabUrls: Error on RocketPL", null, ex);
            }

        }

        public static Dictionary<int, string> GetParentPageNames(int portalId, int tabId, Dictionary<int, string> dictPages)
        {
            var objTabs = new TabController();
            var tabInfo = objTabs.GetTab(tabId, portalId);
            if (tabInfo != null)
            {
                dictPages.Add(tabId, tabInfo.TabName);
                var parentTabId = tabInfo.ParentId;
                if (parentTabId > 0)
                {
                    dictPages = GetParentPageNames(portalId, parentTabId, dictPages);
                }
            }
            return dictPages;
        }

        public static Dictionary<int, string> GetChildrenPageName(int portalId, int tabId, Dictionary<int, string> childDict)
        {
            var tabCollection = TabController.Instance.GetTabsByPortal(portalId).WithParentId(tabId);
            foreach (var tabInfo2 in tabCollection)
            {
                if (!childDict.ContainsKey(tabInfo2.TabID)) childDict.Add(tabInfo2.TabID, tabInfo2.TabName);
                childDict = GetChildrenPageName(portalId, tabInfo2.TabID, childDict);
            }
            return childDict;
        }




        private static void ValidateTabUrls(int tabId, string pageUrl)
        {
            try
            {
                    //save data

                    // remove the 200 status with no language, DNN will use this as the default if we don't
                    RemoveNonlang200TabUrls(tabId);

                    // remove duplicate 301
                    RemoveDuplicate301TabUrls(tabId, pageUrl);

                    // resequence taburl records
                    ResequanceTabUrls(tabId);


            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException("ValidateTabUrls: Error on NBrightPL", null, ex);
            }

        }

        private static void RemoveNonlang200TabUrls(int tabId)
        {
            if (tabId > 0)
            {
                
                var objTabs = new TabController();
                var tabUrlList = objTabs.GetTabUrls(tabId, PortalSettings.Current.PortalId);

                // remove the 200 status with no language, DNN will use this as the default if we don't
                TabUrlInfo deleteTabUrlInfo = null;
                foreach (var t in tabUrlList)
                {
                    // Get 200 without langauge.
                    if (t.CultureCode == "" && t.HttpStatus == "200")
                    {
                        deleteTabUrlInfo = t;
                    }
                }
                if (deleteTabUrlInfo != null)
                {
                    objTabs.DeleteTabUrl(deleteTabUrlInfo, PortalSettings.Current.PortalId, true);
                }
            }


        }

        private static void RemoveDuplicate301TabUrls(int tabId, string pageUrl)
        {
            if (tabId > 0)
            {
                // remove duplicate 301
                var objTabs = new TabController();
                var tabUrlList = objTabs.GetTabUrls(tabId, PortalSettings.Current.PortalId);
                var deleteList = new List<TabUrlInfo>();
                var pageurl = pageUrl;
                if (!pageurl.StartsWith("/")) pageurl = "/" + pageurl;

                foreach (var t in tabUrlList)
                {
                    if (t.HttpStatus == "301")
                    {
                        foreach (var t2 in tabUrlList)
                        {
                            if (t2.HttpStatus == "301")
                            {
                                if (t.Url == t2.Url && t.SeqNum != t2.SeqNum)
                                {
                                    deleteList.Add(t2);
                                }
                            }
                        }
                    }
                    if (t.HttpStatus == "200")
                    {
                        foreach (var t2 in tabUrlList)
                        {
                            if (t2.HttpStatus == "301" && t.Url == t2.Url)
                            {
                                deleteList.Add(t2);
                            }
                        }
                    }
                    foreach (var d in deleteList)
                    {
                        objTabs.DeleteTabUrl(d, PortalSettings.Current.PortalId, true);
                    }
                }

            }

        }

        private static void ResequanceTabUrls(int tabId)
        {
            if (tabId > 0)
            {
                // resequence taburl records
                var objTabs = new TabController();
                var tabUrlList = objTabs.GetTabUrls(tabId, PortalSettings.Current.PortalId);
                var seq = 1;
                foreach (var t in tabUrlList)
                {
                    if (t.SeqNum != seq)
                    {
                        t.SeqNum = seq;
                        objTabs.SaveTabUrl(t, PortalSettings.Current.PortalId, true);
                    }
                    seq += 1;
                }
            }
        }

        public static string GetPageURL(int tabId)
        {
            return Globals.NavigateURL(tabId);
        }
        public static string NavigateURL(int tabId, string controlKey = "", string[] param = null)
        {
            if (param == null) param = new string[1];
            param[0] = "";
            var targetUrl = Globals.NavigateURL(tabId, controlKey, param);
            return targetUrl;
        }



    }
}
