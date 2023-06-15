using DNNrocketAPI.Components;
using Simplisity;
using System;
using RocketPortal.Components;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using RocketTools.Components;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Common.Utilities;

namespace RocketTools.API
{
    public partial class StartConnect
    {
        #region "Roles"

        public String GetRoles()
        {
            var info = GetCachedInfo(_pageRef);
            if (info.GUIDKey == "new") return "reload"; // we have lost the cache and page data, reload and start agian.

            info.RemoveRecordList("rolelist");
            var l = UserUtils.GetRoles(_portalId);
            foreach (var m in l)
            {
                var sRec = new SimplisityRecord();
                sRec.SetXmlProperty("genxml/roleid", m.Key.ToString());
                sRec.SetXmlProperty("genxml/rolename", m.Value);
                info.AddRecordListItem("rolelist", sRec);
            }
            var razorTempl = _appThemeSystem.GetTemplate("roleselectsection.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String GetModules()
        {
            var info = GetCachedInfo(_pageRef);
            if (info.GUIDKey == "new") return "reload"; // we have lost the cache and page data, reload and start agian.

            info.RemoveRecordList("modulelist");
            foreach (var t in info.GetRecordList("tabtreeview"))
            {
                if (t.GetXmlPropertyBool("genxml/clone")) // use clone checkbox name
                {
                    var tabid = t.GetXmlPropertyInt("genxml/tabid");
                    var l = RocketToolsUtils.GetTabModuleTitles(tabid);
                    foreach (var m in l)
                    {
                        var sRec = new SimplisityRecord();
                        sRec.SetXmlProperty("genxml/moduleid", m.Key.ToString());
                        sRec.SetXmlProperty("genxml/moduletitle", t.GetXmlProperty("genxml/tabname") + ": " + m.Value);
                        info.AddRecordListItem("modulelist", sRec);
                    }
                }
            }
            var razorTempl = _appThemeSystem.GetTemplate("rolesmodulesection.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String RolesAdmin()
        {
            var razorTempl = _appThemeSystem.GetTemplate("roles.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String RolesOK()
        {
            var razorTempl = _appThemeSystem.GetTemplate("rolesok.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, null, null, null, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ApplyRoles()
        {
            var info = GetCachedInfo(_pageRef);
            foreach (var r in info.GetRecordList("rolelist"))
            {
                var roleid = r.GetXmlPropertyInt("genxml/roleid");
                if (roleid > 0)
                {
                    foreach (var m in info.GetRecordList("modulelist"))
                    {
                        var moduleid = m.GetXmlPropertyInt("genxml/moduleid");
                        if (moduleid > 0)
                        {
                            if (r.GetXmlPropertyBool("genxml/addrole"))
                                AddRoleToModule(_portalId, moduleid, roleid);
                            else
                                RemoveRoleToModule(_portalId, moduleid, roleid);
                        }
                    }
                }
            }
            //DNNrocketUtils.RecycleApplicationPool();
            return RolesOK();
        }

        public static void AddRoleToModule(int portalId, int moduleid, int roleid)
        {
            var moduleInfo = GetModuleInfo(moduleid);
            var roleexist = false;
            var permissionID = -1;
            var PermissionsList2 = moduleInfo.ModulePermissions.ToList();
            var role = RoleController.Instance.GetRoleById(portalId, roleid);
            if (role != null)
            {
                var permissionController = new PermissionController();

                var editPermissionsList = permissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "EDIT");
                PermissionInfo editPermisison = null;
                //Edit
                if (editPermissionsList != null && editPermissionsList.Count > 0)
                {
                    editPermisison = (PermissionInfo)editPermissionsList[0];
                }


                foreach (var p in PermissionsList2)
                {
                    if (p.RoleName == role.RoleName)
                    {
                        permissionID = p.PermissionID;
                        p.AllowAccess = true;
                        roleexist = true;

                        var modulePermission = new ModulePermissionInfo(editPermisison);
                        modulePermission.RoleID = role.RoleID;
                        modulePermission.AllowAccess = true;
                        moduleInfo.ModulePermissions.Add(modulePermission);
                    }
                }

                // ADD Role
                if (!roleexist)
                {
                    ArrayList systemModuleEditPermissions = permissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "EDIT");
                    foreach (PermissionInfo permission in systemModuleEditPermissions)
                    {
                        if (permission.PermissionKey == "EDIT")
                        {
                            var objPermission = new ModulePermissionInfo(permission)
                            {
                                ModuleID = moduleInfo.DesktopModuleID,
                                RoleID = role.RoleID,
                                RoleName = role.RoleName,
                                AllowAccess = true,
                                UserID = Null.NullInteger,
                                DisplayName = Null.NullString
                            };
                            var permId = moduleInfo.ModulePermissions.Add(objPermission, true);
                            ModuleController.Instance.UpdateModule(moduleInfo);
                        }
                    }
                }


                // Check for DEPLOY 
                // This was added for upgrade on module.  I'm unsure if it's still required.
                //if (!roleexist)
                //{
                //    ArrayList permissions = PermissionController.GetPermissionsByPortalDesktopModule();
                //    foreach (PermissionInfo permission in permissions)
                //    {
                //        if (permission.PermissionKey == "DEPLOY")
                //        {
                //            var objPermission = new ModulePermissionInfo(permission)
                //            {

                //                ModuleID = moduleInfo.DesktopModuleID,
                //                RoleID = role.RoleID,
                //                RoleName = role.RoleName,
                //                AllowAccess = true,
                //                UserID = Null.NullInteger,
                //                DisplayName = Null.NullString
                //            };
                //            var permId = moduleInfo.ModulePermissions.Add(objPermission, true);
                //            ModuleController.Instance.UpdateModule(moduleInfo);
                //        }
                //    }
                //}


            }
        }
        public static void RemoveRoleToModule(int portalId, int moduleid, int roleid)
        {
            var moduleInfo = GetModuleInfo(moduleid);
            var roleexist = false;
            var permissionID = -1;
            var PermissionsList2 = moduleInfo.ModulePermissions.ToList();
            var role = RoleController.Instance.GetRoleById(portalId, roleid);
            if (role != null)
            {
                foreach (var p in PermissionsList2)
                {
                    if (p.RoleName == role.RoleName)
                    {
                        permissionID = p.PermissionID;
                        roleexist = true;
                    }
                }
                if (roleexist && permissionID > -1)
                {
                    moduleInfo.ModulePermissions.Remove(permissionID, role.RoleID, Null.NullInteger);
                    ModuleController.Instance.UpdateModule(moduleInfo);
                }
            }
        }
        private static ModuleInfo GetModuleInfo(int moduleId)
        {
            var objMCtrl = new DotNetNuke.Entities.Modules.ModuleController();
            var objMInfo = objMCtrl.GetModule(moduleId);
            return objMInfo;
        }



        #endregion

    }

}
