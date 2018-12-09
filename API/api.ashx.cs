using NBrightCore.common;
using NBrightCore.render;
using NBrightDNN;
using System;
using System.Web;
using Simplisity;
using DotNetNuke.Entities.Users;
using DNNrocketAPI.Interfaces;

namespace DNNrocketAPI
{
    public class ProcessAPI : IHttpHandler
    {
        private String _editlang = "";

        public void ProcessRequest(HttpContext context)
        {
            var strOut = "ERROR: Invalid.";
            try
            {
                var paramCmd = context.Request.QueryString["cmd"];

                var requestXml = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputxml"));
                
                var sInfo = SimplisityUtils.GetSimplisityInfo(requestXml);

                var ajaxprovider = sInfo.GetXmlProperty("genxml/hidden/ajaxprovider");
                if (ajaxprovider == "")
                {
                    ajaxprovider = NBrightCore.common.Utils.RequestQueryStringParam(context, "ajaxprovider");
                }

                if (ajaxprovider == "" || ajaxprovider == "systemapi")
                {
                    if (UserController.Instance.GetCurrentUserInfo().IsSuperUser)
                    {
                        switch (paramCmd)
                        {
                            case "systemapi_admin_getsystemlist":
                                strOut = SystemFunction.SystemAdminList(sInfo);
                                break;
                        }
                    }
                    else
                    {
                        strOut = "ERROR: Invalid Security.";
                    }

                }
                else
                {


                    var pluginData = new PluginData(0);
                    var provList = pluginData.GetAjaxProviders();
                    if (ajaxprovider != "")
                    {
                        strOut = "API not found: " + ajaxprovider;
                        if (provList.ContainsKey(ajaxprovider))
                        {
                            var ajaxprov = AjaxInterface.Instance(ajaxprovider);
                            if (ajaxprov != null)
                            {
                                strOut = ajaxprov.ProcessCommand(paramCmd, context, _editlang);
                            }
                        }
                    }
                    else
                    {
                        strOut = "Unknown ajaxprovider: " + ajaxprovider;
                        foreach (var d in provList)
                        {
                            if (paramCmd.ToLower().StartsWith(d.Key.ToLower() + "_") || paramCmd.ToLower().StartsWith("cmd" + d.Key.ToLower() + "_"))
                            {
                                var ajaxprov = AjaxInterface.Instance(d.Key);
                                if (ajaxprov != null)
                                {
                                    strOut = ajaxprov.ProcessCommand(paramCmd, context, _editlang);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                strOut = ex.ToString();
            }


            #region "return results"

            //send back xml as plain text
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            context.Response.Write(strOut);
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


    }
}