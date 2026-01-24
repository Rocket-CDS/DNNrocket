using DNNrocketAPI.Components;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Skins;
using System;
using System.Web;
using System.Web.UI;
using DotNetNuke.Web.Client.ClientResourceManagement;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// HTTP Module to replace the skin and clear old skin's scripts
    /// </summary>
    public class HttpModuleRocket : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            try
            {
                application.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
            }
            catch (Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        private void OnPreRequestHandlerExecute(object source, EventArgs e)
        {
            try
            {
                HttpContext context = HttpContext.Current;

                // Only process Page requests
                if (!(context.Handler is Page page)) return;

                // Hook into PreRender - just before rendering               
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

                var portalSettings = PortalUtils.GetCurrentPortalSettings();
                if (portalSettings == null) return;

                // Get ctl parameter
                var ctl = GetCtlParameter(context);
                if (string.IsNullOrEmpty(ctl)) return;

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

        private string DetermineSkinToApply(PortalSettings portalSettings, string ctl)
        {
            if (string.IsNullOrEmpty(ctl)) return null;

            switch (ctl.ToLower())
            {
                case "adminpanel": return "/rocketadmin";
                case "apptheme": return "/rocketadmin";
                case "edit": return "/rocketedit";
                case "module": return "/rocketedit";
                case "recyclebin": return "/rocketedit";
                default: return null;
            }
        }

        public void Dispose() { }
    }
}