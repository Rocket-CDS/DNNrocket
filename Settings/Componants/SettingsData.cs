using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketSettings
{

    public class SettingsData
    {
        private int _tabid;
        private int _moduleid;
        private string _guidKey;
        private string _langRequired;
        private string _entityTypeCode;
        private string _listName;
        private bool _onlyRead;
        private string _tableName;
        private DNNrocketController _objCtrl;
        private Dictionary<string,string> _settingDict;

        public SimplisityInfo Info;

        public SettingsData(string guidKey, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata", bool onlyRead = false, string tableName = "DNNrocket")
        {
            InitSettingsData(-1, -1, guidKey, langRequired, entityTypeCode, listname, onlyRead, tableName);
        }


        public SettingsData(int tabId, int moduleId, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata", bool onlyRead = false, string tableName = "DNNrocket")
        {
            var guidKey = "moduleid" + moduleId;
            InitSettingsData(tabId, moduleId, guidKey, langRequired, entityTypeCode, listname, onlyRead, tableName);
        }


        private void InitSettingsData(int tabId, int moduleId, string guidKey, string langRequired, string entityTypeCode = "ROCKETSETTINGS", string listname = "settingsdata", bool onlyRead = false, string tableName = "DNNrocket")
        {
            InvalidKeyValues = false;
            _tableName = tableName;
            _onlyRead = onlyRead;
            _entityTypeCode = entityTypeCode;
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetCurrentCulture();
            _tabid = tabId;
            _moduleid = moduleId;
            _guidKey = guidKey;
            _listName = listname;
            _objCtrl = new DNNrocketController();

            GetSettingData(_guidKey, _langRequired);
            if (Info == null) Info = new SimplisityInfo();

            Info.Lang = _langRequired;
            Info.GUIDKey = _guidKey;
            Info.TypeCode = _entityTypeCode;
            Info.ModuleId = _moduleid;
        }

        public void Delete()
        {
            if (Info != null)
            {
                _objCtrl.Delete(Info.ItemID);
                Info = null;
            }
        }

        public void Save(SimplisityInfo postInfo)
        {
            var dbInfo = _objCtrl.GetData(_entityTypeCode, Info.ItemID, _langRequired, -1, _moduleid, true, _tableName);
            if (dbInfo != null)
            {
                dbInfo.XMLData = postInfo.XMLData;
                CreateMissingLanguageRecords(postInfo.XMLData);
                _objCtrl.SaveData(dbInfo, Info.ItemID, _tableName);
            }

        }

        public void Update()
        {
            _objCtrl.SaveData(Info, -1, _tableName);
        }

        /// <summary>
        /// update all langauge records which are empty.
        /// </summary>
        private void CreateMissingLanguageRecords(string xmlData = "<genxml></genxml>")
        {
            var cc = DNNrocketUtils.GetCultureCodeList();
            foreach (var l in cc)
            {
                if (Info.Lang != l)
                {
                    var dbRecord = _objCtrl.GetRecordLang(Info.ItemID, l, false, _tableName);
                    var nodcount = 0;
                    if (dbRecord != null)
                    {
                        var nodList = dbRecord.XMLDoc.SelectNodes("genxml/*");
                        nodcount = nodList.Count;
                    }
                    if (nodcount == 0)
                    {
                        var newInfo = Info;
                        newInfo.Lang = l;
                        _objCtrl.SaveData(newInfo, Info.ItemID, _tableName);
                    }
                }
            }
        }


        public void AddRow()
        {
            Info.AddListItem(_listName);
            _objCtrl.SaveData(Info, Info.ItemID, _tableName);
            CreateMissingLanguageRecords();
        }

        public string ExportData(bool withTextData = false)
        {
            var xmlOut = "<root>";
            xmlOut += Info.ToXmlItem(withTextData);
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
                var s = new SimplisityInfo();
                Info.FromXmlItem(nod.OuterXml);
            }
        }

        public string Get(string xpath)
        {
            return Info.GetXmlProperty(xpath);
        }
        public bool GetBool(string xpath)
        {
            return Info.GetXmlPropertyBool(xpath);
        }
        public int GetInt(string xpath)
        {
            return Info.GetXmlPropertyInt(xpath);
        }
        public double GetDouble(string xpath)
        {
            return Info.GetXmlPropertyDouble(xpath);
        }
        
        public Dictionary<string,string> ToDictionary()
        {
            var rtnDict = new Dictionary<string, string>();
            foreach (var s in List)
            {
                var v = s.GetXmlProperty("genxml/textbox/value");
                if (v == "") v = s.GetXmlProperty("genxml/lang/genxml/textbox/valuelang");
                var k = s.GetXmlProperty("genxml/textbox/name");
                if (!rtnDict.ContainsKey(k)) rtnDict.Add(k, v);
            }

            // add non-list values.
            var l = Info.ToDictionary();
            foreach (var s in l)
            {
                if (!rtnDict.ContainsKey(s.Key)) rtnDict.Add(s.Key, s.Value);
            }
            return rtnDict;
        }


        #region "properties"

        public int ModuleId { get {return _moduleid;} }
        public int TabId { get { return _tabid; } }
        public string EntityTypeCode { get { return _entityTypeCode; } set { _entityTypeCode = value; } }
        public bool InvalidKeyValues { get; set; }

        public List<SimplisityInfo> List
        {
            get {
                if (Info == null) return new List<SimplisityInfo>();
                var l = Info.GetList(_listName);
                if (l == null) return new List<SimplisityInfo>();
                return l;
            }
        }

        #endregion

        #region "private methods"

        private void GetSettingData(string guidKey, string cultureCode)
        {
            Info = _objCtrl.GetData(guidKey, _entityTypeCode, cultureCode, -1, _moduleid, _onlyRead, _tableName);
        }
        #endregion


    }

}
