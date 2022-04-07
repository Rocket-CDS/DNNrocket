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
            var xmlOut = "";

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
                    strOut = GetPortalList();
                    break;
                case "dashboard_activesystempanel":
                    strOut = ActiveSystemPanel();
                    break;



                case "dataclients_getlist":
                    strOut = GetDataClientList();
                    break;
                case "dataclients_delete":
                    strOut = DeleteDataClient();
                    break;
                case "dataclients_toggleactive":
                    strOut = DataClientActive();
                    break;
                case "dataclients_register":
                    strOut = GetDataClientRegister();
                    break;
                case "dataclients_getsystems":
                    xmlOut = ActiveSystemJson();
                    break;


                case "organisations_list":
                    strOut = GetOrgList();
                    break;
                case "organisations_save":
                    strOut = SaveOrg();
                    break;
                case "organisations_add":
                    strOut = AddOrg();
                    break;
                case "organisations_delete":
                    strOut = DeleteOrg();
                    break;



                case "rocketportal_get":
                    strOut = GetDashboard();
                    break;
                case "rocketportal_login":
                    _userParams.TrackClear(_systemData.SystemKey);
                    //strOut = UserUtils.LoginForm(_systemData.SystemKey, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    strOut = ReloadPage();
                    break;



                case "portal_addmanager":
                    strOut = AddManager();
                    break;
                case "portal_createmanager":
                    strOut = CreateManager();
                    break;
                case "portal_unauthuser":
                    strOut = UnAuthoriseUser();
                    break;
                case "portal_authuser":
                    strOut = AuthoriseUser();
                    break;
                case "portal_deleteuser":
                    strOut = DeleteUser();
                    break;                    
                case "portal_actionprovider":
                    strOut = LocalUtils.RunActionProvider(_postInfo);
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
            }


            // -----------------------------------------------------------------------
            // if we have changed language, reset the editlang.  The _nextLang is defined on the "InitCmd" function.
            if (_nextLang != _editLang) DNNrocketUtils.SetEditCulture(_nextLang);
            // -----------------------------------------------------------------------
            if (xmlOut != "") rtnDic.Add("outputxml", xmlOut);
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

            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid == 0) portalid = PortalUtils.GetCurrentPortalId();
            _portalData = new PortalLimpet(portalid);

            // SECURITY --------------------------------
            if (paramCmd == "dataclients_register" || paramCmd == "dataclients_getsystems")
            {
                var sk = _paramInfo.GetXmlProperty("genxml/remote/securitykey");
                if (_portalData.SecurityKey != sk) return "";
            }
            else
            {
                if (!UserUtils.IsInRole("Registered Users")) return "rocketportal_login";
            }
            // SECURITY --------------------------------

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
            var razorTempl = _appThemeSystem.GetTemplate("ActiveSystemPanel.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, null, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string ActiveSystemJson()
        {
            var rtnxml = "<root>";
            var l = _portalData.GetSystems();
            foreach (var s in l)
            {
                rtnxml += s.Record.ToXmlItem();
            }
            rtnxml += "</root>";
            return rtnxml;
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
        private string ReloadPage()
        {
            try
            {
                // user does not have access, logoff
                UserUtils.SignOut();

                var razorTempl = _appThemeSystem.GetTemplate("Reload.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, _portalData, _passSettings, _sessionParams, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
