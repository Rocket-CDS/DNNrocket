using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class ArticleReview
    {
        public ArticleReview(SimplisityInfo info, string fieldId = "")
        {
            Info = info;
            if (Info == null) Info = new SimplisityInfo();
            FieldId = fieldId;
            if (Info.GetXmlProperty("genxml/hidden/reviewkey") == "")
            {
                var linkkey = GeneralUtils.GetUniqueString();
                Info.SetXmlProperty("genxml/hidden/reviewkey", linkkey);
                ReviewDate = DateTime.Now;
            }
        }

        public SimplisityInfo Info { get; private set; }
        public string FieldId { get; private set; }
        public string ReviewKey
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/hidden/reviewkey");
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
        public string Comment
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/comment" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/comment");
                return rtn;
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
        public DateTime ReviewDate { get { return Info.GetXmlPropertyDate("genxml/textbox/reviewdate"); } set { Info.SetXmlProperty("genxml/textbox/reviewdate", value.ToString("O"), TypeCode.DateTime); } }
    }
}
