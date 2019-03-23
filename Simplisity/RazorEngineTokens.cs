using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Simplisity
{
    public class RazorEngineTokens<T> : TemplateBase<T>
    {
        public Dictionary<String,List<String>> Metadata;

        public RazorEngineTokens()
        {
            Metadata = new Dictionary<String, List<String>>();
        }

        #region "token to add meta data for tokens"

        public IEncodedString AddMetaData(String metaType, String metaValue)
        {
            var l = new List<String>();
            if (Metadata.ContainsKey(metaType)) l = Metadata[metaType];                
            l.Add(metaValue);

            if (Metadata.ContainsKey(metaType))
                Metadata[metaType] = l;
            else
                Metadata.Add(metaType,l);

            return new RawString(""); //return nothing
        }

        public IEncodedString AddMetaData(String metaKey, String metaValue, String templateFullName)
        {
            // if we have a templatename add to preprocess cache meta data.
            return AddPreProcessMetaData(metaKey, metaValue, templateFullName,"");
        }

        /// <summary>
        /// This method add the meta data to a specific cache list, so the we can use that data in the module code, before the razor template is rendered.
        /// This allows use to use the metadata token to add data selection information, like search filters and sort before we get the data from the DB.
        /// </summary>
        /// <param name="metaKey"></param>
        /// <param name="metaValue"></param>
        /// <param name="templateFullName">This is the cache key that is used and MUST be {theme}.{templatename}.{templateExtension}  e.g. Classic.list.cshtml</param>
        /// <param name="moduleId">moduleid to identify individual modules (required for filters)</param>
        /// <returns></returns>
        public IEncodedString AddPreProcessMetaData(String metaKey, String metaValue,String templateFullName,String moduleId)
        {

            var cachedlist = (Dictionary<String, String>)CacheUtils.GetCache("preprocessmetadata" + templateFullName + moduleId);
            if (cachedlist == null)
            {
                cachedlist = new Dictionary<String, String>();
            }

            if (cachedlist.ContainsKey(metaKey))
                cachedlist[metaKey] = metaValue;
            else
                cachedlist.Add(metaKey, metaValue);

            CacheUtils.SetCache("preprocessmetadata" + templateFullName + moduleId, cachedlist, "metadata");

            // add to internal metadata, so we can use it in the razor template if needed.
            return AddMetaData(metaKey, metaValue);
        }

        #endregion

        #region "general html control tokens"

        public IEncodedString HiddenField(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0)
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row);
            if (value == "") value = defaultValue;
            var strOut = "<input value='" + value + "' id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " type='hidden' />";

            return new RawString(strOut);
        }

        public IEncodedString TextBox(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0)
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row);

            // [TODO: add encrypt option.]
            //var value = encrypted ? NBrightCore.common.Security.Decrypt(PortalController.Instance.GetCurrentPortalSettings().GUID.ToString(), info.GetXmlProperty(xpath)) : info.GetXmlProperty(xpath);
            if (value == "") value = defaultValue;

            var typeattr = "type='text'";
            if (attributes.ToLower().Contains(" type=")) typeattr = "";

            var strOut = "<input value='" + value.Replace("'", "&#39;") + "' id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " " + typeattr + " />";

            return new RawString(strOut);
        }

        public IEncodedString TextArea(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0)
        {
            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row);
            if (value == "") value = defaultValue;
            var strOut = "<textarea id='" + id + "' s-xpath='" + xpath + "' " + attributes + " " + upd + " type='text'>" + value + "</textarea>";

            return new RawString(strOut);
        }

        public IEncodedString CheckBox(SimplisityInfo info, String xpath,String text, String attributes = "", Boolean defaultValue = false, bool localized = false, int row = 0)
        {
            if (info == null) info = new SimplisityInfo();
            var value = getChecked(info, xpath, defaultValue);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = getChecked(info, "genxml/lang/" + xpath, defaultValue);
            }
            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row);
            var strOut = "    <input id='" + id + "' s-xpath='" + xpath + "' type='checkbox' " + value + " " + attributes + " " + upd + " /><label for='" + id + "'>" + text + "</label>";
            return new RawString(strOut);
        }

        public IEncodedString CheckBoxList(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "", Boolean defaultValue = false, bool localized = false, int row = 0)
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
                var upd = getUpdateAttr(xpath, attributes, localized);
                var id = getIdFromXpath(xpath, row);
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
                    strOut += "    <input id='" + id + "_" + c.ToString("") + "' " + supdate + " s-xpath='" + xpath + "' name='" + id + "$" + c.ToString("") + "' type='checkbox' value='" + v + "' " + value + " /><label for='" + id + "_" + c.ToString("") + "'>" + datat[c] + "</label>";
                    c += 1;
                }
                strOut += "</div>";
            }
            return new RawString(strOut);
        }

        public IEncodedString RadioButtonList(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "", String defaultValue = "",String labelattributes = "", bool localized = false, int row = 0)
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
                var id = getIdFromXpath(xpath, row);
                strOut = "<div " + attributes + ">";
                var c = 0;
                var s = "";
                if (value == "") value = defaultValue; 
                foreach (var v in datav)
                {
                    if (value == v)
                        s = "checked";
                    else
                        s = "";
                    strOut += "    <label " + labelattributes + "><input id='" + id + "_" + c.ToString("") + "' s-xpath='" + xpath + "' " + upd + " name='" + id + "radio' type='radio' value='" + v + "'  " + s + "/>" + datat[c] + "</label>";
                    c += 1;
                }
                strOut += "</div>";
            }
            return new RawString(strOut);
        }

        public IEncodedString DropDownList(SimplisityInfo info, String xpath, Dictionary<string,string> dataDictionary, String attributes = "", String defaultValue = "", bool localized = false, int row = 0)
        {
            if (info == null) info = new SimplisityInfo();
            var strOut = "";
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row);
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

        public IEncodedString DropDownList(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "", String defaultValue = "", bool localized = false, int row = 0)
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
                    dataDictionary.Add(v, datat[c]);
                    c += 1;
                }
                return DropDownList(info, xpath, dataDictionary, attributes, defaultValue, localized, row);
            }
            return new RawString("");
        }

        #endregion

        #region "extra tokens"

        public IEncodedString EmailOf(SimplisityInfo info, String xpath)
        {
            var strOut = info.GetXmlProperty(xpath);
            strOut = GeneralUtils.FormatAsMailTo(strOut);
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

        public IEncodedString FileSelectList(string selectedfilename, String relitiveRootFolder, String attributes = "", Boolean allowEmpty = true)
        {
            var nbi = new SimplisityInfo();
            nbi.SetXmlProperty("genxml/selectedfilename", selectedfilename);
            return FileSelectList(nbi, "genxml/selectedfilename", relitiveRootFolder, attributes, allowEmpty);
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
            var id = getIdFromXpath(xpath,0);
            strOut = "<select id='" + id + "' " + upd + " " + attributes + "  s-xpath='" + xpath + "'  >";
            var c = 0;
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
            var dirlist = System.IO.Directory.GetDirectories(mappathRootFolder);
            var tList = new List<String>();
            foreach (var d in dirlist)
            {
                var dr = new System.IO.DirectoryInfo(d);
                tList.Add(dr.Name);
            }
            var strOut = "";

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, 0);
            strOut = "<select id='" + id + "' " + upd + " " + attributes + "  s-xpath='" + xpath + "' >";
            var c = 0;
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

        public IEncodedString SimplisityPaging(SimplisityInfo info)
        {
            var strOut = "";
            strOut += "<a href='#' class='w3-button w3-hover-theme'>|&#10094;</a>";
            strOut += "<a href='#' class='w3-button w3-hover-theme'>..</a>";
            strOut += "<a href='#' class='w3-button w3-hover-theme'>&#10094;</a>";
            for (int i = 1; i < 6; i++)
            {
                strOut += "<a href='#' class='w3-button w3-hover-theme simplisity_pageclick' s-page='" + i + "' s-pagesize='3' >" + i + "</a>";
            }
            strOut += "<a href='#' class='w3-button w3-hover-theme'>&#10095;</a>";
            strOut += "<a href='#' class='w3-button w3-hover-theme'>..</a>";
            strOut += "<a href='#' class='w3-button w3-hover-theme'>&#10095;|</a>";

            return new RawString(strOut);
        }


        #endregion


        #region functions


        public String getIdFromXpath(String xpath, int row)
        {
            if (xpath == "") return "";
            var rtnid = xpath.Split('/').Last();
            if (row > 0 )
            {
                rtnid = rtnid + "_" + row;
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
