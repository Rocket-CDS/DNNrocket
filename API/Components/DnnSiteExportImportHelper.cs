using System;
using System.Threading;
using System.Threading.Tasks;
using Dnn.ExportImport.Components.Controllers;
using Dnn.ExportImport.Components.Dto;
using Dnn.ExportImport.Components.Common;
using Dnn.ExportImport.Components.Entities;
using DotNetNuke.Entities.Portals;

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
        int timeoutMinutes = DefaultTimeoutMinutes,
        int pollingIntervalMs = DefaultPollingIntervalMs)
    {
        var jobId = ExportWebsite(portalId, userId, exportName, exportDescription);
        return WaitForJobCompletion(jobId, timeoutMinutes, pollingIntervalMs);
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
            Pages = new[] { new PageToExport { TabId = -1 } },
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
        var jobId = ImportWebsite(portalId, userId, packageId, collisionResolution);
        return WaitForJobCompletion(jobId, timeoutMinutes, pollingIntervalMs);
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