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

            var rtn = ActionSimplisityInfo(postInfo, paramInfo, paramCmd, systemkey);
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

            var rtn = ActionSimplisityInfo(postInfo, paramInfo, paramCmd, systemkey);
            if (rtn.Headers.Contains("Access-Control-Allow-Origin")) rtn.Headers.Remove("Access-Control-Allow-Origin");
            rtn.Headers.Add("Access-Control-Allow-Origin", "*");
            return rtn;
        }
        /// <summary>
        /// When a remote module or html passes a compress remoteParam setting string.
        /// Usually from a RemoteLimpet object 
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

            var postInfo = new SimplisityInfo();
            var paramInfo = new SimplisityInfo();
            var rawInfo = new SimplisityRecord();
            if (rawData != "")
            {
                rawInfo.XMLData = rawData;
                var nodList = rawInfo.XMLDoc.SelectNodes("items/item");
                if (nodList != null)
                {
                    foreach (XmlNode nod in nodList)
                    {
                        var inputInfo = new SimplisityInfo();
                        inputInfo.FromXmlItem(nod.OuterXml);
                        if (inputInfo.TypeCode == "postInfo") postInfo = inputInfo;
                        if (inputInfo.TypeCode == "paramInfo") paramInfo = inputInfo;
                    }
                }
            }

            var remoteSystemKey = paramInfo.GetXmlProperty("genxml/hidden/remotesystemkey");
            if (remoteSystemKey == "")
            {
                var remoteParamXml = paramInfo.GetXmlProperty("genxml/settings/remoteparams");
                if (remoteParamXml != "")
                {
                    remoteParamXml = StringCompress.DecompressString(remoteParamXml);
                    var remoteParam = new RemoteLimpet(-1);
                    remoteParam.Record.FromXmlItem(remoteParamXml);
                    remoteSystemKey = remoteParam.RemoteSystemKey;
                }
            }
            if (remoteSystemKey == "") return this.Request.CreateResponse(HttpStatusCode.OK, "RemoteSystemKey not found");

            var paramCmd = context.Request.QueryString["cmd"];
            var portalId = PortalUtils.GetPortalId();
            paramInfo.PortalId = portalId;
            paramInfo.SetXmlProperty("genxml/hidden/remotecall", "True");

            // We often want the remote moduleid, so we can use it in the razor to identify the module
            if (context.Request.QueryString.AllKeys.Contains("moduleid"))
            {
                if (GeneralUtils.IsNumeric(context.Request.QueryString["moduleid"])) paramInfo.SetXmlProperty("genxml/hidden/moduleid", context.Request.QueryString["moduleid"]);
            }


            var rtn = ActionSimplisityInfo(postInfo, paramInfo, paramCmd, remoteSystemKey);
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

        private HttpResponseMessage ActionSimplisityInfo(SimplisityInfo postInfo, SimplisityInfo paramInfo, string paramCmd, string systemkey)
        {
            var strOut = "ERROR: Invalid.";
            var jsonReturn = "";
            var xmlReturn = "";
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
                var systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemkey);
                var systemData = new SystemLimpet(systemInfo);

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
                if (interfacekey == "") interfacekey = systemData.DefaultInterface;
                if (interfacekey == "") interfacekey = systemkey;

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
                        default:
                            var rocketInterface = new RocketInterface(systemInfo, interfacekey);
                            var returnDictionary = new Dictionary<string, object>();

                            // before event
                            var rtnDictInfo = DNNrocketUtils.EventProviderBefore(paramCmd, systemData, postInfo, paramInfo, _editlang);
                            if (rtnDictInfo.ContainsKey("post")) postInfo = (SimplisityInfo)rtnDictInfo["post"];
                            if (rtnDictInfo.ContainsKey("param")) paramInfo = (SimplisityInfo)rtnDictInfo["param"];

                            // command action
                            if (rocketInterface.Exists)
                            {
                                returnDictionary = DNNrocketUtils.GetProviderReturn(paramCmd, systemInfo, rocketInterface, postInfo, paramInfo, TemplateRelPath, _editlang);

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
                                    jsonReturn = (string)returnDictionary["outputjson"];
                                }
                                if (returnDictionary.ContainsKey("outputxml"))
                                {
                                    jsonReturn = (string)returnDictionary["outputxml"];
                                }

                            }
                            else
                            {
                                // check for systemspi, does not exist.  It's used to create the systemprovders 
                                if (systemkey == "" || systemkey == "systemapi" || systemkey == "login")
                                {
                                    var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.System.StartConnect");
                                    returnDictionary = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, postInfo, paramInfo, _editlang);
                                    strOut = (string)returnDictionary["outputhtml"];
                                }
                                else
                                {
                                    strOut = "ERROR: Invalid systemkey: " + systemkey + "  interfacekey: " + interfacekey + " cmd: " + paramCmd + " <br/> - Check Database for SYSTEM,'" + systemkey + "' (No spaces) - Check 'simplisity_startpanel' and 'simplisity_panel' for correct s-cmd.  ";
                                    strOut += "<br/> - Ensure you do not have an infinate loop by activating a simplisity_panel within the returning template, on the document ready JS function.";
                                    CacheUtils.ClearAllCache();
                                }
                            }

                            // after Event
                            returnDictionary = DNNrocketUtils.EventProviderAfter(paramCmd, systemData, postInfo, paramInfo, _editlang);
                            if (returnDictionary.ContainsKey("outputhtml")) strOut = (string)returnDictionary["outputhtml"];
                            if (returnDictionary.ContainsKey("outputjson")) jsonReturn = (string)returnDictionary["outputjson"];
                            if (returnDictionary.ContainsKey("outputxml")) xmlReturn = (string)returnDictionary["outputxml"];
                            
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                strOut = LogUtils.LogException(ex);
            }


            #region "return results"

            if (jsonReturn != null && jsonReturn != "")
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, jsonReturn, System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
            }
            if (xmlReturn != null && xmlReturn != "")
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, xmlReturn, System.Net.Http.Formatting.XmlMediaTypeFormatter.DefaultMediaType);
            }
            return this.Request.CreateResponse(HttpStatusCode.OK, strOut);

            #endregion

        }

        private string GetSideMenu(SimplisityInfo sInfo, string systemkey)
        {
            try
            {
                var strOut = "";
                var themeFolder = sInfo.GetXmlProperty("genxml/hidden/theme");
                var razortemplate = sInfo.GetXmlProperty("genxml/hidden/template");
                var moduleid = sInfo.GetXmlPropertyInt("genxml/hidden/moduleid");
                if (moduleid == 0) moduleid = -1;

                var passSettings = sInfo.ToDictionary();

                var systemDataList = new SystemLimpetList();
                var sInfoSystem = systemDataList.GetSystemByKey(systemkey);
                var systemData = new SystemLimpet(sInfoSystem);
                var sidemenu = new SideMenu(sInfoSystem);
                var templateControlRelPath = sInfo.GetXmlProperty("genxml/hidden/relpath");
                sidemenu.ModuleId = moduleid;

                var razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", systemData.DebugMode);

                if (razorTempl == "")
                {
                    // no razor template for sidemenu, so use default.
                    razorTempl = RenderRazorUtils.GetRazorTemplateData(razortemplate, TemplateRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", systemData.DebugMode);
                }

                strOut = RenderRazorUtils.RazorDetail(razorTempl, sidemenu, passSettings, null, systemData.DebugMode);

                return strOut;
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
