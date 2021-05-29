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
        public AppThemeSystemLimpet(string systemKey) : base("/DesktopModules/dnnrocketmodules/" + systemKey.Trim('/') + "/Themes/config-w3", "")
        {
            SystemKey = systemKey;
        }

        public string SystemKey { get; set; }


    }
}
