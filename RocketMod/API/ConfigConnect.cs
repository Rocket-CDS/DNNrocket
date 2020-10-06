using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using RocketMod.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketMod
{
    public partial class StartConnect
    {
        private void SaveConfig()
        {
            // do Backup
            DoBackUp();

            var appTheme = new AppTheme(_systemData.SystemKey, _moduleParams.AppThemeFolder);
            _moduleParams.AppThemeLogo = appTheme.Logo;
            _moduleParams.Name = _postInfo.GetXmlProperty("genxml/hidden/name");
            _moduleParams.ImageFolder = _postInfo.GetXmlProperty("genxml/hidden/imagefolder");
            _moduleParams.DocumentFolder = _postInfo.GetXmlProperty("genxml/hidden/documentfolder");
            _moduleParams.AppThemeVersion = _postInfo.GetXmlProperty("genxml/hidden/appthemeversion");
            _moduleParams.AppThemeNotes = _postInfo.GetXmlProperty("genxml/hidden/appthemenotes");
            _moduleParams.DetailUrlParam = GeneralUtils.UrlFriendly(_postInfo.GetXmlProperty("genxml/hidden/detailurlparam"));
            _moduleParams.DetailView = _postInfo.GetXmlPropertyBool("genxml/hidden/detailview");
            _moduleParams.ModuleType = "RocketMod";
            _moduleParams.CacheDisbaled = _postInfo.GetXmlPropertyBool("genxml/hidden/disablecache");
            _moduleParams.AutoBackUp = _postInfo.GetXmlPropertyBool("genxml/hidden/autobackup");

            if (_systemData.DebugMode) _moduleParams.CacheDisbaled = true;

            _moduleParams.ShareData = _postInfo.GetXmlProperty("genxml/hidden/sharedata");
            _moduleParams.TabId = _tabid;
            _moduleParams.SystemKey = _systemData.SystemKey;
            _moduleParams.ExportResourceFiles = _postInfo.GetXmlPropertyBool("genxml/hidden/exportresourcefiles");

            _moduleParams.Save();
            _passSettings.Remove("saved");
            _passSettings.Add("saved", "true");

            // update module with a better name
            DNNrocketUtils.UpdateModuleTitle(_tabid, _moduleid, _moduleParams.Name + ":" + _moduleid);

            CacheFileUtils.ClearAllCache();


        }

        public String ResetRocketMod(int moduleid)
        {
            try
            {
                _moduleParams.Delete();

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("moduleid" + moduleid, "ROCKETMODFIELDS", "", moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);
                info = objCtrl.GetData("moduleid" + moduleid, "ROCKETMODSETTINGS", "", moduleid, true);
                if (info != null) objCtrl.Delete(info.ItemID);

                return GetSelectApp();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String ResetDataRocketMod(int moduleid)
        {
            try
            {
                var mParams = new ModuleParams(moduleid);
                var articleData = new ArticleLimpet(mParams.ModuleRef, mParams.ModuleId, _nextLang);
                articleData.Delete();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetDashBoard()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "dashboard.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _moduleParams, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String GetDataSourceList()
        {
            try
            {
                var mp = new ModuleParamList(_systemKey, DNNrocketUtils.GetCurrentCulture(), true, true);
                if (_moduleParams.ShareData == "0") mp.DataList.Add(_moduleParams); // current to list, so we can assigned current module as data source.
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "selectdatasource.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, mp, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String GetDataSourceSelect()
        {
            try
            {
                var selectmoduleid = _paramInfo.GetXmlPropertyInt("genxml/hidden/selectedmoduleid");
                var selectmoduleref = _paramInfo.GetXmlProperty("genxml/hidden/selectedmoduleref");
                if (selectmoduleid == 0)
                {
                    selectmoduleid = _moduleid;
                    selectmoduleref = _moduleParams.ModuleRef;
                }
                else
                {
                    _moduleParams.ShareData = "0";
                }
                _moduleParams.ModuleIdDataSource = selectmoduleid;
                _moduleParams.ModuleRefDataSource = selectmoduleref;
                _moduleParams.Save();
                return GetDashBoard();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string ExportData()
        {
            var exportData = new ExportLimpet(_rocketInterface, _moduleid, _systemKey);
            return exportData.GetXml();
        }
        private void ImportData()
        {
            var oldmoduleid = _postInfo.GetXmlPropertyInt("export/moduleid");
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            var importData = new ImportLimpet(_rocketInterface, portalid, _moduleid, oldmoduleid, _postInfo.XMLData);
            CacheUtilsDNN.ClearAllCache();
            DNNrocketUtils.ClearAllCache();
        }

        private void CopyLanguage()
        {
            var objCtrl = new DNNrocketController();

            // data passed to system with _paramInfo
            var copylanguage = _paramInfo.GetXmlProperty("genxml/hidden/copylanguage");
            var destinationlanguage = _paramInfo.GetXmlProperty("genxml/hidden/destinationlanguage");
            var backup = _paramInfo.GetXmlPropertyBool("genxml/checkbox/backup");
            var backuprootfolder = _paramInfo.GetXmlProperty("genxml/hidden/backuprootfolder");

            if (destinationlanguage != copylanguage)
            {
                // BackUp data to file 
                if (backup)
                {
                    var saveList = new List<SimplisityInfo>();
                    var l2 = objCtrl.GetList(PortalUtils.GetPortalId(), -1, "MODULEPARAMS", " and r1.XmlData.value('(genxml/hidden/moduletype)[1]','nvarchar(max)') = 'RocketMod'");
                    foreach (var sInfo in l2)
                    {
                        var moduleParams = new ModuleParams(sInfo.ModuleId, _systemKey);
                        if (DNNrocketUtils.ModuleExists(moduleParams.TabId, sInfo.ModuleId) && !DNNrocketUtils.ModuleIsDeleted(moduleParams.TabId, sInfo.ModuleId))
                        {
                            var exportData = new ExportLimpet(_rocketInterface, moduleParams.ModuleId, moduleParams.SystemKey);
                            foreach (var s in exportData.GetList())
                            {
                                saveList.Add(s);
                            }
                        }
                    }
                    var fileName = DNNrocketUtils.BackUpNewFileName(backuprootfolder, _systemKey);
                    var backupData = new BackUpData(fileName);
                    backupData.BackUp(saveList);
                }


                // copy language
                var l = objCtrl.GetList(PortalUtils.GetPortalId(), -1, "ROCKETMODLANG", " and r1.Lang = '" + copylanguage + "'", "", "", 0, 0, 0, 0, _tableName);
                foreach (var sInfo in l)
                {
                    var objRecLang = objCtrl.GetRecordLang(sInfo.ParentItemId, destinationlanguage, _tableName);
                    if (objRecLang != null)
                    {
                        objRecLang.XMLData = sInfo.XMLData;
                        objCtrl.Update(objRecLang);

                        // recreate the IDX record.
                        var idxInfo = objCtrl.GetInfo(objRecLang.ParentItemId, objRecLang.Lang, _tableName);
                        objCtrl.RebuildIndex(idxInfo, _tableName);
                        objCtrl.RebuildLangIndex(idxInfo.PortalId, idxInfo.ItemID, _tableName);
                    }

                }

                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();

            }

        }


        private void ValidateData()
        {
            var objCtrl = new DNNrocketController();

            // remove deleted modules
            var filter = "and r1.XMlData.value('(genxml/hidden/systemkey)[1]','nvarchar(max)') = '" + _systemKey + "' ";
            var dirlist = objCtrl.GetList(PortalUtils.GetPortalId(), -1, "MODULEPARAMS", filter);
            foreach (var sInfo in dirlist)
            {
                var moduleParams = new ModuleParams(sInfo.ModuleId, _systemKey);
                if (moduleParams.ModuleId == sInfo.ModuleId && !DNNrocketUtils.ModuleExists(moduleParams.TabId, sInfo.ModuleId) && moduleParams.ModuleType.ToLower() == "rocketmod")
                {
                    ResetRocketMod(sInfo.ModuleId);
                    ResetDataRocketMod(sInfo.ModuleId);
                }
            }

        }
        private String GetSetup()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("setup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _rocketInterface.Info, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
