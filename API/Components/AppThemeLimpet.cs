using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace DNNrocketAPI.Components
{

    public class AppThemeLimpet : AppThemeBase
    {
        public AppThemeLimpet(string systemKey, string appThemeFolder, string versionFolder = "") : base("/DesktopModules/RocketThemes/" + appThemeFolder, versionFolder)
        {
            if (systemKey == "")
            {
                var s = appThemeFolder.Split('.');
                if (s.Length == 2) systemKey = s[0];
            }
            SystemKey = systemKey;
        }

        public string SystemKey { get; set; }


    }

}
