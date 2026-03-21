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
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// Provides helper methods for automatic export and import of DNN websites.
/// </summary>
public class DnnSiteExportImportHelper
{
    private const int DefaultPollingIntervalMs = 2000; // Poll every 2 seconds
    private const int DefaultTimeoutMinutes = 120; // 2 hours default timeout

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

            // ItemsToExport should be EMPTY or contain only custom extension categories
            // Built-in categories are controlled by boolean flags below
            ItemsToExport = null, // or Array.Empty<string>() or omit entirely

            // These flags control the built-in categories
            IncludeContent = true,              // → Constants.Category_Content
            IncludeFiles = true,                // → Constants.Category_Assets
            IncludeUsers = true,                // → Constants.Category_Users
            IncludeRoles = true,                // → Constants.Category_Roles
            IncludeVocabularies = true,         // → Constants.Category_Vocabularies
            IncludeTemplates = true,            // → Constants.Category_Templates
            IncludeProperfileProperties = true, // → Constants.Category_ProfileProps
            IncludeExtensions = false,           // → Constants.Category_Packages (set to true if needed)

            IncludePermissions = true,
            IncludeDeletions = false,
            ExportMode = ExportMode.Full,

            // Pages array triggers Constants.Category_Pages and Constants.Category_Workflows
            Pages = new[]
            {
            new PageToExport
            {
                TabId = -1,
                ParentTabId = -1,
                CheckedState = TriCheckedState.CheckedWithAllChildren
            }
        },

            RunNow = true
        };

        var exportController = new ExportController();
        return exportController.QueueOperation(userId, exportDto);
    }
    /// <summary>
    /// Triggers the export of a website with all default settings (async, returns immediately).
    /// </summary>
    /// <param name="portalId">The ID of the portal to export.</param>
    /// <param name="userId">The ID of the user performing the export.</param>
    /// <param name="exportName">The name of the export job (optional).</param>
    /// <param name="exportDescription">The description of the export job (optional).</param>
    /// <returns>The job ID of the queued export operation.</returns>
    public int ExportWebsiteOLD(int portalId, int userId, string exportName = null, string exportDescription = null)
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
                // Constants.Category_Packages,  // Uncomment to explicitly control packages
                Constants.Category_Vocabularies, // "VOCABULARIES"
                Constants.Category_Templates,    // "TEMPLATES"
                Constants.Category_ProfileProps, // "PROFILE_PROPERTIES"
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
            IncludeExtensions = false,
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
    public ExportImportJob ImportWebsiteAndWait(int portalId,int userId,string packageId,CollisionResolution collisionResolution = CollisionResolution.Overwrite,int timeoutMinutes = DefaultTimeoutMinutes,int pollingIntervalMs = DefaultPollingIntervalMs)
    {
        var objCtrl = new DNNrocketController();
        var jobId = ImportWebsite(portalId, userId, packageId, collisionResolution);
        var exportImportJob = WaitForJobCompletion(jobId, timeoutMinutes, pollingIntervalMs);

        return exportImportJob;
    }
    /// <summary>
    /// Imports an export package (async, returns immediately).
    /// </summary>
    /// <param name="portalId">The ID of the portal to import into (overwrite).</param>
    /// <param name="userId">The ID of the user performing the import.</param>
    /// <param name="packageId">The package ID (folder name in the ExportImport directory).</param>
    /// <param name="collisionResolution">How to handle collisions during import (default: Overwrite).</param>
    /// <returns>The job ID of the queued import operation.</returns>
    public int ImportWebsite(int portalId, int userId, string packageId, CollisionResolution collisionResolution = CollisionResolution.Overwrite)
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

