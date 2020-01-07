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
    public partial class View : PortalModuleBase, IActionable
    {
        #region Event Handlers

        private bool _debugmode = false;
        private bool _activatedetail = false;

        private string _paramCmd;
        private string _systemkey;

        private string _interfacekey;
        private string _templateRelPath;
        private string _entiytypecode;

        private DNNrocketInterface _rocketInterface;

        private ModuleParams _moduleParams;
        private ModuleParams _dataModuleParams;
        private SimplisityInfo _systemInfo;
        private SystemData _systemData;
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

            var moduleInfo = ModuleController.Instance.GetModule(ModuleId, TabId, false);
            var desktopModule = moduleInfo.DesktopModule;
            _systemkey = desktopModule.ModuleDefinitions.First().Key.ToLower(); // Use the First DNN Module definition as the DNNrocket systemkey

            InitSystemInfo();

            _systemData = new SystemData(_systemInfo);
            _interfacekey = desktopModule.ModuleName.ToLower();  // Use the module name as DNNrocket interface key.
            _moduleParams = new ModuleParams(ModuleId, _systemData.SystemKey);
            _dataModuleParams = new ModuleParams(_moduleParams.DataSourceModId, _systemData.SystemKey);
            _rocketInterface = new DNNrocketInterface(_systemInfo, _interfacekey);
            var systemData = new SystemData(_systemInfo);
            _debugmode = systemData.DebugMode;
            _activatedetail = _moduleParams.GetValueBool("activatedetail");
            _paramCmd = _moduleParams.GetValue("command");

            if (_rocketInterface != null && _rocketInterface.Exists)
            {
                _templateRelPath = _rocketInterface.TemplateRelPath;
                _entiytypecode = _rocketInterface.EntityTypeCode;
                _paramCmd = _rocketInterface.DefaultCommand;
                if (String.IsNullOrEmpty(_templateRelPath)) _templateRelPath = base.ControlPath; // if we don't define template path in the interface assume it's the control path.

                if (_moduleParams.Exists)
                {
                    if (_moduleParams.TabId != PortalSettings.Current.ActiveTab.TabID)
                    {
                        // change of page for module
                        _moduleParams.TabId = PortalSettings.Current.ActiveTab.TabID; 
                        _moduleParams.Save();
                        CacheFileUtils.ClearAllCache(_moduleParams.CacheGroupId);
                    }

                    DNNrocketUtils.IncludePageHeaders(_moduleParams, this.Page, TabId, systemData.DebugMode);
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


        }


        protected override void OnPreRender(EventArgs e)
        {
            var hasEditAccess = false;
            if (UserId > 0) hasEditAccess = DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(this.ModuleConfiguration);

           
            var itemref = DNNrocketUtils.RequestQueryStringParam(Request, _moduleParams.DetailUrlParam);
            // check for detail page display
            if (GeneralUtils.IsNumeric(itemref) && _moduleParams.DetailView)
            {
                var info = _objCtrl.GetInfo(Convert.ToInt32(itemref), DNNrocketUtils.GetCurrentCulture());
                if (info != null)
                {
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
            }

            var postInfo = new SimplisityInfo();
            var paramInfo = new SimplisityInfo();
            paramInfo.PortalId = PortalSettings.Current.PortalId;
            paramInfo.ModuleId = ModuleId;
            paramInfo.SetXmlProperty("genxml/hidden/moduleid", ModuleId.ToString());
            paramInfo.SetXmlProperty("genxml/hidden/tabid", _moduleParams.TabId.ToString());
            paramInfo.SetXmlProperty("genxml/hidden/systemkey", _systemkey);

            if (_rocketInterface != null && _rocketInterface.Exists)
            {
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
                foreach (string key in Request.Form)
                {
                    if (key != null && !key.StartsWith("_")) // null can happen and don't bother with system form keys, we don;t use those.
                    {
                        paramInfo.SetXmlProperty("genxml/postform/" + key.Replace("_", "-"), Request.Form[key]); // remove '_' from xpath
                    }
                }

                var strOut = "";
                var cacheOutPut = "";
                var cacheKey = "view.ascx" + ModuleId + DNNrocketUtils.GetCurrentCulture() + paramString + DNNrocketUtils.GetCurrentCulture() + hasEditAccess;

                if (_moduleParams.CacheEnabled && _systemData.CacheOn) cacheOutPut = (string)CacheFileUtils.GetCache(cacheKey, _moduleParams.CacheGroupId);

                if (String.IsNullOrEmpty(cacheOutPut))
                {
                    var returnDictionary = DNNrocketUtils.GetProviderReturn(_paramCmd, _systemInfo, _rocketInterface, postInfo, paramInfo, _templateRelPath, DNNrocketUtils.GetCurrentCulture());

                    var model = new SimplisityRazor();
                    model.ModuleId = ModuleId;
                    model.TabId = TabId;

                    if (hasEditAccess)
                    {
                        model.SetSetting("editiconcolor", _systemData.GetSetting("editiconcolor"));
                        model.SetSetting("editicontextcolor", _systemData.GetSetting("editicontextcolor"));
                        strOut = "<div id='rocketmodcontentwrapper" + ModuleId + "' class=' w3-display-container'>";
                        var razorTempl = DNNrocketUtils.GetRazorTemplateData("viewinjecticons.cshtml", _templateRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", _systemData.DebugMode);
                        strOut += DNNrocketUtils.RazorRender(model, razorTempl, _systemData.DebugMode);
                    }

                    if (returnDictionary.ContainsKey("outputhtml"))
                    {
                        strOut += returnDictionary["outputhtml"];
                    }

                    if (hasEditAccess)
                    {
                        strOut += "</div>";
                        var razorTempl = DNNrocketUtils.GetRazorTemplateData("viewinject.cshtml", _templateRelPath, "config-w3", DNNrocketUtils.GetCurrentCulture(), "1.0", _systemData.DebugMode);
                        strOut += DNNrocketUtils.RazorRender(model, razorTempl, _systemData.DebugMode);
                    }
                    CacheFileUtils.SetCache(cacheKey, strOut, _moduleParams.CacheGroupId);
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
                PageIncludes.IncludeTextInHeader(Page, "<link rel='stylesheet' href='/DesktopModules/DNNrocket/fa/css/all.min.css'><link rel='stylesheet' href='/DesktopModules/DNNrocket/css/w3.css'><script type='text/javascript' src='/DesktopModules/DNNrocket/Simplisity/js/simplisity.js'></script>");
            }
        }


        #endregion

        private void InitSystemInfo()
        {
            _systemInfo = (SimplisityInfo)CacheUtils.GetCache(_systemkey + "modid" + ModuleId);
            if (_systemInfo == null)
            {
                _systemInfo = _objCtrl.GetByGuidKey(-1, -1, "SYSTEM", _systemkey);
                if (_systemInfo != null) CacheUtils.GetCache(_systemkey + "modid" + ModuleId, _systemkey);
            }
            if (_systemInfo == null)
            {
                // no system data, so must be new install.
                var sData = new SystemDataList(); // load XML files.
                _systemInfo = _objCtrl.GetByGuidKey(-1, -1, "SYSTEM", _systemkey);
                if (_systemInfo != null) CacheUtils.GetCache(_systemkey + "modid" + ModuleId, _systemkey);
            }

        }


        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();
                if (!_moduleParams.GetValueBool("noiframeedit"))
                {
                    actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.edit") , "", "", "register.gif", "javascript:" + _interfacekey + "editiframe_" + ModuleId + "()", false, SecurityAccessLevel.Edit, true, false);
                }

                if (_systemInfo != null) // might be null of initialization
                {
                    var adminurl = _systemInfo.GetXmlProperty("genxml/textbox/adminurl");
                    adminurl = adminurl.ToLower().Replace("[moduleid]", ModuleId.ToString());
                    adminurl = adminurl.ToLower().Replace("[tabid]", TabId.ToString());
                    if (adminurl != "")
                    {
                        actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.rocketadmin"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, false);
                        if (adminurl.Contains("?"))
                        {
                            adminurl += "&newpage=1";
                        }
                        else
                        {
                            adminurl += "?newpage=1";
                        }
                        actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.rocketadmintab"), "", "", "icon_dashboard_16px.gif", adminurl, false, SecurityAccessLevel.Edit, true, true);
                    }
                    actions.Add(GetNextActionID(), DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocket/API/App_LocalResources/", "DNNrocket.clearallcache"), "", "", "action_refresh.gif", "?clearallcache=" + ModuleId, false, SecurityAccessLevel.Edit, true, false);
                }
                return actions;
            }
        }

        #endregion



    }

}
