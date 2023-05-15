using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class ArticleDoc
    {
        public ArticleDoc(SimplisityInfo info, string fieldId)
        {
            Info = info;
            FieldId = fieldId;
            if (Info.GetXmlProperty("genxml/hidden/dockey") == "")
            {
                var dockey = GeneralUtils.GetUniqueString();
                Info.SetXmlProperty("genxml/hidden/dockey", dockey);
            }
        }

        public SimplisityInfo Info { get; private set; }
        public string FieldId { get; private set; }
        public string DocKey
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/hidden/dockey");
                return rtn;
            }
        }
        public string MapPath
        {
            get
            {
                return DNNrocketUtils.MapPath(RelPath);
            }
        }
        public string RelPath
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/hidden/documentpath" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/hidden/documentpath" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/hidden/documentpath");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/hidden/documentpath");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/hidden/documentpath" + FieldId, value);
            }
        }
        public string Name
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/documentname" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/documentname");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/documentname" + FieldId, value);
            }
        }
        public bool Hidden
        {
            get
            {
                var rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/documenthidden" + FieldId);
                if (Info.GetXmlProperty("genxml/lang/genxml/checkbox/documenthidden" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/documenthidden");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/checkbox/documenthidden" + FieldId, value.ToString());
            }
        }

    }
}
