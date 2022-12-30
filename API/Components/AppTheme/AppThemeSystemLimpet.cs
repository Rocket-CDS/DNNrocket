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

    public class AppThemeSystemLimpet : AppThemeBase
    {
        public AppThemeSystemLimpet(int portalId, string systemKey) : base(portalId, "/DesktopModules/dnnrocketmodules/" + systemKey.Trim('/') + "/Themes/config-w3", "", systemKey)
        {
            SystemKey = systemKey;
        }
        [Obsolete("Use AppThemeSystemLimpet(int portalid, string systemKey) instead")]
        public AppThemeSystemLimpet(string systemKey) : base(-1, "/DesktopModules/dnnrocketmodules/" + systemKey.Trim('/') + "/Themes/config-w3", "")
        {
            SystemKey = systemKey;
        }

        public string SystemKey { get; set; }


    }
}
