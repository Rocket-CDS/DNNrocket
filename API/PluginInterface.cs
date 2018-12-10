using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NBrightCore.common;
using NBrightCore.render;
using NBrightDNN;
using DotNetNuke.Common.Utilities;
using DNNrocketAPI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using Simplisity;

namespace DNNrocketAPI
{
    public class PluginInterface: AjaxInterface
    {
        public override string Ajaxkey { get; set; }

        #region "Admin Methods"

        public static string TemplateRelPath = "/DesktopModules/DNNrocket/api";

        public override string ProcessCommand(string paramCmd, HttpContext context, string editlang = "")
        {
            var strOut = "PLUGIN - ERROR!! - No Security rights or function command.";
            var ajaxInfo = DNNrocketUtils.GetAjaxFields(context);
            var userId = ajaxInfo.GetXmlPropertyInt("genxml/hidden/userid");

            if (UserController.Instance.GetCurrentUserInfo().IsSuperUser || PluginUtils.CheckPluginSecurity(PortalSettings.Current.PortalId, "plugins2"))
            {
                switch (paramCmd)
                {
                    case "plugins_admin_getlist":
                        strOut = PluginAdminList(context);
                        break;
                    case "plugins_admin_getdetail":
                        strOut = PluginAdminDetail(context);
                        break;
                    case "plugins_adminaddnew":
                        strOut = PluginAddNew(context);
                        break;
                    case "plugins_addpluginsmodels":
                        PluginAddInterface(context);
                        strOut = PluginAdminDetail(context);
                        break;
                    case "plugins_admin_save":
                        PluginSave(context);
                        strOut = PluginAdminDetail(context);
                        break;
                    case "plugins_admin_delete":
                        PluginDelete(context);
                        strOut = PluginAdminList(context);
                        break;                        
                    case "plugins_movepluginsadmin":
                        PluginMove(context);
                        strOut = PluginAdminList(context);
                        break;
                    case "plugins_addpluginsparam":
                        PluginAddParameter(context);
                        strOut = PluginAdminDetail(context);
                        break;
                }
            }
            return strOut;
        }


        #endregion

        public override void Validate()
        {
            // ignore
        }


