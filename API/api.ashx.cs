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
using DNNrocket.Login;

namespace DNNrocketAPI
{
    public class ProcessAPI : IHttpHandler
    {
        private String _editlang = "";
        public static string TemplateRelPath = "/DesktopModules/DNNrocket/api";

        public void ProcessRequest(HttpContext context)
        {
            var strOut = "ERROR: Invalid.";
            try
            {
                _editlang = DNNrocketUtils.GetEditCulture();

                var paramCmd = context.Request.QueryString["cmd"];

                //var requestXml = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputxml"));                
                //var sInfo = SimplisityUtils.GetSimplisityInfo(requestXml);

                var requestJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputjson"));
                var sInfoJson = SimplisityJson.GetSimplisityInfoFromJson(requestJson, _editlang);

                var systemprovider = sInfoJson.GetXmlProperty("genxml/hidden/systemprovider");
                if (systemprovider == "") systemprovider = DNNrocketUtils.RequestQueryStringParam(context, "systemprovider");
                var interfacekey = sInfoJson.GetXmlProperty("genxml/hidden/interfacekey");
                if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];

                // does NOT work across portals.
                CacheUtils.SetCache("debugOutMapPath", PortalSettings.Current.HomeDirectoryMapPath);
                //--------------------------

                if (paramCmd == "getsidemenu")
                {
                    strOut = GetSideMenu(sInfoJson, systemprovider);
                }
                else
                {

                    if (systemprovider == "" || systemprovider == "systemapi")
                    {
                        strOut = SystemFunction.ProcessCommand(paramCmd, sInfoJson, _editlang);
                    }
                    else
                    {
                        if (systemprovider != "")
                        {
                            // Run API Provider.
                            strOut = "API not found: " + systemprovider;
                            var objCtrl = new DNNrocketController();

                            var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemprovider);
                            if (systemInfo != null)
                            {
                                var systemRecord = new SystemRecord(systemInfo);
                                var iface = systemRecord.GetInterface(interfacekey);
                                if (iface != null)
                                {

                                    var assembly = iface.GetXmlProperty("genxml/textbox/assembly");
                                    var namespaceclass = iface.GetXmlProperty("genxml/textbox/namespaceclass");
                                    if (assembly == "" || namespaceclass == "")
                                    {
                                        strOut = "No assembly or namespaceclass defined: " + systemprovider + " : " + assembly + "," + namespaceclass;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var ajaxprov = APInterface.Instance(assembly, namespaceclass);
                                            strOut = ajaxprov.ProcessCommand(paramCmd, sInfoJson, _editlang);
                                        }
                                        catch (Exception ex)
                                        {
                                            strOut = "No valid assembly found: " + systemprovider + " : " + assembly + "," + namespaceclass;
                                        }
                                    }
                                }
                                else
                                {
                                    strOut = "interfacekey not found: " + interfacekey;
                                }
                            }
                            else
                            {
                                strOut = "No valid system found: " + systemprovider;
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

        public static string GetSideMenu(SimplisityInfo sInfo, string systemprovider)
        {
            try
            {
                var strOut = "";
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");

                var passSettings = sInfo.ToDictionary();

                var systemData = new SystemData();
                var sInfoSystem = systemData.GetSystemByKey(systemprovider);
                var systemRecord = new SystemRecord(sInfoSystem);
                var sidemenu = new Componants.SideMenu(systemRecord);
                var templateControlRelPath = systemRecord.Info().GetXmlProperty("genxml/textbox/relpath");

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

    }
}