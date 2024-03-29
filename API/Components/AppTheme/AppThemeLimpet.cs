﻿using DNNrocketAPI;
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
        /// <summary>
        /// Read AppTheme from <system>/Themes/<AppFolder>/<version>
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="systemData"></param>
        /// <param name="appThemeFolder"></param>
        /// <param name="versionFolder"></param>
        public AppThemeLimpet(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder = "") : base(portalId, systemData.SystemRelPath.TrimEnd('/') + "/Themes/" + appThemeFolder, versionFolder, systemData.SystemKey)
        {
            ProjectName = "";
            SystemKey = systemData.SystemKey;
        }
        [Obsolete("Use AppThemeLimpet(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder) instead")]
        public AppThemeLimpet(SystemLimpet systemData, string appThemeFolder, string versionFolder = "") : base(-1, systemData.SystemRelPath.TrimEnd('/') + "/Themes/" + appThemeFolder, versionFolder)
        {
            ProjectName = "";
            SystemKey = systemData.SystemKey;
        }
        [Obsolete("Use AppThemeLimpet(int portalId, SystemLimpet systemData, string appThemeFolder, string versionFolder) instead")]
        public AppThemeLimpet(SystemLimpet systemData, string versionFolder = "") : base(-1, systemData.SystemRelPath.TrimEnd('/') + "/Themes/config-w3", versionFolder)
        {
            ProjectName = "";
            SystemKey = systemData.SystemKey;
        }
        [Obsolete("Use AppThemeLimpet(int portalId, string appThemeFolder, string versionFolder, string projectName) instead")]
        public AppThemeLimpet(string appThemeFolder, string versionFolder = "") : base(-1, "/DesktopModules/RocketThemes/" + appThemeFolder, versionFolder)
        {
            ProjectName = "";
            SystemKey = "";
            var s = appThemeFolder.Split('.');
            if (s.Length == 2) SystemKey = s[0];
        }
        /// <summary>
        /// Read AppTheme from /DesktopModules/RocketThemes/<projectName>/[systemkey].<AppFolder>/<version>
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="appThemeFolder"></param>
        /// <param name="versionFolder"></param>
        /// <param name="projectName"></param>
        public AppThemeLimpet(int portalId, string appThemeFolder, string versionFolder, string projectName) : base(portalId, "/DesktopModules/RocketThemes/" + projectName + "/" + appThemeFolder, versionFolder, projectName)
        {
            ProjectName = projectName;
            SystemKey = "";
            if (appThemeFolder != null)
            {
                var s = appThemeFolder.Split('.');
                if (s.Length == 2) SystemKey = s[0];
            }
        }

        public string SystemKey { get; set; }
        public string ProjectName { get; set; }


    }
}
