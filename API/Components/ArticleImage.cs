using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class ArticleImage
    {
        /// <summary>
        /// Create an Article Image Data class
        /// </summary>
        /// <param name="info">The SimplisityInfo class that contains the image data  </param>
        /// <param name="fieldId">The field name.  This can be empty, if only 1 image field exists on the page.</param>
        public ArticleImage(SimplisityInfo info, string fieldId = "")
        {
            Info = info;
            FieldId = fieldId;
            if (Info.GetXmlProperty("genxml/hidden/imagekey") == "")
            {
                var imagekey = GeneralUtils.GetUniqueString();
                Info.SetXmlProperty("genxml/hidden/imagekey", imagekey);
            }
        }

        public SimplisityInfo Info { get; private set; }
        public string FieldId { get; private set; }
        public string ImageKey
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/hidden/imagekey");
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
                var rtn = Info.GetXmlProperty("genxml/hidden/imagepath" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/hidden/imagepath" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/hidden/imagepath");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/hidden/imagepath");
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
                var rtn = Info.GetXmlProperty("genxml/textbox/imagename" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagename" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/imagename");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagename");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/imagename" + FieldId, value);
            }
        }
        public string Alt 
        { 
            get 
            { 
                var rtn = Info.GetXmlProperty("genxml/textbox/imagealt" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagealt" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/imagealt");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagealt");
                if (rtn == "") rtn = Name;
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/imagealt" + FieldId, value);
            }
        }
        public string Summary
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/imagesummary" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagesummary" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/imagesummary");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagesummary");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/imagesummary" + FieldId, value);
            }
        }
        public string Html
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/imagehtml" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagehtml" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/imagehtml");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/imagehtml");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/imagehtml" + FieldId, value);
            }
        }
        public string Url
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/url" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/url" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/url");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/url");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/url" + FieldId, value);
            }
        }
        public string UrlText
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/textbox/urltext" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/urltext" + FieldId);
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/urltext");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/urltext");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/urltext" + FieldId, value);
            }
        }
        public int Width
        {
            get
            {
                var rtn = Info.GetXmlPropertyInt("genxml/textbox/imagewidth" + FieldId);
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/lang/genxml/textbox/imagewidth" + FieldId);
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/textbox/imagewidth");
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/lang/genxml/textbox/imagewidth");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/imagewidth" + FieldId, value.ToString());
            }
        }
        public int Height
        {
            get
            {
                var rtn = Info.GetXmlPropertyInt("genxml/textbox/imageheight" + FieldId);
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/lang/genxml/textbox/imageheight" + FieldId);
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/textbox/imageheight");
                if (rtn == 0) rtn = Info.GetXmlPropertyInt("genxml/lang/genxml/textbox/imageheight");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/imageheight" + FieldId, value.ToString());
            }
        }
        public bool Hidden
        {
            get
            {
                var rtn = Info.GetXmlPropertyBool("genxml/checkbox/imagehidden" + FieldId);
                if (Info.GetXmlProperty("genxml/checkbox/imagehidden" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/imagehidden" + FieldId);
                if (Info.GetXmlProperty("genxml/lang/genxml/checkbox/imagehidden" + FieldId) == "") rtn = Info.GetXmlPropertyBool("genxml/checkbox/imagehidden");
                if (Info.GetXmlProperty("genxml/checkbox/imagehidden") == "") rtn = Info.GetXmlPropertyBool("genxml/lang/genxml/checkbox/imagehidden");
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/checkbox/imagehidden" + FieldId, value.ToString());
            }
        }

    }
}
