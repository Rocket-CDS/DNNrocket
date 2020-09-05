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

            _remoteParams = new RemoteLimpet(ModuleId, _systemkey);
            _rocketInterface = new RocketInterface(_systemInfo, _interfacekey);
            _systemData = new SystemLimpet(_systemInfo);

            _templateRelPath = _rocketInterface.TemplateRelPath;
            if (String.IsNullOrEmpty(_templateRelPath)) _templateRelPath = base.ControlPath; // if we don't define template path in the interface assume it's the control path.


            // add parameters remoteParams  (do here, so it appears in header call)
            _remoteParams.RemoveAllUrlParam(); // remove any existing url params.
            foreach (String key in Request.QueryString.AllKeys)
            {
                if (key != null) // test for null, but should not happen.   
                {
                    _remoteParams.AddUrlParam(key, Request.QueryString[key]);
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


            if (!this.Page.Items.Contains("dnnrocket_remotepageheader") || _remoteParams.CacheDisbaled) // flag to insure we only inject once for page load.
            {
                Page.Items.Add("dnnrocket_remotepageheader", true);
                var cachekey = TabId + ".remotepageheader.cshtml";
                string cacheHead = (string)CacheFileUtils.GetCache(cachekey, "remotepageheader");
                if (String.IsNullOrEmpty(cacheHead) || _remoteParams.CacheDisbaled)
                {
                    var modulesOnPage = DNNrocketUtils.GetAllModulesOnPage(TabId);
                    foreach (var modId in modulesOnPage)
                    {
                        var remoteParamsMod = new RemoteLimpet(modId);
                        var strOut = remoteParamsMod.headerAPI();
                        if (strOut != "") cacheHead += " " + strOut;
                    }

                    CacheFileUtils.SetCache(cachekey, cacheHead);
                    PageIncludes.IncludeTextInHeader(Page, cacheHead);
                }
                else
                {
                    PageIncludes.IncludeTextInHeader(Page, cacheHead);
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
            _hasEditAccess = false;
            if (UserId > 0) _hasEditAccess = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(this.ModuleConfiguration);
        }


        protected override void OnPreRender(EventArgs e)
        {
            var postInfo = new SimplisityInfo();
            var paramInfo = new SimplisityInfo();
            paramInfo.PortalId = PortalSettings.Current.PortalId;
            paramInfo.ModuleId = ModuleId;
            paramInfo.SetXmlProperty("genxml/hidden/moduleid", ModuleId.ToString());
            paramInfo.SetXmlProperty("genxml/hidden/tabid", _remoteParams.TabId.ToString());
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
            var cacheKey = "view.ascx" + ModuleId + DNNrocketUtils.GetCurrentCulture() + paramString + DNNrocketUtils.GetCurrentCulture() + _hasEditAccess;
            var model = new SimplisityRazor();
            model.ModuleId = ModuleId;
            model.TabId = TabId;
            if (_remoteParams.CacheEnabled && systemData.CacheOn) cacheOutPut = (string)CacheFileUtils.GetCache(cacheKey);
            if (String.IsNullOrEmpty(cacheOutPut))
            {

                if (_hasEditAccess)
                {
                    model.SetSetting("editiconcolor", systemData.GetSetting("editiconcolor"));
                    model.SetSetting("editicontextcolor", systemData.GetSetting("editicontextcolor"));
                    strOut = "<div id='rocketcontentwrapper" + ModuleId + "' class=' w3-display-container '>";
                    var razorTempl = RenderRazorUtils.GetRazorTemplateData("viewinjecticons.cshtml", _templateRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", true);
                    strOut += RenderRazorUtils.RazorRender(model, razorTempl, true);
                }

                strOut += _remoteParams.htmlAPI();

                if (_hasEditAccess)
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
                    var adminFilePath = DNNrocketUtils.MapPath(_systemData.SystemRelPath.Trim('/')) + "\\admin.html";
                    if (File.Exists(adminFilePath))
                    {
                        var adminurl = _systemData.AdminUrl;
                        adminurl = adminurl.ToLower().Replace("[moduleid]", ModuleId.ToString());
                        adminurl = adminurl.ToLower().Replace("[tabid]", TabId.ToString());
                        if (adminurl != "")
                        {
                            actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.rocketadmin"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, false);
                            if (adminurl.Contains("?"))
                                adminurl += "&newpage=1";
                            else
                                adminurl += "?newpage=1";
                            actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.rocketadmintab"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, true);
                        }
                    }

                    var module = ModuleController.Instance.GetModule(ModuleId,TabId,false);
                    var adminsettingsFilePath = DNNrocketUtils.MapPath("/DesktopModules/DNNrocket/config/") + "\\" + module.DesktopModule.FolderName + ".html";
                    if (File.Exists(adminsettingsFilePath))
                    {
                        var adminsettingsurl = "/DesktopModules/DNNrocket/config/" + module.DesktopModule.FolderName + ".html?moduleid=" + ModuleId + "&tabid=" + TabId;
                        actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.settings"), "", "", "icon_dashboard_16px.gif", adminsettingsurl, false, SecurityAccessLevel.Edit, true, true);
                    }
                }
                return actions;
            }
        }

        #endregion



    }

}
