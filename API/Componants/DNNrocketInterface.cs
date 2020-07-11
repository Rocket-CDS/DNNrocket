using DNNrocketAPI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocketAPI.Componants
{

    public class DNNrocketInterface
    {
        public DNNrocketInterface(SimplisityInfo systemInfo, string interfaceKey)
        {
            Exists = false;
            Info = new SimplisityInfo();
            if (systemInfo != null)
            {
                SystemKey = systemInfo.GetXmlProperty("genxml/textbox/ctrlkey");
                var interfaceInfo = systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfaceKey);
                if (interfaceInfo != null)
                {
                    Info = interfaceInfo;
                    Exists = true;
                }
            }
        }

        public DNNrocketInterface(SimplisityInfo interfaceInfo)
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
        public string SystemKey { get; set; }
        public bool IsProvider(string providerType)
        {
            return Info.GetXmlPropertyBool("genxml/checkboxlist/providertype/chk[@data='"+ providerType + "']/@value");
        }

        public SimplisityInfo Info { get; }

        public bool Exists { get; }


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
        public string EntityTypeCode
        {
            get { return Info.GetXmlProperty("genxml/textbox/entitytypecode"); }
            set { Info.SetXmlProperty("genxml/textbox/entitytypecode", value.ToString()); }
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

        public string DefaultTheme
        {
            get { return Info.GetXmlProperty("genxml/textbox/defaulttheme"); }
            set { Info.SetXmlProperty("genxml/textbox/defaulttheme", value.ToString()); }
        }
        public string ThemeVersion
        {
            get { return Info.GetXmlProperty("genxml/textbox/themeversion"); }
            set { Info.SetXmlProperty("genxml/textbox/themeversion", value.ToString()); }
        }
        public string DefaultTemplate
        {
            get { return Info.GetXmlProperty("genxml/textbox/defaulttemplate"); }
            set { Info.SetXmlProperty("genxml/textbox/defaulttemplate", value.ToString()); }
        }
        public string DefaultCommand
        {
            get { return Info.GetXmlProperty("genxml/textbox/defaultcommand"); }
            set { Info.SetXmlProperty("genxml/textbox/defaultcommand", value.ToString()); }
        }
        public bool IsActive
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/active"); }
            set { Info.SetXmlProperty("genxml/checkbox/active", value.ToString()); }
        }
        public string DatabaseTable
        {
            get {
                var i = Info.GetXmlProperty("genxml/textbox/databasetable");
                if (i == "") return "DNNrocket";
                return i;
            }
            set {Info.SetXmlProperty("genxml/textbox/databasetable", value.ToString());}
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
                var securityrolesadministrators = Info.GetXmlPropertyInt("genxml/radio/securityrolesadministrators");
                var securityrolesmanager = Info.GetXmlPropertyInt("genxml/radio/securityrolesmanager");
                var securityroleseditor = Info.GetXmlPropertyInt("genxml/radio/securityroleseditor");
                var securityrolesclienteditor = Info.GetXmlPropertyInt("genxml/radio/securityrolesclienteditor");
                var securityrolesregisteredusers = Info.GetXmlPropertyInt("genxml/radio/securityrolesregisteredusers");
                var securityrolessubscribers = Info.GetXmlPropertyInt("genxml/radio/securityrolessubscribers");
                var securityrolesall = Info.GetXmlPropertyInt("genxml/radio/securityrolesall");

                var roleAdministrators = userInfo.IsInRole("Administrators");
                var roleManager = userInfo.IsInRole("Manager") || userInfo.IsInRole("manager") || userInfo.IsInRole("MANAGER");
                var roleEditor = userInfo.IsInRole("Editor") || userInfo.IsInRole("editor") || userInfo.IsInRole("EDITOR");
                var roleClientEditor = userInfo.IsInRole("ClientEditor") || userInfo.IsInRole("clienteditor") || userInfo.IsInRole("CLIENTEDITOR");
                var roleRegisteredUsers = userInfo.IsInRole("Registered Users");
                var roleSubscribers = userInfo.IsInRole("Subscribers");

                // ##### Allow #####  (Do before block, so we allow access if 1 roles has acccess and another does not.)
                if (securityrolesall == 1) return true;
                if (roleAdministrators && securityrolesadministrators == 1) return true;
                if (roleManager && securityrolesmanager == 1) return true;
                if (roleEditor && securityroleseditor == 1) return true;
                if (roleClientEditor && securityrolesclienteditor == 1) return true;
                if (roleRegisteredUsers && securityrolesregisteredusers == 1) return true;
                if (roleSubscribers && securityrolessubscribers == 1) return true;

                // ##### block #####
                if (roleAdministrators && securityrolesadministrators == 2) return false;
                if (roleManager && securityrolesmanager == 2) return false;
                if (roleEditor && securityroleseditor == 2) return false;
                if (roleClientEditor && securityrolesclienteditor == 2) return false;
                if (roleRegisteredUsers && securityrolesregisteredusers == 2) return false;
                if (roleSubscribers && securityrolessubscribers == 2) return false;
                if (securityrolesall == 2) return false; // su only

           }

            return false;
        }

    }

}
