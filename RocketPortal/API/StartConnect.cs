using DNNrocketAPI;
using DNNrocketAPI.Components;
using RocketPortal.Components;
using Simplisity;
using System;
using System.Collections.Generic;

namespace RocketPortal.API
{
    public partial class StartConnect : APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private string _nextLang;
        private UserParams _userParams;
        private PortalLimpet _portalData;
        private SessionParams _sessionParams;
        private int _tabid;
        private int _moduleid;
        private string _editLang;
        private AppThemeDNNrocketLimpet _appThemeSystem;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);
            var rtnDic = new Dictionary<string, object>();

            var strOut = "";

            switch (paramCmd)
            {

                case "dashboard_clearallcache":
                    CacheFileUtils.ClearFileCacheAllPortals();
                    CacheUtils.ClearAllCache();
                    CacheUtilsDNN.ClearAllCache();
                    DNNrocketUtils.ClearAllCache();
                    strOut = "OK";
                    break;
                case "dashboard_activesystems":
                    strOut = ActiveSystems();
                    break;
                case "dashboard_adminpanel":
                    strOut = AdminPanel();
                    break;
                case "dashboard_get":
                    strOut = MyServices();
                    break;



                case "rocketportal_get":
                    strOut = GetDashboard();
                    break;
                case "rocketportal_login":
                    _userParams.TrackClear(_systemData.SystemKey);
                    strOut = UserUtils.LoginForm(_systemData.SystemKey, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    break;
                case "rocketportal_activesystems":
                    strOut = ActiveSystemPanel();
                    break;



                case "portal_actionprovider":
                    strOut = LocalUtils.RunActionProvider(_portalData, _postInfo);
                    break;
                case "portal_list":
                    strOut = GetPortalList();
                    break;
                case "portal_create":
                    strOut = CreatePortal();
                    break;
                case "portal_detail":
                    strOut = GetPortalDetail();
                    break;
                case "portal_save":
                    strOut = SavePortal();
                    break;
                case "portal_createcmspage":
                    CreateCmsPage(_paramInfo.GetXmlPropertyInt("genxml/hidden/portalid"));
                    strOut = GetPortalDetail();
                    break;
                case "portal_delete":
                    strOut = DeletePortal();
                    break;
                case "portal_validate":
                    strOut = ValidatePortals();
                    break;
                case "portal_addsetting":
                    _portalData.Record.AddListItem("settingsdata");
                    _portalData.Update();
                    strOut = GetPortalDetail();
                    break;
                case "portal_assignhomeskin":
                    strOut = AssignHomeSkin();
                    break;


            }


            // -----------------------------------------------------------------------
            // if we have changed language, reset the editlang.  The _nextLang is defined on the "InitCmd" function.
            if (_nextLang != _editLang) DNNrocketUtils.SetEditCulture(_nextLang);
            // -----------------------------------------------------------------------

            if (!rtnDic.ContainsKey("outputjson")) rtnDic.Add("outputhtml", strOut);

            return rtnDic;


        }


        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemData = new SystemLimpet("rocketportal");
            _rocketInterface = new RocketInterface(interfaceInfo);
            _appThemeSystem = new AppThemeDNNrocketLimpet(_systemData.SystemKey);
            _sessionParams = new SessionParams(_paramInfo);
            _userParams = new UserParams(_sessionParams.BrowserSessionId);
            _rocketInterface = new RocketInterface(interfaceInfo);

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);


            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _passSettings = new Dictionary<string, string>();

            // SECURITY --------------------------------
            if (paramCmd.StartsWith("dashboard_"))
            {
                if (!UserUtils.IsManager()) return "rocketportal_login";
            }
            else
            {
                if (!UserUtils.IsSuperUser()) return "rocketportal_login";
            }
            // SECURITY --------------------------------


            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid == 0) portalid = PortalUtils.GetCurrentPortalId();
            _portalData = new PortalLimpet(portalid);

            return paramCmd;
        }
        private string GetDashboard()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("Dashboard.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string AdminPanel()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("AdminPanel.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string ActiveSystemPanel()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("ActiveSystemPanel.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string ActiveSystems()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("ActiveSystems.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private String MyServices()
        {
            try
            {
                if (UserUtils.IsSuperUser()) return GetPortalList();

                var portalList = new PortalLimpetList(_paramInfo);
                var razorTempl = _appThemeSystem.GetTemplate("MyServices.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, portalList, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
