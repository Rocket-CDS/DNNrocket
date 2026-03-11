using Dnn.ExportImport.Components.Common;
using Dnn.ExportImport.Components.Controllers;
using Dnn.ExportImport.Components.Dto;
using Dnn.ExportImport.Components.Entities;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Linq;

/// <summary>
/// Provides helper methods for automatic export and import of DNN websites.
/// </summary>
public class DnnSiteExportImportHelper
{
    private const int DefaultPollingIntervalMs = 2000; // Poll every 2 seconds
    private const int DefaultTimeoutMinutes = 120; // 2 hours default timeout

    /// <summary>
    /// Triggers the export of a website with all default settings and waits for completion.
    /// </summary>
    /// <param name="portalId">The ID of the portal to export.</param>
    /// <param name="userId">The ID of the user performing the export.</param>
    /// <param name="exportName">The name of the export job (optional).</param>
    /// <param name="exportDescription">The description of the export job (optional).</param>
    /// <param name="timeoutMinutes">Maximum time to wait in minutes (default: 120).</param>
    /// <param name="pollingIntervalMs">How often to check job status in milliseconds (default: 2000).</param>
    /// <returns>The completed export job.</returns>
    public ExportImportJob ExportWebsiteAndWait(
        int portalId,
        int userId,
        string exportName = null,
        string exportDescription = null,
        SimplisityRecord extraExportSettings = null,
        int timeoutMinutes = DefaultTimeoutMinutes,
        int pollingIntervalMs = DefaultPollingIntervalMs)
    {
        var jobId = ExportWebsite(portalId, userId, exportName, exportDescription);
        var exportImportJob = WaitForJobCompletion(jobId, timeoutMinutes, pollingIntervalMs);
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
                extraExportSettings.AddRecordListItem("portalsettings",ps);
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
                                            var exportDirectory = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJob.Directory);

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

