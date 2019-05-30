using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Simplisity
{

    public class SimplisityRecord : ICloneable
    {
        public int ItemID { get; set; }
        public int PortalId { get; set; }
        public int ModuleId { get; set; }
        public string TypeCode { get; set; }
        public string GUIDKey { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TextData { get; set; }
        public int XrefItemId { get; set; }
        public int ParentItemId { get; set; }
        public XmlDocument XMLDoc { get; set; }
        public string Lang { get; set; }
        public int UserId { get; set; }
        public int RowCount { get; set; }
        public string EncodingKey { get; set; }
        public int SystemId { get; set; }

        private string _xmlData;

        public SimplisityRecord(
            int ItemID,
            int PortalId,
            int ModuleId,
            string TypeCode,
            string GUIDKey,
            DateTime ModifiedDate,
            string TextData,
            int XrefItemId,
            int ParentItemId,
            string XMLData,
            string Lang,
            int UserId,
            int RowCount,            
            string EncodingKey,
            int SystemId)
        {
            this.ItemID = ItemID;
            this.PortalId = PortalId;
            this.ModuleId = ModuleId;
            this.TypeCode = TypeCode;
            this.GUIDKey = GUIDKey;
            this.ModifiedDate = ModifiedDate;
            this.TextData = TextData;
            this.XrefItemId = XrefItemId;
            this.ParentItemId = ParentItemId;
            this.XMLData = XMLData;
            this.Lang = Lang;
            this.UserId = UserId;
            this.RowCount = RowCount;
            this.EncodingKey = EncodingKey;
            this.SystemId = SystemId;
        }

        public SimplisityRecord(SimplisityInfo info)
        {
            info.RemoveLangRecord();
            this.ItemID = info.ItemID;
            this.PortalId = info.PortalId;
            this.ModuleId = info.ModuleId;
            this.TypeCode = info.TypeCode;
            this.GUIDKey = info.GUIDKey;
            this.ModifiedDate = info.ModifiedDate;
            this.TextData = info.TextData;
            this.XrefItemId = info.XrefItemId;
            this.ParentItemId = info.ParentItemId;
            this.XMLData = info.XMLData;
            this.Lang = info.Lang;
            this.UserId = info.UserId;
            this.RowCount = info.RowCount;
            this.EncodingKey = info.EncodingKey;
            this.SystemId = info.SystemId;
        }

        public SimplisityRecord()
        {
            this.Lang = "en-US"; // we need a langauge for formating data, default to en-US, but the language should be passed when we need formatted date.
            if (XMLDoc == null) XMLData = "<genxml></genxml>"; // if we don;t have anything, create an empty default to stop errors.
        }

        public SimplisityRecord(string lang)
        {
            this.Lang = lang;
            if (XMLDoc == null) XMLData = "<genxml></genxml>"; // if we don;t have anything, create an empty default to stop errors.
        }


        public string XMLData
        {
            get { return _xmlData; }
            set
            {
                XMLDoc = null;
                _xmlData = value;
                try
                {
                    if (!String.IsNullOrEmpty(_xmlData))
                    {
                        XMLDoc = new XmlDocument();
                        XMLDoc.LoadXml(_xmlData);
                    }
                }
                catch (Exception)
                {
                    //trap erorr and don't report. (The XML might be invalid, but we don;t want to stop processing here.)
                    XMLDoc = null;
                }
            }
        }


        public string GetXmlNode(string xpath)
        {
            if (!string.IsNullOrEmpty(_xmlData) & XMLDoc != null)
            {
                try
                {
                    var selectSingleNode = XMLDoc.SelectSingleNode(xpath);
                    if (selectSingleNode != null) return selectSingleNode.InnerXml;
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return "XML READ ERROR";
                }
            }
            return "";
        }

        public void RemoveXmlNode(string xPath)
        {
            var xmlNod = XMLDoc.SelectSingleNode(xPath);
            if (xmlNod != null)
            {
                if (xmlNod.ParentNode != null) xmlNod.ParentNode.RemoveChild(xmlNod);
            }
            XMLData = XMLDoc.OuterXml;
        }

        /// <summary>
        /// Add a XML node to a parent destination 
        /// </summary>
        /// <param name="strXml">source XML</param>
        /// <param name="xPathSource">source xpath</param>
        /// <param name="xPathRootDestination">parent xpath in destination</param>
        public void AddXmlNode(string strXml, string xPathSource, string xPathRootDestination)
        {
            var xmlDocNew = new XmlDocument();
            xmlDocNew.LoadXml(strXml);

            var xmlTarget = XMLDoc.SelectSingleNode(xPathRootDestination);
            if (xmlTarget != null)
            {
                var xmlNod2 = xmlDocNew.SelectSingleNode(xPathSource);
                if (xmlNod2 != null)
                {
                    var newNod = XMLDoc.ImportNode(xmlNod2, true);
                    xmlTarget.AppendChild(newNod);
                    XMLData = XMLDoc.OuterXml;
                }
            }
        }

        /// <summary>
        /// Replace xml node in SimplisityInfo structure
        /// </summary>
        /// <param name="strXml">New XML, must be in NBright Strucutre (genxml/...)</param>
        /// <param name="xPathSource">Source path of the xml, this is for the new node and the old existing node</param>
        /// <param name="xPathRootDestination">parent node to place the new node onto</param>
        /// <param name="addNode">add if the node doesn;t already exists.</param>
        public void ReplaceXmlNode(string strXml, string xPathSource, string xPathRootDestination, bool addNode = true)
        {
            var xmlDocNew = new XmlDocument();
            xmlDocNew.LoadXml(strXml);

            var xmlNod = XMLDoc.SelectSingleNode(xPathSource);
            if (xmlNod != null)
            {
                var xmlNod2 = xmlDocNew.SelectSingleNode(xPathSource);
                if (xmlNod2 != null)
                {
                    var newNod = XMLDoc.ImportNode(xmlNod2, true);
                    var selectSingleNode = XMLDoc.SelectSingleNode(xPathRootDestination);
                    if (selectSingleNode != null)
                    {
                        selectSingleNode.ReplaceChild(newNod, xmlNod);
                        XMLData = XMLDoc.OuterXml;
                    }
                }
            }
            else
            {
                if (addNode) AddXmlNode(strXml, xPathSource, xPathRootDestination);
            }
        }

        /// <summary>
        /// return int data type from XML
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public int GetXmlPropertyInt(string xpath)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    var x = GetGenXmlValueRawFormat(XMLData, xpath);
                    if (GeneralUtils.IsNumeric(x)) return Convert.ToInt32(x);
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// return double data type from XML
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public double GetXmlPropertyDouble(string xpath)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    var x = GetGenXmlValueRawFormat(XMLData, xpath);
                    if (GeneralUtils.IsNumeric(x))
                    {
                        return Convert.ToDouble(x, CultureInfo.GetCultureInfo("en-US"));
                        // double should always be saved as en-US                        
                    }
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// return Bool data type from XML
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public bool GetXmlPropertyBool(string xpath)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    var x = GetGenXmlValueRawFormat(XMLData, xpath);
                    // bool usually stored as "True" "False"
                    if (x.ToLower() == "true") return true;
                    // Test for 1 as true also.
                    if (GeneralUtils.IsNumeric(x))
                    {
                        if (Convert.ToInt32(x) > 0) return true;
                    }
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return false;
                }
            }
            return false;
        }

        public string GetXmlProperty(string xpath, string lang = "")
        {
            if (lang != "") Lang = lang;
            return GetXmlProperty(xpath, false);
        }

        public string GetXmlProperty(string xpath)
        {
            return GetXmlProperty(xpath, false);
        }

        public string GetXmlProperty(string xpath, bool encrypted)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    var output = GetGenXmlValueFormat(XMLData, xpath, Lang);
                    return encrypted ? GeneralUtils.Decrypt(EncodingKey, output) : output;
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return "XML READ ERROR";
                }
            }
            return "";
        }

        /// <summary>
        /// get the data fromthe XML wothout reformatting for numbers or dates. (ISO YYYY-MM-DD for dates , en-US for numbers)
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public string GetXmlPropertyRaw(string xpath)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    return GetGenXmlValueRawFormat(XMLData, xpath);
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return "XML READ ERROR";
                }
            }
            return "";
        }

        public void SetXmlPropertyDouble(string xpath, Double value, int precision = 2)
        {
            SetXmlPropertyDouble(xpath, Math.Round(value, precision).ToString(""));
        }

        public void SetXmlPropertyDouble(string xpath, string value)
        {
            SetXmlProperty(xpath, value, System.TypeCode.Double, false);
        }

        public void SetXmlProperty(string xpath, string value, System.TypeCode DataTyp)
        {
            SetXmlProperty(xpath, value, DataTyp, true, false, false);
        }

        public void SetXmlProperty(string xpath, string value, System.TypeCode DataTyp, bool cdata)
        {
            SetXmlProperty(xpath, value, DataTyp, cdata, false, false);
        }

        public void SetXmlProperty(string xpath, string Value, System.TypeCode DataTyp = System.TypeCode.String, bool cdata = true, bool ignoresecurityfilter = false, bool filterlinks = false)
        {
            if (xpath != "" && !string.IsNullOrEmpty(XMLData))
            {
                if (DataTyp == System.TypeCode.Double)
                {
                    // always save double in en-US format
                    if (GeneralUtils.IsNumeric(Value, Lang))
                    {
                        var dbl = Convert.ToDouble(Value, CultureInfo.GetCultureInfo(Lang));
                        Value = dbl.ToString(CultureInfo.GetCultureInfo("en-US"));
                    }
                }
                if (DataTyp == System.TypeCode.DateTime)
                {
                    if (GeneralUtils.IsDate(Value, Lang))
                    {
                        Value = GeneralUtils.FormatToSave(Value, System.TypeCode.DateTime, Lang);
                    }
                    else
                    {
                        Value = "";
                    }
                }
                XMLData = SetGenXmlValue(XMLData, xpath, Value, cdata, ignoresecurityfilter, filterlinks);

                // do the datatype after the node is created
                if (DataTyp == System.TypeCode.DateTime)
                    XMLData = SetGenXmlValue(XMLData, xpath + "/@datatype", "date", cdata);

                if (DataTyp == System.TypeCode.Double)
                    XMLData = SetGenXmlValue(XMLData, xpath + "/@datatype", "double", cdata);
            }
        }

        public string ToXmlItem(bool withTextData = true)
        {
            // don't use serlization, becuase depending what is in the TextData field could make it fail.
            var xmlOut = new StringBuilder("<item><itemid>" + ItemID.ToString("") + "</itemid><portalid>" + PortalId.ToString("") + "</portalid><moduleid>" + ModuleId.ToString("") + "</moduleid><xrefitemid>" + XrefItemId.ToString("") + "</xrefitemid><parentitemid>" + ParentItemId.ToString("") + "</parentitemid><typecode>" + TypeCode + "</typecode><guidkey>" + GUIDKey + "</guidkey><lang>" + Lang + "</lang><userid>" + UserId.ToString("") + "</userid>" + XMLData);
            if (withTextData && TextData != null)
            {
                xmlOut.Append("<data><![CDATA[" + TextData.Replace("<![CDATA[", "***CDATASTART***").Replace("]]>", "***CDATAEND***") + "]]></data>");
            }
            xmlOut.Append("</item>");

            return xmlOut.ToString();
        }

        public void FromXmlItem(string xmlItem)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlItem);

            //itemid
            var selectSingleNode = xmlDoc.SelectSingleNode("item/itemid");
            if (selectSingleNode != null) ItemID = Convert.ToInt32(selectSingleNode.InnerText);

            //portalid
            selectSingleNode = xmlDoc.SelectSingleNode("item/portalid");
            if (selectSingleNode != null) PortalId = Convert.ToInt32(selectSingleNode.InnerText);

            // moduleid
            selectSingleNode = xmlDoc.SelectSingleNode("item/moduleid");
            if (selectSingleNode != null) ModuleId = Convert.ToInt32(selectSingleNode.InnerText);

            //xrefitemid
            selectSingleNode = xmlDoc.SelectSingleNode("item/xrefitemid");
            if (selectSingleNode != null) XrefItemId = Convert.ToInt32(selectSingleNode.InnerText);

            //parentitemid
            selectSingleNode = xmlDoc.SelectSingleNode("item/parentitemid");
            if (selectSingleNode != null) ParentItemId = Convert.ToInt32(selectSingleNode.InnerText);

            //typecode
            selectSingleNode = xmlDoc.SelectSingleNode("item/typecode");
            if (selectSingleNode != null) TypeCode = selectSingleNode.InnerText;

            //guidkey
            selectSingleNode = xmlDoc.SelectSingleNode("item/guidkey");
            if (selectSingleNode != null) GUIDKey = selectSingleNode.InnerText;

            //XmlData
            selectSingleNode = xmlDoc.SelectSingleNode("item/genxml");
            if (selectSingleNode != null) XMLData = selectSingleNode.OuterXml;

            //TextData
            selectSingleNode = xmlDoc.SelectSingleNode("item/data");
            if (selectSingleNode != null)
                TextData = selectSingleNode.InnerText.Replace("***CDATASTART***", "<![CDATA[")
                    .Replace("***CDATAEND***", "]]>");

            //lang
            selectSingleNode = xmlDoc.SelectSingleNode("item/lang");
            if (selectSingleNode != null) Lang = selectSingleNode.InnerText;

            //userid
            selectSingleNode = xmlDoc.SelectSingleNode("item/userid");
            if ((selectSingleNode != null) && (GeneralUtils.IsNumeric(selectSingleNode.InnerText)))
                UserId = Convert.ToInt32(selectSingleNode.InnerText);

        }


        public Dictionary<String, String> ToDictionary(String xpathroot = "")
        {
            var rtnDictionary = new Dictionary<string, string>();
            if (XMLDoc != null)
            {
                var nods = XMLDoc.SelectNodes("genxml/*");
                if (nods != null)
                {
                    foreach (XmlNode nod in nods)
                    {
                        rtnDictionary = AddToDictionary(rtnDictionary, xpathroot + "genxml/" + nod.Name + "/*");
                    }
                }
            }
            if (!rtnDictionary.ContainsKey("moduleid")) rtnDictionary.Add("moduleid", ModuleId.ToString(""));
            if (!rtnDictionary.ContainsKey("portalid")) rtnDictionary.Add("portalid", PortalId.ToString(""));
            if (!rtnDictionary.ContainsKey("itemid")) rtnDictionary.Add("itemid", ItemID.ToString(""));
            if (!rtnDictionary.ContainsKey("systemid")) rtnDictionary.Add("systemid", SystemId.ToString(""));
            return rtnDictionary;
        }



        public object Clone()
        {
            var obj = (SimplisityInfo)this.MemberwiseClone();
            obj.XMLData = this.XMLData;
            return obj;
        }


        #region "private functions"

        public static string GetGenXmlValueFormat(string dataXml, string xPath, string lang)
        {
            var xmlNod = GetGenXmlNodeData(dataXml, xPath);
            if (xmlNod == null)
            {
                return "";
            }

            if (String.IsNullOrEmpty(lang)) lang = "en-US";

            if (xmlNod.Attributes != null && (xmlNod.Attributes["datatype"] != null))
            {
                switch (xmlNod.Attributes["datatype"].InnerText.ToLower())
                {
                    case "double":
                        return GeneralUtils.FormatToDisplay(xmlNod.InnerText, lang, System.TypeCode.Double, "N");
                    case "date":
                        return GeneralUtils.FormatToDisplay(xmlNod.InnerText, lang, System.TypeCode.DateTime, "d");
                    case "html":
                        return xmlNod.InnerXml;
                    default:
                        var strOut = xmlNod.InnerText;
                        if (strOut.Contains("<![CDATA["))
                        {
                            //convert back cdata marks con verted so it saves OK into XML 
                            strOut = strOut.Replace("**CDATASTART**", "<![CDATA[");
                            strOut = strOut.Replace("**CDATAEND**", "]]>");
                        }
                        return strOut;
                }
            }
            return xmlNod.InnerText;
        }

        private static string GetGenXmlValueRawFormat(string dataXml, string xPath)
        {
            var xmlNod = GetGenXmlNodeData(dataXml, xPath);
            if (xmlNod == null)
            {
                return "";
            }
            return xmlNod.InnerText;
        }

        private static XmlNode GetGenXmlNodeData(string dataXml, string xPath, string xmlRootName = "genxml")
        {
            try
            {
                if (String.IsNullOrEmpty(dataXml)) return null;
                if (String.IsNullOrEmpty(xPath)) return null;

                xPath = GetShortId(xPath);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(dataXml);
                var xmlNod = xmlDoc.SelectSingleNode(xPath);
                // check we don;t have a language node
                if (xmlNod == null)
                {
                    //this leads to confusion.

                    //xmlNod = xmlDoc.SelectSingleNode(xmlRootName + "/lang/" + xPath);
                }
                return xmlNod;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static string GetShortId(string fullId)
        {
            var s = fullId.Split('_');
            if (GeneralUtils.IsNumeric(s[s.GetUpperBound(0)]))
            {
                if (s.GetLength(0) > 1)
                {
                    return s[(s.GetUpperBound(0)) - 1];
                }
                return "";
            }
            return s[s.GetUpperBound(0)];
        }


        private Dictionary<string, string> AddToDictionary(Dictionary<string, string> inpDictionary, string xpath)
        {
            if (XMLDoc != null)
            {
                var nods = XMLDoc.SelectNodes(xpath);
                if (nods != null)
                {
                    foreach (XmlNode nod in nods)
                    {
                        if (inpDictionary.ContainsKey(nod.Name))
                        {
                            inpDictionary[nod.Name] = nod.InnerText; // overwrite same name node
                        }
                        else
                        {
                            inpDictionary.Add(nod.Name, nod.InnerText);
                        }
                        if (nod.Attributes != null && nod.Attributes["selectedtext"] != null)
                        {
                            var textname = nod.Name + "text";
                            if (inpDictionary.ContainsKey(textname))
                            {
                                inpDictionary[textname] = nod.Attributes["selectedtext"].Value;
                                // overwrite same name node
                            }
                            else
                            {
                                inpDictionary.Add(textname, nod.Attributes["selectedtext"].Value);
                            }
                        }
                    }
                }
            }
            return inpDictionary;
        }


        private static string SetGenXmlValue(string dataXml, string xpath, string Value, bool cdata = true, bool ignoresecurityfilter = false, bool filterlinks = false)
        {
            if (ignoresecurityfilter)
            {
                // clear cross scripting if not html field.
                Value = GeneralUtils.FormatDisableScripting(Value, filterlinks);
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(dataXml);
            if (xpath.Contains("@"))
                ReplaceXmLatt(xmlDoc, xpath, Value);
            else
                internalReplaceXmlNode(xmlDoc, xpath, Value, cdata);
            return xmlDoc.OuterXml;
        }

        private static void ReplaceXmLatt(XmlDocument xmlDoc, string xPath, string newValue)
        {
            var nod = xmlDoc.SelectSingleNode(xPath);
            if ((nod != null))
            {
                if (nod.Attributes != null)
                {
                    nod.Attributes["value"].InnerText = newValue;
                }
            }
            else
            {
                var xpatharray = xPath.Split('@');
                if (xpatharray.Length == 2)
                {
                    var attrName = xpatharray[1];
                    var oAtt = xmlDoc.CreateAttribute(attrName);
                    oAtt.Value = newValue;

                    nod = xmlDoc.SelectSingleNode(xpatharray[0].TrimEnd('/'));
                    if (nod != null) nod.Attributes.Append(oAtt);
                }
            }
        }

        private static void internalReplaceXmlNode(XmlDocument xmlDoc, string xPath, string newValue, bool cdata)
        {
            var nod = xmlDoc.SelectSingleNode(xPath);
            if ((nod != null))
            {
                if (newValue == "") cdata = false; //stops invalid "<" char error
                if (cdata)
                {
                    nod.InnerXml = "<![CDATA[" + newValue + "]]>";
                }
                else
                {
                    nod.InnerXml = newValue;
                }
            }
            else
            {
                string[] partsOfXPath = xPath.Trim('/').Split('/');

                // Build full path so we can append.
                var xPathTest = "";
                var xPathPrevious = "";
                var lp = 1;
                foreach (var p in partsOfXPath)
                {
                    if (lp < partsOfXPath.Length)
                    {
                        xPathTest += "/" + p;
                        xPathTest = xPathTest.TrimStart('/');
                        var nodtest = xmlDoc.SelectSingleNode(xPathTest);
                        if ((nodtest == null))
                        {
                            var elemtest = xmlDoc.CreateElement(p);
                            var selectSingleNodeTest = xmlDoc.SelectSingleNode(xPathPrevious);
                            if (selectSingleNodeTest != null)
                            {
                                selectSingleNodeTest.AppendChild(elemtest);
                            }

                        }
                        xPathPrevious = xPathTest;
                    }
                    lp += 1;
                }



                //Create a new node.
                var elem = xmlDoc.CreateElement(partsOfXPath[partsOfXPath.Length - 1]);
                if (cdata)
                {
                    elem.InnerXml = "<![CDATA[" + newValue + "]]>";
                }
                else
                {
                    elem.InnerXml = newValue;
                }

                //Add the node to the document.
                var selectSingleNode = xmlDoc.SelectSingleNode(GeneralUtils.ReplaceLastOccurrence(xPath, partsOfXPath[partsOfXPath.Length - 1], "").TrimEnd('/'));
                if (selectSingleNode != null)
                {
                    selectSingleNode.AppendChild(elem);
                }
            }
        }


        #endregion

    }

}
