using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using Rocket.AppThemes.Componants;

namespace RocketRemoteMod
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private RemoteLimpet _remoteParams;
        private int _moduleid;
        private int _tabid;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var strOut = "";
            var rtnDic = new Dictionary<string, object>();

            if (UserUtils.IsInRole("Administrator"))
            {
                switch (paramCmd)
                {
                    case "rocketremotemod_setup":
                        strOut = GetSetup();
                        break;
                    case "rocketremotemod_getdashboard":
                        strOut = GetDashBoard();
                        break;
                    case "rocketremotemod_adminsettings":
                        strOut = AdminSettings();
                        break;
                    case "rocketremotemod_saveconfig":
                        SaveConfig();
                        strOut = AdminSettings();
                        break;                        

                }
            }
            else
            {
                strOut = UserUtils.LoginForm(new SimplisityInfo(), new SimplisityInfo(), "login", UserUtils.GetCurrentUserId());
            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;


        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);

            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _passSettings = new Dictionary<string, string>();

            _remoteParams = new RemoteLimpet(_moduleid, _systemData.SystemKey);

            return paramCmd;
        }
        private void SaveConfig()
        {
            if (_systemData.DebugMode) _remoteParams.CacheDisbaled = true;
            _remoteParams.TabId = _tabid;
            _remoteParams.SystemKey = _systemData.SystemKey;
            _remoteParams.Save(_postInfo);
            _passSettings.Remove("saved");
            _passSettings.Add("saved", "true");

            // update module with a better name
            DNNrocketUtils.UpdateModuleTitle(_tabid, _moduleid, _remoteParams.Name + ":" + _moduleid);

            CacheFileUtils.ClearAllCache();
        }
        public string AdminSettings()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "adminsettings.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _remoteParams, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string GetDashBoard()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _remoteParams, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string GetSetup()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                _passSettings.Remove("moduleid");
                _passSettings.Add("moduleid", _moduleid.ToString());
                _passSettings.Remove("tabid");
                _passSettings.Add("tabid", _tabid.ToString());
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", _systemData.DebugMode);
                return RenderRazorUtils.RazorDetail(razorTempl, _rocketInterface.Info, _passSettings, null, _systemData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }

}
