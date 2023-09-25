using System;
using Simplisity;
using System.IO;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using System.Collections;
using DotNetNuke.UI.UserControls;
using System.Web.Security;
using System.Reflection;
using System.Web.UI.WebControls;
using DotNetNuke.Security.Roles;

namespace DNNrocketAPI.Components
{
    public static class ModuleUtils
    {
        public static void CreateModuleFromManifest(string manifestMapPath)
        {
            var manifest = FileUtils.ReadFile(manifestMapPath);
            var installer = new DotNetNuke.Services.Installer.Installer(manifest, DNNrocketUtils.MapPath("/"), true);
            installer.Install();
        }

        /// <summary>
        /// UNTESTED.  Unsure if this will work.  [use CraeteModuleFromManifest(string manifestMapPath)]
        /// </summary>
        /// <param name="sRec"></param>
        /// <returns></returns>
        [Obsolete("UNTESTED.  Unsure if this will work.  Use CraeteModuleFromManifest(string manifestMapPath)")]
        public static bool CreateModuleDefinition(SimplisityRecord sRec)
        {
            try
            {

                //<genxml>
                //    <moduleowner>ModuleCompany</moduleowner>
                //    <modulefolder>ModuleFolder</modulefolder>
                //    <modulename>ModuleName</modulename>
                //    <modulefriendlyname>Firendly Module Name</modulefriendlyname>
                //    <moduledescription>Rocket Module</moduledescription>
                //    <moduletitle>Module Title</moduletitle>
                //</genxml>

                var className = sRec.GetXmlProperty("genxml/moduleowner") + "." + sRec.GetXmlProperty("genxml/modulename");
                var controlName = sRec.GetXmlProperty("genxml/modulename");
                var moduleOwner = sRec.GetXmlProperty("genxml/moduleowner");
                var moduleFolder = sRec.GetXmlProperty("genxml/modulefolder");
                var moduleFriendlyName = sRec.GetXmlProperty("genxml/modulefriendlyname");
                var moduleDescription = sRec.GetXmlProperty("genxml/moduledescription");
                var moduleFullFolderMapPath = moduleOwner.Replace("/", "\\") + "\\" + moduleFolder.Replace("/", "\\");
                var moduleFullFolderRel = moduleOwner + "/" + moduleFolder;
                var moduleTitle = sRec.GetXmlProperty("genxml/moduletitle");

                if (PackageController.Instance.GetExtensionPackage(Null.NullInteger, p => p.Name == className) == null)
                {
                    //Create module folder
                    var moduleFolderPath = Globals.ApplicationMapPath + "\\DesktopModules\\" + moduleFullFolderMapPath;

                    if (!Directory.Exists(moduleFolderPath))
                    {
                        Directory.CreateDirectory(moduleFolderPath);
                    }

                    //Create module control
                    if (controlName != "")
                    {
                        //Create package
                        var objPackage = new PackageInfo();
                        objPackage.Name = className;
                        objPackage.FriendlyName = moduleFriendlyName;
                        objPackage.Description = moduleDescription;
                        objPackage.Version = new Version(1, 0, 0);
                        objPackage.PackageType = "Module";
                        objPackage.License = "";
                        objPackage.Owner = moduleOwner;
                        objPackage.Organization = moduleOwner;
                        objPackage.FolderName = "DesktopModules/" + moduleFullFolderRel;
                        objPackage.License = "The license for this package is not currently included within the installation file, please check with the vendor for full license details.";
                        objPackage.ReleaseNotes = "This package has no Release Notes.";
                        PackageController.Instance.SaveExtensionPackage(objPackage);

                        //Create desktopmodule
                        var objDesktopModule = new DesktopModuleInfo();
                        objDesktopModule.DesktopModuleID = Null.NullInteger;
                        objDesktopModule.ModuleName = className;
                        objDesktopModule.FolderName = moduleFullFolderRel;
                        objDesktopModule.FriendlyName = moduleFriendlyName;
                        objDesktopModule.Description = moduleDescription;
                        objDesktopModule.IsPremium = false;
                        objDesktopModule.IsAdmin = false;
                        objDesktopModule.Version = "01.00.00";
                        objDesktopModule.BusinessControllerClass = "";
                        objDesktopModule.CompatibleVersions = "";
                        objDesktopModule.AdminPage = "";
                        objDesktopModule.HostPage = "";
                        objDesktopModule.Dependencies = "";
                        objDesktopModule.Permissions = "";
                        objDesktopModule.PackageID = objPackage.PackageID;
                        objDesktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(objDesktopModule, false, true);
                        objDesktopModule = DesktopModuleController.GetDesktopModule(objDesktopModule.DesktopModuleID, Null.NullInteger);

                        //Add OwnerName to the DesktopModule taxonomy and associate it with this module
                        var vocabularyId = -1;
                        var termId = -1;
                        var objTermController = DotNetNuke.Entities.Content.Common.Util.GetTermController();
                        var objTerms = objTermController.GetTermsByVocabulary("Module_Categories");
                        foreach (Term term in objTerms)
                        {
                            vocabularyId = term.VocabularyId;
                            if (term.Name == moduleOwner)
                            {
                                termId = term.TermId;
                            }
                        }
                        if (termId == -1)
                        {
                            termId = objTermController.AddTerm(new Term(vocabularyId) { Name = moduleOwner });
                        }
                        var objTerm = objTermController.GetTerm(termId);
                        var objContentController = DotNetNuke.Entities.Content.Common.Util.GetContentController();
                        var objContent = objContentController.GetContentItem(objDesktopModule.ContentItemId);
                        objTermController.AddTermToContent(objTerm, objContent);

                        //Add desktopmodule to all portals
                        DesktopModuleController.AddDesktopModuleToPortals(objDesktopModule.DesktopModuleID);

                        //Create module definition
                        var objModuleDefinition = new ModuleDefinitionInfo();
                        objModuleDefinition.ModuleDefID = Null.NullInteger;
                        objModuleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID;
                        // need core enhancement to have a unique DefinitionName  
                        objModuleDefinition.FriendlyName = className;
                        //objModuleDefinition.FriendlyName = txtModule.Text;
                        //objModuleDefinition.DefinitionName = GetClassName();
                        objModuleDefinition.DefaultCacheTime = 0;
                        objModuleDefinition.ModuleDefID = ModuleDefinitionController.SaveModuleDefinition(objModuleDefinition, false, true);

                        //Create modulecontrol
                        var objModuleControl = new ModuleControlInfo();
                        objModuleControl.ModuleControlID = Null.NullInteger;
                        objModuleControl.ModuleDefID = objModuleDefinition.ModuleDefID;
                        objModuleControl.ControlKey = "";
                        objModuleControl.ControlSrc = "DesktopModules/" + moduleFullFolderRel + "/" + controlName;
                        objModuleControl.ControlTitle = "";
                        objModuleControl.ControlType = SecurityAccessLevel.View;
                        objModuleControl.HelpURL = "";
                        objModuleControl.IconFile = "";
                        objModuleControl.ViewOrder = 0;
                        objModuleControl.SupportsPartialRendering = false;
                        objModuleControl.SupportsPopUps = false;
                        ModuleControlController.AddModuleControl(objModuleControl);

                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                LogUtils.LogException(exc);
            }
            return false;
        }
        public static void AddModuleEditRights(int tabId, int moduleId, string roleName)
        {
            //get the current ModuleInfo 
            ModuleInfo newModule = ModuleController.Instance.GetModule(moduleId, tabId, false);
            if (newModule != null)
            {
                var roleInfo = DNNrocketUtils.GetRoleByName(newModule.PortalID, roleName);
                if (roleInfo != null)
                {

                    //add admin edit permission
                    ModulePermissionInfo modulePermissionInfo2 = new ModulePermissionInfo();
                    modulePermissionInfo2.ModuleID = moduleId;
                    modulePermissionInfo2.AllowAccess = true;
                    //view permission id, 1 = view, 2 = edit
                    modulePermissionInfo2.PermissionID = 2;
                    //administrator role id (from Roles table in dnn database)
                    modulePermissionInfo2.RoleID = roleInfo.RoleID;

                    //add the ModulePermissionInfo to the module
                    newModule.ModulePermissions.Add(modulePermissionInfo2);

                    //save the permissions
                    ModulePermissionController.SaveModulePermissions(newModule);

                    //clear the dnn cache (if it is the current module, not a new one)
                    DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(tabId);
                    DotNetNuke.Common.Utilities.DataCache.ClearTabsCache(newModule.PortalID);
                    DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(newModule.PortalID, false);
                }
            }
        }
        public static bool HasModuleEditRights(int tabId, int moduleId)
        {
            try
            {
                if (tabId <= 0 && moduleId <= 0) return true;
                if (tabId == 0) return false;
                if (moduleId == 0) return false;
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);
                if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "MANAGE", moduleInfo))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public static void DeleteAllTabModules(int pageid)
        {
            var mList = ModuleController.Instance.GetTabModules(pageid);
            foreach (var m in mList)
            {
                ModuleController.Instance.DeleteModule(m.Key);
            }
        }

        public static void AddExistingModule(int portalId, int moduleId, int tabId, string paneName, int position, string align, bool cloneModule)
        {
            ModuleInfo moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);

            int userID = -1;
                UserInfo user = UserController.Instance.GetCurrentUserInfo();
                if (((user != null)))
                {
                    userID = user.UserID;
                }

            if ((moduleInfo != null))
            {
                // clone the module object ( to avoid creating an object reference to the data cache )
                ModuleInfo newModule = moduleInfo.Clone();

                newModule.UniqueId = Guid.NewGuid(); // Cloned Module requires a different uniqueID
                newModule.TabModuleID = Null.NullInteger;
                newModule.TabID = tabId;
                newModule.ModuleOrder = position;
                newModule.PaneName = paneName;
                newModule.Alignment = align;

                //copy tab module settings
                newModule.TabModuleSettings.Clear();
                foreach (var key in moduleInfo.TabModuleSettings.Keys)
                {
                    newModule.TabModuleSettings.Add(key, moduleInfo.TabModuleSettings[key]);
                }

                ModuleController.Instance.AddModule(newModule);
            }
        }

