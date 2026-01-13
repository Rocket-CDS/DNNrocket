using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Skins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using static System.Collections.Specialized.BitVector32;

namespace DNNrocketAPI.Components
{

    public abstract class RocketModuleSettingsBase : ModuleSettingsBase
    {

        protected string AdminSkinName = "/rocketedit"; // Just the skin name, no path or extension

        /// <summary>
        /// Set the admin skin using DNN's built-in cookie system
        /// </summary>
        protected void ApplyAdminSkinCookie()
        {
            var cookieName = "_SkinSrc" + PortalSettings.PortalId;
            var skinCookie = new HttpCookie(cookieName, AdminSkinName)
            {
                Path = "/",
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(8) // Cookie expires in 8 hours
            };

            Response.Cookies.Add(skinCookie);

            // Redirect to apply the new skin
            Response.Redirect(Request.RawUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Remove the admin skin cookie to revert to normal skin
        /// </summary>
        protected void RemoveAdminSkinCookie()
        {
            var cookieName = "_SkinSrc" + PortalSettings.PortalId;

            // Create an expired cookie to delete it
            var expiredCookie = new HttpCookie(cookieName, "")
            {
                Path = "/",
                Expires = DateTime.Now.AddDays(-1)
            };

            Response.Cookies.Add(expiredCookie);

            // Redirect to revert to normal skin
            Response.Redirect(Request.RawUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Check if admin skin cookie is currently set
        /// </summary>
        protected bool HasAdminSkinCookie()
        {
            var cookieName = "_SkinSrc" + PortalSettings.PortalId;
            var cookie = Request.Cookies[cookieName];
            return cookie != null && !string.IsNullOrEmpty(cookie.Value);
        }

    }
}
