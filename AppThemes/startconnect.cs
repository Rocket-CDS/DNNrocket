using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace DNNrocket.AppThemes
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private string _editLang;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private string _appThemeFolder;
        private string _appVersionFolder;
        private const string _tableName = "DNNRocket";
        private UserParams _UserParams;
        private string _selectedSystemKey;
        private string _paramCmd;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR - Must be SuperUser"; // return ERROR if not matching commands.

            _passSettings = new Dictionary<string, string>(); 
            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _editLang = langRequired;
            if (_editLang == "") _editLang = DNNrocketUtils.GetEditCulture();

            if (UserUtils.IsSuperUser())
            {
                // Do NOT globally clear cache, performace drop on external call for FTP and HTML AppTheme Lists
                //CacheUtilsDNN.ClearAllCache("apptheme");

                _paramCmd = paramCmd;
                _UserParams = new UserParams(UserUtils.GetCurrentUserId());

                if (_paramInfo.GetXmlPropertyBool("genxml/hidden/reload"))
                {
                    var menucmd = _UserParams.GetCommand(_systemData.SystemKey);
                    if (menucmd != "")
                    {
                        paramCmd = menucmd;
                        _paramInfo = _UserParams.GetParamInfo(_systemData.SystemKey);
                        var interfacekey = _UserParams.GetInterfaceKey(_systemData.SystemKey);
                        _rocketInterface = new RocketInterface(systemInfo, interfacekey);
                    }
                }
                else
                {
                    if (_paramInfo.GetXmlPropertyBool("genxml/hidden/track"))
                    {
                        _UserParams.Track(_systemData.SystemKey, paramCmd, _paramInfo, _rocketInterface.InterfaceKey);
                    }
                }


                _appThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
                _appVersionFolder = _paramInfo.GetXmlProperty("genxml/hidden/appversionfolder");
                if (_appVersionFolder == "") _appVersionFolder = _UserParams.Get("selectedappversion");
                if (_appVersionFolder == "") _appVersionFolder = "1.0";
                _UserParams.Set("selectedappversion", _appVersionFolder);

                if (_paramInfo.GetXmlPropertyBool("genxml/hidden/clearselectedsystemkey"))
                {
                    _UserParams.Delete();
                    _UserParams.Set("selectedsystemkey", "");
                }

                _selectedSystemKey = _paramInfo.GetXmlProperty("genxml/hidden/urlparamsystemkey");
                if (_selectedSystemKey == "")  _selectedSystemKey = _paramInfo.GetXmlProperty("genxml/hidden/selectedsystemkey");
                if (_selectedSystemKey == "")
                {
                    _selectedSystemKey = _UserParams.Record.GetXmlProperty("genxml/hidden/selectedsystemkey");
                }
                else
                {
                    _UserParams.Set("selectedsystemkey", _selectedSystemKey);
                    _UserParams.Update();
                }

                AssignEditLang();

                switch (paramCmd)
                {
                    case "rocketapptheme_getlist":
                        strOut = GetList();
                        break;
                    case "rocketapptheme_getdetail":
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_save":
                        SaveData();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addimage":
                        AddListImage();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addresx":
                        AddResxFile();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addcss":
                        AddCssFile();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addjs":
                        AddJsFile();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_addtemplate":
                        AddTemplateFile();
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_rebuildresx":
                        strOut = RebuildResx();
                        break;  
                    case "rocketapptheme_addfield":
                        SaveData();
                        strOut = AddListField();
                        break;
                    case "rocketapptheme_addsettingfield":
                        SaveData();
                        strOut = AddSettingListField();
                        break;
                    case "rocketapptheme_createversion":
                        strOut = CreateNewVersion();
                        break;
                    case "rocketapptheme_changeversion":
                        _appVersionFolder = _paramInfo.GetXmlProperty("genxml/hidden/appversionfolder");
                        var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                        appTheme.Record.SetXmlProperty("genxml/select/versionfolder", _appVersionFolder);
                        appTheme.Update();
                        _UserParams.Set("selectedappversion", _appVersionFolder);
                        strOut = GetDetail();
                        break;
                    case "rocketapptheme_deleteversion":
                        strOut = DeleteVersion();
                        break;
                    case "rocketapptheme_deletetheme":
                        strOut = DeleteTheme();
                        break;
                    case "rocketapptheme_addapptheme":
                        strOut = CreateNewAppTheme();
                        break;
                    case "rocketapptheme_export":
                        return ExportAppTheme();
                    case "rocketapptheme_import":
                        strOut = ImportAppTheme();
                        break;
                    case "rocketapptheme_docopy":
                        strOut = DoCopyAppTheme();
                        break;
                    case "rocketapptheme_uploadversioncheck":
                        strOut = CheckServerVersion();
                        break;
                    case "rocketapptheme_uploadapptheme":
                        strOut = UploadAppTheme();
                        break;
                    case "rocketapptheme_refreshprivatelist":
                        var ftpConnect = new FtpConnect(_selectedSystemKey);
                        ftpConnect.ReindexPrivateXmlList();
                        strOut = GetPrivateListAppTheme(false);
                        break;
                    case "rocketapptheme_getprivatelist":
                        strOut = GetPrivateListAppTheme(true);
                        break;
                    case "rocketapptheme_saveftpdetails":
                        strOut = SaveServerDetails();
                        break;
                    case "rocketapptheme_downloadprivate":
                        strOut = GetPrivateAppTheme();
                        break;
                    case "rocketapptheme_refreshpubliclist":
                        strOut = GetPublicListAppTheme(false);
                        break;
                    case "rocketapptheme_getpubliclist":
                        strOut = GetPublicListAppTheme(true);
                        break;
                    case "rocketapptheme_downloadpublic":
                        strOut = GetPublicAppTheme();
                        break;
                    case "rocketapptheme_downloadallpublic":
                        strOut = GetAllPublicAppThemes();
                        break;
                    case "rocketapptheme_downloadallprivate":
                        strOut = GetAllPrivateAppThemes();
                        break;

                    case "rocketapptheme_getresxdata":
                        strOut = GetResxDetail();
                        break;
                    case "rocketapptheme_addresxdata":
                        strOut = AddResxDetail();
                        break;
                    case "rocketapptheme_removeresxdata":
                        strOut = RemoveResxDetail();
                        break;
                    case "rocketapptheme_saveresxdata":
                        strOut = SaveResxDetail();
                        break;

                    case "rocketapptheme_geteditor":
                        strOut = GetEditorDetail();
                        break;
                    case "rocketapptheme_saveeditor":
                        strOut = SaveEditor();
                        break;

                    case "rocketapptheme_geneditform":
                        strOut = AppThemeUtils.GenerateEditForm(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                        break;
                    case "rocketapptheme_genviewform":
                        strOut = AppThemeUtils.GenerateView(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                        break;
                    case "rocketapptheme_gensettingform":
                        strOut = AppThemeUtils.GenerateSettingForm(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                        break;
                    case "rocketapptheme_deletefile":
                        DeleteFile();
                        strOut = GetDetail();
                        break;
                        
                }
            }
            else
            {
                strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }

            return DNNrocketUtils.ReturnString(strOut);
        }

        public void DeleteFile()
        {
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            if (filename != "")
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var fileNameMapPath = "";
                if (Path.GetExtension(filename) == ".css") fileNameMapPath = appTheme.AppThemeVersionFolderMapPath.Trim('\\') + "\\css\\" + filename;
                if (Path.GetExtension(filename) == ".resx") fileNameMapPath = appTheme.AppThemeVersionFolderMapPath.Trim('\\') + "\\resx\\" + filename;
                if (Path.GetExtension(filename) == ".js") fileNameMapPath = appTheme.AppThemeVersionFolderMapPath.Trim('\\') + "\\js\\" + filename;
                if (Path.GetExtension(filename) == ".cshtml") fileNameMapPath = appTheme.AppThemeVersionFolderMapPath.Trim('\\') + "\\default\\" + filename;

                File.Delete(fileNameMapPath);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public string CreateNewVersion()
        {
            try
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var rtn = appTheme.CopyVersion(appTheme.AppVersion, appTheme.LatestVersion + 1);
                _appVersionFolder = appTheme.AppVersionFolder.ToString();
                _UserParams.Set("selectedappversion", _appVersionFolder);
                ClearServerCacheLists();
                if (rtn != "") return rtn;
                return GetDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string DeleteVersion()
        {
            try
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                appTheme.DeleteVersion();
                _appVersionFolder = appTheme.AppVersionFolder;
                ClearServerCacheLists();
                return GetDetail();
            }
            catch (Exception)
            {
                var strErr = "<div class='w3-panel w3-red'>";
                strErr += "<p>" + DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/Api/App_LocalResources", "DNNrocket.deletethemeerror") + "</p>";
                strErr += "</div>";
                return strErr;
            }
        }
        public string DeleteTheme()
        {
            try
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                appTheme.DeleteTheme();
                ClearServerCacheLists();
                return GetList();
            }
            catch (Exception)
            {
                var strErr = "<div class='w3-panel w3-red'>";
                strErr += "<p>" + DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/Api/App_LocalResources", "DNNrocket.deletethemeerror") + "</p>";
                strErr += "</div>";
                return strErr;
            }
        }

        public void ClearServerCacheLists()
        {
            // clear all cache for aptheme
            CacheUtilsDNN.ClearAllCache();
            CacheFileUtils.ClearAllCache();
            DNNrocketUtils.ClearPortalCache();
            CacheUtils.ClearAllCache();
        }

        public string CreateNewAppTheme()
        {
            try
            {
                var appthemeprefix = GeneralUtils.AlphaNumeric(FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemeprefix")));
                var appthemename = GeneralUtils.AlphaNumeric(FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemename")));

                var newAppThemeName = appthemename;
                if (appthemeprefix != "") newAppThemeName = appthemeprefix + "_" + newAppThemeName;

                var appSystemThemeFolderRel = "/DesktopModules/RocketThemes/" + _selectedSystemKey;
                var appThemeFolderRel = appSystemThemeFolderRel + "/" + newAppThemeName;
                var appThemeFolderMapPath = DNNrocketUtils.MapPath(appThemeFolderRel);

                if (Directory.Exists(appThemeFolderMapPath))
                {
                    return DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/AppThemes/App_LocalResources/", "AppThemes.appthemeexists");
                }

                // crearte new apptheme.
                var appTheme = new AppTheme(_selectedSystemKey, newAppThemeName, "1.0");
                appTheme.AppThemePrefix = appthemeprefix;
                appTheme.AppThemeName = newAppThemeName;
                appTheme.Update();

                _UserParams.Set("selectedappversion", "1.0");

                ClearServerCacheLists();

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetResxDetail()
        {
            try
            {
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var resxData = new ResxData(appTheme.GetFileMapPath(fname));
                var dataObjects = new Dictionary<string, object>();
                dataObjects.Add("resxData", resxData);
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("ResxPopUp.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
                return RenderRazorUtils.RazorObjectRender(razorTempl, appTheme, dataObjects, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String AddResxDetail()
        {
            try
            {
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var resxData = new ResxData(appTheme.GetFileMapPath(fname));

                var key = "new" + (resxData.DataDictionary.Count + 1).ToString() + ".Text";
                var lp = (resxData.DataDictionary.Count + 1);
                while (resxData.DataDictionary.ContainsKey(key))
                {
                    lp += 1;
                    key = "new" + (lp).ToString() + ".Text";
                }
                resxData.AddField(key, "");
                resxData.Update();
                return GetResxDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String RemoveResxDetail()
        {
            try
            {
                var key = _paramInfo.GetXmlProperty("genxml/hidden/key");
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var resxData = new ResxData(appTheme.GetFileMapPath(fname));
                resxData.RemoveField(key);
                resxData.Update();
                return GetResxDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String SaveResxDetail()
        {
            try
            {
                var key = _paramInfo.GetXmlProperty("genxml/hidden/key");
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var resxData = new ResxData(appTheme.GetFileMapPath(fname));
                resxData.RemoveAllFields();
                var resxlist = _postInfo.GetRecordList("resxdictionarydata");
                foreach (var r in resxlist)
                {
                    resxData.AddField(r.GetXmlProperty("genxml/key"), r.GetXmlProperty("genxml/value"));
                }
                resxData.Update();
                return GetResxDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public String GetDetail()
        {
            try
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                return GetEditTemplate(appTheme);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String GetList()
        {
            try
            {
                var appThemeDataList = new AppThemeDataList(_selectedSystemKey);
                var template = _rocketInterface.DefaultTemplate;
                if (template == "") template = "appthemelist.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(template, appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(),"1.0",true);
                var passSettings = _postInfo.ToDictionary();
                passSettings.Add("AppProjectThemesFolderMapPath", appThemeDataList.AppProjectThemesFolderMapPath);

                return RenderRazorUtils.RazorDetail(razorTempl, appThemeDataList, passSettings,null,true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        public string GetAllPublicAppThemes()
        {
            try
            {
                var appThemeDataPublicList = new AppThemeDataPublicList(_selectedSystemKey, true);
                foreach (var a in appThemeDataPublicList.List)
                {
                    DownloadPublicAppTheme(a.GetXmlProperty("genxml/hidden/appthemefolder"));
                }
                return GetPublicListAppTheme(true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }
        public string GetAllPrivateAppThemes()
        {
            try
            {
                var appThemeDataPrivateList = new AppThemeDataPublicList(_selectedSystemKey, true);
                foreach (var a in appThemeDataPrivateList.List)
                {
                    if (!a.GetXmlPropertyBool("genxml/hidden/islatestversion") && !a.GetXmlPropertyBool("genxml/hidden/localupdated"))
                    {
                        DownloadPrivateAppTheme(a.GetXmlProperty("genxml/hidden/appthemefolder"));
                    }
                }
                return GetPrivateListAppTheme(true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }
        private void DownloadPublicAppTheme(string appThemeFolder = "")
        {
            if (appThemeFolder == "") appThemeFolder = _appThemeFolder;
            var httpConnect = new HttpConnect(_selectedSystemKey);
            var userid = UserUtils.GetCurrentUserId();
            var destinationMapPath = PortalUtils.TempDirectoryMapPath() + "\\" + userid + "_" + appThemeFolder + ".zip";
            httpConnect.DownloadAppThemeToFile(appThemeFolder, destinationMapPath);
            var appTheme = new AppTheme(_selectedSystemKey, destinationMapPath, true);
            appTheme.Update();
            File.Delete(destinationMapPath);
            ClearServerCacheLists();
        }

        public string GetPublicAppTheme()
        {
            try
            {
                var appThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
                DownloadPublicAppTheme(appThemeFolder);
                return GetPublicListAppTheme(true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        public string GetPublicListAppTheme(bool useCache)
        {
            try
            {
                // use cache on lists, because of time to build
                var strOut = "";
                var cacheKey = "dnnrocket*GetPublicListAppTheme*" + _selectedSystemKey;
                var changesystemkey = _paramInfo.GetXmlPropertyBool("genxml/hidden/changesystemkey");
                useCache = (useCache && !changesystemkey);
                if (useCache)
                {
                    strOut = (string)CacheUtilsDNN.GetCache(cacheKey);
                    if (!String.IsNullOrEmpty(strOut)) return strOut;
                }
                var appThemeDataList = new AppThemeDataList(_selectedSystemKey);
                var appThemeDataPublicList = new AppThemeDataPublicList(_selectedSystemKey, useCache);
                var template = "AppThemeOnlinePublicList.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(template, appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                var passSettings = _postInfo.ToDictionary();

                if (changesystemkey)
                {
                    // clear privatre cache
                    var cachekey = "AppThemeDataPrivateList" + "*" + UserUtils.GetCurrentUserId();
                    CacheUtilsDNN.RemoveCache(cachekey);
                    cachekey = "AppThemeDataPrivateList" + "*SystemFolders" + UserUtils.GetCurrentUserId();
                    CacheUtilsDNN.RemoveCache(cachekey);
                }

                strOut = RenderRazorUtils.RazorDetail(razorTempl, appThemeDataPublicList, passSettings, null, true);
                CacheUtilsDNN.SetCache(cacheKey, strOut);
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        private void DownloadPrivateAppTheme(string appThemeFolder = "")
        {
            if (appThemeFolder == "") appThemeFolder = _appThemeFolder;

            var ftpConnect = new FtpConnect(_selectedSystemKey);
            var userid = UserUtils.GetCurrentUserId();
            var destinationMapPath = PortalUtils.TempDirectoryMapPath() + "\\" + userid + "_" + appThemeFolder + ".zip";
            ftpConnect.DownloadAppThemeToFile(appThemeFolder, destinationMapPath);
            if (File.Exists(destinationMapPath))
            {
                var appTheme = new AppTheme(_selectedSystemKey, destinationMapPath, true);
                appTheme.Update();
                File.Delete(destinationMapPath);
            }
            ClearServerCacheLists();
        }
        public string GetPrivateAppTheme(string appThemeFolder = "")
        {
            try
            {
                DownloadPrivateAppTheme(appThemeFolder);
                return GetPrivateListAppTheme(true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        public string GetPrivateListAppTheme(bool useCache)
        {
            try
            {
                // use cache on lists, because of time to build
                var strOut = "";
                var cacheKey = "dnnrocket*GetPrivateListAppTheme*" + _selectedSystemKey;
                var changesystemkey = _paramInfo.GetXmlPropertyBool("genxml/hidden/changesystemkey");
                useCache = (useCache && !changesystemkey);
                if (useCache)
                {
                    strOut = (string)CacheUtilsDNN.GetCache(cacheKey);
                    if (!String.IsNullOrEmpty(strOut)) return strOut;
                }
                var appThemeDataList = new AppThemeDataList(_selectedSystemKey);
                var appThemeDataPrivateList = new AppThemeDataPrivateList(appThemeDataList.SelectedSystemKey, useCache);
                var template = "AppThemeOnlinePrivateList.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(template, appThemeDataList.AppProjectFolderRel, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                var passSettings = _postInfo.ToDictionary();

                if (changesystemkey)
                {
                    // clear public cache
                    var cachekey = "AppThemeDataPublicList" + "*" + UserUtils.GetCurrentUserId();
                    CacheUtilsDNN.RemoveCache(cachekey);
                    cachekey = "AppThemeDataPublicList" + "*SystemFolders" + UserUtils.GetCurrentUserId();
                    CacheUtilsDNN.RemoveCache(cachekey);
                }


                strOut = RenderRazorUtils.RazorDetail(razorTempl, appThemeDataPrivateList, passSettings, null, true);
                CacheUtilsDNN.SetCache(cacheKey, strOut);
                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        private Dictionary<string, object> ExportAppTheme()
        {
            var appThemeFolder = _paramInfo.GetXmlProperty("genxml/urlparams/appthemefolder");
            if (appThemeFolder == "") appThemeFolder = _appThemeFolder;
            var appVersionFolder = _paramInfo.GetXmlProperty("genxml/urlparams/appversionfolder");
            if (appVersionFolder == "") appVersionFolder = _appVersionFolder;
            var appTheme = new AppTheme(_selectedSystemKey, appThemeFolder, appVersionFolder);

            var exportZipMapPath = appTheme.ExportZipFile();

            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", exportZipMapPath);
            rtnDic.Add("downloadname", appTheme.AppThemeFolder + ".zip");

            return rtnDic;
        }
        private string CheckServerVersion()
        {
            var rtnDic = ExportAppTheme();
            if (rtnDic["filenamepath"] != null && (string)rtnDic["filenamepath"] != "")
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var ftpConnect = new FtpConnect(_selectedSystemKey);

                var serverxml = ftpConnect.DownloadAppThemeXml(_appThemeFolder);
                double version = -1;
                double rev = -1;
                var sRec = new SimplisityRecord();
                if (!String.IsNullOrEmpty(serverxml) && serverxml.ToUpper() != "FAIL")
                {
                    sRec.XMLData = serverxml;
                    version = sRec.GetXmlPropertyDouble("item/genxml/hidden/latestversion");
                    rev = sRec.GetXmlPropertyDouble("item/genxml/hidden/latestrev");
                }
                if (appTheme.LatestVersion > version)
                {
                    return UploadAppTheme();
                }
                if (appTheme.LatestVersion == version)
                {
                    var razorTempl2 = RenderRazorUtils.GetRazorTemplateData("versioncheckequal.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
                    return RenderRazorUtils.RazorDetail(razorTempl2, new SimplisityInfo(sRec), _passSettings, null, true);
                }
                var razorTempl1 = RenderRazorUtils.GetRazorTemplateData("versioncheck.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
                return RenderRazorUtils.RazorDetail(razorTempl1, new SimplisityInfo(sRec), _passSettings, null, true);
            }
            var razorTempl = RenderRazorUtils.GetRazorTemplateData("ftpfail.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
        }
        private string UploadAppTheme()
        {
            var rtnDic = ExportAppTheme();
            if (rtnDic["filenamepath"] != null && (string)rtnDic["filenamepath"] != "")
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var ftpConnect = new FtpConnect(_selectedSystemKey);
                var rtn = ftpConnect.UploadAppTheme(appTheme);
                var appThemeDataPrivateList = new AppThemeDataPrivateList(_selectedSystemKey, true);
                appThemeDataPrivateList.ClearCache();
                ClearServerCacheLists();
                return rtn;
            }
            var razorTempl = RenderRazorUtils.GetRazorTemplateData("ftpfail.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
        }

        private string ImportAppTheme()
        {
            var fileuploadlist = _paramInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var userid = UserUtils.GetCurrentUserId();
                        var userFolder = PortalUtils.TempDirectoryMapPath();
                        var friendlyname = GeneralUtils.DeCode(f);
                        var fname = userFolder + "\\" + userid + "_" + friendlyname;
                        var _appTheme = new AppTheme(_selectedSystemKey, fname, true);
                        // delete import file
                        File.Delete(fname);
                    }
                }
                ClearServerCacheLists();
            }
            return GetList();
        }

        public void SaveData()
        {
            var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
            appTheme.Save(_postInfo);
            ClearServerCacheLists();
        }
        public string DoCopyAppTheme()
        {
            try
            {
                var appthemeprefix = FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemeprefix"));
                var appthemename = FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemename"));
                var newAppThemeName = appthemename;
                if (appthemeprefix != "") newAppThemeName = appthemeprefix + "_" + newAppThemeName;
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                appTheme.Copy(newAppThemeName);
                appTheme = new AppTheme(_selectedSystemKey, newAppThemeName, appTheme.LatestVersionFolder);
                ClearServerCacheLists();
                return GetDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string SaveServerDetails()
        {
            var systemGlobalData = new SystemGlobalData();
            systemGlobalData.FtpServer = _postInfo.GetXmlProperty("genxml/textbox/ftpserver");
            systemGlobalData.FtpUserName = _postInfo.GetXmlProperty("genxml/textbox/ftpuser");
            systemGlobalData.FtpPassword = _postInfo.GetXmlProperty("genxml/textbox/ftppassword");
            systemGlobalData.Update();

            var systemData = new SystemLimpet(_selectedSystemKey);
            systemData.FtpRoot = _postInfo.GetXmlProperty("genxml/textbox/ftproot");
            systemData.Update();

            ClearServerCacheLists();

            return "OK";
        }

        public String GetEditorDetail()
        {
            try
            {
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var jsonString = GeneralUtils.EnCode(appTheme.GetTemplate(fname));
                _passSettings.Add("filename", fname);
                _passSettings.Add("jsonFileData", jsonString);

                var editormode = "htmlmixed";
                if (Path.GetExtension(fname) == ".js") editormode = "javascript";
                if (Path.GetExtension(fname) == ".css") editormode = "css";
                _passSettings.Add("editormode", editormode);

                _passSettings.Add("interfacekey", _rocketInterface.InterfaceKey);

                var razorTempl = RenderRazorUtils.GetRazorTemplateData("EditorPopUp.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
                return RenderRazorUtils.RazorObjectRender(razorTempl, appTheme, null, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string SaveEditor()
        {
            var editorcode = _postInfo.GetXmlProperty("genxml/hidden/editorcodesave");
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);

            appTheme.Update();
            appTheme.SaveEditor(filename, editorcode);
            CacheFileUtils.ClearAllCache();
            return "OK";
        }

        public void AddListImage()
        {
            var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
            appTheme.AddListImage();
        }

        public string RebuildResx()
        {
            var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
            var culturecoderesx = _paramInfo.GetXmlProperty("genxml/hidden/culturecoderesx");

            var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            var resxData = new ResxData(appTheme.GetFileMapPath(fname));
            resxData.Rebuild(appTheme);
            return GetResxDetail();
        }
        private void AddResxFile()
        {
            var resxfilename = _postInfo.GetXmlProperty("genxml/textbox/resxfilename");
            if (resxfilename != "")
            {
                var culturecoderesx = _paramInfo.GetXmlProperty("genxml/hidden/culturecoderesx");
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                if (culturecoderesx != "") culturecoderesx = "." + culturecoderesx;

                if (Path.GetExtension(resxfilename) != ".resx") resxfilename = Path.GetFileNameWithoutExtension(resxfilename) + culturecoderesx + ".resx";
                var fileMapPath = appTheme.AppThemeVersionFolderMapPath + "\\resx\\" + resxfilename;
                var resxFileData = "";

                var resxData = new ResxData(fileMapPath);
                if (!resxData.Exists)
                {
                    if (File.Exists(fileMapPath))
                    {
                        resxFileData = FileUtils.ReadFile(fileMapPath);
                    }
                    else
                    {
                        var baserexFileMapPath = appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\resxtemplate.xml";
                        resxFileData = FileUtils.ReadFile(baserexFileMapPath);
                    }
                    FileUtils.SaveFile(fileMapPath, resxFileData);
                }
            }
        }
        private void AddCssFile()
        {
            var cssfilename = _postInfo.GetXmlProperty("genxml/textbox/cssfilename");
            if (cssfilename != "")
            {
                if (Path.GetExtension(cssfilename) != ".css") cssfilename =  Path.GetFileNameWithoutExtension(cssfilename) + ".css";
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var fileMapPath = appTheme.AppThemeVersionFolderMapPath + "\\css\\" + cssfilename;
                var cssFileData = "";
                if (File.Exists(fileMapPath)) cssFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, cssFileData);
            }
        }
        private void AddJsFile()
        {
            var jsfilename = _postInfo.GetXmlProperty("genxml/textbox/jsfilename");
            if (jsfilename != "")
            {
                if (Path.GetExtension(jsfilename) != ".js") jsfilename = Path.GetFileNameWithoutExtension(jsfilename) + ".js";
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var fileMapPath = appTheme.AppThemeVersionFolderMapPath + "\\js\\" + jsfilename;
                var jsFileData = "";
                if (File.Exists(fileMapPath)) jsFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, jsFileData);
            }
        }
        private void AddTemplateFile()
        {
            var templatefilename = _postInfo.GetXmlProperty("genxml/textbox/templatefilename");
            if (templatefilename != "")
            {
                if (Path.GetExtension(templatefilename) != ".cshtml") templatefilename = Path.GetFileNameWithoutExtension(templatefilename) + ".cshtml";
                var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
                var fileMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\" + templatefilename;
                var templateFileData = "";
                if (File.Exists(fileMapPath)) templateFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, templateFileData);
            }
        }
        private string AddListField()
        {
            var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
            appTheme.AddListField();
            return GetEditTemplate(appTheme);
        }
        private string AddSettingListField()
        {
            var appTheme = new AppTheme(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
            appTheme.AddSettingListField();
            return GetEditTemplate(appTheme);
        }
        private void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }
        private string GetEditTemplate(AppTheme appTheme)
        {
            var razorTempl = RenderRazorUtils.GetRazorTemplateData("AppThemeDetails.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            return RenderRazorUtils.RazorDetail(razorTempl, appTheme, _passSettings, null, true);
        }


    }
}
