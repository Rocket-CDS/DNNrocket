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
        [Obsolete("Use AppThemeSystemLimpet instead")]
        public AppThemeLimpet(SystemLimpet systemData, string versionFolder = "") : base(systemData.SystemRelPath.TrimEnd('/') + "/Themes/config-w3", versionFolder)
        {
            SystemKey = systemData.SystemKey;
        }
        public AppThemeLimpet(string appThemeFolder, string versionFolder = "") : base("/DesktopModules/RocketThemes/" + appThemeFolder, versionFolder)
        {
            SystemKey = "";
            var s = appThemeFolder.Split('.');
            if (s.Length == 2) SystemKey = s[0];
        }

        public string SystemKey { get; set; }


    }
}
