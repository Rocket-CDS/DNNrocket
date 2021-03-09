using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RocketMod.Components
{
    public class ImportLimpet
    {
        private DNNrocketController _objCtrl;
        private SimplisityInfo _importInfo;
        private int _moduleId;
        private int _oldmoduleId;
        private static RocketInterface _rocketInterface;
        private string _tablename;


        public ImportLimpet(RocketInterface rocketInterface, int portalid, int moduleId, int oldmoduleId, string content)
        {
            _rocketInterface = rocketInterface;
            _importInfo = new SimplisityInfo();
            _importInfo.XMLData = content;
            _moduleId = moduleId;
            _oldmoduleId = oldmoduleId;
            _tablename = _rocketInterface.DatabaseTable;
            if (_tablename == "") _tablename = "DNNrocket";
            _objCtrl = new DNNrocketController();

            var xmlNodList = _importInfo.XMLDoc.SelectNodes("export/entitytype");
            if (xmlNodList != null)
            {
                foreach (XmlNode nod in xmlNodList)
                {
                    DeleteRecords(nod.InnerText, "");
                }
            }

            // get new moduleref 
            var currentModuleParams = new ModuleParams(moduleId);
            var newmoduleref = currentModuleParams.ModuleRef;

            var legacyIdList = new Dictionary<int, int>();
            xmlNodList = _importInfo.XMLDoc.SelectNodes("export/item");
            if (xmlNodList != null)
            {
                foreach (XmlNode xmlNod1 in xmlNodList)
                {
                    var importInfo = new SimplisityInfo();
                    importInfo.FromXmlItem(xmlNod1.OuterXml);

                    var oldItemId = importInfo.ItemID;

                    importInfo.SetXmlProperty("genxml/legacyitemid", oldItemId.ToString());
                    if (importInfo.TypeCode == "MODULEPARAMS") importInfo.SetXmlProperty("genxml/hidden/moduleref", newmoduleref);                    
                    importInfo.ItemID = -1; // new item
                    importInfo.PortalId = portalid;
                    importInfo.ModuleId = moduleId;

                    // change standard guidkey using moduleid.
                    if (importInfo.GUIDKey == "moduleparams*" + _oldmoduleId) importInfo.GUIDKey = "moduleparams*" + moduleId;
                    if (importInfo.GUIDKey == "moduleid" + _oldmoduleId) importInfo.GUIDKey = "moduleid" + moduleId;

                    var newItemId = _objCtrl.Update(importInfo, _tablename);

                    if (!legacyIdList.ContainsKey(oldItemId)) legacyIdList.Add(oldItemId, newItemId);

                }
            }


            // re-line any parent itemid
            xmlNodList = _importInfo.XMLDoc.SelectNodes("export/entitytype");
            if (xmlNodList != null)
            {
                foreach (XmlNode nod in xmlNodList)
                {
                    foreach (var legacyId in legacyIdList)
                    {
                        var dataList = _objCtrl.GetList(-1, _moduleId, nod.InnerText, " and ParentItemId = " + legacyId.Key + " ", "", "", 0, 0, 0, 0, _tablename);
                        foreach (var sInfo in dataList)
                        {
                            sInfo.ParentItemId = legacyId.Value;
                            _objCtrl.Update(sInfo, _tablename);
                        }
                    }
                }
            }

        }

        private void DeleteRecords(string entityTypeCode, string searchFilter)
        {
            var dataList = _objCtrl.GetList(-1, _moduleId, entityTypeCode, searchFilter, "", "", 0, 0, 0, 0, _tablename);
            foreach (var sInfo in dataList)
            {
                _objCtrl.Delete(sInfo.ItemID, _tablename);
            }
        }

    }
}
