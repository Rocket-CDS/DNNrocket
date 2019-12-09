using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

namespace DNNrocket.SystemData
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private static SystemInfoData _systemInfoData;
        private static string _controlRelPath;
        private static SimplisityInfo _postInfo;
        private static SimplisityInfo _paramInfo;
        private static DNNrocketInterface _rocketInterface;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang = "")
        {
            _postInfo = postInfo;
            _paramInfo = paramInfo;
            _systemInfoData = new SystemInfoData(systemInfo);
            _rocketInterface = new DNNrocketInterface(interfaceInfo);
            var commandSecurity = new CommandSecurity(-1, -1, _rocketInterface);

            DNNrocketUtils.CreateRocketDirectories();

            //CacheUtils.ClearAllCache();

            _controlRelPath = "/DesktopModules/DNNrocket/SystemData";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemkey is defined. [SystemData]";

            var rtnInfo = new SimplisityInfo();
            // Security Check MUST be in the extension.
            if (commandSecurity.SecurityCheckIsSuperUser())
            {
                switch (paramCmd)
                {
                    case "systemapi_admin_getsystemlist":
                        strOut = SystemAdminList(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_admin_getdetail":
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_adminaddnew":
                        strOut = SystemAddNew(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_addinterface":
                        SystemAddListItem(paramInfo, "interfacedata");
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_admin_save":
                        SystemSave(postInfo, paramInfo);
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_admin_delete":
                        SystemDelete(paramInfo);
                        strOut = SystemAdminList(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_addparam":
                        SystemAddListItem(paramInfo, "idxfielddata");
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_addsetting":
                        SystemAddListItem(paramInfo, "settingsdata");
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_addgroup":
                        SystemAddListItem(paramInfo, "groupsdata");
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_addprovtype":
                        SystemAddListItem(paramInfo, "provtypesdata");
                        strOut = SystemAdminDetail(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_rebuildindex":
                        RebuildIndex(paramInfo, false);
                        strOut = "<h1>Rebuilding Index</h1>";
                        break;
                    case "systemapi_deleterebuildindex":
                        RebuildIndex(paramInfo, true);
                        strOut = "<h1>Deleting and Rebuilding Index</h1>";
                        break;
                    case "systemapi_copyinterface":
                        strOut = CopyInterface(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_clearallcache":
                        CacheUtils.ClearAllCache();
                        DNNrocketUtils.ClearAllCache();
                        strOut = SystemAdminList(paramInfo, _controlRelPath);
                        break;
                    case "systemapi_globaldetail":
                        strOut = SystemGlobalDetail();
                        break;
                    case "systemapi_globalsave":
                        SystemGlobalSave();
                        strOut = SystemGlobalDetail();
                        break;

                    case "systemapi_licenselist":
                        strOut = GetLicenseList();
                        break;
                    case "systemapi_licensesave":
                        strOut = GetLicenseList();
                        break;                       
                }
            }

            switch (paramCmd)
            {
                case "login_sendreset":
                    //strOut = ResetPass(sInfo);
                    break;
                case "systemapi_licenserecieveremote":
                    strOut = "FAIL";
                    var licenseXml = _paramInfo.GetXmlProperty("genxml/postform/licensecode");
                    if (SaveRemoteLicense(licenseXml)) strOut = "OK";
                    break;
                case "systemapi_licenseverify":
                    strOut = "<i class='fas fa-times w3-text-red w3-right fa-3x'></i>";
                    if (verifyLicense()) strOut = "<i class='fas fa-check w3-text-green w3-right fa-3x'></i>";
                    break;
                case "systemapi_entercertificatekey":
                    EnterCertificateKey();
                    break;
                default:
                    if (!commandSecurity.SecurityCheckIsSuperUser())
                    {
                        strOut = UserUtils.LoginForm(systemInfo, rtnInfo, "systemapi", UserUtils.GetCurrentUserId());
                    }
                    break;
            }

            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;            
        }

        public static bool GetRemoteLicense()
        {
            try
            {
                var systemGlobalData = new SystemGlobalData();
                var reqparm = new NameValueCollection();
                reqparm.Add("sitekey", DNNrocketUtils.SiteGuid());
                reqparm.Add("systemkey", _systemInfoData.SystemKey);
                reqparm.Add("domainurl", DNNrocketUtils.GetDefaultWebsiteDomainUrl());
                var rtnLicenseStatus = SimplisityUtils.PostData(systemGlobalData.LicenseUrl.TrimEnd('/') + "/DesktopModules/DNNrocket/api/api.ashx", "rocketlicense", "clientlicense_getlicense", "", "", reqparm);
                if (rtnLicenseStatus == "OK") return true;
                return false;
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
                return false;
            }
        }

        public static bool verifyLicense()
        {
            try
            {
                var getremote = GetRemoteLicense();
                if (!getremote) return false;
                var licensecode = _paramInfo.GetXmlProperty("genxml/postform/licensecode");
                if (licensecode != "")
                {
                    var licenseInfo = new SimplisityInfo();
                    licenseInfo.FromXmlItem(GeneralUtils.DeCode(licensecode));
                    var sitekey = licenseInfo.GetXmlProperty("genxml/textbox/sitekey");
                    var systemkey = licenseInfo.GetXmlProperty("genxml/select/systemkey");
                    var licenseData = new LicenseData(systemkey, sitekey);
                    return licenseData.ValidateCertificateKey(sitekey);
                }
                return false;
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
                return false;
            }
        }

        public static bool SaveRemoteLicense(string licenseXml)
        {
            try
            {
                if (licenseXml != "")
                {
                    licenseXml = GeneralUtils.DeCode(licenseXml);
                    var sRec = new SimplisityRecord();
                    sRec.FromXmlItem(licenseXml);

                    var systemkey = sRec.GetXmlProperty("genxml/select/systemkey");
                    var sitekey = sRec.GetXmlProperty("genxml/textbox/sitekey");
                    var licenseData = new LicenseData(systemkey, sitekey);
                    if (licenseData.Exists)
                    {
                        licenseData.Record.XMLData = sRec.XMLData;
                        licenseData.Update();
                    }
                    else
                    {
                        var objCtrl = new DNNrocketController();
                        sRec.ItemID = -1;
                        objCtrl.Update(sRec);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
                return false;
            }
        }

        public static void EnterCertificateKey()
        {
            try
            {
                var certificateKey = _postInfo.GetXmlProperty("genxml/textbox/certificatekey");
                var domainurl = DNNrocketUtils.GetDefaultWebsiteDomainUrl(); ;
                var systemkey = _systemInfoData.SystemKey;
                var sitekey = DNNrocketUtils.SiteGuid();

                var licenseData = new LicenseData(systemkey, sitekey);
                if (certificateKey == "")
                {
                    licenseData.Delete();
                }
                else
                {
                    licenseData.DomainUrl = domainurl;
                    licenseData.CreateNew(-1);
                    licenseData.CertificateKey = certificateKey;
                    licenseData.Update();
                }
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
            }

        }


        public static String GetLicenseList()
        {
            try
            {
                var LicenseListData = new LicenseListData();
                var razorTempl = DNNrocketUtils.GetRazorTemplateData("Admin_SystemLicense.cshtml", _controlRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                var passSettings = _postInfo.ToDictionary();
                return DNNrocketUtils.RazorDetail(razorTempl, LicenseListData, passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }


        public static string SystemAdminList(SimplisityInfo sInfo, string templateControlRelPath)
        {
            try
            {
                var systemData = new DNNrocketAPI.SystemData();
                var list = systemData.GetSystemList();
                return RenderSystemAdminList(list, sInfo, 0, templateControlRelPath);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String SystemAdminDetail(SimplisityInfo sInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "Invalid ItemId";
                var selecteditemid = sInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                if (selecteditemid > 0)
                {
                    var passSettings = sInfo.ToDictionary();


                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetInfo(selecteditemid);

                    strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);
                }


                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String SystemGlobalDetail()
        {
            try
            {
                var passSettings = _paramInfo.ToDictionary();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData("Admin_SystemGlobalDetail.cshtml", _controlRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                var globalData = new SystemGlobalData();

                var strOut = DNNrocketUtils.RazorDetail(razorTempl, globalData, passSettings);
                return strOut;
            }
            catch (Exception ex)
            {
                DNNrocketUtils.LogException(ex);
                return ex.ToString();
            }
        }

        public static void SystemGlobalSave()
        {
            var globalData = new SystemGlobalData();
            globalData.Save(_postInfo);

            CacheUtils.ClearAllCache();
        }



        public static String RenderSystemAdminList(List<SimplisityInfo> list, SimplisityInfo sInfo, int recordCount, string templateControlRelPath)
        {

            try
            {
                if (list == null) return "";
                var strOut = "";

                // select a specific entity data type for the product (used by plugins)
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                if (themeFolder == "") themeFolder = "config-w3";
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = sInfo.ToDictionary();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                strOut = DNNrocketUtils.RazorList(razorTempl, list, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public static String SystemAddNew(SimplisityInfo sInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                if (themeFolder == "") themeFolder = "config-w3";
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = sInfo.ToDictionary();

                var info = new SimplisityInfo();
                info.ItemID = -1;
                info.PortalId = 99999;
                info.Lang = DNNrocketUtils.GetCurrentCulture();
                info.SetXmlProperty("genxml/hidden/index", "99");
                info.TypeCode = "SYSTEM";
                info.GUIDKey = GeneralUtils.GetUniqueKey();
                var objCtrl = new DNNrocketController();
                info.ItemID = objCtrl.SaveRecord(info).ItemID;

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);

                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void SystemAddListItem(SimplisityInfo sInfo, string listname)
        {
            try
            {
                var selecteditemid = sInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                if (GeneralUtils.IsNumeric(selecteditemid))
                {
                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetInfo(Convert.ToInt32(selecteditemid));
                    info.AddListItem(listname);
                    objCtrl.SaveRecord(info);
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }


        public static void SystemSave(SimplisityInfo postInfo, SimplisityInfo paramInfo)
        {
            // remove any debug logs created in debug mode.
            DNNrocketUtils.LogDebugClear();

            //remove any params
            postInfo.RemoveXmlNode("genxml/postform");
            postInfo.RemoveXmlNode("genxml/urlparams");

            var selecteditemid = paramInfo.GetXmlProperty("genxml/hidden/selecteditemid");
            if (GeneralUtils.IsNumeric(selecteditemid))
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(Convert.ToInt32(selecteditemid));
                info.XMLData = postInfo.XMLData;
                info.GUIDKey = postInfo.GetXmlProperty("genxml/textbox/ctrlkey");

                if (info.GetXmlProperty("genxml/textbox/defaultinterface") == "")
                {
                    info.SetXmlProperty("genxml/textbox/defaultinterface", info.GetXmlProperty("genxml/interfacedata/genxml[1]/textbox/interfacekey"));
                }

                objCtrl.SaveRecord(info);

                // Capture existing SYSTEMLINK records, so we can selectivly delete. To protect the system during operation, so records are always there.
                var systemlinklist = objCtrl.GetList(info.PortalId, -1, "SYSTEMLINK");
                var systemlinkidxlist = objCtrl.GetList(info.PortalId, -1, "SYSTEMLINKIDX");
                var newsystemlinklist = new List<int>();
                var newsystemlinkidxlist = new List<int>();

                // make systemlink record
                var entityList = new List<string>();
                foreach (var i in info.GetList("idxfielddata"))
                {
                    var entitytypecode = i.GetXmlProperty("genxml/dropdownlist/entitytypecode");
                    var xreftypecode = i.GetXmlProperty("genxml/dropdownlist/xreftypecode");
                    var entityguidkey = entitytypecode;

                    if (!entityList.Contains(entityguidkey))
                    {
                        //build xref list
                        var xmldata = "<genxml>";
                        foreach (XmlNode xrefnod in info.XMLDoc.SelectNodes("genxml/idxfielddata/genxml[dropdownlist/entitytypecode/text()='" + entitytypecode + "']"))
                        {
                            xmldata += xrefnod.OuterXml;
                        }
                        xmldata += "</genxml>";

                        var idxinfo = objCtrl.GetByGuidKey(info.PortalId, info.ItemID, "SYSTEMLINK", entityguidkey); // use system id as moduleid
                        if (idxinfo == null)
                        {
                            idxinfo = new SimplisityInfo();
                            idxinfo.PortalId = info.PortalId;
                            idxinfo.TypeCode = "SYSTEMLINK";
                            idxinfo.GUIDKey = entityguidkey;
                            idxinfo.XMLData = xmldata;
                            idxinfo.ParentItemId = info.ItemID;
                            idxinfo.ModuleId = info.ItemID;
                            var itemid = objCtrl.SaveRecord(idxinfo).ItemID;
                            idxinfo.ItemID = itemid;
                        }
                        else
                        {
                            idxinfo.ParentItemId = info.ItemID;
                            idxinfo.XMLData = xmldata;
                            objCtrl.SaveRecord(idxinfo);
                        }
                        newsystemlinklist.Add(idxinfo.ItemID);
                        entityList.Add(entityguidkey);

                        // SYSTEMLINKIDX
                        foreach (XmlNode xrefnod in idxinfo.XMLDoc.SelectNodes("genxml/genxml"))
                        {
                            var i2 = new SimplisityRecord();
                            i2.XMLData = xrefnod.OuterXml;

                            var idxref = i2.GetXmlProperty("genxml/textbox/indexref");
                            var typecodeIdx = i2.GetXmlProperty("genxml/dropdownlist/xreftypecode");
                            if (typecodeIdx == "")
                            {
                                typecodeIdx = i2.GetXmlProperty("genxml/dropdownlist/entitytypecode");
                            }
                            var idxinfo2 = objCtrl.GetByGuidKey(info.PortalId, info.ItemID, "SYSTEMLINK" + typecodeIdx, idxref);
                            if (idxinfo2 == null)
                            {
                                idxinfo2 = new SimplisityInfo();
                                idxinfo2.PortalId = info.PortalId;
                                idxinfo2.TypeCode = "SYSTEMLINK" + typecodeIdx;
                                idxinfo2.GUIDKey = idxref;
                                idxinfo2.ParentItemId = idxinfo.ItemID;
                                idxinfo2.ModuleId = info.ItemID;
                                idxinfo2.XMLData = i2.XMLData;
                                idxinfo2.TextData = typecodeIdx;
                                var itemid = objCtrl.SaveRecord(idxinfo2).ItemID;
                                idxinfo2.ItemID = itemid;
                            }
                            else
                            {
                                idxinfo2.ParentItemId = idxinfo.ItemID;
                                idxinfo2.XMLData = i2.XMLData;
                                idxinfo2.TextData = typecodeIdx;
                                objCtrl.SaveRecord(idxinfo2);
                            }
                            newsystemlinkidxlist.Add(idxinfo2.ItemID);
                        }
                    }
                }

                // delete any that have been remove.
                foreach (var sl in systemlinklist)
                {
                    if (!newsystemlinklist.Contains(sl.ItemID))
                    {
                        objCtrl.Delete(sl.ItemID);
                    }
                }
                foreach (var sl in systemlinkidxlist)
                {
                    if (!newsystemlinkidxlist.Contains(sl.ItemID))
                    {
                        objCtrl.Delete(sl.ItemID);
                    }
                }




                CacheUtils.ClearAllCache();
            }
        }

        public static void SystemDelete(SimplisityInfo sInfo)
        {
            var itemid = sInfo.GetXmlProperty("genxml/hidden/selecteditemid");
            if (GeneralUtils.IsNumeric(itemid))
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(Convert.ToInt32(itemid));

                CacheUtils.ClearAllCache();
            }
        }


        public static void RebuildIndex(SimplisityInfo postInfo, bool deleteindex )
        {
            var itemid = postInfo.GetXmlProperty("genxml/hidden/selecteditemid");
            if (GeneralUtils.IsNumeric(itemid))
            {
                var objCtrl = new DNNrocketController();
                var sysInfo = objCtrl.GetInfo(Convert.ToInt32(itemid));

                var entityList = new List<string>();
                // get interface data, the language needs indexing.
                foreach (var i in sysInfo.GetList("interfacedata"))
                {
                    var entityTypeCode = i.GetXmlProperty("genxml/textbox/entitytypecode");
                    if (!entityList.Contains(entityTypeCode) && entityTypeCode != "")
                    {
                        entityList.Add(entityTypeCode);
                    }
                }
                foreach (var i in sysInfo.GetList("idxfielddata"))
                {
                    var entityTypeCode = i.GetXmlProperty("genxml/dropdownlist/entitytypecode");
                    if (!entityList.Contains(entityTypeCode) && entityTypeCode != "")
                    {
                        entityList.Add(entityTypeCode);
                    }
                }
                foreach (var entityCode in entityList)
                {
                    if (deleteindex)
                    {
                        var l2 = objCtrl.GetList(-1, -1, entityCode);
                        foreach (var sInfo in l2)
                        {
                            objCtrl.DeleteIndex(sInfo);
                        }
                    }
                    var l = objCtrl.GetList(-1, -1, entityCode + "LANG","","","",0,0,0,0);
                    if (l.Count == 0)
                    {
                        // the record may NOT have a LANG record.
                        l = objCtrl.GetList(-1, -1, entityCode, "", "", "", 0, 0, 0, 0);
                        foreach (var sInfo in l)
                        {
                            objCtrl.RebuildIndex(sInfo);
                        }
                    }
                    else
                    {
                        foreach (var sInfo in l)
                        {
                            objCtrl.RebuildLangIndex(sInfo.PortalId, sInfo.ItemID);
                        }
                    }
                }

                CacheUtils.ClearAllCache();
            }

        }


        public static string CopyInterface(SimplisityInfo postInfo, string templateControlRelPath)
        {
            var strOut = "";
            var info = new SimplisityInfo();
            info.SetXmlProperty("genxml/delay", "2000");
            var itemid = postInfo.GetXmlProperty("genxml/hidden/fromsystemid");
            if (GeneralUtils.IsNumeric(itemid))
            {
                var objCtrl = new DNNrocketController();
                var sysInfo = objCtrl.GetInfo(Convert.ToInt32(itemid));
                if (sysInfo != null)
                {
                    var interfacekey = postInfo.GetXmlProperty("genxml/hidden/interfacekey");
                    var tosystemid = postInfo.GetXmlProperty("genxml/hidden/tosystemid");

                    var sysInfoTo = objCtrl.GetInfo(Convert.ToInt32(tosystemid));
                    if (sysInfoTo != null)
                    {
                        var interfaceToCopy = sysInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfacekey);
                        if (interfaceToCopy != null)
                        {
                            var interfaceExists = sysInfoTo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfacekey);
                            if (interfaceExists == null)
                            {
                                sysInfoTo.AddListItem("interfacedata", interfaceToCopy);
                                objCtrl.SaveRecord(sysInfoTo);
                                info.SetXmlProperty("genxml/message", "Interface Copied");
                                info.SetXmlProperty("genxml/color", "w3-pale-green");
                                info.SetXmlProperty("genxml/delay", "2000");
                            }
                            else
                            {
                                interfaceToCopy.SetXmlProperty("genxml/textbox/interfacekey", interfaceToCopy.GetXmlProperty("genxml/textbox/interfacekey") + "-copy");
                                sysInfoTo.AddListItem("interfacedata", interfaceToCopy);
                                objCtrl.SaveRecord(sysInfoTo);
                                info.SetXmlProperty("genxml/message", "Interface Copied - Refresh page to View");
                                info.SetXmlProperty("genxml/color", "w3-pale-green");
                                info.SetXmlProperty("genxml/delay", "2000");
                            }

                        }
                        else
                        {
                            info.SetXmlProperty("genxml/message", "Interface does not exists.");
                            info.SetXmlProperty("genxml/color", "w3-pale-red");
                        }
                    }
                    else
                    {
                        info.SetXmlProperty("genxml/message", "System does not exists.");
                        info.SetXmlProperty("genxml/color", "w3-pale-red");
                    }
                }
                CacheUtils.ClearAllCache();
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info);

            }
            return strOut;
        }

    }
}
