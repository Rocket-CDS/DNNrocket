// --- Copyright (c) notice NevoWeb ---
//  Copyright (c) 2014 SARL NevoWeb.  www.nevoweb.com. The MIT License (MIT).
// Author: D.C.Lee
// ------------------------------------------------------------------------
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// ------------------------------------------------------------------------
// This copyright notice may NOT be removed, obscured or modified without written consent from the author.
// --- End copyright notice --- 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.UI.Skins;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.Data;
using DotNetNuke.Common.Utilities;
using Simplisity;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Framework;
using DNNrocketAPI.Components;
using System.Security.Cryptography;
using DNNrocketAPI.ApiControllers;
using DNNrocketAPI;
using System.Web.UI;
using System.Xml.Linq;
using System.Security.Cryptography.Xml;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Policy;
using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;

namespace RocketTools
{

    public partial class Meta : SkinObjectBase
    {
        private int _portalId;
        private string _cultureCode;
        private string _metatitle = "";
        private string _metadescription = "";
        private string _metatagwords = "";
        private string _articleTable = "";
        private int _articleDefaultTabId = 0;
        private int _articleListTabId = 0;        
        private bool _disablealternate = false;
        private bool _disablecanonical = false;
        private SimplisityInfo _dataRecordTemp;
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                var is404 = false;
                _portalId = PortalUtils.GetCurrentPortalId();
                _cultureCode = DNNrocketUtils.GetCurrentCulture();

                // Read Data
                var objCtrl = new DNNrocketController();
                var cacheKey2 = "PLSETTINGS" + _portalId;
                var plRecord = (SimplisityRecord)CacheUtils.GetCache(cacheKey2, _portalId.ToString());
                if (plRecord == null)
                {
                    plRecord = objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
                    CacheUtils.SetCache(cacheKey2, plRecord, _portalId.ToString());
                }
                var cacheKey = "PL_" + _cultureCode + "_" + PortalSettings.ActiveTab.TabID.ToString("");
                var dataRecord = (SimplisityRecord)CacheUtils.GetCache(cacheKey, _portalId.ToString());
                if (dataRecord == null)
                {
                    dataRecord = objCtrl.GetRecordByGuidKey(_portalId, -1, "PL", cacheKey);
                    CacheUtils.SetCache(cacheKey, dataRecord, _portalId.ToString());
                }

                // check for paramid
                var queryKeyList = GetUrlCategoryQueryKeyList(_portalId);
                var appendList = new Dictionary<string, QueryParamsData>();
                var paramidList = DNNrocketUtils.GetQueryKeys(_portalId);
                var hasCategoryParam = false;
                if (paramidList.Count > 1)
                {
                    foreach (var qk in queryKeyList)
                    {
                        if (paramidList.ContainsKey(qk))
                        {
                            hasCategoryParam = true;
                            break;
                        }
                    }
                }
                if (hasCategoryParam)
                {
                    // move the catid to the last param, so it's only taken if only the catid is in the URL.
                    var paramidList2 = new Dictionary<string, QueryParamsData>();
                    foreach (var p in paramidList)
                    {
                        if (!queryKeyList.Contains(p.Key))
                            paramidList2.Add(p.Key, p.Value);
                        else
                            appendList.Add(p.Key, p.Value);
                    }
                    foreach (var qk in appendList)
                    {
                        paramidList2.Add(qk.Value.queryparam , qk.Value);
                    }
                    paramidList = paramidList2;
                }

