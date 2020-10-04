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
            set
            {
                Info.SetXmlProperty("genxml/hidden/imagepath" + FieldId, value);
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
            set
            {
                Info.SetXmlProperty("genxml/textbox/name" + FieldId, value);
            }
        }
        public string Alt 
        { 
            get 
            { 
                var rtn = Info.GetXmlProperty("genxml/textbox/imagealt" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagealt" + FieldId);
                if (rtn == "") rtn = Name;
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/imagealt" + FieldId, value);
            }
        }
        public string Summary
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/summary" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/summary" + FieldId);
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/summary" + FieldId, value);
            }
        }
        public string Html
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/html" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/html" + FieldId);
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/summary" + FieldId, value);
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
            set
            {
                Info.SetXmlProperty("genxml/textbox/width" + FieldId, value.ToString());
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
            set
            {
                Info.SetXmlProperty("genxml/textbox/height" + FieldId, value.ToString());
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
