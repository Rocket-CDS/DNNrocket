using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.AppThemes.Componants
{
    public static class AppThemeUtils
    {

        public string BuildDynamicForm(List<SimplisityInfo> fieldList, int row)
        {
            var strOut = "";
            // calc rows
            var frows = new List<List<SimplisityInfo>>();
            var fline = new List<SimplisityInfo>();
            var col = 0;
            foreach (var f in fieldList)
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
                        strOut += RadioButtonList(info, xpath, datavalue, datatext, attributes, defaultValue, "", localized, row).ToString();
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
                        strOut += ImageEdit(info, f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower(), width, height, attributes, localized, row).ToString();
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
                        var fieldid = f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += DocumentEdit(info, fieldid, attributes, localized, row).ToString();
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += "<div class='w3-col m12'>";
                        strOut += CKEditor(info, xpath, "", "", localized, row).ToString();
                        strOut += "</div>";
                        strOut += "<div class='w3-col' style='width:0px;height:300px;'></div>";
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        strOut += "<div id='imagelistcontainer'>";
                        strOut += RenderTemplate("editimages.cshtml", "/DesktopModules/DNNrocket/RocketMod/", "config-w3", model, "1.0", true);
                        strOut += "</div>";
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        strOut += "<div id='documentlistcontainer'>";
                        strOut += RenderTemplate("editdocuments.cshtml", "/DesktopModules/DNNrocket/RocketMod/", "config-w3", model, "1.0", true);
                        strOut += "</div>";
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strOut += "<div id='linklistcontainer'>";
                        strOut += RenderTemplate("editlinks.cshtml", "/DesktopModules/DNNrocket/RocketMod/", "config-w3", model, "1.0", true);
                        strOut += "</div>";
                    }

                    strOut += "</div>";
                }
                strOut += "</div>";
            }
            return strOut;
        }



    }
}
