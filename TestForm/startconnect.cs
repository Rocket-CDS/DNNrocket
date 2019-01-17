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
            var imageDirectory = DNNrocketUtils.MapPath(templateControlRelPath + "/images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            var strOut = "";
            var listname = postInfo.GetXmlProperty("genxml/hidden/listname");
            var themeFolder = postInfo.GetXmlProperty("genxml/hidden/theme");
            var razortemplate = postInfo.GetXmlProperty("genxml/hidden/template");
            var fileuploadlist = postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");

            var objCtrl = new DNNrocketController();
            var info = objCtrl.GetData("testform", "TEST", DNNrocketUtils.GetEditCulture());

            if (fileuploadlist != "")
            {

                foreach (var f in fileuploadlist.Split(','))
                {
                    var imgInfo = new SimplisityInfo();
                    var imagerelpath = templateControlRelPath + "/images/" + f;
                    var imagepath = imageDirectory + "\\" + f;
                    imgInfo.SetXmlProperty("genxml/hidden/imagerelpath", imagerelpath);
                    imgInfo.SetXmlProperty("genxml/hidden/imagepath", imagepath);
                    imgInfo.SetXmlProperty("genxml/hidden/filename", f);

                    info.AddListRow(listname, imgInfo);
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
