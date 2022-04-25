using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using RocketPortal.Components;
using Simplisity;

namespace RocketTools.API
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private RocketInterface _rocketInterface;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private UserParams _userParams;
        private AppThemeDNNrocketLimpet _appThemeSystem;
        private Dictionary<string, object> _dataObjects;
        private PortalLimpet _portalData;
        private SessionParams _sessionParams;
        private string _pageRef;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.
            var rtnDic = new Dictionary<string, object>();

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            if (UserUtils.IsSuperUser())
            {

                switch (paramCmd)
                {
                    case "rockettools_adminpanel":
                        strOut = AdminPanel();
                        break;


                    case "rockettools_login":
                        _userParams.TrackClear(_systemData.SystemKey);
                        strOut = ReloadPage();
                        break;
                    case "rocketroles_roles":
                        strOut = RolesAdmin();
                        break;
                    case "rocketroles_getmodules":
                        SaveTreeView("tabtreeview");
                        strOut = GetModules();
                        break;
                    case "rocketroles_getroles":
                        SaveModules("tabmodules");
                        strOut = GetRoles();
                        break;
                    case "rocketroles_applyroles":
                        SaveModules("tabmodules");
                        strOut = ApplyRoles();
                        break;
                    case "rocketroles_createdefaultroles":
                        DNNrocketUtils.CreateDefaultRocketRoles(PortalUtils.GetPortalId());
                        strOut = RolesAdmin();
                        break;


                    case "rocketclones_getdisplay":
                        strOut = CloneAdmin();
                        break;
                    case "rocketclones_getmodules":
                        strOut = GetCloneSelectModules();
                        break;
                    case "rocketclones_getdestination":
                        strOut = CloneDestination();
                        break;
                    case "rocketclones_clone":
                        SaveModules("clonemodules");
                        SaveTreeView("clonetreeview");
                        CloneModules();
                        strOut = ClonesOK();
                        break;

                    case "rocketlang_getdisplay":
                        strOut = LangAdmin();
                        break;
                    case "rocketlang_copy":
                        SaveSystems("languagesystemlist");
                        CopyLang();
                        strOut = LangAdmin();
                        break;
                    case "rocketlang_deletebackup":
                        strOut = DeleteBackUp();
                        break;
                    case "rocketlang_restorebackup":
                        strOut = RestoreBackUp();
                        break;
                    case "rocketlang_deletebackupall":
                        strOut = DeleteAllBackUp();
                        break;                        


                    case "rocketactions_getdisplay":
                        strOut = ActionAdmin();
                        break;
                    case "rocketactions_validate":
                        SaveSystems("validatesystemlist");
                        DoValidation();
                        strOut = ActionAdmin();
                        break;
                }
            }
            else
            {
                strOut = ReloadPage();
            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;

        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {

            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemData = new SystemLimpet("tools");
            _appThemeSystem = new AppThemeDNNrocketLimpet(_systemData.SystemKey);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _passSettings = new Dictionary<string, string>();
            _sessionParams = new SessionParams(_paramInfo);
            _userParams = new UserParams(_sessionParams.BrowserSessionId);

            // Assign Langauge
            DNNrocketUtils.SetCurrentCulture();
            if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
            if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
            DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
            DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

            _pageRef = _paramInfo.GetXmlProperty("genxml/hidden/pageref");

            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid == 0) portalid = PortalUtils.GetCurrentPortalId();
            _portalData = new PortalLimpet(portalid);

            // SECURITY --------------------------------
            if (!UserUtils.IsAdministrator()) return "rockettools_login";
            // SECURITY --------------------------------

            _dataObjects = new Dictionary<string, object>();
            _dataObjects.Add("appthemesystem", _appThemeSystem);
            _dataObjects.Add("portaldata", _portalData);
            _dataObjects.Add("systemdata", _systemData);

            return paramCmd;
        }

        private string AdminPanel()
        {
            var razorTempl = _appThemeSystem.GetTemplate("AdminPanel.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        private string ReloadPage()
        {
            // user does not have access, logoff
            UserUtils.SignOut();
            var razorTempl = _appThemeSystem.GetTemplate("Reload.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }


        #region "Language"
        public String LangAdmin()
        {
            var razorTempl = _appThemeSystem.GetTemplate("language.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public void CopyLang()
        {
            var info = GetCachedInfo();
            var copylanguage = _postInfo.GetXmlProperty("genxml/copylanguage");
            var destinationlanguage = _postInfo.GetXmlProperty("genxml/destinationlanguage");
            var backup = _postInfo.GetXmlProperty("genxml/checkbox/backup");            

            foreach (var s in info.GetRecordList("languagesystemlist"))
            {
                var systemkey = s.GetXmlProperty("genxml/systemkey");
                if (systemkey != "")
                {
                    var systemData = new SystemLimpet(systemkey);
                    if (systemData.Exists)
                    {
                        foreach (var rocketInterface in systemData.GetInterfaceList())
                        {
                            if (rocketInterface.Exists && rocketInterface.IsProvider("copylanguage"))
                            {
                                var paramInfo = new SimplisityInfo();
                                paramInfo.SetXmlProperty("genxml/hidden/destinationlanguage", destinationlanguage);
                                paramInfo.SetXmlProperty("genxml/hidden/copylanguage", copylanguage);
                                paramInfo.SetXmlProperty("genxml/hidden/backuprootfolder", _rocketInterface.InterfaceKey);
                                paramInfo.SetXmlProperty("genxml/checkbox/backup", backup);
                                paramInfo.SetXmlProperty("genxml/hidden/portalid", DNNrocketUtils.GetPortalId().ToString());
                                var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, new SimplisityInfo(), paramInfo, "", "");
                            }
                        }
                    }
                }
            }
        }

        private string DeleteBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            var backUpDataList = new BackUpDataList(_rocketInterface.InterfaceKey, "*_BackUp.xml");
            backUpDataList.DeleteBackUpFile(filemappath);
            return LangAdmin();
        }
        private string DeleteAllBackUp()
        {
            var backUpDataList = new BackUpDataList(_rocketInterface.InterfaceKey, "*_BackUp.xml");
            backUpDataList.DeleteAllBackUpFiles();
            return LangAdmin();
        }

        private string RestoreBackUp()
        {
            var filemappath = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/hidden/filemappath"));
            if (File.Exists(filemappath))
            {
                var backupData = new BackUpData(filemappath);
                backupData.RestoreData();
                CacheUtilsDNN.ClearAllCache();
                DNNrocketUtils.ClearAllCache();
            }

            return LangAdmin();
        }

        #endregion

        #region "Clones"
        public String CloneAdmin()
        {
            var razorTempl = _appThemeSystem.GetTemplate("clones.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public String GetCloneSelectModules()
        {
            var info = GetCachedInfo();
            info.GUIDKey = "";  // clear flag on new selection.


            info.RemoveRecordList("clonemodulelist");
            var tabid = _postInfo.GetXmlPropertyInt("genxml/fromtabid");
            info.SetXmlProperty("genxml/fromtabid", _postInfo.GetXmlProperty("genxml/fromtabid"));
            var pageData = new PageRecordData(DNNrocketUtils.GetPortalId(), tabid);
            var l = DNNrocketUtils.GetTabModuleTitles(tabid);
            foreach (var m in l)
            {
                var sRec = new SimplisityRecord();
                sRec.SetXmlProperty("genxml/moduleid", m.Key.ToString());
                sRec.SetXmlProperty("genxml/moduletitle", pageData.Name + ": " + m.Value);
                info.AddRecordListItem("clonemodulelist", sRec);
            }
            var razorTempl = _appThemeSystem.GetTemplate("clonesmodulesection.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public String CloneDestination()
        {
            var razorTempl = _appThemeSystem.GetTemplate("clonesdestination.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public void CloneModules()
        {
            var info = GetCachedInfo();
            var fromTabId = info.GetXmlPropertyInt("genxml/fromtabid");
            foreach (var m in info.GetRecordList("clonemodules"))
            {
                var moduleid = m.GetXmlPropertyInt("genxml/moduleid");
                if (moduleid > 0)
                {
                    foreach (var t in info.GetRecordList("clonetreeview"))
                    {
                        var toTabid = t.GetXmlPropertyInt("genxml/tabid");
                        DNNrocketUtils.CloneModule(moduleid, fromTabId, toTabid);
                    }
                }
            }
        }

        public String ClonesOK()
        {
            var razorTempl = _appThemeSystem.GetTemplate("clonesok.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }


        #endregion

        #region "Roles"

        public String GetRoles()
        {
            var info = GetCachedInfo();
            if (info.GUIDKey == "new") return "reload"; // we have lost the cache and page data, reload and start agian.

            info.RemoveRecordList("rolelist");
            var l = UserUtils.GetRoles(DNNrocketUtils.GetPortalId());
            foreach (var m in l)
            {
                var sRec = new SimplisityRecord();
                sRec.SetXmlProperty("genxml/roleid", m.Key.ToString());
                sRec.SetXmlProperty("genxml/rolename", m.Value);
                info.AddRecordListItem("rolelist", sRec);
            }
            var razorTempl = _appThemeSystem.GetTemplate("roleselectsection.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }        
        public String GetModules()
        {
            var info = GetCachedInfo();
            if (info.GUIDKey == "new") return "reload"; // we have lost the cache and page data, reload and start agian.

            info.RemoveRecordList("modulelist");
            foreach (var t in info.GetRecordList("tabtreeview"))
            {
                var tabid = t.GetXmlPropertyInt("genxml/tabid");
                var l = DNNrocketUtils.GetTabModuleTitles(tabid);
                foreach (var m in l)
                {
                    var sRec = new SimplisityRecord();
                    sRec.SetXmlProperty("genxml/moduleid", m.Key.ToString());
                    sRec.SetXmlProperty("genxml/moduletitle", t.GetXmlProperty("genxml/tabname") + ": " +  m.Value);
                    info.AddRecordListItem("modulelist", sRec);
                }
            }
            var razorTempl = _appThemeSystem.GetTemplate("rolesmodulesection.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, info, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String RolesAdmin()
        {
            var razorTempl = _appThemeSystem.GetTemplate("roles.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public String RolesOK()
        {
            var razorTempl = _appThemeSystem.GetTemplate("rolesok.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public string ApplyRoles()
        {
            var info = GetCachedInfo();
            foreach (var m in info.GetRecordList("tabmodules"))
            {
                var moduleid = m.GetXmlPropertyInt("genxml/moduleid");
                if (moduleid > 0)
                {
                    var nodList1 = _postInfo.XMLDoc.SelectNodes("genxml/rolecheckbox/*");
                    foreach (XmlNode nod1 in nodList1)
                    {
                        var strroleid = nod1.Name.Replace("roleid", "");
                        if (GeneralUtils.IsNumeric(strroleid))
                        {
                            var roleid = Convert.ToInt32(strroleid);
                            if (roleid > 0)
                            {
                                if (nod1.InnerText.ToLower() == "true")
                                {
                                    DNNrocketUtils.AddRoleToModule(DNNrocketUtils.GetPortalId(), moduleid, roleid);
                                }
                                else
                                {
                                    DNNrocketUtils.RemoveRoleToModule(DNNrocketUtils.GetPortalId(), moduleid, roleid);
                                }
                            }
                        }
                    }
                }
            }

            return RolesOK();
        }

        #endregion

        #region "Actions"
        public String ActionAdmin()
        {
            var razorTempl = _appThemeSystem.GetTemplate("actions.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }

        public void DoValidation()
        {
            var info = GetCachedInfo();

            foreach (var s in info.GetRecordList("validatesystemlist"))
            {
                var systemkey = s.GetXmlProperty("genxml/systemkey");
                if (systemkey != "")
                {
                    var systemData = new SystemLimpet(systemkey);
                    if (systemData.Exists)
                    {
                        foreach (var rocketInterface in systemData.GetInterfaceList())
                        {
                            if (rocketInterface.Exists && rocketInterface.IsProvider("validatedata"))
                            {
                                var paramInfo = new SimplisityInfo();
                                paramInfo.SetXmlProperty("genxml/hidden/portalid", DNNrocketUtils.GetPortalId().ToString());
                                var returnDictionary = DNNrocketUtils.GetProviderReturn(rocketInterface.DefaultCommand, systemData.SystemInfo, rocketInterface, new SimplisityInfo(), paramInfo, "", "");
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region "general"
        public SimplisityInfo GetCachedInfo()
        {
            var info = (SimplisityInfo)CacheUtilsDNN.GetCache(_pageRef);
            if (info == null)
            {
                info = new SimplisityInfo();
                info.GUIDKey = "new";  // flag to check if we have lost the previous selection
            }
            return info;
        }
        public void SaveSystems(string listName)
        {
            var info = GetCachedInfo();
            info.GUIDKey = "";  // clear flag on new selection.

            var nodList = _postInfo.XMLDoc.SelectNodes("genxml/checkbox/*");
            info.RemoveRecordList(listName);
            foreach (XmlNode nod in nodList)
            {
                if (nod.InnerText.ToLower() == "true")
                {
                    var sRec = new SimplisityRecord();
                    sRec.SetXmlProperty("genxml/elementid", nod.Name);
                    sRec.SetXmlProperty("genxml/systemid", nod.Name.Replace("systemid", ""));
                    info.AddRecordListItem(listName, sRec);
                }
            }
            CacheUtilsDNN.SetCache(_pageRef, info);
        }
        public void SaveModules(string listName)
        {
            var info = GetCachedInfo();
            info.GUIDKey = "";  // clear flag on new selection.

            var nodList = _postInfo.XMLDoc.SelectNodes("genxml/checkbox/*");
            info.RemoveRecordList(listName);
            foreach (XmlNode nod in nodList)
            {
                if (nod.InnerText.ToLower() == "true")
                {
                    var sRec = new SimplisityRecord();
                    sRec.SetXmlProperty("genxml/elementid", nod.Name);
                    sRec.SetXmlProperty("genxml/moduleid", nod.Name.Replace("moduleid", ""));
                    info.AddRecordListItem(listName, sRec);
                }
            }
            CacheUtilsDNN.SetCache(_pageRef, info);
        }

        public void SaveTreeView(string listName)
        {
            var info = GetCachedInfo();
            info.GUIDKey = "";  // clear flag on new selection.

            var nodList = _postInfo.XMLDoc.SelectNodes("genxml/treeview/*/*");
            info.RemoveRecordList(listName);
            foreach (XmlNode nod in nodList)
            {
                if (nod.InnerText.ToLower() == "true")
                {
                    var tabid = nod.Name.Replace("tabid", "");
                    if (GeneralUtils.IsNumeric(tabid))
                    {
                        var sRec = new SimplisityRecord();
                        sRec.SetXmlProperty("genxml/treeid", nod.Name);
                        sRec.SetXmlProperty("genxml/tabid", tabid);

                        var pageData = new PageRecordData(DNNrocketUtils.GetPortalId(), Convert.ToInt32(tabid));
                        sRec.SetXmlProperty("genxml/tabname", pageData.Name);

                        info.AddRecordListItem(listName, sRec);
                    }
                }
            }
            CacheUtilsDNN.SetCache(_pageRef, info);
        }

        #endregion
    }
}
