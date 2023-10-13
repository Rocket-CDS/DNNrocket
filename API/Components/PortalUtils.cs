using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Simplisity;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using RazorEngine;
using System.Security.Cryptography;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using System.Xml;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Modules;
using System.Net;
using System.IO;
using DotNetNuke.Common.Lists;
using DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Users.Membership;
using System.Globalization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Services.Mail;
using DotNetNuke.Common;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Roles;
using static DotNetNuke.Common.Globals;
using DotNetNuke.Abstractions.Portals;
using System.Runtime.Remoting.Messaging;

namespace DNNrocketAPI.Components
{
    public static class PortalUtils
    {
        public static int GetCurrentPortalId()
        {
            return PortalUtils.GetPortalId();
        }
        public static void  DeletePortal(int portalId)   
        {
            var portal = GetPortal(portalId);
            PortalController.DeletePortal(portal, "");
        }
        public static int CreatePortal(string portalName, string strPortalAlias, int userId = -1, string description = "NewPortal", string cultureCode = "en-US")
        {
            if (userId <= 0) userId = UserUtils.GetCurrentUserId();
            var serverPath = "";
            var childPath = "";
            var keyWords = "";
            var homeDirectory = "";
            //var template = new PortalController.PortalTemplateInfo(DNNrocketUtils.MapPath("/Portals/_default/Blank Website.template"), "en-US");
            var template = new DotNetNuke.Entities.Portals.Templates.PortalTemplateInfo(DNNrocketUtils.MapPath("/Portals/_default/Blank Website.template"), cultureCode);
            var isChild = false;

            //Create Portal
            var portalId = PortalController.Instance.CreatePortal(portalName,
                                                     userId,
                                                     description,
                                                     keyWords,
                                                     template,
                                                     homeDirectory,
                                                     strPortalAlias,
                                                     serverPath,
                                                     "",
                                                     isChild);

            return portalId;
        }

        public static void BuildPortal(int portalid, int portalAdminUserId, string buildconfigfile)
        {
            var xmlData = FileUtils.ReadFile(buildconfigfile);
            if (xmlData != "")
            {
                var sRec = new SimplisityRecord(xmlData);
                // create pages
                var nodList = sRec.XMLDoc.SelectNodes("genxml/pages/page");
                if (nodList == null || nodList.Count == 0)
                {
                    // Add HomePage Skin and Modules
                    var homeTabId = PagesUtils.GetHomePage(portalid, DNNrocketUtils.GetCurrentCulture());
                    if (homeTabId >= 0)
                    {
                        ModuleUtils.DeleteAllTabModules(homeTabId);
                    }
                }
                else
                {
                    foreach (XmlNode nod in nodList)
                    {
                        var tabid = -1;
                        var pagename = nod.SelectSingleNode("page/name").Value;
                        if (PagesUtils.PageExists(portalid, pagename))
                        {
                            tabid = PagesUtils.GetPageByTabPath(portalid, pagename);
                        }
                        else
                        {
                            tabid = PagesUtils.CreatePage(portalid, pagename);
                            var skin = nod.SelectSingleNode("page/skin").Value;
                            if (skin != "")
                            {
                                var skinlayout = nod.SelectSingleNode("page/skinlayout").Value;
                                var skincontainer = nod.SelectSingleNode("page/skincontainer").Value;
                                PagesUtils.AddPageSkin(portalid, tabid, skin, skinlayout);
                            }
                        }
                        var moduleList = nod.SelectNodes("page/modules/module");
                        foreach (XmlNode mNod in moduleList)
                        {
                            var modulename = nod.SelectSingleNode("module/name").Value;
                            var definitionname = nod.SelectSingleNode("module/definitionname").Value;
                            var modulecontainer = nod.SelectSingleNode("module/container").Value;
                            if (definitionname != "")
                            {
                                //[TODO: add required modules to containers.]
                                var dtid = ModuleUtils.GetDesktopModuleId(portalid, definitionname);
                                if (dtid > -1) ModuleUtils.AddNewModuleToTab(portalid, tabid, modulename, dtid, "", 0, 0, "");
                            }
                        }
                    }
                }
            }
            else
            {
                // Add HomePage Skin and Modules
                var homeTabId = PagesUtils.GetHomePage(portalid, DNNrocketUtils.GetCurrentCulture());
                if (homeTabId >= 0)
                {
                    ModuleUtils.DeleteAllTabModules(homeTabId);
                }
            }


            DNNrocketUtils.CreateDefaultRocketRoles(portalid);

            // Add current user as manager, if not superuser
            if (UserUtils.GetCurrentUserId() != portalAdminUserId)
            {
                UserUtils.CreateUser(portalid, UserUtils.GetCurrentUserName(), UserUtils.GetCurrentUserEmail(), DNNrocketRoles.Manager);
                var role = UserUtils.GetRoleByName(portalid, DNNrocketRoles.Premium);
                var roleid = role.GetXmlPropertyInt("genxml/roleid");
                if (roleid > 0) UserUtils.AddUserRole(portalid, UserUtils.GetCurrentUserId(), roleid);
            }

            PortalUtils.Registration(portalid, 0);
            PortalUtils.EnablePopups(portalid, false);

            BuildDefaultSystems(portalid);
        }
        public static void BuildDefaultSystems(int portalId)
        {
            var systems = new SystemLimpetList();
            foreach (var s in systems.GetSystemList())
            {
                ActivateSystem(portalId, s);
            }
        }
        public static void ActivateSystem(int portalId, SystemLimpet systemData, SimplisityInfo postInfo = null)
        {
            if (systemData != null && systemData.Exists)
            {
                var paramCmd = systemData.SystemKey + "_activate";
                var interfacekey = paramCmd.Split('_')[0];
                var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                if (rocketInterface.Exists)
                {
                    var paramInfo = new SimplisityInfo();
                    paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
                    if (postInfo == null)
                    {
                        postInfo = new SimplisityInfo();
                        var configFileName = DNNrocketUtils.MapPath("/DesktopModules/DNNRocketModules/" + systemData.SystemKey + "/Installation/SystemInit.rules");
                        if (File.Exists(configFileName))
                        {
                            var xmlData = FileUtils.ReadFile(configFileName);
                            postInfo.XMLData = xmlData;

                            DNNrocketUtils.GetProviderReturn(paramCmd, systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "/DesktopModules/DNNrocket/api", "");
                        }
                    }
                    else
                    {
                        DNNrocketUtils.GetProviderReturn(paramCmd, systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "/DesktopModules/DNNrocket/api", "");
                    }
                }
            }
        }
        public static List<int> GetPortals()
        {
            var rtnList = new List<int>();
            foreach (PortalInfo portal in PortalController.Instance.GetPortals())
            {
                rtnList.Add(portal.PortalID);
            }
            return rtnList;
        }

