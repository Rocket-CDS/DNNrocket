using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Simplisity
{

    public class SimplisityInfo : SimplisityRecord, ICloneable
    {
        public object CloneInfo()
        {
            var obj = (SimplisityInfo)this.MemberwiseClone();
            obj.XMLData = this.XMLData;
            return obj;
        }

        public SimplisityInfo()
        {
            this.Lang = "en-US"; // default language format
            if (XMLDoc == null) XMLDoc = new XmlDocument();
        }

        public SimplisityInfo(string lang)
        {
            this.Lang = lang;
            if (XMLDoc == null) XMLDoc = new XmlDocument();
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
            this.SortOrder = simplisityRecord.SortOrder;
            if (XMLDoc == null) XMLDoc = new XmlDocument();
        }


        public SimplisityRecord GetLangRecord()
        {
            var rtn = (SimplisityRecord)base.Clone();
            rtn.XMLData = GetXmlNode(RootNodeName + "/lang");
            if (rtn != null && rtn.XMLData == "") rtn.XMLData = "<" + RootNodeName + "/>";
            return rtn;
        }

        public string GetLangXml()
        {
            var rtn = GetXmlNode(RootNodeName + "/lang");
            if (rtn == "") rtn = "<" + RootNodeName + "/>";
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
            if (XMLDoc.SelectSingleNode(RootNodeName + "/lang") == null)
            {
                SetXmlProperty(RootNodeName + "/lang", "", System.TypeCode.String, false);
            }
            AddXmlNode(strXml, "/*", RootNodeName + "/lang");
        }

        public void RemoveLangRecord()
        {
            RemoveXmlNode(RootNodeName + "/lang");
        }

        #region "Lists"

        public List<string> GetLists()
        {
            var rtnList = new List<string>();

            if (XMLDoc != null)
            {
                var lp = 1;
                var listNames = XMLDoc.SelectNodes(RootNodeName + "/*[@list]");
                foreach (XmlNode i in listNames)
                {
                    rtnList.Add(i.Name);
                    lp += 1;
                }
            }
            return rtnList;
        }


        public List<SimplisityInfo> GetList(string listName)
        {
            var rtnList = new List<SimplisityInfo>();

            if (XMLDoc != null)
            {
                var lp = 1;
                var listRecords = XMLDoc.SelectNodes(RootNodeName + "/" + listName + "/*");
                if (listRecords != null)
                {
                    foreach (XmlNode i in listRecords)
                    {
                        var nbi = new SimplisityInfo();
                        nbi.XMLData = i.OuterXml;
                        nbi.TypeCode = "LIST";
                        nbi.GUIDKey = listName;

                        var listXmlNode = "";
                        var listitemref = nbi.GetXmlProperty(RootNodeName + "/hidden/simplisity-listitemref");
                        if (listitemref == "")
                        {
                            listXmlNode = GetXmlNode(RootNodeName + "/lang/" + RootNodeName + "/" + listName + "/*[" + lp + "]");
                        }
                        else
                        {
                            listXmlNode = GetXmlNode(RootNodeName + "/lang/" + RootNodeName + "/" + listName + "/*[hidden/simplisity-listitemreflang='" + listitemref + "']");
                        }

                        nbi.SetLangXml("<" + RootNodeName + ">" + listXmlNode + "</" + RootNodeName + ">");
                        rtnList.Add(nbi);
                        lp += 1;
                    }
                }
            }
            return rtnList;
        }

        public void RemoveList(string listName)
        {
            if (XMLDoc != null)
            {
                RemoveXmlNode(RootNodeName + "/" + listName);
                RemoveXmlNode(RootNodeName + "/lang/" + RootNodeName + "/" + listName);
            }
        }

        public void RemoveListItem(string listName, int index)
        {
            if (XMLDoc != null && index > 0)
            {
                RemoveXmlNode(RootNodeName + "/" + listName + "/*[" + index + "]");
                RemoveXmlNode(RootNodeName + "/lang/" + RootNodeName + "/" + listName + "/*[" + index + "]");
            }
        }

        public void RemoveListItem(string listName, string itemkeyxpath, string itemkey)
        {
            if (XMLDoc != null)
            {
                var lp = 1;
                var list = GetList(listName);
                foreach (var i in list)
                {
                    if (itemkey == i.GetXmlProperty(itemkeyxpath))
                    {
                        RemoveListItem(listName, lp);
                    }
                    lp += 1;
                }
            }
        }

        public SimplisityInfo GetListItem(string listName, int index)
        {
            if (XMLDoc != null)
            {
                var list = GetList(listName);
                if (index > (list.Count - 1)) return new SimplisityInfo();
                return list[index];
            }
            return null;
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

        public void AddListItem(string listName, SimplisityInfo sInfo)
        {
            if (XMLDoc != null)
            {
                var xmllangdata = sInfo.GetLangXml();
                sInfo.RemoveLangRecord();
                var xmldata = sInfo.XMLData;

                AddListItem(listName, xmldata, xmllangdata);

            }
        }

        public void AddListItem(string listName, string xmldata = "", string xmllangdata = "")
        {
            if (XMLDoc != null)
            {
                if (xmldata == "") xmldata = "<" + listName + "row></" + listName + "row>";
                if (xmllangdata == "") xmllangdata = "<" + listName + "row></" + listName + "row>";

                // get listcount, so we can add a sort value
                var l = GetList(listName);
                var sortcount = l.Count + 1;

                if (XMLDoc.SelectSingleNode(RootNodeName + "/" + listName) == null)
                {
                    SetXmlProperty(RootNodeName + "/" + listName, "", System.TypeCode.String, false);
                }

                AddXmlNode(xmldata, "/*", RootNodeName + "/" + listName);

                SetXmlProperty(RootNodeName + "/" + listName + "/*[last()]/index", sortcount.ToString(), System.TypeCode.String, false);

                if (XMLDoc.SelectSingleNode(RootNodeName + "/lang") == null)
                {
                    SetXmlProperty(RootNodeName + "/lang", "", System.TypeCode.String, false);
                    SetXmlProperty(RootNodeName + "/lang/" + RootNodeName, "", System.TypeCode.String, false);
                }
                if (XMLDoc.SelectSingleNode(RootNodeName + "/lang/" + RootNodeName + "/" + listName) == null)
                {
                    SetXmlProperty(RootNodeName + "/lang/" + RootNodeName + "/" + listName, "", System.TypeCode.String, false);
                }

                AddXmlNode(xmllangdata, "/*", RootNodeName + "/lang/" + RootNodeName + "/" + listName);

            }
        }

        /// <summary>
        /// Get Dictionary of all values on XML. 
        /// For both Neutral and Language data.  
        /// Excludes Lists.  
        /// The nodes on the 3rd level will be returned "genxml/mynode/*"
        /// </summary>
        /// <returns></returns>
        public new Dictionary<string, string> ToDictionary()
        {
            var rtnDictionary = new Dictionary<string, string>();
            if (XMLDoc != null)
            {
                var nods = XMLDoc.SelectNodes(RootNodeName + "/*");
                if (nods != null)
                {
                    foreach (XmlNode nod in nods)
                    {
                        if (nod.Attributes["list"] == null)
                        {
                            rtnDictionary = AddToDictionary(rtnDictionary, RootNodeName + "/" + nod.Name + "/*");
                        }
                    }                
                }

                var recLang = GetLangRecord();
                var l = recLang.ToDictionary();
                foreach (var d in l)
                {
                    rtnDictionary.Add(d.Key,d.Value);
                }
            }
            return rtnDictionary;
        }


        #endregion 

    }

}
