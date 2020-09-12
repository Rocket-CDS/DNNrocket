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
using System.IO;

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

        private string _systemkey;
        private string _interfacekey;
        private string _templateRelPath;
        private SystemLimpet _systemData;
        private RocketInterface _rocketInterface;
        private RemoteLimpet _remoteParams;
        private SimplisityInfo _systemInfo;
        private DNNrocketController _objCtrl;
        private bool _hasEditAccess;
        private string _paramString;
        protected override void OnInit(EventArgs e)
        {
            try
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

                _remoteParams = new RemoteLimpet(ModuleId, _systemkey);
                _rocketInterface = new RocketInterface(_systemInfo, _interfacekey);
                _systemData = new SystemLimpet(_systemInfo);

                _templateRelPath = _rocketInterface.TemplateRelPath;
                if (String.IsNullOrEmpty(_templateRelPath)) _templateRelPath = base.ControlPath; // if we don't define template path in the interface assume it's the control path.


                // add parameters remoteParams  (do here, so it appears in header call)
                _paramString = "";
                _remoteParams.RemoveAllUrlParam(); // remove any existing url params.
                foreach (String key in Request.QueryString.AllKeys)
                {
                    if (key != null) // test for null, but should not happen.   
                    {
                        _remoteParams.AddUrlParam(key, Request.QueryString[key]);
                        _paramString += key + "=" + Request.QueryString[key]; // for cacheKey
                    }
                }
                // get all form data (drop the ones we already processed) 
                _remoteParams.RemoveAllFormParam(); // remove any existing form params.
                foreach (string key in Request.Form.AllKeys)
                {
                    if (key != null && (key.ToLower() != "paramjson" && key.ToLower() != "inputjson"))
                    {
                        _remoteParams.AddFormParam(key, Convert.ToString(Request.Form[key]));
                    }
                }


                if (!this.Page.Items.Contains("dnnrocket_remotepageheader")) // flag to insure we only inject once for page load.
                {
                    var cachekey = TabId + ".remotepageheader.cshtml";
                    string cacheHead = (string)CacheFileUtils.GetCache(cachekey);
                    if (String.IsNullOrEmpty(cacheHead) || _remoteParams.CacheDisbaled)
                    {
                        var appList = new List<string>();
                        cacheHead = ""; // clear so if we rebuild, we don;t use the cached data
                        var modulesOnPage = DNNrocketUtils.GetAllModulesOnPage(TabId);
                        foreach (var modId in modulesOnPage)
                        {
                            var remoteParamsMod = new RemoteLimpet(modId);
                            var appcheck = remoteParamsMod.AppThemeFolder + "*" + remoteParamsMod.AppThemeVersion + "*" + remoteParamsMod.EngineURL;
                            if (!appList.Contains(appcheck))
                            {
                                var strOut = remoteParamsMod.headerAPI();
                                if (strOut != "") cacheHead += " " + strOut;
                                appList.Add(appcheck);
                            }
                        }
                        CacheFileUtils.SetCache(cachekey, cacheHead);
                        PageIncludes.IncludeTextInHeader(Page, cacheHead);
                    }
                    else
                    {
                        PageIncludes.IncludeTextInHeader(Page, cacheHead);
                    }
                    Page.Items.Add("dnnrocket_remotepageheader", true);
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
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
            _hasEditAccess = false;
            if (UserId > 0) _hasEditAccess = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(this.ModuleConfiguration);
        }


        protected override void OnPreRender(EventArgs e)
        {
            var strOut = "";
            var systemData = new SystemLimpet(_systemInfo);
            var cacheOutPut = "";
            var cacheKey = _remoteParams.RemoteTemplate + ModuleId + DNNrocketUtils.GetCurrentCulture() + _paramString + DNNrocketUtils.GetCurrentCulture() + _hasEditAccess;
            var model = new SimplisityRazor();
            model.ModuleId = ModuleId;
            model.TabId = TabId;
            if (_remoteParams.CacheEnabled && systemData.CacheOn) cacheOutPut = (string)CacheFileUtils.GetCache(cacheKey);
            if (String.IsNullOrEmpty(cacheOutPut))
            {
                strOut += _remoteParams.htmlAPI();
                CacheFileUtils.SetCache(cacheKey, strOut);
            }
            else
            {
                strOut = cacheOutPut;
            }

            var lit = new Literal();
            strOut = "<div id='simplisitymodulewrapper" + ModuleId + "' class=' simplisity_panel '>" + strOut + "</div>";
            lit.Text = strOut;
            phData.Controls.Add(lit);

            if (_hasEditAccess && (!Page.Items.Contains("dnnrocketview-addedheader") || !(bool)Page.Items["dnnrocketview-addedheader"]))
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
                if (_systemData.Exists) // might be null of initialization
                {
                    var adminurl = "/DesktopModules/DNNrocket/RocketRemoteMod/admin.html?moduleid=[moduleid]&tabid=[tabid]&systemkey=[systemkey]";
                    adminurl = adminurl.ToLower().Replace("[moduleid]", ModuleId.ToString());
                    adminurl = adminurl.ToLower().Replace("[tabid]", TabId.ToString());
                    adminurl = adminurl.ToLower().Replace("[systemkey]",  _systemkey);
                    if (adminurl != "")
                    {
                        actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.settings"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, false);
                    }
                }
                return actions;
            }
        }

        #endregion



    }

}
