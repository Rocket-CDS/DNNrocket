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
using DNNrocketAPI.Components;

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

        private bool _debugmode = false;
        private bool _activatedetail = false;

        private string _paramCmd;
        private string _systemkey;

        private string _interfacekey;
        private string _templateRelPath;

        private RocketInterface _rocketInterface;

        private ModuleParams _moduleParams;
        private SimplisityInfo _systemInfo;
        private DNNrocketController _objCtrl;

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);
            
            _objCtrl = new DNNrocketController();

            var clearallcache = DNNrocketUtils.RequestParam(Context, "clearallcache");
            if (clearallcache != "")
            {
                DNNrocketUtils.ClearPortalCache(PortalId);
                CacheFileUtils.ClearAllCache();
            }

            _systemkey = DNNrocketUtils.GetModuleSystemKey(ModuleId, TabId);
            _interfacekey = DNNrocketUtils.GetModuleInterfaceKey(ModuleId, TabId);
            _systemInfo = DNNrocketUtils.GetModuleSystemInfo(_systemkey, ModuleId);

            _moduleParams = new ModuleParams(ModuleId, _systemkey);
            _rocketInterface = new RocketInterface(_systemInfo, _interfacekey);
            var systemData = new SystemLimpet(_systemInfo);
            _debugmode = systemData.DebugMode;
            _activatedetail = _moduleParams.GetValueBool("activatedetail");
            _paramCmd = _moduleParams.GetValue("command");

            if (_rocketInterface != null && _rocketInterface.Exists)
            {

                _templateRelPath = _rocketInterface.TemplateRelPath;
                _paramCmd = _rocketInterface.DefaultCommand;
                if (String.IsNullOrEmpty(_templateRelPath)) _templateRelPath = base.ControlPath; // if we don't define template path in the interface assume it's the control path.

                if (_moduleParams.TabId != PortalSettings.Current.ActiveTab.TabID)
                {
                    // change of page for module
                    _moduleParams.TabId = PortalSettings.Current.ActiveTab.TabID; 
                    _moduleParams.Save();
                    CacheFileUtils.ClearAllCache();
                }

                if (_moduleParams.CacheDisbaled)
                {
                    CacheUtils.ClearAllCache(_moduleParams.CacheGroupId);
                    CacheUtilsDNN.ClearAllCache();
                    CacheFileUtils.ClearAllCache();
                }

                if (!this.Page.Items.Contains("dnnrocket_pageheader")) // flag to insure we only inject once for page load.
                {
                    DNNrocketUtils.IncludePageHeaders(_systemkey, this.Page, TabId, _moduleParams.CacheEnabled);
                }
            }
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

            var itemref = DNNrocketUtils.RequestQueryStringParam(Request, _moduleParams.DetailUrlParam);
            // check for detail page display
            if (GeneralUtils.IsNumeric(itemref) && _moduleParams.DetailView)
            {
                var info = _objCtrl.GetInfo(Convert.ToInt32(itemref), DNNrocketUtils.GetCurrentCulture());
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

            var strOut = "";
            var systemData = new SystemLimpet(_systemInfo);
            var cacheOutPut = "";
            var cacheKey = "view.ascx" + ModuleId + DNNrocketUtils.GetCurrentCulture() + paramString + hasEditAccess;
            var model = new SimplisityRazor();
            model.ModuleId = ModuleId;
            model.TabId = TabId;
            if (_moduleParams.CacheEnabled && systemData.CacheOn) cacheOutPut = (string)CacheFileUtils.GetCache(cacheKey);
            if (String.IsNullOrEmpty(cacheOutPut))
            {

                if (hasEditAccess)
                {
                    model.SetSetting("editiconcolor", systemData.GetSetting("editiconcolor"));
                    model.SetSetting("editicontextcolor", systemData.GetSetting("editicontextcolor"));
                    strOut = "<div id='rocketcontentwrapper" + ModuleId + "' class=' w3-display-container '>";
                    var razorTempl = RenderRazorUtils.GetRazorTemplateData("viewinjecticons.cshtml", _templateRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                    strOut += RenderRazorUtils.RazorRender(model, razorTempl, true);
                }


                    // -------------------------------------------------------------------------------------------------------
                    // -------------------------- Remote Call not setup, do normal actions. ----------------------------------
                    // -------------------------------------------------------------------------------------------------------
                    if (_rocketInterface != null && _rocketInterface.Exists)
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

                        if (returnDictionary.ContainsKey("outputhtml"))
                        {
                            strOut += returnDictionary["outputhtml"];
                        }

                    }
                    else
                    {
                        strOut = "Invalid Interface: systemkey: " + _systemkey + "  interfacekey: " + _interfacekey;
                    }

                if (hasEditAccess)
                {
                    strOut += "</div>";
                }

                CacheFileUtils.SetCache(cacheKey, strOut);

            }
            else
            {
                strOut = cacheOutPut;
            }

            var lit = new Literal();
            strOut = "<div id='simplisitymodulewrapper" + ModuleId + "' class=' simplisity_panel '>" + strOut;
            strOut += "</div>";
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

                if (_systemInfo != null) // might be null of initialization
                {
                    var adminurl = _systemInfo.GetXmlProperty("genxml/textbox/adminurl");
                    adminurl = adminurl.ToLower().Replace("[moduleid]", ModuleId.ToString());
                    adminurl = adminurl.ToLower().Replace("[tabid]", TabId.ToString());
                    if (adminurl != "")
                    {
                        actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.rocketadmin"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, false);
                        actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.rocketadmintab"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, true);
                    }
                }
                return actions;
            }
        }

        #endregion



    }

}