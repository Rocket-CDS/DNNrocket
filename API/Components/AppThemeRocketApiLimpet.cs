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

    public class AppThemeRocketApiLimpet : AppThemeBase
    {
        public AppThemeRocketApiLimpet() : base("/DesktopModules/dnnrocket/API/Themes/config-w3", "")
        {
            SystemKey = "systemapi";
        }

        public string SystemKey { get; set; }


    }
}
