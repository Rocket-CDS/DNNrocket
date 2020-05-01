using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DNNrocketAPI.Componants;

namespace RocketMod
{

    public class ArticleData
    {
        private string _langRequired;
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "ROCKETMOD";
        private DNNrocketController _objCtrl;

        public ArticleData(int itemId, int moduleid, string langRequired)
        {
            Info = new SimplisityInfo();
            Info.ItemID = itemId;
            Info.TypeCode = _entityTypeCode;
            Info.ModuleId = moduleid;

            _objCtrl = new DNNrocketController();
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();
            Populate();
        }
        private void Populate()
        {
            if (ItemID <= 0)
            {
                    Update(); // create new record
            }
            Info = _objCtrl.GetData(_entityTypeCode, ItemID, _langRequired, ModuleId, _tableName); // get existing record.
        }
        public void Delete()
        {
            _objCtrl.Delete(Info.ItemID, _tableName);
        }

        public void Save(SimplisityInfo postInfo)
        {
            Info.XMLData = postInfo.XMLData;
            Update();
        }
        public void Update()
        {
            Info = _objCtrl.SaveData(Info, _tableName);
        }
        public void AddListItem(string listname)
        {
            Info.AddListItem(listname);
            Update();
        }
        public string EntityTypeCode { get { return _entityTypeCode; } }
        public SimplisityInfo Info { get; private set; }
        public int ModuleId { get { return Info.ModuleId; } set { Info.ModuleId = value; } }
        public int XrefItemId { get { return Info.XrefItemId; } set { Info.XrefItemId = value; } }
        public int ParentItemId { get { return Info.ParentItemId; } set { Info.ParentItemId = value; } }
        public int ItemID { get { return Info.ItemID; } set { Info.ItemID = value; } }
        public string GUIDKey { get { return Info.GUIDKey; } set { Info.GUIDKey = value; } }
        public int SortOrder { get { return Info.SortOrder; } set { Info.SortOrder = value; } }
        public string ImageFolder { get; set; }
        public string DocumentFolder { get; set; }
        public string AppTheme { get; set; }
        public string AppThemeVersion { get; set; }
        public string AppThemeRelPath { get; set; }
        /// <summary>
        /// 0 = Single form / record, 1 = List and Detail / Mulitple records.
        /// </summary>
        public int AppThemeDataType { get; set; }
        public bool DebugMode { get; set; }
        public bool Exists { get {if (Info.ItemID  <= 0) { return false; } else { return true; }; } }
    }

}
