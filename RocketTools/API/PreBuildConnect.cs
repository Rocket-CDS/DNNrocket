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
using System.Runtime.InteropServices.ComTypes;

namespace RocketTools.API
{
    public partial class StartConnect
    {
        public string ExportPreBuild()
        {
            var razorTempl = _appThemeTools.GetTemplate("prebuildexport.cshtml");
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

                var exportFolder = DNNrocketUtils.ExportWebsite(_portalData.PortalId, extraExportSettings);
                if (!String.IsNullOrEmpty(exportFolder))
                {
                    var exportDataFolder = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportFolder);

                    var directoryExportMapPath = PortalUtils.HomeDNNrocketDirectoryMapPath(_portalData.PortalId) + "\\ImportExportDirectory";
                    if (File.Exists(directoryExportMapPath))
                    {
                        File.Move(directoryExportMapPath, exportDataFolder + "\\export_directorydata.zip");
                    }

                    var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\PreBuilds";
                    if (!Directory.Exists(exportZipMapPath)) Directory.CreateDirectory(exportZipMapPath);
                    exportZipMapPath += "\\prebuild" + _portalData.PortalId + "_" + GeneralUtils.SanitizeFileName(_portalData.Name.Trim().Replace(".", "_").Replace(" ", "_"));

                    if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
                    if (Directory.Exists(exportDataFolder))
                    {
                        ZipFile.CreateFromDirectory(exportDataFolder, exportZipMapPath);
                        Directory.Delete(exportDataFolder, true);
                        _passSettings.Add("exportprebuild", "true");
                        _passSettings.Add("exportfilename", Path.GetFileName(exportZipMapPath));
                    }
                }
                return ExportPreBuild();
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return ex.ToString();
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
