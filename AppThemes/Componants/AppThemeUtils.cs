using DNNrocketAPI;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.AppThemes.Componants
{
    public static class AppThemeUtils
    {

        public static string GetAppThemePageHeader(string appThemeFolder, string appThemeVersion, string templateRelPath)
        {
            var rtn = "";
            if (!String.IsNullOrEmpty(templateRelPath))
            {
                var appTheme = new AppTheme(appThemeFolder);
                if (appTheme.ActivePageHeaderTemplate != null && appTheme.ActivePageHeaderTemplate != "")
                {
                    var settings = new Dictionary<string, string>();
                    var l = new List<object>();
                    l.Add(new SimplisityInfo());
                    var nbRazor = new SimplisityRazor(l, settings, null);
                    rtn = DNNrocketUtils.RazorRender(nbRazor, appTheme.ActivePageHeaderTemplate, false);
                }
            }
            return rtn;
        }


    }
}
