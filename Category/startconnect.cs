using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DNNrocket.Category
{
    public class StartConnect : DNNrocketAPI.APInterface
    {
        private string _EntityTypeCode;
        private string _editlang;
        private string _systemkey;
        private SimplisityInfo _systemInfo;
        private SystemData _systemData;


        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang = "")
        {
            _systemData = new SystemData(systemInfo);
            var rocketInterface = new DNNrocketInterface(interfaceInfo);
            var commandSecurity = new CommandSecurity(-1,-1,rocketInterface);
            commandSecurity.AddCommand("category_add", true);
            commandSecurity.AddCommand("category_save", true);
            commandSecurity.AddCommand("category_savelist", true);
            commandSecurity.AddCommand("category_delete", true);
            commandSecurity.AddCommand("category_sort", true);
            commandSecurity.AddCommand("category_addimage", true);
            commandSecurity.AddCommand("category_visible", true);
            commandSecurity.AddCommand("category_disable", true);
            commandSecurity.AddCommand("category_getlist", false);
            commandSecurity.AddCommand("category_getdetail", false);
            commandSecurity.AddCommand("category_search", false);

            _systemInfo = systemInfo;
            _systemkey = systemInfo.GUIDKey;
            _EntityTypeCode = DNNrocketUtils.GetEntityTypeCode(interfaceInfo);
            _editlang = editlang;
            if (_editlang == "") _editlang = DNNrocketUtils.GetEditCulture();

            var strOut = "";

            if (commandSecurity.HasSecurityAccess(paramCmd))
            {
                switch (paramCmd)
                {
                    case "category_add":
                        var newInfo = AddNew(systemInfo);
                        postInfo.SetXmlProperty("genxml/hidden/selecteditemid", newInfo.ItemID.ToString());
                        strOut = GetDetail(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_save":
                        Save(postInfo, systemInfo);
                        strOut = GetDetail(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_savelist":
                        SaveList(postInfo, systemInfo);
                        strOut = GetList(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_delete":
                        Delete(postInfo);
                        strOut = GetList(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_sort":
                        strOut = GetList(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_addimage":
                        strOut = AddImageToList(postInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "category_visible":
                        strOut = ToggleHidden(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_disable":
                        strOut = ToggleDisable(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_getlist":
                        strOut = GetList(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_getdetail":
                        strOut = GetDetail(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                    case "category_search":
                        strOut = GetList(postInfo, rocketInterface.TemplateRelPath, systemInfo);
                        break;
                }
            }
            else
            {
                if (commandSecurity.ValidCommand(paramCmd))
                {
                 //   strOut = UserUtils.LoginForm(postInfo, rocketInterface.InterfaceKey);
                }
            }

            var rtnDic = new Dictionary<string, string>();
            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public String GetList(SimplisityInfo postInfo, string templateControlRelPath, SimplisityInfo systemInfo)
        {
            try
            {

                var page = postInfo.GetXmlPropertyInt("genxml/hidden/page");
                var pagesize = postInfo.GetXmlPropertyInt("genxml/hidden/pagesize");

                var searchtext = postInfo.GetXmlProperty("genxml/textbox/searchtext");
                var headerData = new SimplisityInfo();
                headerData.SetXmlProperty("genxml/textbox/searchtext", searchtext);

                var categoryList = CategoryUtils.GetCategoryList(postInfo.PortalId, -1, _editlang, searchtext,"",true,true, systemInfo.ItemID);

                return RenderList(categoryList, postInfo, 0, templateControlRelPath, headerData);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public String RenderList(List<Category> list, SimplisityInfo sInfo, int recordCount, string templateControlRelPath, SimplisityInfo headerData)
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

                strOut = DNNrocketUtils.RazorList(razorTempl, list.Cast<object>().ToList(), passSettings, headerData);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }


        public String GetDetail(SimplisityInfo postInfo, string templateControlRelPath, SimplisityInfo systemInfo)
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
                strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings, systemInfo);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public SimplisityInfo AddNew(SimplisityInfo systemInfo)
        {
            var info = new SimplisityInfo();
            info.ItemID = -1;
            info.ModuleId = systemInfo.ItemID;
            info.PortalId = DNNrocketUtils.GetPortalId();
            info.Lang = DNNrocketUtils.GetEditCulture();
            info.TypeCode = _EntityTypeCode;
            info.GUIDKey = systemInfo.GUIDKey;

            var objCtrl = new DNNrocketController();
            return objCtrl.SaveData(info);
        }

        public void Save(SimplisityInfo postInfo, SimplisityInfo systemInfo)
        {
            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            if (selecteditemid > 0)
            {
                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());

                var parentid = postInfo.GetXmlPropertyInt("genxml/dropdownlist/parentid");
                if (parentid == info.ItemID) 
                {
                    // cannot be parent of itself. so keep the same.
                    postInfo.SetXmlProperty("genxml/dropdownlist/parentid", info.ParentItemId.ToString());
                }
                else
                {
                    // read parent and insert row order (XrefItemID) value + 1.  Gap of 100 ceated on list save, so we should not have to resequence.
                    var pInfo = objCtrl.GetRecord(parentid);
                    if (pInfo != null)
                    {
                        if (parentid != info.ParentItemId)  // for sorting only change on parentid change.
                        {
                            info.XrefItemId = pInfo.XrefItemId + 1;
                        }
                        info.ParentItemId = parentid;
                        postInfo.SetXmlProperty("genxml/level", (pInfo.GetXmlPropertyInt("genxml/level") + 1).ToString());
                    }
                    else
                    {
                        info.ParentItemId = 0;
                        info.XrefItemId = 0;
                        postInfo.SetXmlProperty("genxml/level", "0");
                    }
                }
                info.GUIDKey = _systemkey;
                info.XMLData = postInfo.XMLData;
                info.ModuleId = systemInfo.ItemID;
                objCtrl.SaveData(info);
                CacheUtils.ClearAllCache();
            }
        }

        public void Delete(SimplisityInfo postInfo)
        {
            var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
            if (selecteditemid > 0)
            {
                var objCtrl = new DNNrocketController();
                objCtrl.Delete(selecteditemid);
                CacheUtils.ClearAllCache();
            }
        }

        public string AddImageToList(SimplisityInfo postInfo, string templateControlRelPath)
        {


                var imageDirectory = DNNrocketUtils.HomeDNNrocketDirectoryMapPath() + "\\images";
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                var systemkey = postInfo.GetXmlProperty("genxml/systemkey");
                var systemDataList = new SystemDataList();
                var sInfoSystem = systemDataList.GetSystemByKey(systemkey);
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
                            var newfilename = GeneralUtils.GetUniqueKey();

                            var imgInfo = new SimplisityInfo();
                            var imagerelpath = DNNrocketUtils.HomeDirectoryRel() + "/images/" + newfilename;
                            var imagepath = imageDirectory + "\\" + newfilename;

                            if (File.Exists(DNNrocketUtils.TempDirectoryMapPath() + "\\" + encryptName))
                            {

                                File.Move(DNNrocketUtils.TempDirectoryMapPath() + "\\" + encryptName, imagepath);

                                imgInfo.SetXmlProperty("genxml/hidden", "");
                                imgInfo.SetXmlProperty("genxml/hidden/imagerelpath", imagerelpath);
                                imgInfo.SetXmlProperty("genxml/hidden/imagepath", imagepath);
                                imgInfo.SetXmlProperty("genxml/hidden/filename", newfilename);
                                imgInfo.SetXmlProperty("genxml/hidden/friendlyfilename", friendlyname);
                                imgInfo.SetXmlProperty("genxml/hidden/ext", Path.GetExtension(friendlyname));

                                info.AddListItem(listname, imgInfo);
                            }
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


        public void SaveList(SimplisityInfo postInfo, SimplisityInfo systemInfo)
        {
            // For some mad reason Stringify passes back a json string which cannot be parse by JsonConvert.
            // So we do the required replace chars to make it work. 
            // NOTE: This could be an area of issue!!!!
            var requestJson = "{\"results\":" + postInfo.GetXmlProperty("genxml/hidden/jsondata").Replace("{", "").Replace("}", "").Replace("[", "{").Replace("]", "}") + "}";
            XmlDocument xmldoc = JsonConvert.DeserializeXmlNode(requestJson);

            // the processing could be done in json, but I personally prefer xml.
            var index = 100;
            var level = 0;
            RecursiveUpdateParent(xmldoc.SelectNodes("./results/*"),0, ref index, level);

            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData(_systemkey, "CATEGORYLIST", _editlang);
            info.XMLData = postInfo.XMLData;
            info.AddXmlNode(xmldoc.OuterXml, "results", "genxml");
            info.ModuleId = systemInfo.ItemID;
            objCtrl.SaveData(info);

            CacheUtils.ClearAllCache();
        }

        private void RecursiveUpdateParent(XmlNodeList xmlNodList, int parentid, ref int index, int level)
        {
            var levelstatic = level;
            var lastItemId = 0;
            var objCtrl = new DNNrocketController();
            foreach (XmlNode nod in xmlNodList)
            {
                if (nod.Name.ToLower() == "children")
                {
                    level += 1;
                    RecursiveUpdateParent(nod.SelectNodes("./*"), lastItemId, ref index, level);
                }
                if (nod.Name.ToLower() == "id")
                {
                    if (GeneralUtils.IsNumeric(nod.InnerText))
                    {
                        var catRecord = objCtrl.GetRecord(Convert.ToInt32(nod.InnerText));
                        catRecord.ParentItemId = parentid;
                        catRecord.SetXmlProperty("genxml/dropdownlist/parentid", parentid.ToString());
                        catRecord.SetXmlProperty("genxml/level", levelstatic.ToString());
                        catRecord.XrefItemId = index;
                        catRecord.GUIDKey = _systemkey;
                        objCtrl.SaveRecord(catRecord);
                        index += 100;  // so we can insert rows on parentid change for detail save.
                        lastItemId = catRecord.ItemID;
                    }
                }
            }
        }


        private String ToggleHidden(SimplisityInfo postInfo, string ControlRelPath, SimplisityInfo systemInfo)
        {
            try
            {
                var strOut = "";
                var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());
                var catData = new Category(systemInfo.ItemID, info);
                                
                if (catData.IsHidden)
                {
                    catData.IsHidden = false;
                    strOut = DNNrocketUtils.GetResourceString(ControlRelPath + "/App_LocalResources", "category.hiddenicon_false", "Text", DNNrocketUtils.GetEditCulture());
                }
                else
                {
                    catData.IsHidden = true;
                    strOut = DNNrocketUtils.GetResourceString(ControlRelPath + "/App_LocalResources", "category.hiddenicon_true", "Text", DNNrocketUtils.GetEditCulture());
                }

                objCtrl.SaveData(catData.Info);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private String ToggleDisable(SimplisityInfo postInfo, string ControlRelPath, SimplisityInfo systemInfo)
        {
            try
            {
                var strOut = "";
                var selecteditemid = postInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");

                var objCtrl = new DNNrocketController();
                var info = objCtrl.GetInfo(selecteditemid, DNNrocketUtils.GetEditCulture());
                var catData = new Category(systemInfo.ItemID, info);

                if (catData.IsDisabled)
                {
                    catData.IsDisabled = false;
                    strOut = DNNrocketUtils.GetResourceString(ControlRelPath + "/App_LocalResources", "category.toggleicon_on", "Text", DNNrocketUtils.GetEditCulture());
                }
                else
                {
                    catData.IsDisabled = true;
                    strOut = DNNrocketUtils.GetResourceString(ControlRelPath + "/App_LocalResources", "category.toggleicon_off", "Text", DNNrocketUtils.GetEditCulture());
                }

                objCtrl.SaveData(catData.Info);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


    }
}
