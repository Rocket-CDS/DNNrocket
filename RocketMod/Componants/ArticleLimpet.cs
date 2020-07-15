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

    public class ArticleLimpet
    {
        private string _langRequired;
        private const string _tableName = "DNNRocket";
        private const string _entityTypeCode = "ROCKETMOD";
        private DNNrocketController _objCtrl;
        private int _articleId;

        public ArticleLimpet(string xmlExportItem)
        {
            Info = new SimplisityInfo();
            Info.FromXmlItem(xmlExportItem);
        }
        public ArticleLimpet(int articleId, int moduleid, string langRequired)
        {
            if (articleId <= 0) articleId = -1;  // create new record.
            _articleId = articleId;
            Info = new SimplisityInfo();
            Info.ItemID = articleId;
            Info.TypeCode = _entityTypeCode;
            Info.ModuleId = moduleid;
            Info.UserId = -1;
            Info.PortalId = PortalUtils.GetPortalId();

            _objCtrl = new DNNrocketController();
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();
            Populate(_langRequired);
        }
        private void Populate(string langRequired)
        {
            _objCtrl = new DNNrocketController();
            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();

            var info = _objCtrl.GetData(_entityTypeCode, _articleId, _langRequired, ModuleId, _tableName); // get existing record.
            if (info != null && info.ItemID > 0) Info = info; // check if we have a real record, or a dummy being created and not saved yet.
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
        public int ArticleId { get { return Info.ItemID; } set { Info.ItemID = value; } }
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
