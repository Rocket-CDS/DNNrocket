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
using System.Xml;

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

        public IEncodedString EditFlag(string classvalues = "")
        {
            var cultureCode = DNNrocketUtils.GetEditCulture();
            var strOut = "<img class='" + classvalues + "' src='/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png' alt='" + cultureCode + "' />";
            return new RawString(strOut);
        }

        public IEncodedString ThumbnailImageUrl(string url, int width = 0, int height = 0, string extraurlparams = "")
        {
            url = "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + "&w=" + width + "&h=" + height + extraurlparams;
            return new RawString(url);
        }

        public IEncodedString InjectHiddenFieldData(SimplisityInfo sInfo)
        {
            var strOut = "";
            var nodList = sInfo.XMLDoc.SelectNodes("genxml/hidden/*");
            foreach (XmlNode nod in nodList)
            {
                var id = nod.Name.ToLower();
                strOut += "<input id='" + id + "' type='hidden' value='" + sInfo.GetXmlProperty("genxml/hidden/" + id) + "'/>";
            }
            return new RawString(strOut);
        }

    }
}
