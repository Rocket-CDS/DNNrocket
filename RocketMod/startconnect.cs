using System;
using System.Collections.Generic;
using System.Linq;
using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
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
        private ArticleData _articleData;
        private AppThemeDataList _appThemeDataList;
        private UserStorage _userStorage;
        private string _tableName;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            var rtnDic = new Dictionary<string, string>();

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
                    strOut = GetArticleEdit(true);
                    break;
                case "rocketmodedit_articlesearch":
                    strOut = GetArticleEdit(false);
                    break;
                case "rocketmodedit_addarticle":
                    strOut = AddArticle();
                    break;
                case "rocketmodedit_editarticle":
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_savearticle":
                    SaveArticle();
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_savearticlelist":
                    SaveArticleList();
                    strOut = GetArticleEdit(true);
                    break;
                case "rocketmodedit_deletearticle":
                    DeleteArticle();
                    strOut = GetArticleEdit(true);
                    break;
                case "rocketmodedit_addimage":
                    SaveArticle();
                    RocketModAddListItem("imagelist" + _paramInfo.GetXmlProperty("genxml/hidden/imgfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_adddocument":
                    SaveArticle();
                    RocketModAddListItem("documentlist" + _paramInfo.GetXmlProperty("genxml/hidden/docfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_addlink":
                    SaveArticle();
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

                case "rocketmodapptheme_apptheme":
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

            }

            if (strOut == "" && !_moduleParams.Exists)
            {
                return DNNrocketUtils.ReturnString(GetSetup());
            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
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

            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _userStorage = new UserStorage();
            _userStorage.ModuleId = _moduleid; // use moduleid for tracking commands. 

            if (!CheckSecurity(paramCmd))
            {
                paramCmd = "rocketmodedit_editarticlelist";
            }
            else
            {
                if (_paramInfo.GetXmlPropertyBool("genxml/hidden/reload"))
                {
                    if (paramInfo.GetXmlProperty("genxml/hidden/editmode") == "1")
                    {
                        paramCmd = "rocketmodedit_editarticlelist";
                    }
                    else
                    {
                        var menucmd = _userStorage.GetCommand(_systemKey);
                        if (menucmd != "")
                        {
                            paramCmd = menucmd;
                            _paramInfo = _userStorage.GetParamInfo(_systemKey);
                            var interfacekey = _userStorage.GetInterfaceKey(_systemKey);
                            _rocketInterface = new DNNrocketInterface(systemInfo, interfacekey);
                        }
                    }
                }
                else
                {
                    if (_paramInfo.GetXmlPropertyBool("genxml/hidden/track"))
                    {
                        _userStorage.Track(_systemKey, paramCmd, _paramInfo, _rocketInterface.InterfaceKey);
                    }
                }
            }

            _selectedItemId = _paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

            _editLang = DNNrocketUtils.GetEditCulture();

            _settingsData = GetSettingsData();
            _passSettings = LocalUtils.SettingsToDictionary(_settingsData);

            _passSettings.Remove("tabid");
            _passSettings.Add("tabid", _tabid.ToString());
            _passSettings.Remove("moduleid");
            _passSettings.Add("moduleid", _moduleid.ToString());

            _moduleParams = new ModuleParams(_moduleid, _systemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.DataSourceModId, _systemKey);

            if (!CheckSecurity(paramCmd))
            {
                // default to see if the user has access, but last login was a different user.
                // The client cookie for simplisity will try and run the last users command.
                paramCmd = "rocketmodedit_editarticlelist";
                _rocketInterface = new DNNrocketInterface(systemInfo, "rocketmodedit");
            }

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
                    paramCmd != "rocketmodedit_saveappthemeconfig")
                {
                    return "rocketmodedit_selectapptheme";
                }
                else
                {
                    _appThemeMod = new AppThemeModule(_moduleid, _systemData.SystemKey);
                    _dataAppThemeMod = new AppThemeModule(_moduleParams.DataSourceModId, _systemData.SystemKey);

                    if (!_passSettings.ContainsKey("AppTheme")) _passSettings.Add("AppTheme", _moduleParams.AppThemeFolder);
                    if (!_passSettings.ContainsKey("AppThemeVersion")) _passSettings.Add("AppThemeVersion", _moduleParams.AppThemeVersion);
                    if (!_passSettings.ContainsKey("AppThemeRelPath")) _passSettings.Add("AppThemeRelPath", _moduleParams.AppThemeFolderRel);
                }
            }

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
            _moduleParams.Exists = true;
            _moduleParams.CacheDisbaled = _postInfo.GetXmlPropertyBool("genxml/hidden/disablecache");

            if (_systemData.DebugMode) _moduleParams.CacheDisbaled = true;

            _moduleParams.ShareData = _postInfo.GetXmlProperty("genxml/hidden/sharedata");
            _moduleParams.TabId = _tabid;
            _moduleParams.SystemKey = _systemData.SystemKey;
            _moduleParams.ExportResourceFiles = _postInfo.GetXmlPropertyBool("genxml/hidden/exportresourcefiles");

            _moduleParams.Save();
            _passSettings.Add("saved", "true");

            // update module with a better name
            DNNrocketUtils.UpdateModuleTitle(_tabid, _moduleid, _moduleParams.Name + ":" + _moduleid);

            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);


        }

        private void SaveAppTheme()
        {
            _moduleParams.AppThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
            var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder);
            _moduleParams.AppThemeLogo = appTheme.Logo;
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.AppThemeVersion = appTheme.LatestVersionFolder;
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
            _moduleParams.Exists = true;
            _moduleParams.Save();
            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
        }

        #region "Articles"

        public Dictionary<string,string> DownloadDocument()
        {
            var documentref = _paramInfo.GetXmlProperty("genxml/hidden/document");
            var documentlistname = _paramInfo.GetXmlProperty("genxml/hidden/listname");
            _articleData = new ArticleData(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
            var docInfo = _articleData.Info.GetListItem(documentlistname, "genxml/lang/genxml/hidden/document", documentref);
            var filepath = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/reldocument");
            var namedocument = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/namedocument");
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("filenamepath", filepath);
            rtnDic.Add("downloadname", namedocument);
            rtnDic.Add("fileext", "");
            return rtnDic;
        }
        public void SaveArticle()
        {
            if (_dataAppThemeMod.AppTheme.DataType == 1)
            {
                _articleData = new ArticleData(_dataModuleParams.ModuleId, _editLang);
            }
            else
            {
                _articleData = new ArticleData(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
            }
            _passSettings.Add("saved", "true");
            _articleData.DebugMode = _systemData.DebugMode;
            _articleData.Save(_postInfo);
            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
        }

        public void SaveArticleList()
        {
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
                        var art = new ArticleData(selectitemid, _dataModuleParams.ModuleId, _editLang);
                        art.SortOrder = lp;
                        art.Update();
                        lp += 1;
                    }
                }
            }
            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
        }

        public void DeleteArticle()
        {
            if (_appThemeMod.AppTheme.DataType == 1)
            {
                _articleData = new ArticleData(_dataModuleParams.ModuleId, _editLang);
            }
            else
            {
                _articleData = new ArticleData(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
            }
            _articleData.Delete();
            _selectedItemId = -1;
            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
        }

        public String AddArticle()
        {
            try
            {
                var articleData = new ArticleData(-1, _dataModuleParams.ModuleId, _editLang);
                _selectedItemId = articleData.ItemId;

                CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);

                var strOut = GetArticle();

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public String GetArticleEdit(bool loadCachedHeader = true)
        {
            try
            {
                AssignEditLang(); //check for change of langauge
                if (_dataAppThemeMod.AppTheme.DataType == 1)
                {
                    return GetArticle();
                }
                else
                {
                    if (_selectedItemId <= 0)
                    {
                        return GetArticleList(loadCachedHeader);
                    }

                    return GetArticle();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public String GetArticle()
        {
            try
            {
                var razorTempl = _appThemeMod.GetTemplateRazor("edit");
                if (_moduleParams.DataSourceExternal)
                {
                    razorTempl = _dataAppThemeMod.GetTemplateRazor("edit");
                }

                var strOut = "-- NO DATA -- Itemid: " + _selectedItemId;

                if (_dataAppThemeMod.AppTheme.DataType == 1)
                {
                    _articleData = new ArticleData(_dataModuleParams.ModuleId, _editLang);
                }
                else
                {
                    _articleData = new ArticleData(_selectedItemId, _dataModuleParams.ModuleId, _editLang);
                }

                _articleData.ImageFolder = _dataModuleParams.ImageFolder;
                _articleData.DocumentFolder = _dataModuleParams.DocumentFolder;
                _articleData.AppTheme = _dataModuleParams.AppThemeFolder;
                _articleData.AppThemeVersion = _dataModuleParams.AppThemeVersion;
                _articleData.AppThemeRelPath = _dataModuleParams.AppThemeFolderRel;

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

                strOut = DNNrocketUtils.RazorDetail(razorTempl, _articleData, _passSettings, new SimplisityInfo(), _systemData.DebugMode);

                // if the module settings change to a single form, use the last edited record.
                _settingsData.Info.SetXmlProperty("genxml/selecteditemid", _selectedItemId.ToString());
                _settingsData.Update();

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public String GetArticleList(bool loadCachedHeader)
        {

            try
            {
                var strOut = "";
                var articleDataList = new ArticleDataList(_dataModuleParams.ModuleId, _editLang);
                if (loadCachedHeader)
                {
                    articleDataList.LoadCacheHeader();
                }
                else
                {
                    articleDataList.PageSize = _postInfo.GetXmlPropertyInt("genxml/hidden/pagesize");
                    articleDataList.Page = _paramInfo.GetXmlPropertyInt("genxml/hidden/page");
                    articleDataList.SearchText = _postInfo.GetXmlProperty("genxml/textbox/searchtext");
                    articleDataList.SaveCacheHeader();
                }
                articleDataList.Populate();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editlist.cshtml", "/DesktopModules/DNNrocket/SystemThemes/" + _systemData.SystemKey, _dataModuleParams.AppThemeFolder, _editLang, _dataModuleParams.AppThemeVersion, _systemData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, articleDataList, _passSettings, articleDataList.Header, _systemData.DebugMode);
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
                AssignEditLang(); //check for change of langauge
                var fieldsData = GetFieldsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, _editLang, "1.0", _systemData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, fieldsData, passSettings,null, _systemData.DebugMode);

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
                AssignEditLang(); //check for change of langauge
                var settingsData = GetSettingsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetEditCulture(), "1.0", _systemData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, settingsData, passSettings);

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
            return EditSettingsData();
        }

        private string AddSettingsRow()
        {
            var settingsData = GetSettingsData();
            settingsData.AddRow();
            return EditSettingsData();
        }

        private String SettingsSave()
        {
            var settingsData = GetSettingsData();
            settingsData.Save(_postInfo);
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

        public String ResetDataRocketMod(int moduleid)
        {
            try
            {
                var articleDataList = new ArticleDataList(moduleid, _editLang);
                articleDataList.DeleteAll();
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
                AssignEditLang(); //check for change of langauge
                var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder, _moduleParams.AppThemeVersion);
                if (!appTheme.EnableSettings) return "";
                var razorTempl = appTheme.GetTemplate("settings"); // new module, so settings theme will be systemtheme.
                _settingsData = GetSettingsData();
                return DNNrocketUtils.RazorDetail(razorTempl, _settingsData, _passSettings, null, true);
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
                AssignEditLang(); //check for change of langauge
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return DNNrocketUtils.RazorDetail(razorTempl, _moduleParams, _passSettings,null,true);
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
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return DNNrocketUtils.RazorDetail(razorTempl, mp, _passSettings, null, true);
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
                _moduleParams.DataSourceModId = selectmoduleid;
                _moduleParams.DataSourceModRef = selectmoduleref;
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
                AssignEditLang(); //check for change of langauge
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "apptheme.cshtml";
                var appThemeMod = new AppThemeModule(_moduleid, _systemData.SystemKey);

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                return DNNrocketUtils.RazorDetail(razorTempl, appThemeMod, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string SaveEditor()
        {
            var editorcode = _postInfo.GetXmlProperty("genxml/hidden/editorcodesave");
            var filename = _postInfo.GetXmlProperty("genxml/hidden/editorfilenamesave");
            var editorfileext = _postInfo.GetXmlProperty("genxml/hidden/editorfileext");
            _appThemeMod.SaveEditor(filename, editorfileext, editorcode);
            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
            return "OK";
        }

        public string RemoveTemplate()
        {
            var filename = _postInfo.GetXmlProperty("genxml/hidden/editorfilenamesave");
            var editorfileext = _postInfo.GetXmlProperty("genxml/hidden/editorfileext");
            _appThemeMod.RemoveModuleTemplate(filename, editorfileext);
            CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
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
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(),"1.0", true);
                    var passSettings = _postInfo.ToDictionary();
                    passSettings.Add("moduleid", _moduleParams.ModuleId.ToString());

                    return DNNrocketUtils.RazorDetail(razorTempl, _appThemeDataList, passSettings, null, true);
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
                    var passSettings = _paramInfo.ToDictionary();                 
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());
                    var adminurl = "/DesktopModules/DNNrocket/RocketMod/admin.html?moduleid=" + _moduleid + "&tabid=" + _tabid;
                    passSettings.Add("adminurl", adminurl);
                    var appTheme = new AppTheme(_systemData.SystemKey, appthemefolder, _moduleParams.AppThemeVersion);
                    passSettings.Add("datatype", appTheme.DataType.ToString());

                    if ((_paramInfo.GetXmlPropertyInt("genxml/urlparams/" + _moduleParams.DetailUrlParam) > 0) && _moduleParams.DetailView)
                    {
                        // detail display
                        var itemid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/" + _moduleParams.DetailUrlParam);

                        var razorTempl = _appThemeMod.GetTemplateRazor("detail");

                        var articleData = new ArticleData(_dataModuleParams.ModuleId, DNNrocketUtils.GetCurrentCulture());
                        articleData.Populate(itemid);

                        foreach (var s in _moduleParams.ModuleSettings)
                        {
                            if (!passSettings.ContainsKey(s.Key)) passSettings.Add(s.Key, s.Value);
                        }

                        passSettings.Add("DocumentFolderRel", _dataModuleParams.DocumentFolderRel);
                        passSettings.Add("ImageFolderRel", _dataModuleParams.ImageFolderRel);

                        if (passSettings.ContainsKey("tabid")) passSettings.Remove("tabid");
                        passSettings.Add("tabid", _tabid.ToString());

                        strOut = DNNrocketUtils.RazorDetail(razorTempl, articleData, passSettings);

                    }
                    else
                    {
                        // list display
                        var razorTempl = _appThemeMod.GetTemplateRazor("view");

                        var articleDataList = new ArticleDataList(_dataModuleParams.ModuleId, DNNrocketUtils.GetCurrentCulture());
                        articleDataList.Populate(appTheme.DataType);

                        foreach (var s in _moduleParams.ModuleSettings)
                        {
                            if (!passSettings.ContainsKey(s.Key)) passSettings.Add(s.Key, s.Value);
                        }

                        passSettings.Add("DocumentFolderRel", _dataModuleParams.DocumentFolderRel);
                        passSettings.Add("ImageFolderRel", _dataModuleParams.ImageFolderRel);

                        if (passSettings.ContainsKey("tabid")) passSettings.Remove("tabid");
                        passSettings.Add("tabid", _tabid.ToString());

                        strOut = DNNrocketUtils.RazorDetail(razorTempl, articleDataList, passSettings, articleDataList.Header);
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
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(),"1.0",_systemData.DebugMode);                
                return DNNrocketUtils.RazorDetail(razorTempl, _rocketInterface.Info,_passSettings, new SimplisityInfo(), _systemData.DebugMode);
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
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData("editlist.cshtml", _appthemeRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
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
            var exportData = new ExportData(_rocketInterface, _moduleid, _systemKey);
            return exportData.GetXml();
        }
        private void ImportData()
        {
            var oldmoduleid = _postInfo.GetXmlPropertyInt("export/moduleid");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var importData = new ImportData(_rocketInterface, portalid, _moduleid, oldmoduleid, _postInfo.XMLData);
            CacheUtils.ClearAllCache(_moduleParams.CacheGroupId);
            DNNrocketUtils.ClearAllCache();
        }

        private void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
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
                    var l2 = objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, "MODULEPARAMS", " and r1.XmlData.value('(genxml/hidden/moduletype)[1]','nvarchar(max)') = 'RocketMod'");
                    foreach (var sInfo in l2)
                    {
                        var moduleParams = new ModuleParams(sInfo.ModuleId, _systemKey);
                        if (DNNrocketUtils.ModuleExists(moduleParams.TabId, sInfo.ModuleId) && !DNNrocketUtils.ModuleIsDeleted(moduleParams.TabId, sInfo.ModuleId))
                        {
                            var exportData = new ExportData(_rocketInterface, moduleParams.ModuleId, moduleParams.SystemKey);
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
                var l = objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, "ROCKETMODLANG", " and r1.Lang = '" + copylanguage + "'","","",0,0,0,0, _tableName);
                foreach (var sInfo in l)
                {
                    var objRecLang = objCtrl.GetRecordLang(sInfo.ParentItemId, destinationlanguage, true, _tableName);
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

                CacheUtils.ClearAllCache();
                DNNrocketUtils.ClearAllCache();

            }

        }


        private void ValidateData()
        {
            var objCtrl = new DNNrocketController();

            // remove deleted modules
            var filter = "and r1.XMlData.value('(genxml/hidden/systemkey)[1]','nvarchar(max)') = '" + _systemKey + "' ";
            var dirlist = objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, "MODULEPARAMS", filter);
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

    }
}
