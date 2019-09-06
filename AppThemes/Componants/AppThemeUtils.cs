using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Rocket.AppThemes.Componants;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rocket.AppThemes.Componants
{
    public static class AppThemeUtils
    {

        public static string GeneraateEditForm(AppTheme appTheme, int row)
        {

            List<SimplisityInfo> fieldList = appTheme.Info.GetList("fielddata");
            var resxItem = appTheme.Info.GetListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = GeneralUtils.DeCode(resxItem.GetXmlProperty("genxml/hidden/jsonresx"));
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, appTheme.AppCultureCode);
            }

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
                strOut += "<div class='w3-row'>" + Environment.NewLine;
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

                    strOut +=  "\t<div class='w3-col m" + size + " w3-padding'>" + Environment.NewLine;

                    if (Path.GetExtension(label) == "") label = label + ".Text";
                    var resxLabelItem = jasonInfo.GetListItem("resxlistvalues", "genxml/text/name", label);
                    if (resxLabelItem != null)
                    {
                        strOut += "\t\t<label>@ResourceKey(\"" + appTheme.AppThemeFolder + "." + label + "\")</label>";
                    }
                    else
                    {
                        strOut += "\t\t<label>" + label + "</label>";
                    }
                    if (!localized)
                    {
                        strOut += Environment.NewLine;
                    }
                    else
                    {
                        strOut += "\t\t&nbsp;@EditFlag()" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += "\t\t@TextBox(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                    {
                        xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += "\t\t@CheckBox(info,\"" + xpath + "\", \"\", \"" + attributes + "\", \"" + defaultBool + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strOut += "\t\t@DropDownList(info, \"" + xpath + "\",\"" + datavalue + "\",\"" + datatext + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + ",\"" + row + "\")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strOut += "\t\t@RadioButtonList(info,\"" + xpath + "\",\"" + datavalue + "\",\"" +datatext + "\",\"" + attributes + "\",\"" + defaultValue + "\", \"\"," + localized + ",\"" + row + "\")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strOut += "\t\t@CheckBoxList(info,\"" +  xpath + "\",\"" + datavalue + "\",\"" + datatext+ "\",\"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
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
                        strOut += "\t\t@ImageEdit(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
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
                        strOut += "\t\t@ImageEditFul(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "internalpage")
                    {
                        var allowEmpty = f.GetXmlPropertyBool("genxml/checkbox/allowempty");
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += "\t\t@TabSelectListOnTabId(info,\"" + xpath + "\",\"" + attributes + "\"," + allowEmpty + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "document")
                    {
                        var fieldid = f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += "\t\t@DocumentEdit(info,\"" +  fieldid + "\",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strOut += "\t\t<div class='w3-col m12'>" + Environment.NewLine;
                        strOut += "\t\t\t@CKEditor(info,\"" + xpath + "\",\"\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                        strOut += "\t\t</div>" + Environment.NewLine;
                        strOut += "\t\t<div class='w3-col' style='width:0px;height:300px;'></div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        strOut += "\t\t<div id='imagelistcontainer'>" + Environment.NewLine;
                        strOut += "\t\t\t@RenderTemplate(\"editimages.cshtml\", \"/DesktopModules/DNNrocket/RocketMod/\", \"config-w3\", model, \"1.0\", true)" + Environment.NewLine;
                        strOut += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        strOut += "\t\t<div id='documentlistcontainer'>" + Environment.NewLine;
                        strOut += "\t\t\t@RenderTemplate(\"editdocuments.cshtml\", \"/DesktopModules/DNNrocket/RocketMod/\", \"config-w3\", model, \"1.0\", true)" + Environment.NewLine;
                        strOut += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strOut += "\t\t<div id='linklistcontainer'>" + Environment.NewLine;
                        strOut += "\t\t\t@RenderTemplate(\"editlinks.cshtml\", \"/DesktopModules/DNNrocket/RocketMod/\", \"config-w3\", model, \"1.0\", true)" + Environment.NewLine;
                        strOut += "\t\t</div>" + Environment.NewLine;
                    }

                    strOut += "\t</div>" + Environment.NewLine;
                }
                strOut += "</div>" + Environment.NewLine;
            }
            return strOut;
        }

        public static string GeneraateEditList(AppTheme appTheme, int row)
        {

            List<SimplisityInfo> fieldList = appTheme.Info.GetList("fielddata");
            var resxItem = appTheme.Info.GetListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = GeneralUtils.DeCode(resxItem.GetXmlProperty("genxml/hidden/jsonresx"));
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, appTheme.AppCultureCode);
            }

            var strFieldList = "";

            foreach (var f in fieldList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = "";
                if (localized) xpath = "genxml/lang/" + xpath;
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");

                if (isonlist)
                {
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                    {
                        xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                    }

                    if (xpath != "")
                    {
                        strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                        strFieldList += "\t\t@info.GenXmlProperty(" + xpath + ")" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }
                }
            }

            // merge to template
            var strOut = FileUtils.ReadFile(appTheme.AppProjectFolderMapPath  + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = strOut.Replace("[Token:List]", strFieldList);
                strOut = strOut.Replace("[Token:DisplayName]", appTheme.AppDisplayName);
                strOut = strOut.Replace("[Token:SystemKey]", appTheme.SystemKey);
                strOut = strOut.Replace("[Token:appthemeresx]", appTheme.AppThemeVersionFolderRel  + "/resx/");
                return strOut;
            }

        }

    }
}
