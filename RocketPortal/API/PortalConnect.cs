﻿using DNNrocketAPI.Components;
using Simplisity;
using System;
using RocketPortal.Components;
using System.Collections.Generic;

namespace RocketPortal.API
{
    public partial class StartConnect
    {
        private string CreatePortal()
        {
            var postName = _postInfo.GetXmlProperty("genxml/textbox/name");
            var portalurl = _postInfo.GetXmlProperty("genxml/textbox/engineurl");
            if (!GeneralUtils.IsUriValid(portalurl))
            {
                var globalData = new SystemGlobalData();
                if (globalData.RootDomain == "") return "Invalid Root Domain.  Update Global Settings.";
                portalurl = GeneralUtils.UrlFriendly(GeneralUtils.GetGuidKey()) + "." + globalData.RootDomain;
            }
            var engineurl = portalurl;

            var portalExists = false;
            foreach (var p in PortalUtils.GetPortals())
            {
                if (PortalUtils.GetPortalAliases(p).Contains(engineurl)) portalExists = true;
            }
            if (portalExists)
            {
                _passSettings.Add("portalurl", portalurl);
                var razorTempl = _appThemeSystem.GetTemplate("PortalExistsErr.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
            }

            int portalAdminUserId = -1;
            var userList = UserUtils.GetSuperUsers();
            if (userList.Count >= 1) portalAdminUserId = userList[0].GetXmlPropertyInt("user/userid");
            if (portalAdminUserId > 0)
            {
                var portalid = PortalUtils.CreatePortal(postName, engineurl, portalAdminUserId);
                if (portalid > 0)
                {
                    var buildconfigfile = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket/RocketPortal/WebsiteBuilds").TrimEnd('\\') + "\\" +   _postInfo.GetXmlProperty("genxml/hidden/buildconfig");
                    // add portal record
                    var portalData = new PortalLimpet(portalid);
                    portalData.Record.SetXmlProperty("genxml/textbox/name", postName);
                    var xmlData = FileUtils.ReadFile(buildconfigfile);
                    if (xmlData != "")
                    {
                        var buildconfig = new SimplisityRecord();
                        buildconfig.XMLData = xmlData;
                        portalData.Record.SetXmlProperty("genxml/portaltype", buildconfig.GetXmlProperty("genxml/type"));
                    }
                    portalData.Update();

                    PortalUtils.BuildPortal(portalid, portalAdminUserId, buildconfigfile);

                    _portalData = new PortalLimpet(portalid); // reload before display
                }
            }
            return GetPortalDetail();
        }
        private string SavePortal()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid"); 
            if (portalId >= 0)
            {
                _portalData.Save(_postInfo);
                return GetPortalDetail();
            }
            return "Invalid PortalId";
        }
        private string DeletePortal()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var PortalData = new PortalLimpet(portalId);
                //foreach (var s in PortalData.SystemDataList.GetSystemActiveList())
                //{
                //    // delete system data
                //    var systemData = SystemSingleton.Instance(s.SystemKey);
                //    var interfacekey = "rocketsystem";
                //    var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                //    if (systemData.Active)
                //    {
                //        var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_delete", systemData.SystemInfo, rocketInterface, _postInfo, _paramInfo, "/DesktopModules/DNNrocket/api", "");
                //    }
                //}
                PortalUtils.DeletePortal(portalId); 
                //DNNrocketUtils.RecycleApplicationPool();
                PortalData.Delete();
                _userParams.TrackClear(_systemData.SystemKey);
                return GetPortalList();
            }
            return GetPortalDetail();
        }
        private String GetPortalList()
        {
            try
            {
                var portalList = new PortalLimpetList(_paramInfo);
                var razorTempl = _appThemeSystem.GetTemplate("PortalList.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, portalList, _dataObjects, _passSettings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private String ValidatePortals()
        {
            if (!UserUtils.IsSuperUser()) return "";
            var portalList = new PortalLimpetList(_paramInfo);
            portalList.Validate();
            CacheFileUtils.ClearFileCacheAllPortals();
            return GetPortalDetail(); 
        }
        private String IndexPortals()
        {
            if (!UserUtils.IsSuperUser()) return "";
            var portalList = new PortalLimpetList(_paramInfo);
            portalList.Index();
            CacheFileUtils.ClearFileCacheAllPortals();
            return GetPortalDetail();
        }
        private String GetPortalDetail()
        {
            try
            {
                var razorTempl = _appThemeSystem.GetTemplate("PortalDetail.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string UnAuthoriseUser()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1) UserUtils.UnAuthoriseUser(portalId, userid);
            }
            return GetPortalDetail();
        }
        private string AuthoriseUser()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1) UserUtils.AuthoriseUser(portalId, userid);
            }
            return GetPortalDetail();
        }
        private string AddRole(string role)
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1)
                {
                    var roleRec = UserUtils.GetRoleByName(portalId, role);
                    UserUtils.AddUserRole(portalId, userid, roleRec.GetXmlPropertyInt("genxml/roleid"));
                }
            }
            return GetPortalDetail();
        }
        private string RemoveRole(string role)
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1)
                {
                    var roleRec = UserUtils.GetRoleByName(portalId, role);
                    UserUtils.RemoveUserRole(portalId, userid, roleRec.GetXmlPropertyInt("genxml/roleid"));
                }
            }
            return GetPortalDetail();
        }
        private string DeleteUser()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var userid = _paramInfo.GetXmlPropertyInt("genxml/hidden/userid");
                if (userid > 1) UserUtils.DeleteUser(portalId, userid);
            }
            return GetPortalDetail();
        }
        private string CreateManager()
        {
            var statusString = "";
            var portalid = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid >= 0)
            {
                try
                {
                    var manageremail = _postInfo.GetXmlProperty("genxml/textbox/email");
                    var managerpassword = _postInfo.GetXmlProperty("genxml/textbox/password");
                    if (managerpassword != "")
                    {
                        var userCount = UserUtils.GetUsers(portalid).Count;
                        if (userCount <= 10)
                        {
                            var adminUserCount = UserUtils.GetUsers(portalid, DNNrocketRoles.Administrators).Count;
                            statusString = UserUtils.CreateUser(portalid, manageremail, manageremail);
                            if (statusString == "" && managerpassword != "" && manageremail != "")
                            {
                                var userInfo = UserUtils.GetUserDataByEmail(portalid, manageremail);
                                if (userInfo != null)
                                {
                                    UserUtils.ChangePassword(portalid, userInfo.UserId, managerpassword);

                                    var rolelist = UserUtils.GetRoles(portalid);
                                    foreach (var r in rolelist)
                                    {
                                        if (r.Value == DNNrocketRoles.Manager)
                                        {
                                            UserUtils.AddUserRole(portalid, userInfo.UserId, r.Key);
                                        }
                                    }
                                    if (adminUserCount == 0)
                                    {
                                        var roleRec = UserUtils.GetRoleByName(portalid, DNNrocketRoles.Administrators);
                                        if (roleRec != null && roleRec.GetXmlPropertyInt("genxml/roleid") > 0) UserUtils.AddUserRole(portalid, userInfo.UserId, roleRec.GetXmlPropertyInt("genxml/roleid"));
                                    }
                                }
                                return GetPortalDetail();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            return AddManager(false, statusString);
        }
        private String AddManager(bool success = true, string statusMsg = "")
        {
            try
            {
                _passSettings.Add("success", success.ToString());
                _passSettings.Add("statusmsg", statusMsg);                
                var razorTempl = _appThemeSystem.GetTemplate("PortalAddManager.cshtml");
                var pr = RenderRazorUtils.RazorProcessData(razorTempl, _portalData, _dataObjects, _passSettings, _sessionParams, true);
                if (pr.StatusCode != "00") return pr.ErrorMsg;
                return pr.RenderedText;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string UpdateDefaultLangauge()
        {
            var defaultLanguage = _paramInfo.GetXmlProperty("genxml/hidden/defaultlanguage");
            _portalData.UpdateDefaultLanguage(defaultLanguage);
            return GetPortalDetail();
        }
        private string AddLangauge()
        {
            _portalData.AddLanguage(_paramInfo.GetXmlProperty("genxml/hidden/selectlanguage"));
            return GetPortalDetail();
        }
        private string RemoveLangauge()
        {
            _portalData.RemoveLanguage(_paramInfo.GetXmlProperty("genxml/hidden/selectlanguage"));
            return GetPortalDetail();
        }

        private string AddSystem()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var systemKey = _paramInfo.GetXmlProperty("genxml/hidden/systemkeyref");
                _portalData = new PortalLimpet(portalId);
                _portalData.Update();

                // Setup pages.
                if (systemKey != "")
                {
                    // Create the system record in the DB.
                    var systemData = SystemSingleton.Instance(systemKey);
                    if (!systemData.Exists)
                    {
                        var interfacekey = "rocketsystem";
                        var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                        _paramInfo.SetXmlProperty("genxml/hidden/newportalid", portalId.ToString());
                        var returnDictionary = DNNrocketUtils.GetProviderReturn("rocketsystem_init", systemData.SystemInfo, rocketInterface, _postInfo, _paramInfo, "/DesktopModules/DNNrocket/api", "");
                    }
                }
                return GetPortalDetail();
            }
            return "Invalid PortalId";
        }
        private string RemoveSystem()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var systemKey = _paramInfo.GetXmlProperty("genxml/hidden/systemkeyref");
                return GetPortalDetail();
            }
            return "Invalid PortalId";
        }
        private string ToggleSystem()
        {
            var portalId = _paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalId >= 0)
            {
                var systemKey = _paramInfo.GetXmlProperty("genxml/hidden/systemkeyref");
                //var systemFlag = _portalData.Record.GetXmlPropertyBool("genxml/systems/" + systemKey);
                //if (systemFlag)
                //    return RemoveSystem();
                //else
                //    return AddSystem();
            }
            return "Invalid PortalId";
        }
        private String ResetSecuirtyPortal()
        {
            _portalData.ResetSecurity();
            return GetPortalDetail();
        }
        private String ResetCodes()
        {
            _portalData.ResetCodes();
            return GetPortalDetail();
        }

    }

}
