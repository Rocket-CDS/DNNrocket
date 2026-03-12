using DNNrocketAPI.Components;
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
                    exportZipMapPath += "\\prebuild" + _portalData.PortalId + "_" + GeneralUtils.SanitizeFileName(_portalData.Name).Replace(".", "_") + ".zip";

                    if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
                    if (Directory.Exists(exportDataFolder))
                    {
                        ZipFile.CreateFromDirectory(exportDataFolder, exportZipMapPath);
                        Directory.Delete(exportDataFolder, true);
                        _passSettings.Add("exportprebuild", "true");
                        _passSettings.Add("exportfilename", exportZipMapPath);
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

                // Check portal alias, incase extra language added.
                var currentUrl = _sessionParams.Get("requesturl");
                if (GeneralUtils.IsAbsoluteUrl(currentUrl))
                {
                    var aliasList = PortalUtils.GetPortalAliases(_portalData.PortalId);
                    foreach (var pa in aliasList)
                    {
                        PortalUtils.DeletePortalAlias(_portalData.PortalId, pa);
                    }
                    var newPortAliases = new Dictionary<string, string>();
                    PortalUtils.AddPortalAlias(_portalData.PortalId, PortalUtils.GetDomainFromUrl(currentUrl), "");
                    foreach (var lang in DNNrocketUtils.GetCultureCodeList(_portalData.PortalId))
                    {
                        var aliasUrl = PortalUtils.GetDomainFromUrl(currentUrl) + "/" + lang.ToLower();
                        if (!newPortAliases.ContainsKey(lang)) newPortAliases.Add(lang, aliasUrl);
                    }
                    foreach (var pa in newPortAliases)
                    {
                        PortalUtils.AddPortalAlias(_portalData.PortalId, pa.Value, pa.Key);
                    }
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

    }

}
