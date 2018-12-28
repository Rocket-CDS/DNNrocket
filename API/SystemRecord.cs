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
        private SimplisityRecord _sInfo;

        public SystemRecord(SimplisityRecord sInfo)
        {
            _sInfo = sInfo;
        }

        public SystemRecord(int ItemId)
        {
            var objCtrl = new DNNrocketController();
            _sInfo = objCtrl.GetRecord(ItemId);
        }


        public SimplisityRecord Info()
        {
            return _sInfo;
        }

        public SimplisityRecord GetInterface(string interfacekey)
        {
            if (_sInfo.XMLDoc != null)
            {
                var interfaces = _sInfo.XMLDoc.SelectNodes("genxml/interfaces/*");
                foreach (XmlNode i in interfaces)
                {
                    var sI = new SimplisityRecord();
                    sI.XMLData = i.OuterXml;
                    if (interfacekey == sI.GetXmlProperty("genxml/textbox/interfacekey"))
                    {
                        var nbi = new SimplisityRecord();
                        nbi.XMLData = i.OuterXml;
                        nbi.TypeCode = "SYSTEMINTERFACE";
                        return nbi;
                    }
                }
            }
            return null;
        }

        public List<SimplisityRecord> GetInterfaces(string interfacekey = "")
        {
            var rtnList = new List<SimplisityRecord>();

            if (_sInfo.XMLDoc != null)
            {
                var interfaces = _sInfo.XMLDoc.SelectNodes("genxml/interfaces/*");
                foreach (XmlNode i in interfaces)
                {
                    var nbi = new SimplisityRecord();
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

        public List<SimplisityRecord> GetIndexFields()
        {
            var rtnList = new List<SimplisityRecord>();

            if (_sInfo.XMLDoc != null)
            {
                var parameters = _sInfo.XMLDoc.SelectNodes("genxml/indexfields/*");
                foreach (XmlNode i in parameters)
                {
                    var nbi = new SimplisityRecord();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMIDXFIELD";
                    rtnList.Add(nbi);
                }
            }

            return rtnList;
        }

        public List<SimplisityRecord> GetSettings()
        {
            var rtnList = new List<SimplisityRecord>();

            if (_sInfo.XMLDoc != null)
            {
                var parameters = _sInfo.XMLDoc.SelectNodes("genxml/settings/*");
                foreach (XmlNode i in parameters)
                {
                    var nbi = new SimplisityRecord();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMSETTING";
                    rtnList.Add(nbi);
                }
            }

            return rtnList;
        }

        public List<SimplisityRecord> GetGroups()
        {
            var rtnList = new List<SimplisityRecord>();

            if (_sInfo.XMLDoc != null)
            {
                var parameters = _sInfo.XMLDoc.SelectNodes("genxml/groups/*");
                foreach (XmlNode i in parameters)
                {
                    var nbi = new SimplisityRecord();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMGROUP";
                    rtnList.Add(nbi);
                }
            }

            return rtnList;
        }

        public List<SimplisityRecord> GetProvTypes()
        {
            var rtnList = new List<SimplisityRecord>();

            if (_sInfo.XMLDoc != null)
            {
                var parameters = _sInfo.XMLDoc.SelectNodes("genxml/provtypes/*");
                foreach (XmlNode i in parameters)
                {
                    var nbi = new SimplisityRecord();
                    nbi.XMLData = i.OuterXml;
                    nbi.TypeCode = "SYSTEMPROVTYPE";
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

        public void AddGroup()
        {
            if (GetGroups().Count() == 0)
            {
                _sInfo.SetXmlProperty("genxml/groups", "");
            }

            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/groups");
            objCtrl.Update(_sInfo);
        }

        public void AddProvType()
        {
            if (GetProvTypes().Count() == 0)
            {
                _sInfo.SetXmlProperty("genxml/provtypes", "");
            }

            var objCtrl = new DNNrocketController();
            _sInfo.AddXmlNode("<genxml></genxml>", "genxml", "genxml/provtypes");
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

                var objInfo = new SimplisityRecord();

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
