using DNNrocketAPI.Components;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Skins;
using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ClientDependency.Core;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// HTTP Module to replace the skin and clear old skin's scripts
    /// </summary>
    public class HttpModuleRocket : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += OnBeginRequest; // Earlier!
            application.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
        }

        private void OnBeginRequest(object source, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            var portalSettings = PortalUtils.GetCurrentPortalSettings();
            if (portalSettings != null)
            {
                string cookieName = "_SkinSrc" + portalSettings.PortalId;
                if (context.Request.Cookies[cookieName] != null)
                {
                    // Remove from request cookies (current request)
                    context.Request.Cookies.Remove(cookieName);
                    
                    // Expire in response (future requests)
                    HttpCookie cookie = new HttpCookie(cookieName)
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    };
                    context.Response.Cookies.Add(cookie);
                }
            }
        }

        private void OnPreRequestHandlerExecute(object source, EventArgs e)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (!(context.Handler is Page page))
                {
                    return;
                }

                if (context.Items["HttpModuleRocket_InitCompleteHooked"] != null)
                {
                    return;
                }

                context.Items["HttpModuleRocket_InitCompleteHooked"] = true;
                page.InitComplete += Page_InitComplete;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        private void Page_InitComplete(object sender, EventArgs e)
        {
            try
            {
                Page page = (Page)sender;
                HttpContext context = HttpContext.Current;

                // Clear the SkinSrc cookie set by RocketSkinModelFactory
                var portalSettings = PortalUtils.GetCurrentPortalSettings();
                if (portalSettings != null)
                {
                    string cookieName = "_SkinSrc" + portalSettings.PortalId;
                    if (context.Request.Cookies[cookieName] != null)
                    {
                        HttpCookie cookie = new HttpCookie(cookieName)
                        {
                            Expires = DateTime.Now.AddDays(-1)
                        };
                        context.Response.Cookies.Add(cookie);
                    }
                }

                // Get ctl parameter
                var ctl = GetCtlParameter(context);
                if (string.IsNullOrEmpty(ctl)) return;

                // Only apply the Rocket skin for Rocket modules
                if (!IsRocketModule(context)) return;

                // Determine which skin to apply
                string newSkinSrc = DetermineSkinToApply(portalSettings, ctl);
                if (string.IsNullOrEmpty(newSkinSrc)) return;

                // Format the skin source
                string formattedSkinSrc = SkinController.FormatSkinSrc(newSkinSrc + ".ascx", portalSettings);

                // Find the existing Skin control
                Skin existingSkin = FindSkinControl(page);
                if (existingSkin == null) return;

                // Load the NEW skin
                Skin newSkin = LoadSkinDirectly(page, formattedSkinSrc);
                if (newSkin == null) return;

                // Replace the skin
                ReplaceSkinControl(page, existingSkin, newSkin);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        private Skin LoadSkinDirectly(Page page, string skinPath)
        {
            try
            {
                var loadSkinMethod = typeof(Skin).GetMethod("LoadSkin",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                if (loadSkinMethod != null)
                {
                    var skin = (Skin)loadSkinMethod.Invoke(null, new object[] { page, skinPath });
                    return skin;
                }
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
            return null;
        }

        private Skin FindSkinControl(Control root)
        {
            if (root is Skin skin) return skin;

            foreach (Control child in root.Controls)
            {
                var found = FindSkinControl(child);
                if (found != null) return found;
            }
            return null;
        }

        private void ReplaceSkinControl(Page page, Skin oldSkin, Skin newSkin)
        {
            try
            {
                Control parent = oldSkin.Parent;
                if (parent == null) return;

                int index = parent.Controls.IndexOf(oldSkin);
                if (index < 0) return;

                parent.Controls.RemoveAt(index);
                parent.Controls.AddAt(index, newSkin);
                newSkin.ID = "dnn";
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        /// <summary>
        /// Returns true only when the module referenced by the "mid" query string parameter
        /// has a DesktopModule.ModuleName that starts with "Rocket".
        /// </summary>
        private bool IsRocketModule(HttpContext context)
        {
            var midStr = DNNrocketUtils.RequestQueryStringParam(context, "mid");
            if (string.IsNullOrEmpty(midStr)) midStr = DNNrocketUtils.RequestQueryStringParam(context, "moduleid");
            if (string.IsNullOrEmpty(midStr)) return false;

            if (!int.TryParse(midStr, out int moduleId)) return false;

            try
            {
                var moduleController = new ModuleController();
                var moduleInfo = moduleController.GetModule(moduleId, Null.NullInteger, true);
                if (moduleInfo == null) return false;
                if (moduleInfo.DesktopModule == null) return false;

                return moduleInfo.DesktopModule.ModuleName.StartsWith("Rocket", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
                return false;
            }
        }

        private string GetCtlParameter(HttpContext context)
        {
            var ctl = DNNrocketUtils.RequestQueryStringParam(context, "ctl");
            if (!string.IsNullOrEmpty(ctl)) return ctl;

            var segments = context.Request.Url.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length - 1; i++)
            {
                if (segments[i].Equals("ctl", StringComparison.OrdinalIgnoreCase))
                    return segments[i + 1];
            }
            return null;
        }
        private string DetermineSkinToApply(PortalSettings portalSettings, string ctlValue)
        {
            if (string.IsNullOrEmpty(ctlValue)) return null;

            var rocketAdmin = PortalController.GetPortalSetting("rocketadminskin", portalSettings.PortalId, "[G]Skins/rocketedit/rocketadmin");
            var rocketEdit = PortalController.GetPortalSetting("rocketeditskin", portalSettings.PortalId, "[G]Skins/rocketedit/rocketedit");

            switch (ctlValue.ToLower())
            {
                case "adminpanel":
                    return rocketAdmin;
                case "edit":
                case "rocketedit":
                case "apptheme":
                case "module":
                case "settings":
                case "recyclebin":
                    return rocketEdit;
                default:
                    return null;
            }
        }

        public void Dispose() { }
    }
}