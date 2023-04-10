using DNNrocketAPI.Components;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using Newtonsoft.Json.Linq;
using RazorEngine.Text;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;
using System.Xml;
using System.Xml.Linq;

namespace DNNrocketAPI.render
{
    public class DNNrocketTokens<T> : Simplisity.RazorEngineTokens<T>
    {
        public IEncodedString AddProcessDataResx(AppThemeLimpet appTheme, bool includeAPIresx = false)
        {
            var resxPortalPath = appTheme.PortalFileDirectoryRel.TrimEnd('/') + "/resx/";
            AddProcessData("resourcepath", resxPortalPath);
            var resxSysPath = appTheme.AppThemeVersionFolderRel.TrimEnd('/') + "/resx/";
            AddProcessData("resourcepath", resxSysPath);
            if (includeAPIresx)
            {
                var apiResx = "/DesktopModules/DNNrocket/api/App_LocalResources/";
                AddProcessData("resourcepath", apiResx);
            }
            return new RawString(""); //return nothing
        }
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
        public IEncodedString DropDownCurrencyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            var dataLangKeys = new Dictionary<string, string>();
            var enabledCurrency = DNNrocketUtils.GetCurrencyList();
            return DropDownList(info, xpath, enabledCurrency, attributes, defaultValue, localized, row, listname);
        }
        public IEncodedString DropDownCultureCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            var cultureCodes = new Dictionary<string, string>();
            var cultureList = DNNrocketUtils.GetCultureCodeList(info.PortalId);
            foreach (var cc in cultureList)
            {
                cultureCodes.Add(cc, cc);
            }
            return DropDownList(info, xpath, cultureCodes, attributes, defaultValue, localized, row, listname);
        }



    public IEncodedString DropDownSystemKeyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
        {
            var dataSytemKeys = new Dictionary<string, string>();

            var systemDataList = new SystemLimpetList();
            var list = systemDataList.GetSystemList();
            foreach (var sk in list)
            {
                if (!dataSytemKeys.ContainsKey(sk.SystemKey) && sk.Active)
                {
                    dataSytemKeys.Add(sk.SystemKey, sk.SystemKey);
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
            if (buttontype == ButtonTypes.next)
            {
                return new RawString(ResourceKeyString("DNNrocket." + buttontype, lang) + "&nbsp;" + ResourceKeyString("DNNrocket." + buttontype, lang, "Icon"));
            }
            else
            {
                return new RawString(ResourceKeyString("DNNrocket." + buttontype, lang, "Icon") + "&nbsp;" + ResourceKeyString("DNNrocket." + buttontype, lang));

            }
        }
        public IEncodedString ButtonIcon(ButtonTypes buttontype, String lang = "")
        {
            return new RawString(ResourceKeyString("DNNrocket." + buttontype, lang, "Icon"));
        }
        public IEncodedString ResourceKeyMod(String moduleRef, String resourceFileKey, String lang = "", String resourceExtension = "Text")
        {
            if (moduleRef != "") moduleRef = moduleRef + "_";
            return new RawString(ResourceKeyString(moduleRef + resourceFileKey, lang, resourceExtension));
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

        public IEncodedString RenderLanguageSelector(string scmd, Dictionary<string,string> sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)
        {
            model.SetSetting("scmd", scmd);
            if (!sfieldDict.ContainsKey("selectedculturecode")) sfieldDict.Add("selectedculturecode", "[LANGUAGE]");
            if (!sfieldDict.ContainsKey("scmdprocess")) sfieldDict.Add("scmdprocess", "changeeditculture");
            var sfields = "{";
            foreach (var ds in sfieldDict)
            {
                sfields += "\"" + ds.Key + "\":\"" + ds.Value + "\",";
            }
            sfields = sfields.TrimEnd(',') + "}";
            model.SetSetting("sfields", sfields);
            return RenderTemplate("LanguageChange.cshtml", appThemeSystem, model, true);
        }
        [Obsolete("Use: RenderLanguageSelector(string scmd, Dictionary<string,string> sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)")]
        public IEncodedString RenderLanguageSelector(string scmd, string sfields, string systemKey, SimplisityRazor model)
        {
            model.SetSetting("scmd", scmd);
            model.SetSetting("sfields", sfields);
            return RenderTemplate("LanguageChange.cshtml", new AppThemeSystemLimpet(PortalUtils.GetCurrentPortalId(), systemKey), model, true);
        }
        [Obsolete("Use: RenderLanguageSelector(string scmd, Dictionary<string,string> sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)")]
        public IEncodedString RenderLanguageSelector(string scmd, string sfields, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)
        {
            model.SetSetting("scmd", scmd);
            model.SetSetting("sfields", sfields);
            return RenderTemplate("LanguageChange.cshtml", appThemeSystem, model, true);
        }
        [Obsolete("Use: RenderLanguageSelector(string scmd, Dictionary<string,string> sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)")]
        public IEncodedString RenderLanguageSelector(string scmd, string sfields, AppThemeDNNrocketLimpet appThemeDNNrocket, SimplisityRazor model)
        {
            model.SetSetting("scmd", scmd);
            model.SetSetting("sfields", sfields);
            return RenderTemplate("LanguageChange.cshtml", appThemeDNNrocket, model, true);
        }
        public IEncodedString RenderRemoteLanguageSelector(string scmd, string sfields, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)
        {
            model.SetSetting("scmd", scmd);
            model.SetSetting("sfields", sfields);
            return RenderTemplate("RemoteLanguageChange.cshtml", appThemeSystem, model, true);
        }
        public IEncodedString RenderRemoteLanguageSelector(string scmd, string sfields, AppThemeDNNrocketLimpet appThemeDNNrocket, SimplisityRazor model)
        {
            model.SetSetting("scmd", scmd);
            model.SetSetting("sfields", sfields);
            return RenderTemplate("RemoteLanguageChange.cshtml", appThemeDNNrocket, model, true);
        }
        public IEncodedString RenderTemplate(string razorTemplateName, AppThemeRocketApiLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)
        {
            return RenderTemplate(appThemeSystem.GetTemplate(razorTemplateName), model, cacheOff);
        }
        public IEncodedString RenderTemplate(string razorTemplateName, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)
        {
            return RenderTemplate(appThemeSystem.GetTemplate(razorTemplateName), model, cacheOff);
        }
        public IEncodedString RenderTemplate(string razorTemplateName, AppThemeDNNrocketLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)
        {
            return RenderTemplate(appThemeSystem.GetTemplate(razorTemplateName), model, cacheOff);
        }
        public IEncodedString RenderTemplate(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)
        {
            return RenderTemplate(appTheme.GetTemplate(razorTemplateName),model, cacheOff);
        }
        public IEncodedString RenderTemplate(string razorTemplateName, string moduleRef, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)
        {
            return RenderTemplate(appTheme.GetTemplate(razorTemplateName, moduleRef), model, cacheOff);
        }
        public IEncodedString RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)
        {
            var strOut = "";
            var pr = RenderRazorUtils.RazorProcess(model, razorTemplate, debugMode);
            if (pr.StatusCode == "00")
                strOut = pr.RenderedText;
            else
                if (debugMode) strOut = "ERROR: " + pr.StatusCode + " ---> " + pr.ErrorMsg;

            return new RawString(strOut);
        }
        public IEncodedString RenderPlugin(string interfaceKey, string cmd, SimplisityRazor model)
        {
            var strOut = "";
            var systemData = (SystemLimpet)model.GetDataObject("systemdata");
            if (systemData == null) return new RawString("systemData object not available, the 'systemdata' SystemLimpet object must be added to the Model.");
            foreach (var p in systemData.RazorList)
            {
                if (interfaceKey == p.InterfaceKey)
                {
                    var assembly = p.Assembly;
                    var nameSpaceClass = p.NameSpaceClass;
                    var cacheKeyRazor = assembly + "," + nameSpaceClass;
                    var razorprov = (RazorInterface)CacheUtils.GetCache(cacheKeyRazor);
                    if (razorprov == null)
                    {
                        razorprov = RazorInterface.Instance(assembly, nameSpaceClass);
                        CacheUtils.SetCache(cacheKeyRazor, razorprov);
                    }
                    strOut = razorprov.RenderToken(interfaceKey, cmd, model);
                }
            }
            return new RawString(strOut);
        }
        [Obsolete("Use RenderTemplate(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false) instead")]
        public IEncodedString RenderTemplate(string razorTemplateName, string templateControlRelPath, string themeFolder, SimplisityRazor model, string versionFolder = "1.0", bool debugMode = false)
        {
            var razorTempl = RenderRazorUtils.GetSystemRazorTemplate("",razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), versionFolder, debugMode);
            var strOut = "";
            var pr = RenderRazorUtils.RazorProcess(model, razorTempl, debugMode);
            strOut = pr.RenderedText;
            return new RawString(strOut);
        }
        public IEncodedString RenderXml(SimplisityInfo info)
        {
            var appTheme = new AppThemeDNNrocketLimpet("api");
            var razorTempl = appTheme.GetTemplate("XmlModelDisplay.cshtml");
            var nbRazor = new SimplisityRazor(info);
            var strOut = "";
            var pr = RenderRazorUtils.RazorProcess(nbRazor, razorTempl, true);
            strOut = pr.RenderedText;
            return new RawString(strOut);
        }
        [Obsolete("Use RenderTemplate(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false) instead")]
        public IEncodedString RenderTemplateInfo(string razorTemplateName, SimplisityInfo info, Dictionary<string, object> dataObjects = null, string templateControlRelPath = "/DesktopModules/DNNrocket/api/", string themeFolder = "config-w3", string lang = "", string versionFolder = "1.0", Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugMode = false)
        {
            var razorTempl = RenderRazorUtils.GetSystemRazorTemplate("",razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), versionFolder, debugMode);
            return new RawString(RenderRazorUtils.RazorObjectRender(razorTempl, info, dataObjects, settings, sessionParams, debugMode));
        }        
        public IEncodedString RenderImageSelect(string systemKey, string imageFolderRel, bool singleselect = true, bool autoreturn = false)
        {
            return new RawString(DNNrocketUtils.RenderImageSelect(systemKey, imageFolderRel, singleselect, autoreturn));
        }
        public IEncodedString RenderImageSelect(int moduleid, bool singleselect = true, bool autoreturn = false)
        {
            var moduleParams = new ModuleParams(moduleid);
            return RenderImageSelect(moduleParams, singleselect, autoreturn);
        }
        public IEncodedString RenderImageSelect(ModuleParams moduleParams, bool singleselect = true, bool autoreturn = false)
        {
            return RenderImageSelect(moduleParams.SystemKey, moduleParams.ImageFolderRel, singleselect, autoreturn);
        }

        public IEncodedString RenderDocumentSelect(string systemKey, string docFolderRel, bool singleselect = true, bool autoreturn = false)
        {
            return new RawString(DNNrocketUtils.RenderDocumentSelect(systemKey, docFolderRel, singleselect, autoreturn));
        }
        public IEncodedString RenderDocumentSelect(int moduleid, bool singleselect = true, bool autoreturn = false)
        {
            var moduleParams = new ModuleParams(moduleid);
            return RenderDocumentSelect(moduleParams, singleselect, autoreturn);
        }
        public IEncodedString RenderDocumentSelect(ModuleParams moduleParams, bool singleselect = true, bool autoreturn = false)
        {
            return RenderImageSelect(moduleParams.SystemKey, moduleParams.DocumentFolderRel, singleselect, autoreturn);
        }
        public IEncodedString TranslationLock(SimplisityInfo info, string xpath, bool show = true, int row = 0, bool active = true)
        {
            if (!show) return new RawString("");
            var xpathSplit = xpath.Split('/');
            var fieldId = xpathSplit[xpathSplit.Length - 1];
            var checkboxid = fieldId + "-lock";
            if (row > 0) checkboxid = fieldId + "-lock_" + row;

            var unlocked = "";
            var locked = "display:none;";
            var xpathlock = xpath + "-lock";

            if (info.GetXmlPropertyBool(xpathlock))
            {
                unlocked = "display:none;";
                locked = "";
            }
            var strOut = "";
            if (active)
            {
                strOut = "<img class='locktranslation locktranslation" + checkboxid + "' title='" + ResourceKeyString("DNNrocket.translation") + ": " + ResourceKeyString("DNNrocket.on") + "' onclick=\"$('#" + checkboxid + "').prop('checked', true);$('.locktranslation" + checkboxid + "').hide();$('.unlocktranslation" + checkboxid + "').show();\" src='/DesktopModules/DNNrocket/API/images/unlock.png' style='width:16px;cursor:pointer;" + unlocked + "' alt='unlock' />";
                strOut += "<img class='unlocktranslation unlocktranslation" + checkboxid + "' title='" + ResourceKeyString("DNNrocket.translation") + ": " + ResourceKeyString("DNNrocket.off") + "' onclick=\"$('#" + checkboxid + "').prop('checked', false);$('.locktranslation" + checkboxid + "').show();$('.unlocktranslation" + checkboxid + "').hide();\" src='/DesktopModules/DNNrocket/API/images/lock.png' style='width:16px;cursor:pointer;" + locked + "' alt='lock' />";
            }
            else
            {
                if (locked == "")
                {
                    strOut = "<img class='' title='" + ResourceKeyString("DNNrocket.translation") + ": " + ResourceKeyString("DNNrocket.off") + "' onclick=\"$('#" + checkboxid + "').prop('checked', false);$('.locktranslation" + checkboxid + "').show();$('.unlocktranslation" + checkboxid + "').hide();\" src='/DesktopModules/DNNrocket/API/images/lock.png' style='width:16px;' alt='lock' />";
                }
            }
            return new RawString(strOut + CheckBox(info, xpathlock, "", " class='translationlockcheckbox' style='display:none;'", false, false, row).ToString());
        }
        public IEncodedString Translate(SimplisityInfo info, string xpath, bool show = true, int row = 0, bool active = true)
        {
            if (!show) return new RawString("");
            if (!active) return new RawString("");

            var xpathSplit = xpath.Split('/');
            var fieldId = xpathSplit[xpathSplit.Length - 1];
            var itemlistref = "";
            if (row > 0)
            {
                fieldId = fieldId + "_" + row;
                itemlistref = info.GetXmlProperty("genxml/hidden/simplisity-listitemref");
            }
            var strOut = "<img class='translatefield translate' itemlistref='" + itemlistref + "' xpath='" + xpath + "' fieldid='" + fieldId + "' itemid='" + info.ItemID + "' title='" + ResourceKeyString("DNNrocket.translate") + "' src='/DesktopModules/DNNrocket/API/images/translate.png' style='width:16px;cursor:pointer;' alt='" + ResourceKeyString("DNNrocket.translate") + "' />";
            return new RawString(strOut);
        }
        public IEncodedString TranslationKeyUp(string fieldId, int row = 0, bool active = true)
        {
            if (!active) 
                return new RawString(" disabled ");
            else
            {
                var checkboxid = fieldId;
                if (row > 0) checkboxid = fieldId + "_" + row;
                var strOut = " onkeyup=\"$('#translationlocked" + checkboxid + "').prop('checked', true);$('.locktranslation" + checkboxid + "-lock').hide();$('.unlocktranslation" + checkboxid + "-lock').show();\" ";
                return new RawString(strOut);
            }
        }
        public IEncodedString EditFlag(SessionParams sessionParams, string classvalues = "")
        {
            var strOut = "<img class='" + classvalues + "' src='//" + PortalUtils.DomainSubUrl() + "/DesktopModules/DNNrocket/API/images/flags/16/" + sessionParams.CultureCodeEdit + ".png' alt='" + sessionParams.CultureCodeEdit + "' />";
            return new RawString(strOut);
        }
        [Obsolete("Use EditFlag(SessionParams sessionParams, string classvalues) instead")]
        public IEncodedString EditFlag(string classvalues = "")
        {
            var cultureCode = DNNrocketUtils.GetCurrentCulture();
            var strOut = "<img class='" + classvalues + "' src='/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png' alt='" + cultureCode + "' />";
            return new RawString(strOut);
        }
        public IEncodedString DisplayFlag(string cultureCode, string classvalues = "")
        {
            var flagRelPath = "/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png";
            if (File.Exists(DNNrocketUtils.MapPath(flagRelPath)))
            {
                var strOut = "<img class='" + classvalues + "' src='/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png' alt='" + cultureCode + "' />";
                return new RawString(strOut);
            }
            return new RawString("");
        }
        public IEncodedString DisplayEngineFlag(string engineUrl, string cultureCode, string classvalues = "")
        {
            var flagRelPath = "/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png";
            if (File.Exists(DNNrocketUtils.MapPath(flagRelPath)))
            {
                var strOut = "<img class='" + classvalues + "' src='" + engineUrl.TrimEnd('/') + "/DesktopModules/DNNrocket/API/images/flags/16/" + cultureCode + ".png' alt='" + cultureCode + "' />";
                return new RawString(strOut);
            }
            return new RawString("");
        }
        public IEncodedString ImageUrl(string url, string extraurlparams = "", string imgType = "")
        {
            return ImageUrl("", url, 0, 0, extraurlparams, imgType);
        }
        public IEncodedString ImageUrl(string url, int width = 0, int height = 0, string extraurlparams = "", string imgType = "")
        {
            return ImageUrl("", url, width, height, extraurlparams, imgType);
        }
        /// <summary>
        /// Display Thumb URL
        /// </summary>
        /// <param name="engineUrl"></param>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="extraurlparams"></param>
        /// <param name="imgType">Force Image Type or default to image type. "webp","png","jpg",""</param>
        /// <returns></returns>
        public IEncodedString ImageUrl(string engineUrl, string url, int width = 0, int height = 0, string extraurlparams = "", string imgType = "")
        {
            if (url == "") url = "/DesktopModules/DNNrocket/api/images/noimage2.png";
            if (!imgType.StartsWith("&")) imgType = "&imgtype=" + imgType;

            if (width > 0 || height > 0)
            {
                if (imgType == "" && url.ToLower().EndsWith(".png")) imgType = "&imgtype=png";
                if (imgType == "" && url.ToLower().EndsWith(".webp")) imgType = "&imgtype=webp";

                url = engineUrl.TrimEnd('/') + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + "&w=" + width + "&h=" + height + extraurlparams + imgType;
            }
            else
            {
                url = engineUrl.TrimEnd('/') + "/" + url + "&w=" + width + "&h=" + height + extraurlparams;
            }
            return new RawString(url);
        }
        [Obsolete("Method1 is deprecated, please use ImageUrl(...) instead.")]
        public IEncodedString ThumbnailImageUrl(string engineUrl, string url, int width = 0, int height = 0, string extraurlparams = "", bool pngImage = true)
        {
            var pngType = "";
            if (url == "") url = "/DesktopModules/DNNrocket/api/images/noimage2.png";
            if (pngImage && url.ToLower().EndsWith(".png")) pngType = "&imgtype=png";
            if (url.ToLower().EndsWith(".webp")) pngType = "&imgtype=webp";
            if (width > 0 || height > 0)
            {
                url = engineUrl.TrimEnd('/') + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + "&w=" + width + "&h=" + height + extraurlparams + pngType;
            }
            else
            {
                url = engineUrl.TrimEnd('/') + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + extraurlparams + pngType;
            }
            return new RawString(url);
        }
        [Obsolete("Method1 is deprecated, please use ImageUrl(...) instead.")]
        public IEncodedString ThumbnailImageUrl(string url, int width = 0, int height = 0, string extraurlparams = "", bool pngImage = true)
        {
            return ThumbnailImageUrl("", url, width, height, extraurlparams, pngImage);
        }
        [Obsolete("Method1 is deprecated, please use ImageUrl(...) instead.")]
        public IEncodedString ThumbnailImageWebsiteDomainUrl(string url, int width = 0, int height = 0, string extraurlparams = "", bool pngImage = true)
        {
            return ThumbnailImageWebsiteDomainUrl(PortalUtils.DefaultPortalAlias(), url, width, height, extraurlparams, pngImage);
        }

        [Obsolete("Method1 is deprecated, please use ImageUrl(...) instead.")]
        public IEncodedString ThumbnailImageWebsiteDomainUrl(string websiteDomainUrl, string url, int width = 0, int height = 0, string extraurlparams = "", bool pngImage = true)
        {
            var pngType = "";
            if (url == "") url = "/DesktopModules/DNNrocket/api/images/noimage2.png";
            if (pngImage && url.ToLower().EndsWith(".png")) pngType = "&imgtype=png";
            if (!websiteDomainUrl.StartsWith("http")) websiteDomainUrl = "//" + websiteDomainUrl;
            if (width > 0 || height > 0)
            {
                url = websiteDomainUrl + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + "&w=" + width + "&h=" + height + extraurlparams + pngType;
            }
            else
            {
                url = websiteDomainUrl + "/DesktopModules/DNNrocket/API/DNNrocketThumb.ashx?src=" + url + extraurlparams + pngType;
            }
            return new RawString(url);
        }

        public IEncodedString DownloadDocument(int ItemId, int row, string fieldId, string text = "", string attributes = "")
        {
            return DownloadDocument("", "", ItemId, row, fieldId, text, attributes);
        }
        public IEncodedString DownloadDocument(string tableName, string EngineUrlWithProtocol, int ItemId, int row, string fieldId, string text = "", string attributes = "", string listname = "")
        {
            if (text == "")
            {
                text = ResourceKey("DNNrocket.download").ToString();
            }
            var strOut = "<a " + attributes + " href='" + EngineUrlWithProtocol + "/DesktopModules/DNNrocket/api/downloadfile.ashx?fileindex=" + row + "&itemid=" + ItemId + "&fieldid=" + fieldId + "&tablename=" + tableName + "&listname=" + listname + "'>" + text + "</a>";
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
        /// <param name="scriptFileName">CKEditor script name or rel path + script name</param>
        /// <param name="localized"></param>
        /// <param name="row"></param>
        /// <param name="listname"></param>
        /// <param name="langauge"></param>
        /// <returns></returns>
        public IEncodedString CKEditor(SimplisityInfo info, string xpath, string scriptFileName = "scriptClassic.html",  bool localized = false, int row = 0, string listname = "", string langauge = "", bool coded = false)
        {
            if (langauge == "") langauge = DNNrocketUtils.GetCurrentLanguageCode();

            var filePath = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket/CKEditor/" + scriptFileName);
            if (scriptFileName.StartsWith("/")) filePath = DNNrocketUtils.MapPath(scriptFileName);
                
            var strOut = FileUtils.ReadFile(filePath);

            var id = getIdFromXpath(xpath, row, listname);

            strOut = strOut.Replace("{textareaid}", id);
            strOut = strOut.Replace("{language}", langauge);   

            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/"))
            {
                value = info.GetXmlProperty("genxml/lang/" + xpath);
            }
            var codedtext = "";
            if (coded) codedtext = " s-datatype='coded' ";
            strOut += " <textarea id='" + id + "' s-xpath='" + xpath + "' type='text' style='width:100%' " + codedtext + " rows='10'>" + value + "</textarea>";
            return new RawString(strOut);
        }
        public IEncodedString CKEditor4(SimplisityInfo info, string xpath, bool localized = false, int row = 0, string listname = "", string langauge = "", bool coded = false)
        {
            if (langauge == "") langauge = DNNrocketUtils.GetCurrentLanguageCode();
            var id = getIdFromXpath(xpath, row, listname);
            var value = info.GetXmlProperty(xpath);
            if (localized && !xpath.StartsWith("genxml/lang/")) value = info.GetXmlProperty("genxml/lang/" + xpath);
            var codedtext = "";
            if (coded) codedtext = " s-datatype='coded' ";
            var strOut = " <textarea id='" + id + "' s-xpath='" + xpath + "' type='text' style='width:100%' " + codedtext + " rows='10'>" + value + "</textarea>";
            strOut += "<script>";
            strOut += "$(document).ready(function () {";
            strOut += " CKEDITOR.replace('" + id + "');";
            strOut += " CKEDITOR.config.allowedContent = true;";
            strOut += " CKEDITOR.instances." + id + ".on('key', function () {";
            strOut += " $('#" + id + "').val(CKEDITOR.instances." + id + ".getData());";
            strOut += "";
            strOut += " });";
            strOut += " CKEDITOR.instances." + id + ".on('change', function () {";
            strOut += " $('#" + id + "').val(CKEDITOR.instances." + id + ".getData());";
            strOut += "";
            strOut += " });";
            strOut += "});";
            strOut += "</script>";
            return new RawString(strOut);
        }

        public IEncodedString EditorQuill(SimplisityInfo info, string xpath, string attributes, string quillconfig = "", bool localized = false, int row = 0, string listname = "", string langauge = "")
        {
            if (langauge == "") langauge = DNNrocketUtils.GetCurrentLanguageCode();

            var id = getIdFromXpath(xpath, row, listname);

            var scriptQuill = "";
            if (quillconfig == "")
            {
                var filePath = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket/Quill/quillconfig.html");
                scriptQuill = FileUtils.ReadFile(filePath);
            }
            else
            {
                scriptQuill = quillconfig;
            }
            scriptQuill = scriptQuill.Replace("{elementid}", id);
            scriptQuill = scriptQuill.Replace("{language}", langauge);

            if (localized && !xpath.StartsWith("genxml/lang/")) xpath = info.GetXmlProperty("genxml/lang/" + xpath);
            var value = info.GetXmlProperty(xpath);
            var strOut = " <div id='" + id + "quill' " + attributes + " >" + value + "</div>";

            var textarea = TextArea(info, xpath, "style='display:none;'", "",localized, row, listname);

            return new RawString(strOut + " " + textarea + " " + scriptQuill);
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
            string[] paramData = new string[0];

            var tabid = info.GetXmlPropertyInt(xpath);
            if (tabid == 0) return new RawString("");
            var url = PagesUtils.NavigateURL(tabid, "", paramData);

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

        public IEncodedString RenderHandleBars(SimplisityInfo info, AppThemeLimpet appTheme, string templateName, string moduleref = "", string cacheKey = "")
        {
            var dataObjects = new Dictionary<string, SimplisityInfo>();
            dataObjects.Add("data", info);
            return RenderHandleBars(dataObjects, appTheme, templateName, moduleref, cacheKey);
        }

        public IEncodedString RenderHandleBars(Dictionary<string, SimplisityInfo> dataObjects, AppThemeLimpet appTheme, string templateName, string moduleref = "", string cacheKey = "")
        {
            var strOut = "";
            if (cacheKey != "") strOut = (string)CacheUtils.GetCache(moduleref + cacheKey, "hbs");
            if (String.IsNullOrEmpty(strOut))
            {
                string jsonString = SimplisityUtils.ConvertToJson(dataObjects);
                var template = appTheme.GetTemplate(templateName, moduleref);
                JObject model = JObject.Parse(jsonString);
                HandlebarsEngine hbEngine = new HandlebarsEngine();
                strOut = hbEngine.Execute(template, model);
                if (cacheKey != "") CacheUtils.SetCache(moduleref + cacheKey, strOut, "hbs");
            }
            return new RawString(strOut);
        }
        public IEncodedString ModSelectList(SimplisityInfo info, String xpath, int portalId, string systemKey, String attributes = "", bool addEmpty = true )
        {
            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();
            var modList = DNNrocketUtils.GetModList(portalId, systemKey);

            if (info == null) info = new SimplisityInfo();
            var strOut = "";
            var value = info.GetXmlProperty(xpath);

            var upd = getUpdateAttr(xpath, attributes, false);
            var id = getIdFromXpath(xpath, 0, "");
            strOut = "<select id='" + id + "' s-xpath='" + xpath + "' " + upd + " " + attributes + ">";
            var c = 0;
            var s = "";
            if (addEmpty)
            {
                strOut += "    <option value=''></option>";
            }
            foreach (var mData in modList)
            {
                if (value == mData.ModuleRef)
                    s = "selected";
                else
                    s = "";
                var TabName = PagesUtils.GetPageName(mData.Record.GetXmlPropertyInt("genxml/data/tabid"), portalId);
                var moduleName = TabName + ":&nbsp;" + mData.Name + "&nbsp;[" + mData.ModuleId + "]";
                strOut += "    <option value='" + mData.ModuleRef + "' " + s + ">" + moduleName + "</span></option>";
                c += 1;
            }
            strOut += "</select>";

            return new RawString(strOut);
        }



    }


}
