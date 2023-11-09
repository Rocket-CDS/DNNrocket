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

namespace RocketTools
{

    public partial class Meta : SkinObjectBase
    {
        private int _portalId;
        private string _cultureCode;
        private string _metatitle = "";
        private string _metadescription = "";
        private string _metatagwords = "";
        private bool _disablealternate = false;
        private bool _disablecanonical = false;

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                _portalId = PortalUtils.GetCurrentPortalId();
                _cultureCode = DNNrocketUtils.GetCurrentCulture();
                var objCtrl = new DNNrocketController();

                var cacheKey2 = "PLSETTINGS" + _portalId;
                var plRecord = (SimplisityRecord)CacheUtils.GetCache(cacheKey2, _portalId.ToString());
                if (plRecord == null) plRecord = objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");

                var cacheKeyQueryparams = "PLSETTINGSqueryparams" + _portalId;
                var paramidList = (Dictionary<string, string>)CacheUtils.GetCache(cacheKeyQueryparams, _portalId.ToString());
                if (paramidList == null)
                {
                    paramidList = new Dictionary<string, string>();
                    foreach (SimplisityRecord mp in plRecord.GetRecordList("queryparams"))
                    {
                        paramidList.Add(mp.GetXmlProperty("genxml/textbox/queryparam"), mp.GetXmlProperty("genxml/select/tablename"));
                    }
                    CacheUtils.SetCache(cacheKeyQueryparams, paramidList, _portalId.ToString());
                }


                var cacheKey = "PL_" + _cultureCode + "_" + PortalSettings.ActiveTab.TabID.ToString("");
                var dataRecord = (SimplisityRecord)CacheUtils.GetCache(cacheKey, _portalId.ToString());
                if (dataRecord == null)
                {
                    dataRecord = objCtrl.GetRecordByGuidKey(PortalSettings.Current.PortalId, -1, "PL", cacheKey);
                }
                // check for paramid
                var articleMeta = false;
                foreach (var paramDict in paramidList)
                {
                    var paramKey = Request.QueryString[paramDict.Key];
                    if (paramKey != null)
                    {
                        var strParam = Request.QueryString[paramDict.Key];
                        if (GeneralUtils.IsNumeric(strParam))
                        {
                            var dataRecordTemp = objCtrl.GetInfo(Convert.ToInt32(strParam), DNNrocketUtils.GetCurrentCulture(), paramDict.Value);
                            if (dataRecordTemp != null)
                            {
                                if (dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle") != "")
                                    _metatitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle");
                                if (_metatitle == "") _metatitle = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlename");

                                if (dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seodescription") != "")
                                    _metadescription = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seodescription");
                                if (_metadescription == "") _metadescription = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlesummary");

                                if (dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seokeyword") != "")
                                    _metatagwords = dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seokeyword");

                                articleMeta = true;
                            }
                        }
                    }
                }


                if (dataRecord != null)
                {
                    if (!articleMeta)
                    {
                        if (dataRecord.GetXmlProperty("genxml/textbox/pagetitle") != "")
                            _metatitle = dataRecord.GetXmlProperty("genxml/textbox/pagetitle");
                        if (dataRecord.GetXmlProperty("genxml/textbox/pagedescription") != "")
                            _metadescription = dataRecord.GetXmlProperty("genxml/textbox/pagedescription");
                        if (dataRecord.GetXmlProperty("genxml/textbox/tagwords") != "")
                            _metatagwords = dataRecord.GetXmlProperty("genxml/textbox/tagwords");
                    }

                    _disablealternate = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablealternate");
                    _disablecanonical = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablecanonical");

                    CacheUtils.SetCache(cacheKey, dataRecord, _portalId.ToString());
                }

                CDefault tp = (CDefault)this.Page;
                if (_metatitle != "") tp.Title = _metatitle;
                if (_metadescription != "") tp.Description = _metadescription;
                if (_metadescription != "") tp.MetaDescription = _metadescription;
                if (_metadescription != "") tp.Header.Description = _metadescription;
                if (_metatagwords != "") tp.KeyWords = _metatagwords;
                if (_metatagwords != "") tp.MetaKeywords = _metatagwords;
                if (_metatagwords != "") tp.Header.Keywords = _metatagwords;

                if (plRecord != null)
                {
                    foreach (var cssPattern in plRecord.GetRecordList("removecss"))
                    {
                        var sPattern = cssPattern.GetXmlProperty("genxml/textbox/removecss");
                        if (sPattern != "" && !UserUtils.IsAdministrator()) PageIncludes.RemoveCssFile(this.Page, sPattern);
                    }
                }

            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

        }

        protected override void OnPreRender(EventArgs evt)
        {
            base.OnPreRender(evt);
            try
            {
                _portalId = PortalUtils.GetCurrentPortalId();
                _cultureCode = DNNrocketUtils.GetCurrentCulture();

                DotNetNuke.Framework.CDefault tp = (DotNetNuke.Framework.CDefault)this.Page;

                // ********** Add alt url meta for langauges ***********
                var cachekey = "RocketTools*hreflang*" + PortalSettings.Current.PortalId + "*" + _cultureCode + "*" + PortalSettings.ActiveTab.TabID; // use nodeTablist incase the DDRMenu has a selector.
                var canonicalurl = (string)CacheUtils.GetCache(cachekey + "2", _portalId.ToString()); ;
                var hreflangobj = CacheUtils.GetCache(cachekey, _portalId.ToString());

                if (canonicalurl == null) canonicalurl = "";
                var hreflangtext = "";
                if (hreflangobj != null) hreflangtext = hreflangobj.ToString();
                if (hreflangobj == null)
                {
                    var objCtrl = new DNNrocketController();
                    hreflangtext = "";  // clear so we don't produce multiple hreflang with cache.
                    var enabledlanguages = LocaleController.Instance.GetLocales(PortalSettings.Current.PortalId);
                    foreach (var l in enabledlanguages)
                    {
                        var paDataRecord = objCtrl.GetRecordByGuidKey(PortalSettings.Current.PortalId, -1, "PL", "PL_" + l.Key + "_" + PortalSettings.ActiveTab.TabID.ToString(""));
                        if (paDataRecord != null)
                        {
                            var portalalias = PortalUtils.DefaultPortalAlias(_portalId, l.Key);
                            var pagename = paDataRecord.GetXmlProperty("genxml/textbox/pageurl").TrimEnd('/');
                            hreflangtext += "<link rel='alternate' href='" + Server.HtmlEncode(Request.Url.Scheme) + "://" + portalalias + pagename + "' hreflang='" + l.Key.ToLower() + "'/>";
                            if (_cultureCode == l.Key)
                            {
                                canonicalurl = Server.HtmlEncode(Request.Url.Scheme) + "://" + portalalias + pagename;
                                //canonicalurltext = "<link rel='canonical' href='" + "/" + portalalias + pagename + "'/>";
                            }
                        }
                    }
                    CacheUtils.SetCache(cachekey, hreflangtext, _portalId.ToString());
                    CacheUtils.SetCache(cachekey + "2", canonicalurl, _portalId.ToString());
                }
                if (!String.IsNullOrEmpty(hreflangtext) && !_disablealternate) tp.Header.Controls.Add(new LiteralControl(hreflangtext));
                if (!String.IsNullOrEmpty(canonicalurl) && !_disablecanonical) tp.CanonicalLinkUrl = canonicalurl;
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

        }

    }

}
