using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Simplisity;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using RazorEngine;
using System.Security.Cryptography;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using System.Xml;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Modules;
using System.Net;
using System.IO;
using DotNetNuke.Common.Lists;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Threading;
using System.Globalization;
using DNNrocketAPI.Componants;
using DotNetNuke.Common;
using System.IO.Compression;
using Simplisity.TemplateEngine;
using DotNetNuke.Services.Exceptions;
using System.Collections;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules.Definitions;
using System.Drawing;
using DotNetNuke.Services.Scheduling;
using System.Xml.Serialization;

namespace DNNrocketAPI.Componants
{
    public static class DNNrocketUtils
    {
        public static Dictionary<string, object> ReturnString(string strOut, string jsonOut = null)
        {
            var rtnDic = new Dictionary<string, object>();
            rtnDic.Add("outputhtml", strOut);
            rtnDic.Add("outputjson", jsonOut);
            return rtnDic;
        }
        public static string HtmlOf(String htmlString)
        {
            return System.Web.HttpUtility.HtmlDecode(htmlString);
        }

        public static string RequestParam(HttpContext context, string paramName)
        {
            string result = null;

            if (context.Request.Form.Count != 0)
            {
                result = Convert.ToString(context.Request.Form[paramName]);
            }

            if (result == null)
            {
                return RequestQueryStringParam(context.Request, paramName);
            }

            return (result == null) ? String.Empty : result.Trim();
        }

        public static string RequestQueryStringParam(HttpRequest Request, string paramName)
        {
            var result = String.Empty;

            if (Request.QueryString.Count != 0)
            {
                result = Convert.ToString(Request.QueryString[paramName]);
            }

            return (result == null) ? String.Empty : result.Trim();
        }

        public static string RequestQueryStringParam(HttpContext context, string paramName)
        {
            return RequestQueryStringParam(context.Request, paramName);
        }


        public static string RazorRender(SimplisityRazor model, string razorTempl, Boolean debugMode = false)
        {
            var errorPath = "";
            var result = "";
            var errmsg = "";
            try
            {
                if (razorTempl == null || razorTempl == "") return "";
                var service = (IRazorEngineService)HttpContext.Current.Application.Get("DNNrocketIRazorEngineService");
                if (service == null)
                {
                    // do razor test
                    var config = new TemplateServiceConfiguration();
                    config.Debug = debugMode;
                    config.BaseTemplateType = typeof(RazorEngineTokens<>);
                    service = RazorEngineService.Create(config);
                    HttpContext.Current.Application.Set("DNNrocketIRazorEngineService", service);
                }
                Engine.Razor = service;
                var hashCacheKey = GetMd5Hash(razorTempl);

                var israzorCached = CacheUtilsDNN.GetCache(razorTempl); // get a cache flag for razor compile.
                if (israzorCached == null || (string)israzorCached != razorTempl || debugMode)
                {
                    errorPath += "RunCompile1>";
                    result = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
                    CacheUtilsDNN.SetCache(razorTempl, razorTempl);
                }
                else
                {
                    try
                    {
                        errorPath += "Run>";
                        result = Engine.Razor.Run(hashCacheKey, null, model);
                    }
                    catch (Exception ex)
                    {
                        errmsg = ex.ToString();
                        errorPath += "RunCompile2>";
                        result = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
                        CacheUtilsDNN.SetCache(razorTempl, razorTempl);
                    }
                }

            }
            catch (Exception ex)
            {
                CacheUtilsDNN.ClearAllCache();
                result = "CANNOT REBUILD TEMPLATE: errorPath=" + errorPath + " - " + ex.ToString() + " -------> " + result + " [" + errmsg + "]";
            }

            return result;
        }


        public static string GetMd5Hash(string input)
        {
            if (input != "")
            {
                var md5 = MD5.Create();
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (byte t in hash)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString();
            }
            return "";
        }

        public static string RazorDetailByName(string razorTemplateName, object obj, Dictionary<string, object> dataObjects = null, string lang = "", string templateControlRelPath = "/DesktopModules/DNNrocket/api/", string themeFolder = "config-w3", string versionFolder = "1.0", Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            if (razorTemplateName != "")
            {
                if (lang == "") lang = GetCurrentCulture();
                var razorTemplate = GetRazorTemplateData(razorTemplateName, templateControlRelPath, themeFolder, lang, versionFolder, debugmode);
                var sessionParamViewState = "";
                if (sessionParams != null) sessionParamViewState = sessionParams.ViewStateOut();
                return RazorObjectRender(razorTemplate, obj, dataObjects, settings, sessionParams, debugmode);
            }
            return "";
        }

        public static string RazorDetail(string razorTemplate, object obj, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            return RazorObjectRender(razorTemplate, obj, null, settings, sessionParams, debugmode);
        }
        public static string RazorObjectRender(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            var rtnStr = "";
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>();

                if (obj == null) obj = new SimplisityInfo();
                var l = new List<object>();
                l.Add(obj);

                var nbRazor = new SimplisityRazor(l, settings, HttpContext.Current.Request.QueryString);
                nbRazor.SessionParamsData = sessionParams;
                nbRazor.DataObjects = dataObjects;
                rtnStr = RazorRender(nbRazor, razorTemplate, debugmode);
            }

            return rtnStr;
        }

