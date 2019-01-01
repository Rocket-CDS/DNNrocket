using System;
using System.Collections.Generic;
using DNNrocketAPI;
using Simplisity;

namespace DNNrocket.TestList
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        public override string ProcessCommand(string paramCmd, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            //CacheUtils.ClearAllCache();

            var controlRelPath = "/DesktopModules/DNNrocket/TestList";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [testform]";

            switch (paramCmd)
            {
                case "testlist_get":
                    strOut = Get(postInfo, controlRelPath);
                    break;
                case "testlist_add":
                    var newInfo = AddNew(postInfo);
                    postInfo.SetXmlProperty("genxml/hidden/selecteditemid", newInfo.ItemID.ToString());
                    strOut = Get(postInfo, controlRelPath);
                    break;
                case "testlist_save":
                    Save(postInfo);
                    strOut = Get(postInfo, controlRelPath);
                    break;
                default:
                    strOut = "default path";
                    break;
            }
            return strOut;
        }

        public static String Get(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                if (selecteditemid > 0)
                {
                    return GetDetail(postInfo,templateControlRelPath);
                }
                else
                {
                    return GetList(postInfo, templateControlRelPath);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String GetList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {
                var objCtrl = new DNNrocketController();
                var list = objCtrl.GetList(postInfo.PortalId, postInfo.ModuleId, "TESTLIST");
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
            var selecteditemid = postInfo.GetXmlProperty("genxml/hidden/selecteditemid");
            if (GeneralUtils.IsNumeric(selecteditemid))
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(Convert.ToInt32(selecteditemid),DNNrocketUtils.GetEditCulture());
                info.XMLData = postInfo.XMLData;
                objCtrl.Update(info);
                CacheUtils.ClearAllCache();
            }
        }
    }
}
