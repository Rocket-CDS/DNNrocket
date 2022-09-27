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
                    var internallink = Info.GetXmlPropertyInt("genxml/select/linkinternallink" + FieldId);
                    if (internallink == 0) internallink = Info.GetXmlPropertyInt("genxml/lang/genxml/select/linkinternallink" + FieldId);
                    if (internallink == 0) internallink = Info.GetXmlPropertyInt("genxml/select/linkinternallink");
                    if (internallink == 0) internallink = Info.GetXmlPropertyInt("genxml/lang/genxml/select/linkinternallink");
                    rtn = PagesUtils.GetPageURL(internallink);
                }
                else
                {
                    rtn = Info.GetXmlProperty("genxml/textbox/linkexternallink" + FieldId);
                    if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/linkexternallink" + FieldId);
                }
                if (Anchor != "") rtn += "#" + Anchor;
                return rtn;
            }
        }
        public string Name
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/linkname" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/linkname" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/linkname");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/linkname");
                return rtn;
            }
        }
        public int LinkType
        {
            get
            {
                var rtn = Info.GetXmlPropertyInt("genxml/radio/linktype" + FieldId);
                if (Info.GetXmlProperty("genxml/radio/linktype" + FieldId) == "") rtn = Info.GetXmlPropertyInt("genxml/radio/linktype");
                return rtn;
            }
        }
        public string Ref
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/linkref" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/linkref");
                return rtn;
            }
        }
        public string Anchor
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/linkanchor" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/linkanchor");
                return rtn;
            }
        }
        public string Target
        {
            get
            {
                if (Info.GetXmlPropertyBool("genxml/checkbox/linkblank" + FieldId)) return "_blank";
                if (Info.GetXmlPropertyBool("genxml/checkbox/linkblank")) return "_blank";
                return "";
            }
        }
        public bool Hidden
        {
            get
            {
                var rtn = Info.GetXmlPropertyBool("genxml/checkbox/linkhide" + FieldId);
                if (Info.GetXmlProperty("genxml/checkbox/linkhide" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/linkhide" + FieldId);
                if (Info.GetXmlProperty("genxml/lang/genxml/checkbox/linkhide" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/checkbox/linkhide");
                if (Info.GetXmlProperty("genxml/checkbox/linkhide") == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/linkhide");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/checkbox/linkhide" + FieldId, value.ToString());
            }
        }
    }
}
