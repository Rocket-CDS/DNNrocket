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
    public class StartConnect : DNNrocketAPI.APInterface
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
        private CommandSecurity _commandSecurity;
        private DNNrocketInterface _rocketInterface;
        private SimplisityInfo _systemInfo;
        private ModuleParams _moduleParams;
        private ModuleParams _dataModuleParams;
        private int _tabid;
        private int _moduleid;
        private SystemData _systemData;
        private string _systemKey;
        private Dictionary<string, string> _passSettings;
        private SettingsData _settingsData;
        private string _editLang;
        private int _selectedItemId;
        private AppThemeModule _appThemeMod;
        private AppThemeModule _dataAppThemeMod;
        private ArticleLimpet _articleLimpet;
        private AppThemeDataList _appThemeDataList;
        private UserParams _userParams;
        private string _tableName;
        private ArticleLimpetList _articleLimpetList;

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
                case "rocketmodedit_editarticlelist":
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_articlesearch":
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_addarticle":
                    strOut = AddArticle();
                    break;
                case "rocketmodedit_editarticle":
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_savearticle":
                    SaveArticle(true);
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_savearticlelist":
                    SaveArticleList();
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_deletearticle":
                    DeleteArticle();
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_addimage":
                    SaveArticle(false);
                    RocketModAddListItem("imagelist" + _paramInfo.GetXmlProperty("genxml/hidden/imgfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_adddocument":
                    SaveArticle(false);
                    RocketModAddListItem("documentlist" + _paramInfo.GetXmlProperty("genxml/hidden/docfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_addlink":
                    SaveArticle(false);
                    RocketModAddListItem("linklist" + _paramInfo.GetXmlProperty("genxml/hidden/linkfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_saveconfig":
                    SaveConfig();
                    strOut = GetDashBoard();
                    break;
                case "rocketmodedit_reset":
                    strOut = ResetRocketMod(_moduleid);
                    break;
                case "rocketmodedit_resetdata":
                    strOut = ResetDataRocketMod(_moduleid);
                    break;
                case "rocketmodedit_datasourcelist":
                    strOut = GetDataSourceList();
                    break;
                case "rocketmodedit_datasourceselect":
                    strOut = GetDataSourceSelect();
                    break;
                case "rocketmodedit_reindexsortorder":
                    strOut = ReIndexSortOrder();
                    break;
                case "rocketmodedit_activatesortorder":
                    strOut = ActivateSortOrder();
                    break;
                case "rocketmodedit_setsortorder":
                    strOut = MoveSortOrder();
                    break;
                case "rocketmodedit_cancelsortorder":
                    strOut = CancelSortOrder();
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
            _systemData = new SystemData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _appthemeSystemRelPath = "/DesktopModules/DNNrocket/SystemThemes";
            _appthemeSystemMapPath = DNNrocketUtils.MapPath(_appthemeSystemRelPath);
            _rocketModRelPath = "/DesktopModules/DNNrocket/RocketMod";
            _rocketModMapPath = DNNrocketUtils.MapPath(_rocketModRelPath);

            _postInfo = postInfo;
            _systemInfo = systemInfo;
            _systemKey = _systemData.SystemKey;
            _tableName = _rocketInterface.DatabaseTable;

            // set editlang from url param or cache
            _editLang = DNNrocketUtils.GetEditCulture();

            _paramInfo = paramInfo;

            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);

            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _userParams = new UserParams(new SessionParams(_paramInfo).BrowserSessionId);
            _userParams.ModuleId = _moduleid; // use moduleid for tracking commands. 

            if (CheckSecurity(paramCmd))
            {
                if (_paramInfo.GetXmlPropertyBool("genxml/hidden/reload"))
                {
                    if (paramInfo.GetXmlProperty("genxml/hidden/editmode") == "1")
                    {
                        paramCmd = "rocketmodedit_editarticlelist";
                    }
                    else
                    {
                        var menucmd = _userParams.GetCommand(_systemKey);
                        if (menucmd != "")
                        {
                            paramCmd = menucmd;
                            _paramInfo = _userParams.GetParamInfo(_systemKey);
                            var interfacekey = _userParams.GetInterfaceKey(_systemKey);
                            _rocketInterface = new DNNrocketInterface(systemInfo, interfacekey);
                        }
                    }
                }
                else
                {
                    if (_paramInfo.GetXmlPropertyBool("genxml/hidden/track"))
                    {
                        _userParams.Track(_systemKey, paramCmd, _paramInfo, _rocketInterface.InterfaceKey);
                    }
                }
            }

            _selectedItemId = _paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

            _editLang = DNNrocketUtils.GetEditCulture();

            _settingsData = GetSettingsData();
            _passSettings = _settingsData.ToDictionary();

            _passSettings.Remove("tabid");
            _passSettings.Add("tabid", _tabid.ToString());
            _passSettings.Remove("moduleid");
            _passSettings.Add("moduleid", _moduleid.ToString());

            _moduleParams = new ModuleParams(_moduleid, _systemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.ModuleIdDataSource, _systemKey);

            if (!CheckSecurity(paramCmd))
            {
                return "rocketmod_login";
            }
            else
            {
                if (!_moduleParams.Exists &&
                    paramCmd != "module_validate" &&
                    paramCmd != "module_copylanguage" &&
                    paramCmd != "module_import" &&
                    paramCmd != "rocketmod_getdata" &&
                    paramCmd != "rocketmodedit_saveapptheme" &&
                    paramCmd != "rocketmodedit_saveconfig" &&
                    paramCmd != "backup_downloadbackup" &&
                    paramCmd != "rocketmodedit_saveappthemeconfig")
                {
                    return "rocketmodedit_selectapptheme";
                }
                else
                {
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
            }

            _articleLimpetList = new ArticleLimpetList(_paramInfo, _editLang, false);


            return paramCmd;
        }
        public bool CheckSecurity(string paramCmd)
        {
            _commandSecurity = new CommandSecurity(_tabid, _moduleid, _rocketInterface);

            _commandSecurity.AddCommand("dashboard_get", true);

            _commandSecurity.AddCommand("rocketmodedit_saveconfig", true);
            _commandSecurity.AddCommand("rocketmod_saveheader", true);
            _commandSecurity.AddCommand("rocketmod_getsetupmenu", true);
            _commandSecurity.AddCommand("rocketmodedit_reset", true);
            _commandSecurity.AddCommand("rocketmodedit_resetdata", true);
            _commandSecurity.AddCommand("rocketmodedit_selectapptheme", true);
            _commandSecurity.AddCommand("rocketmodedit_saveapptheme", true);
            _commandSecurity.AddCommand("rocketmod_getsidemenu", true);

            _commandSecurity.AddCommand("rocketmodedit_editarticlelist", true);
            _commandSecurity.AddCommand("rocketmodedit_articlesearch", true);
            _commandSecurity.AddCommand("rocketmodedit_editarticle", true);
            _commandSecurity.AddCommand("rocketmodedit_addarticle", true);
            _commandSecurity.AddCommand("rocketmodedit_savearticle", true);
            _commandSecurity.AddCommand("rocketmodedit_deletearticle", true);
            _commandSecurity.AddCommand("rocketmodedit_addimage", true);

            _commandSecurity.AddCommand("rocketmod_getdata", false);
            _commandSecurity.AddCommand("rocketmod_login", false);

            _commandSecurity.AddCommand("rocketmodsettings_edit", true);
            _commandSecurity.AddCommand("rocketmodsettings_add", true);
            _commandSecurity.AddCommand("rocketmodsettings_save", true);
            _commandSecurity.AddCommand("rocketmodsettings_delete", true);

            _commandSecurity.AddCommand("rocketmodfields_edit", true);
            _commandSecurity.AddCommand("rocketmodfields_add", true);
            _commandSecurity.AddCommand("rocketmodfields_save", true);
            _commandSecurity.AddCommand("rocketmodfields_delete", true);

            _commandSecurity.AddCommand("rocketmodview_download", false);

            _commandSecurity.AddCommand("rocketmodapptheme_apptheme", true);
            _commandSecurity.AddCommand("rocketmodapptheme_saveeditor", true);

            _commandSecurity.AddCommand("rocketmodapptheme_geteditor", true);
            _commandSecurity.AddCommand("rocketmodapptheme_apptheme", true);
            _commandSecurity.AddCommand("rocketmodapptheme_getdetail", true);
            _commandSecurity.AddCommand("rocketmodapptheme_saveeditor", true);
            _commandSecurity.AddCommand("rocketmodapptheme_removemodtemplate", true);
            _commandSecurity.AddCommand("module_export", true);
            _commandSecurity.AddCommand("module_import", true);
            _commandSecurity.AddCommand("module_copylanguage", true);
            _commandSecurity.AddCommand("module_validate", true);

            _commandSecurity.AddCommand("backup_get", true);
            _commandSecurity.AddCommand("backup_dobackup", true);
            _commandSecurity.AddCommand("backup_deletebackup", true);
            _commandSecurity.AddCommand("backup_restorebackup", true);
            _commandSecurity.AddCommand("backup_deletebackupall", true);
            _commandSecurity.AddCommand("backup_save", true);
            _commandSecurity.AddCommand("backup_downloadbackup", true);

            _commandSecurity.AddCommand("templatebackup_dobackup", true);
            _commandSecurity.AddCommand("templatebackup_deletebackup", true);
            _commandSecurity.AddCommand("templatebackup_restorebackup", true);
            _commandSecurity.AddCommand("templatebackup_deletebackupall", true);


            return _commandSecurity.HasSecurityAccess(paramCmd);
        }

        public void RocketModAddListItem(string listname)
        {
            try
            {
                var selecteditemid = _paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                if (selecteditemid > 0)
                {
                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetData(_rocketInterface.EntityTypeCode, selecteditemid, _editLang);
                    info.AddListItem(listname);
                    objCtrl.SaveData(info);
                }
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
            }
        }

        private void SaveConfig()
        {
            // do Backup
            DoBackUp();

            var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder);
            _moduleParams.AppThemeLogo = appTheme.Logo;
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.ImageFolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            _moduleParams.DocumentFolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            _moduleParams.AppThemeVersion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");
            _moduleParams.AppThemeNotes = _postInfo.GetXmlProperty("genxml/hidden/appthemenotes");
            _moduleParams.DetailUrlParam = GeneralUtils.UrlFriendly(_postInfo.GetXmlProperty("genxml/hidden/detailurlparam"));
            _moduleParams.DetailView = _postInfo.GetXmlPropertyBool("genxml/hidden/detailview");
            _moduleParams.ModuleType = "RocketMod";
            _moduleParams.CacheDisbaled = _postInfo.GetXmlPropertyBool("genxml/hidden/disablecache");
            _moduleParams.AutoBackUp = _postInfo.GetXmlPropertyBool("genxml/hidden/autobackup");

            if (_systemData.DebugMode) _moduleParams.CacheDisbaled = true;

            _moduleParams.ShareData = _postInfo.GetXmlProperty("genxml/hidden/sharedata");
            _moduleParams.TabId = _tabid;
            _moduleParams.SystemKey = _systemData.SystemKey;
            _moduleParams.ExportResourceFiles = _postInfo.GetXmlPropertyBool("genxml/hidden/exportresourcefiles");

            _moduleParams.Save();
            _passSettings.Remove("saved");
            _passSettings.Add("saved", "true");

            // update module with a better name
            DNNrocketUtils.UpdateModuleTitle(_tabid, _moduleid, _moduleParams.Name + ":" + _moduleid);

            CacheFileUtils.ClearAllCache();


        }

        private void SaveAppTheme()
        {
            _moduleParams.AppThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
            var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder);
            _moduleParams.AppThemeLogo = appTheme.Logo;
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.AppThemeVersion = appTheme.LatestVersionFolder;
            _moduleParams.LatestRev = appTheme.LatestRev;
            _moduleParams.ModuleType = "RocketMod";
            _moduleParams.ImageFolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            _moduleParams.DocumentFolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            _moduleParams.AppThemeFolderRel = appTheme.AppThemeFolderRel;
            _moduleParams.AppThemeVersionFolderRel = appTheme.AppThemeVersionFolderRel;
            _moduleParams.AppProjectFolderRel = appTheme.AppProjectFolderRel;
            _moduleParams.AppSystemThemeFolderRel = appTheme.AppSystemThemeFolderRel;
            _moduleParams.AppThemeNotes = _postInfo.GetXmlProperty("genxml/hidden/appthemenotes");
            _moduleParams.ShareData = "1";
            _moduleParams.TabId = _tabid;
            _moduleParams.SystemKey = appTheme.SystemKey;
            _moduleParams.Save();
            CacheFileUtils.ClearAllCache();
        }

        #region "Articles"

        public Dictionary<string, object> DownloadDocument()
        {
            var documentref = _paramInfo.GetXmlProperty("genxml/hidden/document");
            var documentlistname = _paramInfo.GetXmlProperty("genxml/hidden/listname");
            _articleLimpet = new ArticleLimpet(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
            var docInfo = _articleLimpet.Info.GetListItem(documentlistname, "genxml/lang/genxml/hidden/document", documentref);
            var filepath = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/reldocument");
            var namedocument = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/namedocument");
            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", filepath);
            rtnDic.Add("downloadname", namedocument);
            rtnDic.Add("fileext", "");
            return rtnDic;
        }
        public void SaveArticle(bool doBackup)
        {
            // do Backup
            if (doBackup) DoBackUp();

             _articleLimpet = new ArticleLimpet(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
            
            // do Save
            _passSettings.Add("saved", "true");
            _articleLimpet.DebugMode = _systemData.DebugMode;
            _articleLimpet.Save(_postInfo);

            DNNrocketUtils.SynchronizeModule(_moduleid); // Modified Date
            CacheFileUtils.ClearAllCache();

        }

        public void SaveArticleList()
        {
            // do Backup
            DoBackUp();

            var listNames = _postInfo.GetLists();
            foreach (var listName in listNames)
            {
                var lp = 1;
                var list = _postInfo.GetList(listName); 
                foreach (var info in list)
                {
                    var selectitemid = info.GetXmlPropertyInt("genxml/hidden/itemid");
                    if (selectitemid > 0)
                    {
                        var art = new ArticleLimpet(selectitemid, _dataModuleParams.ModuleId, _editLang);
                        art.SortOrder = lp;
                        art.Update();
                        lp += 1;
                    }
                }
            }
            CacheFileUtils.ClearAllCache();
        }

        public void DeleteArticle()
        {
            _articleLimpet = new ArticleLimpet(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
            _articleLimpet.Delete();
            _selectedItemId = -1;
            _userParams.TrackClear(_systemKey);
            _userParams.Save();

            CacheFileUtils.ClearAllCache();
        }

        public String AddArticle()
        {
            try
            {
                var articleLimpet = new ArticleLimpet(-1, _dataModuleParams.ModuleId, _editLang);
                CacheFileUtils.ClearAllCache();
                var strOut = GetArticle(articleLimpet);
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public String GetArticleEdit()
        {
            try
            {
                if (_selectedItemId <= 0) 
                    return GetArticleList();
                else
                    return GetArticle();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public String GetArticle(ArticleLimpet articleLimpet = null)
        {
            try
            {
                _articleLimpet = articleLimpet;
                var razorTempl = _appThemeMod.GetTemplateRazor("edit.cshtml");
                if (_moduleParams.DataSourceExternal)
                {
                    razorTempl = _dataAppThemeMod.GetTemplateRazor("edit.cshtml");
                }

                var strOut = "-- NO DATA -- Itemid: " + _selectedItemId;

                if (_articleLimpet == null) _articleLimpet = new ArticleLimpet(_selectedItemId, _dataModuleParams.ModuleId, _editLang);

                _articleLimpet.ImageFolder = _dataModuleParams.ImageFolder;
                _articleLimpet.DocumentFolder = _dataModuleParams.DocumentFolder;
                _articleLimpet.AppTheme = _dataModuleParams.AppThemeFolder;
                _articleLimpet.AppThemeVersion = _dataModuleParams.AppThemeVersion;
                _articleLimpet.AppThemeRelPath = _dataModuleParams.AppThemeFolderRel;
                _articleLimpet.AppThemeDataType = _dataAppThemeMod.AppTheme.DataType;

                _passSettings.Add("datatype", _dataAppThemeMod.AppTheme.DataType.ToString());

                // look at fields to find out what lists are included in the template.
                var usedlistcsv = "";
                foreach (SimplisityRecord f in _dataAppThemeMod.AppTheme.Record.GetRecordList("fielddata"))
                {
                    var fieldtype = f.GetXmlProperty("genxml/select/type");
                    if (fieldtype == "imagegallery") usedlistcsv += ".imagelist" + f.GetXmlProperty("genxml/textbox/name") + ",";
                    if (fieldtype == "linkgallery") usedlistcsv += ".linklist" + f.GetXmlProperty("genxml/textbox/name") + ",";
                    if (fieldtype == "documentgallery") usedlistcsv += ".documentlist" + f.GetXmlProperty("genxml/textbox/name") + ",";
                }
                var fieldData = GetFieldsData(_dataModuleParams.ModuleId);
                foreach (SimplisityInfo f in fieldData.List)
                {
                    var fieldtype = f.GetXmlProperty("genxml/select/type");
                    if (fieldtype == "imagegallery") usedlistcsv += ".imagelist" + f.GetXmlProperty("genxml/textbox/name") + ",";
                    if (fieldtype == "linkgallery") usedlistcsv += ".linklist" + f.GetXmlProperty("genxml/textbox/name") + ",";
                    if (fieldtype == "documentgallery") usedlistcsv += ".documentlist" + f.GetXmlProperty("genxml/textbox/name") + ",";
                }

                _passSettings.Add("s-list-csv", usedlistcsv.TrimEnd(','));

                foreach (var s in _moduleParams.ModuleSettings)
                {
                    if (!_passSettings.ContainsKey(s.Key)) _passSettings.Add(s.Key, s.Value);
                }

                strOut = RenderRazorUtils.RazorDetail(razorTempl, _articleLimpet, _passSettings, new SessionParams(_paramInfo), _systemData.DebugMode);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public String GetArticleList()
        {

            try
            {
                var strOut = "";

                _articleLimpetList.Populate();

                var debugmode = false;
                if (_systemData.DebugMode || _moduleParams.CacheDisbaled) debugmode = true;
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("editlist.cshtml", "/DesktopModules/DNNrocket/SystemThemes/" + _systemData.SystemKey, _dataModuleParams.AppThemeFolder, _editLang, _dataModuleParams.AppThemeVersion, debugmode);
                strOut = RenderRazorUtils.RazorDetail(razorTempl, _articleLimpetList, _passSettings, new SessionParams(_paramInfo), _systemData.DebugMode);
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion



        #region "Fields"

        private SettingsData GetFieldsData(int moduleid = 0)
        {
            if (moduleid == 0) moduleid = _moduleid;
            var settingsData = new SettingsData(_tabid, moduleid, _editLang, "ROCKETMODFIELDS", "fielddata", false, _rocketInterface.DatabaseTable);
            return settingsData;
        }

        private String EditFieldsData()
        {
            try
            {
                var fieldsData = GetFieldsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, _editLang, "1.0", _systemData.DebugMode);
                strOut = RenderRazorUtils.RazorDetail(razorTempl, fieldsData, passSettings, null, _systemData.DebugMode);

                if (strOut == "") strOut = "ERROR: No data returned for EditfieldsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string FieldsDelete()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Delete();
            return EditFieldsData();
        }

        private string AddFieldsRow()
        {
            var fieldsData = GetFieldsData();
            fieldsData.AddRow();
            return EditFieldsData();
        }

        private String FieldsSave()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Save(_postInfo);
            
            // AFTER SAVE: calc and save any xpath for fields.
            var objCtrl = new DNNrocketController();
            var dbInfo = objCtrl.GetRecord("ROCKETMODFIELDS", fieldsData.Info.ItemID, -1, true, _rocketInterface.DatabaseTable);
            if (dbInfo != null)
            {
                dbInfo = DNNrocketUtils.UpdateFieldXpath(dbInfo, "fielddata");
                objCtrl.Update(dbInfo, _rocketInterface.DatabaseTable);
            }

            _passSettings.Add("saved", "true");
            return EditFieldsData();
        }

        #endregion

        #region "Settings"

        private SettingsData GetSettingsData()
        {
            return new SettingsData(_tabid, _moduleid, _editLang, "ROCKETMODSETTINGS", "rocketmodsettings", true, _rocketInterface.DatabaseTable);
        }
        private String EditSettingsData()
        {
            try
            {
                var settingsData = GetSettingsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetEditCulture(), "1.0", _systemData.DebugMode);
                strOut = RenderRazorUtils.RazorDetail(razorTempl, settingsData, passSettings);

                if (strOut == "") strOut = "ERROR: No data returned for EditSettingsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string SettingsDelete()
        {
            var settingsData = GetSettingsData();
            settingsData.Delete();
            CacheFileUtils.ClearAllCache();
            return EditSettingsData();
        }

        private string AddSettingsRow()
        {
            var settingsData = GetSettingsData();
            settingsData.AddRow();
            CacheFileUtils.ClearAllCache();
            return EditSettingsData();
        }

        private String SettingsSave()
        {
            var settingsData = GetSettingsData();
            settingsData.Save(_postInfo);
            CacheFileUtils.ClearAllCache();
            return EditSettingsData();
        }

        #endregion

        public String ResetRocketMod(int moduleid)
        {
            try
            {
                _moduleParams.Delete();

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleid" + moduleid, "ROCKETMODFIELDS", "", moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);
                info = objCtrl.GetData("moduleid" + moduleid, "ROCKETMODSETTINGS", "", moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);

                return GetSelectApp();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String ReIndexSortOrder()
        {
            try
            {
                _articleLimpetList.SortOrderReIndex();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String ActivateSortOrder()
        {
            _articleLimpetList.SessionParamData.ActivateItemSort(_paramInfo.GetXmlPropertyInt("genxml/hidden/itemid"));
            return GetArticleList();
        }
        public String CancelSortOrder()
        {
            _articleLimpetList.SessionParamData.CancelItemSort();
            return GetArticleList();
        }
        public String MoveSortOrder()
        {
            var toId = _paramInfo.GetXmlPropertyInt("genxml/hidden/itemid");
            _articleLimpetList.SortOrderMove(toId);
            return GetArticleList();
        }

        public String ResetDataRocketMod(int moduleid)
        {
            try
            {
                _articleLimpetList.DeleteAll();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String GetSettingSection()
        {
            try
            {
                var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder, _moduleParams.AppThemeVersion);
                if (!appTheme.EnableSettings) return "";
                var razorTempl = appTheme.GetTemplate("settings.cshtml"); // new module, so settings theme will be systemtheme.
                _settingsData = GetSettingsData();
                return RenderRazorUtils.RazorDetail(razorTempl, _settingsData, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetDashBoard()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _moduleParams, _passSettings,null,true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetDataSourceList()
        {
            try
            {
                var mp = new ModuleParamList(_systemKey, DNNrocketUtils.GetCurrentCulture(),true, true);
                if (_moduleParams.ShareData == "0") mp.DataList.Add(_moduleParams); // current to list, so we can assigned current module as data source.
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "selectdatasource.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, mp, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String GetDataSourceSelect()
        {
            try
            {
                var selectmoduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/selectedmoduleid");
                var selectmoduleref = _paramInfo.GetXmlProperty("genxml/hidden/selectedmoduleref");
                if (selectmoduleid == 0)
                {
                    selectmoduleid = _moduleid;
                    selectmoduleref = _moduleParams.ModuleRef;
                }
                else
                {
                    _moduleParams.ShareData = "0";
                }
                _moduleParams.ModuleIdDataSource = selectmoduleid;
                _moduleParams.ModuleRefDataSource = selectmoduleref;
                _moduleParams.Save();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetAppModTheme()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "apptheme.cshtml";
                var appThemeMod = new AppThemeModule(_moduleid, _systemData.SystemKey);

                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                return RenderRazorUtils.RazorDetail(razorTempl, appThemeMod, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string SaveEditor()
        {
            DoTemplateBackUp(); // backup before we save

            var editorcode = _postInfo.GetXmlProperty("genxml/hidden/editorcodesave");
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            _appThemeMod.SaveEditor(filename, editorcode);
            CacheFileUtils.ClearAllCache();
            return "OK";
        }

        public string RemoveTemplate()
        {
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            _appThemeMod.RemoveModuleTemplate(filename);
            CacheFileUtils.ClearAllCache();
            return GetAppModTheme();
        }


        public String GetSelectApp()
        {

            try
            {
                var strOut = "";
                if (!_moduleParams.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    _appThemeDataList = new AppThemeDataList(_systemData.SystemKey);

                    var razortemplate = "selectapp.cshtml";
                    var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(),"1.0", true);
                    var passSettings = _postInfo.ToDictionary();
                    if (!passSettings.ContainsKey("moduleid")) passSettings.Add("moduleid", _moduleParams.ModuleId.ToString());

                    return RenderRazorUtils.RazorDetail(razorTempl, _appThemeDataList, passSettings, null, true);
                }
                else
                {
                    //strOut = EditData();
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public String GetDisplay()
        {
            try
            {
                var strOut = "";
                if (_moduleParams.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    var appthemefolder = _moduleParams.AppThemeFolder;
                    var passSettings = new PassSettings(_paramInfo);                 
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());
                    var adminurl = "/DesktopModules/DNNrocket/RocketMod/admin.html?moduleid=" + _moduleid + "&tabid=" + _tabid;
                    passSettings.Add("adminurl", adminurl);
                    var appTheme = new AppTheme(_systemData.SystemKey, appthemefolder, _moduleParams.AppThemeVersion);
                    passSettings.Add("datatype", appTheme.DataType.ToString());

                    if ((_paramInfo.GetXmlPropertyInt("genxml/urlparams/" + _moduleParams.DetailUrlParam) > 0) && _moduleParams.DetailView)
                    {
                        // detail display
                        var itemid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/" + _moduleParams.DetailUrlParam);

                        var razorTempl = _appThemeMod.GetTemplateRazor("detail.cshtml");

                        var articleLimpet = new ArticleLimpet(itemid, _dataModuleParams.ModuleId, DNNrocketUtils.GetCurrentCulture());

                        foreach (var s in _moduleParams.ModuleSettings)
                        {
                            passSettings.Add(s.Key, s.Value);
                        }

                        passSettings.Add("DocumentFolderRel", _dataModuleParams.DocumentFolderRel);
                        passSettings.Add("ImageFolderRel", _dataModuleParams.ImageFolderRel);
                        passSettings.Add("DetailView", _moduleParams.DetailView.ToString());

                        passSettings.Add("tabid", _tabid.ToString());

                        strOut = RenderRazorUtils.RazorDetail(razorTempl, articleLimpet, passSettings.DictionaryData);

                    }
                    else
                    {
                        // list display
                        var razorTempl = _appThemeMod.GetTemplateRazor("view.cshtml");

                        var articleLimpetList = new ArticleLimpetList(_paramInfo, DNNrocketUtils.GetCurrentCulture(), true);

                        foreach (var s in _moduleParams.ModuleSettings)
                        {
                            passSettings.Add(s.Key, s.Value);
                        }

                        passSettings.Add("DocumentFolderRel", _dataModuleParams.DocumentFolderRel);
                        passSettings.Add("ImageFolderRel", _dataModuleParams.ImageFolderRel);
                        passSettings.Add("DetailView", _moduleParams.DetailView.ToString());

                        passSettings.Add("tabid", _tabid.ToString());

                        strOut = RenderRazorUtils.RazorDetail(razorTempl, articleLimpetList, passSettings.DictionaryData, new SessionParams(_paramInfo));
                    }


                }
                else
                {
                    strOut = GetSetup();
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        private String GetSetup()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(),"1.0",_systemData.DebugMode);                
                return RenderRazorUtils.RazorDetail(razorTempl, _rocketInterface.Info,_passSettings, null, _systemData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public bool CheckIfList()
        {
            try
            {
                if (_moduleParams.Exists)
                {
                    var themeFolder = _moduleParams.AppThemeFolder;
                    var razorTempl = RenderRazorUtils.GetRazorTemplateData("editlist.cshtml", _appthemeRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                    if (razorTempl != "") return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
                return false;
            }

        }

        private string ExportData()
        {
            var exportData = new ExportLimpet(_rocketInterface, _moduleid, _systemKey);
            return exportData.GetXml();
        }
        private void ImportData()
        {
            var oldmoduleid = _postInfo.GetXmlPropertyInt("export/moduleid");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var importData = new ImportLimpet(_rocketInterface, portalid, _moduleid, oldmoduleid, _postInfo.XMLData);
            CacheUtilsDNN.ClearAllCache();
            DNNrocketUtils.ClearAllCache();
        }

        private void CopyLanguage()
        {
            var objCtrl = new DNNrocketController();

            // data passed to system with _paramInfo
            var copylanguage = _paramInfo.GetXmlProperty("genxml/hidden/copylanguage");
            var destinationlanguage = _paramInfo.GetXmlProperty("genxml/hidden/destinationlanguage");
            var backup = _paramInfo.GetXmlPropertyBool("genxml/checkbox/backup");
            var backuprootfolder = _paramInfo.GetXmlProperty("genxml/hidden/backuprootfolder");
            
            if (destinationlanguage != copylanguage)
            {
                // BackUp data to file 
                if (backup)
                {
                    var saveList = new List<SimplisityInfo>();
                    var l2 = objCtrl.GetList(PortalUtils.GetPortalId(), -1, "MODULEPARAMS", " and r1.XmlData.value('(genxml/hidden/moduletype)[1]','nvarchar(max)') = 'RocketMod'");
                    foreach (var sInfo in l2)
                    {
                        var moduleParams = new ModuleParams(sInfo.ModuleId, _systemKey);
                        if (DNNrocketUtils.ModuleExists(moduleParams.TabId, sInfo.ModuleId) && !DNNrocketUtils.ModuleIsDeleted(moduleParams.TabId, sInfo.ModuleId))
                        {
                            var exportData = new ExportLimpet(_rocketInterface, moduleParams.ModuleId, moduleParams.SystemKey);
                            foreach (var s in exportData.GetList())
                            {
                                saveList.Add(s);
                            }
                        }
                    }
                    var fileName = DNNrocketUtils.BackUpNewFileName(backuprootfolder, _systemKey);
                    var backupData = new BackUpData(fileName);
                    backupData.BackUp(saveList);
                }


                // copy language
                var l = objCtrl.GetList(PortalUtils.GetPortalId(), -1, "ROCKETMODLANG", " and r1.Lang = '" + copylanguage + "'","","",0,0,0,0, _tableName);
                foreach (var sInfo in l)
                {
                    var objRecLang = objCtrl.GetRecordLang(sInfo.ParentItemId, destinationlanguage, _tableName);
                    if (objRecLang != null)
                    {
                        objRecLang.XMLData = sInfo.XMLData;
                        objCtrl.Update(objRecLang);

                        // recreate the IDX record.
                        var idxInfo = objCtrl.GetInfo(objRecLang.ParentItemId, objRecLang.Lang, _tableName);
                        objCtrl.RebuildIndex(idxInfo, _tableName);
                        objCtrl.RebuildLangIndex(idxInfo.PortalId, idxInfo.ItemID, _tableName);
                    }

                }

                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();

            }

        }


        private void ValidateData()
        {
            var objCtrl = new DNNrocketController();

            // remove deleted modules
            var filter = "and r1.XMlData.value('(genxml/hidden/systemkey)[1]','nvarchar(max)') = '" + _systemKey + "' ";
            var dirlist = objCtrl.GetList(PortalUtils.GetPortalId(), -1, "MODULEPARAMS", filter);
            foreach (var sInfo in dirlist)
            {
                var moduleParams = new ModuleParams(sInfo.ModuleId, _systemKey);
                if (moduleParams.ModuleId == sInfo.ModuleId && !DNNrocketUtils.ModuleExists(moduleParams.TabId, sInfo.ModuleId) && moduleParams.ModuleType.ToLower() == "rocketmod")
                {
                    ResetRocketMod(sInfo.ModuleId);
                    ResetDataRocketMod(sInfo.ModuleId);
                }
            }

        }

        public String GetEditorDetail()
        {
            try
            {
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var jsonString = GeneralUtils.EnCode(_appThemeMod.AppTheme.GetTemplate(fname));
                _passSettings.Add("filename", fname);
                _passSettings.Add("jsonFileData", jsonString);

                var editormode = "htmlmixed";
                if (Path.GetExtension(fname) == ".js") editormode = "javascript";
                if (Path.GetExtension(fname) == ".css") editormode = "css";
                _passSettings.Add("editormode", editormode);

                _passSettings.Add("interfacekey", _rocketInterface.InterfaceKey);
                _passSettings.Add("moduleref", _moduleParams.ModuleRef);

                if (_appThemeMod.IsModuleLevelTemplate(fname)) _passSettings.Add("moduletemplatebutton", "True");

                var razorTempl = RenderRazorUtils.GetRazorTemplateData("EditorPopUp.cshtml", "/DesktopModules/DNNrocket/AppThemes", "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorObjectRender(razorTempl, _appThemeMod.AppTheme, null, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #region "BackUP"

        private String GetBackUp()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                if (_passSettings.ContainsKey("searchpattern")) _passSettings.Remove("searchpattern");
                _passSettings.Add("searchpattern", "*_backup.xml");
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("backup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _rocketInterface.Info, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string DeleteBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_BackUp.xml");
            backUpDataList.DeleteBackUpFile(filemappath);
            return GetBackUp();
        }
        private string DeleteTemplateBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_templates.xml");
            backUpDataList.DeleteBackUpFile(filemappath);
            return GetAppModTheme();
        }
        private string DeleteAllBackUp()
        {
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_BackUp.xml");
            backUpDataList.DeleteAllBackUpFiles();
            return GetBackUp();
        }
        private string DeleteAllTemplateBackUp()
        {
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_templates.xml");
            backUpDataList.DeleteAllBackUpFiles();
            return GetAppModTheme();
        }

        private string RestoreTemplateBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {
                var backupData = new BackUpModuleTemplates(filemappath, _moduleid, _systemKey);
                backupData.RestoreData();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }
            return GetAppModTheme();
        }
        private string SaveBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {

                var backupData = new BackUpData(filemappath);
                backupData.BackUpText = _postInfo.GetXmlProperty("backup/backuptext");
                backupData.Save();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }

            return GetBackUp();
        }

        private string RestoreBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {
                DoBackUp();

                var backupData = new BackUpData(filemappath);
                backupData.RestoreData();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }

            return GetBackUp();
        }


        private void DoBackUp(bool forcebackup = false)
        {
            // BackUp data to file 
            if ((_moduleParams.Exists && _moduleParams.AutoBackUp) || forcebackup)
            {
                var saveList = new List<SimplisityInfo>();
                if (DNNrocketUtils.ModuleExists(_moduleParams.TabId, _moduleid) && !DNNrocketUtils.ModuleIsDeleted(_moduleParams.TabId, _moduleParams.ModuleId))
                {
                    var exportData = new ExportLimpet(_rocketInterface, _moduleParams.ModuleId, _moduleParams.SystemKey);
                    foreach (var s in exportData.GetList())
                    {
                        saveList.Add(s);
                    }
                }

                // check if previous backup is same
                var BackUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_BackUp.xml");

                var fileNameTemp = PortalUtils.TempDirectoryMapPath() + "\\" + ("rocketmod" + _moduleid + GeneralUtils.GetUniqueKey());
                var backupData = new BackUpData(fileNameTemp);
                backupData.BackUp(saveList);

                CacheUtils.SetCache("backupidentical" + _moduleid, "");

                var l = BackUpDataList.GetBackUpFileMapPathList("dnnrocketmodule");
                if (l.Count > 0)
                {
                    var lastBackupFileMapPath = l.First(); // take first file.  If the system returns in wrong order the file compare may fail.
                    try
                    {
                        if (!FileUtils.CompareAreSame(lastBackupFileMapPath, fileNameTemp) || forcebackup)
                        {
                            // move new file and remove temp file
                            var fileName = DNNrocketUtils.BackUpNewFileName("rocketmod" + _moduleid, _systemKey);
                            File.Copy(fileNameTemp, fileName);
                        }
                        else
                        {
                            CacheUtils.SetCache("backupidentical" + _moduleid, DateTime.Now.ToString("O"));
                        }
                        File.Delete(fileNameTemp);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                else
                {
                    // move new file and remove temp file
                    var fileName = DNNrocketUtils.BackUpNewFileName("rocketmod" + _moduleid, _systemKey);
                    File.Copy(fileNameTemp, fileName);
                }
            }
        }

        private void DoTemplateBackUp()
        {
            var fileMapPath = DNNrocketUtils.BackUpNewFileName("rocketmod" + _moduleid, _systemKey, "Templates.xml");
            var backupTemplates = new BackUpModuleTemplates(fileMapPath, _moduleid, _systemKey);
            backupTemplates.BackUp();
        }
        private Dictionary<string, object> DownloadBackUp()
        {
            var rtnDic = new Dictionary<string, object>();
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/filemappath"));
            if (File.Exists(filemappath))
            {
                var backupData = new BackUpModuleTemplates(filemappath, _moduleid, _systemKey);

                rtnDic.Add("filenamepath", backupData.FileMapPath);
                rtnDic.Add("downloadname", "DataBackUp" + backupData.ModuleId + ".xml");
            }
            return rtnDic;
        }


        #endregion


    }
}