        public static int AddNewModuleToTab(int portalId, int tabId, string title, int desktopModuleId, string paneName, int position, int permissionType, string align)
        {
            try
            {
                DesktopModuleInfo desktopModule;
                if (!DesktopModuleController.GetDesktopModules(portalId).TryGetValue(desktopModuleId, out desktopModule))
                {
                    throw new ArgumentException("desktopModuleId");
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }

            foreach (ModuleDefinitionInfo objModuleDefinition in
                ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(desktopModuleId).Values)
            {
                var objModule = new ModuleInfo();
                objModule.Initialize(portalId);

                objModule.PortalID = portalId;
                objModule.TabID = tabId;
                objModule.ModuleOrder = position;
                objModule.ModuleTitle = title;
                objModule.PaneName = paneName;
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID;

                ModuleController.Instance.InitialModulePermission(objModule, objModule.TabID, permissionType);

                objModule.CultureCode = Null.NullString;
                objModule.AllTabs = false;
                objModule.Alignment = align;

                return ModuleController.Instance.AddModule(objModule);
            }
            return -1;
        }

        public static int GetDesktopModuleId(int portalId, string definitionName)
        {
            var m = DesktopModuleController.GetDesktopModuleByModuleName(definitionName, portalId);
            if (m != null) return m.DesktopModuleID;
            return -1;
        }

    }
}
