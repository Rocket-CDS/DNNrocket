using DotNetNuke.Entities.Portals;
using NBrightCore.TemplateEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DNNrocketAPI.Interfaces
{
    public static class SystemFunction
    {

        public static string TemplateRelPath = "/DesktopModules/DNNrocket/api";

        public static string SystemAdminList(SimplisityInfo sInfo)
        {
            try
            {
                var systemData = new SystemData();
                var list = systemData.GetSystemList();
                return RenderSystemAdminList(list, sInfo, 0);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static String RenderSystemAdminList(List<SimplisityInfo> list, SimplisityInfo sInfo, int recordCount)
        {

            try
            {
                if (list == null) return "";
                var strOut = "";

                // select a specific entity data type for the product (used by plugins)
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                if (themeFolder == "") themeFolder = "config-w3";
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = new Dictionary<string, string>();
                foreach (var s in sInfo.ToDictionary())
                {
                    passSettings.Add(s.Key, s.Value);
                }

                strOut = RazorTemplRenderList(razortemplate, 0, "", list, TemplateRelPath, themeFolder, sInfo.Lang, passSettings, new SimplisityInfo());

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public static string RazorTemplRenderList(string razorTemplName, int moduleid, string cacheKey, List<SimplisityInfo> objList, string templateControlPath, string theme, string lang, Dictionary<string, string> settings, SimplisityInfo headerData)
        {
                var razorTempl = GetRazorTemplateData(razorTemplName, templateControlPath, theme, lang);
                if (razorTempl != "")
                {
                    var nbRazor = new SimplisityRazor(objList.Cast<object>().ToList(), settings, HttpContext.Current.Request.QueryString);
                    nbRazor.ModuleId = moduleid;
                    nbRazor.FullTemplateName = theme + "." + razorTemplName;
                    nbRazor.TemplateName = razorTemplName;
                    nbRazor.ThemeFolder = theme;
                    nbRazor.Lang = lang;

                    nbRazor.HeaderData = headerData;

                    var razorTemplateKey = "NBrightBuyRazorKey" + theme + razorTemplName + PortalSettings.Current.PortalId.ToString();
                    razorTempl = DNNrocketUtils.RazorRender(nbRazor, razorTempl, razorTemplateKey, true);
                }
            return razorTempl;
        }

        public static string GetRazorTemplateData(string templatename, string templateControlPath,string lang, string themeFolder = "config-w3")
        {
            var controlMapPath = HttpContext.Current.Server.MapPath(templateControlPath);
            var templCtrl = new TemplateGetter(PortalSettings.Current.HomeDirectoryMapPath, controlMapPath, "Themes\\config-w3", "Themes\\" + themeFolder);
            var templ = templCtrl.GetTemplateData(templatename, lang);
            return templ;
        }



    }
}
