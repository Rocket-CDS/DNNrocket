using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketMod
{

    public class ArticleData
    {
        private int _moduleid;
        private string _langRequired;
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "ROCKETMOD";
        private DNNrocketController _objCtrl;

        public ArticleData(int itemId, int moduleid, string langRequired)
        {
            _moduleid = moduleid;
            _objCtrl = new DNNrocketController();
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();
            if (!Populate(itemId)) AddArticle();
        }
        public ArticleData(int moduleid, string langRequired)
        {
            _moduleid = moduleid;
            _objCtrl = new DNNrocketController();
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();

            var guidkey = "rocketdata*" + moduleid;
            if (!Populate(guidkey)) AddArticle(guidkey);
        }

        public void Delete()
        {
            _objCtrl.Delete(Info.ItemID, _tableName);
        }

        public void Save(SimplisityInfo postInfo)
        {
            var dbInfo = _objCtrl.GetData( _entityTypeCode, Info.ItemID, _langRequired, _moduleid, true, _tableName);
            if (dbInfo != null)
            {
                // update all langauge record which are empty.
                var cc = DNNrocketUtils.GetCultureCodeList();
                foreach (var l in cc)
                {
                    var dbRecord = _objCtrl.GetRecordLang(Info.ItemID, l, false, _tableName);
                    var nodList = dbRecord.XMLDoc.SelectNodes("genxml/*");
                    if (nodList.Count == 0)
                    {
                        var dbInfo2 = _objCtrl.GetData(_entityTypeCode, Info.ItemID, l, _moduleid, true, _tableName);
                        if (dbInfo2 != null)
                        {
                            dbInfo2.XMLData = postInfo.XMLData;
                            _objCtrl.SaveData(dbInfo2, _tableName);
                        }
                    }
                }

                dbInfo.XMLData = postInfo.XMLData;
                _objCtrl.SaveData(dbInfo, _tableName); // save before list sort, so we have hte data in DB.

            }
        }
        public void Update()
        {
            _objCtrl.SaveData(Info, _tableName);
        }

        public void AddListItem(string listname)
        {
            Info.AddListItem(listname);
            Update();
        }

        private void AddArticle()
        {
            Info = _objCtrl.GetData(_entityTypeCode, -1, _langRequired, _moduleid, false, _tableName);    
        }
        private void AddArticle(string guidkey)
        {
            Info = _objCtrl.GetData(_entityTypeCode, -1, _langRequired, _moduleid, false, _tableName);
            Info.GUIDKey = guidkey;
            _objCtrl.SaveData(Info, _tableName);
        }

        public bool Populate(int ItemId)
        {
            Info = _objCtrl.GetData(_entityTypeCode, ItemId, _langRequired, _moduleid, true, _tableName);
            if (Info == null) return false;
            return true;
        }
        public bool Populate(string guidkey)
        {
            var tempInfo = _objCtrl.GetByGuidKey(-1, _moduleid, _entityTypeCode, guidkey, "", _tableName);
            if (tempInfo == null) return false;
            Info = _objCtrl.GetData(_entityTypeCode, tempInfo.ItemID, _langRequired, _moduleid, true, _tableName);
            return true;
        }

        public string EntityTypeCode { get { return _entityTypeCode; } }

        public SimplisityInfo Info { get; private set; }

        public int ModuleId { get { return Info.ModuleId; } set { Info.ModuleId = value; } }
        public int XrefItemId { get { return Info.XrefItemId; } set { Info.XrefItemId = value; } }
        public int ParentItemId { get { return Info.ParentItemId; } set { Info.ParentItemId = value; } }
        public int ItemId { get { return Info.ItemID; } }
        public int SortOrder { get { return Info.SortOrder; } set { Info.SortOrder = value; } }
        public string ImageFolder { get; set; }
        public string DocumentFolder { get; set; }
        public string AppTheme { get; set; }
        public string AppThemeVersion { get; set; }
        public string AppThemeRelPath { get; set; }
        public bool DebugMode { get; set; }
        public bool Exists { get {if (Info == null) { return false; } else { return true; }; } }
    }

}
