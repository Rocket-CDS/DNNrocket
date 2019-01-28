using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.Category
{
    public class startconnect : DNNrocketAPI.APInterface
    {
        private static string _EntityTypeCode;
        private static string _editlang;

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {
            _EntityTypeCode = DNNrocketUtils.GetEntityTypeCode(interfaceInfo);
            _editlang = editlang;
            if (_editlang == "") _editlang = DNNrocketUtils.GetEditCulture();

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [" + interfaceInfo.GetXmlProperty("genxml/textbox/interfacekey") + "]";

            switch (paramCmd)
            {
                case "category_getlist":
                    strOut = GetList(postInfo, ControlRelPath);
                    break;
                case "category_getdetail":
                    strOut = GetDetail(postInfo, ControlRelPath);
                    break;
                case "category_add":
                    var newInfo = AddNew();
                    postInfo.SetXmlProperty("genxml/hidden/selecteditemid", newInfo.ItemID.ToString());
                    strOut = GetDetail(postInfo, ControlRelPath);
                    break;
                case "category_save":
                    Save(postInfo);
                    strOut = GetDetail(postInfo, ControlRelPath);
                    break;
                case "category_delete":
                    Delete(postInfo);
                    strOut = GetList(postInfo, ControlRelPath);
                    break;
                case "category_sort":
                    strOut = GetList(postInfo, ControlRelPath);
                    break;
                case "category_search":
                    strOut = GetList(postInfo, ControlRelPath);
                    break;
                case "category_addimage":
                    strOut = AddImageToList(postInfo, ControlRelPath);
                    break;
                default:
                    strOut = "COMMAND NOT FOUND!!! - [" + paramCmd + "] [" + interfaceInfo.GetXmlProperty("genxml/textbox/interfacekey") + "]";
                    break;
            }
            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static String GetList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            try
            {

                var page = postInfo.GetXmlPropertyInt("genxml/hidden/page");
                var pagesize = postInfo.GetXmlPropertyInt("genxml/hidden/pagesize");

                var searchtext = postInfo.GetXmlProperty("genxml/textbox/searchtext");

                var filter = "";
                if (searchtext != "")
                {
                    filter = " and (categoryname.GuidKey like '%" + searchtext + "%' or categoryref.GuidKey like '%" + searchtext + "%')";
                }


                var objCtrl = new DNNrocketController();
                var listcount = objCtrl.GetListCount(postInfo.PortalId, postInfo.ModuleId, _EntityTypeCode, filter, _editlang);
                var list = objCtrl.GetList(postInfo.PortalId, postInfo.ModuleId, _EntityTypeCode, filter, _editlang, "", 0, page, pagesize, listcount);

                var headerData = new SimplisityInfo();
                headerData.SetXmlProperty("genxml/hidden/rowcount", listcount.ToString());
                headerData.SetXmlProperty("genxml/hidden/page", page.ToString());
                headerData.SetXmlProperty("genxml/hidden/pagesize", pagesize.ToString());
                headerData.SetXmlProperty("genxml/textbox/searchtext", searchtext);

                return RenderList(list, postInfo, 0, templateControlRelPath, headerData);
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

                strOut = DNNrocketUtils.RazorList(razorTempl, list, passSettings, headerData);

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
            info.TypeCode = _EntityTypeCode;
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

        public static string AddImageToList(SimplisityInfo postInfo, string templateControlRelPath)
        {


                var imageDirectory = DNNrocketUtils.HomeDirectory() + "\\images";
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                var systemprovider = postInfo.GetXmlProperty("genxml/systemprovider");
                var systemData = new SystemData();
                var sInfoSystem = systemData.GetSystemByKey(systemprovider);
                var encryptkey = sInfoSystem.GetXmlProperty("genxml/textbox/encryptkey");

                var strOut = "";
                var listname = postInfo.GetXmlProperty("genxml/hidden/listname");
                var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
                var fileuploadlist = postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");

            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            if (selecteditemid > 0)
            {

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());

                if (info != null && fileuploadlist != "")
                {

                    foreach (var f in fileuploadlist.Split(';'))
                    {
                        if (f != "")
                        {
                            var friendlyname = GeneralUtils.DeCode(f);
                            var encryptName = DNNrocketUtils.EncryptFileName(encryptkey, friendlyname);
                            var newfilename = GeneralUtils.GetUniqueKey(12);

                            var imgInfo = new SimplisityInfo();
                            var imagerelpath = DNNrocketUtils.HomeRelDirectory() + "/images/" + newfilename;
                            var imagepath = imageDirectory + "\\" + newfilename;

                            File.Move(DNNrocketUtils.TempDirectory() + "\\" + encryptName, imagepath);

                            imgInfo.SetXmlProperty("genxml/hidden", "");
                            imgInfo.SetXmlProperty("genxml/hidden/imagerelpath", imagerelpath);
                            imgInfo.SetXmlProperty("genxml/hidden/imagepath", imagepath);
                            imgInfo.SetXmlProperty("genxml/hidden/filename", newfilename);
                            imgInfo.SetXmlProperty("genxml/hidden/friendlyfilename", friendlyname);
                            imgInfo.SetXmlProperty("genxml/hidden/ext", Path.GetExtension(friendlyname));

                            info.AddListRow(listname, imgInfo);
                        }
                    }

                    objCtrl.SaveData(info);

                }

                var passSettings = postInfo.ToDictionary();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            }


            return strOut;
        }


    }
}
