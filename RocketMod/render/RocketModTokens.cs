using DNNrocketAPI;
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

namespace RocketMod
{
    public class RocketModTokens<T> : DNNrocketAPI.render.DNNrocketTokens<T>
    {

        public IEncodedString RenderRocketModFields(SimplisityInfo info, int row)
        {
            var strOut = BuidlRocketForm(info,row);
            return new RawString(strOut);
        }

        private string BuidlRocketForm(SimplisityInfo info, int row)
        {
            var objCtrl = new DNNrocketController();
            var strOut = "";
            var fieldInfo = objCtrl.GetByType(info.PortalId, info.ModuleId, "ROCKETMODFIELDS", "", info.Lang);
            if (fieldInfo != null)
            {
                var fl = fieldInfo.GetList("settingsdata");

                // calc rows
                var frows = new List<List<SimplisityInfo>>();
                var fline = new List<SimplisityInfo>();
                var col = 0;
                foreach (var f in fl)
                {
                    var size = f.GetXmlPropertyInt("genxml/textbox/size");
                    if (size == 0 || size > 12) size = 12;
                    col += size;
                    if (col > 12)
                    {
                        frows.Add(fline);
                        fline = new List<SimplisityInfo>();
                        fline.Add(f);
                        col = size;
                    }
                    else
                    {
                        fline.Add(f);
                    }
                }
                frows.Add(fline);

                foreach (var flines in frows)
                {
                    strOut += "<div class='w3-row'>";
                    foreach (var f in flines)
                    {
                        var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                        var xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localized) xpath = "genxml/lang/" + xpath;
                        var size = f.GetXmlProperty("genxml/textbox/size");
                        var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");

                        strOut += "<div class='w3-col m" + size + " w3-padding'>";
                        strOut += "<label>" + label + "</label>";
                        if (localized)
                        {
                            strOut += "&nbsp;" + EditFlag().ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                        {
                            strOut += TextBox(info, xpath, "class='w3-input w3-border' ", "", localized, row).ToString();
                        }
                        strOut += "</div>";
                    }
                    strOut += "</div>";
                }
            }
            return strOut;
        }


    }
}
