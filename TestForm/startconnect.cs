using DNNrocketAPI;
using Simplisity;
using System;

namespace DNNrocket.TestForm
{
    public class startconnect : DNNrocketAPI.APInterface
    {

        public override string ProcessCommand(string paramCmd, SimplisityInfo sInfo, string editlang = "")
        {

            //CacheUtils.ClearAllCache();

            var controlRelPath = "/DesktopModules/DNNrocket/TestForm";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [testform]";

            switch (paramCmd)
            {
                case "testform_get":
                    strOut = TestFormDetail(sInfo, controlRelPath);
                    break;
                case "testform_save":
                    TestFormSave(sInfo);
                    strOut = TestFormDetail(sInfo, controlRelPath);
                    break;
            }
            return strOut;

        }

        public static String TestFormDetail(SimplisityInfo sInfo, string templateControlRelPath)
        {
            try
            {
                var strOut = "";
                var selecteditemid = sInfo.GetXmlPropertyInt("genxml/hidden/selecteditemid");
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = sInfo.ToDictionary();
                               
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                strOut = DNNrocketUtils.RazorDetail(razorTempl, new SimplisityInfo(), passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void TestFormSave(SimplisityInfo sInfo)
        {
            var objCtrl = new DNNrocketController();
            var nbi = objCtrl.GetSinglePageData("testform", "TEST", "");

            if (nbi == null)
            {
                nbi = new SimplisityInfo();
                nbi.TypeCode = "";
                nbi.GUIDKey = "";
                nbi.ItemID = -1;
            }

            var smiLang = sInfo.GetLangRecord();

            nbi.XMLData = sInfo.XMLData;
            nbi.RemoveLangRecord();
            objCtrl.SaveSinglePageData("testform", "TEST", "", nbi);

        }


    }
}
