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
            _passSettings.Add("importprebuild","false");
            _passSettings.Add("importprebuilderr", "");
            var fileuploadlist = _postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            var fileuploadbase64 = _postInfo.GetXmlProperty("genxml/hidden/fileuploadbase64");
            if (fileuploadbase64 != "")
            {
                var filenameList = fileuploadlist.Split('*');
                var filebase64List = fileuploadbase64.Split('*');
                var destDir = PortalUtils.TempDirectoryMapPath() + "\\Import";
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                var fileList = DocUtils.UploadBase64fileDict(filenameList, filebase64List, destDir);
                if (fileList.Count == 0)
                {
                    _passSettings.Add("importprebuilderr", "ERROR: No import file found.");
                    return ImportPreBuild();
                }
                var docFileMapPath = fileList.First();
                if (File.Exists(docFileMapPath.Key))
                {
                    if (DNNrocketUtils.ImportWebsite(_portalData.PortalId, docFileMapPath.Key))
                        _passSettings.Add("importprebuild", "true");
                    else
                    {
                        _passSettings.Add("importprebuild", "false");
                        _passSettings.Add("importprebuilderr", "ERROR: Import Failed.");
                    }
                    File.Delete(docFileMapPath.Key);
                }
            }
            return ImportPreBuild();
        }

    }

}
