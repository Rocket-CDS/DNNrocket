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

        private DNNrocketController _objCtrl;

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            _objCtrl = new DNNrocketController();
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

            var itemref = DNNrocketUtils.RequestQueryStringParam(Request, _remoteData.DetailUrlParam);
            // check for detail page display
            if (GeneralUtils.IsNumeric(itemref) && _remoteData.DetailView)
            {
                var info = _remoteData.PageInfo;
                if (info != null)
                {
                    var pagetitle = info.GetXmlProperty("genxml/lang/genxml/textbox/pagetitle");
                    if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/textbox/pagetitle");
                    if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/lang/genxml/textbox/title");
                    if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/textbox/title");
                    if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/textbox/pagename");
                    if (pagetitle == "") pagetitle = info.GetXmlProperty("genxml/lang/genxml/textbox/pagename");

                    var pagekeywords = info.GetXmlProperty("genxml/lang/genxml/textbox/pagekeywords");

                    var pagedescription = info.GetXmlProperty("genxml/lang/genxml/textbox/pagedescription").Replace(Environment.NewLine," ");

                    DotNetNuke.Framework.CDefault tp = (DotNetNuke.Framework.CDefault)this.Page;
                    if (pagetitle != "") tp.Title = pagetitle;
                    if (pagedescription != "") tp.MetaDescription = pagedescription;
                    if (pagekeywords != "") tp.MetaKeywords = pagekeywords;
                    if (pagedescription != "") tp.Description = pagedescription;
                    if (pagekeywords != "") tp.KeyWords = pagekeywords;
                }
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
            paramInfo.SetXmlProperty("genxml/hidden/tabid", _moduleParams.TabId.ToString());
            paramInfo.SetXmlProperty("genxml/hidden/systemkey", _systemkey);

            var adminmenu = DNNrocketUtils.RequestParam(Context, "adminmenu");

            if (_rocketInterface != null && _rocketInterface.Exists)
            {
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

                    var systemData = new SystemData(_systemInfo);

                    if (_moduleParams.CacheEnabled && systemData.CacheOn) cacheOutPut = (string)CacheFileUtils.GetCache(cacheKey, _moduleParams.CacheGroupId);

                if (String.IsNullOrEmpty(cacheOutPut))
                {

                    // before event
                    var rtnDictInfo = DNNrocketUtils.EventProviderBefore(_paramCmd, systemData, postInfo, paramInfo, DNNrocketUtils.GetCurrentCulture());
                    if (rtnDictInfo.ContainsKey("post")) postInfo = (SimplisityInfo)rtnDictInfo["post"];
                    if (rtnDictInfo.ContainsKey("param")) paramInfo = (SimplisityInfo)rtnDictInfo["param"];

                    var returnDictionary = DNNrocketUtils.GetProviderReturn(_paramCmd, _systemInfo, _rocketInterface, postInfo, paramInfo, _templateRelPath, DNNrocketUtils.GetCurrentCulture());

                    // after Event
                    var eventDictionary = DNNrocketUtils.EventProviderAfter(_paramCmd, systemData, postInfo, paramInfo, DNNrocketUtils.GetCurrentCulture());
                    if (eventDictionary.ContainsKey("outputhtml")) returnDictionary["outputhtml"] = eventDictionary["outputhtml"];
                    if (eventDictionary.ContainsKey("outputjson")) returnDictionary["outputjson"] = eventDictionary["outputjson"];

                    var model = new SimplisityRazor();
                    model.ModuleId = ModuleId;
                    model.TabId = TabId;

                    if (hasEditAccess)
                    {

                        model.SetSetting("editiconcolor", systemData.GetSetting("editiconcolor"));
                        model.SetSetting("editicontextcolor", systemData.GetSetting("editicontextcolor"));
                        strOut = "<div id='rocketmodcontentwrapper" + ModuleId + "' class=' w3-display-container'>";
                        var razorTempl = DNNrocketUtils.GetRazorTemplateData("viewinjecticons.cshtml", _templateRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", systemData.DebugMode);
                        strOut += DNNrocketUtils.RazorRender(model, razorTempl, systemData.DebugMode);
                    }

                    if (returnDictionary.ContainsKey("outputhtml"))
                    {
                        strOut += returnDictionary["outputhtml"];
                    }

                    if (hasEditAccess)
                    {
                        strOut += "</div>";
                        var razorTempl = DNNrocketUtils.GetRazorTemplateData("viewinject.cshtml", _templateRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", systemData.DebugMode);
                        strOut += DNNrocketUtils.RazorRender(model, razorTempl, systemData.DebugMode);
                    }

                    if (adminmenu == ModuleId.ToString())
                    {
                        // this extends the DNN action menu, to make it more flexable and allow reuse of this view.ascx
                        strOut += DNNrocketUtils.GetAdminMenu(TabId, ModuleId, _rocketInterface);
                    }

                    CacheFileUtils.SetCache(cacheKey, strOut);

                }
                else
                {
                    strOut = cacheOutPut;
                }


                var lit = new Literal();
                lit.Text = strOut;
                phData.Controls.Add(lit);
            }
            else
            {
                var strOut = "Invalid Interface: systemkey: " + _systemkey + "  interfacekey: " + _interfacekey;
                var lit = new Literal();
                lit.Text = strOut;
                phData.Controls.Add(lit);
            }

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

                if (_systemInfo != null) // might be null of initialization
                {
                    actions.Add(GetNextActionID(), "<i class='fas fa-user-cog fa-lg'></i>&nbsp;" + DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.admin"), "", "", "", "?adminmenu=" + ModuleId, false, SecurityAccessLevel.Edit, true, false);
                    actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.clearallcache"), "", "", "action_refresh.gif", "?clearallcache=" + ModuleId, false, SecurityAccessLevel.Edit, true, false);
                }
                return actions;
            }
        }

        #endregion



    }

}
