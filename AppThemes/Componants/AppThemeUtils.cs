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
        public static void GenerateTemplates(AppTheme appTheme)
        {
            // output generated template.
            var formHtml = "";
            if (appTheme.RegenerateEdit) formHtml = GenerateEditForm(appTheme, "fielddata", "edit.cshtml", 0);
            var tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";
            if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

            formHtml = "";
            if (appTheme.RegenerateSettings) formHtml = GenerateEditForm(appTheme, "settingfielddata", "settings.cshtml", 0);
            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\settings.cshtml";
            if (formHtml != "") FileUtils.SaveFile(tempMapPath, formHtml);

            var listHtml = "";
            if (appTheme.RegenerateEditList) listHtml = GenerateEditList(appTheme, 0);
            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
            if (listHtml != "") FileUtils.SaveFile(tempMapPath, listHtml);

            var viewHtml = "";
            if (appTheme.RegenerateView) viewHtml = GenerateView(appTheme, 0, "view.cshtml");
            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
            if (viewHtml != "") FileUtils.SaveFile(tempMapPath, viewHtml);

            var detailHtml = "";
            if (appTheme.RegenerateDetail) detailHtml = GenerateView(appTheme, 0, "detail.cshtml");
            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\detail.cshtml";
            if (detailHtml != "") FileUtils.SaveFile(tempMapPath, detailHtml);

        }
        public static void TemplateDefaults(AppTheme appTheme, string listname)
        {
            // Tempalte Defaults
            var tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\edit.cshtml";

            if (!File.Exists(tempMapPath))
            {
                var formHtml = GenerateEditForm(appTheme, "fielddata", "edit.cshtml", 0);
                FileUtils.SaveFile(tempMapPath, formHtml);
            }

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\editlist.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var listHtml = GenerateEditList(appTheme, 0);
                FileUtils.SaveFile(tempMapPath, listHtml);
            }

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\view.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var viewHtml = GenerateView(appTheme, 0, "view.cshtml");
                FileUtils.SaveFile(tempMapPath, viewHtml);
            }

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\detail.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var viewHtml = GenerateView(appTheme, 0, "detail.cshtml");
                FileUtils.SaveFile(tempMapPath, viewHtml);
            }

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\pageheader.cshtml";
            if (!File.Exists(tempMapPath)) FileUtils.SaveFile(tempMapPath, "");

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\default\\settings.cshtml";
            if (!File.Exists(tempMapPath))
            {
                var formHtml = GenerateEditForm(appTheme, "settingfielddata", "settings.cshtml", 0);
                FileUtils.SaveFile(tempMapPath, formHtml);
            }
            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\css\\" + appTheme.AppThemeFolder + ".css";
            if (!File.Exists(tempMapPath)) FileUtils.SaveFile(tempMapPath, "");

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\js\\" + appTheme.AppThemeFolder + ".js";
            if (!File.Exists(tempMapPath)) FileUtils.SaveFile(tempMapPath, "");

            tempMapPath = appTheme.AppThemeVersionFolderMapPath + "\\resx\\" + appTheme.AppThemeFolder + ".resx";
            if (!File.Exists(tempMapPath))
            {
                // we need to use the base resx file, so format is easy.
                var baserexFileMapPath = appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\resxtemplate.xml";
                var resxFileData = FileUtils.ReadFile(baserexFileMapPath);
                FileUtils.SaveFile(tempMapPath, resxFileData);
            }

        }
        public static string GenerateEditForm(AppTheme appTheme, string listname, string basefile, int row)
        {
            List<SimplisityRecord> fieldList = appTheme.Record.GetRecordList(listname);
            var systemData = new SystemData(appTheme.SystemKey);

            var resxItem = appTheme.Record.GetRecordListItem("resxlist", "genxml/hidden/culturecode", "");
            var jsondata = "";
            if (resxItem != null)
            {
                var jsonresx = resxItem.GetXmlProperty("genxml/hidden/jsonresx");
                jsondata = GeneralUtils.DeCode(jsonresx);
            }
            var jasonInfo = new SimplisityInfo();
            if (jsondata != "")
            {
                jasonInfo = SimplisityJson.GetSimplisityInfoFromJson(jsondata, DNNrocketUtils.GetEditCulture());
            }

            var strFieldList = "";
            // calc rows
            var frows = new List<List<SimplisityRecord>>();
            var fline = new List<SimplisityRecord>();
            var col = 0;
            foreach (var f in fieldList)
            {
                var size = f.GetXmlPropertyInt("genxml/select/size");
                if (size == 0 || size > 12) size = 12;
                col += size;
                if (col > 12)
                {
                    frows.Add(fline);
                    fline = new List<SimplisityRecord>();
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
                strFieldList += "<div class='w3-row'>" + Environment.NewLine;
                foreach (var f in flines)
                {
                    var localized = f.GetXmlProperty("genxml/checkbox/localized").ToLower();
                    var localizedbool = f.GetXmlPropertyBool("genxml/checkbox/localized");
                    var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                    var size = f.GetXmlProperty("genxml/select/size");
                    var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                    var labelname = f.GetXmlProperty("genxml/textbox/name");
                    var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                    var defaultBool = f.GetXmlProperty("genxml/textbox/defaultvalue").ToLower();
                    if (defaultBool == "") defaultBool = "false";
                    var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                    var fieldname = f.GetXmlProperty("genxml/textbox/name");
                    var resxKeyName = appTheme.AppThemeFolder + "." + labelname.Replace(".Text", "");

                    strFieldList += "\t<div class='w3-col m" + size + " w3-padding'>" + Environment.NewLine;

                    if (labelname != "")
                    {
                        var resxLabelItem = jasonInfo.GetRecordListItem("resxlistvalues", "genxml/text/*[starts-with(name(), 'name')]", labelname + ".Text");
                        if (resxLabelItem != null)
                        {
                            strFieldList += "\t\t<label>@ResourceKey(\"" + resxKeyName + "\")</label>";
                        }
                        else
                        {
                            strFieldList += "\t\t<label>" + label + "</label>";
                        }
                    }
                    else
                    {
                        strFieldList += "\t\t<label>&nbsp;</label>";
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
                        strFieldList += "\t\t@TextBox(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textboxdate")
                    {
                        strFieldList += "\t\t@TextBoxDate(info,\"" + xpath + "\",\"" + attributes + " s-datatype='date' \",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "textarea")
                    {
                        strFieldList += "\t\t@TextArea(info,\"" + xpath + "\",\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                    {
                        strFieldList += "\t\t@CheckBox(info,\"" + xpath + "\", \"\", \"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        strFieldList += "@{ var datatext = ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString();}";
                        strFieldList += "@if (datatext == \"\") ";
                        strFieldList += "{ ";
                        strFieldList += " datatext = \"" + datavalue + "\"; ";
                        strFieldList += "} ";
                        strFieldList += "\t\t@DropDownList(info, \"" + xpath + "\",\"" + datavalue + "\",datatext,\"" + attributes + "\",\"" + defaultValue + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        strFieldList += "@{ var datatext = ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString();}";
                        strFieldList += "@if (datatext == \"\") ";
                        strFieldList += "{ ";
                        strFieldList += " datatext = \"" + datavalue + "\"; ";
                        strFieldList += "} ";
                        strFieldList += "\t\t@RadioButtonList(info,\"" + xpath + "\",\"" + datavalue.Replace("\"", "\\\"") + "\",datatext,\"" + attributes + "\",\"" + defaultValue + "\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                    {
                        var datavalue = GeneralUtils.DecodeCSV(f.GetXmlProperty("genxml/hidden/dictionarykey"));
                        strFieldList += "@{ var datatext = ResourceCSV(\"" + resxKeyName + "\",\"" + datavalue + "\").ToString();}";
                        strFieldList += "@if (datatext == \"\") ";
                        strFieldList += "{ ";
                        strFieldList += " datatext = \"" + datavalue + "\"; ";
                        strFieldList += "} ";
                        strFieldList += "\t\t@CheckBoxList(info,\"" + xpath + "\",\"" + datavalue + "\",datatext,\"" + attributes + "\"," + defaultBool + "," + localized + "," + row + ")" + Environment.NewLine;
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
                        strFieldList += "\t\t@ImageEdit(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
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
                        strFieldList += "\t\t@ImageEditFull(info,\"" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower() + "\"," + width + "," + height + ",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "internalpage")
                    {
                        var allowEmpty = f.GetXmlPropertyBool("genxml/checkbox/allowempty");
                        strFieldList += "\t\t@TabSelectListOnTabId(info,\"" + xpath + "\",\"" + attributes + "\"," + allowEmpty + "," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "document")
                    {
                        var fieldid = f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t\t@DocumentEdit(info,\"" + fieldid + "\",\"" + attributes + "\"," + localized + "," + row + ")" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        strFieldList += "\t\t<div class='w3-col m12'>" + Environment.NewLine;
                        strFieldList += "\t\t\t@CKEditor(info,\"" + xpath + "\",\"\", \"\"," + localized + "," + row + ")" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                        strFieldList += "\t\t<div class='w3-col' style='width:0px;height:300px;'></div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        strFieldList += "\t\t<div id='imagelistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"imgfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editimages.cshtml\", \"" + systemData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "documentgallery")
                    {
                        strFieldList += "\t\t<div id='documentlistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"docfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editdocuments.cshtml\", \"" + systemData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "linkgallery")
                    {
                        strFieldList += "\t\t<div id='linklistcontainer'>" + Environment.NewLine;
                        strFieldList += "@{" + Environment.NewLine;
                        strFieldList += " Model.SetSetting(\"linkfieldname\", \"" + fieldname + "\");" + Environment.NewLine;
                        strFieldList += "}" + Environment.NewLine;
                        strFieldList += "\t\t\t@RenderTemplate(\"editlinks.cshtml\", \"" + systemData.SystemRelPath + "\", \"config-w3\", Model, \"1.0\", true)" + Environment.NewLine;
                        strFieldList += "\t\t</div>" + Environment.NewLine;
                    }

                    strFieldList += "\t</div>" + Environment.NewLine;
                }
                strFieldList += "</div>" + Environment.NewLine;
            }


            // merge to template            
            var strOut = FileUtils.ReadFile(systemData.SystemMapPath + "\\AppThemeBase\\" + basefile);
            if (strOut == "") strOut = FileUtils.ReadFile(appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\" + basefile);
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = ReplaceTemplateTokens(appTheme, strOut, strFieldList, systemData);
                return strOut;
            }

        }
        public static string ReplaceTemplateTokens(AppTheme appTheme, string templateText, string strFieldList, SystemData systemData)
        {
            templateText = templateText.Replace("[Token:AppThemeFields]", strFieldList);
            templateText = templateText.Replace("[Token: AppThemeFields]", strFieldList);
            templateText = templateText.Replace("[Token:List]", strFieldList);
            templateText = templateText.Replace("[Token: List]", strFieldList);
            templateText = templateText.Replace("[Token:SystemKey]", appTheme.SystemKey);
            templateText = templateText.Replace("[Token: SystemKey]", appTheme.SystemKey);
            templateText = templateText.Replace("[Token:DefaultInterface]", systemData.DefaultInterface);
            templateText = templateText.Replace("[Token: DefaultInterface]", systemData.DefaultInterface);
            templateText = templateText.Replace("[Token:appthemeresx]", appTheme.AppThemeVersionFolderRel + "/resx/");
            templateText = templateText.Replace("[Token: appthemeresx]", appTheme.AppThemeVersionFolderRel + "/resx/");

            return templateText;
        }
        public static string GenerateEditList(AppTheme appTheme, int row)
        {

            List<SimplisityRecord> fieldList = appTheme.Record.GetRecordList("fielddata");

            var strFieldList = "";
            var sortedList = new List<SimplisityRecord>();
            for (int i = 1; i < 12; i++)
            {
                foreach (var f in fieldList)
                {
                    var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                    if (isonlist && f.GetXmlPropertyInt("genxml/select/listcol") == i)
                    {
                        sortedList.Add(f);
                    }
                }
            }

            foreach (var f in sortedList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                var fieldname = f.GetXmlProperty("genxml/textbox/name");

                if (isonlist)
                {
                    if (xpath != "")
                    {
                        strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                        strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;

                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                        {
                            strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                            strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                            strFieldList += "\t</div>" + Environment.NewLine;
                        }

                        if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                        {
                            strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                            strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist" + fieldname + "\", 0).GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                            strFieldList += "\t</div>" + Environment.NewLine;
                        }
                    }

                }
            }

            // merge to template
            var systemData = new SystemData(appTheme.SystemKey);
            var strOut = FileUtils.ReadFile(systemData.SystemMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "") strOut = FileUtils.ReadFile(appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\editlist.cshtml");
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = ReplaceTemplateTokens(appTheme, strOut, strFieldList, systemData);
                return strOut;
            }

        }
        public static string GenerateView(AppTheme appTheme, int row, string templatefilename)
        {
            List<SimplisityRecord> fieldList = appTheme.Record.GetRecordList("fielddata");

            var strFieldList = "";
            foreach (var f in fieldList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                var fieldname = f.GetXmlProperty("genxml/textbox/name");

                if (xpath != "")
                {
                    strFieldList += "\t<div class='w3-col m2 w3-padding'>" + Environment.NewLine;
                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                    {
                        strFieldList += "\t\t@HtmlOf(info.GetXmlProperty(\"" + xpath + "\"))" + Environment.NewLine;
                    }
                    else
                    {
                        strFieldList += "\t\t@info.GetXmlProperty(\"" + xpath + "\")" + Environment.NewLine;
                    }
                    strFieldList += "\t</div>" + Environment.NewLine;


                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagefull" || f.GetXmlProperty("genxml/select/type").ToLower() == "image")
                    {
                        xpath = "genxml/hidden/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }

                    if (f.GetXmlProperty("genxml/select/type").ToLower() == "imagegallery")
                    {
                        xpath = "genxml/hidden/" + fieldname.Trim(' ').ToLower();
                        strFieldList += "\t<div class='w3-col w3-padding' style='width:80px;'>" + Environment.NewLine;
                        strFieldList += "<img src=\"@ThumbnailImageUrl(info.GetListItem(\"imagelist" + fieldname + "\", 0).GetXmlProperty(\"" + xpath + "\"), 60 , 60)\" />" + Environment.NewLine;
                        strFieldList += "\t</div>" + Environment.NewLine;
                    }
                }

            }

            // merge to template
            var systemData = new SystemData(appTheme.SystemKey);
            var strOut = FileUtils.ReadFile(systemData.SystemMapPath + "\\AppThemeBase\\" + templatefilename);
            if (strOut == "") strOut = FileUtils.ReadFile(appTheme.AppProjectFolderMapPath + "\\AppThemeBase\\" + templatefilename);
            if (strOut == "")
            {
                return strFieldList;
            }
            else
            {
                strOut = ReplaceTemplateTokens(appTheme, strOut, strFieldList, systemData);
                return strOut;
            }

        }
    }
}
