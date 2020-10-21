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

namespace RocketMod
{
    public class ArticleRowLimpet
    {
        public ArticleRowLimpet()
        {
            Info = new SimplisityInfo();
            ArticleRowRef = GeneralUtils.GetGuidKey();
        }

        #region "images"
        public List<SimplisityInfo> GetImageList()
        {
            return Info.GetList(ImageListName);
        }
        public void AddImage(PortalCatalogLimpet portalCatalog, string uniqueName)
        {
            if (GetImageList().Count < portalCatalog.ArticleImageLimit)
            {
                var articleImage = new ArticleImage(new SimplisityInfo(), "");
                articleImage.RelPath = portalCatalog.ImageFolderRel.TrimEnd('/') + "/" + uniqueName;
                Info.AddListItem(ImageListName, articleImage.Info);
            }
        }
        public ArticleImage GetImage(int idx)
        {
            return new ArticleImage(Info.GetListItem(ImageListName, idx), "");
        }
        public List<ArticleImage> GetImages()
        {
            var rtn = new List<ArticleImage>();
            foreach ( var i in Info.GetList(ImageListName))
            {
                rtn.Add(new ArticleImage(i, ""));
            }
            return rtn;
        }
        #endregion

        #region "docs"
        public List<SimplisityInfo> GetDocumentsList()
        {
            return Info.GetList(DocumentListName);
        }
        public void AddDoc(string uniqueName)
        {
            var articleDoc = new ArticleDoc(new SimplisityInfo(), "");
            articleDoc.RelPath = portalCatalog.DocFolderRel.TrimEnd('/') + "/" + uniqueName;
            articleDoc.Name = uniqueName;
            Info.AddListItem(DocumentListName, articleDoc.Info);
        }
        public ArticleDoc GetDocuments(int idx)
        {
            return new ArticleDoc(Info.GetListItem(DocumentListName, idx), "");
        }
        public List<ArticleDoc> GetDocuments()
        {
            var rtn = new List<ArticleDoc>();
            foreach (var i in Info.GetList(DocumentListName))
            {
                rtn.Add(new ArticleDoc(i, ""));
            }
            return rtn;
        }
        #endregion

        #region "links"
        public List<SimplisityInfo> GetLinkList()
        {
            return Info.GetList(LinkListName);
        }
        public void AddLink()
        {
            Info.AddListItem(LinkListName);
        }
        public ArticleLink Getlink(int idx)
        {
            return new ArticleLink(Info.GetListItem(LinkListName, idx), "");
        }
        public List<ArticleLink> Getlinks()
        {
            var rtn = new List<ArticleLink>();
            foreach (var i in Info.GetList(LinkListName))
            {
                rtn.Add(new ArticleLink(i, ""));
            }
            return rtn;
        }
        #endregion

        #region "properties"

        public string ArticleRowRef { get { return Info.GetXmlProperty("genxml/hidden/articlerowref"); } set { Info.GetXmlProperty("genxml/hidden/articlerowref", value); } }        
        public int ArticleId { get; set; }
        public int PortalId { get; set; }
        public SimplisityInfo Info { get; set; }
        public string GUIDKey { get { return Info.GUIDKey; } set { Info.GUIDKey = value; } }
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
        public string ImageListName { get { return "imagelist"; } }
        public string DocumentListName { get { return "documentlist"; } }
        public string LinkListName { get { return "linklist"; } }

        

        #endregion

    }

}
