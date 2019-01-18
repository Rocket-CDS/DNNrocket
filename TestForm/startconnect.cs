using DNNrocketAPI;
using Simplisity;
using System;
using System.IO;

namespace DNNrocket.TestForm
{
    public class startconnect : DNNrocketAPI.APInterface
    {

        public override string ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string userHostAddress, string editlang = "")
        {

            //CacheUtils.ClearAllCache();

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [testform]";

            switch (paramCmd)
            {
                case "testform_get":
                    strOut = TestFormDetail(postInfo, ControlRelPath);
                    break;
                case "testform_save":
                    TestFormSave(postInfo);
                    strOut = TestFormDetail(postInfo, ControlRelPath);
                    break;
                case "testform_add":
                    strOut = AddToList(postInfo, ControlRelPath);
                    break;
                case "testform_delete":
                    Delete(postInfo);
                    strOut = TestFormDetail(postInfo, ControlRelPath);
                    break;
                case "testform_addimage":
                    strOut = AddImageToList(postInfo, ControlRelPath);
                    break;
                case "testform_adddoc":
                    strOut = AddDocToList(postInfo, ControlRelPath);
                    break;
            }
            return strOut;

        }

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

        public static void Delete(SimplisityInfo postInfo)
        {
            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());
            objCtrl.Delete(info.ItemID);
        }

        public static string AddImageToList(SimplisityInfo postInfo, string templateControlRelPath)
        {

            var imageDirectory =  DNNrocketUtils.HomeDirectory() + "/images";
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

                objCtrl.SaveData("testform", "TEST", info);  //TestFormSave so if we add multiple it works correct.

            }

            var passSettings = postInfo.ToDictionary();

            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut = DNNrocketUtils.RazorDetail(razorTempl, info, passSettings);

            return strOut;
        }

        public static string AddDocToList(SimplisityInfo postInfo, string templateControlRelPath)
        {
            var docDirectory = DNNrocketUtils.HomeDirectory() + "/docs";
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
                        var newfilename = GeneralUtils.GetUniqueKey(12);

                        var docInfo = new SimplisityInfo();
                        var docrelpath = DNNrocketUtils.HomeRelDirectory() + "/docs/" + newfilename;
                        var docpath = docDirectory + "\\" + newfilename;

                        File.Move(DNNrocketUtils.TempDirectory() + "\\" + encryptName, docpath);

                        docInfo.SetXmlProperty("genxml/hidden", "");
                        docInfo.SetXmlProperty("genxml/hidden/docrelpath", docrelpath);
                        docInfo.SetXmlProperty("genxml/hidden/docpath", docpath);
                        docInfo.SetXmlProperty("genxml/hidden/filename", newfilename);
                        docInfo.SetXmlProperty("genxml/hidden/friendlyfilename", friendlyname);
                        docInfo.SetXmlProperty("genxml/hidden/ext", Path.GetExtension(friendlyname));

                        info.AddListRow(listname, docInfo);
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
