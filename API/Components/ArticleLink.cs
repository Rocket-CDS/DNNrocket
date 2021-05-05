using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class ArticleLink
    {
        public ArticleLink(SimplisityInfo info, string fieldId = "")
        {
            Info = info;
            if (Info == null) Info = new SimplisityInfo();
            FieldId = fieldId;
            if (Info.GetXmlProperty("genxml/hidden/linkkey") == "")
            {
                var linkkey = GeneralUtils.GetUniqueString();
                Info.SetXmlProperty("genxml/hidden/linkkey", linkkey);
            }
        }

        public SimplisityInfo Info { get; private set; }
        public string FieldId { get; private set; }
        public string LinkKey
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/hidden/linkkey");
                return rtn;
            }
        }
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
                    rtn = Info.GetXmlProperty("genxml/textbox/externallink" + FieldId);
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
                return Info.GetXmlPropertyInt("genxml/radio/linktype" + FieldId);
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
        public bool Hidden
        {
            get
            {
                var rtn = Info.GetXmlPropertyBool("genxml/checkbox/hide" + FieldId);
                if (Info.GetXmlProperty("genxml/checkbox/hide" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/hide" + FieldId);
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/checkbox/hide" + FieldId, value.ToString());
            }
        }
    }
}
