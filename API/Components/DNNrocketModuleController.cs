using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DNNrocketAPI.Interfaces;
using Simplisity;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Entities.Content.Taxonomy;
using System.IO;
using DotNetNuke.Services.Journal;
using System.Reflection;
using System.Runtime.InteropServices;
using DotNetNuke.Services.Search.Internals;
using System.Security.Policy;
using static DotNetNuke.Entities.Portals.PortalSettings;
using System.Linq;

namespace DNNrocketAPI.Components
{

    public class DNNrocketModuleController : ModuleSearchBase, IPortable, IUpgradeable
    {

        #region Optional Interfaces

        #region IPortable Members

        public int GetImportTotal()
        {
            return 1;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "moduleId">The Id of the module to be exported</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public string ExportModule(int ModuleId)
        {
            var xmlOut = "";
            var objModCtrl = new ModuleController();
            var objModInfo = objModCtrl.GetModule(ModuleId, Null.NullInteger, true);
            if (objModInfo != null)
            {
                var portalId = objModInfo.PortalID;
                var tabId = objModInfo.TabID;
                var moduleRef = objModInfo.PortalID + "_ModuleID_" + ModuleId;
                var moduleSettings = new ModuleBase(objModInfo.PortalID, moduleRef, ModuleId, objModInfo.TabID); ;
                var systemData = SystemSingleton.Instance(moduleSettings.SystemKey);
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
                                paramInfo.SetXmlProperty("genxml/hidden/tabid", tabId.ToString());
                                paramInfo.SetXmlProperty("genxml/hidden/moduleid", ModuleId.ToString());
                                paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
                                paramInfo.SetXmlProperty("genxml/hidden/moduleref", moduleSettings.ModuleRef);
                                paramInfo.SetXmlProperty("genxml/hidden/systemkey", systemData.SystemKey);
                                systemData.Record.PortalId = portalId;  // export run on shcheduler,

                                var securityKey = DNNrocketUtils.SetTempStorage(new SimplisityInfo());
                                paramInfo.SetXmlProperty("genxml/hidden/securitykey", securityKey);

                                LogUtils.LogSystem("START EXPORT: " + rocketInterface.DefaultCommand + " moduleId: " + ModuleId);
                                try
                                {
                                    var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "", "");
                                    if (returnDictionary.ContainsKey("outputhtml")) xmlOut += returnDictionary["outputhtml"];
                                }
                                catch (Exception ex)
                                {
                                    LogUtils.LogException(ex);
                                }
                                LogUtils.LogSystem("END EXPORT: " + rocketInterface.DefaultCommand + " moduleId: " + ModuleId);
                            }
                        }
                    }
                }
            }
            return xmlOut;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "moduleId">The ID of the Module being imported</param>
        /// <param name = "content">The Content being imported</param>
        /// <param name = "version">The Version of the Module Content being imported</param>
        /// <param name = "userId">The UserID of the User importing the Content</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------

        public void ImportModule(int moduleId, string content, string version, int userId)
        {
            var objModCtrl = new ModuleController();
            if (content.StartsWith("<") && content.EndsWith(">"))
            {
                var objModInfo = objModCtrl.GetModule(moduleId, Null.NullInteger, true);
                if (objModInfo != null)
                {
                    var postInfo = new SimplisityInfo();
                    postInfo.XMLData = content;

                    var portalId = objModInfo.PortalID;
                    var tabId = objModInfo.TabID;
                    var systemKey = postInfo.GetXmlProperty("export/systemkey");
                    var databasetable = postInfo.GetXmlProperty("export/databasetable");

                    var systemData = SystemSingleton.Instance(systemKey);
                    if (systemData.Exists)
                    {
                        foreach (var rocketInterface in systemData.ProviderList)
                        {
                            if (rocketInterface.IsProvider("importmodule"))
                            {
                                if (rocketInterface.Exists)
                                {
                                    var paramInfo = new SimplisityInfo();
                                    paramInfo.SetXmlProperty("genxml/hidden/tabid", tabId.ToString());
                                    paramInfo.SetXmlProperty("genxml/hidden/moduleid", moduleId.ToString());
                                    paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
                                    paramInfo.SetXmlProperty("genxml/hidden/databasetable", databasetable);
                                    paramInfo.SetXmlProperty("genxml/hidden/systemkey", systemKey);
                                    systemData.Record.PortalId = portalId;  // import run on shcheduler,

                                    var securityKey = DNNrocketUtils.SetTempStorage(new SimplisityInfo());
                                    paramInfo.SetXmlProperty("genxml/hidden/securitykey", securityKey);

                                    LogUtils.LogSystem("START IMPORT: " + rocketInterface.DefaultCommand + " moduleId: " + moduleId);
                                    try
                                    {
                                        var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "", "");
                                    }
                                    catch (Exception ex)
                                    {
                                        LogUtils.LogException(ex);
                                    }
                                    LogUtils.LogSystem("END IMPORT: " + rocketInterface.DefaultCommand + " moduleId: " + moduleId);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                LogUtils.LogSystem("IMPORT: Invalid content: " + content);
            }

        }

        #endregion

        #region ModuleSearchBase

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            var searchDocuments = new List<SearchDocument>();

            if (moduleInfo != null)
            {
                try
                {

                    var lastContentModifiedOnDate = moduleInfo.LastContentModifiedOnDate;
                    //LogUtils.LogSystem("[DEBUG:GetModifiedSearchDocuments] - ModuleId: " + moduleInfo.ModuleID + " lastContentModifiedOnDate: " + moduleInfo.LastContentModifiedOnDate.ToString("O") + "  beginDateUtc: " + beginDateUtc.ToString("O"));

                    // "lastContentModifiedOnDate" is the date when content was updated and is updated by using DNNrocketUtils.SynchronizeModule(_moduleid); on Content save.
                    if ((lastContentModifiedOnDate.ToUniversalTime() > beginDateUtc && lastContentModifiedOnDate.ToUniversalTime() < DateTime.UtcNow))
                    {
                        var portalId = moduleInfo.PortalID;
                        var systemKey = moduleInfo.DesktopModule.ModuleName.Replace("Mod", "api");
                        var systemData = SystemSingleton.Instance(systemKey);
                        if (!systemData.Exists) // legacy naming convension
                        {
                            systemKey = moduleInfo.DesktopModule.ModuleName.Replace("Mod", "");
                            systemData = SystemSingleton.Instance(systemKey);
                        }

                        if (systemData.Exists)
                        {
                            foreach (var rocketInterface in systemData.ProviderList)
                            {
                                if (rocketInterface.IsProvider("searchindex"))
                                {
                                    if (rocketInterface.Exists)
                                    {
                                        var paramInfo = new SimplisityInfo();
                                        paramInfo.SetXmlProperty("genxml/hidden/moduleid", moduleInfo.ModuleID.ToString());
                                        paramInfo.SetXmlProperty("genxml/hidden/tabid", moduleInfo.TabID.ToString());
                                        paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());

                                        // We should return ALL languages into the dictionary
                                        var returnDictionary2 = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, new SimplisityInfo(), paramInfo, "", "");
                                        if (returnDictionary2.ContainsKey("searchindex") && returnDictionary2["searchindex"] != null)
                                        {
                                            var returnList = (List<Dictionary<string, object>>)returnDictionary2["searchindex"];
                                            foreach (var idxDict in returnList)
                                            {
                                                var description = "";
                                                var body = "";
                                                var title = "";
                                                var url = "";
                                                var queryString = "";
                                                var culturecode = "";
                                                var tags = new List<string>();
                                                var moddate = DateTime.Now;
                                                var uniqueKey = moduleInfo.ModuleID.ToString();
                                                if (idxDict.ContainsKey("uniquekey")) uniqueKey = (string)idxDict["uniquekey"];
                                                if (string.IsNullOrEmpty(uniqueKey)) uniqueKey = moduleInfo.ModuleID.ToString();
                                                if (idxDict.ContainsKey("description")) description = HtmlUtils.Shorten(HtmlUtils.Clean((string)idxDict["description"], false), 150, "...");
                                                if (idxDict.ContainsKey("body")) body = HtmlUtils.Shorten(HtmlUtils.Clean((string)idxDict["body"], false), 500, "...");
                                                if (idxDict.ContainsKey("title")) title = (string)idxDict["title"];
                                                if (idxDict.ContainsKey("url")) url = (string)idxDict["url"];
                                                if (idxDict.ContainsKey("culturecode")) culturecode = (string)idxDict["culturecode"];
                                                if (idxDict.ContainsKey("querystring")) queryString = (string)idxDict["querystring"];
                                                if (idxDict.ContainsKey("tags")) tags = (List<string>)idxDict["tags"];
                                                if (idxDict.ContainsKey("modifieddate"))
                                                {
                                                    var strDate = idxDict["modifieddate"];
                                                    if (GeneralUtils.IsDateInvariantCulture(strDate)) moddate = Convert.ToDateTime(strDate);
                                                }
                                                if (String.IsNullOrEmpty(title)) title = moduleInfo.ModuleTitle;
                                                var searchDoc = new SearchDocument
                                                {
                                                    UniqueKey = uniqueKey,
                                                    PortalId = moduleInfo.PortalID,
                                                    Title = title,
                                                    Description = description,
                                                    Body = body,
                                                    Url = url,
                                                    QueryString = queryString,
                                                    ModifiedTimeUtc = moddate.ToUniversalTime(),
                                                    CultureCode  = culturecode
                                                };
                                                if (moduleInfo.Terms != null && moduleInfo.Terms.Count > 0)
                                                {
                                                    searchDoc.Tags = CollectHierarchicalTags(moduleInfo.Terms);
                                                }
                                                searchDoc.Tags = searchDoc.Tags.Concat(tags).ToArray();

                                                if (idxDict.ContainsKey("removesearchrecord") && (string)idxDict["removesearchrecord"] == "true")
                                                    InternalSearchController.Instance.DeleteSearchDocument(searchDoc);
                                                else
                                                    searchDocuments.Add(searchDoc);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                }
            }
            return searchDocuments;
        }


        private static List<string> CollectHierarchicalTags(List<Term> terms)
        {
            Func<List<Term>, List<string>, List<string>> collectTagsFunc = null;
            collectTagsFunc = (ts, tags) =>
            {
                if (ts != null && ts.Count > 0)
                {
                    foreach (var t in ts)
                    {
                        if (!tags.Contains(t.Name))
                        {
                            tags.Add(t.Name);
                            tags.AddRange(collectTagsFunc(t.ChildTerms, new List<string>()));
                        }
                    }
                }
                return tags;
            };

            return collectTagsFunc(terms, new List<string>());
        }

        #endregion

        #region IUpgradeable Members
        public string UpgradeModule(string Version)
        {
            LogUtils.LogSystem("UPGRADE START: " + Version);

            foreach (var portalid in PortalUtils.GetAllPortalIds())
            {
                PortalUtils.CreateRocketDirectories(portalid);
                DNNrocketUtils.CreateDefaultRocketRoles(portalid);
            }

            LogUtils.LogSystem("UPGRADE END: " + Version);

            DNNrocketUtils.RecycleApplicationPool();

            return "";
        }
        #endregion

        #endregion

    }


}
