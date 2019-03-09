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
                    base.FromXmlItem(interfaceInfo.ToXmlItem());
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
                base.FromXmlItem(interfaceInfo.ToXmlItem());
            }
        }

        public SimplisityInfo Info
        {
            get {
                var info = new SimplisityInfo();
                info.FromXmlItem(base.ToXmlItem());
                return info;
            }

        }

        public bool Exists { get; }

        
        public string InterfaceKey
        {
            get { return base.GetXmlProperty("genxml/textbox/interfacekey"); }
            set { base.SetXmlProperty("genxml/textbox/interfacekey", value.ToString()); }
        }

        public string EntityTypeCode
        {
            get { return base.GetXmlProperty("genxml/textbox/entitytypecode"); }
            set { base.SetXmlProperty("genxml/textbox/entitytypecode", value.ToString()); }
        }

        public string TemplateRelPath
        {
            get { return base.GetXmlProperty("genxml/textbox/relpath"); }
            set { base.SetXmlProperty("genxml/textbox/relpath", value.ToString()); }
        }

        public string NameSpaceClass
        {
            get { return base.GetXmlProperty("genxml/textbox/namespaceclass"); }
            set { base.SetXmlProperty("genxml/textbox/namespaceclass", value.ToString()); }
        }
        public string Assembly
        {
            get { return base.GetXmlProperty("genxml/textbox/assembly"); }
            set { base.SetXmlProperty("genxml/textbox/assembly", value.ToString()); }
        }

        public string DefaultTheme
        {
            get { return base.GetXmlProperty("genxml/textbox/defaulttheme"); }
            set { base.SetXmlProperty("genxml/textbox/defaulttheme", value.ToString()); }
        }
        public string DefaultTemplate
        {
            get { return base.GetXmlProperty("genxml/textbox/defaulttemplate"); }
            set { base.SetXmlProperty("genxml/textbox/defaulttemplate", value.ToString()); }
        }
        public string DefaultCommand
        {
            get { return base.GetXmlProperty("genxml/textbox/defaultcommand"); }
            set { base.SetXmlProperty("genxml/textbox/defaultcommand", value.ToString()); }
        }
        public bool IsActive
        {
            get { return base.GetXmlPropertyBool("genxml/checkbox/active"); }
            set { base.SetXmlProperty("genxml/checkbox/active", value.ToString()); }
        }



    }

}
