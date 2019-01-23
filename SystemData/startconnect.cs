using DNNrocket.Login;
using DNNrocketAPI;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using Simplisity;
using System;
using System.Collections.Generic;

namespace DNNrocket.SystemData
{
    public class startconnect : DNNrocketAPI.APInterface
    {

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo,string userHostAddress, string editlang = "")
        {

            //CacheUtils.ClearAllCache();

            var controlRelPath = "/DesktopModules/DNNrocket/SystemData";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [SystemData]";

            var rtnInfo = new SimplisityInfo();
            if (UserController.Instance.GetCurrentUserInfo().IsSuperUser)
            {
                if (paramCmd == "systemapi_signout")
                {
                    var ps = new PortalSecurity();
                    ps.SignOut();
                    strOut = LoginUtils.LoginForm(rtnInfo);
                }
                else
                {
                    switch (paramCmd)
                    {
                        case "systemapi_admin_getsystemlist":
                            strOut = SystemAdminList(postInfo, controlRelPath);
                            break;
                        case "systemapi_admin_getdetail":
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_adminaddnew":
                            strOut = SystemAddNew(postInfo, controlRelPath);
                            break;
                        case "systemapi_addinterface":
                            SystemAddListRow(postInfo, "interfacedata");
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_admin_save":
                            SystemSave(postInfo);
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_admin_delete":
                            SystemDelete(postInfo);
                            strOut = SystemAdminList(postInfo, controlRelPath);
                            break;
                        case "systemapi_addparam":
                            SystemAddListRow(postInfo, "idxfielddata");
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_addsetting":
                            SystemAddListRow(postInfo, "settingsdata");
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_addgroup":
                            SystemAddListRow(postInfo, "groupsdata");
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_addprovtype":
                            SystemAddListRow(postInfo, "provtypesdata");
                            strOut = SystemAdminDetail(postInfo, controlRelPath);
                            break;
                        case "systemapi_rebuildindex":
                            RebuildIndex(postInfo, false);
                            strOut = "<h1>Rebuilding Index</h1>";
                            break;
                        case "systemapi_deleterebuildindex":
                            RebuildIndex(postInfo, true);
                            strOut = "<h1>Deleting and Rebuilding Index</h1>";
                            break;
                        case "systemapi_copyinterface":
                            CopyInterface(postInfo);
                            strOut = "";
                            break;
                    }
                }
            }
            else
            {
                var ps = new PortalSecurity();
                ps.SignOut();

                switch (paramCmd)
                {
                    case "systemapi_login":
                        strOut = LoginUtils.DoLogin(postInfo, userHostAddress);
                        break;
                    case "systemapi_sendreset":
                        //strOut = ResetPass(sInfo);
                        break;
                    default:
                        strOut = LoginUtils.LoginForm(rtnInfo);
                        break;
                }
            }
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;            
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


                    var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

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

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

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
                info.GUIDKey = GeneralUtils.GetUniqueKey(12);
                var objCtrl = new DNNrocketController();
                info.ItemID = objCtrl.Update(info);

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void SystemAddListRow(SimplisityInfo sInfo, string listname)
        {
            try
            {
                var selecteditemid = sInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                if (GeneralUtils.IsNumeric(selecteditemid))
                {
                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetInfo(Convert.ToInt32(selecteditemid));
                    info.AddListRow(listname);
                    objCtrl.Update(info);
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }


        public static void SystemSave(SimplisityInfo postInfo)
        {
            var selecteditemid = postInfo.GetXmlProperty("genxml/hidden/selecteditemid");
            if (GeneralUtils.IsNumeric(selecteditemid))
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(Convert.ToInt32(selecteditemid));
                info.XMLData = postInfo.XMLData;
                info.GUIDKey = postInfo.GetXmlProperty("genxml/textbox/ctrlkey");
                objCtrl.Update(info);

                // Capture existing SYSTEMLINK records, so we can selectivly delete. Protect system suring operation, records always there.
                var systemlinklist = objCtrl.GetList(info.PortalId, -1, "SYSTEMLINK");
                var systemlinkidxlist = objCtrl.GetList(info.PortalId, -1, "SYSTEMLINKIDX");
                var newsystemlinklist = new List<int>();
                var newsystemlinkidxlist = new List<int>();

                // make systemlink, so we can get systeminfo from entitytypecode.
                var entityList = new List<string>();
                foreach (var i in info.GetList("interfacedata"))
                {
                    entityList.Add(i.GetXmlProperty("genxml/textbox/entitytypecode"));
                }

                // Get idxfields.
                var idxListStr = new List<string>();
                var idxList = new List<SimplisityInfo>();
                foreach (var i in info.GetList("idxfielddata"))
                {
                    idxList.Add(i);
                    idxListStr.Add(i.GetXmlProperty("genxml/dropdownlist/entitytypecode") + "," + i.GetXmlProperty("genxml/textbox/indexref"));
                }


                foreach (var entityname in entityList)
                {
                    var idxinfo = objCtrl.GetByGuidKey(info.PortalId, -1, "SYSTEMLINK", entityname);
                    if (idxinfo == null)
                    {
                        idxinfo = new SimplisityInfo();
                        idxinfo.PortalId = info.PortalId;
                        idxinfo.TypeCode = "SYSTEMLINK";
                        idxinfo.GUIDKey = entityname;
                        idxinfo.ParentItemId = info.ItemID;
                        var itemid = objCtrl.Update(idxinfo);
                        idxinfo.ItemID = itemid;
                    }
                    else
                    {
                        idxinfo.ParentItemId = info.ItemID;
                        objCtrl.Update(idxinfo);
                    }
                    newsystemlinklist.Add(idxinfo.ItemID);

                    //SYSTEMIDX  - use indexref to create a table join in SQL PROC.
                    foreach (var idxfield in idxList)
                    {
                        if (idxfield.GetXmlProperty("genxml/dropdownlist/entitytypecode") == entityname)
                        {
                            var idxref = idxfield.GetXmlProperty("genxml/textbox/indexref");
                            var idxinfo2 = objCtrl.GetByGuidKey(info.PortalId, -1, "SYSTEMLINKIDX", idxref);
                            if (idxinfo2 == null)
                            {
                                idxinfo2 = new SimplisityInfo();
                                idxinfo2.PortalId = info.PortalId;
                                idxinfo2.TypeCode = "SYSTEMLINKIDX";
                                idxinfo2.GUIDKey = idxref;
                                idxinfo2.ParentItemId = idxinfo.ItemID;
                                var itemid = objCtrl.Update(idxinfo2);
                                idxinfo2.ItemID = itemid;
                            }
                            else
                            {
                                idxinfo2.ParentItemId = idxinfo.ItemID;
                                objCtrl.Update(idxinfo2);
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

                CacheUtils.ClearCache();
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
                    var l = objCtrl.GetList(-1, -1, entityCode);
                    foreach (var sInfo in l)
                    {
                        if (deleteindex)
                        {
                            objCtrl.DeleteIndex(sInfo);
                        }
                        objCtrl.RebuildIndex(sInfo, sInfo.ItemID);
                    }
                    l = objCtrl.GetList(-1, -1, entityCode + "LANG");
                    foreach (var sInfo in l)
                    {
                        objCtrl.RebuildIndex(sInfo, sInfo.ItemID);
                    }
                }

                CacheUtils.ClearCache();
            }

        }


        public static void CopyInterface(SimplisityInfo postInfo)
        {
            var itemid = postInfo.GetXmlProperty("genxml/hidden/fromsystemid");
            if (GeneralUtils.IsNumeric(itemid))
            {
                var objCtrl = new DNNrocketController();
                var sysInfo = objCtrl.GetInfo(Convert.ToInt32(itemid));
                if (sysInfo != null)
                {
                    var interfacekey = postInfo.GetXmlProperty("genxml/hidden/interfacekey");
                    var tosystemid = postInfo.GetXmlProperty("genxml/hidden/tosystemid");
                    var sysInfoTo = objCtrl.GetInfo(Convert.ToInt32(itemid));
                    if (sysInfoTo != null)
                    {
                        // find from interface
                        foreach (var i in sysInfo.GetList("interfacedata"))
                        {

                        }

                        // check doesn;t exist

                            // copy to new ystsem
                    }
                }

                    CacheUtils.ClearCache();
            }

        }

    }
}
