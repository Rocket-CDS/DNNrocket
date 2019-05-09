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

        public IEncodedString RenderRocketModFields(int portalid, int moduleid, SimplisityInfo info, int row, string lang = "")
        {
            var strOut = BuidlRocketForm(portalid, moduleid, info, row, lang);
            return new RawString(strOut);
        }

        private string BuidlRocketForm(int portalid, int moduleid, SimplisityInfo info, int row, string lang = "")
        {
            var objCtrl = new DNNrocketController();
            var strOut = "";
            if (lang == "") lang = DNNrocketUtils.GetEditCulture();
            var fieldInfo = objCtrl.GetByType(portalid, moduleid, "ROCKETMODFIELDS", "", lang);
            if (fieldInfo != null)
            {
                var fl = fieldInfo.GetList("settingsdata");

                // calc rows
                var frows = new List<List<SimplisityInfo>>();
                var fline = new List<SimplisityInfo>();
                var col = 0;
                foreach (var f in fl)
                {
                    var size = f.GetXmlPropertyInt("genxml/select/size");
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
                        var xpath = "";
                        if (localized) xpath = "genxml/lang/" + xpath;
                        var size = f.GetXmlProperty("genxml/select/size");
                        var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                        var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                        var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");

                        strOut += "<div class='w3-col m" + size + " w3-padding'>";
                        strOut += "<label>" + label + "</label>";
                        if (localized)
                        {
                            strOut += "&nbsp;" + EditFlag().ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                        {
                            xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += TextBox(info, xpath, "class='w3-input w3-border' ", defaultValue, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                        {
                            xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += CheckBox(info, xpath, "", "class='w3-input w3-border' ", defaultBool, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                        {
                            xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                            var datatext = f.GetXmlProperty("genxml/hidden/dictionaryvalue");
                            strOut += DropDownList(info, xpath, datavalue, datatext, "class='w3-input w3-border' ", defaultValue, localized, row).ToString();
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
