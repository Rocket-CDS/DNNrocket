using DNNrocketAPI.Componants;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
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

    public IEncodedString DropDownLanguageList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
    {
        var dataLangKeys = new Dictionary<string, string>();
        var enabledlanguages = DNNrocketUtils.GetCultureCodeList();
        foreach (var l in enabledlanguages)
        {
            if (!dataLangKeys.ContainsKey(l))
            {
                dataLangKeys.Add(l, "<img class='' src='/DesktopModules/DNNrocket/API/images/flags/16/" + l + ".png' alt='" + l + "' /><span class='w3-small'>&nbsp;" + DNNrocketUtils.GetCultureCodeName(l) + "</span>");
            }

        }
        return DropDownList(info, xpath, dataLangKeys, attributes, defaultValue, localized, row, listname);
    }

    public IEncodedString DropDownSystemKeyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            var dataSytemKeys = new Dictionary<string, string>();

            var systemData = new DNNrocketAPI.SystemData();
            var list = systemData.GetSystemList();
            foreach (var sk in list)
            {
                if (!dataSytemKeys.ContainsKey(sk.GUIDKey) && sk.GetXmlPropertyBool("genxml/checkbox/proavailable"))
                {
                    dataSytemKeys.Add(sk.GUIDKey, sk.GUIDKey);
                }
            }
            return DropDownList(info, xpath, dataSytemKeys, attributes, defaultValue, localized, row, listname);
        }

        public IEncodedString ResourceCSV(String resourceFileKey, string keyListCSV, string lang = "", string resourceExtension = "Text")
        {
            var csvList = keyListCSV.Split(',');
            var strOut = "";
            foreach (var csv in csvList)
            {
                var resourceFileKeyCsv = resourceFileKey + "-" + csv;
                strOut += ResourceKeyString(resourceFileKeyCsv, lang, resourceExtension).Replace(",",".") + ",";
            }
            return new RawString(strOut.TrimEnd(','));
        }
        public IEncodedString ButtonText(ButtonTypes buttontype, String lang = "")
        {
            return new RawString(ResourceKeyString("DNNrocket." + buttontype, lang, "Icon") + "&nbsp;" + ResourceKeyString("DNNrocket." + buttontype, lang));
        }

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
            if (Processdata.ContainsKey("resourcepath"))
            {
                var l = Processdata["resourcepath"];
                foreach (var r in l)
                {
                    strOut = DNNrocketUtils.GetResourceString(r, resourceFileKey, resourceExtension, lang);
                    if (strOut != "") break;
                }
            }
            return strOut;
        }

        public IEncodedString RenderPagingTemplate(string scmd, string spost, SimplisityRazor model, string sreturn = "", string versionFolder = "1.0")
        {
            if (model.HeaderData == null)
            {
                model.HeaderData = new SimplisityInfo();
            }
            model.HeaderData.SetXmlProperty("genxml/s-paging-return", sreturn);
            model.HeaderData.SetXmlProperty("genxml/s-paging-cmd", scmd);
            model.HeaderData.SetXmlProperty("genxml/s-paging-post", spost);
            return RenderTemplate("Paging.cshtml", "\\DesktopModules\\DNNrocket\\api", "config-w3", model, versionFolder);
        }

        public IEncodedString RenderTemplate(string razorTemplateName, string templateControlRelPath, string themeFolder, SimplisityRazor model, string versionFolder = "1.0", bool debugMode = false)
        {
            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), versionFolder, debugMode);
            var strOut = DNNrocketUtils.RazorRender(model, razorTempl, debugMode);
            return new RawString(strOut);
        }
        public IEncodedString RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)
        {
            var strOut = DNNrocketUtils.RazorRender(model, razorTemplate, debugMode);
            return new RawString(strOut);
        }

        public IEncodedString RenderImageSelect(int imagesize, bool selectsingle = true, bool autoreturn = false, string uploadFolder = "images", string razorTemplateName = "ImageSelect.cshtml", string templateControlRelPath = "/DesktopModules/DNNrocket/images/", string themeFolder = "config-w3")
        {
            return new RawString(DNNrocketUtils.RenderImageSelect(imagesize, selectsingle,autoreturn,uploadFolder,razorTemplateName,templateControlRelPath,themeFolder));
        }

        public IEncodedString RenderDocumentSelect(bool selectsingle = true, bool autoreturn = false, string uploadFolder = "docs", string razorTemplateName = "DocSelect.cshtml", string templateControlRelPath = "/DesktopModules/DNNrocket/documents/", string themeFolder = "config-w3")
        {
            return new RawString(DNNrocketUtils.RenderDocumentSelect(selectsingle, autoreturn, uploadFolder, razorTemplateName, templateControlRelPath, themeFolder));
        }


        public IEncodedString EditFlag(string classvalues = "")
        {
            var cultureCode = DNNrocketUtils.GetEditCulture();
            var strOut = "<img class='" + classvalues + "' src='/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png' alt='" + cultureCode + "' />";
            return new RawString(strOut);
        }

        public IEncodedString ThumbnailImageUrl(string url, int width = 0, int height = 0, string extraurlparams = "", bool pngImage = true)
        {
            var pngType = "";
            if (url == "") url = "/DesktopModules/DNNrocket/api/images/noimage2.png";
            if (pngImage && url.ToLower().EndsWith(".png")) pngType = "&imgtype=png";
            if (width > 0 || height > 0)
            {
                url = "//" + DNNrocketUtils.GetDefaultWebsiteDomainUrl() + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + "&w=" + width + "&h=" + height + extraurlparams + pngType;
            }
            else
            {
                url = "//" + DNNrocketUtils.GetDefaultWebsiteDomainUrl() + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + extraurlparams + pngType;
            }
            return new RawString(url);
        }

        public IEncodedString DownloadDocument(int ItemId, int row, string fieldId, string text = "", string attributes = "")
        {
            if (text == "")
            {
                text = ResourceKey("DNNrocket.download").ToString();
            }
            var strOut = "<a " + attributes + " href='/DesktopModules/DNNrocket/api/downloadfile.ashx?fileindex=" + row + "&itemid=" + ItemId + "&fieldid=" + fieldId + "'>" + text + "</a>";
            return new RawString(strOut);
        }


        public IEncodedString ImageEdit(SimplisityInfo info, string fieldId, int width = 0, int height = 0,string attributes = "", bool localized = false, int row = 0, string listname = "", bool pngImage = true)
        {
            return ImageEditToken(info, fieldId, width, height, attributes, localized, row, "", listname, pngImage);
        }
        public IEncodedString ImageEditFull(SimplisityInfo info, string fieldId, int width = 140, int height = 140, string attributes = "", bool localized = false, int row = 0, string listname = "", bool pngImage = true)
        {
            return ImageEditToken(info, fieldId, width, height, attributes, localized, row, "full", listname, pngImage);
        }
        public IEncodedString ImageEditFullName(SimplisityInfo info, string fieldId, int width = 150, int height = 150, string attributes = "", bool localized = false, int row = 0, string listname = "", bool pngImage = true)
        {
            return ImageEditToken(info, fieldId, width, height, attributes, localized, row, "name", listname, pngImage);
        }

        private IEncodedString ImageEditToken(SimplisityInfo info, string fieldId, int width, int height, string attributes, bool localized, int row, string uiType, string listname, bool pngImage)
        {
            var xpath = "genxml/hidden/imagepath" + fieldId;
            var xpathwidth = "genxml/textbox/width" + fieldId;
            var xpathheight = "genxml/textbox/height" + fieldId;
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                xpath = "genxml/lang/" + xpath;
                xpathwidth = "genxml/lang/" + xpathwidth;
                xpathheight = "genxml/lang/" + xpathheight;
            }
            if (width == 0) width = 200;
            var xpathalt = "genxml/lang/genxml/textbox/alt" + fieldId;
            var xpathname = "genxml/textbox/name" + fieldId;

            var strOut = "<div class='w3-row'>";

            strOut += "<div class='w3-col w3-padding' style='width:" + (width + 20) + "px;'>";

            // Image section
            strOut += "<div class='w3-display-container' style='width: " + width + "px'>";

            if (info == null) info = new SimplisityInfo();
            var value = info.GetXmlProperty(xpath);
            var imgurl = info.GetXmlProperty(xpath);
            var valuewidth = info.GetXmlPropertyInt(xpathwidth);
            var valueheight = info.GetXmlPropertyInt(xpathheight);

            var upd = getUpdateAttr(xpath, "", localized);
            var id = getIdFromXpath(xpath, row, listname);
            strOut += "<input value='" + value + "' id='" + id + "' s-xpath='" + xpath + "' " + upd + " type='hidden' />";

            if (imgurl == "")
            {
                strOut += "<img src='" + ThumbnailImageUrl(imgurl,width, height, "", pngImage) + "' imageheight='" + height + "' imagewidth='" + width + "'  " + attributes + ">";
                strOut += "<span class='w3-button w3-transparent w3-display-topright dnnrocket-imagechange' title=''><i class='fas fa-edit'></i></span>";
            }
            else
            {
                strOut += "<img src='" + ThumbnailImageUrl(imgurl, width, height, "", pngImage) + "' imageheight='" + height + "' imagewidth='" + width + "' " + attributes + ">";
                strOut += "<span class='w3-button w3-transparent w3-display-topright dnnrocket-imageremove ' title=''>&times;</span>";
            }

            strOut += "</div>";

            strOut += "</div>";

            //Text seciton

            if (uiType == "full")
            {
                strOut += "<div class='w3-rest'>";
                strOut += "<div class='w3-row'>";

                strOut += "<div class='w3-col w3-padding' style='width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.width") + "</label>";
                strOut += TextBox(info, xpathwidth, " class='w3-input w3-border' autocomplete='off'", "200", false, row);
                strOut += "</div>";

                strOut += "<div class='w3-col w3-padding' style='width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.height") + "</label>";
                strOut += TextBox(info, xpathheight, " class='w3-input w3-border' autocomplete='off' datatype='int'", "0", false, row);
                strOut += "</div>";

                strOut += "<div class='w3-rest w3-padding' style='min-width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.alt") + "</label>&nbsp;" + EditFlag();
                strOut += TextBox(info, xpathalt, " class='w3-input w3-border' autocomplete='off'", "", true, row);
                strOut += "</div>";

                strOut += "</div>";
                strOut += "</div>";
            }

            if (uiType == "name")
            {
                strOut += "<div class='w3-rest'>";

                strOut += "<div class='w3-row'>";

                strOut += "<div class='w3-rest w3-padding' style='min-width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.alt") + "</label>&nbsp;" + EditFlag();
                strOut += TextBox(info, xpathalt, " class='w3-input w3-border' autocomplete='off'", "", true, row);
                strOut += "</div>";

                strOut += "</div>";

                strOut += "<div class='w3-row'>";

                strOut += "<div class='w3-col w3-padding' style='width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.width") + "</label>";
                strOut += TextBox(info, xpathwidth, " class='w3-input w3-border' autocomplete='off'", "200", false, row);
                strOut += "</div>";

                strOut += "<div class='w3-col w3-padding' style='width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.height") + "</label>";
                strOut += TextBox(info, xpathheight, " class='w3-input w3-border' autocomplete='off' datatype='int'", "0", false, row);
                strOut += "</div>";

                strOut += "<div class='w3-rest w3-padding' style='min-width:100px;'>";
                strOut += "<label class=''>" + ResourceKey("DNNrocketImages.name") + "</label>";
                strOut += TextBox(info, xpathname, " class='w3-input w3-border' autocomplete='off'", "", false, row);
                strOut += "</div>";

                strOut += "</div>";

                strOut += "</div>";
            }

            strOut += "</div>";


            return new RawString(strOut);
        }


        public IEncodedString DocumentEdit(SimplisityInfo info, string fieldId, string attributes = "", bool localized = true, int row = 0, string listname = "")
        {
            var xpath = "genxml/hidden/" + fieldId;
            var xpathname = "genxml/textbox/name" + fieldId;
            var xpathrel = "genxml/hidden/rel" + fieldId;
            var value = info.GetXmlProperty(xpath);
            var valuename = info.GetXmlProperty(xpathname);
            var valuerel = info.GetXmlProperty(xpathrel);

            var strOut = "<div class='w3-display-container' >";

            if (info == null) info = new SimplisityInfo();

            value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }

            valuename = info.GetXmlProperty(xpathname);
            if (localized && !xpathname.StartsWith("genxml/lang/"))
            {
                valuename = info.GetXmlProperty("genxml/lang/" + xpathname);
            }

            valuerel = info.GetXmlProperty(xpathrel);
            if (localized && !xpathname.StartsWith("genxml/lang/"))
            {
                valuerel = info.GetXmlProperty("genxml/lang/" + xpathrel);
            }

            var disabled = "";
            if (value == "") disabled = "disabled";
            var upd = getUpdateAttr(xpath, "", localized);
            var updname = getUpdateAttr(xpathname, "", localized);
            var id = getIdFromXpath(xpath, row, listname);
            var idname = getIdFromXpath(xpathname, row, listname);
            var idrel =  getIdFromXpath(xpathrel, row, listname);
            strOut += "<input value='" + value + "' id='" + id + "' s-xpath='" + xpath + "' " + upd + " type='hidden' />";
            strOut += "<input value='" + valuerel + "' id='" + idrel + "' s-xpath='" + xpathrel + "' " + upd + " type='hidden' />";
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
        public IEncodedString CKEditor(SimplisityInfo info, string xpath, string attributes, string startupfile, bool localized = false, int row = 0, string listname = "")
        {
            if (startupfile == "") startupfile = "startup.js";
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var upd = getUpdateAttr(xpath, attributes, localized);
            var id = getIdFromXpath(xpath, row, listname);

            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }

            var strOut = " <textarea id='" + id + "' s-datatype='coded' s-xpath='" + xpath + "' type='text'  name='editor" + id + "' " + attributes + " " + upd + " >" + value + "</textarea>";
            strOut += GetCKEditorStartup(id, startupfile);
            return new RawString(strOut);
        }
        public IEncodedString CKEditor(SimplisityInfo info, String xpath, String attributes = "")
        {
            return CKEditor(info, xpath, attributes, "startup.js");
        }

        public IEncodedString CKEditorFull(SimplisityInfo info, String xpath, String attributes, string startupfile = "startupfull.js", bool localized = false, int row = 0, string listname = "")
        {
            return CKEditor(info, xpath, attributes, startupfile, localized, row);
        }
        public IEncodedString CKEditorFull(SimplisityInfo info, String xpath, String attributes = "", bool localized = false, int row = 0, string listname = "")
        {
            return CKEditor(info, xpath, attributes, "startupfull.js", localized, row);
        }

        private string GetCKEditorStartup(string id, string filename)
        {
            var strOut = "<script>";
            var filepath = HttpContext.Current.Server.MapPath("/DesktopModules/DNNrocket/CKEditor/" + filename);
            strOut += FileUtils.ReadFile(filepath);
            strOut = strOut.Replace("{id}", id);
            var systemGlobalData = new SystemGlobalData();;            
            strOut = strOut.Replace("{cssfilelist}", systemGlobalData.CKEditorCssList);            
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

        public IEncodedString TabSelectList(SimplisityInfo info, String xpath, String attributes = "", Boolean allowEmpty = true, bool localized = false, int row = 0, string listname = "", bool showAllTabs = false)
        {
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var tList = DNNrocketUtils.GetTreeTabList(showAllTabs);
            var strOut = "";

            var upd = getUpdateAttr(xpath, "", localized);
            var id = getIdFromXpath(xpath, row, listname);

            strOut = "<select id='" + id + "' s-xpath='" + xpath + "' " + upd + " " + attributes + ">";
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

        public IEncodedString TabSelectListOnTabId(SimplisityInfo info, String xpath, String attributes = "", Boolean allowEmpty = true, bool localized = false, int row = 0, string listname = "", bool showAllTabs = false)
        {
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var tList = DNNrocketUtils.GetTreeTabListOnTabId(showAllTabs);
            var strOut = "";

            var upd = getUpdateAttr(xpath, "", localized);
            var id = getIdFromXpath(xpath, row, listname);

            strOut = "<select id='" + id + "' s-xpath='" + xpath + "' " + upd + " " + attributes + ">";
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

        public IEncodedString GetTabUrlByGuid(String tabguid)
        {
            var strOut = "";

            var t = (from kvp in TabController.GetTabsBySortOrder(PortalSettings.Current.PortalId) where kvp.UniqueId.ToString() == tabguid select kvp.TabID);
            if (t.Any())
            {
                var tabid = t.First();
                strOut = Globals.NavigateURL(tabid);
            }

            return new RawString(strOut);
        }

        public IEncodedString GetTabUrlByGuid(SimplisityInfo info, String xpath)
        {
            var strOut = "";
            var t = (from kvp in TabController.GetTabsBySortOrder(PortalSettings.Current.PortalId) where kvp.UniqueId.ToString() == info.GetXmlProperty(xpath) select kvp.TabID);
            if (t.Any())
            {
                var tabid = t.First();
                strOut = Globals.NavigateURL(tabid);
            }
            return new RawString(strOut);
        }

        public IEncodedString LinkPageURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = "", string attributes = "")
        {
            var tabid = info.GetXmlPropertyInt(xpath);
            if (tabid == 0) return new RawString("");
            var url = PagesUtils.GetPageURL(tabid);

            return GetLinkURL(url, openInNewWindow, text, attributes);
        }

        public IEncodedString LinkURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = "", string attributes = "")
        {
            var url = info.GetXmlProperty(xpath);
            return GetLinkURL(url, openInNewWindow, text, attributes);
        }

        private IEncodedString GetLinkURL(string url, bool openInNewWindow = true, string text = "", string attributes = "")
        {
            var strOut = "";
            if (url != "")
            {
                if (!url.ToLower().StartsWith("http"))
                {
                    url = url.Replace("//", "");
                    url = "http://" + url;
                }
                Uri uriResult;
                bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
                if (result)
                {
                    System.Uri uri = new Uri(url);
                    string uriWithoutScheme = uri.Host + uri.PathAndQuery + uri.Fragment;
                    if (openInNewWindow) attributes = attributes + " target='_blank'";
                    if (text == "") text = uriWithoutScheme;
                    strOut = "<a " + attributes + " href='//" + uriWithoutScheme + "'>" + text.TrimEnd('/') + "</a>";
                }
            }
            return new RawString(strOut);
        }

        public IEncodedString DataSourceList(SimplisityInfo info, int systemkey, string xpath, string attributes = "", bool allowEmpty = true, bool localized = false)
        {
            var strOut = "";
            if (info != null)
            {
                var objCtrl = new DNNrocketController();
                var filter = "and r1.XMlData.value('(genxml/hidden/systemkey)[1]','nvarchar(max)') = '" + systemkey + "' ";
                var dirlist = objCtrl.GetList(info.PortalId,-1, "MODULEPARAMS", filter);
                var tList = new Dictionary<int,string>();
                foreach (var sInfo in dirlist)
                {
                    var displayname = sInfo.GetXmlProperty("genxml/textbox/name") + ": ";
                    displayname += sInfo.GetXmlProperty("genxml/hidden/apptheme");
                    displayname += " [" + sInfo.GetXmlProperty("genxml/hidden/apptheme") + "]";
                    if (!tList.ContainsKey(sInfo.ModuleId)) tList.Add(sInfo.ModuleId, displayname);
                }


                var upd = getUpdateAttr(xpath, attributes, localized);
                var id = getIdFromXpath(xpath, 0, "");
                strOut = "<select id='" + id + "' " + upd + " " + attributes + "  s-xpath='" + xpath + "' >";
                var s = "";
                if (allowEmpty) strOut += "    <option value=''></option>";
                foreach (var tItem in tList)
                {
                    if (info.GetXmlPropertyInt(xpath) == tItem.Key)
                        s = "selected";
                    else
                        s = "";
                    strOut += "    <option value='" + tItem.Key + "' " + s + ">" + tItem.Value + "</option>";
                }
                strOut += "</select>";
            }

            return new RawString(strOut);
        }

        public IEncodedString ButtonGap(int gapSize = 10)
        {
            var strOut = "<div class='w3-hide-small w3-col' style='width:" + gapSize + "px'>&nbsp;</div><div class='w3-hide-medium w3-hide-large'>&nbsp;</div>";
            return new RawString(strOut);
        }

        public IEncodedString RenderSideMenu(SimplisityRazor model, string projectfolder, string resxFileWithoutExt, bool backbutton = false, bool signoutbutton = true, bool appthemes = false)
        {
            model.SetSetting("projectfolder", projectfolder);
            model.SetSetting("resxfile", resxFileWithoutExt);
            model.SetSetting("menu-backbutton", backbutton.ToString());
            model.SetSetting("menu-signoutbutton", signoutbutton.ToString());
            model.SetSetting("menu-appthemes", appthemes.ToString());
            return RenderTemplate("MenuOut.cshtml", "\\DesktopModules\\DNNrocket\\api", "config-w3", model, "1.0", true);
        }

        public IEncodedString AddPageCssLinkHeader(string pageId, string cssRelPath, string attributes = "")
        {
            var cachekey = "rocketlinkcssdata*" + pageId + "*" + cssRelPath;
            var rtn = (string)CacheUtils.GetCache(cachekey);
            //if (!String.IsNullOrEmpty(rtn)) return new RawString("");
            rtn = "<link rel='stylesheet' href='" + cssRelPath + "' " + attributes + " />";
            CacheUtils.SetCache(cachekey, rtn);
            return new RawString(rtn);
        }

        public IEncodedString AddPageJsScriptHeader(string pageId, string jsRelPath, string attributes = "")
        {
            var cachekey = "rocketlinkjsdata*" + pageId + "*" + jsRelPath;
            var rtn = (string)CacheUtils.GetCache(cachekey);
            //if (!String.IsNullOrEmpty(rtn)) return new RawString("");
            rtn = "<script type='text/javascript' src='" + jsRelPath + "' " + attributes + " ></script>";
            CacheUtils.SetCache(cachekey, rtn);
            return new RawString(rtn);
        }

        public IEncodedString GetTreeTabList(int portalId, List<int> selectedTabIdList, string treeviewId, string lang = "", string attributes = "", bool showAllTabs = false)
        {
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();
            var tabList = TabController.GetTabsBySortOrder(portalId, lang, true);
            var rtnString = "";
            var strOut = GetTreeTabList(rtnString, tabList, 0, 0, treeviewId, attributes, selectedTabIdList, showAllTabs);

            // add JS to action hummingbirdtree
            strOut += "";

            return new RawString(strOut);
        }

        private static string GetTreeTabList(string rtnString, List<TabInfo> tabList, int level, int parentid, string id, string attributes, List<int> selectedTabIdList, bool showAllTabs)
        {

            if (level > 50) // stop infinate loop
            {
                return rtnString;
            }

            if (level == 0)
                rtnString += "<ul id=" + id + " " + attributes + " >";
            else
                rtnString += "<ul>";

            foreach (TabInfo tInfo in tabList)
            {
                var parenttestid = tInfo.ParentId;
                if (parenttestid < 0) parenttestid = 0;
                if (parentid == parenttestid)
                {
                    if (!tInfo.IsDeleted && (tInfo.TabPermissions.Count > 2 || showAllTabs))
                    {
                        var checkedvalue = "";
                        if (selectedTabIdList.Contains(tInfo.TabID)) checkedvalue = "checked";

                        rtnString += "<li>";
                        if (tInfo.HasChildren)
                        {
                            rtnString += "<i class='fa fa-plus' style='width:30px;'></i>";
                        }
                        else
                        {
                            rtnString += "<i class='far fa-circle w3-text-white ' style='width:30px;'></i>";
                        }
                        rtnString += "&nbsp;<label>";
                        rtnString += "<input id='tabid-" + id + "-" + tInfo.TabID + "' data-id='" + tInfo.TabID + "' s-xpath='genxml/treeview/" + id + "/tabid" + tInfo.TabID + "' s-update='save' " + checkedvalue + " type='checkbox'>";
                        rtnString += tInfo.TabName;
                        rtnString += "</label>";
                        if (tInfo.HasChildren)
                        {
                            rtnString = GetTreeTabList(rtnString, tabList, level + 1, tInfo.TabID, id, attributes, selectedTabIdList, showAllTabs);
                        }
                        rtnString += "</li>";
                    }
                }
            }
            rtnString += "</ul>";
            return rtnString;
        }


    }


}
