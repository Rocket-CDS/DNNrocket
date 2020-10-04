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
            _objCtrl = new DNNrocketController();
            Info = new SimplisityInfo();
            Info.FromXmlItem(xmlExportItem);
        }
        /// <summary>
        /// Constructor for single page modules, use the moduleref as the guidkey. 
        /// </summary>
        /// <param name="moduleRef"></param>
        /// <param name="moduleid"></param>
        /// <param name="langRequired"></param>
        public ArticleLimpet(string moduleRef, int moduleid, string langRequired)
        {
            _objCtrl = new DNNrocketController();
            var articleInfo = _objCtrl.GetByGuidKey(PortalUtils.GetCurrentPortalId(), moduleid, _entityTypeCode, moduleRef, "", _tableName);
            if (articleInfo == null)
                Populate(-1, moduleid, langRequired, moduleRef);
            else
                Populate(articleInfo.ItemID, moduleid, langRequired, moduleRef);
        }
        public ArticleLimpet(int articleId, int moduleid, string langRequired)
        {
            _objCtrl = new DNNrocketController();
            Populate(articleId, moduleid, langRequired, "");
        }
        private void Populate(int articleId, int moduleid, string langRequired, string guidKey)
        {
            if (articleId <= 0) articleId = -1;  // create new record.
            _articleId = articleId;
            Info = new SimplisityInfo();
            Info.ItemID = articleId;
            Info.TypeCode = _entityTypeCode;
            Info.ModuleId = moduleid;
            Info.UserId = -1;
            Info.PortalId = PortalUtils.GetPortalId();
            Info.GUIDKey = guidKey;

            _langRequired = langRequired;
            if (_langRequired == "") _langRequired = DNNrocketUtils.GetEditCulture();

            var info = _objCtrl.GetData(_entityTypeCode, _articleId, _langRequired, ModuleId, _tableName); // get existing record.
            if (info != null && info.ItemID > 0) Info = info; // check if we have a real record, or a dummy being created and not saved yet.
        }

        #region "images"
        public List<SimplisityInfo> GetImageList()
        {
            return Info.GetList("imagelist");
        }
        public List<ArticleImage> GetImages()
        {
            var rtn = new List<ArticleImage>();
            foreach (var i in GetImageList())
            {
                rtn.Add(new ArticleImage(i, ""));
            }
            return rtn;
        }
        public void AddImage(string uniqueName, string destinationRelPath)
        {
            if (Info.ItemID < 0) Update(); // blank record, not on DB.  Create now.
            var articleImage = new ArticleImage(new SimplisityInfo(), "");
            articleImage.RelPath = destinationRelPath.TrimEnd('/') + "/" + uniqueName;
            Info.AddListItem("imagelist", articleImage.Info);
            Update();
        }

        #endregion

        #region "documents"
        public List<SimplisityInfo> GetDocumentList()
        {
            return Info.GetList("documentlist");
        }
        public List<ArticleDoc> GetDocuments()
        {
            var rtn = new List<ArticleDoc>();
            foreach (var i in GetDocumentList())
            {
                rtn.Add(new ArticleDoc(i, ""));
            }
            return rtn;
        }
        public void AddDocument(string uniqueName, string destinationRelPath)
        {
            if (Info.ItemID < 0) Update(); // blank record, not on DB.  Create now.
            var articleDoc = new ArticleDoc(new SimplisityInfo(), "");
            articleDoc.RelPath = destinationRelPath.TrimEnd('/') + "/" + uniqueName;
            Info.AddListItem("documentlist", articleDoc.Info);
            Update();
        }

        #endregion

        #region "links"
        public List<SimplisityInfo> GetLinkList()
        {
            return Info.GetList("linklist");
        }
        public List<ArticleLink> GetLinks()
        {
            var rtn = new List<ArticleLink>();
            foreach (var i in GetLinkList())
            {
                rtn.Add(new ArticleLink(i, ""));
            }
            return rtn;
        }
        public void AddLink()
        {
            if (Info.ItemID < 0) Update(); // blank record, not on DB.  Create now.
            var articleLink = new ArticleLink(new SimplisityInfo(), "");
            Info.AddListItem("linklist", articleLink.Info);
            Update();
        }

        #endregion

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
        public string ListSelectorcCVS { get { return ".linklist,.imagelist,.documentlist"; } }
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
