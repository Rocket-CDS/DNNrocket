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
using DotNetNuke.Entities.Tabs;
using DotNetNuke.UI.Skins;
using DotNetNuke.Entities.Portals;
using System.Web.UI;
using DotNetNuke.Services.Localization;
using DotNetNuke.Common.Utilities;
using Simplisity;
using DNNrocketAPI;
using DataProvider = DotNetNuke.Data.DataProvider;
using DotNetNuke.Framework;

namespace DNNrocket.DNNrocketAPI
{

    public partial class Meta : SkinObjectBase 
    {
        private DNNrocketController _objCtrl;
        private static SimplisityInfo _dataRecord;
        private static TabInfo _dnnTab;
        private bool _debugmode;

        protected override void OnLoad(EventArgs e)
        {
            try
            {

                _objCtrl = new DNNrocketController();
                var objTabCtrl = new TabController();

                _dnnTab = objTabCtrl.GetTab(PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.PortalId);



                var guidKey = "tabid" + PortalSettings.Current.ActiveTab.TabID;
                _dataRecord = _objCtrl.GetData(guidKey, "ROCKETPL", DNNrocketUtils.GetCurrentCulture());
                var systemInfo = _objCtrl.GetInfo(_dataRecord.SystemId);
                _debugmode = false;
                if (systemInfo != null) _debugmode = systemInfo.GetXmlPropertyBool("genxml/checkbox/debugmode");


                if (_dataRecord != null)
                {
                    CDefault tp = (CDefault)this.Page;

                    if (_dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle") != "")
                    tp.Title = _dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle");

                    tp.KeyWords = _dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/tagwords");
                    if (_dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/description") != "")
                        tp.Description = _dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/description");

                    var hreflangtext = AlternateUrls(_debugmode);
                    tp.Header.Controls.Add(new LiteralControl(hreflangtext));
                    tp.CanonicalLinkUrl = CanonicalUrls(_debugmode);

                }
            }
            catch (Exception exc)
            {
                //ignore
            }
        }

        public static string AlternateUrls(bool _debugmode)
        {
            var cachekey = "rocketPL*hreflang*" + PortalSettings.Current.PortalId + "*" + DNNrocketUtils.GetCurrentCulture() + "*" + PortalSettings.Current.ActiveTab.TabID; // use nodeTablist incase the DDRMenu has a selector.
            var pagename = "";
            var canonicalurl = "";
            var hreflangtext = "";
            var hreflangobj = CacheUtils.GetCache(cachekey);
            if (hreflangobj != null) hreflangtext = hreflangobj.ToString();
            if (hreflangtext == "" || _debugmode)
            {
                hreflangtext = "";  // clear so we don't produce multiple hreflang with cache.
                var enabledlanguages = LocaleController.Instance.GetLocales(PortalSettings.Current.PortalId);


                var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DataProvider.Instance().GetPortalAliases());

                foreach (var l in enabledlanguages)
                {
                    var urldata = "";
                    if (_dataRecord != null)
                    {
                        var defaultPageName = _dnnTab.TabName;
                        var portalalias = DNNrocketUtils.GetPortalAlias(l.Key);
                        pagename = _dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagename");
                        if (pagename == "") pagename = defaultPageName;
                        urldata = "//" + portalalias + "/" + GeneralUtils.UrlFriendly(pagename);
                    }

                    hreflangtext += "<link rel='alternative' href='" + urldata + "' hreflang='" + l.Key.ToLower() + "' />";
                    if (DNNrocketUtils.GetCurrentCulture() == l.Key)
                    {
                        canonicalurl = urldata;
                    }


                }
                CacheUtils.SetCache(cachekey, hreflangtext);
            }

            return hreflangtext;

        }

        public static string CanonicalUrls(bool _debugmode)
        {
            var cachekey = "rocketPL*canonicalurl*" + PortalSettings.Current.PortalId + "*" + DNNrocketUtils.GetCurrentCulture() + "*" + PortalSettings.Current.ActiveTab.TabID; // use nodeTablist incase the DDRMenu has a selector.
            var pagename = "";
            var canonicalurl = (string)CacheUtils.GetCache(cachekey);
            if (canonicalurl == "" || _debugmode)
            {
                canonicalurl = "";
                var enabledlanguages = LocaleController.Instance.GetLocales(PortalSettings.Current.PortalId);

                foreach (var l in enabledlanguages)
                {
                    if (DNNrocketUtils.GetCurrentCulture() == l.Key)
                    {
                        var urldata = "";
                        if (_dataRecord != null)
                        {
                            var defaultPageName = _dnnTab.TabName;
                            var portalalias = DNNrocketUtils.GetPortalAlias(l.Key);
                            pagename = _dataRecord.GetXmlProperty("genxml/lang/genxml/textbox/pagename");
                            if (pagename == "") pagename = defaultPageName;
                            urldata = "//" + portalalias + "/" + GeneralUtils.UrlFriendly(pagename);
                        }
                        canonicalurl = urldata;
                    }
                }
                CacheUtils.SetCache(cachekey, canonicalurl);
            }

            return canonicalurl;

        }

        


    }


}
