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

        public SimplisityInfo Info()
        {
            return _sInfo;
        }

        public List<SimplisityInfo> GetInterfaces()
        {
            var rtnList = new List<SimplisityInfo>();

            var interfaces = _sInfo.XMLDoc.SelectNodes("genxml/interfaces/*");
            foreach (XmlNode i in interfaces)
            {
                var nbi = new SimplisityInfo();
                nbi.XMLData = i.OuterXml;
                nbi.TypeCode = "SYSTEMINTERFACE";
                rtnList.Add(nbi);
            }
            return rtnList;
        }

        public List<SimplisityInfo> GetParameters()
        {
            var rtnList = new List<SimplisityInfo>();

            var parameters = _sInfo.XMLDoc.SelectNodes("genxml/parameters/*");
            foreach (XmlNode i in parameters)
            {
                var nbi = new SimplisityInfo();
                nbi.XMLData = i.OuterXml;
                nbi.TypeCode = "SYSTEMPARAM";
                rtnList.Add(nbi);
            }
            return rtnList;
        }

        public void AddInterface()
        {
            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/interfaces");
            objCtrl.Update(_sInfo);
        }

        public void AddParameter()
        {
            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/parameters");
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

                var objInfo = new SimplisityInfo();

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
