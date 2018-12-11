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
                _editlang = DNNrocketUtils.GetCurrentCulture();

                var paramCmd = context.Request.QueryString["cmd"];

                var requestXml = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputxml"));
                
                var sInfo = SimplisityUtils.GetSimplisityInfo(requestXml);

                var systemprovider = sInfo.GetXmlProperty("genxml/hidden/systemprovider");
                if (systemprovider == "")
                {
                    systemprovider = DNNrocketUtils.RequestQueryStringParam(context, "systemprovider");
                }


                if (systemprovider == "" || systemprovider == "systemapi")
                {
                    if (UserController.Instance.GetCurrentUserInfo().IsSuperUser)
                    {
                        // By default we prcess the DNNrocketAPI system api.
                        strOut = SystemFunction.ProcessCommand(paramCmd,sInfo, _editlang);
                    }
                    else
                    {
                        strOut = "ERROR: Invalid Security.";
                    }

                }
                else
                {
                    if (systemprovider != "")
                    {
                        // Run API Provider.
                        strOut = "API not found: " + systemprovider;

                        var assembly = sInfo.GetXmlProperty("genxml/textbox/assembly");
                        var namespaceclass = sInfo.GetXmlProperty("genxml/textbox/namespaceclass");
                        var ajaxprov = APInterface.Instance(assembly, namespaceclass);
                        if (ajaxprov != null)
                        {
                            strOut = ajaxprov.ProcessCommand(paramCmd, sInfo, _editlang);
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