using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketMod
{
    public partial class StartConnect
    {

        public Dictionary<string, object> DownloadDocument()
        {
            var articleData = new ArticleLimpet(_dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _currentLang);

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
        public ArticleLimpet SaveArticle(bool doBackup)
        {
            var articleData = new ArticleLimpet(_dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _editLang);

            // do Backup
            if (doBackup) DoBackUp();

            // do Save
            _passSettings.Add("saved", "true");
            articleData.DebugMode = _systemData.DebugMode;
            articleData.Save(_postInfo);

            return articleData;
        }

        public String GetArticleSingle()
        {
            try
            {
                var articleData = new ArticleLimpet(_dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _nextLang);
                if (articleData.ArticleId <= 0) articleData.Update(); // create a record for this module.

                var strOut = "";

                articleData.ImageFolder = _dataModuleParams.ImageFolder;
                articleData.DocumentFolder = _dataModuleParams.DocumentFolder;
                articleData.AppThemeFolder = _dataModuleParams.AppThemeFolder;
                articleData.AppThemeVersion = _dataModuleParams.AppThemeVersion;
                articleData.AppThemeRelPath = _dataModuleParams.AppThemeFolderRel;
                articleData.AppThemeDataType = _dataAppThemeMod.AppTheme.DataType;

                foreach (var s in _moduleParams.ModuleSettings)
                {
                    if (!_passSettings.ContainsKey(s.Key)) _passSettings.Add(s.Key, s.Value);
                }

                var templateName = "editsingle.cshtml";

                var razorTempl = _dataAppThemeMod.AppTheme.GetTemplate(templateName);
                if (razorTempl == "") return "No '" + templateName + "' template found in " + _dataModuleParams.AppThemeFolder + " v" + _dataModuleParams.AppThemeVersion;
                strOut = RenderRazorUtils.RazorDetail(razorTempl, articleData, _passSettings, new SessionParams(_paramInfo), true);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public String GetDisplay()
        {
            try
            {
                var strOut = "";
                if (_moduleParams.Exists)
                {
                    var articleData = new ArticleLimpet(_dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _currentLang);

                    articleData.ImageFolder = _dataModuleParams.ImageFolder;
                    articleData.DocumentFolder = _dataModuleParams.DocumentFolder;
                    articleData.AppThemeFolder = _dataModuleParams.AppThemeFolder;
                    articleData.AppThemeVersion = _dataModuleParams.AppThemeVersion;
                    articleData.AppThemeRelPath = _dataModuleParams.AppThemeFolderRel;
                    articleData.AppThemeDataType = _dataAppThemeMod.AppTheme.DataType;

                    _passSettings.Add("datatype", _dataAppThemeMod.AppTheme.DataType.ToString());

                    foreach (var s in _moduleParams.ModuleSettings)
                    {
                        if (!_passSettings.ContainsKey(s.Key)) _passSettings.Add(s.Key, s.Value);
                    }

                    var templateName = "view.cshtml";

                    var razorTempl = _dataAppThemeMod.AppTheme.GetTemplate(templateName);
                    if (razorTempl == "") return "No '" + templateName + "' template found in " + _dataModuleParams.AppThemeFolder + " v" + _dataModuleParams.AppThemeVersion;
                    strOut = RenderRazorUtils.RazorDetail(razorTempl, articleData, _passSettings, new SessionParams(_paramInfo), true);

                    return strOut;

                }
                else
                {
                    strOut = GetSetup();
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        #region "Images"

        public string AddArticleImage()
        {
            var articleData = SaveArticle(false);
            var imgList = ImgUtils.MoveImageToFolder(_postInfo, _dataModuleParams.ImageFolderMapPath);
            foreach (var nam in imgList)
            {
                articleData.AddImage(nam, _dataModuleParams.ImageFolderRel);
            }
            articleData.Update();
            return GetArticleSingle();
        }


        #endregion

        #region "Documents"

        public string AddArticleDocument()
        {
            var articleData = SaveArticle(false);
            var docList = MoveDocumentToFolder(_postInfo, _dataModuleParams.DocumentFolderMapPath);
            foreach (var nam in docList)
            {
                articleData.AddDocument(nam, _dataModuleParams.DocumentFolderRel);
            }
            articleData.Update();
            return GetArticleSingle();
        }
        private List<string> MoveDocumentToFolder(SimplisityInfo postInfo, string destinationFolder, int maxDocs = 10)
        {
            destinationFolder = destinationFolder.TrimEnd('\\');
            var rtn = new List<string>();
            var userid = UserUtils.GetCurrentUserId(); // prefix to filename on upload.
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
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
                        if (docCount <= maxDocs)
                        {
                            var unqName = DNNrocketUtils.GetUniqueFileName(friendlyname.Replace(" ", "_"), destinationFolder);
                            var fname = destinationFolder + "\\" + unqName;
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

        #endregion

        #region "Links"

        public string AddArticleLink()
        {
            var articleData = SaveArticle(false);
            articleData.AddLink();
            articleData.Update();
            return GetArticleSingle();
        }


        #endregion

    }
}
