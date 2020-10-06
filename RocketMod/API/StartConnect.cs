using System;
using System.Collections.Generic;
using System.Linq;
using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using Rocket.AppThemes.Componants;
using RocketMod.Componants;
using System.IO;

namespace RocketMod
{
    public partial class StartConnect : DNNrocketAPI.APInterface
    {
        //private ModuleData _moduleData;
        private string _appthemeRelPath;
        private string _appthemeMapPath;
        private string _appthemeSystemRelPath;
        private string _appthemeSystemMapPath;
        private string _rocketModRelPath;
        private string _rocketModMapPath;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private ModuleParams _moduleParams;
        private ModuleParams _dataModuleParams;
        private int _tabid;
        private int _moduleid;
        private SystemLimpet _systemData;
        private string _systemKey;
        private Dictionary<string, string> _passSettings;
        private SettingsData _settingsData;
        private AppThemeModule _appThemeMod;
        private AppThemeModule _dataAppThemeMod;
        private AppThemeDataList _appThemeDataList;
        private UserParams _userParams;
        private string _tableName;
        private string _nextLang;
        private string _editLang;
        private string _currentLang;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var rtnDic = new Dictionary<string, object>();
            var downloadDict = new Dictionary<string, object>();

            switch (paramCmd)
            {
                case "rocketmod_login":
                    strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    break;
                case "rocketmod_getdata":
                    strOut = GetDisplay();
                    break;
                case "rocketmod_getsetupmenu":
                    strOut = GetSetup();
                    break;

                case "dashboard_get":
                    strOut = GetDashBoard();
                    break;

                case "rocketmodedit_selectapptheme":
                    strOut = GetSelectApp();
                    break;
                case "rocketmodedit_saveapptheme":
                    SaveAppTheme();
                    strOut = GetDashBoard();
                    break;
                case "rocketmodedit_saveappthemeconfig":
                    SaveAppTheme();
                    strOut = GetSettingSection();
                    break;
                case "rocketmodedit_saveappthemesetting":
                    SettingsSave();
                    strOut = GetDashBoard();
                    break;
                case "rocketmodedit_articlesearch":
                    strOut = GetArticleSingle();
                    break;
                case "rocketmodedit_editarticle":
                    strOut = GetArticleSingle();
                    break;
                case "rocketmodedit_savearticle":
                    SaveArticle(true);
                    strOut = GetArticleSingle();
                    break;
                case "rocketmodedit_addimage":
                    strOut = AddArticleImage();
                    break;
                case "rocketmodedit_adddocument":
                    strOut = AddArticleDocument();
                    break;
                case "rocketmodedit_addlink":
                    strOut = AddArticleLink();
                    break;
                case "rocketmodedit_saveconfig":
                    SaveConfig();
                    strOut = GetDashBoard();
                    break;
                case "rocketmodedit_reset":
                    strOut = ResetRocketMod(_moduleid);
                    break;
                case "rocketmodedit_resetdata":
                    strOut = ResetDataRocketMod(_dataModuleParams.ModuleId);
                    break;
                case "rocketmodedit_datasourcelist":
                    strOut = GetDataSourceList();
                    break;
                case "rocketmodedit_datasourceselect":
                    strOut = GetDataSourceSelect();
                    break;


                case "rocketmodsettings_edit":
                    strOut = EditSettingsData();
                    break;
                case "rocketmodsettings_add":
                    strOut = AddSettingsRow();
                    break;
                case "rocketmodsettings_save":
                    strOut = SettingsSave();
                    break;
                case "rocketmodsettings_delete":
                    strOut = SettingsDelete();
                    break;

                case "rocketmodfields_edit":
                    strOut = EditFieldsData();
                    break;
                case "rocketmodfields_add":
                    strOut = AddFieldsRow();
                    break;
                case "rocketmodfields_save":
                    strOut = FieldsSave();
                    break;
                case "rocketmodfields_delete":
                    strOut = FieldsDelete();
                    break;

                case "rocketmodview_download":
                    rtnDic = DownloadDocument();
                    break;

                case "rocketmodapptheme_geteditor":
                    strOut = GetEditorDetail();
                    break;
                case "rocketmodapptheme_apptheme":
                    strOut = GetAppModTheme();
                    break;
                case "rocketmodapptheme_getdetail":
                    strOut = GetAppModTheme();
                    break;
                case "rocketmodapptheme_saveeditor":
                    strOut = SaveEditor();
                    break;
                case "rocketmodapptheme_removemodtemplate":
                    strOut = RemoveTemplate();
                    break;

                case "module_export":
                    strOut = ExportData();
                    break;
                case "module_import":
                    ImportData();
                    break;

                case "module_copylanguage":
                    CopyLanguage();
                    break;

                case "module_validate":
                    ValidateData();
                    break;

                case "backup_get":
                    strOut = GetBackUp();
                    break;
                case "backup_dobackup":
                    DoBackUp(true);
                    strOut = GetBackUp();
                    break;
                case "backup_deletebackup":
                    strOut = DeleteBackUp();
                    break;
                case "backup_restorebackup":
                    strOut = RestoreBackUp();
                    break;
                case "backup_deletebackupall":
                    strOut = DeleteAllBackUp();
                    break;
                case "backup_save":
                    strOut = SaveBackUp();
                    break;
                case "backup_downloadbackup":
                    downloadDict =  DownloadBackUp();
                    break;                    


                case "templatebackup_dobackup":
                    DoTemplateBackUp();
                    strOut = GetAppModTheme();
                    break;
                case "templatebackup_deletebackup":
                    strOut = DeleteTemplateBackUp();
                    break;
                case "templatebackup_restorebackup":
                    strOut = RestoreTemplateBackUp();
                    break;
                case "templatebackup_deletebackupall":
                    strOut = DeleteAllTemplateBackUp();
                    break;

            }

