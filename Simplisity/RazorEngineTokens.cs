﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Simplisity
{
    public class RazorEngineTokens<T> : TemplateBase<T>
    {
        public Dictionary<String,List<String>> Processdata;

        public RazorEngineTokens()
        {
            Processdata = new Dictionary<String, List<String>>();
        }

        #region "token to add meta data for tokens"

        public IEncodedString AddProcessData(String metaType, String metaValue)
        {
            var l = new List<String>();
            if (Processdata.ContainsKey(metaType)) l = Processdata[metaType];                
            l.Add(metaValue);

            if (Processdata.ContainsKey(metaType))
                Processdata[metaType] = l;
            else
                Processdata.Add(metaType,l);

            return new RawString(""); //return nothing
        }

        public IEncodedString AddProcessData(String metaKey, String metaValue, String templateFullName)
        {
            // if we have a templatename add to preprocess cache meta data.
            return AddPreProcessData(metaKey, metaValue, templateFullName,"");
        }

        /// <summary>
        /// This method add the meta data to a specific cache list, so the we can use that data in the module code, before the razor template is rendered.
        /// This allows us to use the metadata token to add data selection information, like search filters and sort before we get the data from the DB.
        /// </summary>
        /// <param name="metaKey"></param>
        /// <param name="metaValue"></param>
        /// <param name="templateFullName">This is the cache key that is used and MUST be {theme}.{templatename}.{templateExtension}  e.g. Classic.list.cshtml</param>
        /// <param name="moduleId">moduleid to identify individual modules (required for filters)</param>
        /// <returns></returns>
        public IEncodedString AddPreProcessData(String metaKey, String metaValue,String templateFullName,String moduleId)
        {

            var cachedlist = (Dictionary<String, String>)CacheUtils.GetCache("preprocessdata" + templateFullName + moduleId);
            if (cachedlist == null)
            {
                cachedlist = new Dictionary<String, String>();
            }

            if (cachedlist.ContainsKey(metaKey))
                cachedlist[metaKey] = metaValue;
            else
                cachedlist.Add(metaKey, metaValue);

            CacheUtils.SetCache("preprocessdata" + templateFullName + moduleId, cachedlist);

            // add to internal metadata, so we can use it in the razor template if needed.
            return AddProcessData(metaKey, metaValue);
        }

        public IEncodedString AddCssLinkHeader(string cssRelPath)
        {
            var rtn = "<link rel='stylesheet' href='" + cssRelPath + "' />";
            return new RawString(rtn); 
        }

        public IEncodedString AddJsScriptHeader(string jsRelPath)
        {
            var rtn = "<script type='text/javascript' src='" + jsRelPath + "'></script>";
            return new RawString(rtn); 
        }

        #endregion

        #region "general html control tokens"

        public IEncodedString HiddenField(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);
            if (value == "") value = defaultValue;
            var strOut = "<input value='" + value.Replace("'", "&#39;") + "' id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " type='hidden' />";

            return new RawString(strOut);
        }
        public IEncodedString TextBox(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "", string type = "text")
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);

            // [TODO: add encrypt option.]
            if (value == "") value = defaultValue;

            var typeattr = "type='" + type + "'";
            if (attributes.ToLower().Contains(" type=")) typeattr = "";

            var strOut = "<input value='" + value.Replace("'", "&#39;") + "' id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " " + typeattr + " />";

            return new RawString(strOut);
        }

        public IEncodedString TextBoxDate(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlPropertyDate(xpath);
            var valueStr = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlPropertyDate("genxml/lang/" + xpath);
                valueStr = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);

            if (info.Lang != "" && valueStr == "" && GeneralUtils.IsDate(defaultValue, info.Lang))
            {
                valueStr = defaultValue;
                value = Convert.ToDateTime(defaultValue, new CultureInfo(info.Lang));
            }
            if (valueStr == "" && GeneralUtils.IsDate(defaultValue, "en-US"))
            {
                value = Convert.ToDateTime(defaultValue, new CultureInfo("en-US"));
            }

            var typeattr = "type='date'";
            if (attributes.ToLower().Contains(" type=")) typeattr = "";

            var strOut = "<input value='" + value.ToString("yyyy-MM-dd")+ "' id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " " + typeattr + " />";

            return new RawString(strOut);
        }

        public IEncodedString TextArea(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);
            if (value == "") value = defaultValue;
            var strOut = "<textarea id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " type='text'>" + value + "</textarea>";

            return new RawString(strOut);
        }

        public IEncodedString CheckBox(SimplisityInfo info, String xpath,String text, String attributes = "", Boolean defaultValue = false, bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();
            var value = getChecked(info, xpath, defaultValue);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = getChecked(info, "genxml/lang/" + xpath, defaultValue);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);
            var strOut = "    <input id='" + id + "' s-xpath='" + xpath + "' type='checkbox' " + value + " " + attributes + " " + upd + " /><label for='" + id + "'>" + text + "</label>";
            return new RawString(strOut);
        }

        public IEncodedString CheckBoxList(SimplisityInfo info, string xpath, Dictionary<string, string> dataDictionary, string attributes = "", bool defaultValue = false, bool localized = false, int row = 0, string listname = "")
        {
            var datatext = "";
            var datavalue = "";
            foreach (var d in dataDictionary)
            {
                datatext += d.Value.Replace(",",".") + ",";
                datavalue += d.Key.Replace(",", ".") + ",";
            }
            return CheckBoxList(info, xpath, datavalue.TrimEnd(','), datatext.TrimEnd(','), attributes, defaultValue, localized, row, listname);
        }
        public IEncodedString CheckBoxList(SimplisityInfo info, string xpath, string datavalue, string datatext, string attributes = "", bool defaultValue = false, bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();

            var strOut = "";

            var datat = datatext.Split(',');
            if (datavalue == "")
            {
                var lp = 1;
                foreach (var v in datat)
                {
                    datavalue += lp.ToString() + ",";
                    lp += 1;
                }
                datavalue = datavalue.TrimEnd(',');
            }
                var datav = datavalue.Split(',');
            if (datav.Count() == datat.Count())
            {
                if (attributes == "") attributes = " class='simplisitycheckboxlist' ";
                var upd = getUpdateAttr(xpath, attributes, localized);
                var id = getIdFromXpath(xpath, row, listname);
                strOut = "<div id='" + id + "' " + upd + " " + attributes + ">";
                var c = 0;
                foreach (var v in datav)
                {
                    var value = getChecked(info, xpath + "/chk[@data='" + v + "']/@value", defaultValue);
                    if (localized && !xpath.StartsWith("genxml/lang/"))
                    {
                        value = getChecked(info, "genxml/lang/" + xpath + "/chk[@data='" + v + "']/@value", defaultValue);
                    }
                    var supdate = "";
                    if (localized)
                    {
                        supdate = " s-update='lang' ";
                    }
                    strOut += "   <span class='simplisitycheckbox'> <input id='" + id + "_" + c.ToString("") + "' " + supdate + " s-xpath='" + xpath + "' name='" + id + "$" + c.ToString("") + "' type='checkbox' value='" + v + "' " + value + " /><label for='" + id + "_" + c.ToString("") + "'>&nbsp;" + datat[c] + "</label></span>";
                    c += 1;
                }
                strOut += "</div>";
            }
            return new RawString(strOut);
        }

        public IEncodedString RadioButtonList(SimplisityInfo info, string xpath, Dictionary<string, string> dataDictionary, string attributes = "", string defaultValue = "", string labelattributes = "", bool localized = false, int row = 0, string listname = "", string inputclass = "")
        {
            var datatext = "";
            var datavalue = "";
            foreach (var d in dataDictionary)
            {
                datatext += d.Value.Replace(",", ".") + ",";
                datavalue += d.Key.Replace(",", ".") + ",";
            }
            return RadioButtonList(info, xpath, datavalue.TrimEnd(','), datatext.TrimEnd(','), attributes, defaultValue, labelattributes, localized, row, listname, inputclass);
        }
        public IEncodedString RadioButtonList(SimplisityInfo info, string xpath, string datavalue, string datatext, string attributes = "", string defaultValue = "",string labelattributes = "", bool localized = false, int row = 0, string listname = "", string inputclass = "")
        {
            if (info == null) info = new SimplisityInfo();
            var strOut = "";
            var datav = datavalue.Split(',');
            var datat = datatext.Split(',');
            if (datav.Count() == datat.Count())
            {
                var value = info.GetXmlProperty(xpath);
                if (localized && !xpath.StartsWith("genxml/lang/"))
                {
                    value = info.GetXmlProperty("genxml/lang/" + xpath);
                }

                var upd = getUpdateAttr(xpath, attributes, localized);
                var id = getIdFromXpath(xpath, row, listname);
                strOut = "<div " + attributes + ">";
                var c = 0;
                var s = "";
                if (value == "") value = defaultValue; 
                foreach (var v in datav)
                {
                    if (value == v)
                        s = "checked='checked'";
                    else
                        s = "";
                    strOut += "    <input id='" + id + "_" + c.ToString("") + "' class='" + inputclass + "' s-xpath='" + xpath + "' " + upd + " name='" + id + "radio' type='radio' value='" + v + "'  " + s + "/><label " + labelattributes + " for='" + id + "_" + c.ToString("") + "'>&nbsp;" + datat[c] + "</label>";
                    c += 1;
                }
                strOut += "</div>";
            }
            return new RawString(strOut);
        }

        public IEncodedString DropDownList(SimplisityInfo info, String xpath, Dictionary<string,string> dataDictionary, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();
            var strOut = "";
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);
            strOut = "<select id='" + id + "' s-xpath='" + xpath + "' " + upd + " " + attributes + ">";
            var c = 0;
            var s = "";
            if (value == "") value = defaultValue;
            foreach (var v in dataDictionary)
            {
                if (value == v.Key)
                    s = "selected";
                else
                    s = "";

                strOut += "    <option value='" + v.Key + "' " + s + ">" + v.Value + "</option>";
                c += 1;
            }
            strOut += "</select>";

           return new RawString(strOut);
        }

        public IEncodedString DropDownList(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            if (info == null) info = new SimplisityInfo();
            var datav = datavalue.Split(',');
            var datat = datatext.Split(',');
            if (datav.Count() == datat.Count())
            {
                var dataDictionary = new Dictionary<string, string>();
                var c = 0;
                foreach (var v in datav)
                {
                    if (!dataDictionary.ContainsKey(v))
                    {
                        dataDictionary.Add(v, datat[c]);
                    }
                    c += 1;
                }
                return DropDownList(info, xpath, dataDictionary, attributes, defaultValue, localized, row, listname);
            }
            return new RawString("");
        }


        /// <summary>
        /// outputs the index fields required for a list, so we can process a sort order correctly.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public IEncodedString SortableListIndex(SimplisityInfo info, int row)
        {
            var listitemref = info.GetXmlProperty("genxml/hidden/simplisity-listitemref");
            if (listitemref == "") listitemref = GeneralUtils.GetUniqueString();
            var strOut = "<input value='" + listitemref + "' id='simplisity-listitemref_" + row + "' s-xpath='genxml/hidden/simplisity-listitemref' s-update='save' type='hidden'>";
            strOut += "<input value='" + listitemref + "' id='simplisity-listitemreflang_" + row + "' s-xpath='genxml/lang/genxml/hidden/simplisity-listitemreflang' s-update='lang' type='hidden'>";
            strOut += "<input value='" + row + "' id='index_" + row + "' s-xpath='genxml/index' s-update='save' type='hidden'>";
            return new RawString(strOut);
        }


        #endregion

        #region "extra tokens"

        public IEncodedString EmailOf(SimplisityInfo info, String xpath, string subject = "", string visibleText = "")
        {
            var strOut = info.GetXmlProperty(xpath);
            strOut = GeneralUtils.FormatAsMailTo(strOut, subject, visibleText);
            return new RawString(strOut);
        }
        public IEncodedString HtmlOf(SimplisityInfo info, String xpath)
        {
            var strOut = info.GetXmlProperty(xpath);
            strOut = System.Web.HttpUtility.HtmlDecode(strOut);
            return new RawString(strOut);
        }

        public IEncodedString HtmlOf(String htmlString)
        {
            return new RawString(System.Web.HttpUtility.HtmlDecode(htmlString));
        }
        public IEncodedString DateOf(SimplisityInfo info, String xpath, String cultureCode, String format = "d")
        {
            return DateOf(info, xpath, false, cultureCode, format);
        }
        public IEncodedString DateOf(SimplisityInfo info, String xpath, bool displayEmpty, String cultureCode, String format = "d")
        {
            if (!displayEmpty)
            {
                if (info.GetXmlProperty(xpath) != "")
                    return new RawString(info.GetXmlPropertyDate(xpath).ToString(format, new CultureInfo(cultureCode)));
                else
                    return new RawString("");
            }
            return new RawString(info.GetXmlPropertyDate(xpath).ToString(format, new CultureInfo(cultureCode)));
        }
        public IEncodedString DateOf(DateTime dateTime, String cultureCode, String format = "g")
        {
            if (dateTime > DateTime.MinValue) return new RawString(dateTime.ToString(format, new CultureInfo(cultureCode)));
            return new RawString("");
        }
        /// <summary>
        /// Succinct shortens your text to a specified size, and then dots to the end.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="value">The value.</param>
        /// <param name="showdots">if set to <c>true</c> [showdots].</param>
        /// <returns></returns>
        public IEncodedString Succinct(string value, int size, bool showdots = true)
        {
            var x = size;
            if (value.Length < size)
            {
                x = value.Length;
            }
            var rtn = value.Substring(0, x);
            if (showdots && value.Length > size) rtn += "...";
            return new RawString(rtn);
        }
        public IEncodedString BreakOf(SimplisityInfo info, String xpath)
        {
            var strOut = info.GetXmlProperty(xpath);
            return BreakOf(strOut);
        }

        public IEncodedString BreakOf(IEncodedString strIn)
        {
            return BreakOf(strIn.ToString());
        }

        public IEncodedString BreakOf(String strIn)
        {
            var strOut = System.Web.HttpUtility.HtmlEncode(strIn);
            strOut = strOut.Replace(Environment.NewLine, "<br/>");
            strOut = strOut.Replace("\n", "<br/>");  // for some reason the textbox returns unix char for newline.  (Maybe convert from json does it).
            strOut = strOut.Replace("\t", "&nbsp;&nbsp;&nbsp;");
            strOut = strOut.Replace("'", "&apos;");
            return new RawString(strOut);
        }

        public IEncodedString CheckBoxListOf(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "")
        {
            var strOut = "";
            var datav = datavalue.Split(',');
            var datat = datatext.Split(',');
            if (datav.Count() == datat.Count())
            {
                strOut = "<ul " + attributes + " s-xpath='" + xpath + "'  >";
                var c = 0;
                foreach (var v in datav)
                {
                    if (info.GetXmlProperty(xpath + "/chk[@data='" + v + "']/@value") == "True") strOut += "    <li>" + datat[c] + "</li>";
                    c += 1;
                }
                strOut += "</ul>";
            }
            return new RawString(strOut.ToString());
        }

        public IEncodedString FileSelectList(string selectedfilename, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true)
        {
            var nbi = new SimplisityInfo();
            nbi.SetXmlProperty("genxml/selectedfilename", selectedfilename);
            return FileSelectList(nbi, "genxml/selectedfilename", mappathRootFolder, attributes, allowEmpty);
        }

        public IEncodedString FileSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
        {
            var dirlist = System.IO.Directory.GetFiles(mappathRootFolder);
            var tList = new List<String>();
            foreach (var d in dirlist)
            {
                var dr = new System.IO.FileInfo(d);
                tList.Add(dr.Name);
            }
            var strOut = "";

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath,0, "");
            strOut = "<select id='" + id + "' " + upd + " " + attributes + "  s-xpath='" + xpath + "'  >";
            var s = "";
            if (allowEmpty) strOut += "    <option value=''></option>";
            foreach (var tItem in tList)
            {
                if (info.GetXmlProperty(xpath) == tItem)
                    s = "selected";
                else
                    s = "";
                strOut += "    <option value='" + tItem + "' " + s + ">" + tItem + "</option>";
            }
            strOut += "</select>";

            return new RawString(strOut);
        }


        public IEncodedString FolderSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
        {
            var dirlist = new string[0]; ;
            try
            {
                dirlist = System.IO.Directory.GetDirectories(mappathRootFolder);
            }
            catch (Exception)
            {
                // ignore, might have invalid folder.
            }

            var tList = new List<String>();
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                tList.Add(dr.Name);
            }
            var strOut = "";

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, 0, "");
            strOut = "<select id='" + id + "' " + upd + " " + attributes + "  s-xpath='" + xpath + "' >";
            var s = "";
            if (allowEmpty) strOut += "    <option value=''></option>";
            foreach (var tItem in tList)
            {
                if (info.GetXmlProperty(xpath) == tItem)
                    s = "selected";
                else
                    s = "";
                strOut += "    <option value='" + tItem + "' " + s + ">" + tItem + "</option>";
            }
            strOut += "</select>";

            return new RawString(strOut);
        }

        public IEncodedString SFields(params string[] sFields)
        {
            var strOut = "";
            var lp = 0;
            if (sFields.Count() % 2 == 0)
            {
                while (lp < sFields.Count())
                {
                    strOut += "\"" + sFields[lp] + "\":";
                    lp += 1;
                    strOut += "\"" + sFields[lp] + "\",";
                    lp += 1;
                }
            }
            else
            {
                strOut = "Key does not contain a value.  Parameters must be even containing a Key and Value";
            }
            if (strOut != "") strOut = "s-fields='{" + strOut.TrimEnd(',') + "}'";
            return new RawString(strOut);
        }

        #endregion

        #region "security token"

        /// <summary>
        /// This token is used to place a siteKey onto the return template.
        /// This key can then be checked by the client module to confirm a valid template has been returned.
        /// </summary>
        /// <param name="sessionParams"></param>
        /// <returns></returns>
        public IEncodedString SecuritySiteKey(SessionParams sessionParams)
        {
            var strOut = "<i style='display:none;'>" + sessionParams.SiteKey + "</i>";
            return new RawString(strOut);
        }

        #endregion

        #region functions


        public String getIdFromXpath(String xpath, int row, string listname)
        {
            if (xpath == "") return "";
            var rtnid = xpath.Split('/').Last();
            if (row > 0 )
            {
                rtnid = rtnid + "_" + listname + row;
            }
            return rtnid;
        }

        public String getUpdateAttr(String xpath, String attributes, bool localized)
        {
            if (attributes.Contains("s-update=")) return "";
            if (localized) return "s-update='lang'";
            if (xpath == "") return "";
            var upd = "s-update='save'";
            if (xpath.StartsWith("genxml/lang/")) upd = "s-update='lang'";
            return upd;
        }

        public String getChecked(SimplisityInfo info, String xpath, Boolean defaultValue)
        {
            if (info.GetXmlProperty(xpath).ToLower() == "true") return "checked='True'";
            if (info.GetXmlProperty(xpath) == "")
            {
                if (defaultValue) return "checked='True'";                
            }            
            return "";
        }
        #endregion


    }


}
