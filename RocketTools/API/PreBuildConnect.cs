using Dnn.ExportImport.Components.Controllers;
using Dnn.ExportImport.Components.Entities;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.UserControls;
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
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace RocketTools.API
{
    public partial class StartConnect
    {
        private Dictionary<string, string> _oldTabIdTabPathDict;


        #region "Export"
        public string ExportPreBuild()
        {
            var exportZipMapPath = PortalUtils.TempDirectoryMapPath() + "\\PreBuilds";
            if (Directory.Exists(exportZipMapPath))
            {
                foreach (var f in Directory.GetFiles(exportZipMapPath))
                {
                    _passSettings.Add("exportfilename", Path.GetFileName(f));
                }
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

                // Start the export but DON'T wait.
                var helper = new DnnSiteExportImportHelper();
                var jobId = helper.ExportWebsite(_portalData.PortalId, UserUtils.GetCurrentUserId(), "Rocket PreBuild Export", "Full website backup");

                var entitiesController = EntitiesController.Instance;
                var job = entitiesController.GetJobById(jobId);
                if (job == null) return "ERROR: No Export JobId found";

                RemoveWorkflowFromPortal();

                var extraExportSettings = ExportExtraData(_portalData.PortalId, job.Directory);

                // Save info for later
                var jobInfo = new SimplisityInfo();
                jobInfo.SetXmlProperty("genxml/jobid", jobId.ToString());
                jobInfo.SetXmlProperty("genxml/extrasettings", extraExportSettings.XMLData);

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
        public SimplisityRecord ExportExtraData(int portalId, string exportImportJobDirectory)
        {
            var extraExportSettings = new SimplisityRecord();

            // Export HomeTab
            var homeTabId = PortalUtils.GetCurrentPortalSettings().HomeTabId;
            if (homeTabId > 0)
            {
                var homeTabInfo = DNNrocketUtils.GetTabInfo(portalId, homeTabId, true);
                if (homeTabInfo != null)
                {
                    extraExportSettings.SetXmlProperty("genxml/hometab/hometabid", homeTabId.ToString());
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

            // pause until export folder is available, timeout after 60 seconds
            var exportDirectory = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJobDirectory);
            var timeout = TimeSpan.FromSeconds(60);
            var pollInterval = TimeSpan.FromMilliseconds(1000);
            var startTime = DateTime.Now;
            while (!Directory.Exists(exportDirectory))
            {
                if (DateTime.Now - startTime > timeout)
                {
                    LogUtils.LogSystem($"Timeout waiting for export directory: {exportDirectory}");
                    break;
                }
                Thread.Sleep(pollInterval);
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
                                        var detailTabInfo = DNNrocketUtils.GetTabInfo(portalId, Convert.ToInt32(detailTabId), true);
                                        if (detailTabInfo != null) extraExportSettings.SetXmlProperty("genxml/" + systemData.SystemKey + "/detailtabpath", detailTabInfo.TabPath);
                                    }
                                    var listTabIdSQL = "select XMLData.value('(genxml/listpage)[1]','nvarchar(max)') from {databaseOwner}[{objectQualifier}RocketDirectoryAPI] where portalid = " + portalId + " and TypeCode = '" + systemData.SystemKey + "PortalCatalog'";
                                    var listTabId = objCtrl.ExecSql(listTabIdSQL);
                                    if (GeneralUtils.IsNumeric(listTabId))
                                    {
                                        var listTabInfo = DNNrocketUtils.GetTabInfo(portalId, Convert.ToInt32(listTabId), true);
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

            Thread.Sleep(2000);  // wait for zip to register.

            var fName = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJobDirectory + "/RocketSettings.xml");
            extraExportSettings.XMLDoc.Save(fName);

            return extraExportSettings;
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

        #endregion

        #region "Import"

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
            var userId = UserUtils.GetCurrentUserId();
            var portalId = _portalData.PortalId;
            var importfileupload = _postInfo.GetXmlProperty("genxml/hidden/importfileupload");
            if (importfileupload != "" && Path.GetExtension(importfileupload).ToLower() == ".zip")
            {
                var importFile = PortalUtils.TempDirectoryMapPath(portalId) + "\\" + UserUtils.GetCurrentUserId() + "_" + Path.GetFileNameWithoutExtension(importfileupload);
                if (!File.Exists(importFile)) return ImportPreBuild();

                // Remove Workflow for HtmlText, solve DNN bug
                var sqlcmd = "DELETE ht FROM {databaseOwner}[{objectQualifier}HtmlText] ht INNER JOIN {databaseOwner}[{objectQualifier}Modules] m ON ht.ModuleID = m.ModuleID WHERE m.PortalID = " + portalId;
                _objCtrl.ExecSql(sqlcmd);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}ContentWorkflows] where PortalID = " + portalId);
                RemoveWorkflowFromPortal();

                // Remove Existing RocketData
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}DNNrocket] where PortalID = " + portalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}RocketDirectoryAPI] where PortalID = " + portalId);
                _objCtrl.ExecSql("delete {databaseOwner}[{objectQualifier}RocketContentAPI] where PortalID = " + portalId);


                // Clear system file
                var systemDataMapPath = PortalUtils.HomeDNNrocketDirectoryMapPath(portalId);
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

                PortalUtils.ClearPortalContent(portalId);

                // ######### DO IMPORT ####################################################
                var exportImportJob = DNNrocketUtils.ImportWebsite(portalId, importFile);

                try
                {
                    File.Delete(importFile);
                }
                catch (Exception ex)
                {
                    LogUtils.LogSystem("ERROR: on import: " + ex.ToString());
                }
                if (exportImportJob == null) return "ERROR: no importJob created";
                // ######################################################################## 

                // Check portal alias, in case extra language added.
                //var currentUrl = _sessionParams.Get("requesturl");
                //if (GeneralUtils.IsAbsoluteUrl(currentUrl))
                //{
                //    var domain = PortalUtils.GetDomainFromUrl(currentUrl);
                //    var defaultUrl = PortalUtils.DefaultPortalAlias(portalId);
                //    var cultureCodeList = DNNrocketUtils.GetCultureCodeList(portalId).ToList();
                //    var defaultCultureCode = _portalData.DefaultLanguage();
                //    var aliasList = PortalUtils.GetPortalAliases(portalId);

                //    foreach (var pa in aliasList)
                //    {
                //        if (pa != defaultUrl) PortalUtils.DeletePortalAlias(portalId, pa);
                //    }                   

                //    if (cultureCodeList.Count == 1)
                //    {
                //        // Only one language - assign it to the root domain
                //        PortalUtils.AddPortalAlias(portalId, domain, cultureCodeList[0]);
                //    }
                //    else if (cultureCodeList.Count > 1)
                //    {
                //        // Multiple languages - root domain gets default language
                //        PortalUtils.AddPortalAlias(portalId, domain, defaultCultureCode);

                //        // Add other languages with culture code in URL path
                //        foreach (var lang in cultureCodeList)
                //        {
                //            var aliasUrl = domain + "/" + lang.ToLower();
                //            PortalUtils.AddPortalAlias(portalId, aliasUrl, lang);
                //        }
                //    }
                //    // ensure we have a portalalias
                //    aliasList = PortalUtils.GetPortalAliases(portalId);
                //    if (aliasList.Count == 0) PortalUtils.AddPortalAlias(portalId, domain, "");
                //}

                // --------------------------------------
                if (exportImportJob != null)
                {
                    var objCtrl = new DNNrocketController();
                    var importMapPathFolder = DNNrocketUtils.MapPath("/App_Data/ExportImport/" + exportImportJob.Directory);
                    _oldTabIdTabPathDict = new Dictionary<string, string>();

                    var rocketSettings = new SimplisityRecord();
                    var fName = importMapPathFolder + "\\RocketSettings.xml";
                    if (File.Exists(fName))
                    {
                        rocketSettings.XMLData = FileUtils.ReadFile(fName);

                        // build TabPathDict
                        foreach (var row in rocketSettings.GetRecordList("tabpath"))
                        {
                            _oldTabIdTabPathDict.Add(row.GetXmlProperty("row/@TabId"), row.GetXmlProperty("row/@TabPath"));
                        }

                        if (true)
                        {

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
                        }
                        if (false)
                        {
                            // WARNING: Copying all portalsettings will stop Pages from working in the menu.
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
                        }
                        if (true)
                        {

                            var homeTabPath = GetTabPath(rocketSettings.GetXmlPropertyInt("genxml/hometab/hometabid"));
                            var homeTabId = GetTabId(portalId, homeTabPath);
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
                        }

                        if (true)
                        {

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

                    }


                    if (true)
                    {

                        // Import System Directory data
                        var systemListData = new SystemLimpetList();
                        foreach (var systemData in systemListData.GetSystemActiveList())
                        {
                            if (systemData.Exists)
                            {
                                foreach (var rocketInterface in systemData.ProviderList)
                                {
                                    if (rocketInterface.IsProvider("importmodule"))
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
                    }

                    if (true)
                    {
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
                            RemapInternalLinks(portalId, mRec.ModuleId, rocketSettings);
                        }
                    }
                    if (true)
                    {
                        // Download public Apptheme
                        var appThemeProjectData = new AppThemeProjectLimpet();
                        foreach (var at in appThemeProjectData.List)
                        {
                            try
                            {
                                var projectName = at.GetXmlProperty("genxml/textbox/name");
                                if (at.GetXmlProperty("genxml/textbox/githubtoken") == "") appThemeProjectData.DownloadGitHubProject(projectName);
                            }
                            catch (Exception)
                            {
                                // ignore
                            }
                        }
                    }

                    Directory.Delete(importMapPathFolder, true);

                    _passSettings.Add("importprebuild", "true"); // flag to show completed msg
                }


                CacheFileUtils.ClearAllCache(_portalData.PortalId);
            }
            return ImportPreBuild();
        }
        private static int GetNewModuleId(int portalId, int legacyModuleId)
        {
            var objCtrl = new DNNrocketController();
            var sqlCmd = "select [ModuleId] from {databaseOwner}[{objectQualifier}DNNrocket] where PortalId = " + portalId + " and [XMLData].value('(genxml/legacymoduleid)[1]','int') = " + legacyModuleId;
            var strItemId = objCtrl.ExecSql(sqlCmd);
            if (GeneralUtils.IsNumeric(strItemId)) return Convert.ToInt32(strItemId);
            return -1;
        }
        private string GetTabPath(int oldTabId)
        {
            if (_oldTabIdTabPathDict.ContainsKey(oldTabId.ToString())) return _oldTabIdTabPathDict[oldTabId.ToString()];
            return "";
        }
        private int GetTabId(int portalId, string tabPath)
        {
            var newTabId = -1;
            if (tabPath != "")
            {
                var objCtrl = new DNNrocketController();
                var sqlCmd = "select TabId FROM {databaseOwner}[{objectQualifier}Tabs] where portalid = " + portalId + " and tabpath = '" + tabPath + "'";
                var sqlRtn = objCtrl.ExecSql(sqlCmd);
                if (GeneralUtils.IsNumeric(sqlRtn)) return Convert.ToInt32(sqlRtn);
            }
            return newTabId;
        }

        private void RemapInternalLinks(int portalId, int moduleId, SimplisityRecord rocketSettings)
        {
            var objCtrl = new DNNrocketController();
            var moduleRef = portalId + "_ModuleID_" + moduleId;

            var artRec = objCtrl.GetRecordByGuidKey(portalId, moduleId, "ART", moduleRef, "", "RocketContentAPI");
            if (artRec != null)
            {
                var upd = false;
                var rows = artRec.GetRecordList("rows");
                var rowlp = 1;
                foreach (var row in rows)
                {
                    var oldTabId1 = row.GetXmlPropertyInt("genxml/select/internallinkarticlelink");
                    if (oldTabId1 > 0)
                    {
                        artRec.SetXmlPropertyInt("genxml/rows/genxml[" + rowlp + "]/select/internallinkarticlelink", GetTabId(portalId, GetTabPath(oldTabId1)));
                        upd = true;
                    }

                    var l = row.GetRecordList("linklist");
                    var lklp = 1;
                    foreach (var lk in l)
                    {
                        var oldTabId = lk.GetXmlPropertyInt("genxml/select/internallinkarticlelink");
                        if (oldTabId > 0)
                        {
                            artRec.SetXmlPropertyInt("genxml/rows/genxml[" + rowlp + "]/linklist/genxml[" + lklp + "]/select/internallinkarticlelink", GetTabId(portalId,GetTabPath(oldTabId)));
                            upd = true;
                        }
                        lklp += 1;
                    }
                    rowlp += 1;
                }
                if (upd)
                {
                    objCtrl.Update(artRec, "RocketContentAPI");
                    objCtrl.RebuildLangIndex(portalId, artRec.ItemID, "RocketContentAPI");
                }
            }
        }
        public string RemoveWorkflowFromPortal()
        {
            if (!UserUtils.IsAdministrator())
            {
                return "Must be Administrator.";
            }

            try
            {
                var portalId = _portalData.PortalId;

                // 1. Remove workflow state from all Tabs (set StateID to -1) - only if column exists
                var checkStateIdColumn = @"SELECT COUNT(*) 
                                   FROM INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = '{objectQualifier}Tabs' 
                                   AND COLUMN_NAME = 'StateID'";
                var stateIdExists = _objCtrl.ExecSql(checkStateIdColumn);

                if (stateIdExists == "1")
                {
                    var sql1 = "UPDATE {databaseOwner}[{objectQualifier}Tabs] SET StateID = -1 WHERE PortalID = " + portalId + " AND StateID IS NOT NULL AND StateID != -1";
                    _objCtrl.ExecSql(sql1);
                }

                // 2. Mark all TabVersions as published and set to the latest version
                var sql2 = @"UPDATE tv SET tv.IsPublished = 1 
                     FROM {databaseOwner}[{objectQualifier}TabVersions] tv 
                     INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t ON tv.TabID = t.TabID
                     INNER JOIN (
                         SELECT tv2.TabID, MAX(tv2.Version) AS MaxVersion
                         FROM {databaseOwner}[{objectQualifier}TabVersions] tv2
                         INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t2 ON tv2.TabID = t2.TabID
                         WHERE t2.PortalID = " + portalId + @"
                         GROUP BY tv2.TabID
                     ) maxv ON tv.TabID = maxv.TabID AND tv.Version = maxv.MaxVersion
                     WHERE t.PortalID = " + portalId;
                _objCtrl.ExecSql(sql2);

                // 3. Unpublish all older versions (keep only the latest as published)
                var sql3 = @"UPDATE tv SET tv.IsPublished = 0 
                     FROM {databaseOwner}[{objectQualifier}TabVersions] tv 
                     INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t ON tv.TabID = t.TabID
                     INNER JOIN (
                         SELECT tv2.TabID, MAX(tv2.Version) AS MaxVersion
                         FROM {databaseOwner}[{objectQualifier}TabVersions] tv2
                         INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t2 ON tv2.TabID = t2.TabID
                         WHERE t2.PortalID = " + portalId + @"
                         GROUP BY tv2.TabID
                     ) maxv ON tv.TabID = maxv.TabID AND tv.Version < maxv.MaxVersion
                     WHERE t.PortalID = " + portalId;
                _objCtrl.ExecSql(sql3);

                // 4. Clear ContentWorkflowLogs for this portal
                var sql4 = @"DELETE cwl FROM {databaseOwner}[{objectQualifier}ContentWorkflowLogs] cwl
                     INNER JOIN {databaseOwner}[{objectQualifier}ContentItems] ci ON cwl.ContentItemID = ci.ContentItemID
                     INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t ON ci.ContentKey = CAST(t.TabID AS NVARCHAR(50))
                     WHERE t.PortalID = " + portalId;
                _objCtrl.ExecSql(sql4);

                // 5. Get the Published state ID
                var sqlGetPublishedState = @"SELECT MIN(StateID) 
                                     FROM {databaseOwner}[{objectQualifier}ContentWorkflowStates] 
                                     WHERE StateName = 'Published'";
                var publishedStateIdStr = _objCtrl.ExecSql(sqlGetPublishedState);
                var publishedStateId = GeneralUtils.IsNumeric(publishedStateIdStr) ? publishedStateIdStr : "-1";

                // 6. Update all ContentItems to published state for this portal
                var sql5 = @"UPDATE ci SET ci.StateID = " + publishedStateId + @" 
                     FROM {databaseOwner}[{objectQualifier}ContentItems] ci
                     INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t ON ci.ContentKey = CAST(t.TabID AS NVARCHAR(50))
                     WHERE t.PortalID = " + portalId + @"
                     AND ci.StateID IS NOT NULL 
                     AND ci.StateID != " + publishedStateId;
                _objCtrl.ExecSql(sql5);

                // 7. Optional: Delete draft TabVersionDetails
                var sql6 = @"DELETE tvd 
                     FROM {databaseOwner}[{objectQualifier}TabVersionDetails] tvd 
                     INNER JOIN {databaseOwner}[{objectQualifier}TabVersions] tv ON tvd.TabVersionID = tv.TabVersionID
                     INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t ON tv.TabID = t.TabID
                     WHERE tv.IsPublished = 0 
                     AND t.PortalID = " + portalId;
                _objCtrl.ExecSql(sql6);

                // 8. Optional: Delete unpublished TabVersions
                var sql7 = @"DELETE tv 
                     FROM {databaseOwner}[{objectQualifier}TabVersions] tv
                     INNER JOIN {databaseOwner}[{objectQualifier}Tabs] t ON tv.TabID = t.TabID
                     WHERE tv.IsPublished = 0 
                     AND t.PortalID = " + portalId;
                _objCtrl.ExecSql(sql7);

                LogUtils.LogSystem("Workflow removed successfully for portal " + portalId);
                return "Workflow removed successfully";
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return "ERROR: " + ex.Message;
            }
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

        #endregion

    }

}
