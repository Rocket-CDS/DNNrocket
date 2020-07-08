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
        private CommandSecurity _commandSecurity;
        private DNNrocketInterface _rocketInterface;
        private SystemData _systemData;
        private Dictionary<string, string> _passSettings;
        private ModuleParams _moduleParams;
        private string _editLang;
        private int _moduleid;
        private int _tabid;
        private AppThemeDataList _appThemeDataList;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

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
                    case "rocketremotemod_saveconfig":
                        SaveConfig();
                        strOut = GetDashBoard();
                        break;                        

                }
            }
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;


        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _systemData = new SystemData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);

            _postInfo = postInfo;
            // set editlang from url param or cache
            _editLang = DNNrocketUtils.GetEditCulture();

            _paramInfo = paramInfo;

            _editLang = DNNrocketUtils.GetEditCulture();


            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _passSettings = new Dictionary<string, string>();

            _moduleParams = new ModuleParams(_moduleid, _systemData.SystemKey);

            return paramCmd;
        }
        private void SaveConfig()
        {
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.RemotePortalKey = _postInfo.GetXmlProperty("genxml/hidden/remoteportalkey");
            _moduleParams.RemoteSystemKey = _postInfo.GetXmlProperty("genxml/hidden/remotesystemkey");            
            _moduleParams.RemoteCmd = _postInfo.GetXmlProperty("genxml/hidden/remotecmd");
            _moduleParams.RemoteTemplate = _postInfo.GetXmlProperty("genxml/hidden/remotetemplate");
            _moduleParams.RemoteHeaderCmd = _postInfo.GetXmlProperty("genxml/hidden/remoteheadercmd");
            _moduleParams.RemoteHeaderTemplate = _postInfo.GetXmlProperty("genxml/hidden/remoteheadertemplate");
            _moduleParams.ModuleType = "RocketRemoteMod";
            _moduleParams.APIurl = _postInfo.GetXmlProperty("genxml/hidden/apiurl");
            _moduleParams.CacheDisbaled = _postInfo.GetXmlPropertyBool("genxml/hidden/disablecache");

            _moduleParams.AppThemeFolder = _postInfo.GetXmlProperty("genxml/hidden/appthemefolder");
            _moduleParams.AppThemeVersion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");

            if (_systemData.DebugMode) _moduleParams.CacheDisbaled = true;
            _moduleParams.TabId = _tabid;
            _moduleParams.SystemKey = _systemData.SystemKey;

            _moduleParams.Save();

            _passSettings.Remove("saved");
            _passSettings.Add("saved", "true");

            // update module with a better name
            DNNrocketUtils.UpdateModuleTitle(_tabid, _moduleid, _moduleParams.Name + ":" + _moduleid);

            CacheFileUtils.ClearAllCache();
        }
        public String GetDashBoard()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return DNNrocketUtils.RazorDetail(razorTempl, _moduleParams, _passSettings, null, true);
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
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", _systemData.DebugMode);
                return DNNrocketUtils.RazorDetail(razorTempl, _rocketInterface.Info, _passSettings, null, _systemData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }

}
