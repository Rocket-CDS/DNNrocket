using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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
        public int SortOrder { get; set; }

        private string _rootNodeName;  // for speed on reading property

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
            string EncodingKey)
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

            _rootNodeName = "";
            if (XMLDoc == null) XMLDoc = new XmlDocument();
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
            this.SortOrder = info.SortOrder;

            _rootNodeName = "";
            if (XMLDoc == null) XMLDoc = new XmlDocument();
        }

        public SimplisityRecord()
        {
            this.Lang = "en-US"; // we need a langauge for formating data, default to en-US, but the language should be passed when we need formatted date.
            if (XMLDoc == null) XMLDoc = new XmlDocument();
            _rootNodeName = "";
        }
        public SimplisityRecord(string lang)
        {
            this.Lang = lang; // we need a langauge for formating data, default to en-US, but the language should be passed when we need formatted date.
            if (XMLDoc == null) XMLDoc = new XmlDocument();
            _rootNodeName = "";
        }

        public string XMLData
        {
            get
            {
                return XMLDoc.OuterXml;
            }
            set
            {
                try
                {
                    var v = value;
                    if (String.IsNullOrEmpty(v)) v = "<" + RootNodeName + "/>";
                    XMLDoc = new XmlDocument();
                    XMLDoc.LoadXml(v);
                }
                catch (Exception)
                {
                    //trap erorr and don't report. (The XML might be invalid, but we don;t want to stop processing here.)
                    XMLDoc = null;
                }
            }
        }

        public XDocument XDocument
        {
            get
            {
                return XDocument.Load(new XmlNodeReader(XMLDoc));
            }
        }

        public string RootNodeName
        {
            get
            {
                if (_rootNodeName == "")
                {
                    if (XMLDoc == null || XMLDoc.DocumentElement == null) XMLData = "<genxml/>";
                    XmlElement rootNode = XMLDoc.DocumentElement;
                    if (rootNode == null) return "genxml"; // return default
                    _rootNodeName = rootNode.Name;                    
                }
                return _rootNodeName;
            }
            set { _rootNodeName = value; }
        }

        public string GetXmlNode(string xpath)
        {
            if (XMLDoc != null && !string.IsNullOrEmpty(XMLDoc.OuterXml))
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
            if (!String.IsNullOrEmpty(strXml))
            {
                var xmlDocNew = new XmlDocument();
                xmlDocNew.LoadXml(strXml);

                var xmlTarget = XMLDoc.SelectSingleNode(xPathRootDestination);
                if (xmlTarget == null)
                {
                    SetXmlProperty(xPathRootDestination, "", System.TypeCode.String,false);
                    xmlTarget = XMLDoc.SelectSingleNode(xPathRootDestination);
                }
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
        /// return decimal data type from XML.
        /// Slower than double for calculation, but useful for percission like currency.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public decimal GetXmlPropertyDecimal(string xpath)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    var x = GetGenXmlValueRawFormat(XMLData, xpath);
                    if (GeneralUtils.IsNumeric(x))
                    {
                        return Convert.ToDecimal(x, CultureInfo.GetCultureInfo("en-US"));
                        // decimal should always be saved as en-US                        
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
        public DateTime GetXmlPropertyDate(string xpath)
        {
            if (!string.IsNullOrEmpty(XMLData))
            {
                try
                {
                    var x = GetGenXmlValueRawFormat(XMLData, xpath);
                    if (GeneralUtils.IsDate(x, "en-US"))
                    {
                        return Convert.ToDateTime(x, CultureInfo.GetCultureInfo("en-US"));
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                catch (Exception ex)
                {
                    var ms = ex.ToString();
                    return DateTime.MinValue;
                }
            }
            return DateTime.MinValue;
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

        public void SetXmlPropertyDecimal(string xpath, decimal value)
        {
            SetXmlProperty(xpath, value.ToString(), System.TypeCode.Decimal, false);
        }
        public void SetXmlPropertyDecimal(string xpath, string value)
        {
            SetXmlProperty(xpath, value, System.TypeCode.Decimal, false);
        }
        public void SetXmlPropertyDouble(string xpath, double value)
        {
            SetXmlProperty(xpath, value.ToString(CultureInfo.GetCultureInfo("en-US")), System.TypeCode.Double, false);
        }
        public void SetXmlPropertyDouble(string xpath, string value)
        {
            SetXmlProperty(xpath, value, System.TypeCode.Double, false);
        }
        public void SetXmlPropertyInt(string xpath, int value)
        {
            SetXmlProperty(xpath, value.ToString(), System.TypeCode.Int32, false);
        }
        public void SetXmlPropertyInt(string xpath, string value)
        {
            SetXmlProperty(xpath, value, System.TypeCode.Int32, false);
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
            if (xpath != "")
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
                if (DataTyp == System.TypeCode.Decimal)
                {
                    // always save decimal in en-US format
                    if (GeneralUtils.IsNumeric(Value, Lang))
                    {
                        var dec = Convert.ToDecimal(Value, CultureInfo.GetCultureInfo(Lang));
                        Value = dec.ToString(CultureInfo.GetCultureInfo("en-US"));
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
                if (DataTyp == System.TypeCode.Int32)
                {
                     Value = GeneralUtils.FormatToSave(Value, System.TypeCode.Int32);
                }

                try
                {
                    SetGenXmlValue(xpath, Value, cdata, ignoresecurityfilter, filterlinks);

                    // do the datatype after the node is created
                    if (DataTyp == System.TypeCode.DateTime) SetGenXmlValue(xpath + "/@datatype", "date", cdata);
                    if (DataTyp == System.TypeCode.Double) SetGenXmlValue(xpath + "/@datatype", "double", cdata);
                    if (DataTyp == System.TypeCode.Decimal) SetGenXmlValue(xpath + "/@datatype", "decimal", cdata);
                    if (DataTyp == System.TypeCode.Int32) SetGenXmlValue(xpath + "/@datatype", "int", cdata);
                }
                catch
                {
                    // the XMLData may be invalid to the xpath, ignore and do nothing. 
                }

            }
        }

        public string ToXmlItem(bool withTextData = true)
        {
            // don't use serlization, becuase depending what is in the TextData field could make it fail.
            var xmlOut = new StringBuilder("<item><itemid>" + ItemID.ToString("") + "</itemid><portalid>" + PortalId.ToString("") + "</portalid><moduleid>" + ModuleId.ToString("") + "</moduleid><xrefitemid>" + XrefItemId.ToString("") + "</xrefitemid><parentitemid>" + ParentItemId.ToString("") + "</parentitemid><typecode>" + TypeCode + "</typecode><guidkey>" + GUIDKey + "</guidkey><lang>" + Lang + "</lang><userid>" + UserId.ToString("") + "</userid>" + "<sortorder>" + SortOrder.ToString("") + "</sortorder>");
            if (XMLData.StartsWith("<genxml>"))
            {
                xmlOut.Append(XMLData);
            }
            else
            {
                // non standard XML.  Add the dgenxml for import and add a remove sttribute, so it imports correctly in the FromXmlItem method.
                xmlOut.Append("<genxml remove='true'>" + XMLData + "</genxml>");
            }


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
            if (selectSingleNode != null)
            {
                if (selectSingleNode.Attributes["remove"] != null && selectSingleNode.Attributes["remove"].Value  != "")
                    XMLData = selectSingleNode.InnerXml;
                else
                    XMLData = selectSingleNode.OuterXml;
            }

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

            //sortorder
            selectSingleNode = xmlDoc.SelectSingleNode("item/sortorder");
            if ((selectSingleNode != null) && (GeneralUtils.IsNumeric(selectSingleNode.InnerText)))
                SortOrder = Convert.ToInt32(selectSingleNode.InnerText);
        }

        /// <summary>
        /// Get Dictionary of all values on XML. 
        /// Excludes Lists. 
        /// The nodes on the 3rd level will be returned "genxml/mynode/*"
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToDictionary()
        {
            return ToDictionary("");
        }
        public Dictionary<string, string> ToDictionary(string nodegroup)
        {
            var rtnDictionary = new Dictionary<string, string>();
            if (XMLDoc != null)
            {
                var nods = XMLDoc.SelectNodes(RootNodeName + "/*");
                if (nods != null)
                {
                    foreach (XmlNode nod in nods)
                    {
                        if (nod.Attributes["list"] == null && (nod.Name == nodegroup || nodegroup == ""))
                        {
                            rtnDictionary = AddToDictionary(rtnDictionary, RootNodeName + "/" + nod.Name + "/*");
                        }
                    }
                }
            }
            return rtnDictionary;
        }


        public object Clone()
        {
            var obj = (SimplisityRecord)this.MemberwiseClone();
            obj.XMLData = this.XMLData;
            return obj;
        }

        public string PrettyXml()
        {
            var stringBuilder = new StringBuilder();

            if (XMLData != "")
            {
                var element = XElement.Parse(XMLData);

                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineOnAttributes = true;

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    element.Save(xmlWriter);
                }

                return stringBuilder.ToString();
            }
            return "";
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

        private static XmlNode GetGenXmlNodeData(string dataXml, string xPath)
        {
            try
            {
                if (String.IsNullOrEmpty(dataXml)) return null;
                if (String.IsNullOrEmpty(xPath)) return null;

                var xPathShort = GetShortId(xPath);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(dataXml);

                var xmlNod = xmlDoc.SelectSingleNode(xPathShort);                
                if (xmlNod == null) xmlNod = xmlDoc.SelectSingleNode(xPath); // might not be a short id.
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


        protected Dictionary<string, string> AddToDictionary(Dictionary<string, string> inpDictionary, string xpath)
        {
            if (XMLDoc != null)
            {
                var nods = XMLDoc.SelectNodes(xpath);
                if (nods != null)
                {
                    foreach (XmlNode nod in nods)
                    {
                        if (nod.Name != "")
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
            }
            return inpDictionary;
        }


        private void SetGenXmlValue(string xpath, string Value, bool cdata = true, bool ignoresecurityfilter = false, bool filterlinks = false)
        {
            if (ignoresecurityfilter)
            {
                // clear cross scripting if not html field.
                Value = GeneralUtils.FormatDisableScripting(Value, filterlinks);
            }

            if (xpath.Contains("@"))
                ReplaceXmLatt(xpath, Value);
            else
                internalReplaceXmlNode(xpath, Value, cdata);
        }

        private void ReplaceXmLatt(string xPath, string newValue)
        {
            var nod = XMLDoc.SelectSingleNode(xPath);
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
                    var oAtt = XMLDoc.CreateAttribute(attrName);
                    oAtt.Value = newValue;

                    nod = XMLDoc.SelectSingleNode(xpatharray[0].TrimEnd('/'));
                    if (nod != null) nod.Attributes.Append(oAtt);
                }
            }
        }

        private void internalReplaceXmlNode(string xPath, string newValue, bool cdata)
        {
            var nod = XMLDoc.SelectSingleNode(xPath);
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

                // check for root element
                XmlElement rootNode = XMLDoc.DocumentElement;
                // if root does not exist create it, base on the first element of the xml.
                if (rootNode == null) XMLDoc.CreateElement(partsOfXPath[0]);
                // if xpath root does not match document root element, make xpath root same as document root. (To make it valid XmlDocument and avoid multiple root nodes)
                if (partsOfXPath.Length >= 1 && rootNode != null) partsOfXPath[0] = rootNode.Name;

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
                        var nodtest = XMLDoc.SelectSingleNode(xPathTest);
                        if ((nodtest == null))
                        {
                            var elemtest = XMLDoc.CreateElement(p);
                            if (xPathPrevious == "") xPathPrevious = "/";
                            var selectSingleNodeTest = XMLDoc.SelectSingleNode(xPathPrevious);
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
                var elem = XMLDoc.CreateElement(partsOfXPath[partsOfXPath.Length - 1]);
                if (cdata)
                {
                    elem.InnerXml = "<![CDATA[" + newValue + "]]>";
                }
                else
                {
                    elem.InnerXml = newValue;
                }

                //Add the node to the document.
                var selectSingleNode = XMLDoc.SelectSingleNode(GeneralUtils.ReplaceLastOccurrence(xPath, partsOfXPath[partsOfXPath.Length - 1], "").TrimEnd('/'));
                if (selectSingleNode != null)
                {
                    selectSingleNode.AppendChild(elem);
                }
            }
        }


        #endregion

        #region "Lists"

        public List<string> GetRecordLists()
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


        public List<SimplisityRecord> GetRecordList(string listName)
        {
            var rtnList = new List<SimplisityRecord>();

            if (XMLDoc != null)
            {
                var lp = 1;
                var listRecords = XMLDoc.SelectNodes(RootNodeName + "/" + listName + "/*");
                if (listRecords != null)
                {
                    foreach (XmlNode i in listRecords)
                    {
                        var nbi = new SimplisityRecord();
                        nbi.XMLData = i.OuterXml;
                        nbi.TypeCode = "LIST";
                        nbi.GUIDKey = listName;
                        rtnList.Add(nbi);
                        lp += 1;
                    }
                }
            }
            return rtnList;
        }

        public void RemoveRecordList(string listName)
        {
            if (XMLDoc != null)
            {
                RemoveXmlNode(RootNodeName + "/" + listName);
            }
        }
        public void RemoveRecordListItem(string listName, int index)
        {
            if (XMLDoc != null)
            {
                RemoveXmlNode(RootNodeName + "/" + listName + "/*[" + (index + 1) + "]");
            }
        }
        public void RemoveRecordListItem(string listName, string itemkeyxpath, string itemkey)
        {
            if (XMLDoc != null)
            {

                var list = GetRecordList(listName);
                var lp = 1;
                foreach (var i in list)
                {
                    if (itemkey == i.GetXmlProperty(itemkeyxpath))
                    {
                        RemoveXmlNode(RootNodeName + "/" + listName + "/*[" + lp + "]");
                    }
                    lp += 1;
                }
            }
        }
        /// <summary>
        /// Get record list item
        /// </summary>
        /// <param name="listName">Name of list</param>
        /// <param name="index">1 based index</param>
        public SimplisityRecord GetRecordListItem(string listName, int index)
        {
            if (XMLDoc != null)
            {
                var list = GetRecordList(listName);
                if (index > (list.Count - 1)) return new SimplisityRecord();
                return list[index];
            }
            return null;
        }

        public SimplisityRecord GetRecordListItem(string listName, string itemkeyxpath, string itemkey)
        {
            if (XMLDoc != null)
            {

                var list = GetRecordList(listName);
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

        public void AddRecordListItem(string listName, SimplisityRecord sInfo)
        {
            if (XMLDoc != null)
            {
                AddListItem(listName, sInfo.XMLData);
            }
        }

        public void AddListItem(string listName, string xmlData = "", string xpathSource = "/")
        {
            if (XMLDoc != null)
            {
                // get listcount, so we can add a sort value
                var l = GetRecordList(listName);
                var sortcount = l.Count;

                if (XMLDoc.SelectSingleNode(RootNodeName + "/" + listName) == null)
                {
                    SetXmlProperty(RootNodeName + "/" + listName, "", System.TypeCode.String, false);
                    SetXmlProperty(RootNodeName + "/" + listName + "/@list", "true", System.TypeCode.String, false);
                }

                if (xmlData == "") xmlData = "<" + listName + "row></" + listName + "row>";

                AddXmlNode(xmlData,"/*", RootNodeName + "/" + listName);

                SetXmlProperty(RootNodeName + "/" + listName + "/*[position() = last()]/index", sortcount.ToString(), System.TypeCode.String, false);
            }
        }

        #endregion 


    }


}
