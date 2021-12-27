using DNNrocketAPI;
using DNNrocketAPI.Components;
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
            if (List.Count == 0)
            {
                // setup defaults
                var defaultFileMapPath = "/DesktopModules/DNNrocket/RocketPortal/Installation/SystemDefaults.rules";
                var filenamepath = DNNrocketUtils.MapPath(defaultFileMapPath);
                var xmlString = FileUtils.ReadFile(filenamepath);
                var s = new SimplisityInfo();
                s.XMLData = xmlString;
                foreach (XmlNode orgNod in s.XMLDoc.SelectNodes("root/organisations/*"))
                {
                    Record.AddListItem(_listName, orgNod.OuterXml);
                }
                Update();
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
            var s = new SimplisityRecord();
            s.SetXmlProperty("genxml/textbox/org", "");
            s.SetXmlProperty("genxml/textbox/name", "");
            s.SetXmlProperty("genxml/checkbox/active", "");
            s.SetXmlProperty("genxml/checkbox/default", "");
            Record.AddRecordListItem(_listName, s);
            Update();
        }
        public void DeleteRow(int idx)
        {
            Record.RemoveRecordListItem(_listName, idx);
            Update();
        }
        public bool Active(int idx)
        {
            return Record.GetXmlPropertyBool("genxml/" + _listName + "/genxml[" + idx + 1 + "]/checkbox/active");
        }
        public void Active(int idx, bool value)
        {
            Record.SetXmlProperty("genxml/" + _listName + "/genxml[" + idx + 1 + "]/checkbox/active", value.ToString());
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
        public string DefaultOrg()
        {
            var dl = Record.GetRecordList(_listName);
            foreach (var d in dl)
            {
                if (d.GetXmlPropertyBool("genxml/checkbox/default")) return d.GetXmlProperty("genxml/textbox/org");
            }
            if (dl.Count == 0) return "";
            return dl.First().GetXmlProperty("genxml/textbox/org");
        }

        public Dictionary<string, string> ActiveList()
        {
            var orgDict = new Dictionary<string, string>();
            foreach (SimplisityRecord o in List)
            {
                if (o.GetXmlPropertyBool("genxml/checkbox/active"))
                {
                    if (!orgDict.ContainsKey(o.GetXmlProperty("genxml/textbox/org")))
                    {
                        orgDict.Add(o.GetXmlProperty("genxml/textbox/org"), o.GetXmlProperty("genxml/textbox/name"));
                    }
                }
            }
            return orgDict;
        }

        public List<SimplisityRecord> List { get { return Record.GetRecordList(_listName); } }


    }

}