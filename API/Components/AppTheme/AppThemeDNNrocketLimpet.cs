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

    public class AppThemeDNNrocketLimpet : AppThemeBase
    {
        [Obsolete("Use AppThemeDNNrocketLimpet(int portalId, string systemKey) instead")]
        public AppThemeDNNrocketLimpet(string systemKey) : base(PortalUtils.GetCurrentPortalId(), "/DesktopModules/dnnrocket/" + systemKey.Trim('/') + "/Themes/config-w3", "")
        {
            SystemKey = systemKey;
        }
        public AppThemeDNNrocketLimpet(int portalId, string systemKey) : base(portalId, "/DesktopModules/dnnrocket/" + systemKey.Trim('/') + "/Themes/config-w3", "")
        {
            SystemKey = systemKey;
        }

        public string SystemKey { get; set; }


    }
}
