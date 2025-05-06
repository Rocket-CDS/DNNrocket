using System;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Web.MvcPipeline.Models;

namespace DNNrocket.MVC
{
    public static class RocketSkinHelper
    {
        public static MvcHtmlString Testing() 
        {
            return new MvcHtmlString("<h1>testing RocketSkinHelper</h1>"); 
        }
    }
}
