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

                // -------------------------------------------------------------
                // -------------- OUTPUT TEST DATA -----------------------------
                // -------------------------------------------------------------
                // does NOT work across portals.
                //CacheUtils.SetCache("debugOutMapPath", PortalSettings.Current.HomeDirectoryMapPath);
                // -------------------------------------------------------------
                // -------------- OUTPUT TEST DATA -----------------------------
                // -------------------------------------------------------------

                var requestJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputjson"));
                var sInfoJson = SimplisityJson.GetSimplisityInfoFromJson(requestJson, _editlang);

                // -------------------------------------------------------------
                // -------------- OUTPUT TEST DATA -----------------------------
                // -------------------------------------------------------------
                //FileUtils.SaveFile(PortalSettings.Current.HomeDirectoryMapPath + @"\requestJson.xml", requestJson);
                //FileUtils.SaveFile(PortalSettings.Current.HomeDirectoryMapPath + @"\sInfoJson.xml", sInfoJson.XMLData);

                //--------------------------

                // -------------------------------------------------------------
                // -------------- OUTPUT TEST DATA -----------------------------
                // -------------------------------------------------------------


                var systemprovider = sInfoJson.GetXmlProperty("genxml/hidden/systemprovider");
                if (systemprovider == "") systemprovider = DNNrocketUtils.RequestQueryStringParam(context, "systemprovider");
                var interfacekey = sInfoJson.GetXmlProperty("genxml/hidden/interfacekey");
                if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];


                if (paramCmd == "getsidemenu")
                {
                    strOut = GetSideMenu(sInfoJson, systemprovider);
                }
                else
                {

                    var objCtrl = new DNNrocketController();
                    var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemprovider);

                    if (systemprovider == "" || systemprovider == "systemapi")
                    {
                        var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.SystemData.startconnect");
                        strOut = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, sInfoJson, HttpContext.Current.Request.UserHostAddress, _editlang);
                    }
                    else
                    {
                        if (systemprovider != "")
                        {
                            // Run API Provider.
                            strOut = "API not found: " + systemprovider;
                            if (systemInfo != null)
                            {
                                var interfaceInfo = systemInfo.GetListItem("interfacedata", "genxml/textbox/interfacekey", interfacekey);                                
                                if (interfaceInfo != null)
                                {

                                    var assembly = interfaceInfo.GetXmlProperty("genxml/textbox/assembly");
                                    var namespaceclass = interfaceInfo.GetXmlProperty("genxml/textbox/namespaceclass");
                                    if (assembly == "" || namespaceclass == "")
                                    {
                                        strOut = "No assembly or namespaceclass defined: " + systemprovider + " : " + assembly + "," + namespaceclass;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var ajaxprov = APInterface.Instance(assembly, namespaceclass);
                                            strOut = ajaxprov.ProcessCommand(paramCmd, systemInfo, interfaceInfo, sInfoJson, HttpContext.Current.Request.UserHostAddress, _editlang);
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
                var sidemenu = new Componants.SideMenu(sInfoSystem);
                var templateControlRelPath = sInfoSystem.GetXmlProperty("genxml/textbox/relpath");

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