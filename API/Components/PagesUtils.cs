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
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Web.DDRMenu;
using RazorEngine;
using System.Web.UI.WebControls;
using Simplisity;
using DotNetNuke.Abstractions.Portals;

namespace DNNrocketAPI.Components
{
    public static class PagesUtils
    {

        public static void SaveTabUrls(int tabId, string pageUrl, int portalId, string Lang)
        {
            try
            {
                //save data

                // update tab url table.
                if (pageUrl != "" & tabId > 0)
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
            if (tabId <= 0) return dictPages;

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
            if (tabId <= 0) return childDict;

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
                if (tabId > 0)
                {
                    //save data

                    // remove the 200 status with no language, DNN will use this as the default if we don't
                    RemoveNonlang200TabUrls(tabId);

                    // remove duplicate 301
                    RemoveDuplicate301TabUrls(tabId, pageUrl);

                    // resequence taburl records
                    ResequanceTabUrls(tabId);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException("ValidateTabUrls: Error on DNNrocket PageUtils", null, ex);
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
            if (tabId <= 0) return "";
            return DNNrocketUtils.NavigateURL(tabId);
        }
        public static int GetPageByTabPath(int portalId, string tabPath)
        {
            return TabController.GetTabByTabPath(portalId, tabPath, string.Empty);
        }
        public static int GetHomePage(int portalId, string cultureCode)
        {
            var l = TabController.GetTabPathDictionary(portalId, cultureCode);
            return l.First().Value;
        }
        public static bool PageExists(int portalId, int tabId = 0)
        {
            var controller = new TabController();
            var tab = controller.GetTab(tabId, portalId);
            if (tab == null)
                return false;
            else
                if (tab.IsDeleted) return false;
            return true;
        }
        public static bool PageExists(int portalId, string pageName, int parentId = 0)
        {
            var controller = new TabController();
            var tab = controller.GetTabByName(pageName, portalId);
            if (tab == null)
                return false;
            else
                if (tab.IsDeleted) return false;
            return true;
        }
        public static string GetPageName(int tabId, int portalId)
        {
            if (tabId <= 0) return "";
            var controller = new TabController();
            var tab = controller.GetTab(tabId, portalId);
            if (tab == null)
                return "";
            else
                if (tab.IsDeleted) return "";
            return tab.TabName;
        }
        public static int CreatePage(int portalId, string pageName, bool IsVisible = true,bool DisableLink = false, int parentId = 0)
        {
            var controller = new TabController();
            var tab = controller.GetTabByName(pageName, portalId);
            if (tab == null)
            {
                var newTab = new TabInfo { TabID = Null.NullInteger, ParentId = Null.NullInteger, TabName = pageName };
                // set new page properties
                newTab.PortalID = portalId; // the portal you want the page created on
                newTab.TabName = pageName; // the new page name
                newTab.Title = ""; // the new page title
                newTab.Description = ""; // the new page description
                newTab.KeyWords = ""; // the new page keywords (used for meta keywords and search)
                newTab.IsDeleted = false; // whether it is deleted - always false for new pages
                newTab.IsSuperTab = false; // whether it should only be accessible by superusers
                newTab.IsVisible = true; // whether it is visible
                newTab.DisableLink = false; // whether it has a menu link
                newTab.IconFile = ""; // the image file used in the menu                
                newTab.Url = ""; // if the page is a redirection to a URL such as another page or an external site
                if (parentId > 0) newTab.ParentId = parentId;

                return controller.AddTab(newTab);
            }
            if (tab.IsDeleted)
            {
                var portalSettings = PortalUtils.GetPortalSettings(portalId);
                controller.RestoreTab(tab, portalSettings);
            }
            return tab.TabID;
        }
        public static void AddPagePermissions(int portalId, int pageId, string roleName)
        {
            if (pageId > 0)
            {

                TabInfo newTab = TabController.Instance.GetTab(pageId, portalId, false);
                if (newTab != null)
                {
                    TabPermissionCollection newPermissions = newTab.TabPermissions;

                    var roleid = -1;
                    if (roleName != "")
                    {
                        var role = RoleController.Instance.GetRole(portalId, r => r.RoleName == roleName);
                        if (role != null) roleid = role.RoleID;
                    }
                    //Add permission to the page so that all users can view it
                    foreach (PermissionInfo p in PermissionController.GetPermissionsByTab())
                    {
                        if (p.PermissionKey == "VIEW")
                        {
                            var objPermission = new TabPermissionInfo(p);
                            objPermission.TabID = pageId;
                            objPermission.RoleID = roleid;
                            objPermission.RoleName = roleName;
                            objPermission.AllowAccess = true;
                            //objPermission.UserID = ??;
                            objPermission.DisplayName = roleName;

                            bool canAdd = !newTab.TabPermissions.Cast<TabPermissionInfo>()
                      .Any(tp => tp.TabID == objPermission.TabID
                                 && tp.PermissionID == objPermission.PermissionID
                                 && tp.RoleID == objPermission.RoleID
                                 && tp.UserID == objPermission.UserID);

                            if (canAdd) newTab.TabPermissions.Add(objPermission);
                        }
                    }

                    TabController.Instance.UpdateTab(newTab);
                    TabController.Instance.ClearCache(portalId);
                }
            }
        }
        public static void RemovePagePermissions(int portalId, int pageId, string roleName)
        {
            if (pageId > 0)
            {

                TabInfo newTab = TabController.Instance.GetTab(pageId, portalId, false);
                if (newTab != null)
                {
                    TabPermissionCollection newPermissions = newTab.TabPermissions;

                    var roleid = int.Parse(Globals.glbRoleAllUsers);
                    if (roleName != "")
                    {
                        var role = RoleController.Instance.GetRole(portalId, r => r.RoleName == roleName);
                        if (role != null)
                        {
                            roleid = role.RoleID;
                            LogUtils.LogSystem("RoleId to Remove: " + roleid);
                            foreach (PermissionInfo p in PermissionController.GetPermissionsByTab())
                            {
                                if (p.PermissionKey == "VIEW")
                                {
                                    var tabPub = new TabPublishingController();

                                    var existPermission = GetAlreadyPermission(newTab, "VIEW", roleid);
                                    if (existPermission != null)
                                    {
                                        LogUtils.LogSystem("Remove: " + roleid);
                                        newTab.TabPermissions.Remove(existPermission);
                                    }
                                }
                            }
                            TabController.Instance.UpdateTab(newTab);
                            TabController.Instance.ClearCache(portalId);
                        }
                    }
                }
            }
        }
        public static void RemoveAllUsersPagePermissions(int portalId, int pageId)
        {
            if (pageId > 0)
            {

                TabInfo newTab = TabController.Instance.GetTab(pageId, portalId, false);
                if (newTab != null)
                {
                    TabPermissionCollection newPermissions = newTab.TabPermissions;

                    var roleid = int.Parse(Globals.glbRoleAllUsers);
                    foreach (PermissionInfo p in PermissionController.GetPermissionsByTab())
                    {
                        if (p.PermissionKey == "VIEW")
                        {
                            var tabPub = new TabPublishingController();

                            var existPermission = GetAlreadyPermission(newTab, "VIEW", roleid);
                            if (existPermission != null)
                            {
                                newTab.TabPermissions.Remove(existPermission);
                            }
                        }
                    }
                    TabController.Instance.UpdateTab(newTab);
                    TabController.Instance.ClearCache(portalId);
                }
            }
        }


        private static TabPermissionInfo GetAlreadyPermission(TabInfo tab, string permissionKey, int roleId)
        {
            var permission = PermissionController.GetPermissionsByTab().Cast<PermissionInfo>().SingleOrDefault<PermissionInfo>(p => p.PermissionKey == permissionKey);

            return
                tab.TabPermissions.Cast<TabPermissionInfo>()
                    .FirstOrDefault(tp => tp.RoleID == roleId && tp.PermissionID == permission.PermissionID);
        }

        public static void AddPageSkin(int portalId, int pageId, string skinFolderName, string skinNameAscx)
        {
            if (pageId > 0)
            {
                var skinSrc = "[G]skins/" + skinFolderName + "/" + skinNameAscx;
                var controller = new TabController();
                var newTab = controller.GetTab(pageId, portalId);
                if (newTab.SkinSrc != skinSrc)
                {
                    newTab.SkinSrc = skinSrc;
                    controller.UpdateTab(newTab);
                }
            }
        }
        public static List<TabUrlInfo> GetTabUrls(int portalId, int tabId)
        {
            var controller = new TabController();
            return controller.GetTabUrls(tabId, portalId);
        }

    }
}
