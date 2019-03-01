using System;
using System.Collections.Generic;

namespace Simplisity.TemplateEngine
{
    public class TemplateGetter
    {

        private TemplateController TemplCtrl1;
        private TemplateController TemplCtrl2;

        /// <summary>
        /// Initialize the template getter 
        /// </summary>
        /// <param name="primaryMapPath">folder to look for a themes (On a multiple portal system this will usually be the portal root)</param>
        /// <param name="secondaryMapPath">fallback folder to look for a themes if not found in primary (Usually the module admin folder, where default installed templates are saved)</param>
        /// <param name="themeFolder">custom theme folder name to look for, if no template is found here the system theme, then the default theme will be searched.</param>
        /// <param name="systemThemeFolder">system theme folder, will search themefolder, systemThemefolder and then defaultThemeFolder</param>
        public TemplateGetter(string primaryBaseFolderMapPath, string themeFolder, string secondaryBaseFolderMapPath = "")
        {
            TemplCtrl1 = new TemplateController(primaryBaseFolderMapPath, themeFolder);
            TemplCtrl2 = new TemplateController(secondaryBaseFolderMapPath, themeFolder);                
        }


        /// <summary>
        /// Get template from the filesytem, search primary mappath (both themes), if not found search socendary mappath (both themes)
        /// </summary>
        /// <param name="templatename">template file anme</param>
        /// <param name="lang">langauge to get</param>
        /// <param name="replaceTemplateTokens">replace the [Template:*] tokens</param>
        /// <param name="replaceStringTokens">replace the [String:*] tokens</param>
        /// <param name="portalLevel">if false the system level template will be returned, even if a portal level template exists</param>
        /// <param name="settings">If passed a replacement of settings tokens is done directly after the template is loaded</param>
        /// <returns></returns>
        public string GetTemplateData(string templatename, string lang)
        {
            var templateData = "";
            var objT = new Template("");
            if (TemplCtrl1 != null)
            {
                objT = TemplCtrl1.GetTemplate(templatename, lang);
                templateData = objT.TemplateData;
                if (!objT.IsTemplateFound)
                {
                    objT = TemplCtrl2.GetTemplate(templatename, lang);
                    templateData = objT.TemplateData;
                }
            }

            return templateData;
        }


    }
}