                var articleMeta = false;
                var metaList = new List<HtmlMeta>();
                var articleid = 0;
                var articleParamKey = "";
                var foundArticle = false;
                var catMeta = false;
                foreach (var paramDict in paramidList)
                {
                    var paramKey = Request.QueryString[paramDict.Key];
                    if (paramKey != null)
                    {
                        var strParam = Request.QueryString[paramDict.Key];
                        if (GeneralUtils.IsNumeric(strParam))
                        {
                            if (!foundArticle) // we can have only 1 article in the SEO, take the first found.  (catid is moved to last position inthe list.)
                            {

                                _articleTable = paramDict.Value.tablename;
                                try
                                {
                                    articleid = Convert.ToInt32(strParam);
                                }
                                catch (Exception ex)
                                {
                                    Exceptions.ProcessModuleLoadException(this, ex);
                                    DNNrocketUtils.Handle404Exception(this.Response, PortalSettings.Current);
                                }
                                _dataRecordTemp = objCtrl.GetInfo(articleid, DNNrocketUtils.GetCurrentCulture(), _articleTable);
                                if (_dataRecordTemp != null)
                                {
                                    foundArticle = true;

                                    _metatitle = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle");
                                    if (_metatitle == "") _metatitle = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/name");
                                    if (_metatitle == "") _metatitle = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlename");

                                    _metadescription = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seodescription");
                                    if (_metadescription == "") _metadescription = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/summary");
                                    if (_metadescription == "") _metadescription = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlesummary");

                                    _metatagwords = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seokeyword");

                                    var portalContentRec = DNNrocketUtils.GetPortalContentRecByRefId(_dataRecordTemp.PortalId, paramDict.Value.systemkey, _articleTable);
                                    if (portalContentRec == null) portalContentRec = new SimplisityRecord();
                                    _articleDefaultTabId = portalContentRec.GetXmlPropertyInt("genxml/detailpage");
                                    if (_articleDefaultTabId == 0) _articleDefaultTabId = PortalSettings.ActiveTab.TabID;
                                    _articleListTabId = portalContentRec.GetXmlPropertyInt("genxml/listpage");
                                    if (_articleListTabId == 0) _articleListTabId = PortalSettings.ActiveTab.TabID;

                                    articleParamKey = paramDict.Key;
                                    if (!queryKeyList.Contains(articleParamKey))
                                    {
                                        string[] urlparams = { articleParamKey, articleid.ToString(), DNNrocketUtils.UrlFriendly(_metatitle) };
                                        var ogurl = DNNrocketUtils.NavigateURL(_articleDefaultTabId, _dataRecordTemp.Lang, urlparams);

                                        metaList.Add(BuildMeta("", "og:type", "article"));
                                        metaList.Add(BuildMeta("", "og:title", _metatitle.Truncate(150).Replace("\"", "")));
                                        metaList.Add(BuildMeta("", "og:description", _metadescription.Truncate(250).Replace("\"", "")));
                                        metaList.Add(BuildMeta("", "og:url", ogurl));
                                        var imgRelPath = _dataRecordTemp.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/imagepatharticleimage").ToString();
                                        if (imgRelPath != "") imgRelPath = _dataRecordTemp.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/imagepathproductimage").ToString();
                                        if (imgRelPath != "") metaList.Add(BuildMeta("", "og:image", Request.Url.GetLeftPart(UriPartial.Authority).TrimEnd('/') + "/" + imgRelPath.TrimStart('/')));

                                        articleMeta = true;
                                    }
                                    else
                                    {
                                        catMeta = true;
                                    }

                                    // if the systemkey for the articleid do not match we throw a 404. (PRD and CAT for RocketEcomemrce)
                                    if (!_dataRecordTemp.TypeCode.StartsWith(paramDict.Value.systemkey) && (_dataRecordTemp.TypeCode != "PRD" && _dataRecordTemp.TypeCode != "CAT")) is404 = true;
                                    break;
                                }
                                else
                                {
                                    // record does not exist, throw a 404.
                                    is404 = true;
                                }
                            }
                        }
                    }
                }


                if (dataRecord != null)
                {
                    if (!foundArticle) // Use PL data if no article or cat title found.
                    {
                        _metatitle = dataRecord.GetXmlProperty("genxml/textbox/pagetitle");
                        _metadescription = dataRecord.GetXmlProperty("genxml/textbox/pagedescription");
                        _metatagwords = dataRecord.GetXmlProperty("genxml/textbox/tagwords");
                    }

                    _disablealternate = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablealternate");
                    _disablecanonical = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablecanonical");

                }

                CDefault tp = (CDefault)this.Page;

