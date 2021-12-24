using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rocket.AppThemes.Components
{

    public class OrganisationLimpet
    {
        private const string _entityTypeCode = "ORGANISATIONS";
        private const string _listName = "organisations";
        private const string _tableName = "DNNrocket";
        private DNNrocketController _objCtrl;

        public SimplisityRecord Record;

        public OrganisationLimpet()
        {
            _objCtrl = new DNNrocketController();
            Record = _objCtrl.GetRecordByGuidKey(-1, -1, _entityTypeCode, _entityTypeCode, "", _tableName);
            if (Record == null)
            {
                Record = new SimplisityRecord();
                Record.GUIDKey = _entityTypeCode;
                Record.TypeCode = _entityTypeCode;
                Record.ModuleId = -1;
                Record.PortalId = -1;
            }
        }
        public void Delete()
        {
            if (Record.ItemID > 0)
            {
                _objCtrl.Delete(Record.ItemID);
                Record = null;
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
            Record.XMLData = postInfo.XMLData;
            Update();
        }

        public void Update()
        {
            Record.ItemID = _objCtrl.Update(Record, _tableName);
        }
        public void AddRow()
        {
            Record.AddRecordListItem(_listName, new SimplisityRecord());
            Update();
        }
        public string ExportData(bool withTextData = false)
        {
            var xmlOut = "<root>";
            xmlOut += Record.ToXmlItem(withTextData);
            xmlOut += "</root>";

            return xmlOut;
        }

        public void ImportData(string XmlIn)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlIn);

            var nodList = xmlDoc.SelectNodes("root/item");
            foreach (XmlNode nod in nodList)
            {
                Record.FromXmlItem(nod.OuterXml);
            }
        }
        public Dictionary<string, string> ToDictionary()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var s in List)
            {
                var a = s.GetXmlPropertyBool("genxml/checkbox/active");
                if (a)
                {
                    var v = s.GetXmlProperty("genxml/textbox/value");
                    var k = s.GetXmlProperty("genxml/textbox/url");
                    if (!rtnDict.ContainsKey(k)) rtnDict.Add(k, v);
                }
            }
            return rtnDict;
        }

        public List<SimplisityRecord> List { get { return Record.GetRecordList(_listName); } }

    }

}