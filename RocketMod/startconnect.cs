using System;
using System.Collections.Generic;
using System.Linq;
using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketSettings;
using Simplisity;
using Rocket.AppThemes.Componants;
using RocketMod.Componants;

namespace RocketMod
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        //private static ModuleData _moduleData;
        private static string _appthemeRelPath;
        private static string _appthemeMapPath;
        private static string _appthemeSystemRelPath;
        private static string _appthemeSystemMapPath;
        private static string _rocketModRelPath;
        private static string _rocketModMapPath;
        private static SimplisityInfo _postInfo;
        private static SimplisityInfo _paramInfo;
        private static CommandSecurity _commandSecurity;
        private static DNNrocketInterface _rocketInterface;
        private static SimplisityInfo _systemInfo;
        private static ModuleParams _moduleParams;
        private static ModuleParams _dataModuleParams;
        private static int _tabid;
        private static int _moduleid;
        private static SystemInfoData _systemInfoData;
        private static string _systemKey;
        private static Dictionary<string, string> _passSettings;
        private static SettingsData _settingsData;
        private static string _editLang;
        private static int _selectedItemId;
        private static AppThemeModule _appThemeMod;
        private static AppThemeModule _dataAppThemeMod;
        private static ArticleData _articleData;
        private static AppThemeDataList _appThemeDataList;
        private static UserStorage _userStorage;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.

            paramCmd = initCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

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
                    RocketModAddListItem("imagelist" + _paramInfo.GetXmlProperty("genxml/hidden/imgfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_adddocument":
                    RocketModAddListItem("documentlist" + _paramInfo.GetXmlProperty("genxml/hidden/docfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_addlink":
                    RocketModAddListItem("linklist" + _paramInfo.GetXmlProperty("genxml/hidden/linkfieldname"));
                    strOut = GetArticleEdit();
                    break;
                case "rocketmodedit_saveconfig":
                    SaveConfig();
                    strOut = GetDashBoard();
                    break;
                case "rocketmodedit_reset":
                    strOut = ResetRocketMod();
                    break;
                case "rocketmodedit_resetdata":
                    strOut = ResetDataRocketMod();
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


            }

            if (strOut == "" && !_moduleParams.Exists)
            {
                return DNNrocketUtils.ReturnString(GetSetup());
            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static string initCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {

            _systemInfoData = new SystemInfoData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            _appthemeRelPath = "/DesktopModules/DNNrocket/AppThemes";
            _appthemeMapPath = DNNrocketUtils.MapPath(_appthemeRelPath);
            _appthemeSystemRelPath = "/DesktopModules/DNNrocket/AppThemes/SystemThemes";
            _appthemeSystemMapPath = DNNrocketUtils.MapPath(_appthemeSystemRelPath);
            _rocketModRelPath = "/DesktopModules/DNNrocket/RocketMod";
            _rocketModMapPath = DNNrocketUtils.MapPath(_rocketModRelPath);

            _postInfo = postInfo;
            _systemInfo = systemInfo;
            _systemKey = _systemInfoData.SystemKey;

            // set editlang from url param or cache
            _editLang = DNNrocketUtils.GetEditCulture();

            _userStorage = new UserStorage();
            _paramInfo = paramInfo;
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
                        var menucmd = _userStorage.Get(_systemKey + "-s-menu-cmd");
                        if (menucmd != "")
                        {
                            paramCmd = menucmd;
                            var paramXml = _userStorage.Get(_systemKey + "-s-menu-paramInfo");
                            if (paramXml != "") _paramInfo.FromXmlItem(paramXml);

                            // we may have a diiferent rocketInterface if we have reloaded from the last command.
                            var interfacekey = paramInfo.GetXmlProperty("genxml/hidden/interfacekey");
                            if (interfacekey == "") interfacekey = paramInfo.GetXmlProperty("genxml/urlparams/interfacekey").Trim(' ');
                            if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];
                            var rocketInterface = new DNNrocketInterface(systemInfo, interfacekey);
                            interfaceInfo = rocketInterface.Info;
                        }
                    }
                }
                else
                {
                    if (_paramInfo.GetXmlPropertyBool("genxml/hidden/track"))
                    {
                        _userStorage.Track(_systemKey, paramCmd, _paramInfo);
                    }
                }
            }

            // we should ALWAYS pass back the moduleid & tabid in the template post.
            // But for the admin start we need it to be passed by the admin.aspx url parameters.  Which then puts it in the s-fields for the simplsity start call.
            _moduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
            if (_moduleid == 0) _moduleid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
            _tabid = _paramInfo.GetXmlPropertyInt("genxml/hidden/tabid"); // needed for security.
            if (_tabid == 0) _tabid = _paramInfo.GetXmlPropertyInt("genxml/urlparams/tabid");

            _selectedItemId = _paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

            _editLang = DNNrocketUtils.GetEditCulture();

            _settingsData = GetSettingsData();
            _passSettings = LocalUtils.SettingsToDictionary(_settingsData);

            _moduleParams = new ModuleParams(_moduleid, _systemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.DataSourceModId, _systemKey);

            if (paramCmd != "rocketmod_getdata") CacheUtils.ClearAllCache(_moduleParams.CacheGroupId);

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
                if (!_moduleParams.Exists && paramCmd != "module_import" && paramCmd != "rocketmod_getdata" && paramCmd != "rocketmodedit_saveapptheme" && paramCmd != "rocketmodedit_saveconfig" && paramCmd != "rocketmodedit_saveappthemeconfig")
                {
                    return "rocketmodedit_selectapptheme";
                }
                else
                {
                    _appThemeMod = new AppThemeModule(_moduleid, _systemInfoData.SystemKey);
                    _dataAppThemeMod = new AppThemeModule(_moduleParams.DataSourceModId, _systemInfoData.SystemKey);

                    if (!_passSettings.ContainsKey("AppTheme")) _passSettings.Add("AppTheme", _moduleParams.AppThemeFolder);
                    if (!_passSettings.ContainsKey("AppThemeVersion")) _passSettings.Add("AppThemeVersion", _moduleParams.AppThemeVersion);
                    if (!_passSettings.ContainsKey("AppThemeRelPath")) _passSettings.Add("AppThemeRelPath", _moduleParams.AppThemeFolderRel);
                }
            }

            return paramCmd;
        }
        public static bool CheckSecurity(string paramCmd)
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

        public static void RocketModAddListItem(string listname)
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

        private static void SaveConfig()
        {
            var appTheme = new AppTheme(_systemInfoData.SystemKey, _moduleParams.AppThemeFolder);
            _moduleParams.AppThemeLogo = appTheme.Logo;
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.ImageFolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            _moduleParams.DocumentFolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            _moduleParams.AppThemeVersion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");
            _moduleParams.AppThemeNotes = _postInfo.GetXmlProperty("genxml/hidden/appthemenotes");
            _moduleParams.ModuleType = "RocketMod";
            _moduleParams.Exists = true;
            _moduleParams.CacheDisbaled = _postInfo.GetXmlPropertyBool("genxml/hidden/disbalecache");

            if (_systemInfoData.DebugMode) _moduleParams.CacheDisbaled = true;

            _moduleParams.ShareData = _postInfo.GetXmlProperty("genxml/hidden/sharedata");
            _moduleParams.TabId = _tabid;
            _moduleParams.SystemKey = _systemInfoData.SystemKey;
            _moduleParams.ExportResourceFiles = _postInfo.GetXmlPropertyBool("genxml/hidden/exportresourcefiles");

            _moduleParams.Save();
            _passSettings.Add("saved", "true");
        }

        private static void SaveAppTheme()
        {
            _moduleParams.AppThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
            var appTheme = new AppTheme(_systemInfoData.SystemKey, _moduleParams.AppThemeFolder);
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
        }

        #region "Articles"

        public static Dictionary<string,string> DownloadDocument()
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
        public static void SaveArticle()
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
            _articleData.DebugMode = _systemInfoData.DebugMode;
            _articleData.Save(_postInfo);
        }

        public static void SaveArticleList()
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
        }

        public static void DeleteArticle()
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
        }

        public static String AddArticle()
        {
            try
            {
                var articleData = new ArticleData(-1, _dataModuleParams.ModuleId, _editLang);
                _selectedItemId = articleData.ItemId;
                var strOut = GetArticle();

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String GetArticleEdit(bool loadCachedHeader = true)
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


        public static String GetArticle()
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

                strOut = DNNrocketUtils.RazorDetail(razorTempl, _articleData, _passSettings, new SimplisityInfo(), _systemInfoData.DebugMode);

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

        public static String GetArticleList(bool loadCachedHeader)
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

                var razorTempl = DNNrocketUtils.GetRazorTemplateData("editlist.cshtml", _appthemeRelPath + "/SystemThemes/" + _systemInfoData.SystemKey, _dataModuleParams.AppThemeFolder, _editLang, _dataModuleParams.AppThemeVersion, _systemInfoData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, articleDataList, _passSettings, articleDataList.Header, _systemInfoData.DebugMode);
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion



        #region "Fields"

        private static SettingsData GetFieldsData(int moduleid = 0)
        {
            if (moduleid == 0) moduleid = _moduleid;
            var settingsData = new SettingsData(_tabid, moduleid, _editLang, "ROCKETMODFIELDS", "fielddata", false, _rocketInterface.DatabaseTable);
            return settingsData;
        }

        private static String EditFieldsData()
        {
            try
            {
                AssignEditLang(); //check for change of langauge
                var fieldsData = GetFieldsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, _editLang, "1.0", _systemInfoData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, fieldsData, passSettings,null, _systemInfoData.DebugMode);

                if (strOut == "") strOut = "ERROR: No data returned for EditfieldsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private static string FieldsDelete()
        {
            var fieldsData = GetFieldsData();
            fieldsData.Delete();
            return EditFieldsData();
        }

        private static string AddFieldsRow()
        {
            var fieldsData = GetFieldsData();
            fieldsData.AddRow();
            return EditFieldsData();
        }

        private static String FieldsSave()
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

        private static SettingsData GetSettingsData()
        {
            return new SettingsData(_tabid, _moduleid, _editLang, "ROCKETMODSETTINGS", "rocketmodsettings", true, _rocketInterface.DatabaseTable);
        }
        private static String EditSettingsData()
        {
            try
            {
                AssignEditLang(); //check for change of langauge
                var settingsData = GetSettingsData();
                var strOut = "";
                var passSettings = _paramInfo.ToDictionary();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetEditCulture(), "1.0", _systemInfoData.DebugMode);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, settingsData, passSettings);

                if (strOut == "") strOut = "ERROR: No data returned for EditSettingsData() : " + _rocketInterface.TemplateRelPath + "/Themes/" + _rocketInterface.DefaultTheme + "/default/" + _rocketInterface.DefaultTemplate;
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private static string SettingsDelete()
        {
            var settingsData = GetSettingsData();
            settingsData.Delete();
            return EditSettingsData();
        }

        private static string AddSettingsRow()
        {
            var settingsData = GetSettingsData();
            settingsData.AddRow();
            return EditSettingsData();
        }

        private static String SettingsSave()
        {
            var settingsData = GetSettingsData();
            settingsData.Save(_postInfo);
            return EditSettingsData();
        }

        #endregion

        public static String ResetRocketMod()
        {
            try
            {
                _moduleParams.Delete();

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleid" + _moduleid, "ROCKETMODFIELDS", "", _moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);
                info = objCtrl.GetData("moduleid" + _moduleid, "ROCKETMODSETTINGS", "", _moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);

                return GetSelectApp();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String ResetDataRocketMod()
        {
            try
            {
                var articleDataList = new ArticleDataList(_moduleid, _editLang);
                articleDataList.DeleteAll();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static String GetSettingSection()
        {
            try
            {
                AssignEditLang(); //check for change of langauge
                var appTheme = new AppTheme(_systemInfoData.SystemKey, _moduleParams.AppThemeFolder, _moduleParams.AppThemeVersion);
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

        public static String GetDashBoard()
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

        public static String GetDataSourceList()
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
        public static String GetDataSourceSelect()
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

        public static String GetAppModTheme()
        {
            try
            {
                AssignEditLang(); //check for change of langauge
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "apptheme.cshtml";
                var appThemeMod = new AppThemeModule(_moduleid, _systemInfoData.SystemKey);

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                return DNNrocketUtils.RazorDetail(razorTempl, appThemeMod, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static string SaveEditor()
        {
            var editorcode = _postInfo.GetXmlProperty("genxml/hidden/editorcodesave");
            var filename = _postInfo.GetXmlProperty("genxml/hidden/editorfilenamesave");
            var editorfileext = _postInfo.GetXmlProperty("genxml/hidden/editorfileext");
            _appThemeMod.SaveEditor(filename, editorfileext, editorcode);
            return "OK";
        }

        public static string RemoveTemplate()
        {
            var filename = _postInfo.GetXmlProperty("genxml/hidden/editorfilenamesave");
            var editorfileext = _postInfo.GetXmlProperty("genxml/hidden/editorfileext");
            _appThemeMod.RemoveModuleTemplate(filename, editorfileext);
            return GetAppModTheme();
        }


        public static String GetSelectApp()
        {

            try
            {
                var strOut = "";
                if (!_moduleParams.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    _appThemeDataList = new AppThemeDataList(_systemInfoData.SystemKey);

                    var razortemplate = "selectapp.cshtml";
                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, _rocketModRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(),"1.0", _systemInfoData.DebugMode);
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


        public static String GetDisplay()
        {
            try
            {
                var strOut = "";
                if (_moduleParams.Exists)
                {
                    var objCtrl = new DNNrocketController();

                    var appthemefolder = _moduleParams.AppThemeFolder;
                    var razorTempl = _appThemeMod.GetTemplateRazor("view");
                    var passSettings = _paramInfo.ToDictionary();                 
                    passSettings.Add("addeditscript", _commandSecurity.HasModuleEditRights().ToString());
                    var adminurl = "/DesktopModules/DNNrocket/RocketMod/admin.html?moduleid=" + _moduleid + "&tabid=" + _tabid;
                    passSettings.Add("adminurl", adminurl);
                    var appTheme = new AppTheme(_systemInfoData.SystemKey, appthemefolder, _moduleParams.AppThemeVersion);
                    passSettings.Add("datatype", appTheme.DataType.ToString());
                    var articleDataList = new ArticleDataList(_dataModuleParams.ModuleId, DNNrocketUtils.GetCurrentCulture());
                    articleDataList.Populate(appTheme.DataType);

                    foreach (var s in _moduleParams.ModuleSettings)
                    {
                        if (!passSettings.ContainsKey(s.Key)) passSettings.Add(s.Key, s.Value);
                    }

                    passSettings.Add("DocumentFolderRel", _dataModuleParams.DocumentFolderRel);
                    passSettings.Add("ImageFolderRel", _dataModuleParams.ImageFolderRel);                    

                    strOut = DNNrocketUtils.RazorDetail(razorTempl, articleDataList, passSettings, articleDataList.Header);

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

        private static String GetSetup()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                if (!_passSettings.ContainsKey("tabid")) _passSettings.Add("tabid", _tabid.ToString());
                var strOut = "";
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(),"1.0",_systemInfoData.DebugMode);
                return DNNrocketUtils.RazorDetail(razorTempl, _rocketInterface.Info,_passSettings, new SimplisityInfo(), _systemInfoData.DebugMode);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static bool CheckIfList()
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

        private static string ExportData()
        {
            var exportData = new ExportData(_rocketInterface, _moduleid, _systemKey);
            return exportData.GetXml();
        }
        private static void ImportData()
        {
            var oldmoduleid = _postInfo.GetXmlPropertyInt("export/moduleid");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var importData = new ImportData(_rocketInterface, portalid, _moduleid, oldmoduleid, _postInfo.XMLData);
            CacheUtils.ClearAllCache(_moduleParams.CacheGroupId);
            DNNrocketUtils.ClearAllCache();
        }

        private static void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }


    }
}
