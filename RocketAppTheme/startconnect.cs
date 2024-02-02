using DNNrocketAPI;
using DNNrocketAPI.Components;
using DNNrocketAPI.Interfaces;
using Rocket.AppThemes.Components;
using RocketPortal.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace DNNrocket.AppThemes
{
    public partial class StartConnect : IProcessCommand
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private string _editLang;
        private SessionParams _sessionParams;
        private string _returnUrl;
        private string _returnUrlCode;
        private AppThemesDOL _dataObject;

        public Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            object jsonOut = null;
            var strOut = "ERROR - Must be SuperUser"; // return ERROR if not matching commands.

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            if (UserUtils.IsSuperUser())
            {

                AssignEditLang();

                switch (paramCmd)
                {
                    case "rocketapptheme_adminpanel":
                        strOut = AdminPanel();
                        break;
                    case "rocketapptheme_getlist":
                        strOut = GetEditList();
                        break;
                    case "rocketapptheme_getappstore":
                        strOut = GetAppStoreList();
                        break;                        
                    case "rocketapptheme_clearcache":
                        ClearServerCacheLists();
                        strOut = GetAppStoreList();
                        break;
                    case "rocketapptheme_downloadallgithub":
                        strOut = DownloadAppThemes();
                        break;
                    default:

                        switch (paramCmd)
                        {                            
                            case "rocketapptheme_getdetail":
                                strOut = GetDetail("AppThemeDetails.cshtml");
                                break;
                            case "rocketapptheme_addimage":
                                strOut = AddListImage();
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
                            case "rocketapptheme_docopy":
                                strOut = DoCopyAppTheme();
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
                            case "rocketapptheme_copyresx":
                                strOut = CopyResxFile();
                                break;                                

                            case "rocketapptheme_getdepdata":
                                strOut = GetDepDetail();
                                break;
                            case "rocketapptheme_adddepdata":
                                strOut = AddDep();
                                break;
                            case "rocketapptheme_savedepdata":
                                strOut = SaveDep();
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


                            case "rocketapptheme_versionjson":
                                strOut = "";
                                var appThemeFolder = _postInfo.GetXmlProperty("genxml/hidden/activevalue");
                                var projectName = _postInfo.GetXmlProperty("genxml/select/selectedprojectname");
                                var appTheme = new AppThemeLimpet(_dataObject.PortalData.PortalId, appThemeFolder, "", projectName);
                                jsonOut = appTheme.VersionListJson();
                                break;
                            case "rocketapptheme_appthemejson":
                                strOut = "";
                                var projectName2 = _postInfo.GetXmlProperty("genxml/hidden/activevalue");
                                var appThemeList2 = new AppThemeDataList(_dataObject.PortalData.PortalId, projectName2, _paramInfo.GetXmlProperty("genxml/hidden/selectedsystemkey"));
                                var addEmpty = _postInfo.GetXmlPropertyBool("genxml/hidden/addempty");
                                jsonOut = appThemeList2.NameListJson(addEmpty);
                                break;                                

                        }

                        break;
                }
            }
            else
            {
                strOut = ReloadPage();
            }

            return DNNrocketUtils.ReturnString(strOut, jsonOut);
        }


        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;

            var portalid = PortalUtils.GetCurrentPortalId();
            if (portalid < 0 && systemInfo.PortalId >= 0) portalid = systemInfo.PortalId;

            _rocketInterface = new RocketInterface(interfaceInfo);
            _sessionParams = new SessionParams(_paramInfo);

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

            _dataObject = new AppThemesDOL(portalid, _sessionParams);

            if (paramCmd == "rocketapptheme_getlist" && !String.IsNullOrEmpty(_dataObject.AppTheme.AppThemeFolder))
            {
                paramCmd = "rocketapptheme_getdetail";
            }

            return paramCmd;
        }
        private string AdminPanel()
        {
            var razorTempl = _dataObject.AppThemeSystem.GetTemplate("AdminPanel.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string DeleteFile()
        {
            var filename = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            if (filename != "")
            {
                var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
                _dataObject.AppTheme.DeleteFile(filename, moduleref);
                _dataObject.AppTheme = new AppThemeLimpet(_dataObject.PortalId, _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
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

                var fileNameMapPath = _dataObject.AppTheme.AppThemeVersionFolderMapPath.Trim('\\') + "\\img\\" + filename;

                File.Delete(fileNameMapPath);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                _dataObject.AppTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
            }
            return GetDetail();
        }

        public string CreateNewVersion()
        {
            try
            {
                if (PortalUtils.GetPortalId() != 0) return "";

                var rtn = _dataObject.AppTheme.CopyVersion(_dataObject.AppTheme.AppVersion, _dataObject.AppTheme.LatestVersion + 1);
                _dataObject.AppTheme.AppVersionFolder = _dataObject.AppTheme.AppVersionFolder.ToString();
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

                _dataObject.AppTheme.DeleteVersion();
                _dataObject.AppTheme.AppVersionFolder = _dataObject.AppTheme.AppVersionFolder;
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

                _dataObject.AppTheme.DeleteTheme();
                ClearServerCacheLists();
                return GetAppStoreList();
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
            CacheUtils.ClearAllCache();
            CacheFileUtils.ClearFileCacheAllPortals();
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

                var appSystemThemeFolderRel = "/DesktopModules/RocketThemes/";
                var appThemeFolderRel = appSystemThemeFolderRel + "/" + newAppThemeName;
                var appThemeFolderMapPath = DNNrocketUtils.MapPath(appThemeFolderRel);

                if (Directory.Exists(appThemeFolderMapPath))
                {
                    return DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/AppThemes/App_LocalResources/", "AppThemes.appthemeexists");
                }

                // crearte new _dataObject.AppTheme.
                var appTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), newAppThemeName, "1.0", _dataObject.AppTheme.ProjectName);

                ClearServerCacheLists();

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String CopyResxFile()
        {
            try
            {
                var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
                var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
                var copylang = _paramInfo.GetXmlProperty("genxml/hidden/copylang");
                var appVersionFolder = _paramInfo.GetXmlProperty("genxml/hidden/appversionfolder");
                var resxData = _dataObject.AppTheme.GetResx(fname, moduleref);
                var newFileName = Path.GetFileNameWithoutExtension(fname) + "." + copylang + Path.GetExtension(fname);
                var fileName = _dataObject.AppTheme.AppThemeFolderPortalMapPath.TrimEnd('\\') + "\\" + appVersionFolder + "\\resx\\" + newFileName;
                FileUtils.SaveFile(fileName, resxData.ResxFileData);
                _dataObject.AppTheme.Populate();
                return GetDetail("AppThemeDetails.cshtml"); 
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
                _dataObject.Settings.Add("filename", fname);
                var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
                var resxData = _dataObject.AppTheme.GetResx(fname, moduleref);
                _dataObject.SetDataObject("resxData", resxData);
                var razorTempl = _dataObject.AppThemeSystem.GetTemplate("ResxPopUp.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
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
                var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
                var resxData = _dataObject.AppTheme.GetResx(fname, moduleref);
                var key = "" + (resxData.DataDictionary.Count + 1).ToString() + ".Text";
                var lp = (resxData.DataDictionary.Count + 1);
                while (resxData.DataDictionary.ContainsKey(key))
                {
                    lp += 1;
                    key = "" + (lp).ToString() + ".Text";
                }
                resxData.AddField(key, "");
                _dataObject.AppTheme.SaveResx(fname, resxData, moduleref);
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
                var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
                var resxData = _dataObject.AppTheme.GetResx(fname, moduleref);
                resxData.RemoveField(key);
                _dataObject.AppTheme.SaveResx(fname, resxData, moduleref);
                return GetResxDetail();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String SaveResxDetail()
        {
            var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            var resxData = _dataObject.AppTheme.GetResx(fname, moduleref);
            resxData.RemoveAllFields();
            var resxlist = _postInfo.GetRecordList("resxdictionarydata");
            foreach (var r in resxlist)
            {
                resxData.AddField(r.GetXmlProperty("genxml/key"), r.GetXmlProperty("genxml/value"));
            }
            _dataObject.AppTheme.SaveResx(fname, resxData, moduleref);
            CacheFileUtils.ClearAllCache(_dataObject.PortalId);
            return GetResxDetail();
        }

        public String GetDepDetail()
        {
            var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            _dataObject.Settings.Add("filename", fname);
            var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            var depData = _dataObject.AppTheme.GetDep(fname, moduleref);
            _dataObject.SetDataObject("depdata", depData);
            _dataObject.Settings.Add("interfacekey", _rocketInterface.InterfaceKey);
            var razorTempl = _dataObject.AppThemeSystem.GetTemplate("DepDetail.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String AddDep()
        {
            var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            _dataObject.AppTheme.AddDep(fname, moduleref);
            return GetDepDetail();
        }
        public String SaveDep()
        {
            var fname = _paramInfo.GetXmlProperty("genxml/hidden/filename");
            var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            _dataObject.AppTheme.SaveDep(fname, _postInfo, moduleref);
            _dataObject.AppTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
            CacheFileUtils.ClearAllCache(_dataObject.PortalId);
            return GetDepDetail();
        }

        public String GetDetail(string templateName = "AppThemeDetails.cshtml")
        {
            try
            {
                return GetEditTemplate(_dataObject.AppTheme, templateName);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetAppStoreList()
        {
            PortalUtils.CreateRocketDirectories(_dataObject.PortalId);

            var appThemeDataList = new AppThemeDataList(_dataObject.PortalData.PortalId, _dataObject.AppTheme.ProjectName);
            var razorTempl = _dataObject.AppThemeSystem.GetTemplate("AppStoreList.cshtml");
            _dataObject.SetDataObject("appthemedatalist", appThemeDataList);

            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String GetEditList()
        {
            PortalUtils.CreateRocketDirectories(_dataObject.PortalId);

            var appThemeDataList = new AppThemeDataList(_dataObject.PortalData.PortalId, _dataObject.AppTheme.ProjectName);
            var razorTempl = _dataObject.AppThemeSystem.GetTemplate("EditList.cshtml");
            _dataObject.SetDataObject("appthemedatalist", appThemeDataList);

            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public string DownloadAppThemes()
        {
            _dataObject.AppThemeProjects.DownloadGitHubProject(_dataObject.AppTheme.ProjectName);
            return GetEditList();
        }
        private Dictionary<string, object> ExportAppTheme()
        {
            var appThemeFolder = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/appthemefolder"));
            if (appThemeFolder == "") appThemeFolder = _dataObject.AppTheme.AppThemeFolder;
            var appVersionFolder = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/appversionfolder"));
            if (appVersionFolder == "") appVersionFolder = _dataObject.AppTheme.AppVersionFolder;
            var appTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), appThemeFolder, appVersionFolder, _dataObject.AppTheme.ProjectName);

            var exportZipMapPath = appTheme.ExportZipFile(_dataObject.PortalData.PortalId, _dataObject.ModuleRef);

            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", exportZipMapPath);
            rtnDic.Add("downloadname", appTheme.AppThemeFolder + ".zip");

            return rtnDic;
        }
        public string DoCopyAppTheme()
        {
            try
            {
                if (PortalUtils.GetPortalId() != 0) return "";

                var appthemeprefix = FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemeprefix"));
                var appthemename = FileUtils.RemoveInvalidFileChars(_postInfo.GetXmlProperty("genxml/textbox/appthemename"));
                var newAppThemeName = appthemename;
                if (appthemeprefix == "") appthemeprefix = _dataObject.SystemKey;
                newAppThemeName = appthemeprefix.Replace(".", "") + "." + newAppThemeName.Replace(".","");
                var newAppThemeDirName = _dataObject.AppTheme.AppThemeFolderMapPath.TrimEnd('\\') + "\\..\\" + newAppThemeName;

                _dataObject.AppTheme.Copy(newAppThemeDirName);
                _dataObject.AppTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), newAppThemeName, _dataObject.AppTheme.LatestVersionFolder, _dataObject.AppTheme.ProjectName);
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
                var jsonString = GeneralUtils.EnCode(_dataObject.AppTheme.GetTemplate(fname, _dataObject.ModuleRef));
                _dataObject.Settings.Add("filename", fname);
                _dataObject.Settings.Add("jsonFileData", jsonString);


                var editormode = "htmlmixed";
                if (Path.GetExtension(fname) == ".js") editormode = "javascript";
                if (Path.GetExtension(fname) == ".css") editormode = "css";
                _dataObject.Settings.Add("editormode", editormode);

                _dataObject.Settings.Add("interfacekey", _rocketInterface.InterfaceKey);

                var razorTempl = _dataObject.AppThemeSystem.GetTemplate("EditorPopUp.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
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
            var moduleref = _paramInfo.GetXmlProperty("genxml/hidden/moduleref");
            _dataObject.AppTheme.SaveEditor(filename, editorcode, moduleref);
            CacheFileUtils.ClearAllCache(_dataObject.PortalId);
            _dataObject.AppTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
            return GetEditorDetail();
        }

        public string AddListImage()
        {
            ImgUtils.MoveImageToFolder(_postInfo, _dataObject.AppTheme.ImageFolderMapPath);
            _dataObject.AppTheme.Populate();
            return GetDetail();
        }
        private void AddCssFile()
        {
            var cssfilename = _postInfo.GetXmlProperty("genxml/textbox/cssfilename");
            if (cssfilename != "")
            {
                if (Path.GetExtension(cssfilename) != ".css") cssfilename =  Path.GetFileNameWithoutExtension(cssfilename) + ".css";
                var fileMapPath = _dataObject.AppTheme.AppThemeVersionFolderMapPath + "\\css\\" + cssfilename;
                var cssFileData = "";
                if (File.Exists(fileMapPath)) cssFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, cssFileData);
                _dataObject.AppTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
            }
        }
        private void AddJsFile()
        {
            var jsfilename = _postInfo.GetXmlProperty("genxml/textbox/jsfilename");
            if (jsfilename != "")
            {
                if (Path.GetExtension(jsfilename) != ".js") jsfilename = Path.GetFileNameWithoutExtension(jsfilename) + ".js";
                var fileMapPath = _dataObject.AppTheme.AppThemeVersionFolderMapPath + "\\js\\" + jsfilename;
                var jsFileData = "";
                if (File.Exists(fileMapPath)) jsFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, jsFileData);
                _dataObject.AppTheme = new AppThemeLimpet(PortalUtils.GetCurrentPortalId(), _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
            }
        }
        private void AddTemplateFile()
        {
            var templatefilename = _postInfo.GetXmlProperty("genxml/textbox/templatefilename");
            if (templatefilename != "")
            {
                if (Path.GetExtension(templatefilename) != ".cshtml") templatefilename = Path.GetFileNameWithoutExtension(templatefilename) + ".cshtml";
                var fileMapPath = _dataObject.AppTheme.AppThemeVersionFolderMapPath + "\\default\\" + templatefilename;
                var templateFileData = "";
                if (File.Exists(fileMapPath)) templateFileData = FileUtils.ReadFile(fileMapPath);
                FileUtils.SaveFile(fileMapPath, templateFileData);
                _dataObject.AppTheme = new AppThemeLimpet(_dataObject.PortalData.PortalId, _dataObject.AppTheme.AppThemeFolder, _dataObject.AppTheme.AppVersionFolder, _dataObject.AppTheme.ProjectName);
            }
        }
        private void AssignEditLang()
        {
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);
        }
        private string GetEditTemplate(AppThemeLimpet appTheme, string templateName)
        {
            var razorTempl = _dataObject.AppThemeSystem.GetTemplate(templateName);
            var rtn = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
            if (rtn.StatusCode != "00") return rtn.ErrorMsg;
            return rtn.RenderedText;
        }
        private string ReloadPage()
        {
            try
            {
                // user does not have access, logoff
                UserUtils.SignOut();

                var razorTempl = _dataObject.AppThemeSystem.GetTemplate("Reload.cshtml");
                var rtn = RenderRazorUtils.RazorProcessData(razorTempl, _dataObject.DataObjects, _dataObject.Settings, _sessionParams, true);
                return rtn.RenderedText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
