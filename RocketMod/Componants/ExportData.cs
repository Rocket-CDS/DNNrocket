using DNNrocketAPI;
using DNNrocketAPI.Componants;
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


        public ExportData(int moduleid, string systemKey)
        {

            _moduleParams = new ModuleParams(moduleid, systemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.DataSourceModId, systemKey);
            _objCtrl = new DNNrocketController();
            _documentRelList = new List<string>();
            _imageRelList = new List<string>();
        }

        public string GetXml()
        {
            var xmlOut = "";

            xmlOut += GetXMLExportRecords("MODULEPARAMS", "");
            xmlOut += GetXMLExportRecords("ROCKETMODSETTINGS", "");
            xmlOut += GetXMLExportRecords("ROCKETMODSETTINGSLANG", "");
            xmlOut += GetXMLExportRecords("ROCKETMODFIELDS", "");
            xmlOut += GetXMLExportRecords("ROCKETMODFIELDSLANG", "");

            if (_dataModuleParams.ModuleId != _moduleParams.ModuleId) // only export if data is linked to module.
            {
                // export DATA
                xmlOut += GetXMLExportRecords("ROCKETMOD", "");
                xmlOut += GetXMLExportRecords("ROCKETMODLANG", "");
                // export IMAGES
                foreach (var i in _imageRelList)
                {
                    xmlOut += "<img>" + i + "</img>";
                }
                // export DOCS
                foreach (var i in _documentRelList)
                {
                    xmlOut += "<doc>" + i + "</doc>";
                }

            }

            // export APPTHEME



            return xmlOut;
        }

        private string GetXMLExportRecords(string entityTypeCode, string searchFilter)
        {
            var dataList = _objCtrl.GetList(-1, _moduleParams.ModuleId, entityTypeCode, searchFilter, "", "", 0, 0, 0, 0, "DNNrocket");
            var xmlOut = "";
            foreach (var sInfo in dataList)
            {
                // get imagelists and doclists
                var lists = sInfo.GetLists();
                foreach (var listname in lists)
                {
                    if (listname.StartsWith("imagelist"))
                    {
                        var fieldname = listname.Substring(10,listname.Length);
                        var list = sInfo.GetList(listname);
                        foreach (var i in list)
                        {
                            var imgRel = i.GetXmlProperty("genxml/hidden/imagepath" + fieldname);
                            if (!_imageRelList.Contains(imgRel)) _imageRelList.Add(imgRel);
                        }
                    }
                    if (listname.StartsWith("documentlist"))
                    {
                        var fieldname = listname.Substring(13, listname.Length);
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

    }
}
