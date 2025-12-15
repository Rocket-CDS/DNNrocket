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
using DotNetNuke.UI.Skins;
using DotNetNuke.Entities.Portals;
using Simplisity;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Framework;
using DNNrocketAPI.Components;
using System.Web.UI;

namespace RocketTools
{

    public partial class Meta : SkinObjectBase
    {
        private int _portalId;
        private string _cultureCode;
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
                var urlParams = new Dictionary<string, string>();
                // get all query string params
                foreach (string key in Request.QueryString.AllKeys)
                {
                    if (key != null)
                    {
                        var keyValue = Request.QueryString[key];
                        urlParams.Add(key.ToLower(), keyValue);
                    }
                }

                var metaPageData = PagesUtils.GetMetaData(PortalSettings.ActiveTab.TabID, Request.Url, urlParams);


                CDefault page = (CDefault)this.Page;
                if (!String.IsNullOrEmpty(metaPageData.AlternateLinkHtml) && !_disablealternate) page.Header.Controls.Add(new LiteralControl(metaPageData.AlternateLinkHtml));

                page.CanonicalLinkUrl = ""; // remove so we dont; display anything from invalid module values.

                if (!String.IsNullOrEmpty(metaPageData.Title)) page.Title = metaPageData.Title;
                if (!String.IsNullOrEmpty(metaPageData.Description)) page.Description = metaPageData.Description;
                if (!String.IsNullOrEmpty(metaPageData.Description)) page.MetaDescription = metaPageData.Description;
                if (!String.IsNullOrEmpty(metaPageData.Description)) page.Header.Description = metaPageData.Description;
                if (!String.IsNullOrEmpty(metaPageData.KeyWords)) page.KeyWords = metaPageData.KeyWords;
                if (!String.IsNullOrEmpty(metaPageData.KeyWords)) page.MetaKeywords = metaPageData.KeyWords;
                if (!String.IsNullOrEmpty(metaPageData.KeyWords)) page.Header.Keywords = metaPageData.KeyWords;
                if (!String.IsNullOrEmpty(metaPageData.CanonicalLinkUrl) && !_disablecanonical) page.CanonicalLinkUrl = metaPageData.CanonicalLinkUrl;

                foreach (var metaDict in metaPageData.HtmlMeta)
                {
                    var meta = PagesUtils.BuildMeta("", metaDict.Key, metaDict.Value);
                    page.Header.Controls.Add(meta);
                }

                foreach (var sPattern in metaPageData.CssRemovalPattern)
                {
                    if (sPattern != "" && !UserUtils.IsAdministrator()) PageIncludes.RemoveCssFile(this.Page, sPattern);
                }

                if (metaPageData.Redirect404) DNNrocketUtils.Handle404Exception(this.Response, PortalSettings.Current);

            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

        }
    }

}