        public static string PluginAdminList(HttpContext context)
        {

            try
            {
                    var ajaxInfo = DNNrocketUtils.GetAjaxFields(context);
                    var list = PluginUtils.GetPluginList();
                    return RenderPluginAdminList(list, ajaxInfo, 0);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public static String RenderPluginAdminList(List<SimplisityInfo> list, SimplisityInfo ajaxInfo, int recordCount)
        {

            try
            {
                    if (list == null) return "";
                    var strOut = "";

                    // select a specific entity data type for the product (used by plugins)
                    var themeFolder = ajaxInfo.GetXmlProperty("genxml/hidden/theme");
                    if (themeFolder == "") themeFolder = "config-w3";
                    var razortemplate = ajaxInfo.GetXmlProperty("genxml/hidden/template");

                    var passSettings = new Dictionary<string, string>();
                    foreach (var s in ajaxInfo.ToDictionary())
                    {
                        passSettings.Add(s.Key, s.Value);
                    }

                    strOut = DNNrocketUtils.RazorTemplRenderList(razortemplate, 0, "", list, TemplateRelPath, themeFolder, Utils.GetCurrentCulture(), passSettings);

                    return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static String PluginAdminDetail(HttpContext context)
        {
            try
            {
                    var ajaxInfo = DNNrocketUtils.GetAjaxFields(context);

                    var strOut = "";
                    var selecteditemid = ajaxInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                    if (Utils.IsNumeric(selecteditemid))
                    {
                        var themeFolder = ajaxInfo.GetXmlProperty("genxml/hidden/themefolder");
                        if (themeFolder == "") themeFolder = "config";
                        var razortemplate = ajaxInfo.GetXmlProperty("genxml/hidden/razortemplate");

                        var passSettings = DNNrocketUtils.GetPassSettings(ajaxInfo);

                        var objCtrl = new NBrightBuyController();
                        var info = objCtrl.GetData(Convert.ToInt32(selecteditemid));
                        var pluginRecord = new PluginRecord(info);

                        strOut = DNNrocketUtils.RazorTemplRender(razortemplate, 0, "", pluginRecord, TemplateRelPath, themeFolder, Utils.GetCurrentCulture(), passSettings);
                    }
                    return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String PluginAddNew(HttpContext context)
        {
            try
            {
                    var ajaxInfo = DNNrocketUtils.GetAjaxFields(context);

                    var strOut = "";
                    var themeFolder = ajaxInfo.GetXmlProperty("genxml/hidden/themefolder");
                    if (themeFolder == "") themeFolder = "config";
                    var razortemplate = ajaxInfo.GetXmlProperty("genxml/hidden/razortemplate");

                    var passSettings = DNNrocketUtils.GetPassSettings(ajaxInfo);

                    var info = new SimplisityInfo();
                    info.ItemID = -1;
                    info.Lang = Utils.GetCurrentCulture();
                    info.SetXmlProperty("genxml/hidden/index", "99");
                    info.TypeCode = "PLUGIN";
                    info.GUIDKey = Utils.GetUniqueKey(12);
                    var objCtrl = new NBrightBuyController();
                    info.ItemID = objCtrl.Update(info);
                    var pluginRecord = new PluginRecord(info);

                    strOut = NBrightBuyUtils.RazorTemplRender(razortemplate, 0, "", pluginRecord, TemplateRelPath, themeFolder, Utils.GetCurrentCulture(), passSettings);
                    return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void PluginAddInterface(HttpContext context)
        {
            try
            {
                    var ajaxInfo = DNNrocketUtils.GetAjaxFields(context);
                    var selecteditemid = ajaxInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                    if (Utils.IsNumeric(selecteditemid))
                    {
                        var objCtrl = new NBrightBuyController();
                        var info = objCtrl.GetData(Convert.ToInt32(selecteditemid));
                        var pluginRecord = new PluginRecord(info);
                        pluginRecord.AddInterface();
                    }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }

        public static void PluginAddParameter(HttpContext context)
        {
            try
            {
                var ajaxInfo = DNNrocketUtils.GetAjaxFields(context);
                var selecteditemid = ajaxInfo.GetXmlProperty("genxml/hidden/selecteditemid");
                if (Utils.IsNumeric(selecteditemid))
                {
                    var objCtrl = new NBrightBuyController();
                    var info = objCtrl.GetData(Convert.ToInt32(selecteditemid));
                    var pluginRecord = new PluginRecord(info);
                    pluginRecord.AddParameter();
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
        }

        public static void PluginSave(HttpContext context)
        {
                var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
                var itemid = ajaxInfo.GetXmlProperty("genxml/hidden/itemid");
                if (Utils.IsNumeric(itemid))
                {
                    var objCtrl = new NBrightBuyController();
                    var info = objCtrl.GetData(Convert.ToInt32(itemid));
                    var pluginRecord = new PluginRecord(info);
                    var modelXml = Utils.UnCode(ajaxInfo.GetXmlProperty("genxml/hidden/xmlupdatemodeldata"));
                    var parametersXml = Utils.UnCode(ajaxInfo.GetXmlProperty("genxml/hidden/xmlupdateparamdata"));
                
                    ajaxInfo.RemoveXmlNode("genxml/hidden/xmlupdatemodeldata");
                    pluginRecord.Info().XMLData = ajaxInfo.XMLData;

                    // check for unique ctrl ref
                    var ctrlref = pluginRecord.Info().GetXmlProperty("genxml/textbox/ctrl");
                    var ctrltest = objCtrl.GetByGuidKey(0, -1, "PLUGIN", ctrlref);
                    if (ctrltest != null)
                    {
                        if (ctrltest.ItemID != pluginRecord.Info().ItemID)
                        {
                            pluginRecord.Info().SetXmlProperty("genxml/textbox/ctrl", pluginRecord.Info().GetXmlProperty("genxml/textbox/ctrl") + Utils.GetUniqueKey());
                        }
                    }

                    // make sure index is in correct format, (FLOAT) for SQL
                    pluginRecord.Info().SetXmlProperty("genxml/hidden/index", (pluginRecord.Info().GetXmlPropertyInt("genxml/hidden/index").ToString()), TypeCode.Double);
                    pluginRecord.Info().RemoveXmlNode("genxml/hidden/itemid");
                    pluginRecord.Info().RemoveXmlNode("genxml/hidden/editlanguage");
                    pluginRecord.Info().RemoveXmlNode("genxml/hidden/uilang1");
                    pluginRecord.Info().GUIDKey = pluginRecord.Info().GetXmlProperty("genxml/textbox/ctrl");

                    pluginRecord.UpdateModels(modelXml, Utils.GetCurrentCulture(), "interfaces");
                    pluginRecord.UpdateModels(parametersXml, Utils.GetCurrentCulture(), "parameters");

                objCtrl.Update(pluginRecord.Info());

                    // remove save GetData cache
                    DataCache.ClearCache();

                    //load entity typecode to DB idx settings.
                    NBrightBuyUtils.RegisterEnityTypeToDataBase();

                }
        }

        public static void PluginDelete(HttpContext context)
        {
                var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
                var itemid = ajaxInfo.GetXmlProperty("genxml/hidden/itemid");
                if (Utils.IsNumeric(itemid))
                {
                    var objCtrl = new NBrightBuyController();
                    objCtrl.Delete(Convert.ToInt32(itemid));

                    PluginUtils.CopySystemPluginsToPortal();

                    // remove save GetData cache
                    DataCache.ClearCache();

                }
        }

        public static void PluginMove(HttpContext context)
        {
            var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
            var movepluginsid = ajaxInfo.GetXmlPropertyInt("genxml/hidden/movepluginsid");
            var movetopluginsid = ajaxInfo.GetXmlPropertyInt("genxml/hidden/movetopluginsid");
            if (movepluginsid > 0 && movetopluginsid > 0)
            {
                var objCtrl = new NBrightBuyController();
                var infoTo = objCtrl.GetData(movetopluginsid);
                var info = objCtrl.GetData(movepluginsid);

                if (infoTo.GetXmlPropertyDouble("genxml/hidden/index") < info.GetXmlPropertyDouble("genxml/hidden/index"))
                {
                    info.SetXmlProperty("genxml/hidden/index", (infoTo.GetXmlPropertyDouble("genxml/hidden/index") - 0.5).ToString("00.0"), TypeCode.Double);
                }
                else
                {
                    info.SetXmlProperty("genxml/hidden/index", (infoTo.GetXmlPropertyDouble("genxml/hidden/index") + 0.5).ToString("00.0"), TypeCode.Double);
                }
                objCtrl.Update(info);

                // remove save GetData cache so we read changed data
                DataCache.ClearCache();

                PluginUtils.ResequenceRecords();

                // remove save GetData cache
                DataCache.ClearCache();

            }
        }


    }

}
