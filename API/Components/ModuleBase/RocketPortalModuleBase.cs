using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Collections.Specialized.BitVector32;

namespace DNNrocketAPI.Components
{
    public abstract class RocketPortalModuleBase : PortalModuleBase
    {

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

        protected bool HasAdminSkinCookie()
        {
            var cookieName = "_SkinSrc" + PortalSettings.PortalId;
            var cookie = Request.Cookies[cookieName];
            return cookie != null && !string.IsNullOrEmpty(cookie.Value);
        }
    }
}
