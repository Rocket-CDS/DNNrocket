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
using DNNrocketAPI.Componants;
using System.Collections.Specialized;
using System.Text;

namespace DNNrocketAPI
{
    public class ProcessAPI2 : IHttpHandler
    {
        private String _editlang = "";
        public static string TemplateRelPath = "/DesktopModules/DNNrocket/api";

        public void ProcessRequest(HttpContext context)
        {
            var strOut = "ERROR: Invalid.";
            var strJson = "";

            try
            {
                var objCtrl = new DNNrocketController();

                _editlang = DNNrocketUtils.GetEditCulture();

                var paramCmd = context.Request.QueryString["cmd"];

                var postInfo = new SimplisityInfo();
                postInfo.SetXmlProperty("genxml/hidden", "");


                // Add any url params (uncoded)
                foreach (String key in context.Request.QueryString.Keys)
                {
                    postInfo.SetXmlProperty("genxml/urlparams/" + key, context.Request.QueryString[key]);
                }

                NameValueCollection requestForm = context.Request.Form;
                foreach (string key in requestForm)
                {
                    postInfo.SetXmlProperty("genxml/postform/" + key, requestForm[key]);
                }

                var param = context.Request.BinaryRead(context.Request.ContentLength);
                var strRequest = Encoding.ASCII.GetString(param);
                postInfo.SetXmlProperty("genxml/requestcontent", strRequest);


                var systemprovider = postInfo.GetXmlProperty("genxml/urlparams/systemprovider").Trim(' ');
                if (systemprovider == "") systemprovider = postInfo.GetXmlProperty("genxml/systemprovider");
                if (systemprovider == "") systemprovider = "dnnrocket";

                var interfacekey = postInfo.GetXmlProperty("genxml/urlparams/interfacekey");
                if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];

                postInfo.SetXmlProperty("genxml/systemprovider", systemprovider);

                var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemprovider);
                var rocketInterface = new DNNrocketInterface(systemInfo, interfacekey);

                if (rocketInterface.Exists)
                {
                    var returnDictionary = DNNrocketUtils.GetProviderReturn(paramCmd, systemInfo, rocketInterface, postInfo, TemplateRelPath, _editlang);

                    if (returnDictionary.ContainsKey("outputhtml"))
                    {
                        strOut = returnDictionary["outputhtml"];
                    }
                    if (returnDictionary.ContainsKey("outputjson"))
                    {
                        strJson = returnDictionary["outputjson"];
                    }

                }
            }
            catch (Exception ex)
            {
                strOut = ex.ToString();
            }


            #region "return results"

            context.Response.Clear();
            if (strJson != "")
            {
                //send back xml as plain text
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.Write(JsonConvert.SerializeObject(strJson));
            }
            else
            {
                //send back xml as plain text
                context.Response.ContentType = "text/plain";
                context.Response.Write(strOut);
            }
            context.Response.End();


            #endregion

        }

        public bool IsReusable
        {
            get
            {
                return false;

            }
        }

        public static string GetSideMenu(SimplisityInfo sInfo, string systemprovider)
        {
            try
            {
                var strOut = "";
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = sInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                if (moduleid == 0) moduleid = -1;

                var passSettings = sInfo.ToDictionary();

                var systemData = new SystemData();
                var sInfoSystem = systemData.GetSystemByKey(systemprovider);
                var sidemenu = new Componants.SideMenu(sInfoSystem);
                var templateControlRelPath = sInfo.GetXmlProperty("genxml/hidden/relpath");
                sidemenu.ModuleId = moduleid;

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());

                if (razorTempl == "")
                {
                    // no razor template for sidemenu, so use default.
                    razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, TemplateRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
                }

                strOut = DNNrocketUtils.RazorDetail(razorTempl, sidemenu, passSettings);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string DownloadFile(HttpContext context, string filenamepath, string downloadname, string fileext)
        {
            var strOut = "";
            if (filenamepath != "")
            {
                strOut = filenamepath; // return this is error.
                if (downloadname == "") downloadname = Path.GetFileNameWithoutExtension(filenamepath) + fileext;
                try
                {
                    context.Response.Clear();
                    context.Response.AppendHeader("content-disposition", "attachment; filename=" + downloadname);
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.WriteFile(filenamepath);
                    context.Response.End();
                }
                catch (Exception ex)
                {
                    var errmsg = ex.ToString();
                    // ignore, robots can cause error on thread abort.
                }
            }
            return strOut;
        }



    }
}