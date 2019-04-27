using DNNrocketAPI.Componants;
using DotNetNuke.Entities.Portals;
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

        public IEncodedString RenderPagingTemplate(string scmd, string spost, string sfields, string sreturn, SimplisityRazor model)
        {
            if (model.HeaderData == null)
            {
                model.HeaderData = new SimplisityInfo();
            }
            model.HeaderData.SetXmlProperty("genxml/s-paging-fields", sfields);
            model.HeaderData.SetXmlProperty("genxml/s-paging-return", sreturn);
            model.HeaderData.SetXmlProperty("genxml/s-paging-cmd", scmd);
            model.HeaderData.SetXmlProperty("genxml/s-paging-post", spost);
            return RenderTemplate("Paging.cshtml", "\\DesktopModules\\DNNrocket\\api", "config-w3", model);
        }

        public IEncodedString RenderTemplate(string razorTemplateName, string templateControlRelPath, string themeFolder, SimplisityRazor model)
        {
            var strOut = "";
            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut = DNNrocketUtils.RazorRender(model, razorTempl, false);
            return new RawString(strOut);
        }

        public IEncodedString RenderImageSelect(SimplisityRazor model,int imagesize, bool selectsingle = true, bool autoreturn = false, string uploadFolder = "images", string razorTemplateName = "ImageSelect.cshtml", string templateControlRelPath = "/DesktopModules/DNNrocket/images/", string themeFolder = "config-w3")
        {
            return new RawString(DNNrocketUtils.RenderImageSelect(model, imagesize, selectsingle,autoreturn,uploadFolder,razorTemplateName,templateControlRelPath,themeFolder));
        }

        public IEncodedString RenderDocumentSelect(SimplisityRazor model, bool selectsingle = true, bool autoreturn = false, string uploadFolder = "docs", string razorTemplateName = "DocSelect.cshtml", string templateControlRelPath = "/DesktopModules/DNNrocket/documents/", string themeFolder = "config-w3")
        {
            return new RawString(DNNrocketUtils.RenderDocumentSelect(model, selectsingle, autoreturn, uploadFolder, razorTemplateName, templateControlRelPath, themeFolder));
        }


        public IEncodedString EditFlag(string classvalues = "")
        {
            var cultureCode = DNNrocketUtils.GetEditCulture();
            var strOut = "<img class='" + classvalues + "' src='/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png' alt='" + cultureCode + "' />";
            return new RawString(strOut);
        }

        public IEncodedString ThumbnailImageUrl(string url, int width = 0, int height = 0, string extraurlparams = "")
        {
            if (width > 0 || height > 0)
            {
                url = "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + "&w=" + width + "&h=" + height + extraurlparams;
            }
            else
            {
                url = "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + extraurlparams;
            }
            return new RawString(url);
        }

        public IEncodedString ImageEdit(SimplisityInfo info, string xpath, int width = 0, int height = 0,string attributes = "", bool localized = false, int row = 0)
        {
            if (width == 0) width = 200;
            var imgurl = info.GetXmlProperty(xpath);
            var strOut = "<div class='w3-display-container' style='width: " + width + "px'>";

            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var upd = getUpdateAttr(xpath, "", localized);
            var id = getIdFromXpath(xpath, row);
            strOut += "<input value='" + value + "' id='" + id + "' s-xpath='" + xpath + "' " + upd + " type='hidden' />";

            if (imgurl == "")
            {
                strOut += "<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=/DesktopModules/DNNrocket/api/images/noimage2.png&w=" + width + "&h=" + height + "' imageheight='" + height + "' imagewidth='" + width + "'  " + attributes + ">";
                strOut += "<span class='w3-button w3-transparent w3-display-topright dnnrocket-imagechange' title=''><i class='fas fa-edit'></i></span>";
            }
            else
            {
                strOut += "<img src='/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + imgurl + "&w=" + width + "&h=" + height + "' imageheight='" + height + "' imagewidth='" + width + "' " + attributes + ">";
                strOut += "<span class='w3-button w3-transparent w3-display-topright dnnrocket-imageremove ' title=''>&times;</span>";
            }

            strOut += "</div>";
            return new RawString(strOut);
        }

        public IEncodedString DocumentEdit(SimplisityInfo info, string fieldId, string attributes = "", bool localized = true, int row = 0)
        {
            var xpath = "genxml/hidden/" + fieldId;
            var xpathname = "genxml/textbox/name" + fieldId;
            var value = info.GetXmlProperty(xpath);
            var valuename = info.GetXmlProperty(xpathname);

            var strOut = "<div class='w3-display-container' >";

            if (info == null) info = new SimplisityInfo();

            value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }

            valuename = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                valuename = info.GetXmlProperty("genxml/lang/" + xpathname);
            }

            var disabled = "";
            if (value == "") disabled = "disabled";
            var upd = getUpdateAttr(xpath, "", localized);
            var updname = getUpdateAttr(xpathname, "", localized);
            var id = getIdFromXpath(xpath, row);
            var idname = getIdFromXpath(xpathname, row);
            strOut += "<input value='" + value + "' id='" + id + "' s-xpath='" + xpath + "' " + upd + " type='hidden' />";
            strOut += "<input value='" + valuename + "' id='" + idname + "' s-xpath='" + xpathname + "' " + updname + " " + disabled + " " + attributes + " type='text' />";

            if (value == "")
            {
                strOut += "<span class='w3-button w3-transparent w3-display-topright dnnrocket-documentchange' title=''><i class='fas fa-file-upload'></i></span>";
            }
            else
            {
                strOut += "<span class='w3-button w3-transparent w3-display-topright dnnrocket-documentremove' title=''>&times;</span>";
            }

            strOut += "</div>";
            return new RawString(strOut);
        }


        /// <summary>
        /// Add all genxml/hidden/* fields to the template.
        /// </summary>
        /// <param name="sInfo"></param>
        /// <returns></returns>
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

        #region "CKeditor"

        /// <summary>
        /// Display richText CKEditor for eding
        /// NOTE: Data is sent back tothe server via a temp field.  This is populated by change event on the CKEDITOR.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="xpath"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public IEncodedString CKEditor(SimplisityInfo info, string xpath, string attributes, string startupfile, bool localized = false, int row = 0)
        {
            if (startupfile == "") startupfile = "startup.js";
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row);

            var strOut = " <textarea id='" + id + "' s-datatype='coded' s-xpath='" + xpath + "' type='text'  name='editor" + id + "' " + attributes + " " + upd + " >" + info.GetXmlProperty(xpath) + "</textarea>";
            strOut += GetCKEditorStartup(id, startupfile);
            return new RawString(strOut);
        }
        public IEncodedString CKEditor(SimplisityInfo info, String xpath, String attributes = "")
        {
            return CKEditor(info, xpath, attributes, "startup.js");
        }

        public IEncodedString CKEditorFull(SimplisityInfo info, String xpath, String attributes, string startupfile = "startupfull.js", bool localized = false, int row = 0)
        {
            return CKEditor(info, xpath, attributes, startupfile, localized, row);
        }
        public IEncodedString CKEditorFull(SimplisityInfo info, String xpath, String attributes = "", bool localized = false, int row = 0)
        {
            return CKEditor(info, xpath, attributes, "startupfull.js", localized, row);
        }

        private string GetCKEditorStartup(string id, string filename)
        {
            var strOut = "<script>";
            var filepath = HttpContext.Current.Server.MapPath("/DesktopModules/DNNrocket/CKEditor/" + filename);
            strOut += FileUtils.ReadFile(filepath);
            strOut = strOut.Replace("{id}", id);
            strOut += "</script>";
            return strOut;
        }


        #endregion

        public IEncodedString LinkInternalUrl(int portalid, int tabid, string cultureCode, PortalSettings portalSettings = null, string[] extraparams = null)
        {
            if (portalSettings == null)
            {
                portalSettings = PortalSettings.Current;
            }
            if (extraparams == null)
            {
                extraparams = new string[] { };
            }
            var strOut = DotNetNuke.Common.Globals.NavigateURL(tabid, false, portalSettings,"",cultureCode, extraparams);
            return new RawString(strOut);
        }

        public IEncodedString TabSelectList(SimplisityInfo info, String xpath, String attributes = "", Boolean allowEmpty = true, bool localized = false, int row = 0)
        {
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var tList = DNNrocketUtils.GetTreeTabListOnUniqueId();
            var strOut = "";

            var upd = getUpdateAttr(xpath, "", localized);
            var id = getIdFromXpath(xpath, row);

            strOut = "<select id='" + id + "' " + upd + " guidkey='tab' " + attributes + ">";
            var s = "";
            if (allowEmpty) strOut += "    <option value=''></option>";
            foreach (var tItem in tList)
            {
                if (info.GetXmlProperty(xpath) == tItem.Key.ToString())
                    s = "selected";
                else
                    s = "";
                strOut += "    <option value='" + tItem.Key.ToString() + "' " + s + ">" + tItem.Value + "</option>";
            }
            strOut += "</select>";

            return new RawString(strOut);
        }

        public IEncodedString TabSelectListOnTabId(SimplisityInfo info, String xpath, String attributes = "", Boolean allowEmpty = true, bool localized = false, int row = 0)
        {
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var tList = DNNrocketUtils.GetTreeTabListOnTabId();
            var strOut = "";

            var upd = getUpdateAttr(xpath, "", localized);
            var id = getIdFromXpath(xpath, row);

            strOut = "<select id='" + id + "' " + upd + " guidkey='tab' " + attributes + ">";
            var s = "";
            if (allowEmpty) strOut += "    <option value=''></option>";
            foreach (var tItem in tList)
            {
                if (info.GetXmlProperty(xpath) == tItem.Key.ToString())
                    s = "selected";
                else
                    s = "";
                strOut += "    <option value='" + tItem.Key.ToString() + "' " + s + ">" + tItem.Value + "</option>";
            }
            strOut += "</select>";

            return new RawString(strOut);
        }


    }
}
