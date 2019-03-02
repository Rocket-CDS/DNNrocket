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


        private SimplisityInfo _settings;
        private bool _debugmode = false;
        private bool _activatedetail = false;

        private string _paramCmd;
        private string _systemprovider;

        private SimplisityInfo _systemInfo;
        private string _interfacekey;
        private SimplisityInfo _iface;
        private string _templateRelPath;


        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);


            var objCtrl = new DNNrocketController();

            _settings = DNNrocketUtils.GetModuleSettings(ModuleId);
            _debugmode = _settings.GetXmlPropertyBool("genxml/checkbox/debugmode");
            _activatedetail = _settings.GetXmlPropertyBool("genxml/checkbox/activatedetail");
            // check for detail page display

            _paramCmd = _settings.GetXmlProperty("genxml/hidden/command");
            _systemprovider = _settings.GetXmlProperty("genxml/hidden/systemprovider");
            if (_systemprovider == "") _systemprovider = "dnnrocket";

            _systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", _systemprovider);
            _interfacekey = _settings.GetXmlProperty("genxml/hidden/interfacekey");
            if (_interfacekey == "") _interfacekey = "dnnrocketmodule";
            _iface = _systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", _interfacekey);
            _templateRelPath = base.ControlPath;
            if (_iface != null)
            {
                _templateRelPath = _iface.GetXmlProperty("genxml/textbox/relpath");
            }

            DNNrocketUtils.IncludePageHeaders(base.ModuleId, this.Page, "DNNrocket", _templateRelPath,"pageheader.cshtml", "config-w3");
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {

                base.OnLoad(e);

                if (Page.IsPostBack == false)
                {
                    PageLoad();
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

            var postInfo = new SimplisityInfo();
            postInfo.ModuleId = ModuleId;

            if (_iface != null)
            {
                // we have interface so link to the interface system
                _templateRelPath = _iface.GetXmlProperty("genxml/textbox/relpath");
            }
            else
            {
                // no interface, so link to the default DNNrocketModule.
                _paramCmd = "";
                _interfacekey = "dnnrocketmodule";
                _systemInfo.GUIDKey = "dnnrocket";
                _templateRelPath = "/DesktopModules/DNNrocket/DNNrocketModule";
            }


            var strOut = "";
            var returnDictionary = DNNrocketUtils.GetProviderReturn(_paramCmd, _systemInfo, _interfacekey, postInfo, _templateRelPath, DNNrocketUtils.GetCurrentCulture());

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
                    actions.Add(GetNextActionID(), "***Edit Data***", "", "", "", EditUrl(), false, SecurityAccessLevel.Edit, true, false);
                }

                actions.Add(GetNextActionID(), "***Refrsh***", "", "", "action_refresh.gif", EditUrl() + "?refreshview=1", false, SecurityAccessLevel.Edit, true, false);
                return actions;
            }
        }

        #endregion



    }

}
