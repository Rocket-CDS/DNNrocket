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
            if (userList.Count > 1) portalAdminUserId = userList[0].GetXmlPropertyInt("user/userid");

            var portalid = PortalUtils.CreatePortal("", engineurl, portalAdminUserId);
            if (portalid > 0)
            {
                // Add HomePage Skin and Modules
                var homeTabId = PagesUtils.GetHomePage(portalid, DNNrocketUtils.GetCurrentCulture());
                if (homeTabId >= 0)
                {
                    PagesUtils.AddPageSkin(portalid, homeTabId, "rocketportal", "rockethome.ascx");
                    ModuleUtils.DeleteAllTabModules(homeTabId);
                    var dtid = ModuleUtils.GetDesktopModuleId(portalid, "RocketSystem");
                    if (dtid > -1) ModuleUtils.AddNewModuleToTab(portalid, homeTabId, "RocketSystem", dtid, "", 0, 0, "");
                }

                DNNrocketUtils.CreateDefaultRocketRoles(portalid);

                // add portal record
                var portalData = new PortalLimpet(portalid);
                var systemkey = _postInfo.GetXmlProperty("genxml/radio/systemkey");
                portalData.Record.SetXmlProperty("genxml/radio/systemkey", systemkey);
                portalData.Record.SetXmlProperty("genxml/textbox/name", "");
                portalData.EngineUrl = engineurl;
                portalData.Update();
                _portalData = new PortalLimpet(portalid);

                // Create the system record in the DB.
                var systemData = new SystemLimpet(_portalData.SystemKey);
                var interfacekey = "rocketsystem";
                var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                _paramInfo.SetXmlProperty("genxml/hidden/newportalid", portalid.ToString());
                var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_init", systemData.SystemInfo, rocketInterface, _postInfo, _paramInfo, "/DesktopModules/DNNrocket/api", "");               

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
                _portalData = new PortalLimpet(portalId); // reload portal data after save (for langauge change)
                //CacheUtils.ClearAllCache();  
                return GetPortalDetail();
            }
            return "Invalid PortalId";
        }
        private string DeletePortal()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                // delete system data
                var systemData = new SystemLimpet(_portalData.SystemKey);
                var interfacekey = "rocketsystem";
                var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_delete", systemData.SystemInfo, rocketInterface, _postInfo, _paramInfo, "/DesktopModules/DNNrocket/api", "");


                var PortalShop = new PortalLimpet(portalId);
                PortalUtils.DeletePortal(portalId); // Delete base portal will crash install.
                //DNNrocketUtils.RecycleApplicationPool();
                PortalShop.Delete();
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
            try
            {
                var portalList = new PortalLimpetList(_paramInfo);
                portalList.Validate();
                return GetDashboard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
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
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid >= 0)
            {
                try
                {
                    var manageremail = _postInfo.GetXmlProperty("genxml/textbox/email");
                    var managerpassword = _postInfo.GetXmlProperty("genxml/textbox/password");
                    if (managerpassword != "")
                    {
                        var userId = UserUtils.CreateUser(portalid, manageremail, manageremail);
                        if (userId >= 0 && managerpassword != "")
                        {
                            UserUtils.ResetAndChangePassword(portalid, userId, managerpassword);

                            var rolelist = UserUtils.GetRoles(portalid);
                            foreach (var r in rolelist)
                            {
                                if (r.Value == DNNrocketRoles.Manager)
                                {
                                    UserUtils.AddUserRole(portalid, userId, r.Key);
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
            return AddManager(false);
        }
        private String AddManager(bool success = true)
        {
            try
            {
                _passSettings.Add("success", success.ToString());
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
