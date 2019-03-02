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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using Simplisity;

namespace DNNrocketAPI
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewNBrightGen class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : ModuleSettingsBase
    {

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DNNrocketUtils.IncludePageHeaders(base.ModuleId, this.Page, "NBrightMod", "settings", "config");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Page.IsPostBack == false)
            {
                PageLoad();
            }
        }

        private void PageLoad()
        {
            try
            {
                var obj = DNNrocketUtils.GetModuleSettings(base.ModuleId);
                obj.ModuleId = base.ModuleId; // need to pass the moduleid here, becuase it doesn;t exists in url for settings and on new settings it needs it.

                var systemInfo = new SimplisityInfo();

                var strOut = "";
                var returnDictionary = DNNrocketUtils.GetProviderReturn("viewsettings_getdetails", systemInfo, "viewsettings", obj, base.ControlPath, DNNrocketUtils.GetCurrentCulture());

                if (returnDictionary.ContainsKey("outputhtml"))
                {
                    strOut = returnDictionary["outputhtml"];
                }
                var lit = new Literal();
                lit.Text = strOut;
                phData.Controls.Add(lit);

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        #endregion

    }

}
