using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DNNrocketAPI.Componants;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RocketCatalog.Componants
{
    public class CategoryLimpet
    {
        private DNNrocketController _objCtrl;
        /// <summary>
        /// Should be used to create an article, the portalId is required on creation
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="articleId"></param>
        /// <param name="langRequired"></param>
        public CategoryLimpet(int portalId, int categoryId, string tableName, string entityTypeCode, string langRequired = "", bool populate = true)
        {
            if (langRequired == "") langRequired = DNNrocketUtils.GetCurrentCulture();
            if (categoryId <= 0) categoryId = -1;  // create new record.
            PortalId = portalId;

            Info = new SimplisityInfo();
            Info.ItemID = categoryId;
            Info.TypeCode = entityTypeCode;
            Info.ModuleId = -1;
            Info.UserId = -1;
            Info.PortalId = PortalId;
            Info.Lang = langRequired;

            TableName = tableName;

            Populate();
        }
        private void Populate()
        {
            _objCtrl = new DNNrocketController();

            var info = _objCtrl.GetData(EntityTypeCode, CategoryId, CultureCode, ModuleId, TableName); // get existing record.
            if (info != null && info.ItemID > 0) Info = info; // check if we have a real record, or a dummy being created and not saved yet.
            PortalId = Info.PortalId;
        }
        public void Delete()
        {
            if (Info.ItemID > 0) _objCtrl.Delete(Info.ItemID, TableName);
        }
        private void ReplaceInfoFields(SimplisityInfo postInfo, string xpathListSelect)
        {
            var textList = Info.XMLDoc.SelectNodes(xpathListSelect);
            if (textList != null)
            {
                foreach (XmlNode nod in textList)
                {
                    Info.RemoveXmlNode(xpathListSelect.Replace("*","") + nod.Name);
                }
            }
            textList = postInfo.XMLDoc.SelectNodes(xpathListSelect);
            if (textList != null)
            {
                foreach (XmlNode nod in textList)
                {
                    Info.SetXmlProperty(xpathListSelect.Replace("*", "") + nod.Name, nod.InnerText);
                }
            }
        }
        public int Save(SimplisityInfo postInfo)
        {
            ReplaceInfoFields(postInfo, "genxml/textbox/*");
            ReplaceInfoFields(postInfo, "genxml/lang/genxml/textbox/*");

            var listList = postInfo.GetLists();
            foreach (var list in listList)
            {
                Info.RemoveList(list);
                foreach (var listItem in postInfo.GetList(list))
                {
                    Info.AddListItem(list, listItem);
                }
            }

            return Update();
        }
        public int Update()
        {
            Info = _objCtrl.SaveData(Info, TableName);
            if (Info.GUIDKey == "")
            {
                // get unique ref
                Info.GUIDKey = GeneralUtils.GetGuidKey();
                Info = _objCtrl.SaveData(Info, TableName);
            }
            return Info.ItemID;
        }
        public int ValidateAndUpdate()
        {
            Validate();
            return Update();
        }
        public void Validate()
        {
        }


        #region "images"
        public List<SimplisityInfo> GetImageList()
        {
            return Info.GetList("imagelist");
        }
        public void AddImage(string imageFolderRel, string uniqueName)
        {
            if (GetImageList().Count < 10)
            {
                if (Info.ItemID < 0) Update(); // blank record, not on DB.  Create now.
                var articleImage = new ArticleImage(new SimplisityInfo(), "");
                articleImage.RelPath = imageFolderRel.TrimEnd('/') + "/" + uniqueName;
                Info.AddListItem("imagelist", articleImage.Info);
                Update();
            }
        }
        public ArticleImage GetImage(int idx)
        {
            return new ArticleImage(Info.GetListItem("imagelist", idx), "");
        }
        public List<ArticleImage> GetImages()
        {
            var rtn = new List<ArticleImage>();
            foreach ( var i in Info.GetList("imagelist"))
            {
                rtn.Add(new ArticleImage(i, ""));
            }
            return rtn;
        }
        #endregion


        #region "properties"

        public string CultureCode { get { return Info.Lang; } }
        public string EntityTypeCode { get; set; }
        public SimplisityInfo Info { get; set; }
        public int ModuleId { get { return Info.ModuleId; } set { Info.ModuleId = value; } }
        public int XrefItemId { get { return Info.XrefItemId; } set { Info.XrefItemId = value; } }
        public int ParentItemId { get { return Info.ParentItemId; } set { Info.ParentItemId = value; } }
        public int CategoryId { get { return Info.ItemID; } set { Info.ItemID = value; } }
        public string GUIDKey { get { return Info.GUIDKey; } set { Info.GUIDKey = value; } }
        public int SortOrder { get { return Info.SortOrder; } set { Info.SortOrder = value; } }
        public string ImageFolder { get; set; }
        public bool DebugMode { get; set; }
        public int PortalId { get; set; }
        public bool Exists { get {if (Info.ItemID  <= 0) { return false; } else { return true; }; } }
        public string LogoRelPath
        {
            get
            {
                var articleImage = GetImage(0);
                return articleImage.RelPath;
            }
        }
        public string Name { get { return Info.GetXmlProperty("genxml/lang/genxml/textbox/categoryname"); } }
        public string Ref { get { return Info.GetXmlProperty("genxml/textbox/categoryref"); } }
        public string Summary { get { return Info.GetXmlProperty("genxml/lang/genxml/textbox/categorysummary"); } }
        public string TableName { get; set; }

        #endregion

    }

}
