using DNNrocketAPI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocketAPI.Components
{

    public class RocketInterface
    {
        public RocketInterface(SimplisityInfo systemInfo, string interfaceKey)
        {
            Exists = false;
            Info = new SimplisityInfo();
            if (systemInfo != null)
            {
                var providerDataInfo = systemInfo.GetListItem("providerdata", "genxml/textbox/interfacekey", interfaceKey);
                if (providerDataInfo != null)
                {
                    Info = providerDataInfo;
                    Exists = true;
                }
                if (!Exists)
                {
                    // if we do not have a provider for the API, then look for a UI interface.
                    var interfaceInfo = systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfaceKey);
                    if (interfaceInfo != null)
                    {
                        Info = interfaceInfo;
                        Exists = true;
                    }
                }
            }
        }

        public RocketInterface(SimplisityInfo interfaceInfo)
        {
            Exists = true;
            if (interfaceInfo == null)
            {
                Exists = false;
            }
            else
            {
                Info = interfaceInfo;
            }
        }
        public bool IsProvider(string providerType)
        {
            return (Info.GetXmlProperty("genxml/providertype") == providerType) ;
        }

        /// <summary>
        /// Sorts a plugin list by the "genxml/config/sortorder" value stored against each plugin in the
        /// supplied portal "plugins" list.  Kept as a method here (rather than inline Razor) because
        /// multi-line chained LINQ expressions in .cshtml files are not reliably parsed by the Razor host.
        /// </summary>
        public static List<RocketInterface> GetSortedPluginList(List<RocketInterface> pluginList, SimplisityInfo info)
        {
            return pluginList.OrderBy(o => info.GetListItem("plugins", "genxml/hidden/pluginkey", o.InterfaceKey)?.GetXmlPropertyInt("genxml/config/sortorder") ?? 0).ToList();
        }

        public SimplisityInfo Info { get; }

        public bool Exists { get; }

        public string ProviderType
        {
            get { return Info.GetXmlProperty("genxml/providertype"); }
            set { Info.SetXmlProperty("genxml/providertype", value.ToString()); }
        }

        public string InterfaceIcon
        {
            get { return Info.GetXmlProperty("genxml/textbox/interfaceicon"); }
            set { Info.SetXmlProperty("genxml/textbox/interfaceicon", value.ToString()); }
        }
        public string InterfaceKey
        {
            get { return Info.GetXmlProperty("genxml/textbox/interfacekey"); }
            set { Info.SetXmlProperty("genxml/textbox/interfacekey", value.ToString()); }
        }
        public string OnSystemKey
        {
            get { return Info.GetXmlProperty("genxml/textbox/onsystemkey"); }
            set { Info.SetXmlProperty("genxml/textbox/onsystemkey", value.ToString()); }
        }
        public string TemplateRelPath
        {
            get { return Info.GetXmlProperty("genxml/textbox/relpath"); }
            set { Info.SetXmlProperty("genxml/textbox/relpath", value.ToString()); }
        }
        public string NameSpaceClass
        {
            get { return Info.GetXmlProperty("genxml/textbox/namespaceclass"); }
            set { Info.SetXmlProperty("genxml/textbox/namespaceclass", value.ToString()); }
        }
        public string Assembly
        {
            get { return Info.GetXmlProperty("genxml/textbox/assembly"); }
            set { Info.SetXmlProperty("genxml/textbox/assembly", value.ToString()); }
        }
        public string DefaultTemplate
        {
            get { return "view.cshtml"; }
        }
        public string DefaultCommand
        {
            get { return Info.GetXmlProperty("genxml/textbox/defaultcommand"); }
            set { Info.SetXmlProperty("genxml/textbox/defaultcommand", value.ToString()); }
        }
        public string GroupRef
        {
            get { return Info.GetXmlProperty("genxml/dropdownlist/group"); }
            set { Info.SetXmlProperty("genxml/dropdownlist/group", value.ToString()); }
        }
        public bool IsActive
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/active"); }
            set { Info.SetXmlProperty("genxml/checkbox/active", value.ToString()); }
        }
        public bool IsOnMenu
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/onmenu"); }
            set { Info.SetXmlProperty("genxml/checkbox/onmenu", value.ToString()); }
        }

        public bool SecurityCheckUser(int portalId, int userid)
        {
            var userInfo = UserController.Instance.GetUserById(portalId, userid);
            var validUser = false;
            if (userInfo != null && userInfo.UserID > 0 && !userInfo.IsDeleted && userInfo.Membership.Approved)
            {
                validUser = true;
            }

            if (!validUser) return false;
            if (userInfo.IsSuperUser) return true; //always allow access to su

            if (Exists)
            {
                var securityrolesall = Info.GetXmlPropertyInt("genxml/radio/securityrolesall");

                if (securityrolesall == 1) return true;

                var userRoleNames = GetUserRoleNames(userInfo);

                // ##### Force False #####
                foreach (var roleName in userRoleNames)
                {
                    var setting = GetSecurityRoleSetting(roleName);
                    if (setting == 3) return false;
                }

                // ##### Allow #####  (Do before block, so we allow access if 1 role has access and another does not.)
                foreach (var roleName in userRoleNames)
                {
                    var setting = GetSecurityRoleSetting(roleName);
                    if (setting == 1) return true;
                }

                // ##### block #####
                foreach (var roleName in userRoleNames)
                {
                    var setting = GetSecurityRoleSetting(roleName);
                    if (setting == 2 || setting == 3) return false;
                }

                if (securityrolesall == 2) return false; // su only
            }

            return false;
        }

        /// <summary>
        /// Returns the distinct role names assigned to the user (from DNN), used to dynamically
        /// resolve matching "genxml/radio/securityroles{rolename}" settings in the .rule XML.
        /// </summary>
        private static IEnumerable<string> GetUserRoleNames(UserInfo userInfo)
        {
            if (userInfo?.Roles == null) return Enumerable.Empty<string>();
            return userInfo.Roles.Where(r => !string.IsNullOrEmpty(r)).Distinct(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Looks up the "genxml/radio/securityroles{key}" setting for a given DNN role name.
        /// The xml key is built by stripping spaces from the role name and lower-casing it,
        /// e.g. "Registered Users" -> "securityrolesregisteredusers", "Manager" -> "securityrolesmanager".
        /// This allows new roles (e.g. "Owner" -> "securityrolesowner") to work automatically,
        /// without recompiling, as long as the .rule XML defines a matching radio setting.
        /// </summary>
        private int GetSecurityRoleSetting(string roleName)
        {
            var normalizedKey = "securityroles" + roleName.Replace(" ", string.Empty).ToLowerInvariant();
            return Info.GetXmlPropertyInt("genxml/radio/" + normalizedKey);
        }

    }

}
