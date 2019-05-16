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
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();
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
                        var attributes = f.GetXmlProperty("genxml/textbox/attributes");

                        strOut += "<div class='w3-col m" + size + " w3-padding'>";
                        strOut += "<label>" + label + "</label>";
                        if (localized)
                        {
                            strOut += "&nbsp;" + EditFlag().ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                        {
                            xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += TextBox(info, xpath, attributes, defaultValue, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                        {
                            xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += CheckBox(info, xpath, "", attributes, defaultBool, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                        {
                            xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                            var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                            strOut += DropDownList(info, xpath, datavalue, datatext, attributes, defaultValue, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                        {
                            xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                            var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                            strOut +=  RadioButtonList(info, xpath, datavalue, datatext, attributes, defaultValue, "", localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                        {
                            xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                            var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                            strOut += CheckBoxList(info, xpath, datavalue, datatext, attributes, defaultBool, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                        {
                            var width = f.GetXmlPropertyInt("genxml/textbox/width");
                            var height = f.GetXmlPropertyInt("genxml/textbox/height");
                            if (width == 0 && height == 0)
                            {
                                width = 140;
                                height = 140;
                            }
                            strOut += ImageEdit(info, f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower(),width, height, attributes, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull")
                        {
                            var width = f.GetXmlPropertyInt("genxml/hidden/width");
                            var height = f.GetXmlPropertyInt("genxml/hidden/height");
                            if (width == 0 && height == 0)
                            {
                                width = 140;
                                height = 140;
                            }
                            strOut += ImageEditFull(info, f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower(), width, height, attributes, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "internalpage")
                        {
                            var allowEmpty = f.GetXmlPropertyBool("genxml/checkbox/allowempty");
                            xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += TabSelectListOnTabId(info, xpath, attributes, allowEmpty, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "document")
                        {
                            var fieldid =  f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += DocumentEdit(info, fieldid, attributes, localized, row).ToString();
                        }
                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                        {
                            xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                            strOut += CKEditor(info,xpath,"","",localized,row).ToString();
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
