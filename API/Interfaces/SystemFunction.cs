using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using NBrightCore.TemplateEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DNNrocketAPI.Interfaces
{
    public static class SystemFunction
    {

        public static string TemplateRelPath = "/DesktopModules/DNNrocket/api";


        public static string ProcessCommand(string paramCmd, SimplisityInfo sInfo, string editlang = "")
        {
            var strOut = "ERROR!! - No Security rights or function command.";

            if (UserController.Instance.GetCurrentUserInfo().IsSuperUser)
            {
                switch (paramCmd)
                {
                    case "systemapi_admin_getsystemlist":
                        strOut = SystemAdminList(sInfo);
                        break;
                    case "systemapi_admin_getdetail":
                        strOut = SystemAdminDetail(sInfo);
                        break;
                    case "systemapi_adminaddnew":
                        strOut = SystemAddNew(sInfo);
                        break;
                    case "systemapi_addinterface":
                        SystemAddInterface(sInfo);
                        strOut = SystemAdminDetail(sInfo);
                        break;
                    case "systemapi_admin_save":
                        SystemSave(sInfo);
                        strOut = SystemAdminDetail(sInfo);
                        break;
                    case "systemapi_admin_delete":
                        SystemDelete(sInfo);
                        strOut = SystemAdminList(sInfo);
                        break;
                    case "systemapi_addparam":
                        SystemAddParameter(sInfo);
                        strOut = SystemAdminDetail(sInfo);
                        break;


                }
            }
            return strOut;
        }



        public static string SystemAdminList(SimplisityInfo sInfo)
        {
            try
            {
                var systemData = new SystemData();
                var list = systemData.GetSystemList();
                return RenderSystemAdminList(list, sInfo, 0);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String SystemAdminDetail(SimplisityInfo sInfo)
        {
            try
            {
                var strOut = "";
                var selecteditemid = sInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = sInfo.ToDictionary();

                var systemRecord = new SystemRecord(selecteditemid);

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, TemplateRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                strOut = DNNrocketUtils.RazorDetail(razorTempl, systemRecord, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String RenderSystemAdminList(List<SimplisityInfo> list, SimplisityInfo sInfo, int recordCount)
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

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, TemplateRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                strOut = DNNrocketUtils.RazorList(razorTempl, list, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public static String SystemAddNew(SimplisityInfo sInfo)
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
                var systemRecord = new SystemRecord(info);

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, TemplateRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                strOut = DNNrocketUtils.RazorDetail(razorTempl, systemRecord, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void SystemAddInterface(SimplisityInfo sInfo)
        {
            try
            {
                var selecteditemid = sInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                if (GeneralUtils.IsNumeric(selecteditemid))
                {
                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetRecord(Convert.ToInt32(selecteditemid));
                    var pluginRecord = new SystemRecord(info);
                    pluginRecord.AddInterface();
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }

        public static void SystemAddParameter(SimplisityInfo sInfo)
        {
            try
            {
                var selecteditemid = sInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                if (GeneralUtils.IsNumeric(selecteditemid))
                {
                    var objCtrl = new DNNrocketController();
                    var info = objCtrl.GetRecord(Convert.ToInt32(selecteditemid));
                    var pluginRecord = new SystemRecord(info);
                    pluginRecord.AddParameter();
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }


        public static void SystemSave(SimplisityInfo sInfo)
        {
            var itemid = sInfo.GetXmlProperty("genxml/hidden/itemid");
            if (GeneralUtils.IsNumeric(itemid))
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetRecord(Convert.ToInt32(itemid));
                var systemRecord = new SystemRecord(info);
                var modelXml = GeneralUtils.UnCode(sInfo.GetXmlProperty("genxml/hidden/xmlupdatemodeldata"));
                var parametersXml = GeneralUtils.UnCode(sInfo.GetXmlProperty("genxml/hidden/xmlupdateparamdata"));

                sInfo.RemoveXmlNode("genxml/hidden/xmlupdatemodeldata");
                sInfo.RemoveXmlNode("genxml/hidden/xmlupdateparamdata");
                systemRecord.Info().XMLData = sInfo.XMLData;

                // check for unique ctrl ref
                var ctrlref = systemRecord.Info().GetXmlProperty("genxml/textbox/ctrlkey");
                var ctrltest = objCtrl.GetByGuidKey(0, -1, "SYSTEM", ctrlref);
                if (ctrltest != null)
                {
                    if (ctrltest.ItemID != systemRecord.Info().ItemID)
                    {
                        systemRecord.Info().SetXmlProperty("genxml/textbox/ctrl", systemRecord.Info().GetXmlProperty("genxml/textbox/ctrlkey") + GeneralUtils.GetUniqueKey());
                    }
                }

                // make sure index is in correct format, (FLOAT) for SQL
                systemRecord.Info().SetXmlProperty("genxml/hidden/index", (systemRecord.Info().GetXmlPropertyInt("genxml/hidden/index").ToString()), TypeCode.Double);
                systemRecord.Info().RemoveXmlNode("genxml/hidden/itemid");
                systemRecord.Info().RemoveXmlNode("genxml/hidden/editlanguage");
                systemRecord.Info().RemoveXmlNode("genxml/hidden/uilang1");
                systemRecord.Info().GUIDKey = systemRecord.Info().GetXmlProperty("genxml/textbox/ctrlkey");

                systemRecord.UpdateModels(modelXml, DNNrocketUtils.GetCurrentCulture(), "interfaces");
                systemRecord.UpdateModels(parametersXml, DNNrocketUtils.GetCurrentCulture(), "parameters");

                objCtrl.Update(systemRecord.Info());

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

    }
}
