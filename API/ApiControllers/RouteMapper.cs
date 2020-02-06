using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using DotNetNuke.Web.Api;

namespace DNNrocketAPI
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("dnnrocket", "default", "{controller}/{action}", new[] { "DNNrocketAPI.ApiControllers" });
        }
    }
}
