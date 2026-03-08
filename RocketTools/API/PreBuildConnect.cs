using DNNrocketAPI.Components;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
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
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, null, _dataObjects, null, _sessionParams, true);
            if (!pr.IsValid) return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ExportPreBuildExe()
        {
            try
            {
                if (!UserUtils.IsAdministrator()) return "Must be Administrator.";
                var exportFolder = DNNrocketUtils.ExportWebsite(_portalData.PortalId);
                if (!String.IsNullOrEmpty(exportFolder))
                {
                    var exportDataFolder = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportFolder);

                    var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\PreBuilds";
                    if (!Directory.Exists(exportZipMapPath)) Directory.CreateDirectory(exportZipMapPath);
                    exportZipMapPath += "\\prebuild" + _portalData.PortalId + "_" + GeneralUtils.SanitizeFileName(_portalData.Name).Replace(".", "_") + ".zip";

                    if (File.Exists(exportZipMapPath)) File.Delete(exportZipMapPath);
                    if (Directory.Exists(exportDataFolder))
                    {
                        ZipFile.CreateFromDirectory(exportDataFolder, exportZipMapPath);
                        Directory.Delete(exportDataFolder, true);
                    }
                }
                return exportFolder;
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
                var importFile = PortalUtils.TempDirectoryMapPath() + "\\" + UserUtils.GetCurrentUserId() + "_" + Path.GetFileName(importfileupload);
                if (!File.Exists(importFile)) return ImportPreBuild();

                // Remove Workflow, solve DNN bug
                var sqlcmd = "DELETE ht FROM {databaseOwner}[{objectQualifier}HtmlText] ht INNER JOIN {databaseOwner}[{objectQualifier}Modules] m ON ht.ModuleID = m.ModuleID WHERE m.PortalID = " + _portalData.PortalId;
                _objCtrl.ExecSql(sqlcmd);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}ContentWorkflows] where PortalID = " + _portalData.PortalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}DNNrocket] where PortalID = " + _portalData.PortalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}RocketDirectoryAPI] where PortalID = " + _portalData.PortalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}RocketContentAPI] where PortalID = " + _portalData.PortalId);

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
            }
            return ImportPreBuild();
        }

    }

}
