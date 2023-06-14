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
        protected override void OnPreRender(EventArgs evt)
        {
            base.OnPreRender(evt);
            try
            {
                _portalId = PortalUtils.GetCurrentPortalId();
                _cultureCode = DNNrocketUtils.GetCurrentCulture();

                DotNetNuke.Framework.CDefault tp = (DotNetNuke.Framework.CDefault)this.Page;
                var objCtrl = new DNNrocketController();
                var disablealternate = false;
                var disablecanonical = false;
                var dataRecord = objCtrl.GetRecordByGuidKey(PortalSettings.Current.PortalId, -1, "PL", "PL_" + _cultureCode + "_" + PortalSettings.ActiveTab.TabID.ToString(""));
                if (dataRecord != null)
                {
                    if (dataRecord.GetXmlProperty("genxml/textbox/pagetitle") != "")
                    {
                        tp.Title = dataRecord.GetXmlProperty("genxml/textbox/pagetitle");
                    }
                    if (dataRecord.GetXmlProperty("genxml/textbox/pagedescription") != "")
                    {
                        tp.MetaDescription = dataRecord.GetXmlProperty("genxml/textbox/pagedescription");
                        tp.Header.Description = tp.MetaDescription;
                    }
                    if (dataRecord.GetXmlProperty("genxml/textbox/tagwords") != "")
                    {
                        tp.MetaKeywords = dataRecord.GetXmlProperty("genxml/textbox/tagwords");
                        tp.Header.Keywords = tp.MetaKeywords;
                    }
                    disablealternate = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablealternate");
                    disablecanonical = dataRecord.GetXmlPropertyBool("genxml/checkbox/disablecanonical");
                }
                var settingRecord = (SimplisityRecord)CacheUtils.GetCache("PLSETTINGS" + _portalId, _portalId.ToString());
                if (settingRecord == null)
                {
                    settingRecord = objCtrl.GetRecordByGuidKey(_portalId, -1, "PLSETTINGS", "PLSETTINGS");
                    CacheUtils.SetCache("PLSETTINGS" + _portalId, settingRecord, _portalId.ToString());
                }

                // ********** Add alt url meta for langauges ***********
                var cachekey = "RocketTools*hreflang*" + PortalSettings.Current.PortalId + "*" + _cultureCode + "*" + PortalSettings.ActiveTab.TabID; // use nodeTablist incase the DDRMenu has a selector.
                var canonicalurl = "";
                var hreflangtext = "";
                var hreflangobj = CacheUtils.GetCache(cachekey);
                if (hreflangobj != null) hreflangtext = hreflangobj.ToString();
                if (hreflangtext == "" || true)
                {
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
                    CacheUtils.SetCache(cachekey, hreflangtext);
                }
                if (!String.IsNullOrEmpty(hreflangtext) && !disablealternate) tp.Header.Controls.Add(new LiteralControl(hreflangtext));
                if (!String.IsNullOrEmpty(canonicalurl) && !disablecanonical) tp.CanonicalLinkUrl = canonicalurl;
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

        }

    }

}
