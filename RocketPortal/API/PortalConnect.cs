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
            var portalname = _postInfo.GetXmlProperty("genxml/name");
            var manageremail = _postInfo.GetXmlProperty("genxml/manageremail");
            var managerpassword = _postInfo.GetXmlProperty("genxml/managerpassword");
            var portalurl = _postInfo.GetXmlProperty("genxml/portalurl");
            var engineurl = portalurl;

            var portalid = PortalUtils.CreatePortal(portalname, engineurl);
            if (portalid > 0)
            {
                DNNrocketUtils.CreateDefaultRocketRoles(portalid);

                var userId = UserUtils.CreateUser(portalid, manageremail, manageremail);
                if (userId >= 0)
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
                }

                var cmstabid = DNNrocketUtils.CreatePage(portalid, "cms");
                DNNrocketUtils.AddPagePermissions(portalid, cmstabid, "");
                DNNrocketUtils.AddPageSkin(portalid, cmstabid, "cms", "cms.ascx");

                var homeTabId = PagesUtils.GetHomePage(portalid, DNNrocketUtils.GetCurrentCulture());
                if (homeTabId >= 0)
                {
                    DNNrocketUtils.AddPageSkin(portalid, homeTabId, "rocketportal", "rockethome.ascx");
                    ModuleUtils.DeleteAllTabModules(homeTabId);
                }

                var accesstabid = DNNrocketUtils.CreatePage(portalid, "access");
                DNNrocketUtils.AddPagePermissions(portalid, accesstabid, "");

                DNNrocketUtils.RecycleApplicationPool();

                // add portal record
                var portalData = new PortalLimpet(portalid);
                portalData.Record.SetXmlProperty("genxml/textbox/name", portalname);
                portalData.EngineUrl = engineurl;
                portalData.Update();

                CreateCmsPage(portalid);
            }
            return GetPortalList();
        }
        public void CreateCmsPage(int portalId)
        {
            var cmstabid = DNNrocketUtils.CreatePage(portalId, "cms");
            DNNrocketUtils.AddPagePermissions(portalId, cmstabid, "");
            DNNrocketUtils.AddPageSkin(portalId, cmstabid, "cms", "cms.ascx");
        }
        private string SavePortal()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid"); 
            if (portalId >= 0)
            {
                var portalData = new PortalLimpet(portalId);
                if (portalData.PortalId >= 0) portalData.Save(_postInfo);
                _portalData = new PortalLimpet(portalId); // reload portal data after save (for langauge change)
                CacheUtils.ClearAllCache();  
                return GetPortalDetail();
            }
            return "Invalid PortalId";
        }
        private string DeletePortal()
        {
            var sitekey = _paramInfo.GetXmlProperty("genxml/hidden/sitekey"); // we may have passed selection
            var portalId = PortalUtils.GetPortalIdBySiteKey(sitekey);
            if (portalId >= 0)
            {
                var PortalShop = new PortalLimpet(portalId);
                PortalUtils.DeletePortal(portalId); // Delete base portal will crash install.
                DNNrocketUtils.RecycleApplicationPool();
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
        private string AssignHomeSkin()
        {
            if (!UserUtils.IsSuperUser()) return "FAIL";
            var homeTabId = PagesUtils.GetHomePage(0, DNNrocketUtils.GetCurrentCulture());
            if (homeTabId >= 0) DNNrocketUtils.AddPageSkin(0, homeTabId, "rocketportal", "rocketportal.ascx");
            return "OK";
        }


    }

}
