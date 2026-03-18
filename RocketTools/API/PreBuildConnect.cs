using Dnn.ExportImport.Components.Controllers;
using Dnn.ExportImport.Components.Entities;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using Rocket.AppThemes.Components;
using RocketPortal.Components;
using RocketTools.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace RocketTools.API
{
    public partial class StartConnect
    {
        public string ExportPreBuild()
        {
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\PreBuilds";
            foreach (var f in Directory.GetFiles(exportZipMapPath))
            {
                _passSettings.Add("exportfilename", Path.GetFileName(f));
            }

            var razorTempl = _appThemeTools.GetTemplate("prebuildexport.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, _dataObjects, _passSettings, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ExportPreBuildComplete()
        {
            var jobId = _paramInfo.GetXmlPropertyInt("genxml/hidden/jobid");
            var helper = new DnnSiteExportImportHelper();
            var job = helper.GetJobStatus(jobId);
            if (job == null) return "ERROR: Job not found " + jobId;

            var exportDataFolder = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + job.Directory);
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\PreBuilds";
            if (!Directory.Exists(exportZipMapPath)) Directory.CreateDirectory(exportZipMapPath);
            foreach (var f in Directory.GetFiles(exportZipMapPath))
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            exportZipMapPath += "\\prebuild" + _portalData.PortalId + "_" + GeneralUtils.SanitizeFileName(_portalData.Name.Trim().Replace(".", "_").Replace(" ", "_"));

            if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
            if (Directory.Exists(exportDataFolder))
            {
                ZipFile.CreateFromDirectory(exportDataFolder, exportZipMapPath);
                Directory.Delete(exportDataFolder, true);
                _passSettings.Add("exportprebuild", "true");
                _passSettings.Add("exportfilename", Path.GetFileName(exportZipMapPath));
            }

            var razorTempl = _appThemeTools.GetTemplate("prebuildexportcomplete.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, _dataObjects, _passSettings, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ExportPreBuildProcess()
        {
            var razorTempl = _appThemeTools.GetTemplate("prebuildexportprocess.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, _dataObjects, _passSettings, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ExportPreBuildExe()
        {
            try
            {
                if (!UserUtils.IsAdministrator()) return "Must be Administrator.";

                var extraExportSettings = new SimplisityRecord();
                extraExportSettings.SetXmlProperty("genxml/hometab/hometabid", PortalUtils.GetCurrentPortalSettings().HomeTabId.ToString());

                // Start the export but DON'T wait.
                var helper = new DnnSiteExportImportHelper();
                var jobId = helper.ExportWebsite(_portalData.PortalId, UserUtils.GetCurrentUserId(), "Rocket PreBuild Export", "Full website backup");

                var entitiesController = EntitiesController.Instance;
                var job = entitiesController.GetJobById(jobId);
                if (job == null) return "ERROR: No Export JobId found";

                ExportExtraData(_portalData.PortalId, job.Directory, extraExportSettings);

                // Save info for later
                var jobInfo = new SimplisityInfo();
                jobInfo.SetXmlProperty("genxml/jobid", jobId.ToString());
                jobInfo.SetXmlProperty("genxml/extrasettings", extraExportSettings.XMLData);
                DNNrocketUtils.SetTempStorage(jobInfo, "ExportJob_" + _portalData.PortalId, 1);

                // Return message immediately
                _passSettings.Add("exportstarted", "true");
                _passSettings.Add("jobid", jobId.ToString());
                
                return ExportPreBuildProcess();
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return ex.ToString();
            }
        }
        public string CheckExportStatus()
        {
            try
            {
                var jobId = _paramInfo.GetXmlPropertyInt("genxml/hidden/jobid");
                var helper = new DnnSiteExportImportHelper();
                var job = helper.GetJobStatus(jobId);
                if (job == null) return "ERROR|Job not found";

                // Use ToString() - works without enum reference
                var statusString = job.JobStatus.ToString();

                if (statusString == "Successful")
                {
                    return "true";
                }
                else if (statusString == "Failed")
                {
                    return "false: " + statusString;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return "false: " + ex.ToString();
            }
        }
        public void ExportExtraData(int portalId, string exportImportJobDirectory, SimplisityRecord extraExportSettings)
        {
            if (extraExportSettings != null)
            {
                // Export HomeTab
                var homeTabId = extraExportSettings.GetXmlPropertyInt("genxml/hometab/hometabid");
                if (homeTabId > 0)
                {
                    var homeTabInfo = DNNrocketUtils.GetTabInfo(homeTabId, true);
                    if (homeTabInfo != null)
                    {
                        extraExportSettings.SetXmlProperty("genxml/hometab/name", homeTabInfo.TabName);
                        extraExportSettings.SetXmlProperty("genxml/hometab/description", homeTabInfo.Description);
                        extraExportSettings.SetXmlProperty("genxml/hometab/title", homeTabInfo.Title);
                    }
                }

                // Export All PortalSettings.
                var objCtrl = new DNNrocketController();
                var sqlCmd = "select * from {databaseOwner}[{objectQualifier}PortalSettings] where portalid = " + portalId + " for xml raw";
                var portalSettingsList = objCtrl.ExecSqlXmlList(sqlCmd);
                foreach (var ps in portalSettingsList)
                {
                    extraExportSettings.AddRecordListItem("portalsettings", ps);
                }
                var sqlCmd2 = "select * from {databaseOwner}[{objectQualifier}PortalLocalization] where portalid = " + portalId + " for xml raw";
                var portalLocalizationList = objCtrl.ExecSqlXmlList(sqlCmd2);
                foreach (var ps in portalLocalizationList)
                {
                    extraExportSettings.AddRecordListItem("portallocalization", ps);
                }
                var sqlCmd3 = "select XMLData from {databaseOwner}[{objectQualifier}DNNrocket] where guidkey = 'APPTHEMEPROJECTS'";
                var appThemeXmlData = objCtrl.ExecSql(sqlCmd3);
                if (!String.IsNullOrEmpty(appThemeXmlData))
                {
                    var sRec = new SimplisityRecord();
                    sRec.XMLData = appThemeXmlData;
                    foreach (var at in sRec.GetRecordList("appthemeprojects"))
                    {
                        if (at.GetXmlProperty("genxml/textbox/githubtoken") == "") // do not export seucirty tokens
                        {
                            extraExportSettings.AddRecordListItem("appthemeprojects", at);
                        }
                    }
                }
                var sqlCmd4 = "select TabId, TabPath FROM {databaseOwner}[{objectQualifier}Tabs] where portalid = " + portalId + " for xml raw";
                var tabinfoList = objCtrl.ExecSqlXmlList(sqlCmd4);
                foreach (var at in tabinfoList)
                {
                    extraExportSettings.AddRecordListItem("tabpath", at);
                }

                // Export Directory systems.
                var systemListData = new SystemLimpetList();
                foreach (var systemData in systemListData.GetSystemActiveList())
                {
                    if (systemData.Exists && systemData.BaseSystemKey.ToLower() == "rocketdirectoryapi")
                    {
                        foreach (var rocketInterface in systemData.ProviderList)
                        {
                            if (rocketInterface.IsProvider("exportmodule"))
                            {
                                if (rocketInterface.Exists)
                                {
                                    var postInfo = new SimplisityInfo();
                                    var paramInfo = new SimplisityInfo();
                                    try
                                    {
                                        var exportZipMapPath = "";
                                        paramInfo.SetXmlPropertyInt("genxml/hidden/portalid", portalId);
                                        paramInfo.SetXmlProperty("genxml/hidden/systemkey", systemData.SystemKey);

                                        var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketdirectoryapi_exportdata", systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "", "");
                                        if (returnDictionary.ContainsKey("outputhtml")) exportZipMapPath = (string)returnDictionary["outputhtml"];

                                        if (!string.IsNullOrEmpty(exportZipMapPath) && File.Exists(exportZipMapPath))
                                        {
                                            // Create a temporary folder for extraction
                                            var tempExtractFolder = Path.Combine(PortalUtils.TempDirectoryMapPath(), "rocketdirectoryapi_exportdata");
                                            if (Directory.Exists(tempExtractFolder)) Directory.Delete(tempExtractFolder, true);
                                            Directory.CreateDirectory(tempExtractFolder);

                                            try
                                            {
                                                // Extract the zip file to temp folder
                                                ZipFile.ExtractToDirectory(exportZipMapPath, tempExtractFolder);

                                                // Get the target export directory
                                                var exportDirectory = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJobDirectory);

                                                // Copy all files from temp folder to export directory
                                                foreach (var file in Directory.GetFiles(tempExtractFolder, "*.*", SearchOption.AllDirectories))
                                                {
                                                    var relativePath = file.Substring(tempExtractFolder.Length + 1);
                                                    var targetFile = Path.Combine(exportDirectory, relativePath);

                                                    // Create subdirectories if needed
                                                    var targetDir = Path.GetDirectoryName(targetFile);
                                                    if (!Directory.Exists(targetDir))
                                                    {
                                                        Directory.CreateDirectory(targetDir);
                                                    }

                                                    // Copy the file
                                                    File.Copy(file, targetFile, true);
                                                }
                                            }
                                            finally
                                            {
                                                // Clean up temp folder
                                                if (Directory.Exists(tempExtractFolder))
                                                {
                                                    try
                                                    {
                                                        Directory.Delete(tempExtractFolder, true);
                                                    }
                                                    catch (Exception cleanupEx)
                                                    {
                                                        // Log but don't fail if cleanup fails
                                                        LogUtils.LogException(cleanupEx);
                                                    }
                                                }
                                            }
                                        }
                                        // Do TabPath (Directory Systems)
                                        var detailTabIdSQL = "select XMLData.value('(genxml/detailpage)[1]','nvarchar(max)') from {databaseOwner}[{objectQualifier}RocketDirectoryAPI] where portalid = " + portalId + " and TypeCode = '" + systemData.SystemKey + "PortalCatalog'";
                                        var detailTabId = objCtrl.ExecSql(detailTabIdSQL);
                                        if (GeneralUtils.IsNumeric(detailTabId))
                                        {
                                            var detailTabInfo = DNNrocketUtils.GetTabInfo(Convert.ToInt32(detailTabId), true);
                                            if (detailTabInfo != null) extraExportSettings.SetXmlProperty("genxml/" + systemData.SystemKey + "/detailtabpath", detailTabInfo.TabPath);
                                        }
                                        var listTabIdSQL = "select XMLData.value('(genxml/listpage)[1]','nvarchar(max)') from {databaseOwner}[{objectQualifier}RocketDirectoryAPI] where portalid = " + portalId + " and TypeCode = '" + systemData.SystemKey + "PortalCatalog'";
                                        var listTabId = objCtrl.ExecSql(listTabIdSQL);
                                        if (GeneralUtils.IsNumeric(listTabId))
                                        {
                                            var listTabInfo = DNNrocketUtils.GetTabInfo(Convert.ToInt32(listTabId), true);
                                            if (listTabInfo != null) extraExportSettings.SetXmlProperty("genxml/" + systemData.SystemKey + "/listtabpath", listTabInfo.TabPath);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogUtils.LogException(ex);
                                    }
                                }
                            }
                        }
                    }


                }

                var fName = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJobDirectory + "/RocketSettings.xml");
                extraExportSettings.XMLDoc.Save(fName);

                Thread.Sleep(2000);  // wait for zip to register.
            }

        }

        public string ImportPreBuild()
        {
            var razorTempl = _appThemeTools.GetTemplate("prebuildimport.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, _dataObjects, _passSettings, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ImportPreBuildExe()
        {
            if (!UserUtils.IsAdministrator()) return "Must be Administrator.";
            var importfileupload = _postInfo.GetXmlProperty("genxml/hidden/importfileupload");
            if (importfileupload != "" && Path.GetExtension(importfileupload).ToLower() == ".zip")
            {
                var importFile = PortalUtils.TempDirectoryMapPath() + "\\" + UserUtils.GetCurrentUserId() + "_" + Path.GetFileNameWithoutExtension(importfileupload);
                if (!File.Exists(importFile)) return ImportPreBuild();

                // Remove Workflow, solve DNN bug
                var sqlcmd = "DELETE ht FROM {databaseOwner}[{objectQualifier}HtmlText] ht INNER JOIN {databaseOwner}[{objectQualifier}Modules] m ON ht.ModuleID = m.ModuleID WHERE m.PortalID = " + _portalData.PortalId;
                _objCtrl.ExecSql(sqlcmd);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}ContentWorkflows] where PortalID = " + _portalData.PortalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}DNNrocket] where PortalID = " + _portalData.PortalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}RocketDirectoryAPI] where PortalID = " + _portalData.PortalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}RocketContentAPI] where PortalID = " + _portalData.PortalId);

                // Clear system file
                var systemDataMapPath = PortalUtils.HomeDNNrocketDirectoryMapPath(_portalData.PortalId);
                foreach (var f in Directory.GetFiles(systemDataMapPath))
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogSystem("WARN on import: " + ex.ToString());
                    }
                }
                foreach (var d in Directory.GetDirectories(systemDataMapPath))
                {
                    try
                    {
                        Directory.Delete(d, true);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogSystem("WARN on import: " + ex.ToString());
                    }
                }

                PortalUtils.ClearPortalContent(_portalData.PortalId);
                if (DNNrocketUtils.ImportWebsite(_portalData.PortalId, importFile))
                {
                    _passSettings.Add("importprebuild", "true");
                }
                else
                {
                    _passSettings.Add("importprebuilderr", "ERROR: Import Failed.");
                }
                File.Delete(importFile);

                // Check portal alias, in case extra language added.
                var currentUrl = _sessionParams.Get("requesturl");
                if (GeneralUtils.IsAbsoluteUrl(currentUrl))
                {
                    var domain = PortalUtils.GetDomainFromUrl(currentUrl);
                    var defaultUrl = PortalUtils.DefaultPortalAlias(_portalData.PortalId);
                    var cultureCodeList = DNNrocketUtils.GetCultureCodeList(_portalData.PortalId).ToList();
                    var defaultCultureCode = _portalData.DefaultLanguage();
                    var aliasList = PortalUtils.GetPortalAliases(_portalData.PortalId);

                    foreach (var pa in aliasList)
                    {
                        if (pa != defaultUrl) PortalUtils.DeletePortalAlias(_portalData.PortalId, pa);
                    }                   
                    
                    if (cultureCodeList.Count == 1)
                    {
                        // Only one language - assign it to the root domain
                        PortalUtils.AddPortalAlias(_portalData.PortalId, domain, cultureCodeList[0]);
                    }
                    else if (cultureCodeList.Count > 1)
                    {
                        // Multiple languages - root domain gets default language
                        PortalUtils.AddPortalAlias(_portalData.PortalId, domain, defaultCultureCode);
                        
                        // Add other languages with culture code in URL path
                        foreach (var lang in cultureCodeList)
                        {
                            var aliasUrl = domain + "/" + lang.ToLower();
                            PortalUtils.AddPortalAlias(_portalData.PortalId, aliasUrl, lang);
                        }
                    }
                    // ensure we have a portalalias
                    aliasList = PortalUtils.GetPortalAliases(_portalData.PortalId);
                    if (aliasList.Count == 0) PortalUtils.AddPortalAlias(_portalData.PortalId, domain, "");
                }

                // Download public Apptheme
                var appThemeProjectData = new AppThemeProjectLimpet();
                foreach (var at in appThemeProjectData.List)
                {
                    try
                    {
                        appThemeProjectData.DownloadGitHubProject(at.GetXmlProperty("genxml/textbox/name"));
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                CacheFileUtils.ClearAllCache(_portalData.PortalId);
            }
            return ImportPreBuild();
        }

        public Dictionary<string, object> DownloadPrebuild()
        {
            var docKey = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/dockey"));
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\PreBuilds";
            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", exportZipMapPath + "\\" + docKey);
            rtnDic.Add("downloadname", docKey + ".zip");
            return rtnDic;
        }
        public string SaveWebConfig()
        {
            if (UserUtils.IsSuperUser())
            {
                var maxuploadsizemb = _postInfo.GetXmlProperty("genxml/textbox/maxuploadsizemb");
                var executiontimeoutminutes = _postInfo.GetXmlProperty("genxml/textbox/executiontimeoutminutes");

                var webConfigPath = Globals.ApplicationMapPath + "\\web.config";
                
                if (!string.IsNullOrEmpty(maxuploadsizemb) || !string.IsNullOrEmpty(executiontimeoutminutes))
                {
                    var xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.Load(webConfigPath);

                    // Update maxRequestLength and executionTimeout in system.web/httpRuntime
                    if (!string.IsNullOrEmpty(maxuploadsizemb) || !string.IsNullOrEmpty(executiontimeoutminutes))
                    {
                        var systemWeb = xmlDoc.SelectSingleNode("//configuration/system.web") as System.Xml.XmlElement;
                        if (systemWeb != null)
                        {
                            var httpRuntime = systemWeb.SelectSingleNode("httpRuntime") as System.Xml.XmlElement;
                            if (httpRuntime == null)
                            {
                                httpRuntime = xmlDoc.CreateElement("httpRuntime");
                                systemWeb.AppendChild(httpRuntime);
                            }

                            if (!string.IsNullOrEmpty(maxuploadsizemb) && int.TryParse(maxuploadsizemb, out var uploadSizeMB))
                            {
                                var maxRequestLengthKB = uploadSizeMB * 1024;
                                httpRuntime.SetAttribute("maxRequestLength", maxRequestLengthKB.ToString());
                            }

                            if (!string.IsNullOrEmpty(executiontimeoutminutes) && int.TryParse(executiontimeoutminutes, out var timeoutMinutes))
                            {
                                var executionTimeoutSeconds = timeoutMinutes * 60;
                                httpRuntime.SetAttribute("executionTimeout", executionTimeoutSeconds.ToString());
                            }
                        }
                    }

                    // Update maxAllowedContentLength in system.webServer/security/requestFiltering/requestLimits
                    if (!string.IsNullOrEmpty(maxuploadsizemb) && int.TryParse(maxuploadsizemb, out var uploadMB))
                    {
                        var systemWebServer = xmlDoc.SelectSingleNode("//configuration/system.webServer") as System.Xml.XmlElement;
                        if (systemWebServer != null)
                        {
                            var security = systemWebServer.SelectSingleNode("security") as System.Xml.XmlElement;
                            if (security == null)
                            {
                                security = xmlDoc.CreateElement("security");
                                systemWebServer.AppendChild(security);
                            }

                            var requestFiltering = security.SelectSingleNode("requestFiltering") as System.Xml.XmlElement;
                            if (requestFiltering == null)
                            {
                                requestFiltering = xmlDoc.CreateElement("requestFiltering");
                                security.AppendChild(requestFiltering);
                            }

                            var requestLimits = requestFiltering.SelectSingleNode("requestLimits") as System.Xml.XmlElement;
                            if (requestLimits == null)
                            {
                                requestLimits = xmlDoc.CreateElement("requestLimits");
                                requestFiltering.AppendChild(requestLimits);
                            }

                            var maxAllowedContentLengthBytes = (long)uploadMB * 1024 * 1024;
                            requestLimits.SetAttribute("maxAllowedContentLength", maxAllowedContentLengthBytes.ToString());
                        }
                    }

                    RetryableAction.Retry5TimesWith2SecondsDelay(() => xmlDoc.Save(webConfigPath), "Saving web.config");
                }

            }
            return "";
        }
    }

}
