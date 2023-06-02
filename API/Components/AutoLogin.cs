using DNNrocketAPI.Components;
using System;
using System.Web;

namespace DNNrocketAPI.Components
{
    public class AutoLogin : IHttpModule
    {
        public AutoLogin()
        {
        }

        public String ModuleName
        {
            get { return "RocketAutoLogin"; }
        }

        // In the Init function, register for HttpApplication 
        // events by adding your handlers.
        public void Init(HttpApplication application)
        {
            application.BeginRequest +=
                (new EventHandler(this.Application_BeginRequest));

        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            var logincode = DNNrocketUtils.RequestParam(context, "autologin");
            if (logincode != null && logincode != "")
            {
                LogUtils.LogSystem("logincode: " + logincode);

                var info = DNNrocketUtils.GetTempStorage(logincode);
                if (info != null)
                {
                    var username = info.GetXmlProperty("genxml/username");
                    var useremail = info.GetXmlProperty("genxml/useremail");
                    var userhostaddress = info.GetXmlProperty("genxml/userhostaddress");
                    var newportal = info.GetXmlPropertyInt("genxml/hidden/newportal");
                    var portalSetting = PortalUtils.GetPortalSettings(newportal);

                    UserUtils.UserLogin(newportal, portalSetting.PortalName, userhostaddress, username, false);
                }
            }
        }

        public void Dispose() { }
    }
}