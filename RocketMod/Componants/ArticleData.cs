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
            if (itemId <= 0)
            {
                AddArticle();
            }
            else
            {
                Populate(itemId);
            }
        }

        public void Delete()
        {
            _objCtrl.Delete(Info.ItemID, _tableName);
        }

        public void Save(SimplisityInfo postInfo)
        {
            var dbInfo = _objCtrl.GetData( _entityTypeCode, Info.ItemID, _langRequired, -1, _moduleid, true, _tableName);
            if (dbInfo != null)
            {
                // NOTE: Be careful of the order for saving.  The sort list is a PAIN and required speciifc data.
                // 1 - Create any empty record. (Must be done, so on first save we get the data in the DB)
                // 2 - Apply new XML strcuture 
                // 3 - Do sort list.
                // 4 - Save the new list data to the DB.

                // update all langauge record which are empty.
                var cc = DNNrocketUtils.GetCultureCodeList();
                foreach (var l in cc)
                {
                    var dbRecord = _objCtrl.GetRecordLang(Info.ItemID, l, false, _tableName);
                    var nodList = dbRecord.XMLDoc.SelectNodes("genxml/*");
                    if (nodList.Count == 0)
                    {
                        var dbInfo2 = _objCtrl.GetData(_entityTypeCode, Info.ItemID, l, -1, _moduleid, true, _tableName);
                        if (dbInfo2 != null)
                        {
                            dbInfo2.XMLData = postInfo.XMLData;
                            _objCtrl.SaveData(dbInfo2, Info.ItemID, _tableName);
                        }
                    }
                }

                dbInfo.XMLData = postInfo.XMLData;
                //_objCtrl.SaveData(dbInfo, Info.ItemID, _tableName); // save before list sort, so we have hte data in DB.

                // sort lists from DB and post data
                var sortLists = new DNNrocketAPI.Componants.SortLists(dbInfo, _tableName, DebugMode);
                sortLists.Save();

            }
        }
        public void Update()
        {
            _objCtrl.SaveData(Info, SystemId, _tableName);
        }

        private void AddArticle()
        {
            Info = _objCtrl.GetData(_entityTypeCode, -1, _langRequired, -1, _moduleid, false, _tableName);    
        }

        public void Populate(int ItemId)
        {
            Info = _objCtrl.GetData(_entityTypeCode, ItemId, _langRequired, -1, _moduleid, true, _tableName);
        }

        public string EntityTypeCode { get { return _entityTypeCode; } }

        public SimplisityInfo Info { get; private set; }

        public int ModuleId { get { return Info.ModuleId; } set { Info.ModuleId = value; } }
        public int XrefItemId { get { return Info.XrefItemId; } set { Info.XrefItemId = value; } }
        public int ParentItemId { get { return Info.ParentItemId; } set { Info.ParentItemId = value; } }
        public int SystemId { get { return Info.SystemId; } set { Info.SystemId = value; } }
        public int ItemId { get { return Info.ItemID; } }
        public int SortOrder { get { return Info.SortOrder; } set { Info.SortOrder = value; } }
        public string ImageFolder { get; set; }
        public string DocumentFolder { get; set; }
        public string AppTheme { get; set; }
        public string AppThemeVersion { get; set; }
        public string AppThemeRelPath { get; set; }
        public bool DebugMode { get; set; }
    }

}
