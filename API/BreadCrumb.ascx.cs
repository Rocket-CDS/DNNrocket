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
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.Entities.Portals;
using Simplisity;
using DNNrocketAPI;
using DNNrocketAPI.Components;

namespace RocketTools
{

    public partial class BreadCrumb : SkinObjectBase
    {

        // private members

        protected Literal lBreadCrumb;

        #region "Public Members"

        public string Separator { get; set; }
        public string CssClass { get; set; }
        public string RootLevel { get; set; }
        public Boolean HtmlList { get; set; }
        public Boolean HideWithNoBreadCrumb { get; set; }


        #endregion

        protected override void OnLoad(EventArgs e)
        {

            if (!Page.IsPostBack)
            {

                var objCtrl = new DNNrocketController();

                // public attributes
                var strSeparator = "";
                if (!String.IsNullOrEmpty(Separator))
                {
                    if (Separator.Contains("src="))
                    {
                        Separator = Separator.Replace("src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath);
                    }
                    strSeparator = Separator;
                }
                else
                {
                    if (!HtmlList) strSeparator = "&nbsp;<img alt=\"*\" src=\"" + Globals.ApplicationPath + "/images/breadcrumb.gif\">&nbsp;";
                }

                var strCssClass = "";
                if (!string.IsNullOrEmpty(CssClass)) strCssClass = CssClass;

                int intRootLevel = 0;
                if (GeneralUtils.IsNumeric(RootLevel)) intRootLevel = int.Parse(RootLevel);

                var strBreadCrumbs = "";

                // process bread crumbs
                int intTab = 0;

                if (!(HideWithNoBreadCrumb && (PortalSettings.ActiveTab.BreadCrumbs.Count == 1)))
                {
                    for (intTab = intRootLevel; intTab <= PortalSettings.ActiveTab.BreadCrumbs.Count - 1; intTab++)
                    {
                        if (intTab != intRootLevel) strBreadCrumbs += strSeparator;

                        var objTab = (TabInfo) PortalSettings.ActiveTab.BreadCrumbs[intTab];
                        var dataRecord = objCtrl.GetRecordByGuidKey(PortalSettings.Current.PortalId, -1, "PL", "PL_" + DNNrocketUtils.GetCurrentCulture() + "_" + PortalSettings.ActiveTab.TabID.ToString(""));

                        var pagename = "";
                        if (dataRecord != null)
                        {
                            pagename = dataRecord.GetXmlProperty("genxml/textbox/pagename");
                        }
                        else
                        {
                            // no PL data, so use normal tab data
                            pagename = objTab.TabName;
                        }

                        if (HtmlList)
                        {
                            strBreadCrumbs += "<ul class=\"" + strCssClass + "\">";
                            if (objTab.DisableLink)
                                strBreadCrumbs += "<li>" + pagename + "</li>";
                            else
                                strBreadCrumbs += "<li>" + "<a href=\"" + objTab.FullUrl + "\">" + pagename + "</a>" + "</li>";
                            strBreadCrumbs += "</ul>";
                        }
                        else
                        {
                            if (objTab.DisableLink)
                                strBreadCrumbs += "<span class=\"" + strCssClass + "\">" + pagename + "</span>";
                            else
                                strBreadCrumbs += "<a href=\"" + objTab.FullUrl + "\" class=\"" + strCssClass + "\">" + pagename + "</a>";
                        }
                    }
                }
                lBreadCrumb = new Literal();
                lBreadCrumb.Text = strBreadCrumbs;
                Controls.Add(lBreadCrumb);
            }

        }
    }
}
