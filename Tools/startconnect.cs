﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;

namespace Rocket.Tools
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private RocketInterface _rocketInterface;
        private SystemLimpet _systemData;
        private Dictionary<string, string> _passSettings;
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private string _pageref;
        private UserParams _UserParams;
        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return nothing if not matching commands.
            var rtnDic = new Dictionary<string, object>();

            paramCmd = InitCmd(paramCmd, systemInfo, interfaceInfo, postInfo, paramInfo, langRequired);

            if (UserUtils.IsSuperUser())
            {

                switch (paramCmd)
                {
                    case "rockettools_login":
                        strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
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

                    case "rocketlicense_getaccount":
                        strOut = AccountAdmin();
                        break;

                    case "rocketaccount_savecompanykey":
                        SaveCompanyKey();
                        strOut = "";
                        break;

                }
            }
            else
            {
                strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;

        }

        public string InitCmd(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {

            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _pageref = _paramInfo.GetXmlProperty("genxml/hidden/pageref");

            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _passSettings = new Dictionary<string, string>();

            _UserParams = new UserParams(new SessionParams(_paramInfo).BrowserSessionId);

            if (_paramInfo.GetXmlPropertyBool("genxml/hidden/reload"))
            {
                var menucmd = _UserParams.GetCommand(_systemData.SystemKey);
                if (menucmd != "")
                {
                    paramCmd = menucmd;
                    _paramInfo = _UserParams.GetParamInfo(_systemData.SystemKey);
                    var interfacekey = _UserParams.GetInterfaceKey(_systemData.SystemKey);
                    _rocketInterface = new RocketInterface(systemInfo, interfacekey);
                }
            }
            else
            {
                if (_paramInfo.GetXmlPropertyBool("genxml/hidden/track"))
                {
                    _UserParams.Track(_systemData.SystemKey, paramCmd, _paramInfo, _rocketInterface.InterfaceKey);
                }
            }

            return paramCmd;
        }


        #region "Language"
        public String LangAdmin()
        {
            try
            {
                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "language.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void CopyLang()
        {
            var info = GetCachedInfo();
            var copylanguage = _postInfo.GetXmlProperty("genxml/copylanguage");
            var destinationlanguage = _postInfo.GetXmlProperty("genxml/destinationlanguage");
            var backup = _postInfo.GetXmlProperty("genxml/checkbox/backup");            

            foreach (var s in info.GetRecordList("languagesystemlist"))
            {
                var systemid = s.GetXmlPropertyInt("genxml/systemid");
                if (systemid > 0)
                {
                    var systemData = new SystemLimpet(systemid);
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
            try
            {
                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "clones.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String GetCloneSelectModules()
        {
            try
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

                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "clonesmodulesection.cshtml";

                CacheUtilsDNN.SetCache(_pageref, info);

                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, info, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String CloneDestination()
        {
            try
            {
                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "clonesdestination.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
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
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "clonesok.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        #endregion

        #region "Roles"

        public String GetRoles()
        {
            try
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

                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "roleselectsection.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, info, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }        
        public String GetModules()
        {
            try
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

                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "rolesmodulesection.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, info, _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String RolesAdmin()
        {
            try
            {
                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "roles.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public String RolesOK()
        {
            try
            {
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "rolesok.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
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
            try
            {
                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "actions.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void DoValidation()
        {
            var info = GetCachedInfo();

            foreach (var s in info.GetRecordList("validatesystemlist"))
            {

                var systemid = s.GetXmlPropertyInt("genxml/systemid");
                if (systemid > 0)
                {
                    var systemData = new SystemLimpet(systemid);
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


        #region "Account"

        public String AccountAdmin()
        {
            try
            {
                _passSettings.Add("portalid", DNNrocketUtils.GetPortalId().ToString());
                var controlRelPath = _rocketInterface.TemplateRelPath;
                var themeFolder = _rocketInterface.DefaultTheme;
                var razortemplate = "account.cshtml";
                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, controlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                return RenderRazorUtils.RazorDetail(razorTempl, new SimplisityInfo(), _passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private void SaveCompanyKey()
        {
            var webSiteData = new WebSiteData(false);
            webSiteData.CompanyKey = _postInfo.GetXmlProperty("genxml/companykey");
            webSiteData.Update();
        }


        #endregion

        #region "general"
        public SimplisityInfo GetCachedInfo()
        {
            var info = (SimplisityInfo)CacheUtilsDNN.GetCache(_pageref);
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
            CacheUtilsDNN.SetCache(_pageref, info);
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
            CacheUtilsDNN.SetCache(_pageref, info);
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
            CacheUtilsDNN.SetCache(_pageref, info);
        }

        #endregion
    }
}