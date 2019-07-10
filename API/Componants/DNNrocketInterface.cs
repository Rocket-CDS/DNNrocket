using DNNrocketAPI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNNrocketAPI
{

    public class DNNrocketInterface  
    {
        public DNNrocketInterface(SimplisityInfo systemInfo, string interfaceKey)
        {
            Exists = true;
            if (systemInfo == null)
            {
                Exists = false;
            }
            else
            {
                var interfaceInfo = systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfaceKey);
                if (interfaceInfo == null)
                {
                    Exists = false;
                }
                else
                {
                    Info = interfaceInfo;
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

        public SimplisityInfo Info { get; }

        public bool Exists { get; }

        
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
        public bool IsCached
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/cache"); }
            set { Info.SetXmlProperty("genxml/checkbox/cache", value.ToString()); }
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


    }

}
