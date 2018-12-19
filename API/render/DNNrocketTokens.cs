using NBrightDNN.render;
using RazorEngine.Text;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DNNrocketAPI.render
{
    public class DNNrocketTokens<T> : Simplisity.RazorEngineTokens<T>
    {

        public IEncodedString ResourceKey(String resourceFileKey, String lang = "", String resourceExtension = "Text")
        {
            return new RawString(ResourceKeyString(resourceFileKey, lang, resourceExtension));
        }

        public IEncodedString ResourceKeyJS(String resourceFileKey, String lang = "", String resourceExtension = "Text")
        {
            var strOut = ResourceKeyString(resourceFileKey, lang, resourceExtension);
            strOut = strOut.Replace("'", "\\'");
            return new RawString(strOut);
        }

        private string ResourceKeyString(String resourceFileKey, String lang = "", String resourceExtension = "Text")
        {
            var strOut = "";
            if (Metadata.ContainsKey("resourcepath"))
            {
                var l = Metadata["resourcepath"];
                foreach (var r in l)
                {
                    strOut = DNNrocketUtils.GetResourceString(r, resourceFileKey, resourceExtension, lang);
                    if (strOut != "") break;
                }
            }
            return strOut;
        }

        public IEncodedString RenderTemplate(string razorTemplateName, string templateControlRelPath, string themeFolder, SimplisityRazor model)
        {
            var strOut = "";
            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut = DNNrocketUtils.RazorRender(model, razorTempl, false);
            return new RawString(strOut);
        }


    }
}
