using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace DNNrocket.TestList
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _EntityTypeCode;
        private static string _editlang;

        public override string ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            _EntityTypeCode = DNNrocketUtils.GetEntityTypeCode(interfaceInfo);
            _editlang = DNNrocketUtils.GetEditCulture();

            var controlRelPath = "/DesktopModules/DNNrocket/TestList";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [" + interfaceInfo.GetXmlProperty("genxml/textbox/interfacekey") + "]";

            switch (paramCmd)
            {
                case "testlist_getlist":
                    strOut = GetList(postInfo, controlRelPath);
                    break;
                case "testlist_getdetail":
                    strOut = GetDetail(postInfo, controlRelPath);
                    break;
                case "testlist_add":
                    var newInfo = AddNew(postInfo);
                    postInfo.SetXmlProperty("genxml/hidden/selecteditemid", newInfo.ItemID.ToString());
                    strOut = GetDetail(postInfo, controlRelPath);
                    break;
                case "testlist_save":
                    Save(postInfo);
                    strOut = GetDetail(postInfo, controlRelPath);
                    break;
                case "testlist_delete":
                    Delete(postInfo);
                    strOut = GetList(postInfo, controlRelPath);
                    break;
                case "testlist_sort":
                    strOut = GetList(postInfo, controlRelPath);
                    break;
                default:
                    strOut = "COMMAND NOT FOUND!!! - [" + paramCmd + "] [" + interfaceInfo.GetXmlProperty("genxml/textbox/interfacekey") + "]";
                    break;
            }
            return strOut;
        }

        public static String GetList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var objCtrl = new DNNrocketController();
                var list = objCtrl.GetList(postInfo.PortalId, postInfo.ModuleId, _EntityTypeCode,"", _editlang, "");
                return RenderList(list, postInfo, 0, templateControlRelPath);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String RenderList(List<SimplisityInfo> list, SimplisityInfo sInfo, int recordCount, string templateControlRelPath)
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

        public static String GetDetail(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

                var passSettings = postInfo.ToDictionary();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static SimplisityInfo AddNew(SimplisityInfo postInfo)
        {
            var info = new SimplisityInfo();
            info.ItemID = -1;
            info.PortalId = DNNrocketUtils.GetPortalId();
            info.Lang = DNNrocketUtils.GetEditCulture();
            info.TypeCode = "TESTLIST";
            info.GUIDKey = GeneralUtils.GetUniqueKey(12);
            
            var objCtrl = new DNNrocketController();
            return objCtrl.SaveData(info);
        }

        public static void Save(SimplisityInfo postInfo)
        {
            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            if (selecteditemid > 0)
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());
                info.XMLData = postInfo.XMLData;
                objCtrl.SaveData(info);
                CacheUtils.ClearAllCache();
            }
        }

        public static void Delete(SimplisityInfo postInfo)
        {
            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            if (selecteditemid > 0)
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(selecteditemid);
                CacheUtils.ClearAllCache();
            }
        }

    }
}