        public static int GetPortalId()
        {
            if (PortalSettings.Current == null)
            {
                // don't return a null or cause error by accident.  The calling method should test and deal with it.
                return -1;
            }
            else
            {
                return PortalSettings.Current.PortalId;
            }
        }
        public static int GetPortalIdBySiteKey(string siteKey)
        {
            try
            {
                var guid = new Guid(siteKey);
                var controller = new PortalController();
                var portal = controller.GetPortal(guid);
                if (portal == null) return -1;
                return portal.PortalID;
            }
            catch (Exception)
            {
                ///LogUtils.LogException(ex);
                return -1; // Invalid Guid.
            }
        }
        public static PortalInfo GetPortal(int portalId)
        {
            var controller = new PortalController();
            var portal = controller.GetPortal(portalId);
            return portal;
        }
        public static String GetPortalName(int portalId)
        {
            var portal = GetPortal(portalId);
            if (portal == null) return "";
            return portal.PortalName;
        }
        /// <summary>
        /// Set portal Registration type.
        /// </summary>
        /// <param name="regType"> NoRegistration = 0, PrivateRegistration = 1, PublicRegistration = 2, VerifiedRegistration = 3</param>
        public static void Registration(int portalId, int regType)
        {
            PortalInfo objPortal = PortalController.Instance.GetPortal(portalId);
            objPortal.UserRegistration = regType;
            PortalController.Instance.UpdatePortalInfo(objPortal);
        }
        public static void EnablePopups(int portalId, bool value)
        {
            PortalController.Instance.UpdatePortalSetting(portalId, "EnablePopUps", value.ToString(), true, DNNrocketUtils.GetCurrentCulture(), false);
        }
        public static PortalSettings GetPortalSettings()
        {
            return GetPortalSettings(PortalSettings.Current.PortalId);
        }
        public static PortalSettings GetPortalSettings(int portalId)
        {
            return new PortalSettings(portalId);
        }
        public static int LoginTabId(int portalId)
        {
            return new PortalSettings(portalId).LoginTabId;
        }
        public static int LoginTabId()
        {
            return new PortalSettings(PortalSettings.Current.PortalId).LoginTabId;
        }
        public static int GetPortalByModuleID(int moduleId)
        {
            var objMCtrl = new DotNetNuke.Entities.Modules.ModuleController();
            var objMInfo = objMCtrl.GetModule(moduleId);
            if (objMInfo == null) return -1;
            return objMInfo.PortalID;
        }

