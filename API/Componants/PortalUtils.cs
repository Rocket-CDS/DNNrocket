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

namespace DNNrocketAPI.Componants
{
    public static class PortalUtils
    {
        public static int GetCurrentPortalId()
        {
            return DNNrocketUtils.GetPortalId();
        }
        public static void  DeletePortal(int portalId)   
        {
            var portal = GetPortal(portalId);
            PortalController.DeletePortal(portal, "");
        }
        public static int CreatePortal(string portalName, string strPortalAlias)
        {
            // ************THIS DOES NOT WORK************
            // we seem to be missing some data format.

            var portalSettings = DNNrocketUtils.GetCurrentPortalSettings();
            var serverPath = "";
            var childPath = "";
            var description = "RocketECommerce";
            var keyWords = "";
            var homeDirectory = "";
            var template = new PortalController.PortalTemplateInfo(DNNrocketUtils.MapPath("/Portals/_default/Blank Website.template"), DNNrocketUtils.GetCurrentCulture());
            var isChild = false;

            //Create Portal
            var portalId = PortalController.Instance.CreatePortal(portalName,
                                                     UserUtils.GetCurrentUserId(),
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
                // don't return a null or cause error by accident.  The calling mathod should test and deal with it.
                return -1;
            }
            else
            {
                return PortalSettings.Current.PortalId;
            }
        }
        public static int GetPortalIdBySiteKey(string siteKey)
        {
            var guid = new Guid(siteKey);
            var controller = new PortalController();
            var portal = controller.GetPortal(guid);
            if (portal == null) return -1;
            return portal.PortalID;
        }
        public static PortalInfo GetPortal(int portalId)
        {
            var controller = new PortalController();
            var portal = controller.GetPortal(portalId);
            return portal;
        }

        public static PortalSettings GetPortalSettings()
        {
            return GetPortalSettings(PortalSettings.Current.PortalId);
        }
        public static PortalSettings GetPortalSettings(int portalId)
        {
            var controller = new PortalController();
            var portal = controller.GetPortal(portalId);
            return new PortalSettings(portal);
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
            return (PortalSettings)HttpContext.Current.Items["PortalSettings"];
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
            if (portalId < 0)
            {
                return PortalSettings.Current.DefaultPortalAlias;
            }
            else
            {
                var ps = GetPortalSettings(portalId);
                return ps.DefaultPortalAlias;
            }
        }
        public static string SiteGuid()
        {
            return PortalSettings.Current.GUID.ToString();
        }

        public static string GetPortalAlias(string lang, int portalid = -1)
        {
            var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DotNetNuke.Data.DataProvider.Instance().GetPortalAliases());

            var portalalias = DefaultPortalAlias(portalid);
            foreach (var pa in padic)
            {
                if (pa.Value.PortalID == PortalSettings.Current.PortalId)
                {
                    if (lang == pa.Value.CultureCode)
                    {
                        portalalias = pa.Key;
                    }
                }
            }
            return portalalias;
        }



    }
}
