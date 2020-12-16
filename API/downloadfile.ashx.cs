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
    public class ProcessDownloadFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var objCtrl = new DNNrocketController();
            var msg = "";
            var fileindex = context.Request.QueryString["fileindex"];
            var itemid = context.Request.QueryString["itemid"];
            var fieldid = context.Request.QueryString["fieldid"];
            if (GeneralUtils.IsNumeric(itemid) && GeneralUtils.IsNumeric(fileindex) && !String.IsNullOrEmpty(fieldid))
            {
                var downloadname = context.Request.QueryString["downloadname"];
                var listname = context.Request.QueryString["listname"];
                if (String.IsNullOrEmpty(listname)) listname = "settingsdata";
                var sInfo = objCtrl.GetInfo(Convert.ToInt32(itemid), DNNrocketUtils.GetCurrentCulture());
                var sInfoItem = sInfo.GetListItem(listname, Convert.ToInt32(fileindex));
                var fpath = sInfoItem.GetXmlProperty("genxml/lang/genxml/hidden/rel" + fieldid);
                if (fpath == "") fpath = sInfoItem.GetXmlProperty("genxml/hidden/rel" + fieldid);
                if (fpath != "")
                {
                    fpath = DNNrocketUtils.MapPath(fpath);
                    if (String.IsNullOrEmpty(downloadname)) downloadname = sInfoItem.GetXmlProperty("genxml/lang/genxml/textbox/name" + fieldid);
                    if (String.IsNullOrEmpty(downloadname)) downloadname = sInfoItem.GetXmlProperty("genxml/textbox/name" + fieldid);
                    if (String.IsNullOrEmpty(downloadname)) downloadname = Path.GetFileName(fpath);
                    DNNrocketUtils.ForceDocDownload(fpath, downloadname, context.Response);
                    msg = " - Cannot find: " + fpath;
                }
                else
                {
                    msg = "File Download Error, no data found for '" + fieldid + "'";
                }
            }
            var strOut = "File Download Error, itemid: " + itemid + ", fileindex: " + fileindex + " Message:" + msg;

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