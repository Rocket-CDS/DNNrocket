using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;

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
        public static void AddTabUrl301(int tabId, string pageUrl, string queryString, int portalId, string cultureCode)
        {
            try
            {
                // update tab url table.
                if (pageUrl != "" & tabId > 0)
                {
                    var objTabs = new TabController();

                    if (!pageUrl.StartsWith("/")) pageUrl = "/" + pageUrl;
                    var tabInfo = objTabs.GetTab(tabId, portalId);
                    if (tabInfo != null)
                    {
                        var tabUrlsList = objTabs.GetTabUrls(tabId, portalId);
                        var seqNum = 0;

                        var tabUrlInfo = tabUrlsList.FirstOrDefault(t => t.Url == pageUrl && t.CultureCode == cultureCode);
                        if (tabUrlInfo == null)
                        {
                            tabUrlInfo = new TabUrlInfo();
                            seqNum = Convert.ToInt32(tabUrlsList.Count) + 1;
                        }
                        else
                        {
                            seqNum = tabUrlInfo.SeqNum;
                        }

                        tabUrlInfo.TabId = tabId;
                        tabUrlInfo.SeqNum = seqNum;
                        tabUrlInfo.HttpStatus = "301";
                        tabUrlInfo.Url = pageUrl;
                        tabUrlInfo.QueryString = queryString;
                        tabUrlInfo.CultureCode = cultureCode;
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
        public static void RemoveAllTabUrls(int tabId)
        {
            try
            {
                if (tabId > 0)
                {
                    var objTabs = new TabController();
                    var tabUrlList = objTabs.GetTabUrls(tabId, PortalSettings.Current.PortalId);
                    foreach (var t in tabUrlList)
                    {
                        objTabs.DeleteTabUrl(t, PortalSettings.Current.PortalId, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException("RemoveAllTabUrls: Error on DNNrocket PageUtils", null, ex);
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
            if (tabId <= 0) return false;
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
        public static void SetLoginPage(int portalId, int pageId)
        {
            try
            {
                if (portalId <= 0 || pageId <= 0) return;

                // Verify the page exists and is not deleted
                var controller = new TabController();
                var tab = controller.GetTab(pageId, portalId);
                if (tab == null || tab.IsDeleted) return;

                // Get the portal controller and update the login page setting
                var portalController = new PortalController();
                var portalInfo = portalController.GetPortal(portalId);
                if (portalInfo != null)
                {
                    portalInfo.LoginTabId = pageId;
                    portalController.UpdatePortalInfo(portalInfo);
                    
                    // Clear portal cache to ensure changes take effect
                    DataCache.ClearPortalCache(portalId, true);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException("SetLoginPage: Error setting login page", null, ex);
            }
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
        /// <summary>
        /// Changes the complete theme (skin and container) for a portal
        /// </summary>
        /// <param name="portalId">The portal ID</param>
        /// <param name="skinName">The skin folder name (e.g., "Xcillion")</param>
        /// <param name="skinFile">The skin file name (e.g., "Home")</param>
        /// <param name="containerName">The container folder name (e.g., "Xcillion")</param>
        /// <param name="containerFile">The container file name (e.g., "Title_h2")</param>
        /// <param name="isGlobalTheme">True if it's a global theme, false for portal-specific</param>
        public static void ChangePortalTheme(int portalId, string skinName, string skinFile, string containerName, string containerFile, bool isGlobalTheme = true)
        {
            try
            {
                // Determine the path prefix based on theme location
                string pathPrefix = isGlobalTheme ? "[G]" : "[L]";

                // Build the skin and container paths
                string skinPath = $"{pathPrefix}Skins/{skinName}/{skinFile}.ascx";
                string containerPath = $"{pathPrefix}Containers/{containerName}/{containerFile}.ascx";

                // Set the portal skin (normal viewing)
                SkinController.SetSkin(SkinController.RootSkin, portalId, SkinType.Portal, skinPath);

                // Set the admin skin (edit mode)
                SkinController.SetSkin(SkinController.RootSkin, portalId, SkinType.Admin, skinPath);

                // Set the portal container (normal viewing)
                SkinController.SetSkin(SkinController.RootContainer, portalId, SkinType.Portal, containerPath);

                // Set the admin container (edit mode)
                SkinController.SetSkin(SkinController.RootContainer, portalId, SkinType.Admin, containerPath);

                // Clear portal cache to ensure changes take effect
                DataCache.ClearPortalCache(portalId, true);
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                throw;
            }
        }

        public static List<TabUrlInfo> GetTabUrls(int portalId, int tabId)
        {
            var controller = new TabController();
            return controller.GetTabUrls(tabId, portalId);
        }

        #region "Meta Data"


        public static PageMetaData GetMetaData(int tabId, Uri requestUri, Dictionary<string, string> urlParams)
        {
            var page = new PageMetaData();
            try
            {
                
                var is404 = false;
                var portalId = PortalUtils.GetCurrentPortalId();
                var cultureCode = DNNrocketUtils.GetCurrentCulture();

                // Read Data
                var objCtrl = new DNNrocketController();
                var cacheKey2 = "PLSETTINGS" + portalId;
                var plRecord = (SimplisityRecord)CacheUtils.GetCache(cacheKey2, portalId.ToString());
                if (plRecord == null)
                {
                    plRecord = objCtrl.GetRecordByGuidKey(portalId, -1, "PLSETTINGS", "PLSETTINGS");
                    CacheUtils.SetCache(cacheKey2, plRecord, portalId.ToString());
                }
                var cacheKey = "PL_" + cultureCode + "_" + tabId.ToString("");
                var dataRecord = (SimplisityRecord)CacheUtils.GetCache(cacheKey, portalId.ToString());
                if (dataRecord == null)
                {
                    dataRecord = objCtrl.GetRecordByGuidKey(portalId, -1, "PL", cacheKey);
                    CacheUtils.SetCache(cacheKey, dataRecord, portalId.ToString());
                }

                // check for paramid
                var queryKeyList = GetUrlCategoryQueryKeyList(portalId);
                var appendList = new Dictionary<string, QueryParamsData>();
                var paramidList = DNNrocketUtils.GetQueryKeys(portalId);
                var hasCategoryParam = false;
                if (paramidList.Count > 1)
                {
                    foreach (var qk in queryKeyList)
                    {
                        if (paramidList.ContainsKey(qk))
                        {
                            hasCategoryParam = true;
                            break;
                        }
                    }
                }
                if (hasCategoryParam)
                {
                    // move the catid to the last param, so it's only taken if only the catid is in the URL.
                    var paramidList2 = new Dictionary<string, QueryParamsData>();
                    foreach (var p in paramidList)
                    {
                        if (!queryKeyList.Contains(p.Key))
                            paramidList2.Add(p.Key, p.Value);
                        else
                            appendList.Add(p.Key, p.Value);
                    }
                    foreach (var qk in appendList)
                    {
                        paramidList2.Add(qk.Value.queryparam, qk.Value);
                    }
                    paramidList = paramidList2;
                }

                var articleMeta = false;
                var metaList = new Dictionary<string,string>();
                var articleid = 0;
                var articleParamKey = "";
                var foundArticle = false;
                var catMeta = false;
                var articleTable = "";
                var metatitle = "";
                var metadescription = "";
                var metatagwords = "";
                var disablealternate = false;
                var disablecanonical = false;
                var articleDefaultTabId = 0;
                var articleListTabId = 0;

                foreach (var paramDict in paramidList)
                {
                    if (urlParams.ContainsKey(paramDict.Key))
                    {
                        var paramValue = urlParams[paramDict.Key];
                        if (paramValue != null && GeneralUtils.IsNumeric(paramValue))
                        {
                            if (!foundArticle) // we can have only 1 article in the SEO, take the first found.  (catid is moved to last position inthe list.)
                            {

                                articleTable = paramDict.Value.tablename;
                                articleid = Convert.ToInt32(paramValue);

                                var dataRecordTemp = objCtrl.GetInfo(articleid, DNNrocketUtils.GetCurrentCulture(), articleTable);
                                if (dataRecordTemp != null)
                                {
                                    foundArticle = true;

                                    metatitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle");
                                    if (metatitle == "") metatitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/name");
                                    if (metatitle == "") metatitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlename");
                                    if (metatitle == "") metatitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/productname");
                                    metatitle = metatitle.Truncate(200);

                                    metadescription = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seodescription");
                                    if (metadescription == "") metadescription = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/summary");
                                    if (metadescription == "") metadescription = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlesummary");
                                    if (metadescription == "") metadescription = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/productsummary");
                                    metadescription = metadescription.Truncate(260);

                                    metatagwords = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seokeyword");
                                    metatagwords = metatagwords.Truncate(260);

                                    var portalContentRec = DNNrocketUtils.GetPortalContentRecByRefId(dataRecordTemp.PortalId, paramDict.Value.systemkey, articleTable);
                                    if (portalContentRec == null) portalContentRec = new SimplisityRecord();
                                    articleDefaultTabId = portalContentRec.GetXmlPropertyInt("genxml/detailpage");
                                    if (articleDefaultTabId == 0) articleDefaultTabId = tabId;
                                    articleListTabId = portalContentRec.GetXmlPropertyInt("genxml/listpage");
                                    if (articleListTabId == 0) articleListTabId = tabId;

                                    articleParamKey = paramDict.Key;
                                    if (!queryKeyList.Contains(articleParamKey))
                                    {
                                        string[] urlparams = { articleParamKey, articleid.ToString(), DNNrocketUtils.UrlFriendly(metatitle) };
                                        var ogurl = DNNrocketUtils.NavigateURL(articleDefaultTabId, dataRecordTemp.Lang, urlparams);

                                        metaList.Add("og:type", "article");
                                        metaList.Add("og:title", metatitle.Truncate(200).Replace("\"", ""));
                                        metaList.Add("og:description", metadescription.Truncate(260).Replace("\"", ""));
                                        metaList.Add("og:url", ogurl);
                                        var imgRelPath = dataRecordTemp.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/imagepatharticleimage").ToString();
                                        if (imgRelPath == "") imgRelPath = dataRecordTemp.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/imagepathproductimage").ToString();
                                        if (imgRelPath != "") metaList.Add("og:image", requestUri.GetLeftPart(UriPartial.Authority).TrimEnd('/') + "/" + imgRelPath.TrimStart('/'));

                                        articleMeta = true;
                                    }
                                    else
                                    {
                                        catMeta = true;
                                    }

                                    // if the systemkey for the articleid do not match we throw a 404. (PRD and CAT for RocketEcomemrce)
                                    if (!dataRecordTemp.TypeCode.StartsWith(paramDict.Value.systemkey) && (dataRecordTemp.TypeCode != "PRD" && dataRecordTemp.TypeCode != "CAT")) is404 = true;
                                    break;
                                }
                                else
                                {
                                    // record does not exist, throw a 404.
                                    is404 = true;
                                }
                            }
                        }
                    }
                }

                if (dataRecord != null)
                {
                    if (!foundArticle) // Use PL data if no article or cat title found.
                    {
                        metatitle = dataRecord.GetXmlProperty("genxml/textbox/pagetitle");
                        metadescription = dataRecord.GetXmlProperty("genxml/textbox/pagedescription");
                        metatagwords = dataRecord.GetXmlProperty("genxml/textbox/tagwords");
                    }

                    disablealternate = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablealternate");
                    disablecanonical = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablecanonical");

                }

                // ********** Add alt url meta for langauges ***********
                var cachekey = "RocketTools*hreflang*" + PortalSettings.Current.PortalId + "*" + cultureCode + "*" + tabId + "*" + articleid; // use nodeTablist incase the DDRMenu has a selector.                
                var hreflangobj = CacheUtils.GetCache(cachekey, portalId.ToString());
                var canonicalurl = (string)CacheUtils.GetCache(cachekey + "2", portalId.ToString());
                if (canonicalurl == null) canonicalurl = "";
                var hreflangtext = "";
                if (hreflangobj != null) hreflangtext = hreflangobj.ToString();
                if (hreflangobj == null)
                {
                    hreflangtext = "";  // clear so we don't produce multiple hreflang with cache.
                    var enabledlanguages = LocaleController.Instance.GetLocales(PortalSettings.Current.PortalId);
                    foreach (var l in enabledlanguages)
                    {
                        if ((catMeta || articleMeta) && articleParamKey != "")
                        {
                            var seotitle = "";
                            var dataRecordTemp = objCtrl.GetInfo(articleid, l.Key, articleTable);
                            if (dataRecordTemp != null)
                            {
                                seotitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle");
                                if (seotitle == "") seotitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/name");
                                if (seotitle == "") seotitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlename");
                            }
                            seotitle = DNNrocketUtils.UrlFriendly(seotitle);

                            string[] urlparams = { articleParamKey, articleid.ToString(), seotitle };
                            if (catMeta)
                                hreflangtext += "<link rel='alternate' href='" + DNNrocketUtils.NavigateURL(articleListTabId, "", l.Key, urlparams) + "' hreflang='" + l.Key.ToLower() + "'/>";
                            else
                                hreflangtext += "<link rel='alternate' href='" + DNNrocketUtils.NavigateURL(articleDefaultTabId, "", l.Key, urlparams) + "' hreflang='" + l.Key.ToLower() + "'/>";

                            if (cultureCode == l.Key)
                            {
                                if (catMeta)
                                    canonicalurl = DNNrocketUtils.NavigateURL(articleListTabId, "", urlparams);
                                else
                                    canonicalurl = DNNrocketUtils.NavigateURL(articleDefaultTabId, "", urlparams);
                            }
                        }
                        else
                        {
                            hreflangtext += "<link rel='alternate' href='" + DNNrocketUtils.NavigateURL(tabId, "", l.Key, null) + "' hreflang='" + l.Key.ToLower() + "'/>";
                            if (cultureCode == l.Key) canonicalurl = DNNrocketUtils.NavigateURL(tabId);
                        }
                    }
                    CacheUtils.SetCache(cachekey, hreflangtext, portalId.ToString());
                    CacheUtils.SetCache(cachekey + "2", canonicalurl, portalId.ToString());
                }

                if (!String.IsNullOrEmpty(hreflangtext) && !disablealternate) page.AlternateLinkHtml = hreflangtext;

                page.CanonicalLinkUrl = ""; // remove so we dont; display anything from invalid module values.

                if (metatitle != "") page.Title = metatitle;
                if (metadescription != "") page.Description = metadescription;
                if (metatagwords != "") page.KeyWords = metatagwords;
                if (!String.IsNullOrEmpty(canonicalurl) && !disablecanonical) page.CanonicalLinkUrl = canonicalurl;

                foreach (var meta in metaList)
                {
                    page.HtmlMeta.Add(meta.Key, meta.Value);
                }

                if (plRecord != null)
                {
                    foreach (var cssPattern in plRecord.GetRecordList("removecss"))
                    {
                        var sPattern = cssPattern.GetXmlProperty("genxml/textbox/removecss");
                        if (sPattern != "" && !UserUtils.IsAdministrator()) page.CssRemovalPattern.Add(sPattern);
                    }
                }
                page.Redirect404 = is404;
                return page;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return page;
            }
        }
        public static HtmlMeta BuildMeta(string name, string property, string content)
        {
            HtmlMeta meta = new HtmlMeta();
            meta.Name = name;
            if (!String.IsNullOrEmpty(property)) meta.Attributes.Add("property", property);
            meta.Content = content;
            return meta;
        }
        private static List<string> GetUrlCategoryQueryKeyList(int portalId)
        {
            var cacheKey = "GetUrlCategoryQueryKeyList*" + portalId + "*category";
            var paramKey = (List<string>)CacheUtils.GetCache(cacheKey, "portalid" + portalId);
            if (paramKey == null)
            {
                paramKey = new List<string>();
                var paramidList = DNNrocketUtils.GetQueryKeys(portalId);
                foreach (var paramDict in paramidList)
                {
                    if (paramDict.Value.datatype == "category")
                    {
                        paramKey.Add(paramDict.Value.queryparam);
                    }
                }
                CacheUtils.SetCache(cacheKey, paramKey, "portalid" + portalId);
            }
            return paramKey;
        }

        #endregion

                /// <summary>
/// Updates page details in DNN
/// </summary>
/// <param name="portalId">The portal ID</param>
/// <param name="tabId">The tab/page ID to update</param>
/// <param name="pageName">The new page name (optional)</param>
/// <param name="title">The new page title (optional)</param>
/// <param name="description">The new page description (optional)</param>
/// <param name="keywords">The new page keywords (optional)</param>
/// <param name="isVisible">Whether the page is visible (optional)</param>
/// <param name="disableLink">Whether to disable the page link (optional)</param>
/// <param name="iconFile">The icon file for the page (optional)</param>
/// <param name="url">Redirect URL if this is a redirect page (optional)</param>
/// <returns>True if successful, false otherwise</returns>
public static bool UpdatePageDetails(int portalId, int tabId, string pageName = null, string title = null, 
    string description = null, string keywords = null, bool? isVisible = null, bool? disableLink = null, 
    string iconFile = null, string url = null)
{
    try
    {
        if (tabId <= 0) return false;

        var controller = new TabController();
        var tab = controller.GetTab(tabId, portalId);
        
        if (tab == null || tab.IsDeleted) return false;

        // Update properties only if new values are provided
        if (!string.IsNullOrEmpty(pageName))
            tab.TabName = pageName;
        
        if (!string.IsNullOrEmpty(title))
            tab.Title = title;
        
        if (!string.IsNullOrEmpty(description))
            tab.Description = description;
        
        if (!string.IsNullOrEmpty(keywords))
            tab.KeyWords = keywords;
        
        if (isVisible.HasValue)
            tab.IsVisible = isVisible.Value;
        
        if (disableLink.HasValue)
            tab.DisableLink = disableLink.Value;
        
        if (!string.IsNullOrEmpty(iconFile))
            tab.IconFile = iconFile;
        
        if (!string.IsNullOrEmpty(url))
            tab.Url = url;

        // Update the tab
        controller.UpdateTab(tab);
        
        // Clear cache to ensure changes take effect
        TabController.Instance.ClearCache(portalId);
        
        return true;
    }
    catch (Exception ex)
    {
        Exceptions.ProcessModuleLoadException("UpdatePageDetails: Error updating page details", null, ex);
        return false;
    }
}
    }
}