                // ********** Add alt url meta for langauges ***********
                var cachekey = "RocketTools*hreflang*" + PortalSettings.Current.PortalId + "*" + _cultureCode + "*" + PortalSettings.ActiveTab.TabID + "*" + articleid; // use nodeTablist incase the DDRMenu has a selector.                
                var hreflangobj = CacheUtils.GetCache(cachekey, _portalId.ToString());
                var canonicalurl = (string)CacheUtils.GetCache(cachekey + "2", _portalId.ToString());
                if (canonicalurl == null) canonicalurl = "";
                var hreflangtext = "";
                if (hreflangobj != null) hreflangtext = hreflangobj.ToString();
                if (hreflangobj == null)
                {
                    hreflangtext = "";  // clear so we don't produce multiple hreflang with cache.
                    var enabledlanguages = LocaleController.Instance.GetLocales(PortalSettings.Current.PortalId);
                    foreach (var l in enabledlanguages)
                    {
                        if ((catMeta || articleMeta) && articleParamKey != "")
                        {
                            var seotitle = "";
                            var dataRecordTemp = objCtrl.GetInfo(articleid, l.Key, _articleTable);
                            if (dataRecordTemp != null)
                            {
                                seotitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle");
                                if (seotitle == "") seotitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/name");
                                if (seotitle == "") seotitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlename");
                            }
                            seotitle = DNNrocketUtils.UrlFriendly(seotitle);

                            string[] urlparams = { articleParamKey, articleid.ToString(), seotitle };
                            if (catMeta)
                                hreflangtext += "<link rel='alternate' href='" + DNNrocketUtils.NavigateURL(_articleListTabId, "", l.Key, urlparams) + "' hreflang='" + l.Key.ToLower() + "'/>";
                            else
                                hreflangtext += "<link rel='alternate' href='" + DNNrocketUtils.NavigateURL(_articleDefaultTabId, "", l.Key, urlparams) + "' hreflang='" + l.Key.ToLower() + "'/>";

                            if (_cultureCode == l.Key)
                            {
                                if (catMeta)
                                    canonicalurl = DNNrocketUtils.NavigateURL(_articleListTabId, "", urlparams);
                                else
                                    canonicalurl = DNNrocketUtils.NavigateURL(_articleDefaultTabId, "", urlparams);
                            }
                        }
                        else
                        {
                            hreflangtext += "<link rel='alternate' href='" + DNNrocketUtils.NavigateURL(_articleListTabId, "", l.Key, null) + "' hreflang='" + l.Key.ToLower() + "'/>";
                            if (_cultureCode == l.Key) canonicalurl = DNNrocketUtils.NavigateURL(_articleListTabId);
                        }
                    }
                    CacheUtils.SetCache(cachekey, hreflangtext, _portalId.ToString());
                    CacheUtils.SetCache(cachekey + "2", canonicalurl, _portalId.ToString());
                }
                if (!String.IsNullOrEmpty(hreflangtext) && !_disablealternate) tp.Header.Controls.Add(new LiteralControl(hreflangtext));

                tp.CanonicalLinkUrl = ""; // remove so we dont; display anything from invalid module values.

                if (_metatitle != "") tp.Title = _metatitle;
                if (_metadescription != "") tp.Description = _metadescription;
                if (_metadescription != "") tp.MetaDescription = _metadescription;
                if (_metadescription != "") tp.Header.Description = _metadescription;
                if (_metatagwords != "") tp.KeyWords = _metatagwords;
                if (_metatagwords != "") tp.MetaKeywords = _metatagwords;
                if (_metatagwords != "") tp.Header.Keywords = _metatagwords;
                if (!String.IsNullOrEmpty(canonicalurl) && !_disablecanonical) tp.CanonicalLinkUrl = canonicalurl;

                foreach (var meta in metaList)
                {
                    tp.Header.Controls.Add(meta);
                }

                if (plRecord != null)
                {
                    foreach (var cssPattern in plRecord.GetRecordList("removecss"))
                    {
                        var sPattern = cssPattern.GetXmlProperty("genxml/textbox/removecss");
                        if (sPattern != "" && !UserUtils.IsAdministrator()) PageIncludes.RemoveCssFile(this.Page, sPattern);
                    }
                }

                if (is404) DNNrocketUtils.Handle404Exception(this.Response, PortalSettings.Current);

            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

        }

        private HtmlMeta BuildMeta(string name, string property, string content)
        {
            HtmlMeta meta = new HtmlMeta();
            meta.Name = name;
            if (!String.IsNullOrEmpty(property)) meta.Attributes.Add("property", property);
            meta.Content = content;
            return meta;
        }
        private List<string> GetUrlCategoryQueryKeyList(int portalId)
        {
            var cacheKey = "GetUrlCategoryQueryKeyList*" + portalId + "*category";
            var paramKey = (List<string>)CacheUtils.GetCache(cacheKey, "portalid" + portalId);
            if (paramKey == null)
            {
                paramKey = new List<string>(); 
                var paramidList = DNNrocketUtils.GetQueryKeys(portalId);
                foreach (var paramDict in paramidList)
                {
                    if (paramDict.Value.datatype == "category")
                    {
                        paramKey.Add(paramDict.Value.queryparam);
                    }
                }
                CacheUtils.SetCache(cacheKey, paramKey, "portalid" + portalId);
            }
            return paramKey;
        }

    }

}
