using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNNrocket.TestForm
{
    public class StartConnect : DNNrocketAPI.APInterface
    {

        public override Dictionary<string, string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang = "")
        {
            var rocketInterface = new DNNrocketInterface(interfaceInfo);
            var commandSecurity = new CommandSecurity(-1,-1, rocketInterface);
            commandSecurity.AddCommand("testform_save", true);
            commandSecurity.AddCommand("testform_add", true);
            commandSecurity.AddCommand("testform_delete", true);
            commandSecurity.AddCommand("testform_addimage", true);
            commandSecurity.AddCommand("testform_adddoc", true);
            commandSecurity.AddCommand("testform_get", false);
            commandSecurity.AddCommand("testform_download", false);

            //CacheUtils.ClearAllCache();

            var rtnDic = new Dictionary<string, string>();

            var strOut = "";

            if (commandSecurity.HasSecurityAccess(paramCmd))
            {
                switch (paramCmd)
                {
                    case "testform_save":
                        TestFormSave(postInfo);
                        strOut = TestFormDetail(paramInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "testform_add":
                        strOut = AddToList(paramInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "testform_delete":
                        Delete(paramInfo);
                        strOut = TestFormDetail(paramInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "testform_addimage":
                        strOut = AddImageToList(paramInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "testform_adddoc":
                        strOut = AddDocToList(paramInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "testform_get":
                        strOut = TestFormDetail(paramInfo, rocketInterface.TemplateRelPath);
                        break;
                    case "testform_download":
                        rtnDic.Add("filenamepath", postInfo.GetXmlProperty("genxml/hidden/filerelpath"));
                        rtnDic.Add("downloadname", postInfo.GetXmlProperty("genxml/hidden/downloadname"));
                        rtnDic.Add("fileext", "");
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

            rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public static string AddToList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            var strOut = "";
            var listname = postInfo.GetXmlProperty("genxml/hidden/listname");
            var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
            var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");

            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());

            info.AddListItem(listname);

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

        public static void Delete(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());
            objCtrl.Delete(info.ItemID);
        }

        public static string AddImageToList(SimplisityInfo postInfo, string templateControlRelPath)
        {

            var imageDirectory =  DNNrocketUtils.HomeDNNrocketDirectoryMapPath() + "\\images";
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

            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());

            if (fileuploadlist != "")
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

                objCtrl.SaveData("testform", "TEST", info);  //TestFormSave so if we add multiple it works correct.

            }

            var passSettings = postInfo.ToDictionary();

            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            return strOut;
        }

        public static string AddDocToList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            var docDirectory = DNNrocketUtils.HomeDNNrocketDirectoryMapPath() + "\\docs";
            if (!Directory.Exists(docDirectory))
            {
                Directory.CreateDirectory(docDirectory);
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

            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());

            if (fileuploadlist != "")
            {

                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var encryptName = DNNrocketUtils.EncryptFileName(encryptkey, friendlyname);
                        var newfilename = GeneralUtils.GetUniqueKey();

                        var docInfo = new SimplisityInfo();
                        var docrelpath = DNNrocketUtils.HomeDirectoryRel() + "/docs/" + newfilename;
                        var docpath = docDirectory + "\\" + newfilename;

                        File.Move(DNNrocketUtils.TempDirectoryMapPath() + "\\" + encryptName, docpath);

                        docInfo.SetXmlProperty("genxml/hidden", "");
                        docInfo.SetXmlProperty("genxml/hidden/docrelpath", docrelpath);
                        docInfo.SetXmlProperty("genxml/hidden/docpath", docpath);
                        docInfo.SetXmlProperty("genxml/hidden/filename", newfilename);
                        docInfo.SetXmlProperty("genxml/hidden/friendlyfilename", friendlyname);
                        docInfo.SetXmlProperty("genxml/hidden/ext", Path.GetExtension(friendlyname));

                        info.AddListItem(listname, docInfo);
                    }

                }

                objCtrl.SaveData("testform", "TEST", info);  //TestFormSave so if we add multiple it works correct.

            }

            var passSettings = postInfo.ToDictionary();

            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            return strOut;
        }

    }
}
