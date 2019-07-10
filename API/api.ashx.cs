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

namespace DNNrocketAPI
{
    public class ProcessAPI : IHttpHandler
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

                // Do file upload is this is a file upload request.
                if (context.Request.Files.Count > 0)
                {
                    var fileout = DNNrocketUtils.FileUpload(context);
                }
                else
                {
                    _editlang = DNNrocketUtils.GetEditCulture();

                    var paramCmd = context.Request.QueryString["cmd"];

                    if (paramCmd == "login_signout")
                    {
                        var ps = new PortalSecurity();
                        ps.SignOut();
                        strOut = LoginUtils.LoginForm(new SimplisityInfo(), new SimplisityInfo(),"login", UserUtils.GetCurrentUserId());
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strOut);
                        context.Response.End();
                    }

                    var requestJson = "";
                    var paramJson = "";

                    var paramInfo = new SimplisityInfo(DNNrocketUtils.GetEditCulture());
                    if (DNNrocketUtils.RequestParam(context, "paramjson") != "")
                    {
                        paramJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "paramjson"));
                        paramInfo = SimplisityJson.GetSimplisityInfoFromJson(paramJson, _editlang);
                        paramInfo.RemoveXmlNode("genxml/postform/paramjson");
                        paramInfo.RemoveXmlNode("genxml/postform/inputjson");
                        paramInfo.PortalId = DNNrocketUtils.GetPortalId();
                    }

                    var systemprovider = paramInfo.GetXmlProperty("genxml/hidden/systemprovider").Trim(' ');
                    if (systemprovider == "") systemprovider = "dnnrocket";
                    var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemprovider);
                    var systemInfoData = new SystemInfoData(systemInfo); 

                    var postInfo = new SimplisityInfo(DNNrocketUtils.GetEditCulture());
                    if (DNNrocketUtils.RequestParam(context, "inputjson") != "")
                    {
                        requestJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputjson"));

                        // ---- START: DEBUG POST ------
                        var debugSystemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemprovider);
                        if (debugSystemInfo != null && debugSystemInfo.GetXmlPropertyBool("genxml/checkbox/debugmode"))
                        {
                            FileUtils.SaveFile(PortalSettings.Current.HomeDirectoryMapPath + "\\debug_requestJson.json", requestJson);
                        }
                        // ---- END: DEBUG POST ------

                        postInfo = SimplisityJson.GetSimplisityInfoFromJson(requestJson, _editlang);
                        postInfo.PortalId = DNNrocketUtils.GetPortalId();

                        // ---- START: DEBUG POST ------
                        if (debugSystemInfo != null && debugSystemInfo.GetXmlPropertyBool("genxml/checkbox/debugmode"))
                        {
                            FileUtils.SaveFile(PortalSettings.Current.HomeDirectoryMapPath + "\\debug_postInfo.xml", postInfo.XMLData);
                        }
                        // ---- END: DEBUG POST ------

                    }

                    // Add any url params (uncoded)
                    foreach (String key in context.Request.QueryString.Keys)
                    {
                        try
                        {
                            paramInfo.SetXmlProperty("genxml/urlparams/" + key.Replace("_", "-"), GeneralUtils.DeCode(context.Request.QueryString[key]));
                        }
                        catch (Exception ex)
                        {
                            // it might not be coded. (ignore and use genxml/urlparams/* xpath)
                            paramInfo.SetXmlProperty("genxml/urlparams/" + key.Replace("_", "-"), context.Request.QueryString[key]);
                        }
                    }
                    foreach (string key in context.Request.Form)
                    {
                        paramInfo.SetXmlProperty("genxml/postform/" + key.Replace("_","-"), context.Request.Form[key]); // remove '_' from xpath
                    }


                    var interfacekey = paramInfo.GetXmlProperty("genxml/hidden/interfacekey");
                    if (interfacekey == "") interfacekey = paramInfo.GetXmlProperty("genxml/urlparams/interfacekey").Trim(' ');
                    if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];

                    paramInfo.SetXmlProperty("genxml/systemprovider", systemprovider);

                    if (paramCmd == "login_login")
                    {
                        LoginUtils.DoLogin(systemInfo, postInfo, HttpContext.Current.Request.UserHostAddress);
                        strOut = ""; // the page will rteload after the call
                    }
                    else
                    {
                        switch (paramCmd)
                        {
                        case "getsidemenu":
                            strOut = GetSideMenu(paramInfo, systemprovider);
                            break;
                        default:
                            var rocketInterface = new DNNrocketInterface(systemInfo, interfacekey);
                            var returnDictionary = new Dictionary<string, string>();
                                
                            // before event
                            var rtnDictInfo = DNNrocketUtils.EventProviderBefore(paramCmd, systemInfoData, postInfo, paramInfo, _editlang);
                            if (rtnDictInfo.ContainsKey("post")) postInfo = rtnDictInfo["post"];
                            if (rtnDictInfo.ContainsKey("param")) paramInfo = rtnDictInfo["param"];

                            // command action
                            if (rocketInterface.Exists)
                            {
                                returnDictionary = DNNrocketUtils.GetProviderReturn(paramCmd, systemInfo, rocketInterface, postInfo, paramInfo, TemplateRelPath, _editlang);

                                if (returnDictionary.ContainsKey("outputhtml"))
                                {
                                    strOut = returnDictionary["outputhtml"];
                                }
                                if (returnDictionary.ContainsKey("filenamepath"))
                                {
                                    if (!returnDictionary.ContainsKey("downloadname")) returnDictionary["downloadname"] = "";
                                    if (!returnDictionary.ContainsKey("fileext")) returnDictionary["fileext"] = "";
                                    DownloadFile(context, returnDictionary["filenamepath"], returnDictionary["downloadname"], returnDictionary["fileext"]);
                                }
                                if (returnDictionary.ContainsKey("outputjson"))
                                {
                                    strJson = returnDictionary["outputjson"];
                                }

                            }
                            else
                            {
                                // check for systemspi, does not exist.  It's used to create the systemprovders 
                                if (systemprovider == "" || systemprovider == "systemapi" || systemprovider == "login")
                                {
                                    var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.SystemData.StartConnect");
                                    returnDictionary = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, postInfo, paramInfo, _editlang);
                                    strOut = returnDictionary["outputhtml"];
                                }
                                else
                                {
                                    strOut = "ERROR: Invalid SystemProvider: " + systemprovider + "  interfacekey: " + interfacekey + " - Check Database for SYSTEM,'" + systemprovider + "' (No spaces)";
                                }
                            }

                            // after Event
                            returnDictionary = DNNrocketUtils.EventProviderAfter(paramCmd, systemInfoData, postInfo, paramInfo, _editlang);
                            if (returnDictionary.ContainsKey("outputhtml")) strOut = returnDictionary["outputhtml"];
                            if (returnDictionary.ContainsKey("outputjson")) strJson = returnDictionary["outputjson"];

                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                strOut = ex.ToString();
            }


            #region "return results"

            context.Response.Clear();

            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

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