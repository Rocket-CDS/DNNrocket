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
            if (Url == "") IsDisabled = true;
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
                    if (internallink == 0) internallink = Info.GetXmlPropertyInt("genxml/lang/genxml/select/internallink" + FieldId);
                    if (internallink == 0) internallink = Info.GetXmlPropertyInt("genxml/select/internallink");
                    if (internallink == 0) internallink = Info.GetXmlPropertyInt("genxml/lang/genxml/select/internallink");
                    if (internallink == 0) return "";
                    rtn = PagesUtils.GetPageURL(internallink);
                }
                else
                {
                    rtn = Info.GetXmlProperty("genxml/textbox/externallink" + FieldId);
                    if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/externallink" + FieldId);
                }
                if (Anchor != "" && rtn != "") rtn += "#" + Anchor;
                return rtn;
            }
        }
        public string Name
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/name" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/name" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/name");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/name");
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
                var rtn = Info.GetXmlProperty("genxml/textbox/ref" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/ref");
                return rtn;
            }
        }
        public string Anchor
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/anchor" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/anchor" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/anchor");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/anchor");
                return rtn;
            }
        }
        public string Target
        {
            get
            {
                if (Info.GetXmlPropertyBool("genxml/checkbox/blank" + FieldId)) return "_blank";
                if (Info.GetXmlPropertyBool("genxml/checkbox/blank")) return "_blank";
                return "_self";
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
        public bool IsDisabled { get; set; }
    }
}
