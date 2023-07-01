using DNNrocketAPI;
using DNNrocketAPI.Components;
using DNNrocketAPI.Interfaces;
using RocketPortal.Components;
using Simplisity;
using System;
using System.Collections.Generic;

namespace RocketPortal.API
{
    public partial class StartConnect : IProcessCommand
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
        private Dictionary<string, object> _dataObjects;
        private SystemGlobalData _globalData;

        public Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var strOut = "";
            var xmlOut = "";

            switch (paramCmd)
            {

                case "dashboard_clearallcache":
                    if (UserUtils.IsAdministrator())
                    {
                        CacheFileUtils.ClearFileCacheAllPortals();
                        CacheUtils.ClearAllCache();
                        CacheUtils.ClearAllCache();
                        DNNrocketUtils.ClearAllCache();
                    }
                    strOut = "OK";
                    break;
                case "dashboard_portalsystems":
                    strOut = portalsystems();
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


                case "appthemeprojects_list":
                    strOut = GetProjectList();
                    break;
                case "appthemeprojects_save":
                    strOut = SaveProject();
                    break;
                case "appthemeprojects_add":
                    strOut = AddProject();
                    break;
                case "appthemeprojects_delete":
                    strOut = DeleteProject();
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
                case "portal_addeditorrole":
                    strOut = AddRole(DNNrocketRoles.Editor);
                    break;
                case "portal_removeeditorrole":
                    strOut = RemoveRole(DNNrocketRoles.Editor);
                    break;
                case "portal_addmanagerrole":
                    strOut = AddRole(DNNrocketRoles.Manager);
                    break;
                case "portal_removemanagerrole":
                    strOut = RemoveRole(DNNrocketRoles.Manager);
                    break;                    
                case "portal_addadminrole":
                    strOut = AddRole(DNNrocketRoles.Administrators);
                    break;
                case "portal_removeadminrole":
                    strOut = RemoveRole(DNNrocketRoles.Administrators);
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
                case "portal_setdefaultlanguage":
                    strOut = UpdateDefaultLangauge();
                    break;
                case "portal_addlanguage":
                    strOut = AddLangauge();
                    break;
                case "portal_removelanguage":
                    strOut = RemoveLangauge();
                    break;
                case "portal_addsystem":
                    strOut = AddSystem();
                    break;
                case "portal_removesystem":
                    strOut = RemoveSystem();
                    break;
                case "portal_togglesystem":
                    strOut = ToggleSystem();
                    break;
                case "portal_resetsecurity":
                    strOut = ResetSecuirtyPortal();
                    break;
                case "portal_resetcodes":
                    strOut = ResetCodes();
                    break;
                case "portal_getsystems":
                    xmlOut = ActiveSystemXml(); 
                    break;                    

            }


            // -----------------------------------------------------------------------
            // if we have changed language, reset the editlang.  The _nextLang is defined on the "InitCmd" function.
            if (_nextLang != _editLang) DNNrocketUtils.SetEditCulture(_nextLang);
            // -----------------------------------------------------------------------

            var rtnDic = new Dictionary<string, object>();
            if (xmlOut != "") rtnDic.Add("remote-xml", xmlOut);
            rtnDic.Add("outputhtml", strOut);

            return rtnDic;

        }


        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemData = SystemSingleton.Instance("rocketportal");
            _rocketInterface = new RocketInterface(interfaceInfo);
            _appThemeSystem = new AppThemeDNNrocketLimpet(_systemData.SystemKey);
            _sessionParams = new SessionParams(_paramInfo);
            _userParams = new UserParams(_sessionParams.BrowserSessionId);
            _globalData = new SystemGlobalData();

            // Assign Language
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
            var sk = _paramInfo.GetXmlProperty("genxml/remote/securitykey");
            var ske = _paramInfo.GetXmlProperty("genxml/remote/securitykeyedit");

            if (paramCmd == "portal_getsystems" || paramCmd == "dataclients_getsystems")
            {
                if (!_portalData.SecurityKeyCheck(sk, ske)) return "";
            }
            else
            {
                if (!UserUtils.IsSuperUser()) return "rocketportal_login";
            }

            // SECURITY --------------------------------

            _dataObjects = new Dictionary<string, object>();
            _dataObjects.Add("appthemesystem", _appThemeSystem);
            _dataObjects.Add("portaldata", _portalData);
            _dataObjects.Add("systemdata", _systemData);
            _dataObjects.Add("websitebuild", new WebsiteBuild());
            _dataObjects.Add("globaldata", _globalData);
            


            return paramCmd;
        }
        private string GetDashboard()
        {
            var razorTempl = _appThemeSystem.GetTemplate("Dashboard.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string AdminPanel()
        {
            var razorTempl = _appThemeSystem.GetTemplate("AdminPanel.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string ActiveSystemPanel()
        {
            var razorTempl = _appThemeSystem.GetTemplate("ActiveSystemPanel.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, null, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string ActiveSystemXml()
        {
            var rtnxml = new SimplisityRecord();
            var l = _portalData.GetSystems();
            foreach (var s in l)
            {
                var sRec = new SimplisityRecord();
                sRec.SetXmlProperty("genxml/systemkey", s.Record.GetXmlProperty("genxml/systemkey"));
                sRec.SetXmlProperty("genxml/systemname", s.Record.GetXmlProperty("genxml/systemname"));
                rtnxml.AddRecordListItem("systems", sRec);
            }
            return rtnxml.ToXmlItem();
        }
        private string portalsystems()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("portalsystems.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string ReloadPage()
        {
            // user does not have access, logoff
            UserUtils.SignOut();
            var razorTempl = _appThemeSystem.GetTemplate("Reload.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

    }
}
