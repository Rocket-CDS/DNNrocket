using DNNrocketAPI.Components;
using Simplisity;
using System;
using RocketPortal.Components;

namespace RocketPortal.API
{
    public partial class StartConnect
    {
        private string CreatePortal()
        {
            var globalData = new SystemGlobalData();
            if (globalData.RootDomain == "") return "Invalid Root Domain.  Update Global Settings.";

            var portalurl = GeneralUtils.UrlFriendly(GeneralUtils.GetGuidKey()) + "." + globalData.RootDomain;
            var engineurl = portalurl;

            int portalAdminUserId = -1;
            var userList = UserUtils.GetSuperUsers();
            if (userList.Count >= 1) portalAdminUserId = userList[0].GetXmlPropertyInt("user/userid");
            if (portalAdminUserId > 0)
            {
                var portalid = PortalUtils.CreatePortal("", engineurl, portalAdminUserId);
                if (portalid > 0)
                {
                    // Add HomePage Skin and Modules
                    var homeTabId = PagesUtils.GetHomePage(portalid, DNNrocketUtils.GetCurrentCulture());
                    if (homeTabId >= 0)
                    {
                        PagesUtils.AddPageSkin(portalid, homeTabId, "rocketportal", "rockethome.ascx");
                        ModuleUtils.DeleteAllTabModules(homeTabId);
                        //var dtid = ModuleUtils.GetDesktopModuleId(portalid, "RocketSystem");
                        //if (dtid > -1) ModuleUtils.AddNewModuleToTab(portalid, homeTabId, "RocketSystem", dtid, "", 0, 0, "");
                    }

                    DNNrocketUtils.CreateDefaultRocketRoles(portalid);

                    // add portal record
                    var portalData = new PortalLimpet(portalid);
                    portalData.Record.SetXmlProperty("genxml/textbox/name", "");
                    portalData.EngineUrl = engineurl;
                    portalData.Update();
                    _portalData = new PortalLimpet(portalid);

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
                }
            }
            return GetPortalDetail();
        }
        private string SavePortal()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid"); 
            if (portalId >= 0)
            {
                var portalData = new PortalLimpet(portalId);
                if (portalData.PortalId >= 0) portalData.Save(_postInfo);

                // Create a SysAdmin page, with subpages of each system.
                if (!PagesUtils.PageExists(portalId, "SysAdmin"))
                {
                    var tabid = PagesUtils.CreatePage(portalId, "SysAdmin");
                    PagesUtils.AddPageSkin(portalId, tabid, "rocketportal", "rockethome.ascx");
                }
                var sysAdminTabId = PagesUtils.GetPageByTabPath(portalId, "//SysAdmin");
                if (sysAdminTabId > 0 )
                {
                    foreach (var s in portalData.GetSystems())
                    {
                        if (PagesUtils.GetPageByTabPath(portalId, "//SysAdmin//" + s.SystemKey) == -1)
                        {
                            var tabid = PagesUtils.CreatePage(portalId, s.SystemKey, sysAdminTabId);
                            PagesUtils.AddPagePermissions(portalId, tabid, DNNrocketRoles.Manager);
                            PagesUtils.AddPagePermissions(portalId, tabid, DNNrocketRoles.Editor);
                            PagesUtils.AddPagePermissions(portalId, tabid, DNNrocketRoles.ClientEditor);
                            PagesUtils.AddPageSkin(portalId, tabid, "rocketportal", "rocketadmin.ascx");
                        }

                        // Create the system record in the DB.
                        var systemData = new SystemLimpet(s.SystemKey);
                        if (!systemData.Exists)
                        {
                            var interfacekey = "rocketsystem";
                            var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                            _paramInfo.SetXmlProperty("genxml/hidden/newportalid", portalId.ToString());
                            var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_init", systemData.SystemInfo, rocketInterface, _postInfo, _paramInfo, "/DesktopModules/DNNrocket/api", "");
                        }
                    }
                }
                _portalData = new PortalLimpet(portalId); // reload portal data after save (for language change)               
                return GetPortalDetail();
            }
            return "Invalid PortalId";
        }
        private string DeletePortal()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var PortalData = new PortalLimpet(portalId);
                foreach (var s in PortalData.SystemDataList.GetSystemActiveList())
                {
                    // delete system data
                    var systemData = new SystemLimpet(s.SystemKey);
                    var interfacekey = "rocketsystem";
                    var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                    var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_delete", systemData.SystemInfo, rocketInterface, _postInfo, _paramInfo, "/DesktopModules/DNNrocket/api", "");
                }
                PortalUtils.DeletePortal(portalId); 
                //DNNrocketUtils.RecycleApplicationPool();
                PortalData.Delete();
                _userParams.TrackClear(_systemData.SystemKey);
                return GetPortalList();
            }
            return GetPortalDetail();
        }
        private String GetPortalList()
        {
            try
            {
                var portalList = new PortalLimpetList(_paramInfo);
                var razorTempl = _appThemeSystem.GetTemplate("PortalList.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, portalList, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private String ValidatePortals()
        {
            if (!UserUtils.IsSuperUser()) return "";
            var portalList = new PortalLimpetList(_paramInfo);
            portalList.Validate();
            return GetDashboard();
        }
        private String GetPortalDetail()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("PortalDetail.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string UnAuthoriseUser()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1) UserUtils.UnAuthoriseUser(portalId, userid);
            }
            return GetPortalDetail();
        }
        private string AuthoriseUser()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1) UserUtils.AuthoriseUser(portalId, userid);
            }
            return GetPortalDetail();
        }
        private string DeleteUser()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1) UserUtils.DeleteUser(portalId, userid);
            }
            return GetPortalDetail();
        }
        private string CreateManager()
        {
            var statusString = "";
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid >= 0)
            {
                try
                {
                    var manageremail = _postInfo.GetXmlProperty("genxml/textbox/email");
                    var managerpassword = _postInfo.GetXmlProperty("genxml/textbox/password");
                    if (managerpassword != "")
                    {
                        statusString = UserUtils.CreateUser(portalid, manageremail, manageremail);
                        if (statusString == "" && managerpassword != "" && manageremail != "")
                        {
                            var userInfo = UserUtils.GetUserDataByEmail(portalid, manageremail);
                            if (userInfo != null)
                            {
                                UserUtils.ChangePassword(portalid, userInfo.UserId, managerpassword);

                                var rolelist = UserUtils.GetRoles(portalid);
                                foreach (var r in rolelist)
                                {
                                    if (r.Value == DNNrocketRoles.Manager)
                                    {
                                        UserUtils.AddUserRole(portalid, userInfo.UserId, r.Key);
                                    }
                                }
                            }
                            return GetPortalDetail();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            return AddManager(false, statusString);
        }
        private String AddManager(bool success = true, string statusMsg = "")
        {
            try
            {
                _passSettings.Add("success", success.ToString());
                _passSettings.Add("statusmsg", statusMsg);                
                var razorTempl = _appThemeSystem.GetTemplate("PortalAddManager.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


    }

}
