using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Simplisity
{

    public class SimplisityInfo : SimplisityRecord
    {

        public SimplisityInfo()
        {
            if (XMLDoc == null) XMLData = "<genxml></genxml>"; // if we don;t have anything, create an empty default to stop errors.
        }

        public SimplisityInfo(SimplisityRecord simplisityRecord)
        {
            this.ItemID = simplisityRecord.ItemID;
            this.PortalId = simplisityRecord.PortalId;
            this.ModuleId = simplisityRecord.ModuleId;
            this.TypeCode = simplisityRecord.TypeCode;
            this.GUIDKey = simplisityRecord.GUIDKey;
            this.ModifiedDate = simplisityRecord.ModifiedDate;
            this.TextData = simplisityRecord.TextData;
            this.XrefItemId = simplisityRecord.XrefItemId;
            this.ParentItemId = simplisityRecord.ParentItemId;
            this.XMLData = simplisityRecord.XMLData;
            this.Lang = simplisityRecord.Lang;
            this.UserId = simplisityRecord.UserId;
            this.RowCount = simplisityRecord.RowCount;
            this.EncodingKey = simplisityRecord.EncodingKey;
        }


        public SimplisityRecord GetLangRecord()
        {
            var rtn = (SimplisityRecord)base.Clone();
            rtn.XMLData = GetXmlNode("genxml/lang");
            if (rtn.XMLData == "") rtn.XMLData = "<genxml/>";
            return rtn;
        }

        public string GetLangXml()
        {
            var rtn = GetXmlNode("genxml/lang");
            if (rtn == "") rtn = "<genxml/>";
            return rtn;
        }

        public void SetLangRecord(SimplisityRecord sRecord)
        {
            SetLangXml(sRecord.XMLData);
            if (sRecord.Lang != "")
            {
                base.Lang = sRecord.Lang;
            }
        }

        public void SetLangXml(string strXml)
        {
            if (XMLDoc.SelectSingleNode("genxml/lang") == null)
            {
                SetXmlProperty("genxml/lang", "", System.TypeCode.String, false);
            }
            AddXmlNode(strXml, "genxml", "genxml/lang");
        }

        public void RemoveLangRecord()
        {
            RemoveXmlNode("genxml/lang");
        }


        public List<SimplisityInfo> GetList(string listName)
        {
            var rtnList = new List<SimplisityInfo>();

            if (XMLDoc != null)
            {
                var lp = 1;
                var listRecords = XMLDoc.SelectNodes("genxml/" + listName + "/*");
                foreach (XmlNode i in listRecords)
                {
                    var nbi = new SimplisityInfo();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "LIST";
                    nbi.GUIDKey = listName;
                    nbi.SetLangXml("<genxml>" + GetXmlNode("genxml/lang/genxml/" + listName + "/genxml[" + lp + "]") + "</genxml>");
                    rtnList.Add(nbi);
                    lp += 1;
                }
            }
            return rtnList;
        }


        public SimplisityInfo GetListItem(string listName, string itemkeyxpath, string itemkey)
        {
            if (XMLDoc != null)
            {

                var list = GetList(listName);
                foreach (var i in list)
                {
                    if (itemkey == i.GetXmlProperty(itemkeyxpath))
                    {
                        return i;
                    }
                }
            }
            return null;
        }

        public void AddListRow(string listName, SimplisityInfo sInfo)
        {
            if (XMLDoc != null)
            {
                var xmllangdata = sInfo.GetLangXml();
                sInfo.RemoveLangRecord();
                var xmldata = sInfo.XMLData;

                if (XMLDoc.SelectSingleNode("genxml/" + listName) == null)
                {
                    SetXmlProperty("genxml/" + listName, "", System.TypeCode.String, false);
                }

                AddXmlNode(xmldata, "genxml", "genxml/" + listName);

                if (XMLDoc.SelectSingleNode("genxml/lang") == null)
                {
                    SetXmlProperty("genxml/lang", "", System.TypeCode.String, false);
                }
                if (XMLDoc.SelectSingleNode("genxml/lang/genxml/" + listName) == null)
                {
                    SetXmlProperty("genxml/lang/genxml/" + listName, "", System.TypeCode.String, false);
                }
                AddXmlNode(xmllangdata, "genxml", "genxml/lang/genxml/" + listName);

            }
        }

        public void AddListRow(string listName)
        {
            if (XMLDoc != null)
            {
                if (XMLDoc.SelectSingleNode("genxml/" + listName) == null)
                {
                    SetXmlProperty("genxml/" + listName, "", System.TypeCode.String, false);
                }

                AddXmlNode("<genxml></genxml>", "genxml", "genxml/" + listName);

                if (XMLDoc.SelectSingleNode("genxml/lang") == null)
                {
                    SetXmlProperty("genxml/lang", "", System.TypeCode.String, false);
                }
                if (XMLDoc.SelectSingleNode("genxml/lang/genxml/" + listName) == null)
                {
                    SetXmlProperty("genxml/lang/genxml/" + listName, "", System.TypeCode.String, false);
                }
                AddXmlNode("<genxml></genxml>", "genxml", "genxml/lang/genxml/" + listName);

            }
        }


    }

}
