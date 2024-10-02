using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class ArticleDoc
    {
        public ArticleDoc(SimplisityInfo info, string fieldId)
        {
            Info = info;
            FieldId = fieldId;
            Exists = true;
            if (Info.GetXmlProperty("genxml/hidden/dockey") == "")
            {
                var dockey = GeneralUtils.GetUniqueString();
                Info.SetXmlProperty("genxml/hidden/dockey", dockey);                
            }
            if (RelPath == "") Exists = false;
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
                Info.SetXmlProperty("genxml/hidden/documentpath" + FieldId, "/" + value.TrimStart('/'));
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
                if (rtn == "") rtn = Path.GetFileNameWithoutExtension(FileUtils.RemoveInvalidFileChars(FileName));
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/lang/genxml/textbox/name" + FieldId, value);
            }
        }
        public string FileName
        {
            get
            {
                var rtn = FileUtils.RemoveInvalidFileChars(Info.GetXmlProperty("genxml/textbox/documentname" + FieldId));
                if (rtn == "") rtn = FileUtils.RemoveInvalidFileChars(Info.GetXmlProperty("genxml/textbox/documentname"));
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/textbox/documentname" + FieldId, FileUtils.RemoveInvalidFileChars(value));
            }
        }
        public string Extension
        {
            get
            {
                var rtn = FileUtils.RemoveInvalidFileChars(Info.GetXmlProperty("genxml/hidden/fileextension" + FieldId));
                if (rtn == "") rtn = Path.GetExtension(FileName);
                return rtn;
            }
            set
            {
                Info.SetXmlProperty("genxml/hidden/fileextension" + FieldId, FileUtils.RemoveInvalidFileChars(value));
            }
        }
        public string DownloadName
        {
            get
            {
                var rtn = Path.GetFileNameWithoutExtension(FileUtils.RemoveInvalidFileChars(Name)) + FileUtils.RemoveInvalidFileChars(Extension);
                return rtn;
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
        public bool Exists { get; set; }

    }
}
