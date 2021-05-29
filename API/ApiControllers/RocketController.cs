using DNNrocketAPI.Components;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using Simplisity;
using System;  
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;  
using System.Net;  
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;

namespace DNNrocketAPI.ApiControllers
{

    public class RocketController : DnnApiController
    {
        private String _editlang = "";
        public static string TemplateRelPath = "/DesktopModules/DNNrocket/api";

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage GetTest()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, "Test API2");
        }

        /// <summary>
        /// Normal Endpoint for Simplisity
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Action()
        {
            var context = HttpContext.Current;

            if (!context.Request.QueryString.AllKeys.Contains("cmd"))
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, "No 'cmd' parameter in url.  Unable to process action.");
            }

            var paramCmd = context.Request.QueryString["cmd"];

            var systemkey = "";
            if (context.Request.QueryString.AllKeys.Contains("systemkey")) systemkey = context.Request.QueryString["systemkey"];
            if (systemkey == "" && context.Request.QueryString.AllKeys.Contains("s")) systemkey = context.Request.QueryString["s"]; // reduce chars.

            var postInfo = BuildPostInfo();
            var paramInfo = BuildParamInfo();

            var systemData = new SystemLimpet(systemkey);
            var interfacekey = paramCmd.Split('_')[0];
            var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
            TrackCmd(paramCmd, systemData, rocketInterface, paramInfo);

            var rtn = new HttpResponseMessage();
            var strOut = ProcessAction(postInfo, paramInfo, paramCmd, systemkey);
            if (strOut == "")
            {
                // no admin actions processed, so do normal provider command.
                rtn = ProcessProvider(paramCmd, postInfo, paramInfo, systemData, rocketInterface);
            }
            else
            {
                rtn = this.Request.CreateResponse(HttpStatusCode.OK, strOut, "text/plain");
            }


            if (rtn.Headers.Contains("Access-Control-Allow-Origin")) rtn.Headers.Remove("Access-Control-Allow-Origin");
            rtn.Headers.Add("Access-Control-Allow-Origin", "*");
            return rtn;
        }
        /// <summary>
        /// Endpoint for Simplisity when we only want to deal with the Request Content
        /// Puts the content into a field "genxml/requestcontent".
        /// This is because we cannot pass the context to .Net Standard modules
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage ActionContent()
        {
            var context = HttpContext.Current;

            if (!context.Request.QueryString.AllKeys.Contains("cmd"))
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, "No 'cmd' parameter in url.  Unable to process action.");
            }

            var paramCmd = context.Request.QueryString["cmd"];

            var systemkey = "";
            if (context.Request.QueryString.AllKeys.Contains("systemkey")) systemkey = context.Request.QueryString["systemkey"];
            if (systemkey == "" && context.Request.QueryString.AllKeys.Contains("s")) systemkey = context.Request.QueryString["s"]; // reduce chars.

            var postInfo = BuildPostInfo();
            var paramInfo = BuildParamInfo(true);

            var systemData = new SystemLimpet(systemkey);
            var interfacekey = paramCmd.Split('_')[0];
            var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
            var rtn = ProcessProvider(paramCmd, postInfo, paramInfo, systemData, rocketInterface);

            if (rtn.Headers.Contains("Access-Control-Allow-Origin")) rtn.Headers.Remove("Access-Control-Allow-Origin");
            rtn.Headers.Add("Access-Control-Allow-Origin", "*");
            return rtn;
        }
        /// <summary>
        /// When a remote module or html passes base64 paramInfo.
        /// Usually on a server to server call, on module first load.
        /// **NOTE: ActionRemote does NOT update data from the postInfo.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage ActionRemote()
        {
            var context = HttpContext.Current;

            if (!context.Request.QueryString.AllKeys.Contains("cmd"))
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, "No 'cmd' parameter in url.  Unable to process action.");
            }

            string rawData = Request.Content.ReadAsStringAsync().Result;
            if (rawData == "") return this.Request.CreateResponse(HttpStatusCode.OK, "No Data to process");

            var paramInfo = new SimplisityInfo();
            paramInfo.FromXmlItem(GeneralUtils.Base64Decode(rawData));
            var SystemKey = paramInfo.GetXmlProperty("genxml/settings/remotesystemkey");
            if (SystemKey == "") return this.Request.CreateResponse(HttpStatusCode.OK, "RemoteSystemKey not found");

            var paramCmd = context.Request.QueryString["cmd"];
            var portalId = PortalUtils.GetPortalId();
            paramInfo.PortalId = portalId;

            var systemData = new SystemLimpet(SystemKey);
            var interfacekey = paramCmd.Split('_')[0];
            var rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
            var rtn = ProcessProvider(paramCmd, new SimplisityInfo(), paramInfo, systemData, rocketInterface);

            if (rtn.Headers.Contains("Access-Control-Allow-Origin")) rtn.Headers.Remove("Access-Control-Allow-Origin");
            rtn.Headers.Add("Access-Control-Allow-Origin", "*");
            return rtn;
        }

        private SimplisityInfo BuildPostInfo()
        {
            var context = HttpContext.Current;
            var requestJson = "";
            var postInfo = new SimplisityInfo(_editlang);
            if (DNNrocketUtils.RequestParam(context, "inputjson") != "")
            {
                requestJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "inputjson"));
                postInfo = SimplisityJson.GetSimplisityInfoFromJson(requestJson, _editlang);
                postInfo.PortalId = PortalUtils.GetPortalId();
            }
            return postInfo;
        }

        private SimplisityInfo BuildParamInfo(bool requestContent = false)
        {
            var context = HttpContext.Current;

            var paramJson = "";
            var paramInfo = new SimplisityInfo(_editlang);
            if (DNNrocketUtils.RequestParam(context, "paramjson") != "")
            {
                paramJson = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "paramjson"));
                paramInfo = SimplisityJson.GetSimplisityInfoFromJson(paramJson, _editlang);
                paramInfo.PortalId = PortalUtils.GetPortalId();
            }

            // get all query string params
            foreach (string key in context.Request.QueryString.AllKeys)
            {
                var keyValue = context.Request.QueryString[key];
                paramInfo.SetXmlProperty("genxml/urlparams/" + key.ToLower(), keyValue);
            }
            // get all form data (drop the ones we already processed) 
            foreach (string key in context.Request.Form.AllKeys)
            {
                if (key.ToLower() != "paramjson" && key.ToLower() != "inputjson")
                {
                    var keyValue = DNNrocketUtils.RequestParam(context, key);
                    paramInfo.SetXmlProperty("genxml/form/" + key.ToLower(), keyValue);
                }
            }

            if (requestContent)
            {
                // put the content into a field. Usually used for validation of the request.
                // (WE CANNOT PASS CONTEXT TO .NET STANDARD)
                var requestBinaryContent = context.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
                var requestStringContent = Encoding.ASCII.GetString(requestBinaryContent);
                paramInfo.SetXmlProperty("genxml/requestcontent", requestStringContent);
            }

            return paramInfo;
        }

        private RocketInterface TrackCmd(string paramCmd, SystemLimpet systemData, RocketInterface rocketInterface, SimplisityInfo paramInfo)
        {
            var sessionParams = new SessionParams(paramInfo);
            var userParams = new UserParams(sessionParams.BrowserSessionId);
            if (paramInfo.GetXmlPropertyBool("genxml/hidden/reload"))
            {
                var menucmd = userParams.GetCommand(systemData.SystemKey);
                if (menucmd != "")
                {
                    paramCmd = menucmd;
                    paramInfo = userParams.GetParamInfo(systemData.SystemKey);
                    var interfacekey = userParams.GetInterfaceKey(systemData.SystemKey);
                    rocketInterface = new RocketInterface(systemData.SystemInfo, interfacekey);
                }
            }
            else
            {
                if (paramInfo.GetXmlPropertyBool("genxml/hidden/track")) userParams.Track(systemData.SystemKey, paramCmd, paramInfo, rocketInterface.InterfaceKey);
            }
            return rocketInterface;
        }
        /// <summary>
        /// Admin Actions for Action Endpoint [TODO: rewrite to be cleaner]
        /// </summary>
        /// <param name="postInfo"></param>
        /// <param name="paramInfo"></param>
        /// <param name="paramCmd"></param>
        /// <param name="systemkey"></param>
        /// <returns></returns>
        private string ProcessAction(SimplisityInfo postInfo, SimplisityInfo paramInfo, string paramCmd, string systemkey)
        {
            var strOut = "ERROR: Invalid.";
            var context = HttpContext.Current;

            try
            {
                var objCtrl = new DNNrocketController();

                _editlang = DNNrocketUtils.GetEditCulture();

                if (paramCmd == "login_signout")
                {
                    var ps = new PortalSecurity();
                    ps.SignOut();
                    strOut = UserUtils.LoginForm(new SimplisityInfo(), new SimplisityInfo(), "login", UserUtils.GetCurrentUserId());
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(strOut);
                    context.Response.End();
                }

                systemkey = systemkey.Trim(' ');
                if (systemkey == "") paramInfo.GetXmlProperty("genxml/hidden/systemkey").Trim(' ');
                if (systemkey == "" && paramCmd.Contains("_")) systemkey = paramCmd.Split('_')[0];
                if (systemkey == "") systemkey = "dnnrocket";
                var systemData = new SystemLimpet(systemkey);

                if (paramCmd == "admin_return")
                {
                    var sessionParams = new SessionParams(paramInfo);
                    var moduleid = paramInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                    if (moduleid == 0) moduleid = paramInfo.GetXmlPropertyInt("genxml/urlparams/moduleid");
                    // we need to clear the tracking of commands on return to view.
                    // This command is usually called from "MenuOut.cshtml" and triggers the "returnclick()" function.
                    var UserParams = new UserParams(sessionParams.BrowserSessionId);
                    UserParams.ModuleId = moduleid;  // use moduleid for tracking to stop mized content on modules.
                    UserParams.TrackClear(systemkey);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("OK");
                    context.Response.End();
                }

                var interfacekey = paramInfo.GetXmlProperty("genxml/hidden/interfacekey");
                if (interfacekey == "") interfacekey = paramInfo.GetXmlProperty("genxml/urlparams/interfacekey").Trim(' ');
                if (interfacekey == "") interfacekey = paramInfo.GetXmlProperty("genxml/hidden/i").Trim(' '); // reduce chars
                if (interfacekey == "") interfacekey = paramInfo.GetXmlProperty("genxml/urlparams/i").Trim(' ');
                if (interfacekey == "") interfacekey = paramCmd.Split('_')[0];
                if (interfacekey == "") interfacekey = systemkey;

                paramInfo.SetXmlProperty("genxml/systemkey", systemkey);

                if (paramCmd == "login_doregister")
                {
                    strOut = UserUtils.RegisterUser(postInfo, DNNrocketUtils.GetCurrentCulture());
                    if (strOut == "") UserUtils.DoLogin(systemData.SystemInfo, postInfo, HttpContext.Current.Request.UserHostAddress);
                }
                else if (paramCmd == "login_register")
                {
                    strOut = UserUtils.RegisterForm(systemData.SystemInfo, postInfo, interfacekey, UserUtils.GetCurrentUserId());
                }
                else if (paramCmd == "login_login")
                {
                    UserUtils.DoLogin(systemData.SystemInfo, postInfo, HttpContext.Current.Request.UserHostAddress);
                    strOut = ""; // the page will rteload after the call
                }
                else if (paramCmd == "changeculture")
                {
                    var lang = paramInfo.GetXmlProperty("genxml/hidden/culturecode");
                    DNNrocketUtils.SetCookieValue("language", lang);
                    strOut = lang; // the page will reload after the call
                }
                else
                {
                    switch (paramCmd)
                    {
                        case "getsidemenu":
                            strOut = GetSideMenu(paramInfo, systemkey);
                            break;
                        case "gettopbar":
                            strOut = GetTopBar(paramInfo, systemkey);
                            break;
                        default:
                            strOut = ""; // process the provider                            
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                strOut = LogUtils.LogException(ex);
            }
            return strOut;
        }

        private HttpResponseMessage ProcessProvider(string paramCmd, SimplisityInfo postInfo, SimplisityInfo paramInfo, SystemLimpet systemData, RocketInterface rocketInterface)
        {
            var strOut = "ERROR: Invalid.";
            object jsonReturn = null;
            object xmlReturn = null;
            var returnDictionary = new Dictionary<string, object>();

            // before event
            var rtnDictInfo = DNNrocketUtils.EventProviderBefore(paramCmd, systemData, postInfo, paramInfo, _editlang);
            if (rtnDictInfo.ContainsKey("post")) postInfo = (SimplisityInfo)rtnDictInfo["post"];
            if (rtnDictInfo.ContainsKey("param")) paramInfo = (SimplisityInfo)rtnDictInfo["param"];

            // command action
            if (rocketInterface.Exists)
            {

                returnDictionary = DNNrocketUtils.GetProviderReturn(paramCmd, systemData.SystemInfo, rocketInterface, postInfo, paramInfo, TemplateRelPath, _editlang);

                if (returnDictionary.ContainsKey("outputhtml"))
                {
                    strOut = (string)returnDictionary["outputhtml"];
                }
                if (returnDictionary.ContainsKey("filenamepath"))
                {
                    var downloadname = "download.zip";
                    if (returnDictionary.ContainsKey("downloadname")) downloadname = (string)returnDictionary["downloadname"];
                    DownloadFile((string)returnDictionary["filenamepath"], downloadname);
                }
                if (returnDictionary.ContainsKey("downloadfiledata"))
                {
                    var downloadname = "download.zip";
                    if (returnDictionary.ContainsKey("downloadname")) downloadname = (string)returnDictionary["downloadname"];
                    DownloadStringAsFile((string)returnDictionary["downloadfiledata"], downloadname);
                }
                if (returnDictionary.ContainsKey("outputjson"))
                {
                    jsonReturn = returnDictionary["outputjson"];
                }
                if (returnDictionary.ContainsKey("outputxml"))
                {
                    xmlReturn = returnDictionary["outputxml"];
                }

            }
            else
            {
                // check for systemspi, does not exist.  It's used to create the systemprovders 
                if (systemData.SystemKey == "" || systemData.SystemKey == "systemapi" || systemData.SystemKey == "login")
                {
                    try
                    {
                        var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.System.StartConnect");
                        returnDictionary = ajaxprov.ProcessCommand(paramCmd, systemData.SystemInfo, null, postInfo, paramInfo, _editlang);
                        strOut = (string)returnDictionary["outputhtml"];
                    }
                    catch (Exception ex)
                    {
                        strOut = ex.ToString();
                    }
                }
                else
                {
                    strOut = "ERROR: Invalid systemkey: " + systemData.SystemKey + "  interfacekey: " + rocketInterface.InterfaceKey + " cmd: " + paramCmd + " <br/> - Check Database for SYSTEM,'" + systemData.SystemKey + "' (No spaces) - Check 'simplisity_startpanel' and 'simplisity_panel' for correct s-cmd.  ";
                    strOut += "<br/> - Ensure you do not have an infinate loop by activating a simplisity_panel within the returning template, on the document ready JS function.";
                    CacheUtils.ClearAllCache();
                }
            }

            // after Event
            returnDictionary = DNNrocketUtils.EventProviderAfter(paramCmd, systemData, postInfo, paramInfo, _editlang);
            if (returnDictionary.ContainsKey("outputhtml")) strOut = (string)returnDictionary["outputhtml"];
            if (returnDictionary.ContainsKey("outputjson")) jsonReturn = returnDictionary["outputjson"];
            if (returnDictionary.ContainsKey("outputxml")) xmlReturn = returnDictionary["outputxml"];


            #region "return results"

            if (jsonReturn != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, jsonReturn, System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
            }
            if (xmlReturn != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, xmlReturn, System.Net.Http.Formatting.XmlMediaTypeFormatter.DefaultMediaType);
            }
            return this.Request.CreateResponse(HttpStatusCode.OK, strOut, "text/plain");

            #endregion

        }



        private string GetSideMenu(SimplisityInfo sInfo, string systemkey)
        {
            try
            {
                var strOut = "";
                var systemData = new SystemLimpet(systemkey);
                if (!systemData.Exists) return "ERROR: No SystemKey, Missing system.config";
                var appThemeSystem = new AppThemeSystemLimpet(systemkey);
                var razorTempl = appThemeSystem.GetTemplate("SideMenu.cshtml");
                strOut = RenderRazorUtils.RazorDetail(razorTempl, null, null, null, true);

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string GetTopBar(SimplisityInfo sInfo, string systemkey)
        {
            try
            {
                var passSettings = sInfo.ToDictionary();
                var systemData = new SystemLimpet("rocketcatalog");
                var appThemeSystem = new AppThemeSystemLimpet(systemkey);
                var razorTempl = appThemeSystem.GetTemplate("TopBar.cshtml");
                return RenderRazorUtils.RazorDetail(razorTempl, sInfo, passSettings, null, true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string DownloadFile(string filenamepath, string downloadname)
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

        private string DownloadStringAsFile(string filedata, string downloadname = "")
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
