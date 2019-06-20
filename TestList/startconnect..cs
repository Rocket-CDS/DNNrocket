using System;
using System.Collections.Generic;
using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;

namespace DNNrocket.TestList
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _EntityTypeCode;
        private static string _editlang;
        private static SimplisityInfo _systemInfo;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string userHostAddress, string editlang = "")
        {
            var rocketInterface = new DNNrocketInterface(interfaceInfo);
            var commandSecurity = new CommandSecurity(-1,-1,rocketInterface);
            commandSecurity.AddCommand("testlist_add", true);
            commandSecurity.AddCommand("testlist_save", true);
            commandSecurity.AddCommand("testlist_delete", true);
            commandSecurity.AddCommand("testlist_createrows", true);
            commandSecurity.AddCommand("testlist_deleterows", true);
            commandSecurity.AddCommand("testlist_getlist", false);
            commandSecurity.AddCommand("testlist_getdetail", false);
            commandSecurity.AddCommand("testlist_sort", false);
            commandSecurity.AddCommand("testlist_search", false);

            _systemInfo = systemInfo;
            _EntityTypeCode = DNNrocketUtils.GetEntityTypeCode(interfaceInfo);
            _editlang = editlang;
            if (_editlang == "") _editlang = DNNrocketUtils.GetEditCulture();

            var strOut = "";
            if (commandSecurity.HasSecurityAccess(paramCmd))
            {
                switch (paramCmd)
                {
                    case "testlist_add":
                        var newInfo = AddNew();
                        postInfo.SetXmlProperty("genxml/hidden/selecteditemid", newInfo.ItemID.ToString());
                        strOut = GetDetail(paramInfo, ControlRelPath);
                        break;
                    case "testlist_save":
                        Save(postInfo, paramInfo);
                        strOut = GetDetail(paramInfo, ControlRelPath);
                        break;
                    case "testlist_delete":
                        Delete(paramInfo);
                        strOut = GetList(postInfo, paramInfo, ControlRelPath);
                        break;
                    case "testlist_createrows":
                        CreateRows(paramInfo);
                        strOut = GetList(postInfo, paramInfo, ControlRelPath);
                        break;
                    case "testlist_deleterows":
                        DeleteRows();
                        strOut = GetList(postInfo, paramInfo, ControlRelPath);
                        break;
                    case "testlist_getlist":
                        strOut = GetList(postInfo, paramInfo, ControlRelPath);
                        break;
                    case "testlist_getdetail":
                        strOut = GetDetail(paramInfo, ControlRelPath);
                        break;
                    case "testlist_sort":
                        strOut = GetList(postInfo, paramInfo, ControlRelPath);
                        break;
                    case "testlist_search":
                        strOut = GetList(postInfo, paramInfo, ControlRelPath);
                        break;
                }
            }
            else
            {
                if (commandSecurity.ValidCommand(paramCmd))
                {
                   // strOut = LoginUtils.LoginForm(postInfo, rocketInterface.InterfaceKey);
                }
            }

var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static String GetList(SimplisityInfo postInfo, SimplisityInfo paramInfo, string templateControlRelPath)
        {
            try
            {

                var page = paramInfo.GetXmlPropertyInt("genxml/hidden/page");
                var pagesize = paramInfo.GetXmlPropertyInt("genxml/hidden/pagesize");

                var searchtext = postInfo.GetXmlProperty("genxml/textbox/searchtext");

                var filter = "";
                if (searchtext != "")
                {
                   // filter = " and inputlang1.GuidKey like '%" + searchtext + "%'";
                }


                var objCtrl = new DNNrocketController();
                var listcount = objCtrl.GetListCount(postInfo.PortalId, postInfo.ModuleId, _EntityTypeCode, filter, _editlang);
                var list = objCtrl.GetList(postInfo.PortalId, postInfo.ModuleId, _EntityTypeCode, filter, _editlang, "",0, page, pagesize, listcount);

                var headerData = new SimplisityInfo();
                headerData.SetXmlProperty("genxml/hidden/rowcount", listcount.ToString());
                headerData.SetXmlProperty("genxml/hidden/page", page.ToString());
                headerData.SetXmlProperty("genxml/hidden/pagesize", pagesize.ToString());
                headerData.SetXmlProperty("genxml/textbox/searchtext", searchtext);

                return RenderList(list, paramInfo, 0, templateControlRelPath, headerData);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static String RenderList(List<SimplisityInfo> list, SimplisityInfo sInfo, int recordCount, string templateControlRelPath, SimplisityInfo headerData)
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

                strOut = DNNrocketUtils.RazorList(razorTempl, list, passSettings,headerData);

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


        public static SimplisityInfo AddNew()
        {
            var info = new SimplisityInfo();
            info.ItemID = -1;
            info.PortalId = DNNrocketUtils.GetPortalId();
            info.Lang = DNNrocketUtils.GetEditCulture();
            info.TypeCode = "TESTLIST";
            info.GUIDKey = GeneralUtils.GetUniqueKey(12);
            
            var objCtrl = new DNNrocketController();
            return objCtrl.SaveData(info, _systemInfo.ItemID);
        }

        public static void Save(SimplisityInfo postInfo, SimplisityInfo paramInfo)
        {
            var selecteditemid = paramInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            if (selecteditemid > 0)
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());
                info.XMLData = postInfo.XMLData;
                objCtrl.SaveData(info, _systemInfo.ItemID);
                CacheUtils.ClearAllCache();
            }
        }

        public static void DeleteRows()
        {
            var objCtrl = new DNNrocketController();
            var l = objCtrl.GetList(DNNrocketUtils.GetPortalId(), -1, _EntityTypeCode, "and R1.guidkey = 'testrecord'");
            foreach (var i in l)
            {
                objCtrl.Delete(i.ItemID);
            }
        }

        public static void CreateRows(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();

            for (int i = 0; i < 100; i++)
            {
                var newInfo = AddNew();
                newInfo.XMLData = postInfo.XMLData;
                newInfo.SetXmlProperty("genxml/row", i.ToString());

                var cultureList = DNNrocketUtils.GetCultureCodeList();
                foreach( var c in cultureList)
                {
                    newInfo.SetXmlProperty("genxml/textbox/txtinput", GeneralUtils.GetUniqueKey() + "-" + i.ToString());
                    newInfo.SetXmlProperty("genxml/lang/genxml/textbox/txtinputl", GeneralUtils.GetUniqueKey() + "-" + i.ToString());
                    newInfo.Lang = c;
                    objCtrl.SaveData(newInfo, _systemInfo.ItemID);
                }

                var rec = objCtrl.GetRecord(newInfo.ItemID);
                rec.GUIDKey = "testrecord";
                objCtrl.SaveRecord(rec);
            }
             CacheUtils.ClearAllCache();
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
