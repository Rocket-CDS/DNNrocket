using RazorEngine.Text;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DNNrocketAPI.render
{
    public class RocketModTokens<T> : DNNrocketTokens<T>
    {

        public IEncodedString RenderEditIframe(SimplisityRazor model)
        {
            if (model.HeaderData == null)
            {
                model.HeaderData = new SimplisityInfo();
            }
            return RenderTemplate("editiframe.cshtml", "\\DesktopModules\\DNNrocket\\RocketMod", "config-w3", model);
        }

    }
}