            // -----------------------------------------------------------------------
            // if we have changed language, reset the editlang.  The _nextLang is defined on the "InitCmd" function.
            if (_nextLang != _editLang) DNNrocketUtils.SetEditCulture(_nextLang);
            // -----------------------------------------------------------------------

            if (downloadDict.Count > 0)
            {
                return downloadDict;
            }
            else
            {
                if (strOut == "" && !_moduleParams.Exists)
                {
                    return DNNrocketUtils.ReturnString(GetSetup());
                }               

                rtnDic.Add("outputhtml", strOut);
                return rtnDic;
            }

        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _currentLang = DNNrocketUtils.GetCurrentCulture();
            // -----------------------------------------------------------------------
            // Change of language. 
            // _nextlang is used for returning data. 
            // _editlang is used to save the data and reset to _nextLang at end of processing in "ProcessCommand" method.
            _editLang = DNNrocketUtils.GetEditCulture(); // set editlang from url param or cookie
            _nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (_nextLang == "") _nextLang = _editLang; // default to editLang
            DNNrocketUtils.SetNextCulture(_nextLang); // set the next langauge to a cookie, so the "EditFlag" razor token works.
            // -----------------------------------------------------------------------

            
            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _appthemeSystemRelPath = "/DesktopModules/RocketThemes";
            _appthemeSystemMapPath = DNNrocketUtils.MapPath(_appthemeSystemRelPath);
            _rocketModRelPath = "/DesktopModules/DNNrocket/RocketMod";
            _rocketModMapPath = DNNrocketUtils.MapPath(_rocketModRelPath);
            _systemKey = _systemData.SystemKey;
            _tableName = _rocketInterface.DatabaseTable;

            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _userParams = new UserParams(new SessionParams(_paramInfo).BrowserSessionId);
            _userParams.ModuleId = _moduleid; // use moduleid for tracking commands. 

            //[TODO : Update security.  create a limpet]
            if (_paramInfo.GetXmlPropertyBool("genxml/hidden/reload"))
            {
                if (paramInfo.GetXmlProperty("genxml/hidden/editmode") == "1")
                {
                    paramCmd = "rocketmodedit_editarticle";
                }
                else
                {
                    var menucmd = _userParams.GetCommand(_systemData.SystemKey);
                    if (menucmd != "")
                    {
                        paramCmd = menucmd;
                        _paramInfo = _userParams.GetParamInfo(_systemData.SystemKey);
                        var interfacekey = _userParams.GetInterfaceKey(_systemData.SystemKey);
                        _rocketInterface = new RocketInterface(systemInfo, interfacekey);
                    }
                }
            }

            _editLang = DNNrocketUtils.GetEditCulture();

            _settingsData = GetSettingsData();
            _passSettings = _settingsData.ToDictionary();

            _passSettings.Remove("tabid");
            _passSettings.Add("tabid", _tabid.ToString());
            _passSettings.Remove("moduleid");
            _passSettings.Add("moduleid", _moduleid.ToString());

            _moduleParams = new ModuleParams(_moduleid, _systemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.ModuleIdDataSource, _systemKey);

            var adminpanel = _paramInfo.GetXmlPropertyBool("genxml/hidden/adminpanel");
            var appthemefolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
            if (!_moduleParams.Exists && adminpanel && appthemefolder == "")
            {
                paramCmd = "rocketmodedit_selectapptheme";
            }
            else
            {
                if (appthemefolder != "")
                {
                    _moduleParams.AppThemeFolder = appthemefolder;
                }
                _appThemeMod = new AppThemeModule(_moduleid, _systemData.SystemKey);
                if (_moduleParams.ModuleIdDataSource == _moduleid)
                {
                    _dataAppThemeMod = _appThemeMod;
                }
                else
                {
                    _dataAppThemeMod = new AppThemeModule(_moduleParams.ModuleIdDataSource, _systemData.SystemKey);
                }

                if (!_passSettings.ContainsKey("AppTheme")) _passSettings.Add("AppTheme", _moduleParams.AppThemeFolder);
                if (!_passSettings.ContainsKey("AppThemeVersion")) _passSettings.Add("AppThemeVersion", _moduleParams.AppThemeVersion);
                if (!_passSettings.ContainsKey("AppThemeRelPath")) _passSettings.Add("AppThemeRelPath", _moduleParams.AppThemeFolderRel);
            }

            var securityData = new SecurityLimet(PortalUtils.GetCurrentPortalId(), _systemData.SystemKey, _rocketInterface, -1, -1);
            paramCmd = securityData.HasSecurityAccess(paramCmd, "rocketmod_login");

            // set tracking after the security check.
            if (_paramInfo.GetXmlPropertyBool("genxml/hidden/track")) _userParams.Track(_systemData.SystemKey, paramCmd, _paramInfo, _rocketInterface.InterfaceKey);

            return paramCmd;
        }



    }
}
