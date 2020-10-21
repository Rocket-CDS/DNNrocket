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
        private const string _articleRowxPath = "genxml/hidden/articlerowref";

        public ArticleLimpet()
        {
            _objCtrl = new DNNrocketController();
            Info = new SimplisityInfo();
        }
        public ArticleLimpet(int portalId, string moduleRef, int moduleid, string cultureCode)
        {
            _objCtrl = new DNNrocketController();
            CultureCode = cultureCode;
            if (CultureCode == "") CultureCode = DNNrocketUtils.GetEditCulture();
            var articleInfo = _objCtrl.GetByGuidKey(portalId, moduleid, _entityTypeCode, moduleRef, "", _tableName, CultureCode);
            PortalId = portalId;
            if (articleInfo == null)
            {
                PortalId = portalId;
                Info = new SimplisityInfo();
                Info.ItemID = -1;
                Info.ModuleId = moduleid;
                Info.TypeCode = _entityTypeCode;
                Info.ModuleId = -1;
                Info.UserId = -1;
                Info.GUIDKey = moduleRef;
                Info.PortalId = PortalId;
            }
            else
                Info = articleInfo;
        }
        public ArticleLimpet(string xmlExportItem, string langRequired = "")
        {
            _objCtrl = new DNNrocketController();
            Info = new SimplisityInfo();
            Info.FromXmlItem(xmlExportItem);
            CultureCode = langRequired;
            if (CultureCode == "") CultureCode = Info.Lang;
            PortalId = Info.PortalId;
        }
        public void Delete()
        {
            _objCtrl.Delete(Info.ItemID, _tableName);
        }

        private void ReplaceInfoFields(SimplisityInfo postInfo, string xpathListSelect)
        {
            var textList = Info.XMLDoc.SelectNodes(xpathListSelect);
            if (textList != null)
            {
                foreach (XmlNode nod in textList)
                {
                    Info.RemoveXmlNode(xpathListSelect.Replace("*", "") + nod.Name);
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

            return Update();
        }
        public int Update()
        {
            Info = _objCtrl.SaveData(Info, _tableName);
            if (Info.GUIDKey == "")
            {
                var l = Info.GetList(ArticleRowListName);
                if (l.Count == 0) UpdateArticleRow("<genxml></genxml>"); // Create Default ArticleRow

                // get unique ref
                Info.GUIDKey = GeneralUtils.GetGuidKey();
                Info = _objCtrl.SaveData(Info, _tableName);
            }
            return Info.ItemID;
        }
        public int ValidateAndUpdate()
        {
            Validate();
            return Update();
        }
        public void AddListItem(string listname)
        {
            if (Info.ItemID < 0) Update(); // blank record, not on DB.  Create now.
            Info.AddListItem(listname);
            Update();
        }
        public void Validate()
        {
        }

        #region "Rows"

        public void RemoveArticleRows()
        {
            Info.RemoveList(ArticleRowListName);
        }
        public void RemoveArticleRow(ArticleRowLimpet articleRowData)
        {
            Info.RemoveListItem(ArticleRowListName, _articleRowxPath, articleRowData.ArticleRowRef);
        }
        public void UpdateArticleRow(string xmlData)
        {
            var sInfo = new SimplisityInfo();
            sInfo.XMLData = xmlData;
            UpdateArticleRow(sInfo);
        }
        public void UpdateArticleRow(SimplisityInfo postInfo)
        {
            var articlerowref = postInfo.GetXmlProperty(_articleRowxPath);
            if (articlerowref != "")
            {
                // keep order, if it exists
                var foundrow = false;
                var sortList = GetArticleRows();
                Info.RemoveList(ArticleRowListName);
                foreach (var r in sortList)
                {
                    if (r.ArticleRowRef == articlerowref)
                    {
                        Info.AddListItem(ArticleRowListName, postInfo);
                        foundrow = true;
                    }
                    else
                        Info.AddListItem(ArticleRowListName, r.Info);
                }
                if (!foundrow) Info.AddListItem(ArticleRowListName, postInfo);
            }
            else
            {
                articlerowref = GeneralUtils.GetGuidKey();
                postInfo.SetXmlProperty(_articleRowxPath, articlerowref);
                Info.AddListItem(ArticleRowListName, postInfo);
            }
        }
        public List<ArticleRowLimpet> GetArticleRows()
        {
            var rtnList = new List<ArticleRowLimpet>();
            var l = Info.GetList(ArticleRowListName);
            foreach (var a in l)
            {
                var articleRowLimpet = new ArticleRowLimpet();
                articleRowLimpet.Info = a;
                rtnList.Add(articleRowLimpet);
            }
            return rtnList;
        }
        public ArticleRowLimpet GetArticleRow(string articlerowref)
        {
            var articleRowData = new ArticleRowLimpet();
            if (articlerowref != "")
            {
                var sInfo = Info.GetListItem(ArticleRowListName, _articleRowxPath, articlerowref);
                if (sInfo != null) articleRowData.Info = sInfo;
            }
            articleRowData.ArticleId = ArticleId;
            articleRowData.PortalId = PortalId;
            return articleRowData;
        }
        public ArticleRowLimpet GetArticleRow(int idx)
        {
            var articleRowData = new ArticleRowLimpet();
            var sInfo = Info.GetListItem(ArticleRowListName, idx);
            if (sInfo != null) articleRowData.Info = sInfo;
            articleRowData.ArticleId = ArticleId;
            articleRowData.PortalId = PortalId;
            return articleRowData;
        }
        public string ArticleRowListName { get { return "articlerows"; } }

        #endregion

        public string ListSelectorcCVS { get { return ".linklist,.imagelist,.documentlist"; } }
        public string CultureCode { get; private set; }
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
        public string AppThemeFolder { get; set; }
        public string AppThemeVersion { get; set; }
        public string AppThemeRelPath { get; set; }
        /// <summary>
        /// 0 = Single form / record, 1 = List and Detail / Mulitple records.
        /// </summary>
        public int AppThemeDataType { get; set; }
        public bool DebugMode { get; set; }
        public bool Exists { get {if (Info.ItemID  <= 0) { return false; } else { return true; }; } }
        public int PortalId { get; set; }

        public string LogoRelPath
        {
            get
            {
                var articleImage = GetArticleRow(0).GetImage(0);
                return articleImage.RelPath;
            }
        }
        public string Title
        {
            get
            {
                var title = Info.GetXmlProperty("genxml/lang/genxml/textbox/title");
                if (title == "") title = Info.GetXmlProperty("genxml/textbox/title");
                if (title == "") title = Name;
                return title;
            }
        }
        public string Name
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/name");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/name");
                return rtn;
            }
        }
        public string NameUrl { get { return GeneralUtils.UrlFriendly(Name); } }
        public string Ref { get { return Info.GetXmlProperty("genxml/textbox/ref"); } }
        public string Summary
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/summary");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/summary");
                return rtn;
            }
        }
        public string Description
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/description");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/description");
                if (rtn == "") rtn = Summary;
                return rtn;
            }
        }
        public string KeyWords
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/keywords");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/keywords");
                return rtn;
            }
        }
        public string RichText
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/richtext");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/richtext");
                return rtn;
            }
        }
        public string Text
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/text");
                if (rtn == "") rtn = Info.GetXmlProperty("genxml/textbox/text");
                return rtn;
            }
        }


    }

}
