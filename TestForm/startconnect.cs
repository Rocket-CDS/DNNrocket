using DNNrocketAPI;
using Simplisity;
using System;

namespace DNNrocket.TestForm
{
    public class startconnect : DNNrocketAPI.APInterface
    {

        public override string ProcessCommand(string paramCmd, SimplisityInfo postInfo, string editlang = "")
        {

            //CacheUtils.ClearAllCache();

            var controlRelPath = "/DesktopModules/DNNrocket/TestForm";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [testform]";

            switch (paramCmd)
            {
                case "testform_get":
                    strOut = TestFormDetail(postInfo, controlRelPath);
                    break;
                case "testform_save":
                    TestFormSave(postInfo);
                    strOut = TestFormDetail(postInfo, controlRelPath);
                    break;
            }
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


    }
}
