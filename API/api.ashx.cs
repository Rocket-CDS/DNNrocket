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
using System.Net;

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

                    var paramCmd = context.Request.QueryString["cmd"];

                    _editlang = DNNrocketUtils.GetEditCulture();

                    if (paramCmd == "login_signout")
                    {
                        var ps = new PortalSecurity();
                        ps.SignOut();
                        strOut = UserUtils.LoginForm(new SimplisityInfo(), new SimplisityInfo(),"login", UserUtils.GetCurrentUserId());
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(strOut);
                        context.Response.End();
                    }

                    var requestJson = "";
                    var paramJson = "";

                    var paramInfo = new SimplisityInfo(_editlang);
                    if (DNNrocketUtils.RequestParam(context, "paramjson") != "")
                    {
                        paramJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "paramjson"));
                        paramInfo = SimplisityJson.GetSimplisityInfoFromJson(paramJson, _editlang);
                        paramInfo.RemoveXmlNode("genxml/postform/paramjson");
                        paramInfo.RemoveXmlNode("genxml/postform/inputjson");
                        paramInfo.PortalId = DNNrocketUtils.GetPortalId();
                    }

                    // Add any url params (uncoded)
                    foreach (String key in context.Request.QueryString.Keys)
                    {
                        try
                        {
                            paramInfo.SetXmlProperty("genxml/urlparams/" + key.Replace("_", "-"), GeneralUtils.DeCode(context.Request.QueryString[key]));
                        }
                        catch (Exception)
                        {
                            // it might not be coded. (ignore and use genxml/urlparams/* xpath)
                            paramInfo.SetXmlProperty("genxml/urlparams/" + key.Replace("_", "-"), context.Request.QueryString[key]);
                        }
                    }
                    foreach (string key in context.Request.Form)
                    {
                        paramInfo.SetXmlProperty("genxml/postform/" + key.Replace("_", "-"), context.Request.Form[key]); // remove '_' from xpath
                    }


                    var systemkey = paramInfo.GetXmlProperty("genxml/urlparams/systemkey").Trim(' ');
                    if (systemkey == "") systemkey = paramInfo.GetXmlProperty("genxml/hidden/systemkey").Trim(' ');
                    if (systemkey == "" && paramCmd.Contains("_")) systemkey = paramCmd.Split('_')[0];
                    if (systemkey == "") systemkey = "dnnrocket";
                    var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemkey);
                    var systemData = new SystemData(systemInfo);

                    if (paramCmd == "admin_return")
                    {
                        var moduleid = paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                        if (moduleid == 0) moduleid = paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
                        // we need to clear the tracking of commands on return to view.
                        // This command is usually called from "MenuOut.cshtml" and triggers the "returnclick()" function.
                        var userStorage = new UserStorage();
                        userStorage.ModuleId = moduleid;  // use moduleid for tracking to stop mized content on modules.
                        userStorage.TrackClear(systemkey);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("OK");
                        context.Response.End();
                    }

                    var postInfo = new SimplisityInfo(_editlang);
                    if (DNNrocketUtils.RequestParam(context, "inputjson") != "")
                    {
                        requestJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputjson"));

                        // ---- START: DEBUG POST ------
                        var debugSystemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemkey);
                        if (debugSystemInfo != null && debugSystemInfo.GetXmlPropertyBool("genxml/checkbox/debugmode"))
                        {
                            DNNrocketUtils.LogDebug("===== requestJson =====" + Environment.NewLine + requestJson);
                        }
                        // ---- END: DEBUG POST ------

                        postInfo = SimplisityJson.GetSimplisityInfoFromJson(requestJson, _editlang);
                        postInfo.PortalId = DNNrocketUtils.GetPortalId();

                        // ---- START: DEBUG POST ------
                        if (debugSystemInfo != null && debugSystemInfo.GetXmlPropertyBool("genxml/checkbox/debugmode"))
                        {
                            DNNrocketUtils.LogDebug("===== postInfo.XMLData =====" + Environment.NewLine + postInfo.XMLData);
                        }
                        // ---- END: DEBUG POST ------

                    }



                    var interfacekey = paramInfo.GetXmlProperty("genxml/hidden/interfacekey");
                    if (interfacekey == "") interfacekey = paramInfo.GetXmlProperty("genxml/urlparams/interfacekey").Trim(' ');
                    if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];

                    paramInfo.SetXmlProperty("genxml/systemkey", systemkey);

                    if (paramCmd == "login_doregister")
                    {
                        strOut = UserUtils.RegisterUser(postInfo, DNNrocketUtils.GetCurrentCulture());
                        if (strOut == "") UserUtils.DoLogin(systemInfo, postInfo, HttpContext.Current.Request.UserHostAddress);
                    }
                    else if (paramCmd == "login_register")
                    {
                        strOut = UserUtils.RegisterForm(systemInfo, postInfo, interfacekey, UserUtils.GetCurrentUserId());
                    }
                    else if (paramCmd == "login_login")
                    {
                        UserUtils.DoLogin(systemInfo, postInfo, HttpContext.Current.Request.UserHostAddress);
                        strOut = ""; // the page will rteload after the call
                    }
                    else
                    {
                        switch (paramCmd)
                        {
                        case "getsidemenu":
                            strOut = GetSideMenu(paramInfo, systemkey);
                            break;
                        default:
                            var rocketInterface = new DNNrocketInterface(systemInfo, interfacekey);
                            var returnDictionary = new Dictionary<string, string>();
                                
                            // before event
                            var rtnDictInfo = DNNrocketUtils.EventProviderBefore(paramCmd, systemData, postInfo, paramInfo, _editlang);
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
                                    DownloadFile(returnDictionary["filenamepath"], returnDictionary["downloadname"]);
                                }
                                if (returnDictionary.ContainsKey("downloadfiledata"))
                                {
                                    DownloadStringAsFile(returnDictionary["downloadfiledata"], returnDictionary["downloadname"]);
                                }
                                if (returnDictionary.ContainsKey("outputjson"))
                                {
                                    strJson = returnDictionary["outputjson"];
                                }

                            }
                            else
                            {
                                // check for systemspi, does not exist.  It's used to create the systemprovders 
                                if (systemkey == "" || systemkey == "systemapi" || systemkey == "login")
                                {
                                    var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.System.StartConnect");
                                    returnDictionary = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, postInfo, paramInfo, _editlang);
                                    strOut = returnDictionary["outputhtml"];
                                }
                                else
                                {
                                    strOut = "ERROR: Invalid systemkey: " + systemkey + "  interfacekey: " + interfacekey + " cmd: " + paramCmd + " - Check Database for SYSTEM,'" + systemkey + "' (No spaces) - Check 'simplisity_startpanel' and 'simplisity_panel' for correct s-cmd.  ";
                                }
                            }

                            // after Event
                            returnDictionary = DNNrocketUtils.EventProviderAfter(paramCmd, systemData, postInfo, paramInfo, _editlang);
                            if (returnDictionary.ContainsKey("outputhtml")) strOut = returnDictionary["outputhtml"];
                            if (returnDictionary.ContainsKey("outputjson")) strJson = returnDictionary["outputjson"];

                            break;
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

        public static string GetSideMenu(SimplisityInfo sInfo, string systemkey)
        {
            try
            {
                var strOut = "";
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = sInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                if (moduleid == 0) moduleid = -1;

                var passSettings = sInfo.ToDictionary();

                var systemDataList = new SystemDataList();
                var sInfoSystem = systemDataList.GetSystemByKey(systemkey);
                var systemData = new SystemData(sInfoSystem);
                var sidemenu = new Componants.SideMenu(sInfoSystem);
                var templateControlRelPath = sInfo.GetXmlProperty("genxml/hidden/relpath");
                sidemenu.ModuleId = moduleid;

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(),"1.0", systemData.DebugMode);

                if (razorTempl == "")
                {
                    // no razor template for sidemenu, so use default.
                    razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, TemplateRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", systemData.DebugMode);
                }

                strOut = DNNrocketUtils.RazorDetail(razorTempl, sidemenu, passSettings, null, systemData.DebugMode);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string DownloadFile(string filenamepath, string downloadname)
        {
            var strOut = "";
            if (filenamepath != "")
            {
                strOut = filenamepath; // return this is error.
                if (downloadname == "") downloadname = Path.GetFileName(filenamepath);
                try
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.ClearContent();
                    response.Clear();
                    response.ContentType = "text/plain";
                    response.AppendHeader("content-disposition", "attachment; filename=" + downloadname);
                    response.TransmitFile(filenamepath);
                    response.Flush();
                    response.End();
                }
                catch (Exception ex)
                {
                    var errmsg = ex.ToString();
                    // ignore, robots can cause error on thread abort.
                }
            }
            return strOut;
        }

        public static string DownloadStringAsFile(string filedata, string downloadname = "")
        {
            var strOut = "";
            if (filedata != "")
            {
                if (downloadname == "") downloadname = "downloadfile.txt";
                try
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.ClearContent();
                    response.Clear();
                    response.ContentType = "text/plain";
                    response.AppendHeader("content-disposition", "attachment; filename=" + downloadname);                    
                    response.Write(filedata);
                    response.Flush();
                    response.End();
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