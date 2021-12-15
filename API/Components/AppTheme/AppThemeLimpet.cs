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
        public AppThemeLimpet(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder = "") : base(portalId, systemData.SystemRelPath.TrimEnd('/') + "/Themes/" + appThemeFolder, versionFolder)
        {
            SystemKey = systemData.SystemKey;
        }
        [Obsolete("Use AppThemeLimpet(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder) instead")]
        public AppThemeLimpet(SystemLimpet systemData, string appThemeFolder, string versionFolder = "") : base(-1, systemData.SystemRelPath.TrimEnd('/') + "/Themes/" + appThemeFolder, versionFolder)
        {
            SystemKey = systemData.SystemKey;
        }
        [Obsolete("Use AppThemeLimpet(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder) instead")]
        public AppThemeLimpet(SystemLimpet systemData, string versionFolder = "") : base(-1, systemData.SystemRelPath.TrimEnd('/') + "/Themes/config-w3", versionFolder)
        {
            SystemKey = systemData.SystemKey;
        }
        [Obsolete("Use AppThemeLimpet(int portalId, string appThemeFolder, string versionFolder) instead")]
        public AppThemeLimpet(string appThemeFolder, string versionFolder = "") : base(-1, "/DesktopModules/RocketThemes/" + appThemeFolder, versionFolder)
        {
            SystemKey = "";
            var s = appThemeFolder.Split('.');
            if (s.Length == 2) SystemKey = s[0];
        }
        public AppThemeLimpet(int portalId, string appThemeFolder, string versionFolder = "", string org = "") : base(portalId, "/DesktopModules/RocketThemes/" + org  + "/" + appThemeFolder, versionFolder)
        {
            SystemKey = "";
            var s = appThemeFolder.Split('.');
            if (s.Length == 2) SystemKey = s[0];
        }

        public string SystemKey { get; set; }


    }
}
