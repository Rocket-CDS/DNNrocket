// --- Copyright (c) notice NevoWeb ---
//  Copyright (c) 2015 SARL Nevoweb.  www.Nevoweb.com. The MIT License (MIT).
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using Simplisity;

namespace DNNrocketAPI
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewNBrightGen class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : PortalModuleBase, IActionable
    {
        #region Event Handlers


        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            DNNrocketUtils.IncludePageHeaders(base.ModuleId.ToString(""), this.Page, "NBrightMod","view");
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {

                base.OnLoad(e);

                if (Page.IsPostBack == false)
                {
                    // check we have settings
                    var settings = DNNrocketUtils.GetModuleSettings(ModuleId);

                    if (settings.ModuleId == 0 || settings.GetXmlProperty("genxml/dropdownlist/themefolder") == "")
                    {
                        var lit = new Literal();
                        var rtnValue = DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/api","DNNrocket.nosettings");
                        lit.Text = rtnValue;
                        phData.Controls.Add(lit);
                    }
                    else
                    {
                        PageLoad();
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PageLoad()
        {
            var objCtrl = new DNNrocketController();

            var eid = DNNrocketUtils.RequestQueryStringParam(Request, "eid");
            // check for detail page display
            if (GeneralUtils.IsNumeric(eid))
            {
                var info = objCtrl.GetInfo(Convert.ToInt32(eid), DNNrocketUtils.GetCurrentCulture());

                var pagename = info.GetXmlProperty("genxml/lang/genxml/textbox/pagename");
                if (pagename == "") pagename = info.GetXmlProperty("genxml/textbox/pagename");
                if (pagename == "") pagename = info.GetXmlProperty("genxml/lang/genxml/textbox/title");
                if (pagename == "") pagename = info.GetXmlProperty("genxml/textbox/title");

                var pagetitle = info.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle");
                if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/textbox/pagetitle");
                if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/lang/genxml/textbox/title");
                if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/textbox/title");

                var pagekeywords = info.GetXmlProperty("genxml/lang/genxml/textbox/pagekeywords");

                var pagedescription = info.GetXmlProperty("genxml/lang/genxml/textbox/pagedescription");

                DotNetNuke.Framework.CDefault tp = (DotNetNuke.Framework.CDefault)this.Page;
                if (pagetitle != "") tp.Title = pagetitle;
                if (pagedescription != "") tp.Description = pagedescription;
                if (pagekeywords != "") tp.KeyWords = pagekeywords;

            }

            var settings = DNNrocketUtils.GetModuleSettings(ModuleId);
            var debug = settings.GetXmlPropertyBool("genxml/checkbox/debugmode");
            var activatedetail = settings.GetXmlPropertyBool("genxml/checkbox/activatedetail");
            // check for detail page display

            var paramCmd = settings.GetXmlProperty("genxml/hidden/command");
            var postInfo = new SimplisityInfo();
            var systemInfo = new SimplisityInfo();
            var interfacekey = settings.GetXmlProperty("genxml/hidden/interfacekey");
            var iface = systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfacekey);
            var templateRelPath = iface.GetXmlProperty("genxml/textbox/relpath");

            var strOut = "";
            var returnDictionary = DNNrocketUtils.GetProviderReturn(paramCmd, systemInfo, interfacekey, postInfo, templateRelPath, DNNrocketUtils.GetCurrentCulture());

            if (returnDictionary.ContainsKey("outputhtml"))
            {
                strOut = returnDictionary["outputhtml"];
            }
            var lit = new Literal();
            lit.Text = strOut;
            phData.Controls.Add(lit);

        }

        #endregion


        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var settings = DNNrocketUtils.GetModuleSettings(ModuleId);
                var actions = new ModuleActionCollection();
                if (settings.GUIDKey == settings.GetXmlProperty("genxml/dropdownlist/datasourceref") || settings.GetXmlProperty("genxml/dropdownlist/datasourceref") == "")
                {
                    actions.Add(GetNextActionID(), Localization.GetString("EditModule", this.LocalResourceFile), "", "", "", EditUrl(), false, SecurityAccessLevel.Edit, true, false);
                }

                actions.Add(GetNextActionID(), Localization.GetString("Refresh", this.LocalResourceFile), "", "", "action_refresh.gif", EditUrl() + "?refreshview=1", false, SecurityAccessLevel.Edit, true, false);
                return actions;
            }
        }

        #endregion



    }

}
