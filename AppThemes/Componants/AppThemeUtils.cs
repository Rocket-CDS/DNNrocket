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

            var strFieldList = "";
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

            var imageselect = false;
            var imagegallery = false;
            var docselect = false;
            var docgallery = false;

            foreach (var flines in frows)
            {
                strFieldList += "<div class='w3-row'>" + Environment.NewLine;
                foreach (var f in flines)
                {
                    var localized = f.GetXmlProperty("genxml/checkbox/localized").ToLower();
                    var localizedbool = f.GetXmlPropertyBool("genxml/checkbox/localized");
                    var xpath = "";
                    var size = f.GetXmlProperty("genxml/select/size");
                    var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                    var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                    var defaultBool = f.GetXmlProperty("genxml/textbox/defaultvalue").ToLower();
                    var attributes = f.GetXmlProperty("genxml/textbox/attributes");

                    strFieldList +=  "\t<div class='w3-col m" + size + " w3-padding'>" + Environment.NewLine;

                    if (Path.GetExtension(label) == "") label = label + ".Text";
                    var resxLabelItem = jasonInfo.GetListItem("resxlistvalues", "genxml/text/name", label);
                    if (resxLabelItem != null)
                    {
                        strFieldList += "\t\t<label>@ResourceKey(\"" + appTheme.AppThemeFolder + "." + label + "\")</label>";
                    }
                    else
                    {
                        strFieldList += "\t\t<label>" + label + "</label>";
                    }
                    if (!localizedbool)
                    {
                        strFieldList += Environment.NewLine;
                    }
                    else
                    {
                        strFieldList += "\t\t&nbsp;@EditFlag()" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        strFieldList += "\t\t@TextBox(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                    {
                        xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        strFieldList += "\t\t@CheckBox(info,\"" + xpath + "\", \"\", \"" + attributes + "\", \"" + defaultBool + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strFieldList += "\t\t@DropDownList(info, \"" + xpath + "\",\"" + datavalue + "\",\"" + datatext + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + ",\"" + row + "\")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strFieldList += "\t\t@RadioButtonList(info,\"" + xpath + "\",\"" + datavalue + "\",\"" +datatext + "\",\"" + attributes + "\",\"" + defaultValue + "\", \"\"," + localized + ",\"" + row + "\")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        var datavalue = f.GetXmlProperty("genxml/hidden/dictionarykey");
                        var datatext = f.GetXmlProperty("genxml/lang/genxml/hidden/dictionaryvalue");
                        strFieldList += "\t\t@CheckBoxList(info,\"" +  xpath + "\",\"" + datavalue + "\",\"" + datatext+ "\",\"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        imageselect = true;
                        var width = f.GetXmlPropertyInt("genxml/textbox/width");
                        var height = f.GetXmlPropertyInt("genxml/textbox/height");
                        if (width == 0 && height == 0)
                        {
                            width = 140;
                            height = 140;
                        }
                        strFieldList += "\t\t@ImageEdit(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull")
                    {
                        imageselect = true;
                        var width = f.GetXmlPropertyInt("genxml/hidden/width");
                        var height = f.GetXmlPropertyInt("genxml/hidden/height");
                        if (width == 0 && height == 0)
                        {
                            width = 140;
                            height = 140;
                        }
                        strFieldList += "\t\t@ImageEditFull(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "internalpage")
                    {
                        var allowEmpty = f.GetXmlPropertyBool("genxml/checkbox/allowempty");
                        xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t\t@TabSelectListOnTabId(info,\"" + xpath + "\",\"" + attributes + "\"," + allowEmpty + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "document")
                    {
                        docselect = true;
                        var fieldid = f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t\t@DocumentEdit(info,\"" +  fieldid + "\",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        if (localizedbool) xpath = "genxml/lang/" + xpath;
                        strFieldList += "\t\t<div class='w3-col m12'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@CKEditor(info,\"" + xpath + "\",\"\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                        strFieldList += "\t\t<div class='w3-col' style='width:0px;height:300px;'></div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        imagegallery = true;
                        strFieldList += "\t\t<div id='imagelistcontainer'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editimages.cshtml\", \"/DesktopModules/DNNrocket/AppThemes/\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        docgallery = true;
                        strFieldList += "\t\t<div id='documentlistcontainer'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editdocuments.cshtml\", \"/DesktopModules/DNNrocket/AppThemes/\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strFieldList += "\t\t<div id='linklistcontainer'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editlinks.cshtml\", \"/DesktopModules/DNNrocket/AppThemes/\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }

                    strFieldList += "\t</div>" + Environment.NewLine;
                }
                strFieldList += "</div>" + Environment.NewLine;
            }

            if (!imagegallery && imageselect)
            {
                strFieldList += "<div id=\"dnnrocket_imageselectwrapper\">@RenderImageSelect(100, true, false, articleData.ImageFolder)</div>";
            }
            if (!docgallery && docselect)
            {
                strFieldList += "<div id=\"dnnrocket_documentselectwrapper\">@RenderDocumentSelect(true, false, articleData.DocumentFolder)</div>";
            }


            // merge to template            
            var systemInfoData = new SystemInfoData(appTheme.SystemKey);
            var strOut = FileUtils.ReadFile(systemInfoData.SystemMapPath + "\\AppThemeBase\\edit.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\edit.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = strOut.Replace("[Token:AppThemeFields]", strFieldList);
                strOut = strOut.Replace("[Token:DisplayName]", appTheme.AppDisplayName);
                strOut = strOut.Replace("[Token:SystemKey]", appTheme.SystemKey);
                strOut = strOut.Replace("[Token:appthemeresx]", appTheme.AppThemeVersionFolderRel + "/resx/");
                return strOut;
            }

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
                            strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                            strFieldList += "\t</div>" + Environment.NewLine;
                    }


                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "@if (info.GetXmlProperty(\"" + xpath + "\") != \"\")" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "else" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=60&h=60' imageheight='60' imagewidth='60'>" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "@if (info.GetListItem(\"imagelist\", 0).GetXmlProperty(\"" + xpath + "\") != \"\")" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist\", 0).GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "else" + Environment.NewLine;
                        strFieldList += "{" + Environment.NewLine;
                        strFieldList += "	<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=60&h=60' imageheight='60' imagewidth='60'>" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                    }

                }
            }

            // merge to template
            var systemInfoData = new SystemInfoData(appTheme.SystemKey);
            var strOut = FileUtils.ReadFile(systemInfoData.SystemMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\editlist.cshtml");
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
