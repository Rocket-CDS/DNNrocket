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
using System.Collections.Specialized;
using DNNrocketAPI.Componants;

namespace DNNrocketAPI
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewNBrightGen class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ViewRemote : PortalModuleBase, IActionable
    {
        #region Event Handlers

        private RemoteData _remoteData;
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
            _remoteData = new RemoteData(ModuleId);

            // check for detail page display
            var srec = _remoteData.Record;
            if (srec != null)
            {
                var pagetitle = srec.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle");
                if (pagetitle == "") pagetitle = srec.GetXmlProperty("genxml/textbox/pagetitle");
                if (pagetitle == "") pagetitle = srec.GetXmlProperty("genxml/lang/genxml/textbox/title");
                if (pagetitle == "") pagetitle = srec.GetXmlProperty("genxml/textbox/title");
                if (pagetitle == "") pagetitle = srec.GetXmlProperty("genxml/textbox/pagename");
                if (pagetitle == "") pagetitle = srec.GetXmlProperty("genxml/lang/genxml/textbox/pagename");

                var pagekeywords = srec.GetXmlProperty("genxml/lang/genxml/textbox/pagekeywords");

                var pagedescription = srec.GetXmlProperty("genxml/lang/genxml/textbox/pagedescription").Replace(Environment.NewLine," ");

                DotNetNuke.Framework.CDefault tp = (DotNetNuke.Framework.CDefault)this.Page;
                if (pagetitle != "") tp.Title = pagetitle;
                if (pagedescription != "") tp.MetaDescription = pagedescription;
                if (pagekeywords != "") tp.MetaKeywords = pagekeywords;
                if (pagedescription != "") tp.Description = pagedescription;
                if (pagekeywords != "") tp.KeyWords = pagekeywords;
            }
        }


        protected override void OnPreRender(EventArgs e)
        {
            var hasEditAccess = false;
            if (UserId > 0) hasEditAccess = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(this.ModuleConfiguration);

            var postInfo = new SimplisityInfo();
            var paramInfo = new SimplisityInfo();
            paramInfo.PortalId = PortalSettings.Current.PortalId;
            paramInfo.ModuleId = ModuleId;
            paramInfo.SetXmlProperty("genxml/hidden/moduleid", ModuleId.ToString());

            var adminmenu = DNNrocketUtils.RequestParam(Context, "adminmenu");

            var strOut = "";

            // add parameters to postInfo and cachekey
            var paramString = "";
            foreach (String key in Request.QueryString.AllKeys)
            {
                if (key != null) // test for null, but should not happen.   
                {
                    paramString += key + "=" + Request.QueryString[key];
                    // we need this if we need to process url parmas on the APInterface.  In the cshtml we can use (Model.GetUrlParam("????"))
                    paramInfo.SetXmlProperty("genxml/urlparams/" + key.Replace("_", "-"), Request.QueryString[key]);
                }
            }

            var cacheOutPut = "";
            var cacheKey = "view.ascx" + ModuleId + DNNrocketUtils.GetCurrentCulture() + paramString + DNNrocketUtils.GetCurrentCulture() + hasEditAccess + adminmenu;

            _remoteData.Save();

            var postInfoSend = new SimplisityInfo();
            postInfo.SetXmlProperty("genxml/hidden/text", "test data node");
            var paramInfoSend = new SimplisityInfo();
            paramInfo.SetXmlProperty("genxml/hidden/sitekey", "2E558D17-E566-4407-B15A-AADD3EC2DA8B");
            paramInfo.SetXmlProperty("genxml/hidden/systemkey", "rocketecommerce");
            strOut = _remoteData.CallAPI("dev.dnnrocket.com", "/Desktopmodules/dnnrocket/api/rocket/actionremote", "paymentform_paymentform", GeneralUtils.EnCode("rocketecommerce"), postInfoSend, paramInfoSend, "", "POST","http");

            var lit = new Literal();
            lit.Text = strOut;
            phData.Controls.Add(lit);
 

            if (hasEditAccess && (!Page.Items.Contains("dnnrocketview-addedheader") || !(bool)Page.Items["dnnrocketview-addedheader"]))
            {
                if (!Page.Items.Contains("dnnrocketview-addedheader")) Page.Items.Add("dnnrocketview-addedheader", true);

                PageIncludes.IncludeTextInHeaderAt(Page, "<link rel='stylesheet' href='/DesktopModules/DNNrocket/fa/css/all.min.css'><link rel='stylesheet' href='/DesktopModules/DNNrocket/css/w3.css'>", 1);

                //insert at end of head section, we have a dependancy on JQuery, so we need to inject AFTER jquery.  [This may "possible" cause a conflict with dependancy files loaded before this.]
                PageIncludes.IncludeTextInHeaderAt(Page, "<script type='text/javascript' src='/DesktopModules/DNNrocket/Simplisity/js/simplisity.js'></script>",0);
            }
        }


        #endregion


        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();

                actions.Add(GetNextActionID(), "<i class='fas fa-user-cog fa-lg'></i>&nbsp;" + DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.admin"), "", "", "", "?adminmenu=" + ModuleId, false, SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.clearallcache"), "", "", "action_refresh.gif", "?clearallcache=" + ModuleId, false, SecurityAccessLevel.Edit, true, false);

                return actions;
            }
        }

        #endregion



    }

}
