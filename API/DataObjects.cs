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

    public class DNNrocketInterface : SimplisityInfo 
    {
        public SimplisityInfo Info;
        private bool exists;
        public DNNrocketInterface(SimplisityInfo systemInfo, string interfaceKey)
        {
            exists = true;
            if (systemInfo == null)
            {
                exists = false;
            }
            else
            {
                Info = systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfaceKey);
                if (Info == null) exists = false;
            }
        }

        public DNNrocketInterface(SimplisityInfo simplisityInfo)
        {
            exists = true;
            Info = simplisityInfo;
            if (Info == null) exists = false;
        }

        public bool Exists
        {
            get {return exists; }
        }

        public int SystemId
        {
            get { return Info.ModuleId; }
            set { Info.ModuleId = value; }
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



    }

}