        private static List<PortalInfo> GetAllPortals()
        {
            var pList = new List<PortalInfo>();
            var objPC = new DotNetNuke.Entities.Portals.PortalController();

            var list = objPC.GetPortals();

            if (list == null || list.Count == 0)
            {
                //Problem with DNN6 GetPortals when ran from scheduler.
                PortalInfo objPInfo;
                var flagdeleted = 0;

                for (var lp = 0; lp <= 500; lp++)
                {
                    objPInfo = objPC.GetPortal(lp);
                    if ((objPInfo != null))
                    {
                        pList.Add(objPInfo);
                    }
                    else
                    {
                        // some portals may be deleted, skip 3 to see if we've got to the end of the list.
                        // VERY weak!!! shame!! but issue with a DNN6 version only.
                        if (flagdeleted == 3) break;
                        flagdeleted += 1;
                    }
                }
            }
            else
            {
                foreach (PortalInfo p in list)
                {
                    pList.Add(p);
                }
            }


            return pList;
        }
        public static List<int> GetAllPortalIds()
        {
            var rtnList = new List<int>();
            var allportals = GetAllPortals();
            foreach (var p in allportals)
            {
                rtnList.Add(p.PortalID);
            }
            return rtnList;
        }

        public static List<SimplisityRecord> GetAllPortalRecords()
        {
            var rtnList = new List<SimplisityRecord>();
            var allportals = GetAllPortals();
            foreach (var p in allportals)
            {
                var r = new SimplisityRecord();
                r.PortalId = p.PortalID;
                r.XMLData = DNNrocketUtils.ConvertObjectToXMLString(p);
                rtnList.Add(r);
            }
            return rtnList;
        }
        public static void CreatePortalFolder(DotNetNuke.Entities.Portals.PortalSettings PortalSettings, string FolderName)
        {
            bool blnCreated = false;

            //try normal test (doesn;t work on medium trust, but avoids waiting for GetFolder.)
            try
            {
                blnCreated = System.IO.Directory.Exists(PortalSettings.HomeDirectoryMapPath + FolderName);
            }
            catch (Exception ex)
            {
                var errmsg = ex.ToString();
                blnCreated = false;
            }

            if (!blnCreated)
            {
                FolderManager.Instance.Synchronize(PortalSettings.PortalId, PortalSettings.HomeDirectory, true, true);
                var folderInfo = FolderManager.Instance.GetFolder(PortalSettings.PortalId, FolderName);
                if (folderInfo == null & !string.IsNullOrEmpty(FolderName))
                {
                    //add folder and permissions
                    try
                    {
                        FolderManager.Instance.AddFolder(PortalSettings.PortalId, FolderName);
                    }
                    catch (Exception ex)
                    {
                        var errmsg = ex.ToString();
                    }
                    folderInfo = FolderManager.Instance.GetFolder(PortalSettings.PortalId, FolderName);
                    if ((folderInfo != null))
                    {
                        int folderid = folderInfo.FolderID;
                        DotNetNuke.Security.Permissions.PermissionController objPermissionController = new DotNetNuke.Security.Permissions.PermissionController();
                        var arr = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "");
                        foreach (DotNetNuke.Security.Permissions.PermissionInfo objpermission in arr)
                        {
                            if (objpermission.PermissionKey == "WRITE")
                            {
                                // add READ permissions to the All Users Role
                                FolderManager.Instance.SetFolderPermission(folderInfo, objpermission.PermissionID, int.Parse(DotNetNuke.Common.Globals.glbRoleAllUsers));
                            }
                        }
                    }
                }
            }
        }


        public static PortalSettings GetCurrentPortalSettings()
        {
            PortalSettings rtn = null;
            if (HttpContext.Current != null) rtn = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (rtn == null) rtn = PortalSettings.Current;
            if (rtn == null) rtn = GetPortalSettings(0);
            return rtn;
        }

