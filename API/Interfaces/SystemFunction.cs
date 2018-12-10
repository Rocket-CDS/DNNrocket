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

                strOut = DNNrocketUtils.RazorTemplRenderList(razortemplate, 0, "", list, TemplateRelPath, themeFolder, sInfo.Lang, passSettings, new SimplisityInfo());

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }




    }
}
