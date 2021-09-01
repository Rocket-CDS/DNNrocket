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

namespace DNNrocketAPI.Components
{

    public class DNNrocketModuleController : ModuleSearchBase, IPortable, IUpgradeable
    {

        #region Optional Interfaces

        #region IPortable Members

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
            var xmlOut = "<export>";
            xmlOut += "</export>";
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
            var postInfo = new SimplisityInfo();
            postInfo.XMLData = content;

            var objModInfo = objModCtrl.GetModule(moduleId, Null.NullInteger, true);
            if (objModInfo != null)
            {
                var portalId = objModInfo.PortalID;
                var systemKey = postInfo.GetXmlProperty("export/systemkey");
                var databasetable = postInfo.GetXmlProperty("export/databasetable");

                var systemData = new SystemLimpet(systemKey);
                if (systemData.Exists)
                {
                    foreach (var r in systemData.InterfaceList)
                    {
                        var rocketInterface = r.Value;
                        if (rocketInterface.IsProvider("importmodule"))
                        {
                            if (rocketInterface.Exists)
                            {
                                var paramInfo = new SimplisityInfo();
                                paramInfo.SetXmlProperty("genxml/hidden/moduleid", moduleId.ToString());
                                paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
                                paramInfo.SetXmlProperty("genxml/hidden/databasetable", databasetable);                                
                                var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, postInfo, paramInfo, "", "");
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region ModuleSearchBase

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            var searchDocuments = new List<SearchDocument>();

            if (moduleInfo != null)
            {
                var lastContentModifiedOnDate = moduleInfo.LastContentModifiedOnDate;
                if (lastContentModifiedOnDate == null) lastContentModifiedOnDate = DateTime.Now;

                // "lastContentModifiedOnDate" is the date when content was updated and is updated by using DNNrocketUtils.SynchronizeModule(_moduleid); on Content save.
                if ((lastContentModifiedOnDate.ToUniversalTime() > beginDateUtc && lastContentModifiedOnDate.ToUniversalTime() < DateTime.UtcNow))
                {
                    var portalId = moduleInfo.PortalID;
                    var systemKey = moduleInfo.DesktopModule.ModuleName;
                    var systemData = new SystemLimpet(systemKey);
                    if (systemData.Exists)
                    {
                        foreach (var r in systemData.InterfaceList)
                        {
                            var rocketInterface = r.Value;
                            if (rocketInterface.IsProvider("searchindex"))
                            {
                                if (rocketInterface.Exists)
                                {
                                    var returnDictionary = (Dictionary<string, object>)CacheUtils.GetCache("searchindex_module_" + moduleInfo.ModuleID.ToString());
                                    if (returnDictionary == null)
                                    {
                                        var paramInfo = new SimplisityInfo();
                                        paramInfo.SetXmlProperty("genxml/hidden/moduleid", moduleInfo.ModuleID.ToString());
                                        paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
                                        // We should return ALL languages into the dictionary
                                        returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, new SimplisityInfo(), paramInfo, "", "");
                                        CacheUtils.SetCache("searchindex_module_" + moduleInfo.ModuleID.ToString(), returnDictionary);
                                    }
                                    var description = "";
                                    var body = "";
                                    var moddate = DateTime.Now;
                                    var updatesearch = false;
                                    if (returnDictionary.ContainsKey("description")) description = (string)returnDictionary["description"];
                                    if (returnDictionary.ContainsKey("body")) body = (string)returnDictionary["body"];
                                    if (returnDictionary.ContainsKey("modifieddate"))
                                    {
                                        var strDate = returnDictionary["modifieddate"];
                                        if (GeneralUtils.IsDateInvariantCulture(strDate)) moddate = Convert.ToDateTime(strDate);
                                        updatesearch = true;
                                    }
                                    var searchDoc = new SearchDocument
                                    {
                                        UniqueKey = moduleInfo.ModuleID.ToString(),
                                        PortalId = moduleInfo.PortalID,
                                        Title = moduleInfo.ModuleTitle,
                                        Description = description,
                                        Body = body,
                                        ModifiedTimeUtc = moddate.ToUniversalTime()
                                    };

                                    if (moduleInfo.Terms != null && moduleInfo.Terms.Count > 0)
                                    {
                                        searchDoc.Tags = CollectHierarchicalTags(moduleInfo.Terms);
                                    }

                                    searchDocuments.Add(searchDoc);
                                }
                            }
                        }
                    }
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
                        tags.Add(t.Name);
                        tags.AddRange(collectTagsFunc(t.ChildTerms, new List<string>()));
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

            PortalUtils.CreateRocketDirectories(0);
            
            DNNrocketUtils.CreateDefaultRocketRoles(0);
                        
            var homeTabId = PagesUtils.GetHomePage(0,DNNrocketUtils.GetCurrentCulture());
            if (homeTabId >= 0)
            {
                PagesUtils.AddPageSkin(0, homeTabId, "rocketportal", "rocketportal.ascx");
                ModuleUtils.DeleteAllTabModules(homeTabId);
                PagesUtils.RemovePagePermissions(0, homeTabId, "All Users");
            }

            var cmstabid = PagesUtils.CreatePage(0, "cms");
            PagesUtils.AddPagePermissions(0, cmstabid, "");
            PagesUtils.AddPageSkin(0, cmstabid, "cms", "cms.ascx");


            LogUtils.LogSystem("UPGRADE END: " + Version);


            DNNrocketUtils.RecycleApplicationPool();

            return "";
        }
        #endregion

        #endregion

    }


}
