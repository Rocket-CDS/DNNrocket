using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Componants
{
    public class ArticleImage
    {
        public ArticleImage(SimplisityInfo info, string fieldId)
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
                var rtn = Info.GetXmlProperty("genxml/hidden/imagepath" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/hidden/imagepath" + FieldId);
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
        public string Alt { get { 
                
                var rtn = Info.GetXmlProperty("genxml/textbox/altproductimage" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/altproductimage" + FieldId);
                return rtn;
            }
        }
        public int Width
        {
            get
            {
                var rtn = Info.GetXmlPropertyInt("genxml/textbox/width" + FieldId);
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/lang/genxml/textbox/width" + FieldId);
                return rtn;
            }
        }
        public int Height
        {
            get
            {
                var rtn = Info.GetXmlPropertyInt("genxml/textbox/height" + FieldId);
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/lang/genxml/textbox/height" + FieldId);
                return rtn;
            }
        }

    }
}
