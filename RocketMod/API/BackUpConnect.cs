using DNNrocketAPI;
using DNNrocketAPI.Components;
using Rocket.AppThemes.Componants;
using RocketMod.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RocketMod
{
    public partial class StartConnect
    {

        #region "BackUP"

        private String GetBackUp()
        {
            try
            {
                _rocketInterface.Info.ModuleId = _moduleid;
                if (_passSettings.ContainsKey("searchpattern")) _passSettings.Remove("searchpattern");
                _passSettings.Add("searchpattern", "*_backup.xml");
                var razorTempl = RenderRazorUtils.GetRazorTemplateData("backup.cshtml", _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, _rocketInterface.Info, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string DeleteBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_BackUp.xml");
            backUpDataList.DeleteBackUpFile(filemappath);
            return GetBackUp();
        }
        private string DeleteTemplateBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_templates.xml");
            backUpDataList.DeleteBackUpFile(filemappath);
            return GetAppModTheme();
        }
        private string DeleteAllBackUp()
        {
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_BackUp.xml");
            backUpDataList.DeleteAllBackUpFiles();
            return GetBackUp();
        }
        private string DeleteAllTemplateBackUp()
        {
            var backUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_templates.xml");
            backUpDataList.DeleteAllBackUpFiles();
            return GetAppModTheme();
        }

        private string RestoreTemplateBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {
                var backupData = new BackUpModuleTemplates(filemappath, _moduleid, _systemKey);
                backupData.RestoreData();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }
            return GetAppModTheme();
        }
        private string SaveBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {

                var backupData = new BackUpData(filemappath);
                backupData.BackUpText = _postInfo.GetXmlProperty("backup/backuptext");
                backupData.Save();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }

            return GetBackUp();
        }

        private string RestoreBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {
                DoBackUp();

                var backupData = new BackUpData(filemappath);
                backupData.RestoreData();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }

            return GetBackUp();
        }


        private void DoBackUp(bool forcebackup = false)
        {
            // BackUp data to file 
            if ((_moduleParams.Exists && _moduleParams.AutoBackUp) || forcebackup)
            {
                var saveList = new List<SimplisityInfo>();
                if (DNNrocketUtils.ModuleExists(_moduleParams.TabId, _moduleid) && !DNNrocketUtils.ModuleIsDeleted(_moduleParams.TabId, _moduleParams.ModuleId))
                {
                    var exportData = new ExportLimpet(_rocketInterface, _moduleParams.ModuleId, _moduleParams.SystemKey);
                    foreach (var s in exportData.GetList())
                    {
                        saveList.Add(s);
                    }
                }

                // check if previous backup is same
                var BackUpDataList = new BackUpDataList("rocketmod" + _moduleid, "*_BackUp.xml");

                var fileNameTemp = PortalUtils.TempDirectoryMapPath() + "\\" + ("rocketmod" + _moduleid + GeneralUtils.GetUniqueKey());
                var backupData = new BackUpData(fileNameTemp);
                backupData.BackUp(saveList);

                CacheUtils.SetCache("backupidentical" + _moduleid, "");

                var l = BackUpDataList.GetBackUpFileMapPathList("dnnrocketmodule");
                if (l.Count > 0)
                {
                    var lastBackupFileMapPath = l.First(); // take first file.  If the system returns in wrong order the file compare may fail.
                    try
                    {
                        if (!FileUtils.CompareAreSame(lastBackupFileMapPath, fileNameTemp) || forcebackup)
                        {
                            // move new file and remove temp file
                            var fileName = DNNrocketUtils.BackUpNewFileName("rocketmod" + _moduleid, _systemKey);
                            File.Copy(fileNameTemp, fileName);
                        }
                        else
                        {
                            CacheUtils.SetCache("backupidentical" + _moduleid, DateTime.Now.ToString("O"));
                        }
                        File.Delete(fileNameTemp);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                else
                {
                    // move new file and remove temp file
                    var fileName = DNNrocketUtils.BackUpNewFileName("rocketmod" + _moduleid, _systemKey);
                    File.Copy(fileNameTemp, fileName);
                }
            }
        }

        private void DoTemplateBackUp()
        {
            var fileMapPath = DNNrocketUtils.BackUpNewFileName("rocketmod" + _moduleid, _systemKey, "Templates.xml");
            var backupTemplates = new BackUpModuleTemplates(fileMapPath, _moduleid, _systemKey);
            backupTemplates.BackUp();
        }
        private Dictionary<string, object> DownloadBackUp()
        {
            var rtnDic = new Dictionary<string, object>();
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/filemappath"));
            if (File.Exists(filemappath))
            {
                var backupData = new BackUpModuleTemplates(filemappath, _moduleid, _systemKey);

                rtnDic.Add("filenamepath", backupData.FileMapPath);
                rtnDic.Add("downloadname", "DataBackUp" + backupData.ModuleId + ".xml");
            }
            return rtnDic;
        }


        #endregion


    }
}
