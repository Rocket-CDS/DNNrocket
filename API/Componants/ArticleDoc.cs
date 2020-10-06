using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Componants
{
    public class ArticleDoc
    {
        public ArticleDoc(SimplisityInfo info, string fieldId)
        {
            Info = info;
            FieldId = fieldId;
        }

        public SimplisityInfo Info { get; private set; }
        public string FieldId { get; private set; }

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
                var rtn = Info.GetXmlProperty("genxml/textbox/name" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/name" + FieldId);
                return rtn;
            }
        }
        public bool Hidden
        {
            get
            {
                var rtn = Info.GetXmlPropertyBool("genxml/checkbox/hidden" + FieldId);
                if (Info.GetXmlProperty("genxml/checkbox/hidden" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/hidden" + FieldId);
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/checkbox/hidden" + FieldId, value.ToString());
            }
        }

    }
}
