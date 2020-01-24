using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketMod.Componants
{
    public class ExportData
    {
        private ModuleParams _moduleParams;
        private ModuleParams _dataModuleParams;
        private List<string> _documentRelList;
        private List<string> _imageRelList;
        private DNNrocketController _objCtrl;
        private static DNNrocketInterface _rocketInterface;



        public ExportData(DNNrocketInterface rocketInterface, int moduleid, string systemKey)
        {
            _rocketInterface = rocketInterface;
            _moduleParams = new ModuleParams(moduleid, systemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.ModuleIdDataSource, systemKey);
            _objCtrl = new DNNrocketController();
            _documentRelList = new List<string>();
            _imageRelList = new List<string>();
        }


        public List<SimplisityInfo> GetList()
        {
            var rtnList = new List<SimplisityInfo>();
            foreach (var i in GetExportList("MODULEPARAMS", "")) { rtnList.Add(i); }
            foreach (var i in GetExportList("ROCKETMODSETTINGS", "")) { rtnList.Add(i); }
            foreach (var i in GetExportList("ROCKETMODSETTINGSLANG", "")) { rtnList.Add(i); }
            foreach (var i in GetExportList("ROCKETMODFIELDS", "")) { rtnList.Add(i); }
            foreach (var i in GetExportList("ROCKETMODFIELDSLANG", "")) { rtnList.Add(i); }
            foreach (var i in GetExportList("ROCKETMOD", "")) { rtnList.Add(i); }
            foreach (var i in GetExportList("ROCKETMODLANG", "")) { rtnList.Add(i); }
            return rtnList;
        }

        public string GetXml()
        {
            var xmlOut = "";

            xmlOut = "<moduleid>" + _moduleParams.ModuleId + "</moduleid>";

            xmlOut += GetXMLExportRecords("MODULEPARAMS", "");
            xmlOut += GetXMLExportRecords("ROCKETMODSETTINGS", "");
            xmlOut += GetXMLExportRecords("ROCKETMODSETTINGSLANG", "");
            xmlOut += GetXMLExportRecords("ROCKETMODFIELDS", "");
            xmlOut += GetXMLExportRecords("ROCKETMODFIELDSLANG", "");

            if (_dataModuleParams.ModuleId == _moduleParams.ModuleId) // only export if data is in this module.
            {
                // export DATA
                xmlOut += GetXMLExportRecords("ROCKETMOD", "");
                xmlOut += GetXMLExportRecords("ROCKETMODLANG", "");
                // export IMAGES
                xmlOut += "<imagelist>";
                foreach (var i in _imageRelList)
                {
                    xmlOut += "<rel>" + i + "</rel>";
                }
                xmlOut += "</imagelist>";
                // export DOCS
                xmlOut += "<documentlist>";
                foreach (var i in _documentRelList)
                {
                    xmlOut += "<rel>" + i + "</rel>";
                }
                xmlOut += "</documentlist>";
            }

            // export APPTHEME



            return xmlOut;
        }

        private string GetXMLExportRecords(string entityTypeCode, string searchFilter)
        {
            var tablename = _rocketInterface.DatabaseTable;
            if (tablename == "") tablename = "DNNrocket";
            var dataList = _objCtrl.GetList(-1, _moduleParams.ModuleId, entityTypeCode, searchFilter, "", "", 0, 0, 0, 0, tablename);
            var xmlOut = "<entitytype>" + entityTypeCode + "</entitytype>";
            foreach (var sInfo in dataList)
            {
                // get imagelists and doclists
                var lists = sInfo.GetLists();
                foreach (var listname in lists)
                {
                    if (listname.StartsWith("imagelist"))
                    {
                        var fieldname = listname.Substring(9,listname.Length - 9);
                        var list = sInfo.GetList(listname);
                        foreach (var i in list)
                        {
                            var imgRel = i.GetXmlProperty("genxml/hidden/imagepath" + fieldname);
                            if (!_imageRelList.Contains(imgRel)) _imageRelList.Add(imgRel);
                        }
                    }
                    if (listname.StartsWith("documentlist"))
                    {
                        var fieldname = listname.Substring(12, listname.Length - 12);
                        var list = sInfo.GetList(listname);
                        foreach (var i in list)
                        {
                            var docRel = i.GetXmlProperty("genxml/hidden/rel" + fieldname);
                            if (!_documentRelList.Contains(docRel)) _documentRelList.Add(docRel);
                        }

                    }
                }

                xmlOut += sInfo.ToXmlItem();
            }
            return xmlOut;
        }

        private List<SimplisityInfo> GetExportList(string entityTypeCode, string searchFilter)
        {
            var rtnList = new List<SimplisityInfo>();
            var tablename = _rocketInterface.DatabaseTable;
            if (tablename == "") tablename = "DNNrocket";
            var dataList = _objCtrl.GetList(-1, _moduleParams.ModuleId, entityTypeCode, searchFilter, "", "", 0, 0, 0, 0, tablename);
            foreach (var sInfo in dataList)
            {
                rtnList.Add(sInfo);
            }
            return rtnList;
        }

    }
}