        public static string RazorList(string razorTemplate, List<object> objList, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            var rtnStr = "";
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>();
                var nbRazor = new SimplisityRazor(objList, settings, HttpContext.Current.Request.QueryString);
                nbRazor.SessionParamsData = sessionParams;
                rtnStr = RazorRender(nbRazor, razorTemplate, debugmode);
            }
            return rtnStr;
        }


        public static TemplateGetter GetTemplateEngine(string templateControlPath, string themeFolder, string lang, string versionFolder = "1.0", bool debugMode = false)
        {
            var cacheKey = templateControlPath + "*" + themeFolder + "*" + lang + "*" + versionFolder + "*" + debugMode;
            var templCtrl = CacheUtilsDNN.GetCache(cacheKey);
            if (templCtrl == null)
            {
                var controlMapPath = HttpContext.Current.Server.MapPath(templateControlPath);
                var themeFolderPath = themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder;
                var RocketThemes = PortalUtils.DNNrocketThemesDirectoryMapPath();
                templCtrl = new TemplateGetter(RocketThemes, themeFolderPath, controlMapPath, debugMode);
                CacheUtilsDNN.SetCache(cacheKey, templCtrl);
            }
            return (TemplateGetter)templCtrl;
        }

        public static string GetRazorTemplateData(string templatename, string templateControlPath, string themeFolder, string lang, string versionFolder = "1.0", bool debugMode = false)
        {
            var templCtrl = GetTemplateEngine(templateControlPath, themeFolder, lang, versionFolder, debugMode);
            var templ = templCtrl.GetTemplateData(templatename, lang);
            return templ;
        }


        public static void ZipFolder(string folderMapPath, string zipFileMapPath)
        {
            ZipFile.CreateFromDirectory(folderMapPath, zipFileMapPath);
        }

        public static void ExtractZipFolder(string zipFileMapPath, string outFolderMapPath)
        {
            ZipFile.ExtractToDirectory(zipFileMapPath, outFolderMapPath);
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.  Taken from StreamUtils on SharpZipLib as stripped
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        private static void ZipUtilCopy(Stream source, Stream destination, byte[] buffer)
        {

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException("Buffer is too small", "buffer");
            }

            bool copying = true;

            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }
            }
        }


        public static List<System.IO.FileInfo> GetFiles(string FolderMapPath)
        {
            DirectoryInfo di = new DirectoryInfo(FolderMapPath);
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            if (di.Exists)
            {
                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    files.Add(file);
                }
            }
            return files;
        }

        public static Dictionary<string, string> GetRegionList(string countrycode, string dnnlistname = "Region")
        {
            var parentkey = "Country." + countrycode;
            var objCtrl = new DotNetNuke.Common.Lists.ListController();
            var tList = objCtrl.GetListEntryInfoDictionary(dnnlistname, parentkey);
            var rtnDic = new Dictionary<string, string>();

            foreach (var r in tList)
            {
                var datavalue = r.Value;
                var regionname = datavalue.Text;
                rtnDic.Add(datavalue.Key, regionname);
            }

            return rtnDic;
        }


        public static Dictionary<String, String> GetCountryCodeList(int portalId = -1)
        {
            var rtnDic = new Dictionary<String, String>();
            if (portalId == -1 && PortalSettings.Current != null) portalId = PortalSettings.Current.PortalId;
            if (portalId != -1)
            {
                var objLCtrl = new ListController();
                var l = objLCtrl.GetListEntryInfoItems("Country").ToList();
                foreach (var i in l)
                {
                    rtnDic.Add(i.Value, i.Text);
                }
            }
            return rtnDic;
        }
        public static string GetCountryName(string countryCode, int portalId = -1)
        {
            var l = GetCountryCodeList(portalId);
            if (l.ContainsKey(countryCode)) return l[countryCode];
            return "";
        }


        public static string GetRegionName(string countryRegionCodeKey, string dnnlistname = "Region")
        {
            var codes = countryRegionCodeKey.Split(':');
            if (codes.Length == 2)
            {
                var countrycodesplit = codes[0].Split('.');
                if (countrycodesplit.Length == 3)
                {
                    var l = GetRegionList(countrycodesplit[1], dnnlistname);
                    if (l.ContainsKey(countryRegionCodeKey))
                    {
                        return l[countryRegionCodeKey];
                    }
                    return "";
                }
            }
            return "";
        }


        public static Dictionary<string, string> GetCultureCodeNameList(int portalId = -1)
        {
            var rtnList = new Dictionary<string, string>();
            if (portalId == -1 && PortalSettings.Current != null) portalId = PortalSettings.Current.PortalId;
            if (portalId != -1)
            {
                var enabledLanguages = LocaleController.Instance.GetLocales(portalId);
                foreach (KeyValuePair<string, Locale> kvp in enabledLanguages)
                {
                    rtnList.Add(kvp.Value.Code, kvp.Value.NativeName);
                }
            }
            return rtnList;
        }

        public static Dictionary<string, string> GetAllCultureCodeNameList()
        {
            var rtnList = new Dictionary<string, string>();

            CultureInfo[] cinfo = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
            foreach (CultureInfo cul in cinfo)
            {
                rtnList.Add(cul.Name, cul.DisplayName);
            }
            return rtnList;
        }

        public static string GetCultureCodeName(string cultureCode)
        {
            var d = GetAllCultureCodeNameList();
            if (!d.ContainsKey(cultureCode)) return "";
            return d[cultureCode];
        }

        public static List<string> GetCultureCodeList(int portalId = -1)
        {
            var rtnList = new List<string>();
            if (portalId == -1 && PortalSettings.Current != null) portalId = PortalSettings.Current.PortalId;
            if (portalId != -1)
            {
                var enabledLanguages = LocaleController.Instance.GetLocales(portalId);
                foreach (KeyValuePair<string, Locale> kvp in enabledLanguages)
                {
                    rtnList.Add(kvp.Value.Code);
                }
            }
            return rtnList;
        }

        public static List<SimplisityInfo> GetAllCultureCodeList()
        {
            var rtnList = new List<SimplisityInfo>();

            CultureInfo[] cinfo = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
            foreach (CultureInfo cul in cinfo)
            {
                var flagurl = "/DesktopModules/DNNrocket/API/images/flags/16/" + cul.Name + ".png";
                if (!File.Exists(MapPath(flagurl)))
                {
                    flagurl = "/DesktopModules/DNNrocket/API/images/flags/16/none.png";
                }
                var sni = new SimplisityInfo();
                sni.SetXmlProperty("genxml/displayname", cul.DisplayName);
                sni.SetXmlProperty("genxml/code", cul.Name);
                sni.SetXmlProperty("genxml/flagurl", flagurl);
                rtnList.Add(sni);
            }
            return rtnList;
        }


        public static string GetDataResponseAsString(string dataurl, string headerFieldId = "", string headerFieldData = "")
        {
            string strOut = "";
            if (!string.IsNullOrEmpty(dataurl))
            {
                try
                {

                    // solution for exception
                    // The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    System.Net.HttpWebRequest req = DotNetNuke.Common.Globals.GetExternalRequest(dataurl);
                    if (headerFieldId != "")
                    {
                        //allow a get request to pass data via the header.
                        //  This is limited to 32K by default in IIS, but can be limited to less. (So use with care!)
                        req.Headers.Add(headerFieldId, headerFieldData);
                    }
                    System.Net.WebResponse resp = req.GetResponse();
                    var s = resp.GetResponseStream();
                    if (s != null)
                    {
                        var reader = new StreamReader(s);
                        strOut = reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    strOut = "ERROR - dataurl=" + dataurl + "  ex=" + ex.ToString();
                }
            }
            else
            {
                strOut = "ERROR - No Data Url given";
            }
            return strOut;
        }


        public static string GetLocalizedString(string Key, string resourceFileRoot, string lang)
        {
            return Localization.GetString(Key, resourceFileRoot, lang);
        }

        /// <summary>
        /// Convert Object to XML.
        /// Exmaple:
        ///     var xmlString = ConvertObjectToXMLString(p);
        ///     XElement xElement = XElement.Parse(xmlString);
        /// </summary>
        /// <param name="classObject"></param>
        /// <returns></returns>
        public static string ConvertObjectToXMLString(object classObject)
        {
            string xmlString = null;
            XmlSerializer xmlSerializer = new XmlSerializer(classObject.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, classObject);
                memoryStream.Position = 0;
                xmlString = new StreamReader(memoryStream).ReadToEnd();
            }
            return xmlString;
        }
        /// <summary>
        /// Convert XML to Object.
        /// Exmaple:
        ///     UserDetail userDetail = (UserDetail)ConvertXmlStringtoObject<UserDetail>(xmlString);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T ConvertXmlStringtoObject<T>(string xmlString)
        {
            T classObject;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xmlString))
            {
                classObject = (T)xmlSerializer.Deserialize(stringReader);
            }
            return classObject;
        }
        public static string GetModuleVersion(int moduleId)
        {
            var strVersion = "";
            var objMCtrl = new DotNetNuke.Entities.Modules.ModuleController();
            var objMInfo = objMCtrl.GetModule(moduleId);
            if (objMInfo != null)
            {
                strVersion = objMInfo.DesktopModule.Version;
            }
            return strVersion;
        }

        public static void CreateFolder(string fullfolderPath)
        {
            // This function is to get around medium trust not allowing createfolder in .Net 2.0. 
            // DNN seems to have some work around (Not followed up on what exactly, probably security allowed in shared hosting environments for DNN except???).
            // But this leads me to have this rather nasty call to DNN FolderManager.
            // Prefered method is to us the Utils.CreateFolder function in NBrightCore.

            var blnCreated = false;
            //try normal test (doesn;t work on medium trust, but stops us forcing a "AddFolder" and suppressing the error.)
            try
            {
                blnCreated = System.IO.Directory.Exists(fullfolderPath);
            }
            catch (Exception ex)
            {
                var errmsg = ex.ToString();
                blnCreated = false;
            }

            if (!blnCreated)
            {
                try
                {
                    var f = FolderManager.Instance.AddFolder(FolderMappingController.Instance.GetFolderMapping(8), fullfolderPath);
                }
                catch (Exception ex)
                {
                    var errmsg = ex.ToString();
                    // Suppress error, becuase the folder may already exist!..NASTY!!..try and find a better way to deal with folders out of portal range!!
                }
            }
        }
        public static void CreateDefaultRocketRoles(int portalId)
        {
            CreateRole(portalId, DNNrocketRoles.ClientEditor);
            CreateRole(portalId, DNNrocketRoles.Editor);
            CreateRole(portalId, DNNrocketRoles.Manager);
            CreateRole(portalId, DNNrocketRoles.Premium);
            CreateRole(portalId, DNNrocketRoles.Administrators);
        }
        public static void CreateRole(int portalId, string roleName, string description = "", float serviceFee = 0, int billingPeriod = 0, string billingFrequency = "M", float trialFee = 0, int trialPeriod = 0, string trialFrequency = "N", bool isPublic = false, bool isAuto = false)
        {
            var role = RoleController.Instance.GetRoleByName(portalId, roleName);
            if (role == null)
            {
                RoleInfo objRoleInfo = new RoleInfo();
                objRoleInfo.PortalID = portalId;
                objRoleInfo.RoleName = roleName;
                objRoleInfo.RoleGroupID = Null.NullInteger;
                objRoleInfo.Description = description;
                objRoleInfo.ServiceFee = Convert.ToSingle(serviceFee < 0 ? 0 : serviceFee);
                objRoleInfo.BillingPeriod = billingPeriod;
                objRoleInfo.BillingFrequency = billingFrequency;
                objRoleInfo.TrialFee = Convert.ToSingle(trialFee < 0 ? 0 : trialFee);
                objRoleInfo.TrialPeriod = trialPeriod;
                objRoleInfo.TrialFrequency = trialFrequency;
                objRoleInfo.IsPublic = isPublic;
                objRoleInfo.AutoAssignment = isAuto;
                RoleController.Instance.AddRole(objRoleInfo);
            }
        }
        public static void AddRoleToModule(int portalId, int moduleid, int roleid)
        {
            var moduleInfo = GetModuleInfo(moduleid);
            var roleexist = false;
            var permissionID = -1;
            var PermissionsList2 = moduleInfo.ModulePermissions.ToList();
            var role = RoleController.Instance.GetRoleById(portalId, roleid);
            if (role != null)
            {
                var permissionController = new PermissionController();

                var editPermissionsList = permissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "EDIT");
                PermissionInfo editPermisison = null;
                //Edit
                if (editPermissionsList != null && editPermissionsList.Count > 0)
                {
                    editPermisison = (PermissionInfo)editPermissionsList[0];
                }


                foreach (var p in PermissionsList2)
                {
                    if (p.RoleName == role.RoleName)
                    {
                        permissionID = p.PermissionID;
                        p.AllowAccess = true;
                        roleexist = true;

                        var modulePermission = new ModulePermissionInfo(editPermisison);
                        modulePermission.RoleID = role.RoleID;
                        modulePermission.AllowAccess = true;
                        moduleInfo.ModulePermissions.Add(modulePermission);
                    }
                }

                // ADD Role
                if (!roleexist)
                {
                    ArrayList systemModuleEditPermissions = permissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "EDIT");
                    foreach (PermissionInfo permission in systemModuleEditPermissions)
                    {
                        if (permission.PermissionKey == "EDIT")
                        {
                            var objPermission = new ModulePermissionInfo(permission)
                            {
                                ModuleID = moduleInfo.DesktopModuleID,
                                RoleID = role.RoleID,
                                RoleName = role.RoleName,
                                AllowAccess = true,
                                UserID = Null.NullInteger,
                                DisplayName = Null.NullString
                            };
                            var permId = moduleInfo.ModulePermissions.Add(objPermission, true);
                            ModuleController.Instance.UpdateModule(moduleInfo);
                        }
                    }
                }


                // Check for DEPLOY 
                // This was added for upgrade on module.  I'm unsure if it's still required.
                if (!roleexist)
                {
                    ArrayList permissions = PermissionController.GetPermissionsByPortalDesktopModule();
                    foreach (PermissionInfo permission in permissions)
                    {
                        if (permission.PermissionKey == "DEPLOY")
                        {
                            var objPermission = new ModulePermissionInfo(permission)
                            {

                                ModuleID = moduleInfo.DesktopModuleID,
                                RoleID = role.RoleID,
                                RoleName = role.RoleName,
                                AllowAccess = true,
                                UserID = Null.NullInteger,
                                DisplayName = Null.NullString
                            };
                            var permId = moduleInfo.ModulePermissions.Add(objPermission, true);
                            ModuleController.Instance.UpdateModule(moduleInfo);
                        }
                    }
                }


            }
        }
        public static void RemoveRoleToModule(int portalId, int moduleid, int roleid)
        {
            var moduleInfo = GetModuleInfo(moduleid);
            var roleexist = false;
            var permissionID = -1;
            var PermissionsList2 = moduleInfo.ModulePermissions.ToList();
            var role = RoleController.Instance.GetRoleById(portalId, roleid);
            if (role != null)
            {
                foreach (var p in PermissionsList2)
                {
                    if (p.RoleName == role.RoleName)
                    {
                        permissionID = p.PermissionID;
                        roleexist = true;
                    }
                }

                if (roleexist && permissionID > -1)
                {
                    moduleInfo.ModulePermissions.Remove(permissionID, role.RoleID, Null.NullInteger);
                    ModuleController.Instance.UpdateModule(moduleInfo);
                }
            }


        }

        public static Dictionary<int,string> GetTabModuleTitles(int tabid, bool getDeleted = false)
        {
            var rtnDic = new Dictionary<int, string>();
            var l = ModuleController.Instance.GetTabModules(tabid);
            foreach (var m in l)
            {
                if (getDeleted)
                {
                    rtnDic.Add(m.Value.ModuleID, m.Value.ModuleTitle);
                }
                else
                {
                    if (!m.Value.IsDeleted) rtnDic.Add(m.Value.ModuleID, m.Value.ModuleTitle);
                }
            }
            return rtnDic;
        }
        private static Dictionary<int, ModuleInfo> GetTabModules(int tabid)
        {
            return ModuleController.Instance.GetTabModules(tabid);
        }
        private static ModuleInfo GetModuleInfo(int tabid, int moduleid)
        {
            return ModuleController.Instance.GetModule(moduleid, tabid, false);
        }
        public static void UpdateModuleTitle(int tabid, int moduleid, string title)
        {
            var modInfo = GetModuleInfo(tabid, moduleid);
            if (modInfo != null)
            {
                modInfo.ModuleTitle = title;
                ModuleController.Instance.UpdateModule(modInfo);
            }
        }
        private static ModuleInfo GetModuleInfo(int moduleId)
        {
            var objMCtrl = new DotNetNuke.Entities.Modules.ModuleController();
            var objMInfo = objMCtrl.GetModule(moduleId);
            return objMInfo;
        }
        public static bool ModuleIsDeleted(int tabid, int moduleid)
        {
            var modInfo = GetModuleInfo(tabid, moduleid);
            if (modInfo  != null)
            {
                return modInfo.IsDeleted;
            }
            return true;
        }
        public static bool ModuleExists(int tabid, int moduleid)
        {
            var modInfo = GetModuleInfo(tabid, moduleid);
            if (modInfo == null) return false;
            return true;
        }


        public static int GetModuleTabId(Guid uniqueId)
        {
            var mod = ModuleController.Instance.GetModuleByUniqueID(uniqueId);
            if (mod != null) return mod.TabID;
            return -1;
        }
        public static TabInfo GetTabInfo(int portalid, int tabid, bool ignoreCache = false)
        {
            return TabController.Instance.GetTab(tabid, portalid, ignoreCache);
        }
        public static TabInfo GetTabInfo(int tabid, bool ignoreCache = false)
        {
            return TabController.Instance.GetTab(tabid, PortalSettings.Current.PortalId, ignoreCache);
        }

        public static Dictionary<int, string> GetTreeTabList(bool showAllTabs = false)
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, GetCurrentCulture(), true);
            var rtnList = new Dictionary<int, string>();
            return GetTreeTabList(rtnList, tabList, 0, 0,"", showAllTabs);
        }

        private static Dictionary<int, string> GetTreeTabList(Dictionary<int, string> rtnList, List<TabInfo> tabList, int level, int parentid, string prefix = "", bool showAllTabs = false)
        {

            if (level > 20) // stop infinate loop
            {
                return rtnList;
            }
            if (parentid > 0) prefix += "..";
            foreach (TabInfo tInfo in tabList)
            {
                var parenttestid = tInfo.ParentId;
                if (parenttestid < 0) parenttestid = 0;
                if (parentid == parenttestid)
                {
                    if (!tInfo.IsDeleted && (tInfo.TabPermissions.Count > 2 || showAllTabs))
                    {
                        rtnList.Add(tInfo.TabID, prefix + "" + tInfo.TabName);
                        GetTreeTabList(rtnList, tabList, level + 1, tInfo.TabID, prefix);
                    }
                }
            }

            return rtnList;
        }

        public static Dictionary<int, string> GetTreeTabListOnTabId(bool showAllTabs = false)
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, GetCurrentCulture(), true);
            var rtnList = new Dictionary<int, string>();
            return GetTreeTabListOnTabId(rtnList, tabList, 0, 0, "", showAllTabs);
        }

        private static Dictionary<int, string> GetTreeTabListOnTabId(Dictionary<int, string> rtnList, List<TabInfo> tabList, int level, int parentid, string prefix = "", bool showAllTabs = false)
        {

            if (level > 20) // stop infinate loop
            {
                return rtnList;
            }
            if (parentid > 0) prefix += "..";
            foreach (TabInfo tInfo in tabList)
            {
                var parenttestid = tInfo.ParentId;
                if (parenttestid < 0) parenttestid = 0;
                if (parentid == parenttestid)
                {
                    if (!tInfo.IsDeleted && (tInfo.TabPermissions.Count > 2 || showAllTabs))
                    {
                        rtnList.Add(tInfo.TabID, prefix + "" + tInfo.TabName);
                        GetTreeTabListOnTabId(rtnList, tabList, level + 1, tInfo.TabID, prefix);
                    }
                }
            }

            return rtnList;
        }



        public static String GetResourceString(String resourcePath, String resourceKey, String resourceExt = "Text", String lang = "")
        {
            var resDic = GetResourceData(resourcePath, resourceKey, lang);
            if (resDic != null && resDic.ContainsKey(resourceExt))
            {
                try
                {
                    return resDic[resourceExt].TrimEnd(' ');
                }
                catch (Exception)
                {
                    // This action throws an error when added to the resx while it's cached by DNN.
                }
            }
            return "";
        }

        public static Dictionary<String, String> GetResourceData(String resourcePath, String resourceKey, String lang = "")
        {
            if (lang == "") lang = GetCurrentCulture();
            var ckey = resourcePath + resourceKey + lang;
            var obj = CacheUtilsDNN.GetCache(ckey);
            if (obj != null) return (Dictionary<String, String>)obj;

            var rtnList = new Dictionary<String, String>();
            var s = resourceKey.Split('.');
            if (s.Length == 2 && resourcePath != "")
            {
                var fName = s[0];
                var rKey = s[1];
                var relativefilename = resourcePath.TrimEnd('/') + "/" + fName + ".ascx.resx";
                var fullFileName = System.Web.Hosting.HostingEnvironment.MapPath(relativefilename);
                if (String.IsNullOrEmpty(fullFileName) || !System.IO.File.Exists(fullFileName))
                {
                    relativefilename = resourcePath.TrimEnd('/') + "/" + fName + ".resx";
                    fullFileName = System.Web.Hosting.HostingEnvironment.MapPath(relativefilename);
                }
                if (!String.IsNullOrEmpty(fullFileName) && System.IO.File.Exists(fullFileName))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(fullFileName);
                    var xmlNodList = xmlDoc.SelectNodes("root/data[starts-with(./@name,'" + rKey + ".')]");
                    if (xmlNodList != null)
                    {
                        foreach (XmlNode nod in xmlNodList)
                        {
                            if (nod.Attributes != null)
                            {
                                var n = nod.Attributes["name"].Value;
                                if (lang == "") lang = GetCurrentCulture();
                                var rtnValue = Localization.GetString(n, relativefilename, PortalSettings.Current, lang, true);
                                if (!rtnList.ContainsKey(n.Replace(rKey + ".", "")))
                                {
                                    rtnList.Add(n.Replace(rKey + ".", ""), rtnValue);
                                }
                            }
                        }
                    }
                }

                CacheUtilsDNN.SetCache(ckey, rtnList);
            }
            return rtnList;
        }


        #region "encryption"

        public static String Encrypt(String value, String passkey = "")
        {
            var objSec = new PortalSecurity();
            if (value == null) return "";
            if (passkey == "")
            {
                var ps = GetCurrentPortalSettings();
                passkey = ps.GUID.ToString();
            }
            return objSec.Encrypt(passkey, value);
        }

        public static String Decrypt(String value, String passkey = "")
        {
            var objSec = new PortalSecurity();
            if (value == null) return "";
            if (passkey == "")
            {
                var ps = GetCurrentPortalSettings();
                passkey = ps.GUID.ToString();
            }
            return objSec.Decrypt(passkey, value);
        }

        #endregion


        public static void DeleteCookieValue(string name)
        {
            HttpCookie MyCookie = new HttpCookie(name);
            MyCookie.Expires = DateTime.Now.AddDays(-10);
            MyCookie.Value = null;
            MyCookie.Path = "/";
            HttpContext.Current.Response.Cookies.Set(MyCookie);
        }

        public static void SetCookieValue(string name, string value)
        {
            if (value != null)
            {
                HttpCookie MyCookie = new HttpCookie(name);
                MyCookie.Value = value;
                MyCookie.Path = "/";
                HttpContext.Current.Response.Cookies.Add(MyCookie);
            }
        }

        public static string GetCookieValue(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                return HttpContext.Current.Request.Cookies[name].Value;
            }
            return "";
        }


        public static string SetEditCulture(string editlang)
        {
            var cachekey = "editlang*" + UserUtils.GetCurrentUserId();
            if (String.IsNullOrEmpty(editlang)) editlang = GetCurrentCulture();
            CacheUtilsDNN.SetCache(cachekey, editlang);
            return editlang;
        }

        public static string GetEditCulture()
        {
            var cachekey = "editlang*" + UserUtils.GetCurrentUserId();
            var rtnLang = HttpContext.Current.Request.QueryString["editlang"];
            if (String.IsNullOrEmpty(rtnLang))
            {
                var obj = CacheUtilsDNN.GetCache(cachekey);
                if (obj != null) rtnLang = obj.ToString();
                if (String.IsNullOrEmpty(rtnLang)) rtnLang = GetCurrentCulture();
            }
            else
            {
                SetEditCulture(rtnLang);
            }
            return rtnLang;
        }


        public static string GetCurrentCulture()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                // use url param first.  This is important on changing languages through DNN.
                if (HttpContext.Current.Request.QueryString["language"] != null)
                {
                    return HttpContext.Current.Request.QueryString["language"];
                }
                // no url language, look in the cookies.
                if (HttpContext.Current.Request.Cookies["language"] != null)
                {
                    var l = GetCultureCodeList();
                    var rtnlang = HttpContext.Current.Request.Cookies["language"].Value;
                    if (rtnlang == null || rtnlang == "" || !l.Contains(rtnlang))
                    {
                        if (l.Count >= 1)
                        {
                            rtnlang = l.First();
                        }
                    }
                    return rtnlang;
                }
            }
            // default to system thread, but in API this may be wrong.
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            return currentCulture.Name;
        }

        public static string GetCurrentCountryCode()
        {
            var cc = GetCurrentCulture();
            var c = cc.Split('-');
            var rtn = "";
            if (c.Length > 0) rtn = c[c.Length - 1];
            return rtn;
        }

        public static string GetCurrentLanguageCode()
        {
            var cc = GetCurrentCulture();
            var c = cc.Split('-');
            var rtn = "";
            if (c.Length > 0) rtn = c[0];
            return rtn;
        }
        public static string SystemThemeImgDirectoryRel()
        {
            return "/DesktopModules/DNNrocket/SystemThemes/idximg";
        }
        public static string SystemThemeImgDirectoryMapPath()
        {
            return DNNrocketUtils.MapPath(SystemThemeImgDirectoryRel());
        }
        public static string MapPath(string relpath)
        {
            if (String.IsNullOrWhiteSpace(relpath)) return "";
            relpath = "/" + relpath.TrimStart('/');
            return System.Web.Hosting.HostingEnvironment.MapPath(relpath);
        }
        public static string MapPathReverse(string fullMapPath)
        {
            if (String.IsNullOrWhiteSpace(fullMapPath)) return "";
            return @"\" + fullMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, String.Empty).Replace("\\", "/");
        }
        public static string Email(int portalId = -1)
        {
            if (portalId >= 0)
                return GetPortalSettings(portalId).Email;
            else
                return PortalSettings.Current.Email;
        }

        public static string GetEntityTypeCode(SimplisityInfo interfaceInfo)
        {
            var rtn = "";
            if (interfaceInfo != null)
            {
                rtn = interfaceInfo.GetXmlProperty("genxml/textbox/entitytypecode");
                if (rtn == "")
                {
                    rtn = interfaceInfo.GetXmlProperty("genxml/textbox/interfacekey").ToUpper();
                }
            }
            return rtn;
        }

        public static string GetUniqueFileName(string fileName, string folderMapPath, int idx = 1)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            if (File.Exists(folderMapPath + "\\" + fileName))
            {
                fileName = GetUniqueFileName(Path.GetFileNameWithoutExtension(fileName) + idx + Path.GetExtension(fileName), folderMapPath, idx + 1);
            }
            return fileName;
        }


        public static string EncryptFileName(string encryptkey, string fileName)
        {
            var fn = GeneralUtils.Encrypt(encryptkey, fileName);
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fn = fn.Replace(c, '_');
            }
            return fn;
        }

        public static SimplisityInfo GetSystemByName(string systemkey)
        {
            var objCtrl = new DNNrocketController();
            return objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemkey);
        }
        /// <summary>
        /// Use the First DNN Module definition as the DNNrocket systemkey
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public static string GetModuleSystemKey(int moduleId, int tabId)
        {
            var cacheKey = "systemkey.moduleid" + moduleId + ".tabid" + tabId;
            var systemKey = (string)CacheUtilsDNN.GetCache(cacheKey);
            if (systemKey == null)
            {
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);
                var desktopModule = moduleInfo.DesktopModule;
                systemKey = desktopModule.ModuleDefinitions.First().Key.ToLower();  // Use the First DNN Module definition as the DNNrocket systemkey
                CacheUtilsDNN.SetCache(cacheKey, systemKey);
            }
            return  systemKey;
        }
        /// <summary>
        /// Use the module name as DNNrocket interface key.
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public static string GetModuleInterfaceKey(int moduleId, int tabId)
        {
            var cacheKey = "interfacekey.moduleid" + moduleId + ".tabid" + tabId;
            var interfacekey = (string)CacheUtilsDNN.GetCache(cacheKey);
            if (interfacekey == null)
            {
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);
                var desktopModule = moduleInfo.DesktopModule;
                interfacekey = desktopModule.ModuleName.ToLower();  // Use the module name as DNNrocket interface key.
                CacheUtilsDNN.SetCache(cacheKey, interfacekey);
            }
            return interfacekey;
        }

        public static List<int> GetAllModulesOnPage(int tabId)
        {
            var rtn = new List<int>();
            var l = ModuleController.Instance.GetTabModules(tabId);
            foreach (var m in l)
            {
                 rtn.Add(m.Value.ModuleID);
            }
            return rtn;
        }

        public static SimplisityInfo GetModuleSystemInfo(string systemkey, int moduleId, bool loadSystemXml = true)
        {
            var objCtrl = new DNNrocketController();

            var systemInfo = (SimplisityInfo)CacheUtilsDNN.GetCache(systemkey + "modid" + moduleId);
            if (systemInfo == null)
            {
                systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemkey);
                if (systemInfo != null) CacheUtilsDNN.SetCache(systemkey + "modid" + moduleId, systemInfo);
            }
            if (systemInfo == null && loadSystemXml)
            {
                // no system data, so must be new install.
                var sData = new SystemDataList(); // load XML files.
                systemInfo = objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemkey);
                if (systemInfo != null) CacheUtilsDNN.SetCache(systemkey + "modid" + moduleId, systemInfo);
            }
            return systemInfo;
        }

        public static void IncludePageHeaders(string systemKey, Page page, int tabId, bool debugMode = false)
        {
            page.Items.Add("dnnrocket_pageheader", true);
            var cachekey = tabId + ".pageheader.cshtml";
            string cacheHead = null;
            cacheHead = (string)CacheFileUtils.GetCache(cachekey, "pageheader");
            if (String.IsNullOrEmpty(cacheHead))
            {
                var fileList = new List<string>();
                var modulesOnPage = GetAllModulesOnPage(tabId);
                foreach (var modId in modulesOnPage)
                {
                    var systemInfo = GetModuleSystemInfo(systemKey, modId, false);
                    if (systemInfo != null)
                    {
                        if (!fileList.Contains("GlobalPageHeading"))
                        {
                            fileList.Add("GlobalPageHeading");
                            var systemGlobalData = new SystemGlobalData();
                            cacheHead += systemGlobalData.GlobalPageHeading;
                        }
                        var appThemeMod = new AppThemeModule(modId, systemKey);
                        var fileMapPath = appThemeMod.AppTheme.GetFileMapPath("pageheader.cshtml");
                        if (!fileList.Contains(fileMapPath))
                        {
                            fileList.Add(fileMapPath);
                            var activePageHeaderTemplate = appThemeMod.GetTemplateRazor("pageheader.cshtml");
                            if (activePageHeaderTemplate != "")
                            {
                                var settings = new Dictionary<string, string>();
                                var l = new List<object>();
                                l.Add(appThemeMod);
                                var nbRazor = new SimplisityRazor(l, settings, HttpContext.Current.Request.QueryString);
                                nbRazor.TabId = tabId;
                                nbRazor.ModuleRef = appThemeMod.ModuleParams.ModuleRef;
                                nbRazor.ModuleId = appThemeMod.ModuleParams.ModuleId;
                                nbRazor.ModuleRefDataSource = appThemeMod.ModuleParams.ModuleRefDataSource;
                                nbRazor.ModuleIdDataSource = appThemeMod.ModuleParams.ModuleIdDataSource;
                                cacheHead += RazorRender(nbRazor, activePageHeaderTemplate, false);
                            }
                        }
                    }
                }

                CacheFileUtils.SetCache(cachekey, cacheHead);
                PageIncludes.IncludeTextInHeader(page, cacheHead);
            }
            else
            {
                PageIncludes.IncludeTextInHeader(page, cacheHead);
            }

        }

        public static Dictionary<string, object> GetProviderReturn(string paramCmd, SimplisityInfo systemInfo, DNNrocketInterface rocketInterface, SimplisityInfo postInfo, SimplisityInfo paramInfo, string templateRelPath, string editlang)
        {
            var rtnDic = new Dictionary<string, object>();
            var systemkey = "";
            if (systemInfo != null)
            {
                systemkey = systemInfo.GUIDKey;
            }
            if (systemkey == "" || systemkey == "systemapi" || systemkey == "login")
            {
                if (rocketInterface.Exists && (rocketInterface.Assembly == "" || rocketInterface.NameSpaceClass == ""))
                {
                    var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.System.StartConnect");
                    rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, postInfo, paramInfo, editlang);
                }
                else
                {
                    var ajaxprov = APInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                    rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, rocketInterface.Info, postInfo, paramInfo, editlang);
                }

            }
            else
            {
                if (systemkey != "")
                {
                    // Run API Provider.
                    rtnDic.Add("outputhtml", "API not found: " + systemkey);
                    if (systemInfo != null)
                    {
                        if (rocketInterface.Exists)
                        {
                            var controlRelPath = rocketInterface.TemplateRelPath;
                            if (controlRelPath == "") controlRelPath = templateRelPath;
                            if (rocketInterface.Assembly == "" || rocketInterface.NameSpaceClass == "")
                            {
                                rtnDic.Remove("outputhtml");
                                rtnDic.Add("outputhtml", "No assembly or namespaceclass defined: " + systemkey + " : " + rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass);
                            }
                            else
                            {
                                try
                                {
                                    var ajaxprov = APInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                                    rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, rocketInterface.Info, postInfo, paramInfo, editlang);
                                }
                                catch (Exception ex)
                                {
                                    rtnDic.Remove("outputhtml");
                                    rtnDic.Add("outputhtml", "ERROR: " + systemkey + " : " + rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass + " cmd:" + paramCmd + "<br/>" + ex.ToString());
                                }
                            }
                        }
                        else
                        {
                            rtnDic.Remove("outputhtml");
                            rtnDic.Add("outputhtml", "interfacekey not found: ");
                        }
                    }
                    else
                    {
                        rtnDic.Remove("outputhtml");
                        rtnDic.Add("outputhtml", "No valid system found: " + systemkey);
                    }
                }
            }
            return rtnDic;
        }

        public static Dictionary<string, object> EventProviderBefore(string paramCmd, SystemData systemData, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang)
        {
            var rtnDic = new Dictionary<string, object>();
            if (!systemData.Exists) return rtnDic;  // for systemadmin this may be null
            foreach (var rocketInterface in systemData.EventList)
            {
                if (rocketInterface.IsActive && rocketInterface.IsProvider("eventprovider") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    try
                    {
                        var cacheKey = rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass;
                        var ajaxprov = (EventInterface)CacheUtilsDNN.GetCache(cacheKey);
                        if (ajaxprov == null)
                        {
                            ajaxprov = EventInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                            CacheUtilsDNN.SetCache(cacheKey, ajaxprov);
                        }
                        rtnDic = ajaxprov.BeforeEvent(paramCmd, systemData, rocketInterface.Info, postInfo, paramInfo, editlang);
                    }
                    catch (Exception ex)
                    {
                        var err = ex.ToString();  // ignore event provider errors.  The error trap should be in the provider.
                    }
                }
            }
            return rtnDic;
        }

        public static Dictionary<string, object> EventProviderAfter(string paramCmd, SystemData systemData, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang)
        {
            var rtnDic = new Dictionary<string, object>();
            if (!systemData.Exists) return rtnDic;  // for systemadmin this may be null
            foreach (var rocketInterface in systemData.EventList)
            {
                if (rocketInterface.IsActive && rocketInterface.IsProvider("eventprovider") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    try
                    {
                        var cacheKey = rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass;
                        var ajaxprov = (EventInterface)CacheUtilsDNN.GetCache(cacheKey);
                        if (ajaxprov == null)
                        {
                            ajaxprov = EventInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                            CacheUtilsDNN.SetCache(cacheKey, ajaxprov);
                        }
                        rtnDic = ajaxprov.AfterEvent(paramCmd, systemData, rocketInterface.Info, postInfo, paramInfo, editlang);
                    }
                    catch (Exception ex)
                    {
                        var err = ex.ToString();  // ignore event provider errors.  The error trap should be in the provider.
                    }
                }
            }
            return rtnDic;
        }

        /// <summary>
        /// Render the image select template
        /// </summary>
        /// <param name="imagesize"></param>
        /// <param name="selectsingle"></param>
        /// <param name="autoreturn"></param>
        /// <param name="uploadFolder">this can be a full relative path or a single folder name at portal level in DNNrocket folder "/Portals/<portal>/DNNrocket/<uploadFolder>"</param>
        /// <param name="razorTemplateName"></param>
        /// <param name="templateControlRelPath"></param>
        /// <param name="themeFolder"></param>
        /// <returns></returns>
        public static string RenderImageSelect(string systemkey, string imageFolderRel, bool selectsingle = true, bool autoreturn = false)
        {

            string razorTemplateName = "ImageSelect.cshtml";
            string templateControlRelPath = "/DesktopModules/DNNrocket/images/";
            string themeFolder = "config-w3";
            var imageFolderMapPath = DNNrocketUtils.MapPath(imageFolderRel);

            var imageModel = new SimplisityRazor();

            imageModel.SetSetting("selectsingle", selectsingle.ToString());
            imageModel.SetSetting("autoreturn", autoreturn.ToString());
            imageModel.SetSetting("imagesize", "120");
            imageModel.SetSetting("imagefolderrel", imageFolderRel);
            imageModel.SetSetting("systemkey", systemkey);            

            var imgList = new List<object>();
            foreach (var i in DNNrocketUtils.GetFiles(imageFolderMapPath))
            {
                imgList.Add(i.Name);
            }
            imageModel.List = imgList;

            var strOut = "<div id='dnnrocket_imageselectwrapper'>";
            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture(), "1.0", true);
            strOut += DNNrocketUtils.RazorRender(imageModel, razorTempl, true);
            strOut += "</div>";
            return strOut;
        }


        public static string RenderDocumentSelect(ModuleParams moduleParams, bool selectsingle = true, bool autoreturn = false)
        {
            return RenderDocumentSelect(moduleParams.SystemKey, moduleParams.DocumentFolderRel, selectsingle, autoreturn);
        }
        public static string RenderDocumentSelect(string systemkey, string documentFolderRel, bool selectsingle = true, bool autoreturn = false)
        {
            string razorTemplateName = "DocSelect.cshtml";
            string templateControlRelPath = "/DesktopModules/DNNrocket/documents/";
            string themeFolder = "config-w3";

            var docModel = new SimplisityRazor();

            docModel.GetSettingBool("selectsingle", selectsingle);
            docModel.GetSettingBool("autoreturn", autoreturn);
            docModel.SetSetting("documentfolderrel", documentFolderRel);
            docModel.SetSetting("systemkey", systemkey);

            var docList = new List<object>();
            foreach (var i in DNNrocketUtils.GetFiles(DNNrocketUtils.MapPath(documentFolderRel)))
            {
                var sInfo = new SimplisityInfo();
                sInfo.SetXmlProperty("genxml/name", i.Name);
                sInfo.SetXmlProperty("genxml/relname", documentFolderRel + "/" + i.Name);
                sInfo.SetXmlProperty("genxml/fullname", i.FullName);
                sInfo.SetXmlProperty("genxml/extension", i.Extension);
                sInfo.SetXmlProperty("genxml/directoryname", i.DirectoryName);
                sInfo.SetXmlProperty("genxml/lastwritetime", i.LastWriteTime.ToShortDateString());
                docList.Add(sInfo);
            }
            docModel.List = docList;

            var strOut = "<div id='dnnrocket_documentselectwrapper'>";
            var razorTempl = DNNrocketUtils.GetRazorTemplateData(razorTemplateName, templateControlRelPath, themeFolder, DNNrocketUtils.GetCurrentCulture());
            strOut += DNNrocketUtils.RazorRender(docModel, razorTempl, false);
            strOut += "</div>";
            return strOut;
        }

        public static void ForceDocDownload(string docFilePath, string fileName, HttpResponse response)
        {
            if (File.Exists(docFilePath) & !String.IsNullOrEmpty(fileName))
            {
                try
                {

                    response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
                    response.ContentType = "application/octet-stream";
                    response.WriteFile(docFilePath);
                    response.End();
                }
                catch (Exception ex)
                {
                    // will cause a exception on the respone.End.  Just ignore.
                    var msg = ex.ToString();
                }
            }


        }

        public static void ForceStringDownload(HttpResponse response, string fileName, string fileData)
        {
            response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
            response.ContentType = "application/octet-stream";
            response.Write(fileData);
            response.End();
        }

        public static string LogException(Exception exc)
        {
            Exceptions.LogException(exc);
            return exc.ToString();
        }
        public static SimplisityRecord UpdateFieldXpath(SimplisityRecord record, string listname)
        {
            List<SimplisityRecord> fieldList = record.GetRecordList(listname);
            var lp = 1;
            foreach (var f in fieldList)
            {
                var localized = f.GetXmlPropertyBool("genxml/checkbox/localized");
                var xpath = f.GetXmlProperty("genxml/hidden/xpath");
                var size = f.GetXmlProperty("genxml/select/size");
                var label = f.GetXmlProperty("genxml/lang/genxml/textbox/label");
                var defaultValue = f.GetXmlProperty("genxml/textbox/defaultvalue");
                var defaultBool = f.GetXmlPropertyBool("genxml/textbox/defaultvalue");
                var attributes = f.GetXmlProperty("genxml/textbox/attributes");
                var isonlist = f.GetXmlPropertyBool("genxml/checkbox/isonlist");
                var fieldname = f.GetXmlProperty("genxml/textbox/name");

                if (f.GetXmlProperty("genxml/select/type").ToLower() == "textbox")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "textboxdate")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "textarea")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkbox")
                {
                    xpath = "genxml/checkbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "dropdown")
                {
                    xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "radiolist")
                {
                    xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "checkboxlist")
                {
                    xpath = "genxml/select/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }
                if (f.GetXmlProperty("genxml/select/type").ToLower() == "richtext")
                {
                    xpath = "genxml/textbox/" + f.GetXmlProperty("genxml/textbox/name").Trim(' ').ToLower();
                }

                if (localized) xpath = "genxml/lang/" + xpath;

                if (xpath != "")
                {
                    record.SetXmlProperty("genxml/" + listname + "/genxml[" + lp + "]/hidden/xpath", xpath);
                }
                lp += 1;
            }
            return record;
        }


        //  --------------------- Debug Log files ------------------------------
        public static void LogDebug(string message)
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\debug";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.AppendToLog(mappath, "debug", message);
        }
        public static void LogDebugClear()
        {
            var mappath = PortalUtils.TempDirectoryMapPath().TrimEnd('\\') + "\\debug";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            System.IO.DirectoryInfo di = new DirectoryInfo(mappath);
            foreach (System.IO.FileInfo file in di.GetFiles())
            {

                file.Delete();
            }
        }

        public static void LogTracking(string message, string logName = "Log")
        {
            var mappath = PortalUtils.HomeDNNrocketDirectoryMapPath().TrimEnd('\\') + "\\logs";
            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            FileUtils.AppendToLog(mappath, logName, message);
        }

        /// <summary>
        /// Generates a new backup file name, based on the date and time.  (May not be unique)
        /// </summary>
        /// <returns></returns>
        public static string BackUpNewFileName(string backupRootFolder, string moduleName, string fileAppendix = "BackUp.xml")
        {
            if (!Directory.Exists(PortalUtils.BackUpDirectoryMapPath() + "\\" +  backupRootFolder))
            {
                Directory.CreateDirectory(PortalUtils.BackUpDirectoryMapPath() + "\\" + backupRootFolder);
            }            
            if (!Directory.Exists(PortalUtils.BackUpDirectoryMapPath() + "\\" + backupRootFolder + "\\" + moduleName))
            {
                Directory.CreateDirectory(PortalUtils.BackUpDirectoryMapPath() + "\\" + backupRootFolder + "\\" + moduleName);
            }
            return PortalUtils.BackUpDirectoryMapPath() + "\\" + backupRootFolder + "\\" + moduleName + "\\" + DateTime.Now.ToFileTime() + "_" + fileAppendix;
        }


        /// <summary>
        /// Recycles a web site Application Pool (including the current web site).
        /// Requires to reference Microsoft.Web.Administration and System.Web.Hosting.
        /// IMPORTANT: The IIS user requires extended permissions to recycle application pool(s).
        /// </summary>
        /// <param name="siteName">The site name: leave it NULL to recycle the current site's App Pool.</param>
        /// <returns>TRUE if the site's App Pool has been recycled; FALSE if no site has been found with the given name.</returns>
        public static bool RecycleApplicationPool(string siteName = null)
        {
            try
            {
                RetryableAction.Retry5TimesWith2SecondsDelay(() => File.SetLastWriteTime(Globals.ApplicationMapPath + "\\web.config", DateTime.Now), "Touching config file");
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }

        }

        public static void CloneModule(int moduleid, int fromTabId, int toTabId)
        {
            if ((toTabId > 0) && (fromTabId > 0) && (fromTabId != toTabId))
            {
                var existingmodule = ModuleController.Instance.GetModule(moduleid, toTabId, true);
                if (existingmodule != null)
                {
                    if (existingmodule.IsDeleted)
                    {
                        ModuleController.Instance.DeleteTabModule(toTabId, moduleid, false) ;
                    }
                    existingmodule = ModuleController.Instance.GetModule(moduleid, toTabId, true);
                }
                if (existingmodule == null)
                {
                    ModuleInfo fmi = ModuleController.Instance.GetModule(moduleid, fromTabId, true);
                    ModuleInfo newModule = fmi.Clone();

                    newModule.UniqueId = Guid.NewGuid(); // Cloned Module requires a different uniqueID 
                    newModule.TabID = toTabId;
                    ModuleController.Instance.AddModule(newModule);
                }

            }

        }


        public static void ClearThumbnailLock()
        {
            //Images in Cache still retain the file lock, we must dispose of then out of cache.
            var l = CacheUtils.GetGroupCache("DNNrocketThumb");
            foreach (Bitmap i in l)
            {
                i.Dispose();
            }
            CacheUtils.ClearAllCache("DNNrocketThumb");
        }

        public static bool CreateModuleDefinition(SimplisityRecord sRec)
        {
            try
            {

                //<genxml>
                //    <moduleowner>ModuleCompany</moduleowner>
                //    <modulefolder>ModuleFolder</modulefolder>
                //    <modulename>ModuleName</modulename>
                //    <modulefriendlyname>Firendly Module Name</modulefriendlyname>
                //    <moduledescription>Rocket Module</moduledescription>
                //    <moduletitle>Module Title</moduletitle>
                //</genxml>

                var className = sRec.GetXmlProperty("genxml/moduleowner") + "." + sRec.GetXmlProperty("genxml/modulename");
                var controlName = sRec.GetXmlProperty("genxml/modulename");
                var moduleOwner = sRec.GetXmlProperty("genxml/moduleowner");
                var moduleFolder = sRec.GetXmlProperty("genxml/modulefolder");
                var moduleFriendlyName = sRec.GetXmlProperty("genxml/modulefriendlyname");
                var moduleDescription = sRec.GetXmlProperty("genxml/moduledescription");
                var moduleFullFolderMapPath = moduleOwner.Replace("/", "\\") + "\\" + moduleFolder.Replace("/", "\\");
                var moduleFullFolderRel = moduleOwner + "/" + moduleFolder;
                var moduleTitle = sRec.GetXmlProperty("genxml/moduletitle");

                if (PackageController.Instance.GetExtensionPackage(Null.NullInteger, p => p.Name == className) == null)
                {
                    //Create module folder
                    var moduleFolderPath = Globals.ApplicationMapPath + "\\DesktopModules\\" + moduleFullFolderMapPath;

                    if (!Directory.Exists(moduleFolderPath))
                    {
                        Directory.CreateDirectory(moduleFolderPath);
                    }

                    //Create module control
                    if (controlName != "")
                    {
                        //Create package
                        var objPackage = new PackageInfo();
                        objPackage.Name = className;
                        objPackage.FriendlyName = moduleFriendlyName;
                        objPackage.Description = moduleDescription;
                        objPackage.Version = new Version(1, 0, 0);
                        objPackage.PackageType = "Module";
                        objPackage.License = "";
                        objPackage.Owner = moduleOwner;
                        objPackage.Organization = moduleOwner;
                        objPackage.FolderName = "DesktopModules/" + moduleFullFolderRel;
                        objPackage.License = "The license for this package is not currently included within the installation file, please check with the vendor for full license details.";
                        objPackage.ReleaseNotes = "This package has no Release Notes.";
                        PackageController.Instance.SaveExtensionPackage(objPackage);

                        //Create desktopmodule
                        var objDesktopModule = new DesktopModuleInfo();
                        objDesktopModule.DesktopModuleID = Null.NullInteger;
                        objDesktopModule.ModuleName = className;
                        objDesktopModule.FolderName = moduleFullFolderRel;
                        objDesktopModule.FriendlyName = moduleFriendlyName;
                        objDesktopModule.Description = moduleDescription;
                        objDesktopModule.IsPremium = false;
                        objDesktopModule.IsAdmin = false;
                        objDesktopModule.Version = "01.00.00";
                        objDesktopModule.BusinessControllerClass = "";
                        objDesktopModule.CompatibleVersions = "";
                        objDesktopModule.AdminPage = "";
                        objDesktopModule.HostPage = "";
                        objDesktopModule.Dependencies = "";
                        objDesktopModule.Permissions = "";
                        objDesktopModule.PackageID = objPackage.PackageID;
                        objDesktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(objDesktopModule, false, true);
                        objDesktopModule = DesktopModuleController.GetDesktopModule(objDesktopModule.DesktopModuleID, Null.NullInteger);

                        //Add OwnerName to the DesktopModule taxonomy and associate it with this module
                        var vocabularyId = -1;
                        var termId = -1;
                        var objTermController = DotNetNuke.Entities.Content.Common.Util.GetTermController();
                        var objTerms = objTermController.GetTermsByVocabulary("Module_Categories");
                        foreach (Term term in objTerms)
                        {
                            vocabularyId = term.VocabularyId;
                            if (term.Name == moduleOwner)
                            {
                                termId = term.TermId;
                            }
                        }
                        if (termId == -1)
                        {
                            termId = objTermController.AddTerm(new Term(vocabularyId) { Name = moduleOwner });
                        }
                        var objTerm = objTermController.GetTerm(termId);
                        var objContentController = DotNetNuke.Entities.Content.Common.Util.GetContentController();
                        var objContent = objContentController.GetContentItem(objDesktopModule.ContentItemId);
                        objTermController.AddTermToContent(objTerm, objContent);

                        //Add desktopmodule to all portals
                        DesktopModuleController.AddDesktopModuleToPortals(objDesktopModule.DesktopModuleID);

                        //Create module definition
                        var objModuleDefinition = new ModuleDefinitionInfo();
                        objModuleDefinition.ModuleDefID = Null.NullInteger;
                        objModuleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID;
                        // need core enhancement to have a unique DefinitionName  
                        objModuleDefinition.FriendlyName = className;
                        //objModuleDefinition.FriendlyName = txtModule.Text;
                        //objModuleDefinition.DefinitionName = GetClassName();
                        objModuleDefinition.DefaultCacheTime = 0;
                        objModuleDefinition.ModuleDefID = ModuleDefinitionController.SaveModuleDefinition(objModuleDefinition, false, true);

                        //Create modulecontrol
                        var objModuleControl = new ModuleControlInfo();
                        objModuleControl.ModuleControlID = Null.NullInteger;
                        objModuleControl.ModuleDefID = objModuleDefinition.ModuleDefID;
                        objModuleControl.ControlKey = "";
                        objModuleControl.ControlSrc = "DesktopModules/" + moduleFullFolderRel + "/" + controlName;
                        objModuleControl.ControlTitle = "";
                        objModuleControl.ControlType = SecurityAccessLevel.View;
                        objModuleControl.HelpURL = "";
                        objModuleControl.IconFile = "";
                        objModuleControl.ViewOrder = 0;
                        objModuleControl.SupportsPartialRendering = false;
                        objModuleControl.SupportsPopUps = false;
                        ModuleControlController.AddModuleControl(objModuleControl);

                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                LogException(exc);
            }
            return false;
        }


        #region "DNN cache"

        public static void SetCache(string cacheKey, object objObject)
        {
            DataCache.SetCache(cacheKey, objObject, DateTime.Now + new TimeSpan(2, 0, 0, 0));
        }
        public static object GetCache(string cacheKey)
        {
            return DataCache.GetCache(cacheKey);
        }
        public static void RemoveCache(string cacheKey)
        {
            DataCache.RemoveCache(cacheKey);
        }
        public static void ClearAllCache()
        {
            DataCache.ClearCache();
        }
        public static void ClearPortalCache()
        {
            DataCache.ClearPortalCache(PortalSettings.Current.PortalId, true);
        }

        public static void ClearPortalCache(int portalId)
        {
            DataCache.ClearPortalCache(portalId, true);
        }

        /// <summary>
        /// Synchronizes the module content between cache and database. + Update ModifiedContentDate
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        public static void SynchronizeModule(int moduleId)
        {
            ModuleController.SynchronizeModule(moduleId);
        }


        #endregion


        #region "Portal - obsolete"

        [Obsolete("Use PortalUtils instead")]
        public static List<int> GetPortals() { return PortalUtils.GetPortals(); }
        [Obsolete("Use PortalUtils instead")]
        public static int GetPortalId(){return PortalUtils.GetPortalId();}
        [Obsolete("Use PortalUtils instead")]
        public static PortalSettings GetPortalSettings(){return PortalUtils.GetPortalSettings();}
        [Obsolete("Use PortalUtils instead")]
        public static PortalSettings GetPortalSettings(int portalId){return PortalUtils.GetPortalSettings(portalId);}
        [Obsolete("Use PortalUtils instead")]
        public static int GetPortalByModuleID(int moduleId)
        {
             return PortalUtils.GetPortalByModuleID(moduleId);
        }
        [Obsolete("Use PortalUtils instead")]
        public static List<int> GetAllPortalIds()
        {
            return PortalUtils.GetAllPortalIds();
        }
        [Obsolete("Use PortalUtils instead")]
        public static List<SimplisityRecord> GetAllPortalRecords()
        {
            return PortalUtils.GetAllPortalRecords();
        }
        [Obsolete("Use PortalUtils instead")]
        public static void CreatePortalFolder(PortalSettings PortalSettings, string FolderName)
        {
            PortalUtils.CreatePortalFolder(PortalSettings, FolderName);
        }
        [Obsolete("Use PortalUtils instead")]
        public static PortalSettings GetCurrentPortalSettings()
        {
            return PortalUtils.GetCurrentPortalSettings();
        }
        [Obsolete("Method1 is deprecated, please use PortalUtils.DefaultPortalAlias(portalId) instead.")]
        public static string GetDefaultWebsiteDomainUrl(int portalId = -1)
        {
            return PortalUtils.DefaultPortalAlias(portalId);
        }
        [Obsolete("Use PortalUtils instead")]
        public static List<string> GetPortalAliases(int portalId)
        {
            return PortalUtils.GetPortalAliases(portalId);
        }
        [Obsolete("Use PortalUtils instead")]
        public static string DefaultPortalAlias(int portalId = -1)
        {
            return PortalUtils.DefaultPortalAlias(portalId);
        }
        [Obsolete("Use PortalUtils instead")]
        public static string SiteGuid()
        {
            return PortalUtils.SiteGuid();
        }
        [Obsolete("Use PortalUtils instead")]
        public static string GetPortalAlias(string lang, int portalid = -1)
        {
            return PortalUtils.GetPortalAlias(lang, portalid);
        }


        #endregion

    }
}
