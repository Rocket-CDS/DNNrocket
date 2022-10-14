using System;
using System.Web;
using Simplisity;
using DotNetNuke.Entities.Users;
using DNNrocketAPI.Interfaces;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security;
using DotNetNuke.Services.Mail;
using DotNetNuke.Entities.Users.Membership;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using DNNrocketAPI.Components;
using System.Collections.Specialized;
using System.Text;

namespace DNNrocketAPI.Components
{
    public class ProcessDownloadAuthFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var msg = "";
            var fname = context.Request.QueryString["ref"];
            var downloadname = context.Request.QueryString["downloadname"];
            var publicaccess = Convert.ToBoolean(context.Request.QueryString["public"]);
            if (!publicaccess && !UserUtils.IsAuthorised()) msg = "Access Denied";
            if (msg == "")
            {
                var fpath = PortalUtils.TempDirectoryMapPath() + "\\" + fname;
                if (File.Exists(fpath))
                {
                    if (String.IsNullOrEmpty(downloadname)) downloadname = Path.GetFileName(fpath);
                    DNNrocketUtils.ForceDocDownload(fpath, downloadname, context.Response);
                }
                else
                {
                    msg = "File Download Error, no data found.";
                }
            }
            else
            {
                msg = "Invalid Data Record";
            }

            var strOut = "File Download Error, Message:" + msg;
            context.Response.ContentType = "text/plain";
            context.Response.Write(strOut);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }
}