            var fName = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJob.Directory + "/RocketSettings.xml");
            extraExportSettings.XMLDoc.Save(fName);
        }
        return exportImportJob;
    }

    private static int GetNewModuleId(int portalId, int legacyModuleId)
    {
        var objCtrl = new DNNrocketController();
        var sqlCmd = "select [ModuleId] from {databaseOwner}[{objectQualifier}DNNrocket] where PortalId = " + portalId + " and [XMLData].value('(genxml/legacymoduleid)[1]','int') = " + legacyModuleId;
        var strItemId = objCtrl.ExecSql(sqlCmd);
        if (GeneralUtils.IsNumeric(strItemId)) return Convert.ToInt32(strItemId);
        return -1;
    }

    /// <summary>
    /// Triggers the export of a website with all default settings (async, returns immediately).
    /// </summary>
    /// <param name="portalId">The ID of the portal to export.</param>
    /// <param name="userId">The ID of the user performing the export.</param>
    /// <param name="exportName">The name of the export job (optional).</param>
    /// <param name="exportDescription">The description of the export job (optional).</param>
    /// <returns>The job ID of the queued export operation.</returns>
    public int ExportWebsite(int portalId, int userId, string exportName = null, string exportDescription = null)
    {
        var portal = PortalController.Instance.GetPortal(portalId);
        if (portal == null)
        {
            throw new ArgumentException($"Portal with ID {portalId} does not exist.", nameof(portalId));
        }

        var exportDto = new ExportDto
        {
            PortalId = portalId,
            ExportName = exportName ?? $"Export_{portal.PortalName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}",
            ExportDescription = exportDescription ?? $"Automatic export of {portal.PortalName}",

            // CRITICAL: Must populate ItemsToExport array
            ItemsToExport = new[]
            {
                Constants.Category_Content,      // "CONTENT"
                Constants.Category_Assets,       // "ASSETS"
                Constants.Category_Users,        // "USERS"
                Constants.Category_Roles,        // "ROLES"
                Constants.Category_Vocabularies, // "VOCABULARIES"
                Constants.Category_Templates,    // "TEMPLATES"
                Constants.Category_ProfileProps, // "PROFILE_PROPERTIES"
                Constants.Category_Packages,     // "PACKAGES"
                Constants.Category_Workflows     // "WORKFLOW" (if needed)
            },

            // These flags work in conjunction with ItemsToExport
            IncludeContent = true,
            IncludeFiles = true,
            IncludeUsers = true,
            IncludeRoles = true,
            IncludeVocabularies = true,
            IncludeTemplates = true,
            IncludeProperfileProperties = true,
            IncludeExtensions = true,
            IncludePermissions = true,
            IncludeDeletions = false,
            ExportMode = ExportMode.Full,

            // CRITICAL: Must set CheckedState to CheckedWithAllChildren for full export!
            Pages = new[]
            {
                new PageToExport
                {
                    TabId = -1, // All pages
                    ParentTabId = -1,
                    CheckedState = TriCheckedState.CheckedWithAllChildren // Export all page content & modules
                }
            },
            RunNow = true
        };

        var exportController = new ExportController();
        return exportController.QueueOperation(userId, exportDto);
    }

    /// <summary>
    /// Imports an export package and waits for completion.
    /// </summary>
    /// <param name="portalId">The ID of the portal to import into (overwrite).</param>
    /// <param name="userId">The ID of the user performing the import.</param>
    /// <param name="packageId">The package ID (folder name in the ExportImport directory).</param>
    /// <param name="collisionResolution">How to handle collisions during import (default: Overwrite).</param>
    /// <param name="timeoutMinutes">Maximum time to wait in minutes (default: 120).</param>
    /// <param name="pollingIntervalMs">How often to check job status in milliseconds (default: 2000).</param>
    /// <returns>The completed import job.</returns>
    public ExportImportJob ImportWebsiteAndWait(
        int portalId,
        int userId,
        string packageId,
        CollisionResolution collisionResolution = CollisionResolution.Overwrite,
        int timeoutMinutes = DefaultTimeoutMinutes,
        int pollingIntervalMs = DefaultPollingIntervalMs)
    {
        var objCtrl = new DNNrocketController();
        var jobId = ImportWebsite(portalId, userId, packageId, collisionResolution);
        var exportImportJob = WaitForJobCompletion(jobId, timeoutMinutes, pollingIntervalMs);
        var importMapPathFolder = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJob.Directory);
        if (exportImportJob != null)
        {
            var rocketSettings = new SimplisityRecord();
            var fName = importMapPathFolder + "\\RocketSettings.xml";
            if (File.Exists(fName))
            {
                rocketSettings.XMLData = FileUtils.ReadFile(fName);
                DNNrocketUtils.SetTempRecordStorage(rocketSettings, portalId + "RocketSettings.xml");
                var homeTabId = GetHomeTabIdByName(portalId, rocketSettings.GetXmlProperty("genxml/hometab/name"), rocketSettings.GetXmlProperty("genxml/hometab/title"));
                if (homeTabId > 0)
                {
                    foreach (var l in DNNrocketUtils.GetCultureCodeList(portalId))
                    {
                        var portalInfo = PortalController.Instance.GetPortal(portalId, l);
                        if (portalInfo != null)
                        {
                            portalInfo.HomeTabId = homeTabId;
                            PortalController.Instance.UpdatePortalInfo(portalInfo);
                        }
                    }
                }
                // Import Portal Localization
                var plz = rocketSettings.GetRecordList("portallocalization");
                foreach (var pz in plz)
                {
                    var sqlCmd = "update [dbo].[PortalLocalization] set PortalName = '" + pz.GetXmlProperty("row/@PortalName") + "' where portalid = " + portalId + " and CultureCode = '" + pz.GetXmlProperty("row/@CultureCode") + "' ";
                    objCtrl.ExecSql(sqlCmd);
                    sqlCmd = "update [dbo].[PortalLocalization] set FooterText = '" + pz.GetXmlProperty("row/@FooterText") + "' where portalid = " + portalId + " and CultureCode = '" + pz.GetXmlProperty("row/@CultureCode") + "' ";
                    objCtrl.ExecSql(sqlCmd);
                    sqlCmd = "update [dbo].[PortalLocalization] set LogoFile = '" + pz.GetXmlProperty("row/@LogoFile") + "' where portalid = " + portalId + " and CultureCode = '" + pz.GetXmlProperty("row/@CultureCode") + "' ";
                    objCtrl.ExecSql(sqlCmd);
                }

                // Import portal Setitngs
                var pl = rocketSettings.GetRecordList("portalsettings");
                foreach (var ps in pl)
                {
                    var settingName = ps.GetXmlProperty("row/@SettingName");
                    var settingValue = ps.GetXmlProperty("row/@SettingValue");
                    var cultureCode = ps.GetXmlProperty("row/@CultureCode");

                    var sqlCmd = $@"
                MERGE INTO {{databaseOwner}}[{{objectQualifier}}PortalSettings] AS target
                USING (SELECT {portalId} AS PortalID, '{settingName}' AS SettingName) AS source
                ON target.PortalID = source.PortalID AND target.SettingName = source.SettingName
                WHEN MATCHED THEN
                    UPDATE SET 
                        SettingValue = '{settingValue}',
                        LastModifiedByUserID = {userId},
                        LastModifiedOnDate = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (PortalID, SettingName, SettingValue, CreatedByUserID, CreatedOnDate, LastModifiedByUserID, LastModifiedOnDate, CultureCode)
                    VALUES ({portalId}, '{settingName}', '{settingValue}', {userId}, GETDATE(), -1, GETDATE(), {(string.IsNullOrEmpty(cultureCode) ? "NULL" : $"'{cultureCode}'")});";

                    objCtrl.ExecSql(sqlCmd);

                }

                //Import appthemeprojects
                var importProjects = rocketSettings.GetRecordList("appthemeprojects");
                var appThemeProjectRecord = objCtrl.GetRecordByGuidKey(-1, -1, "APPTHEMEPROJECTS", "APPTHEMEPROJECTS");
                if (appThemeProjectRecord != null)
                {
                    foreach (var at in importProjects)
                    {
                        var appthemeProjects = appThemeProjectRecord.GetRecordListItem("appthemeprojects", "genxml/textbox/githubrepourl", at.GetXmlProperty("genxml/textbox/githubrepourl"));
                        if (appthemeProjects == null && at.GetXmlProperty("genxml/textbox/githubtoken") == "") // do not import security tokens.
                        {
                            appThemeProjectRecord.AddRecordListItem("appthemeprojects", at);
                        }
                    }
                    objCtrl.Update(appThemeProjectRecord);
                }

            }

            // Import System Directory data
            var systemListData = new SystemLimpetList();
            foreach (var systemData in systemListData.GetSystemActiveList())
            {
                if (systemData.Exists)
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
                                    paramInfo.SetXmlPropertyInt("genxml/hidden/portalid", portalId);
                                    paramInfo.SetXmlProperty("genxml/hidden/systemkey", systemData.SystemKey);
                                    paramInfo.SetXmlProperty("genxml/hidden/extractfolder", importMapPathFolder);
                                    var exportZipMapPath = "";
                                    var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketdirectoryapi_importdata", systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "", "");
                                    if (returnDictionary.ContainsKey("outputhtml")) exportZipMapPath = (string)returnDictionary["outputhtml"];
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

            // Do module settings tab ID.
            var moduleSettingsList = objCtrl.GetList(portalId, -1, "MODSETTINGS");
            foreach (var mRec in moduleSettingsList)
            {
                // Update moduleref to new moduleref, to link API moduules.
                if (mRec.GetXmlProperty("genxml/settings/apimoduleref") != "")
                {
                    var lm = mRec.GetXmlProperty("genxml/settings/apimoduleref").Split('_');
                    if (lm.Length == 3)
                    {
                        var legacyModuleId = lm[2];
                        if (GeneralUtils.IsNumeric(legacyModuleId))
                        {
                            var newModuleId = GetNewModuleId(portalId, Convert.ToInt32(legacyModuleId));
                            if (newModuleId > 0)
                            {
                                var newModuleRef = portalId + "_ModuleID_" + newModuleId;
                                mRec.SetXmlProperty("genxml/settings/apimoduleref", newModuleRef);
                                objCtrl.Update(mRec);
                            }
                        }
                    }
                }
            }



        }

        return exportImportJob;
    }
    private int GetHomeTabIdByName(int portalId, string tabname, string tabtitle)
    {
        var l = TabController.Instance.GetTabsByPortal(portalId);
        foreach (var t in l)
        {
            if (t.Value.TabName == tabname & t.Value.Title == tabtitle) return t.Value.TabID;
        }
        return -1;
    }

    /// <summary>
    /// Imports an export package (async, returns immediately).
    /// </summary>
    /// <param name="portalId">The ID of the portal to import into (overwrite).</param>
    /// <param name="userId">The ID of the user performing the import.</param>
    /// <param name="packageId">The package ID (folder name in the ExportImport directory).</param>
    /// <param name="collisionResolution">How to handle collisions during import (default: Overwrite).</param>
    /// <returns>The job ID of the queued import operation.</returns>
    public int ImportWebsite(
        int portalId,
        int userId,
        string packageId,
        CollisionResolution collisionResolution = CollisionResolution.Overwrite)
    {
        var portal = PortalController.Instance.GetPortal(portalId);
        if (portal == null)
        {
            throw new ArgumentException($"Portal with ID {portalId} does not exist.", nameof(portalId));
        }

        var importController = new ImportController();
        var summary = new ImportExportSummary();

        if (!importController.VerifyImportPackage(packageId, summary, out string errorMessage))
        {
            throw new InvalidOperationException($"Invalid import package: {errorMessage}");
        }

        var importDto = new ImportDto
        {
            PortalId = portalId,
            PackageId = packageId,
            CollisionResolution = collisionResolution,
            RunNow = true
        };

        return importController.QueueOperation(userId, importDto);
    }

    /// <summary>
    /// Waits for a job to complete by polling its status.
    /// </summary>
    /// <param name="jobId">The job ID to monitor.</param>
    /// <param name="timeoutMinutes">Maximum time to wait in minutes.</param>
    /// <param name="pollingIntervalMs">How often to check job status in milliseconds.</param>
    /// <returns>The completed job.</returns>
    /// <exception cref="TimeoutException">Thrown when the job doesn't complete within the timeout period.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the job fails or is cancelled.</exception>
    public ExportImportJob WaitForJobCompletion(int jobId, int timeoutMinutes = DefaultTimeoutMinutes, int pollingIntervalMs = DefaultPollingIntervalMs)
    {
        var startTime = DateTime.Now;
        var timeout = TimeSpan.FromMinutes(timeoutMinutes);
        var entitiesController = EntitiesController.Instance;

        while (true)
        {
            var job = entitiesController.GetJobById(jobId);

            if (job == null)
            {
                throw new InvalidOperationException($"Job with ID {jobId} not found.");
            }

            // Check if job is complete
            switch (job.JobStatus)
            {
                case JobStatus.Successful:
                    return job;

                case JobStatus.Failed:
                    throw new InvalidOperationException($"Export/Import job {jobId} failed. Check job logs for details.");

                case JobStatus.Cancelled:
                    throw new InvalidOperationException($"Export/Import job {jobId} was cancelled.");

                case JobStatus.Submitted:
                case JobStatus.InProgress:
                    // Job is still running, continue polling
                    break;

                default:
                    throw new InvalidOperationException($"Unknown job status: {job.JobStatus}");
            }

            // Check timeout
            if (DateTime.Now - startTime > timeout)
            {
                throw new TimeoutException($"Export/Import job {jobId} did not complete within {timeoutMinutes} minutes. Job status: {job.JobStatus}");
            }

            // Wait before next poll
            Thread.Sleep(pollingIntervalMs);
        }
    }

    /// <summary>
    /// Async version: Waits for a job to complete by polling its status.
    /// </summary>
    /// <param name="jobId">The job ID to monitor.</param>
    /// <param name="timeoutMinutes">Maximum time to wait in minutes.</param>
    /// <param name="pollingIntervalMs">How often to check job status in milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The completed job.</returns>
    public async Task<ExportImportJob> WaitForJobCompletionAsync(
        int jobId,
        int timeoutMinutes = DefaultTimeoutMinutes,
        int pollingIntervalMs = DefaultPollingIntervalMs,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.Now;
        var timeout = TimeSpan.FromMinutes(timeoutMinutes);
        var entitiesController = EntitiesController.Instance;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var job = entitiesController.GetJobById(jobId);

            if (job == null)
            {
                throw new InvalidOperationException($"Job with ID {jobId} not found.");
            }

            switch (job.JobStatus)
            {
                case JobStatus.Successful:
                    return job;

                case JobStatus.Failed:
                    throw new InvalidOperationException($"Export/Import job {jobId} failed. Check job logs for details.");

                case JobStatus.Cancelled:
                    throw new InvalidOperationException($"Export/Import job {jobId} was cancelled.");

                case JobStatus.Submitted:
                case JobStatus.InProgress:
                    break;

                default:
                    throw new InvalidOperationException($"Unknown job status: {job.JobStatus}");
            }

            if (DateTime.Now - startTime > timeout)
            {
                throw new TimeoutException($"Export/Import job {jobId} did not complete within {timeoutMinutes} minutes. Job status: {job.JobStatus}");
            }

            await Task.Delay(pollingIntervalMs, cancellationToken);
        }
    }

    /// <summary>
    /// Gets the current status of a job.
    /// </summary>
    /// <param name="jobId">The job ID.</param>
    /// <returns>The job details.</returns>
    public ExportImportJob GetJobStatus(int jobId)
    {
        return EntitiesController.Instance.GetJobById(jobId);
    }
}

