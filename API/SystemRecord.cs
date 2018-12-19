using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Simplisity;

namespace DNNrocketAPI
{

    public class SystemRecord
    {
        private SimplisityInfo _sInfo;
        public SystemRecord(SimplisityInfo sInfo)
        {
            _sInfo = sInfo;
        }

        public SystemRecord(int ItemId)
        {
            var objCtrl = new DNNrocketController();
            _sInfo = objCtrl.GetRecord(ItemId);
        }


        public SimplisityInfo Info()
        {
            return _sInfo;
        }

        public SimplisityInfo GetInterface(string interfacekey)
        {
            if (_sInfo.XMLDoc != null)
            {
                var interfaces = _sInfo.XMLDoc.SelectNodes("genxml/interfaces/*");
                foreach (XmlNode i in interfaces)
                {
                    var nbi = new SimplisityInfo();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMINTERFACE";
                    return nbi;
                }
            }
            return null;
        }

        public List<SimplisityInfo> GetInterfaces(string interfacekey = "")
        {
            var rtnList = new List<SimplisityInfo>();

            if (_sInfo.XMLDoc != null)
            {
                var interfaces = _sInfo.XMLDoc.SelectNodes("genxml/interfaces/*");
                foreach (XmlNode i in interfaces)
                {
                    var nbi = new SimplisityInfo();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMINTERFACE";
                    if (interfacekey == "")
                    {
                        rtnList.Add(nbi);
                    }
                    else
                    {
                        if (interfacekey == nbi.GetXmlProperty("genxml/textbox/interfacekey"))
                        {
                            rtnList.Add(nbi);
                        }
                    }
                }
            }
            return rtnList;
        }

        public List<SimplisityInfo> GetIndexFields()
        {
            var rtnList = new List<SimplisityInfo>();

            if (_sInfo.XMLDoc != null)
            {
                var parameters = _sInfo.XMLDoc.SelectNodes("genxml/indexfields/*");
                foreach (XmlNode i in parameters)
                {
                    var nbi = new SimplisityInfo();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMIDXFIELD";
                    rtnList.Add(nbi);
                }
            }

            return rtnList;
        }

        public List<SimplisityInfo> GetSettings()
        {
            var rtnList = new List<SimplisityInfo>();

            if (_sInfo.XMLDoc != null)
            {
                var parameters = _sInfo.XMLDoc.SelectNodes("genxml/settings/*");
                foreach (XmlNode i in parameters)
                {
                    var nbi = new SimplisityInfo();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMSETTING";
                    rtnList.Add(nbi);
                }
            }

            return rtnList;
        }


        public void AddInterface()
        {
            if (GetInterfaces().Count() == 0)
            {
                _sInfo.SetXmlProperty("genxml/interfaces", "");
            }
            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/interfaces");
            objCtrl.Update(_sInfo);
        }

        public void AddIndexField()
        {
            if (GetIndexFields().Count() == 0)
            {
                _sInfo.SetXmlProperty("genxml/indexfields", "");
            }
            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/indexfields");
            objCtrl.Update(_sInfo);
        }

        public void AddSetting()
        {
            if (GetSettings().Count() == 0)
            {
                _sInfo.SetXmlProperty("genxml/settings", "");
            }

            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/settings");
            objCtrl.Update(_sInfo);
        }

        public void UpdateModels(String xmlAjaxData, string editlang, string nodename)
        {
            var modelList = SimplisityUtils.GetSimplisityXmlList(xmlAjaxData, "", editlang);

            var basefields = "";

            // build xml for data records
            var strXml = "<genxml><" + nodename + ">";
            foreach (var modelInfo in modelList)
            {

                // build list of xpath fields that need processing.
                var filedList = SimplisityUtils.GetAllFieldxPaths(modelInfo);
                foreach (var xpath in filedList)
                {
                    basefields += xpath + ",";
                }

                var objInfo = new SimplisityInfo(true);

                var fields = basefields.Split(',');
                foreach (var f in fields.Where(f => f != ""))
                {
                    var datatype = modelInfo.GetXmlProperty(f + "/@datatype");
                    if (datatype == "date")
                        objInfo.SetXmlProperty(f, modelInfo.GetXmlProperty(f), TypeCode.DateTime);
                    else if (datatype == "double")
                        objInfo.SetXmlPropertyDouble(f, modelInfo.GetXmlProperty(f));
                    else if (datatype == "html")
                        objInfo.SetXmlProperty(f, modelInfo.GetXmlPropertyRaw(f));
                    else
                        objInfo.SetXmlProperty(f, modelInfo.GetXmlProperty(f));
                }
                strXml += objInfo.XMLData;
            }
            strXml += "</" + nodename + "></genxml>";

            // replace models xml 
            Info().ReplaceXmlNode(strXml, "genxml/" + nodename, "genxml");

        }

    }

}
