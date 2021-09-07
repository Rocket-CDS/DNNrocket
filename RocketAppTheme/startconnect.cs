﻿using DNNrocketAPI;
using DNNrocketAPI.Components;
using Rocket.AppThemes.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
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
        private string _appThemeSystemKey;
        private string _appThemeFolder;
        private string _appVersionFolder;
        private AppThemeLimpet _appTheme;
        private AppThemeDNNrocketLimpet _appThemeSystem;
        private const string _systemKey = "rocketapptheme";
        private SessionParams _sessionParams;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = "ERROR - Must be SuperUser"; // return ERROR if not matching commands.

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            if (UserUtils.IsSuperUser()  || UserUtils.IsAdministrator()) // Admin should ONLY be allow to change the portal/module templates
            {
                AssignEditLang();

                switch (paramCmd)
                {
                    case "rocketapptheme_getlist":
                        strOut = GetList();
                        break;
                    case "rocketapptheme_getappstore":
                        strOut = GetAppStoreList();
                        break;                        
                    case "rocketapptheme_clearcache":
                        ClearServerCacheLists();
                        strOut = GetList();
                        break;
                    case "rocketapptheme_downloadallgithub":
                        AppThemeUtils.DownloadAllGitHubAppTheme("ToastedTemplates");
                        strOut = GetAppStoreList();
                        break;
                    default:

                        // actions on selected AppTheme
                        _appThemeSystemKey = _paramInfo.GetXmlProperty("genxml/hidden/appthemesystemkey");
                        _appThemeFolder = _paramInfo.GetXmlProperty("genxml/hidden/appthemefolder");
                        _appVersionFolder = _paramInfo.GetXmlProperty("genxml/hidden/appversionfolder");

                        _appTheme = new AppThemeLimpet(_appThemeFolder, _appVersionFolder);

                        switch (paramCmd)
                        {
                            case "rocketapptheme_getdetail":
                                strOut = GetDetail();
                                break;
                            case "rocketapptheme_addimage":
                                strOut = AddListImage();
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
                            case "rocketapptheme_createversion":
                                strOut = CreateNewVersion();
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

                            case "rocketapptheme_deletefile":
                                strOut = DeleteFile();
                                break;
                            case "rocketapptheme_deleteimagefile":
                                strOut = DeleteImageFile();
                                break;

                            case "rocketapptheme_downloadgithub":
                                AppThemeUtils.DownloadGitHubAppTheme(_paramInfo.GetXmlProperty("genxml/hidden/htmlurl"), _appTheme.AppThemeFolderMapPath);
                                strOut = GetAppStoreList();
                                break;

                        }

                        break;
                }
            }
            else
            {
                strOut = UserUtils.LoginForm(_systemKey, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }

            return DNNrocketUtils.ReturnString(strOut);
        }


        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _passSettings = new Dictionary<string, string>();
            _systemData = new SystemLimpet(_systemKey);
            _appThemeSystem = new AppThemeDNNrocketLimpet(_systemData.SystemKey);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _sessionParams = new SessionParams(_paramInfo);

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

            return paramCmd;
        }

        public string DeleteFile()
        {
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            if (filename != "")
            {
                var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
                _appTheme.DeleteFile(filename, moduleref);
                _appTheme = new AppThemeLimpet(_appThemeFolder, _appVersionFolder);
            }
            return GetDetail();
        }
        public string DeleteImageFile()
        {
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            if (filename != "")
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                var fileNameMapPath = _appTheme.AppThemeVersionFolderMapPath.Trim('\\') + "\\img\\" + filename;

                File.Delete(fileNameMapPath);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                _appTheme = new AppThemeLimpet(_appThemeFolder, _appVersionFolder);
            }
            return GetDetail();
        }

        public string CreateNewVersion()
        {
            try
            {
                if (PortalUtils.GetPortalId() != 0) return "";

                var rtn = _appTheme.CopyVersion(_appTheme.AppVersion, _appTheme.LatestVersion + 1);
                _appVersionFolder = _appTheme.AppVersionFolder.ToString();
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
                if (PortalUtils.GetPortalId() != 0) return "";

                _appTheme.DeleteVersion();
                _appVersionFolder = _appTheme.AppVersionFolder;
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
                if (PortalUtils.GetPortalId() != 0) return "";

                _appTheme.DeleteTheme();
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
                if (PortalUtils.GetPortalId() != 0) return "";

                var appthemeprefix = GeneralUtils.AlphaNumeric(FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemeprefix")));
                var appthemename = GeneralUtils.AlphaNumeric(FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemename")));

                var newAppThemeName = appthemename;
                if (appthemeprefix != "") newAppThemeName = appthemeprefix + "_" + newAppThemeName;

                var appSystemThemeFolderRel = "/DesktopModules/RocketThemes/" + _appThemeSystemKey;
                var appThemeFolderRel = appSystemThemeFolderRel + "/" + newAppThemeName;
                var appThemeFolderMapPath = DNNrocketUtils.MapPath(appThemeFolderRel);

                if (Directory.Exists(appThemeFolderMapPath))
                {
                    return DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/AppThemes/App_LocalResources/", "AppThemes.appthemeexists");
                }

                // crearte new _appTheme.
                var appTheme = new AppThemeLimpet(newAppThemeName, "1.0");

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
                var resxData = new ResxData(_appTheme.GetFileMapPath(fname));
                var dataObjects = new Dictionary<string, object>();
                dataObjects.Add("resxData", resxData);
                var razorTempl = _appThemeSystem.GetTemplate("ResxPopUp.cshtml");
                return RenderRazorUtils.RazorObjectRender(razorTempl, _appTheme, dataObjects, _passSettings, null, true);
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
                var resxData = new ResxData(_appTheme.GetFileMapPath(fname));

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
                var resxData = new ResxData(_appTheme.GetFileMapPath(fname));
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
                var resxData = new ResxData(_appTheme.GetFileMapPath(fname));
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
                return GetEditTemplate(_appTheme);
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
                var appThemeDataList = new AppThemeDataList();
                var razorTempl = _appThemeSystem.GetTemplate("appthemelist.cshtml");
                var passSettings = _postInfo.ToDictionary();
                return RenderRazorUtils.RazorDetail(razorTempl, appThemeDataList, passSettings,null,true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }
        public String GetAppStoreList()
        {
            try
            {
                var appThemeDataList = new AppThemeDataList();
                var razorTempl = _appThemeSystem.GetTemplate("AppStoreList.cshtml");
                var passSettings = _postInfo.ToDictionary();

                return RenderRazorUtils.RazorDetail(razorTempl, appThemeDataList, passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }

        private void DownloadPublicAppTheme(string appThemeFolder = "")
        {
            // [TODO: look at downloading github files]
            //if (appThemeFolder == "") appThemeFolder = _appThemeFolder;
            //var httpConnect = new HttpConnect(_selectedSystemKey);
            //var userid = UserUtils.GetCurrentUserId();
            //var destinationMapPath = PortalUtils.TempDirectoryMapPath() + "\\" + userid + "_" + appThemeFolder + ".zip";
            //httpConnect.DownloadAppThemeToFile(appThemeFolder, destinationMapPath);
            //var appTheme = new AppThemeLimpet(_selectedSystemKey, destinationMapPath, true);
            //_appTheme.Update();
            //File.Delete(destinationMapPath);
            //ClearServerCacheLists();
        }

        private Dictionary<string, object> ExportAppTheme()
        {
            var appThemeFolder = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/appthemefolder"));
            if (appThemeFolder == "") appThemeFolder = _appThemeFolder;
            var appVersionFolder = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/appversionfolder"));
            if (appVersionFolder == "") appVersionFolder = _appVersionFolder;
            var appTheme = new AppThemeLimpet(appThemeFolder, appVersionFolder);

            var exportZipMapPath = appTheme.ExportZipFile();

            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", exportZipMapPath);
            rtnDic.Add("downloadname", appTheme.AppThemeFolder + ".zip");

            return rtnDic;
        }
        private string CheckServerVersion()
        {
            //[TODO: Github?]

            //var rtnDic = ExportAppTheme();
            //if (rtnDic["filenamepath"] != null && (string)rtnDic["filenamepath"] != "")
            //{
            //    var appTheme = new AppThemeLimpet(_selectedSystemKey, _appThemeFolder, _appVersionFolder);
            //    var ftpConnect = new FtpConnect(_selectedSystemKey);

            //    var serverxml = ftpConnect.DownloadAppThemeXml(_appThemeFolder);
            //    double version = -1;
            //    double rev = -1;
            //    var sRec = new SimplisityRecord();
            //    if (!String.IsNullOrEmpty(serverxml) && serverxml.ToUpper() != "FAIL")
            //    {
            //        sRec.XMLData = serverxml;
            //        version = sRec.GetXmlPropertyDouble("item/genxml/hidden/latestversion");
            //        rev = sRec.GetXmlPropertyDouble("item/genxml/hidden/latestrev");
            //    }
            //    if (appTheme.LatestVersion > version)
            //    {
            //        return UploadAppTheme();
            //    }
            //    if (appTheme.LatestVersion == version)
            //    {
            //        var razorTempl2 = RenderRazorUtils.GetSystemRazorTemplate(_systemData.SystemKey, "versioncheckequal.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            //        return RenderRazorUtils.RazorDetail(razorTempl2, new SimplisityInfo(sRec), _passSettings, null, true);
            //    }
            //    var razorTempl1 = RenderRazorUtils.GetSystemRazorTemplate(_systemData.SystemKey, "versioncheck.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            //    return RenderRazorUtils.RazorDetail(razorTempl1, new SimplisityInfo(sRec), _passSettings, null, true);
            //}
            //var razorTempl = RenderRazorUtils.GetSystemRazorTemplate(_systemData.SystemKey, "ftpfail.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), _rocketInterface.ThemeVersion, true);
            //return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            return "TODO - Check version in GitHub";
        }

        private string ImportAppTheme()
        {
            //[TODO: work out import of zip]

            //var fileuploadlist = _paramInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            //if (fileuploadlist != "")
            //{
            //    foreach (var f in fileuploadlist.Split(';'))
            //    {
            //        if (f != "")
            //        {
            //            var userid = UserUtils.GetCurrentUserId();
            //            var userFolder = PortalUtils.TempDirectoryMapPath();
            //            var friendlyname = GeneralUtils.DeCode(f);
            //            var fname = userFolder + "\\" + userid + "_" + friendlyname;
            //            var _appTheme = new AppThemeLimpet(_selectedSystemKey, fname, true);
            //            // delete import file
            //            File.Delete(fname);
            //        }
            //    }
            //    ClearServerCacheLists();
            //}
            return GetList();
        }

        public string DoCopyAppTheme()
        {
            try
            {
                if (PortalUtils.GetPortalId() != 0) return "";

                var appthemeprefix = FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemeprefix"));
                var appthemename = FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemename"));
                var newAppThemeName = appthemename;
                if (appthemeprefix == "") appthemeprefix = _systemData.SystemKey;
                newAppThemeName = appthemeprefix.Replace(".", "") + "." + newAppThemeName.Replace(".","");
                var newAppThemeDirName = _appTheme.AppThemeFolderMapPath.TrimEnd('\\') + "\\..\\" + newAppThemeName;

                _appTheme.Copy(newAppThemeDirName);
                _appTheme = new AppThemeLimpet(newAppThemeName, _appTheme.LatestVersionFolder);
                ClearServerCacheLists();
                return GetDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetEditorDetail()
        {
            try
            {
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var jsonString = GeneralUtils.EnCode(_appTheme.GetTemplate(fname));
                _passSettings.Add("filename", fname);
                _passSettings.Add("jsonFileData", jsonString);


                var editormode = "htmlmixed";
                if (Path.GetExtension(fname) == ".js") editormode = "javascript";
                if (Path.GetExtension(fname) == ".css") editormode = "css";
                _passSettings.Add("editormode", editormode);

                _passSettings.Add("interfacekey", _rocketInterface.InterfaceKey);

                var razorTempl = _appThemeSystem.GetTemplate("EditorPopUp.cshtml");
                return RenderRazorUtils.RazorObjectRender(razorTempl, _appTheme, null, _passSettings, new SessionParams(_paramInfo), true);
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
            _appTheme.SaveEditor(filename, editorcode);
            CacheFileUtils.ClearAllCache();
            _appTheme = new AppThemeLimpet(_appTheme.AppThemeFolder, _appTheme.AppVersionFolder);
            return GetDetail();
        }

        public string AddListImage()
        {
            ImgUtils.MoveImageToFolder(_postInfo, _appTheme.ImageFolderMapPath);
            _appTheme.Populate();
            return GetDetail();
        }

        private void AddResxFile()
        {
            var resxfilename = _postInfo.GetXmlProperty("genxml/textbox/resxfilename");
            if (resxfilename != "")
            {
                var culturecoderesx = _paramInfo.GetXmlProperty("genxml/hidden/culturecoderesx");
                if (culturecoderesx != "") culturecoderesx = "." + culturecoderesx;

                if (Path.GetExtension(resxfilename) != ".resx") resxfilename = Path.GetFileNameWithoutExtension(resxfilename) + culturecoderesx + ".resx";
                var fileMapPath = _appTheme.AppThemeVersionFolderMapPath + "\\resx\\" + resxfilename;
                var resxFileData = "";

                var resxData = new ResxData(fileMapPath);
                if (!resxData.Exists)
                {
                    if (File.Exists(fileMapPath)) resxFileData = FileUtils.ReadFile(fileMapPath);
                    FileUtils.SaveFile(fileMapPath, resxFileData);
                    _appTheme = new AppThemeLimpet(_appThemeFolder, _appTheme.AppVersionFolder);
                }
            }
        }
        private void AddCssFile()
        {
            var cssfilename = _postInfo.GetXmlProperty("genxml/textbox/cssfilename");
            if (cssfilename != "")
            {
                if (Path.GetExtension(cssfilename) != ".css") cssfilename =  Path.GetFileNameWithoutExtension(cssfilename) + ".css";
                var fileMapPath = _appTheme.AppThemeVersionFolderMapPath + "\\css\\" + cssfilename;
                var cssFileData = "";
                if (File.Exists(fileMapPath)) cssFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, cssFileData);
                _appTheme = new AppThemeLimpet(_appThemeFolder, _appTheme.AppVersionFolder);
            }
        }
        private void AddJsFile()
        {
            var jsfilename = _postInfo.GetXmlProperty("genxml/textbox/jsfilename");
            if (jsfilename != "")
            {
                if (Path.GetExtension(jsfilename) != ".js") jsfilename = Path.GetFileNameWithoutExtension(jsfilename) + ".js";
                var fileMapPath = _appTheme.AppThemeVersionFolderMapPath + "\\js\\" + jsfilename;
                var jsFileData = "";
                if (File.Exists(fileMapPath)) jsFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, jsFileData);
                _appTheme = new AppThemeLimpet(_appThemeFolder, _appTheme.AppVersionFolder);
            }
        }
        private void AddTemplateFile()
        {
            var templatefilename = _postInfo.GetXmlProperty("genxml/textbox/templatefilename");
            if (templatefilename != "")
            {
                if (Path.GetExtension(templatefilename) != ".cshtml") templatefilename = Path.GetFileNameWithoutExtension(templatefilename) + ".cshtml";
                var fileMapPath = _appTheme.AppThemeVersionFolderMapPath + "\\default\\" + templatefilename;
                var templateFileData = "";
                if (File.Exists(fileMapPath)) templateFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, templateFileData);
                _appTheme = new AppThemeLimpet(_appThemeFolder, _appTheme.AppVersionFolder);
            }
        }
        private void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }
        private string GetEditTemplate(AppThemeLimpet appTheme)
        {
            var razorTempl = _appThemeSystem.GetTemplate("AppThemeDetails.cshtml");
            return RenderRazorUtils.RazorDetail(razorTempl, appTheme, _passSettings, new SessionParams(_paramInfo), true);
        }


    }
}