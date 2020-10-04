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
            var documentref = _paramInfo.GetXmlProperty("genxml/hidden/document");
            var documentlistname = _paramInfo.GetXmlProperty("genxml/hidden/listname");
            _articleLimpet = new ArticleLimpet(_articleid, _dataModuleParams.ModuleId, _editLang);
            var docInfo = _articleLimpet.Info.GetListItem(documentlistname, "genxml/lang/genxml/hidden/document", documentref);
            var filepath = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/reldocument");
            var namedocument = docInfo.GetXmlProperty("genxml/lang/genxml/hidden/namedocument");
            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("filenamepath", filepath);
            rtnDic.Add("downloadname", namedocument);
            rtnDic.Add("fileext", "");
            return rtnDic;
        }
        public void SaveArticle(bool doBackup)
        {
            // do Backup
            if (doBackup) DoBackUp();

            _articleLimpet = new ArticleLimpet(_articleid, _dataModuleParams.ModuleId, _editLang);

            // do Save
            _passSettings.Add("saved", "true");
            _articleLimpet.DebugMode = _systemData.DebugMode;
            _articleLimpet.Save(_postInfo);

            // We need to clear cache and sync before the langauge change.  Langauge is held in cache for the user.
            DNNrocketUtils.SynchronizeModule(_moduleid); // Modified Date
            CacheFileUtils.ClearAllCache();

            // change language (if changed, reload done in GetArticle() method )
            var nextLang = _paramInfo.GetXmlProperty("genxml/hidden/nextlang");
            if (nextLang != "") _editLang = DNNrocketUtils.SetEditCulture(nextLang);


        }

        public String GetArticleSingle()
        {
            try
            {

                var strOut = "";

                // The data record cannot be accessed by _articleId, so use moduleid on guidkey

                if (_articleLimpet == null) _articleLimpet = new ArticleLimpet(_dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _editLang);
                if (_articleLimpet.ArticleId <= 0) _articleLimpet.Update(); // create a record for this module.

                _articleLimpet.ImageFolder = _dataModuleParams.ImageFolder;
                _articleLimpet.DocumentFolder = _dataModuleParams.DocumentFolder;
                _articleLimpet.AppTheme = _dataModuleParams.AppThemeFolder;
                _articleLimpet.AppThemeVersion = _dataModuleParams.AppThemeVersion;
                _articleLimpet.AppThemeRelPath = _dataModuleParams.AppThemeFolderRel;
                _articleLimpet.AppThemeDataType = _dataAppThemeMod.AppTheme.DataType;


                foreach (var s in _moduleParams.ModuleSettings)
                {
                    if (!_passSettings.ContainsKey(s.Key)) _passSettings.Add(s.Key, s.Value);
                }

                var templateName = "editsingle.cshtml";

                var razorTempl = _dataAppThemeMod.AppTheme.GetTemplate(templateName);
                if (razorTempl == "") return "No '" + templateName + "' template found in " + _dataModuleParams.AppThemeFolder + " v" + _dataModuleParams.AppThemeVersion;
                strOut = RenderRazorUtils.RazorDetail(razorTempl, _articleLimpet, _passSettings, new SessionParams(_paramInfo), true);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public void RocketModAddListItem(string listname)
        {
            try
            {
                var articleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");
                if (articleid > 0)
                {
                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetData(_rocketInterface.EntityTypeCode, articleid, _editLang);
                    info.AddListItem(listname);
                    objCtrl.SaveData(info);
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        public String GetDisplay()
        {
            try
            {
                var strOut = "";
                if (_moduleParams.Exists)
                {
                    if (_articleLimpet == null) _articleLimpet = new ArticleLimpet(_dataModuleParams.ModuleRef, _dataModuleParams.ModuleId, _editLang);
                    if (_articleLimpet.ArticleId <= 0) _articleLimpet.Update(); // create a record for this module.

                    _articleLimpet.ImageFolder = _dataModuleParams.ImageFolder;
                    _articleLimpet.DocumentFolder = _dataModuleParams.DocumentFolder;
                    _articleLimpet.AppTheme = _dataModuleParams.AppThemeFolder;
                    _articleLimpet.AppThemeVersion = _dataModuleParams.AppThemeVersion;
                    _articleLimpet.AppThemeRelPath = _dataModuleParams.AppThemeFolderRel;
                    _articleLimpet.AppThemeDataType = _dataAppThemeMod.AppTheme.DataType;

                    _passSettings.Add("datatype", _dataAppThemeMod.AppTheme.DataType.ToString());

                    foreach (var s in _moduleParams.ModuleSettings)
                    {
                        if (!_passSettings.ContainsKey(s.Key)) _passSettings.Add(s.Key, s.Value);
                    }

                    var templateName = "view.cshtml";

                    var razorTempl = _dataAppThemeMod.AppTheme.GetTemplate(templateName);
                    if (razorTempl == "") return "No '" + templateName + "' template found in " + _dataModuleParams.AppThemeFolder + " v" + _dataModuleParams.AppThemeVersion;
                    strOut = RenderRazorUtils.RazorDetail(razorTempl, _articleLimpet, _passSettings, new SessionParams(_paramInfo), true);

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
            SaveArticle(false);
            var imgList = ImgUtils.MoveImageToFolder(_postInfo, _dataModuleParams.ImageFolderMapPath);
            foreach (var nam in imgList)
            {
                _articleLimpet.AddImage(nam, _dataModuleParams.ImageFolderRel);
            }
            return GetArticleSingle();
        }


        #endregion

        #region "Documents"

        public string AddArticleDocument()
        {
            SaveArticle(false);
            var docList = MoveDocumentToFolder(_postInfo, _dataModuleParams.DocumentFolderMapPath);
            foreach (var nam in docList)
            {
                _articleLimpet.AddDocument(nam, _dataModuleParams.DocumentFolderRel);
            }
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
                SaveArticle(false);
                _articleLimpet.AddLink();
                return GetArticleSingle();
        }


        #endregion

    }
}
