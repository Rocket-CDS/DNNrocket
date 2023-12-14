using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Common.Utilities.Internal;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;

namespace RocketTools.Components
{
    public static class RocketToolsUtils
    {
        public static Dictionary<int, string> GetTreeTabList(bool showAllTabs = false)
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, DNNrocketUtils.GetCurrentCulture(), true);
            var rtnList = new Dictionary<int, string>();
            return GetTreeTabList(rtnList, tabList, 0, 0, "", showAllTabs);
        }
        private static Dictionary<int, string> GetTreeTabList(Dictionary<int, string> rtnList, List<TabInfo> tabList, int level, int parentid, string prefix = "", bool showAllTabs = false)
        {

            if (level > 20) // stop infinate loop
            {
                return rtnList;
            }
            if (parentid > 0) prefix += "..";
            foreach (TabInfo tInfo in tabList)
            {
                var parenttestid = tInfo.ParentId;
                if (parenttestid < 0) parenttestid = 0;
                if (parentid == parenttestid)
                {
                    if (!tInfo.IsDeleted && (tInfo.TabPermissions.Count > 2 || showAllTabs))
                    {
                        rtnList.Add(tInfo.TabID, prefix + "" + tInfo.TabName);
                        GetTreeTabList(rtnList, tabList, level + 1, tInfo.TabID, prefix);
                    }
                }
            }

            return rtnList;
        }
        [Obsolete("use ModuleUtils. ")]
        public static Dictionary<int, string> GetTabModuleTitles(int tabid, bool getDeleted = false)
        {
            return ModuleUtils.GetTabModuleTitles(tabid, getDeleted);
        }
        [Obsolete("use ModuleUtils. ")]
        public static void UpdateModuleTitle(int tabid, int moduleid, string title)
        {
            ModuleUtils.UpdateModuleTitle(tabid, moduleid, title);
        }
        [Obsolete("use ModuleUtils. ")]
        public static bool ModuleIsDeleted(int tabid, int moduleid)
        {
            return ModuleUtils.ModuleIsDeleted(tabid, moduleid);
        }
        [Obsolete("use ModuleUtils. ")]
        public static bool ModuleExists(int tabid, int moduleid)
        {
            return ModuleUtils.ModuleExists(tabid, moduleid);
        }
        [Obsolete("use ModuleUtils. ")]
        public static int GetModuleTabId(Guid uniqueId)
        {
            return ModuleUtils.GetModuleTabId(uniqueId);
        }
        [Obsolete("use ModuleUtils. ")]
        public static ModuleInfo GetModuleInfo(int tabid, int moduleid)
        {
            return ModuleUtils.GetModuleInfo(tabid, moduleid);
        }
        public static void ValidateMeta()
        {
            var objCtrl = new DNNrocketController();
            var list = objCtrl.GetList(PortalSettings.Current.PortalId, -1, "PL");
            foreach (var pl in list)
            {
                var dataRecord = objCtrl.GetRecord(pl.ItemID);
                // Update Meta data for DNN Tab DB, if the meta is empty DNN will not render the meta data.
                var controller = new TabController();
                var newTab = controller.GetTab(dataRecord.SortOrder, PortalUtils.GetPortalId());
                if (newTab.Description == "") newTab.Description = ".";
                if (newTab.KeyWords == "") newTab.KeyWords = ".";
                controller.UpdateTab(newTab);
            }
        }
        public static void ValidateUrls()
        {
            var objCtrl = new DNNrocketController();
            var list = objCtrl.GetList(PortalSettings.Current.PortalId, -1, "PL");
            foreach (var pl in list)
            {
                var dataRecord = objCtrl.GetRecord(pl.ItemID);
                var tabData = new TabData(dataRecord);
                ValidateTabUrls(tabData);
            }
            ValidateMeta();
        }
        public static void ValidateTabUrls(TabData tabData)
        {
            if (tabData != null)
            {
                // remove the 200 status with no language, DNN will use this as the default if we don't
                RemoveNonlang200TabUrls(tabData);
                // remove duplicate 301
                RemoveDuplicate301TabUrls(tabData);
                // resequence taburl records
                ResequanceTabUrls(tabData);
            }
        }
        private static void RemoveNonlang200TabUrls(TabData tabData)
        {
            var objTabs = new TabController();
            var tabUrlList = objTabs.GetTabUrls(tabData.TabId, PortalSettings.Current.PortalId);

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
        private static void RemoveDuplicate301TabUrls(TabData tabData)
        {
            // remove duplicate 301
            var objTabs = new TabController();
            var tabUrlList = objTabs.GetTabUrls(tabData.TabId, PortalSettings.Current.PortalId);
            var deleteList = new List<TabUrlInfo>();
            var pageurl = tabData.PageUrl;
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
        private static void ResequanceTabUrls(TabData tabData)
        {
            // resequence taburl records
            var objTabs = new TabController();
            var tabUrlList = objTabs.GetTabUrls(tabData.TabId, PortalSettings.Current.PortalId);
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

        public static void DeleteTabUrl(int portalId, int tabId, int seqNum)
        {
            if (seqNum > 0)
            {
                var controller = new TabController();
                var l = controller.GetTabUrls(tabId, portalId);
                foreach (TabUrlInfo tInfo in l)
                {
                    if (tInfo.SeqNum == seqNum)
                    {
                        controller.DeleteTabUrl(tInfo, portalId, true);
                    }
                }
            }
        }

        #region "DNN cache"

        public static void SetCache(string cacheKey, object objObject)
        {
            SetCache(cacheKey, objObject, 2);
        }
        public static void SetCache(string cacheKey, object objObject, int keephours)
        {
            DataCache.SetCache(cacheKey, objObject, DateTime.Now + new TimeSpan(0, keephours, 0, 0));
        }
        public static object GetCache(string cacheKey)
        {
            return DataCache.GetCache(cacheKey);
        }
        public static void RemoveCache(string cacheKey)
        {
            DataCache.RemoveCache(cacheKey);
        }
        public static void ClearAllCache()
        {
            DataCache.ClearCache();
        }
        public static void ClearPortalCache()
        {
            DataCache.ClearPortalCache(PortalSettings.Current.PortalId, true);
        }

        public static void ClearPortalCache(int portalId)
        {
            DataCache.ClearPortalCache(portalId, true);
        }

        /// <summary>
        /// Synchronizes the module content between cache and database. + Update ModifiedContentDate
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        public static void SynchronizeModule(int moduleId)
        {
            ModuleController.SynchronizeModule(moduleId);
        }
        public static string GetPortalDefaultAlias(int portalId, string cultureCode)
        {
            var portalalias = PortalSettings.Current.DefaultPortalAlias;
            var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DotNetNuke.Data.DataProvider.Instance().GetPortalAliases());
            var rtnList = new List<string>();
            foreach (var pa in padic)
            {
                if (pa.Value.PortalID == portalId)
                {
                    if (pa.Value.IsPrimary)
                    {
                        if (cultureCode == "" || pa.Value.CultureCode == cultureCode)
                        {
                            portalalias = pa.Key;
                        }
                    }
                }
            }
            return portalalias;
        }


        #endregion

    }
}