        public static Dictionary<string, string> GetPortalAliasesWithCultureCode(int portalId)
        {
            var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DotNetNuke.Data.DataProvider.Instance().GetPortalAliases());
            var rtnList = new Dictionary<string, string>();
            foreach (var pa in padic)
            {
                if (pa.Value.PortalID == portalId)
                {
                    rtnList.Add(pa.Key, pa.Value.CultureCode);
                }
            }
            return rtnList;
        }
        public static List<string> GetPortalAliases(int portalId)
        {
            var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DotNetNuke.Data.DataProvider.Instance().GetPortalAliases());
            var rtnList = new List<string>();
            foreach (var pa in padic)
            {
                if (pa.Value.PortalID == portalId)
                {
                    rtnList.Add(pa.Key);
                }
            }
            return rtnList;
        }
        public static string DefaultPortalAlias(int portalId = -1)
        {
            return DefaultPortalAlias(portalId, "");
        }
        public static string DefaultPortalAlias(int portalId, string cultureCode)
        {
            if (portalId < 0) portalId = GetPortalId();
            var portalalias = "";
            var objCtrl = new DNNrocketController();
            var cmd = "SELECT HTTPAlias, isnull(CultureCode,'') as [CultureCode] FROM {databaseOwner}[{objectQualifier}PortalAlias]  WHERE portalid = " + portalId + "  for xml raw";
            var xmlList = objCtrl.ExecSqlXmlList(cmd);
            if (xmlList.Count > 0)
            {
                foreach(SimplisityRecord x in xmlList)
                {
                    var a = x.GetXmlProperty("row/@HTTPAlias");
                    var cc = x.GetXmlProperty("row/@CultureCode");
                    if (cc == cultureCode)
                    {
                        portalalias = a;
                    }
                }
            }
            if (String.IsNullOrEmpty(portalalias)) portalalias = PortalSettings.Current.DefaultPortalAlias;
            return portalalias;
        }

        public static void AddPortalAlias(int portalId, string portalAlias, string cultureCode = "")
        {
            try
            {
                portalAlias = portalAlias.ToLower().Replace("http://", "").Replace("https://", "");
                PortalController.Instance.AddPortalAlias(portalId, portalAlias);
                if (cultureCode != "")
                {
                    var pa = PortalAliasController.Instance.GetPortalAlias(portalAlias, portalId);
                    pa.CultureCode = cultureCode;
                    PortalAliasController.Instance.UpdatePortalAlias(pa);
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }
        public static void DeletePortalAlias(int portalId, string portalAlias)
        {
            try
            {
                portalAlias = portalAlias.ToLower().Replace("http://", "").Replace("https://", "");
                var pa = PortalAliasController.Instance.GetPortalAlias(portalAlias, portalId);
                if (pa != null) PortalAliasController.Instance.DeletePortalAlias(pa);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }
        public static void SetPrimaryPortalAlias(int portalId, string portalAlias, bool isPrimary = true)
        {
            portalAlias = portalAlias.ToLower().Replace("http://", "").Replace("https://", "");
            PortalAliasInfo newPrimaryPortalAlias = null;
            var paList = PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId);
            foreach (var pa in paList)
            {
                if (pa.HTTPAlias == portalAlias)
                {
                    newPrimaryPortalAlias = pa;
                }
            }
            if (newPrimaryPortalAlias != null)
            {
                foreach (var pa in paList)
                {
                    if (pa.PortalAliasID == newPrimaryPortalAlias.PortalAliasID)
                    {
                        pa.IsPrimary = isPrimary;
                    }
                    PortalAliasController.Instance.UpdatePortalAlias(pa);
                }
            }
        }
        public static void SetDefaultLanguage(int portalId, string cultureCode)
        {
            try
            {
                PortalInfo objPortal = PortalController.Instance.GetPortal(portalId);
                objPortal.DefaultLanguage = cultureCode;
                PortalController.Instance.UpdatePortalInfo(objPortal);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }
        public static string GetDefaultLanguage(int portalId)
        {
            try
            {
                PortalInfo objPortal = PortalController.Instance.GetPortal(portalId);
                return objPortal.DefaultLanguage;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
            return "en-US";
        }
        public static string RootDomain(int portalId = -1)
        {
            var da = DefaultPortalAlias(portalId);
            var daarray = da.Split('.');
            if (daarray.Length <= 2) return da;
            var rtn1 = daarray[daarray.Length - 2] + "." + daarray[daarray.Length - 1];
            var rtnSplit = rtn1.Split('/');
            var rtn = rtnSplit[0];
            return rtn;
        }
        public static string DomainSubUrl(int portalId = -1)
        {
            var da = DefaultPortalAlias(portalId);
            var rtnSplit = da.Split('/');
            var rtn = rtnSplit[0];
            return rtn;
        }
        [Obsolete("This can cause a race condition in Razor. (avoid use) ")]
        public static string SiteGuid(int portalId)
        {
            /// I think this is slow and causes race condition.
            var ps = GetPortalSettings(portalId);
            return ps.GUID.ToString();
        }

        public static string GetPortalAlias(string lang, int portalid = -1)
        {
            if (portalid == -1) portalid = PortalSettings.Current.PortalId;
            var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DotNetNuke.Data.DataProvider.Instance().GetPortalAliases());

            var portalalias = DefaultPortalAlias(portalid);
            foreach (var pa in padic)
            {
                if (pa.Value.PortalID == portalid)
                {
                    if (lang == pa.Value.CultureCode)
                    {
                        portalalias = pa.Key;
                    }
                }
            }
            return portalalias;
        }
        public static string HomeDNNrocketDirectoryMapPath(int portalId = -1)
        {
            return HomeDirectoryMapPath(portalId).TrimEnd('\\') + "\\DNNrocket";
        }
        public static string HomeDNNrocketDirectoryRel(int portalId = -1)
        {
            return HomeDirectoryRel(portalId).TrimEnd('/') + "/DNNrocket";
        }
        public static string DNNrocketThemesDirectoryMapPath(int portalId = -1)
        {
            return HomeDirectoryMapPath(portalId).TrimEnd('\\') + "\\RocketThemes";
        }
        public static string DNNrocketThemesDirectoryRel(int portalId = -1)
        {
            return HomeDirectoryRel(portalId).TrimEnd('/') + "/RocketThemes/";
        }
        public static string TempDirectoryMapPath(int portalId = -1)
        {
            return HomeDirectoryMapPath(portalId).TrimEnd('\\') + "\\DNNrocketTemp";
        }
        public static string BackUpDirectoryMapPath(int portalId = -1)
        {
            return HomeDirectoryMapPath(portalId).TrimEnd('\\') + "\\DNNrocketBackUp";
        }
        public static string TempDirectoryRel(int portalId = -1)
        {
            return HomeDirectoryRel(portalId).TrimEnd('/') + "/DNNrocketTemp/";
        }
        public static string HomeDirectoryMapPath(int portalId)
        {
            if (portalId < 0) portalId = PortalSettings.Current.PortalId;
            var portalInfo = PortalController.Instance.GetPortal(portalId);
            return portalInfo.HomeDirectoryMapPath;
        }
        public static string HomeDirectoryRel(int portalId)
        {
            if (portalId < 0) portalId = PortalSettings.Current.PortalId;
            var portalInfo = PortalController.Instance.GetPortal(portalId);
            return portalInfo.HomeDirectory;
        }
        public static void CreateRocketDirectories(int portalId = -1)
        {            
            if (PortalExists(portalId)) // check we have a portal, could be deleted
            {
                if (!Directory.Exists(TempDirectoryMapPath(portalId)))
                {
                    Directory.CreateDirectory(TempDirectoryMapPath(portalId));
                    Directory.CreateDirectory(TempDirectoryMapPath(portalId) + "\\debug");
                }
                if (!Directory.Exists(HomeDNNrocketDirectoryMapPath(portalId)))
                {
                    Directory.CreateDirectory(HomeDNNrocketDirectoryMapPath(portalId));
                }
                if (!Directory.Exists(DNNrocketThemesDirectoryMapPath(portalId)))
                {
                    Directory.CreateDirectory(DNNrocketThemesDirectoryMapPath(portalId));
                }
                if (!Directory.Exists(BackUpDirectoryMapPath(portalId)))
                {
                    Directory.CreateDirectory(BackUpDirectoryMapPath(portalId));
                }
            }
        }
        public static bool PortalExists(int portalId = -1)
        {
            var p = PortalUtils.GetPortal(portalId); // check we have a portal, could be deleted
            if (p == null) return false;
            return true;
        }
        public static void AddLanguage(int portalId, string cultureCode)
        {
            var local = new Locale { Code = cultureCode, Fallback = Localization.SystemLocale, Text = CultureInfo.GetCultureInfo(cultureCode).NativeName };

            var globalLocales = LocaleController.Instance.GetLocales(-1);
            var addLanguage = false;
            if (!globalLocales.ContainsKey(cultureCode)) addLanguage = true;
            if (addLanguage) Localization.SaveLanguage(local);

            var local2 = LocaleController.Instance.GetLocale(cultureCode);
            if (local2 != null) Localization.AddLanguageToPortal(portalId, local2.LanguageId, false);
        }
        public static void RemoveLanguage(int portalId, string cultureCode)
        {
            var portalLocales = LocaleController.Instance.GetLocales(portalId);
            var removeLanguage = false;
            if (portalLocales.ContainsKey(cultureCode) && portalLocales.Count > 1) removeLanguage = true;
            if (removeLanguage)
            {
                var local = LocaleController.Instance.GetLocale(cultureCode);
                if (local != null)
                {
                    try
                    {
                        Localization.RemoveLanguageFromPortal(portalId, local.LanguageId, false);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogException(ex);
                    }
                }
            }
        }

    }
}
