using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RocketMod.Componants;

namespace RocketMod
{
    public partial class StartConnect
    {

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ArticleLimpet GetActiveArticle(string cultureCode = "")
        {
            if (cultureCode == "") cultureCode = _currentLang;
            return  new ArticleLimpet(PortalUtils.GetCurrentPortalId(), _dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, cultureCode);
        }
        private ArticleRowLimpet GetActiveArtilceRow(string cultureCode = "")
        {
            var articleData = GetActiveArticle(cultureCode);
            var articleRowData = new ArticleRowLimpet();
            var articleRowRef = _paramInfo.GetXmlProperty("genxml/hidden/articlerowref");
            if (articleRowRef == "") articleRowRef = _postInfo.GetXmlProperty("genxml/hidden/articlerowref"); // it may be a new record, the ref is populated by the template default
            if (articleRowRef != "")
                articleRowData = articleData.GetArticleRow(articleRowRef);
            else
            {
                articleRowData.ArticleRowRef = GeneralUtils.GetGuidKey();
                articleRowData.ArticleId = articleData.ArticleId;
                articleRowData.PortalId = articleData.PortalId;
            }

            return articleRowData;
        }

        public void SaveArticle(bool doBackup)
        {
            var articleData = GetActiveArticle(_editLang);

            // do Backup
            if (doBackup) DoBackUp();

            // do Save
            _passSettings.Add("saved", "true");
            articleData.Save(_postInfo);
        }
        public void DeleteArticle()
        {
            GetActiveArticle().Delete();
        }
        public string AddArticleImage()
        {
            var articleData = GetActiveArticle(_editLang);
            articleData.UpdateArticleRow(_postInfo); // Save all fields

            var articleRowData = GetActiveArtilceRow(_editLang);

            var imgList = ImgUtils.MoveImageToFolder(_postInfo, _moduleParams.ImageFolderMapPath);
            foreach (var nam in imgList)
            {
                articleRowData.AddImage(_moduleParams, nam); // add image
            }

            articleData.UpdateArticleRow(articleRowData.Info.XMLData); // update with image
            articleData.Update(); // save to DB

            return GetEditArticleRow();
        }
        public string AddArticleDoc()
        {
            var articleData = GetActiveArticle(_editLang);
            articleData.UpdateArticleRow(_postInfo); // Save all fields

            var articleRowData = GetActiveArtilceRow(_editLang);

            var docList = MoveDocumentToFolder(_postInfo, _moduleParams.DocumentFolderMapPath);
            foreach (var nam in docList)
            {
                articleRowData.AddDoc(_moduleParams, nam);
            }

            articleData.UpdateArticleRow(articleRowData.Info.XMLData); // update with image
            articleData.Update(); // save to DB

            return GetEditArticleRow();
        }
        private List<string> MoveDocumentToFolder(SimplisityInfo postInfo, string destinationFolderMapPath, int maxDocuments = 50)
        {
            destinationFolderMapPath = destinationFolderMapPath.TrimEnd('\\');
            var rtn = new List<string>();
            var userid = UserUtils.GetCurrentUserId(); // prefix to filename on upload.
            if (!Directory.Exists(destinationFolderMapPath)) Directory.CreateDirectory(destinationFolderMapPath);
            var fileuploadlist = postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                var docCount = 1;
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var userfilename = userid + "_" + friendlyname;
                        if (docCount <= maxDocuments)
                        {
                            var unqName = DNNrocketUtils.GetUniqueFileName(friendlyname.Replace(" ", "_"), destinationFolderMapPath);
                            var fname = destinationFolderMapPath + "\\" + unqName;
                            File.Move(PortalUtils.TempDirectoryMapPath() + "\\" + userfilename, fname);
                            if (File.Exists(fname))
                            {
                                rtn.Add(unqName);
                                docCount += 1;
                            }
                        }
                        File.Delete(PortalUtils.TempDirectoryMapPath() + "\\" + userfilename);
                    }
                }
            }
            return rtn;
        }
        public string AddArticleLink()
        {
            var articleData = GetActiveArticle(_editLang);
            articleData.UpdateArticleRow(_postInfo); // Save all fields

            var articleRowData = GetActiveArtilceRow(_editLang);
            articleRowData.AddLink();
            articleData.Update(); // save to DB

            return GetEditArticleRow();
        }
        public String AddArticle()
        {
            return EditArticle();
        }
        public String EditArticle()
        {
            var articleData = GetActiveArticle(_nextLang);
            return EditArticle(articleData);
        }
        public String EditArticle(ArticleLimpet articleData)
        {
            var razorTempl = _moduleParams.AppTheme.GetTemplate("ArticleEdit.cshtml");
            if (razorTempl == "") razorTempl = RenderRazorUtils.GetRazorTemplateData("ArticleEdit.cshtml", _systemData.SystemRelPath, "config-w3", _nextLang, _rocketInterface.ThemeVersion, true);
            var dataObjects = new Dictionary<string, object>();

            dataObjects.Add("articledata", articleData);
            return RenderRazorUtils.RazorDetail(razorTempl, _moduleParams, dataObjects, new SessionParams(_paramInfo), _passSettings, true);
        }
        public String GetEditArticleRow()
        {
            var articleRowData = GetActiveArtilceRow(_nextLang);
            var razorTempl = _moduleParams.AppTheme.GetTemplate("ArticleEditRow.cshtml");
            if (razorTempl == "") razorTempl = RenderRazorUtils.GetRazorTemplateData("ArticleEditRow.cshtml", _systemData.SystemRelPath, "config-w3", _nextLang, _rocketInterface.ThemeVersion, true);
            var dataObjects = new Dictionary<string, object>();

            dataObjects.Add("articlerow", articleRowData);
            return RenderRazorUtils.RazorDetail(razorTempl, _moduleParams, dataObjects, new SessionParams(_paramInfo), _passSettings, true);
        }
        public string ArticleDocumentList()
        {
            var articleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");
            var docList = new List<object>();
            foreach (var i in DNNrocketUtils.GetFiles(_moduleParams.DocumentFolderMapPath))
            {
                var sInfo = new SimplisityInfo();
                sInfo.SetXmlProperty("genxml/name", i.Name);
                sInfo.SetXmlProperty("genxml/relname", _moduleParams.DocumentFolderRel + "/" + i.Name);
                sInfo.SetXmlProperty("genxml/fullname", i.FullName);
                sInfo.SetXmlProperty("genxml/extension", i.Extension);
                sInfo.SetXmlProperty("genxml/directoryname", i.DirectoryName);
                sInfo.SetXmlProperty("genxml/lastwritetime", i.LastWriteTime.ToShortDateString());
                docList.Add(sInfo);
            }

            _passSettings.Add("uploadcmd", "articleadmin_docupload");
            _passSettings.Add("deletecmd", "articleadmin_docdelete");
            _passSettings.Add("articleid", articleid.ToString());

            var razorTempl = RenderRazorUtils.GetRazorTemplateData("DocumentSelect.cshtml", _systemData.SystemRelPath, _rocketInterface.DefaultTheme, _nextLang, _rocketInterface.ThemeVersion, true);
            return RenderRazorUtils.RazorList(razorTempl, docList, _passSettings,  new SessionParams(_paramInfo), true);
        }
        public void ArticleDocumentUploadToFolder()
        {
            var userid = UserUtils.GetCurrentUserId(); // prefix to filename on upload.
            if (!Directory.Exists(_moduleParams.DocumentFolderMapPath)) Directory.CreateDirectory(_moduleParams.DocumentFolderMapPath);
            var fileuploadlist = _paramInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var userfilename = userid + "_" + friendlyname;
                        File.Copy(PortalUtils.TempDirectoryMapPath() + "\\" + userfilename, _moduleParams.DocumentFolderMapPath + "\\" + friendlyname, true);
                        File.Delete(PortalUtils.TempDirectoryMapPath() + "\\" + userfilename);
                    }
                }

            }
        }
        public void ArticleDeleteDocument()
        {
            var docfolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            if (docfolder == "") docfolder = "docs";
            var docDirectory = PortalUtils.HomeDNNrocketDirectoryMapPath() + "\\" + docfolder;
            var docList = _postInfo.GetXmlProperty("genxml/hidden/dnnrocket-documentlist").Split(';');
            foreach (var i in docList)
            {
                if (i != "")
                {
                    var documentname = GeneralUtils.DeCode(i);
                    var docFile = docDirectory + "\\" + documentname;
                    if (File.Exists(docFile))
                    {
                        File.Delete(docFile);
                    }
                }
            }

        }
        public string UpdateArticleRow()
        {
            var articleData = GetActiveArticle(_editLang);
            articleData.UpdateArticleRow(_postInfo);
            articleData.Update();
            _passSettings.Add("saved", "true");
            return GetEditArticleRow();
        }
        public string DeleteArticleRow()
        {
            var articleData = GetActiveArticle();
            var articleRowData = GetActiveArtilceRow();
            articleData.RemoveArticleRow(articleRowData);
            articleData.Update();
            return EditArticle();
        }
        public string MoveArticleRow()
        {
            var sourcerowref = _paramInfo.GetXmlProperty("genxml/hidden/sourcerowref");
            var destrowref = _paramInfo.GetXmlProperty("genxml/hidden/destrowref");

            var articleData = GetActiveArticle();

            var storeList = articleData.GetArticleRows();
            var sourceRow = articleData.GetArticleRow(sourcerowref);
            articleData.RemoveArticleRows();
            if (destrowref == "") articleData.UpdateArticleRow(sourceRow.Info.XMLData);
            foreach (var r in storeList)
            {
                if (r.ArticleRowRef != sourcerowref)
                {
                    articleData.UpdateArticleRow(r.Info.XMLData);
                    if (r.ArticleRowRef == destrowref)
                    {
                        articleData.UpdateArticleRow(sourceRow.Info.XMLData);
                    }
                }
            }

            articleData.Update();
            _passSettings.Add("saved", "true");
            return EditArticle();
        }
        public String GetPublicArticle()
        {
            var dataObjects = new Dictionary<string, object>();

            // check if we are looking for a detail page.  ("key" used as param in url)
            var razorTempl = "";
            var articleData = GetActiveArticle();
            var key = _paramInfo.GetXmlProperty("genxml/urlparam/" + _moduleParams.DetailUrlParam);
            if (key != "")
            {
                // do detail
                razorTempl = _moduleParams.AppTheme.GetTemplate("Detail.cshtml");
                if (razorTempl == "") return "No Razor Detail.cshtml Template.  ";
                dataObjects.Add("articlerow", articleData.GetArticleRow(key));
            }
            else
            {
                // do list
                razorTempl = _moduleParams.AppTheme.GetTemplate("View.cshtml");
                if (razorTempl == "") return "No Razor View.cshtml Template.";
            }

            dataObjects.Add("articledata", articleData);
            return RenderRazorUtils.RazorDetail(razorTempl, _moduleParams, dataObjects, new SessionParams(_paramInfo), _passSettings, true);

        }



        // ------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------


        public Dictionary<string, object> DownloadDocument()
        {
            var articleData = new ArticleLimpet(PortalUtils.GetCurrentPortalId(), _dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _currentLang);

            var documentref = _paramInfo.GetXmlProperty("genxml/hidden/document");
            var documentlistname = _paramInfo.GetXmlProperty("genxml/hidden/listname");
            var docInfo = articleData.Info.GetListItem(documentlistname, "genxml/lang/genxml/hidden/document", documentref);
            var filepath = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/reldocument");
            var namedocument = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/namedocument");
            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", filepath);
            rtnDic.Add("downloadname", namedocument);
            rtnDic.Add("fileext", "");
            return rtnDic;
        }


    }
}
