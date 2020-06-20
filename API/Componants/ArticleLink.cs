using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Componants
{
    public class ArticleLink
    {
        public ArticleLink(SimplisityInfo info, string fieldId = "")
        {
            Info = info;
            if (Info == null) Info = new SimplisityInfo();
            FieldId = fieldId;
        }

        public SimplisityInfo Info { get; private set; }
        public string FieldId { get; private set; }

        public string Url
        {
            get
            {
                string rtn;
                if (LinkType == 1)
                {
                    var internallink = Info.GetXmlPropertyInt("genxml/select/internallink" + FieldId);
                    rtn = PagesUtils.GetPageURL(internallink);
                }
                else
                {
                    rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/externallink" + FieldId);
                }
                if (Anchor != "") rtn += "#" + Anchor;
                return rtn;
            }
        }
        public string Name
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/name" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/name" + FieldId);
                return rtn;
            }
        }
        public int LinkType
        {
            get
            {
                return Info.GetXmlPropertyInt("genxml/select/internallink" + FieldId);
            }
        }
        public string Ref
        {
            get
            {
                return Info.GetXmlProperty("genxml/textbox/ref" + FieldId);
            }
        }
        public string Anchor
        {
            get
            {
                return Info.GetXmlProperty("genxml/textbox/anchor" + FieldId);
            }
        }
        public string Target
        {
            get
            {
                if (Info.GetXmlPropertyBool("genxml/checkbox/blank" + FieldId)) return "_blank";
                return "";
            }
        }
    }
}
