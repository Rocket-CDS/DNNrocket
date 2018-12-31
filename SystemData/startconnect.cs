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

        public override string ProcessCommand(string paramCmd, SimplisityInfo postInfo,string userHostAddress, string editlang = "")
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
                            SystemAddListRow(postInfo, "parameterdata");
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
            return strOut;

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
            var selecteditemid = postInfo.GetXmlProperty("genxml/hidden/itemid");
            if (GeneralUtils.IsNumeric(selecteditemid))
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(Convert.ToInt32(selecteditemid));
                info.XMLData = postInfo.XMLData;
                objCtrl.Update(info);
            }
        }

        public static void SystemDelete(SimplisityInfo sInfo)
        {
            var itemid = sInfo.GetXmlProperty("genxml/hidden/itemid");
            if (GeneralUtils.IsNumeric(itemid))
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(Convert.ToInt32(itemid));

                CacheUtils.ClearCache();
            }
        }




        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        public static string AddToList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            var strOut = "";
            var listname = postInfo.GetXmlProperty("genxml/hidden/listname");
            var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
            var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");

            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());

            info.AddListRow(listname);

            objCtrl.SaveData("testform", "TEST", info);  //TestFormSave so if we add multiple it works correct.

            var passSettings = postInfo.ToDictionary();

            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            return strOut;
        }

        public static String TestFormDetail(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = postInfo.ToDictionary();
                               
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void TestFormSave(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            objCtrl.SaveData("testform", "TEST", postInfo);
        }


    }
}
