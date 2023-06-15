using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using DNNrocketAPI.Interfaces;
using RocketPortal.Components;
using RocketTools.Components;
using Simplisity;

namespace RocketTools.API
{
    public partial class StartConnect : IProcessCommand
    {
        private RocketInterface _rocketInterface;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private UserParams _userParams;
        private AppThemeDNNrocketLimpet _appThemeSystem;
        private Dictionary<string, object> _dataObjects;
        private PortalLimpet _portalData;
        private SessionParams _sessionParams;
        private string _pageRef;
        private int _portalId;
        private DNNrocketController _objCtrl;

        public Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; 
            var storeparamCmd = paramCmd;
            var rtnDic = new Dictionary<string, object>();

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

                switch (paramCmd)
                {
                    case "rockettools_adminpanel":
                        strOut = AdminPanel();
                        break;
                    case "rocketactions_getdisplay":
                        strOut = MainMenu();
                        break;                    
                    case "rockettools_login":
                        _userParams.TrackClear(_systemData.SystemKey);
                        strOut = ReloadPage();
                        break;


                    //  ========== CLONES ===================
                    case "rocketclones_getdisplay":
                        strOut = CloneDetail();
                        break;
                    case "rocketclones_getmodules":
                        strOut = GetCloneSelectModules();
                        break;
                    case "rocketclones_getdestination":
                        strOut = CloneDestination();
                        break;
                    case "rocketclones_clone":
                        SaveModules("clonemodules");
                        SaveTreeView("clonetreeview");
                        CloneModules();
                        strOut = ClonesOK();
                        break;



                    //  ========== ROLES ===================
                    case "rocketroles_roles":
                        strOut = RolesAdmin();
                        break;
                    case "rocketroles_getmodules":
                        SaveTreeView("tabtreeview");
                        strOut = GetModules();
                        break;
                    case "rocketroles_getroles":
                        SaveModules("modulelist");
                        strOut = GetRoles();
                        break;
                    case "rocketroles_applyroles":
                        SaveRoles("rolelist");
                        strOut = ApplyRoles();
                        break;
                    case "rocketroles_createdefaultroles":
                        DNNrocketUtils.CreateDefaultRocketRoles(_portalId);
                        strOut = RolesAdmin();
                        break;


                    //  ========== PL ===================
                    case "rocketpl_getdisplay":
                        strOut = PLDetail();
                        break;
                    case "rocketpl_pageview":
                        strOut = PageView();
                        break;
                    case "rocketpl_pageedit":
                        strOut = PageEdit();
                        break;
                    case "rocketpl_pagesave":
                        strOut = PageSave();
                        break;
                    case "rocketpl_pagedelete":
                        strOut = PageDelete();
                        break;
                    case "rocketpl_savesettings":
                        strOut = SaveSettings();
                        break;
                    case "rocketpl_validateurls":
                        RocketToolsUtils.ValidateUrls();
                        strOut = PLDetail();
                        break;
                    case "rocketpl_validatemeta":
                        RocketToolsUtils.ValidateMeta();
                        strOut = PLDetail();
                        break;
                    case "rocketpl_validate":
                        RocketToolsUtils.ValidateMeta();
                        RocketToolsUtils.ValidateUrls();
                        strOut = PLDetail();
                        break;
                    case "rocketpl_taburldelete":
                        DeleteTabUrl();
                        strOut = PLDetail();
                        break;
                    case "rocketpl_addmenuprovider":
                        strOut = AddMenuProvider();
                        break;
                    case "rocketpl_addqueryparams":
                        strOut = AddQueryParam();
                        break;





                    default:
                        strOut = "INVALID CMD";
                        break;

                }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;

        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _portalId = PortalUtils.GetPortalId();
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemData = new SystemLimpet(systemInfo.GetXmlProperty("genxml/systemkey"));
            _appThemeSystem = new AppThemeDNNrocketLimpet(_systemData.SystemKey);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _passSettings = new Dictionary<string, string>();
            _sessionParams = new SessionParams(_paramInfo);
            _userParams = new UserParams(_sessionParams.BrowserSessionId);
            _objCtrl = new DNNrocketController();

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

            _pageRef = _paramInfo.GetXmlProperty("genxml/hidden/pageref");

            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid == 0) portalid = PortalUtils.GetCurrentPortalId();
            _portalData = new PortalLimpet(portalid);

            // SECURITY --------------------------------
            if (!UserUtils.IsAdministrator()) return "rockettools_login";
            // SECURITY --------------------------------

            _dataObjects = new Dictionary<string, object>();
            _dataObjects.Add("appthemesystem", _appThemeSystem);
            _dataObjects.Add("portaldata", _portalData);
            _dataObjects.Add("systemdata", _systemData);

            return paramCmd;
        }

        #region "General"

        private string MainMenu()
        {
            var razorTempl = _appThemeSystem.GetTemplate("MainMenu.cshtml");
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
        private string ReloadPage()
        {
            // user does not have access, logoff
            UserUtils.SignOut();
            var razorTempl = _appThemeSystem.GetTemplate("Reload.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public SimplisityInfo GetCachedInfo(string pageRef)
        {
            var info = (SimplisityInfo)CacheUtils.GetCache(pageRef);
            if (info == null)
            {
                info = new SimplisityInfo();
                info.GUIDKey = "new";  // flag to check if we have lost the previous selection
            }
            return info;
        }
        #endregion



    }
}
