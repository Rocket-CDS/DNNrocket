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

namespace DNNrocketAPI.Componants
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
            var objModCtrl = new ModuleController();
            var xmlOut = "<export>";

            var objModInfo = objModCtrl.GetModule(ModuleId, Null.NullInteger, true);

            if (objModInfo != null)
            {
                var portalId = objModInfo.PortalID;
                var moduleParams = new ModuleParams(ModuleId);
                var systemInfoData = new SystemInfoData(moduleParams.SystemKey);
                if (systemInfoData.Exists)
                {
                    xmlOut += "<systemkey>" + moduleParams.SystemKey + "</systemkey>";
                    foreach (var r in systemInfoData.InterfaceList)
                    {
                        var rocketInterface = r.Value;
                        if (rocketInterface.IsProvider("exportmodule"))
                        {
                            if (rocketInterface.Exists)
                            {
                                xmlOut += "<databasetable>" + rocketInterface.DatabaseTable + "</databasetable>";
                                var paramInfo = new SimplisityInfo();
                                paramInfo.SetXmlProperty("genxml/hidden/moduleid", ModuleId.ToString());
                                paramInfo.SetXmlProperty("genxml/hidden/portalid", portalId.ToString());
                                var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemInfoData.SystemInfo, rocketInterface, new SimplisityInfo(), paramInfo, "", "");
                                if (returnDictionary.ContainsKey("outputhtml"))
                                {
                                    xmlOut += returnDictionary["outputhtml"];
                                }
                            }
                            else
                            {
                                xmlOut += "<error>No rocketInterface '" + rocketInterface.InterfaceKey + "'</error>";
                            }
                        }
                    }
                }
                else
                {
                    xmlOut += "<error>No systemInfo record found for systemid: " + moduleParams.SystemKey + "</error>";
                }
            }
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

                var systemInfoData = new SystemInfoData(systemKey);
                if (systemInfoData.Exists)
                {
                    foreach (var r in systemInfoData.InterfaceList)
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
                                var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemInfoData.SystemInfo, rocketInterface, postInfo, paramInfo, "", "");
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region ModuleSearchBase

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo modInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();

            return searchDocuments;
        }

        #endregion

        #region IUpgradeable Members
        public string UpgradeModule(string Version)
        {
            return string.Empty;
        }
        #endregion

        #endregion

    }


}
