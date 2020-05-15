using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Simplisity;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using RazorEngine;
using System.Security.Cryptography;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using System.Xml;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Modules;
using System.Net;
using System.IO;
using DotNetNuke.Common.Lists;
using DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Users.Membership;
using System.Globalization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Services.Mail;
using DotNetNuke.Common;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Roles;

namespace DNNrocketAPI.Componants
{
    public static class PortalUtils
    {
        public static int GetCurrentPortalId()
        {
            return DNNrocketUtils.GetPortalId();
        }
        public static List<SimplisityRecord> GetAllPortals()
        {
            var rtnList = DNNrocketUtils.GetAllPortalRecords();
            return rtnList;
        }
        public static int CreatePortal(string portalName, string strPortalAlias)
        {
            // ************THIS DOES NOT WORK************
            // we seem to be missing some data format.

            var portalId = -1;
            //var portalSettings = DNNrocketUtils.GetCurrentPortalSettings();
            //var serverPath = "";
            //var childPath = "";
            //var description = "RocketECommerce";
            //var keyWords = "";
            //var homeDirectory = "Portals/[PortalID]";
            //var template = new PortalController.PortalTemplateInfo("Blank Website.template", DNNrocketUtils.GetCurrentCulture());
            //var isChild = false;

            ////Create Portal
            //var portalId = PortalController.Instance.CreatePortal(portalName,
            //                                         UserUtils.GetCurrentUserId(),
            //                                         description,
            //                                         keyWords,
            //                                         template,
            //                                         homeDirectory,
            //                                         strPortalAlias,
            //                                         serverPath,
            //                                         serverPath + childPath,
            //                                         isChild);
            return portalId;
        }

    }
}
