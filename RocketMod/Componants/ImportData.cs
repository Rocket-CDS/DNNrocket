using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RocketMod.Componants
{
    public class ImportData
    {
        private DNNrocketController _objCtrl;
        private SimplisityInfo _importInfo;
        private int _moduleId;
        private static DNNrocketInterface _rocketInterface;
        private string _tablename;

        public ImportData(DNNrocketInterface rocketInterface, int portalid, int moduleId, string content)
        {
            _rocketInterface = rocketInterface;
            _importInfo = new SimplisityInfo();
            _importInfo.XMLData = content;
            _moduleId = moduleId;
            _tablename = _rocketInterface.DatabaseTable;
            if (_tablename == "") _tablename = "DNNrocket";

            var xmlNodList = _importInfo.XMLDoc.SelectNodes("export/entitytype");
            if (xmlNodList != null)
            {
                foreach (XmlNode nod in xmlNodList)
                {
                    DeleteRecords(nod.InnerText, "");
                }
            }

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
                    importInfo.ItemID = -1; // new item
                    importInfo.PortalId = portalid;
                    importInfo.ModuleId = moduleId;
                    var newItemId = _objCtrl.Update(importInfo, _tablename);

                    if (!legacyIdList.ContainsKey(newItemId)) legacyIdList.Add(newItemId, oldItemId);

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
                        var dataList = _objCtrl.GetList(-1, _moduleId, nod.InnerText, " and parentitemid = " + legacyId.Key + " ", "", "", 0, 0, 0, 0, _tablename);
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
