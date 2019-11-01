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


namespace DNNrocketAPI
{
    public static class DNNrocketUtils
    {

        public static Dictionary<string, string> ReturnString(string strOut, string jsonOut = "")
        {
            var rtnDic = new Dictionary<string, string>();
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

                var israzorCached = CacheUtils.GetCache(razorTempl); // get a cache flag for razor compile.
                if (israzorCached == null || (string)israzorCached != razorTempl || debugMode)
                {
                    errorPath += "RunCompile1>";
                    result = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
                    CacheUtils.SetCache(razorTempl, razorTempl);
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
                        CacheUtils.SetCache(razorTempl, razorTempl);
                    }
                }

            }
            catch (Exception ex)
            {
                CacheUtils.ClearAllCache();
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

        public static string RazorDetail(string razorTemplate, object obj, Dictionary<string, string> settings = null, SimplisityInfo headerData = null, bool debugmode = false)
        {
            var rtnStr = "";
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>();
                if (headerData == null) headerData = new SimplisityInfo();

                if (obj == null) obj = new SimplisityInfo();
                var l = new List<object>();
                l.Add(obj);

                var nbRazor = new SimplisityRazor(l, settings, HttpContext.Current.Request.QueryString);
                nbRazor.HeaderData = headerData;
                rtnStr = RazorRender(nbRazor, razorTemplate, debugmode);

            }

            return rtnStr;
        }


        public static string RazorList(string razorTemplate, List<SimplisityInfo> objList, Dictionary<string, string> settings = null, SimplisityInfo headerData = null, bool debugmode = false)
        {
            return RazorList(razorTemplate, objList.Cast<object>().ToList(), settings, headerData, debugmode);
        }

        public static string RazorList(string razorTemplate, List<object> objList, Dictionary<string, string> settings = null, SimplisityInfo headerData = null, bool debugmode = false)
        {
            var rtnStr = "";
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>(); 
                if (headerData == null) headerData = new SimplisityInfo();
                var nbRazor = new SimplisityRazor(objList, settings, HttpContext.Current.Request.QueryString);
                nbRazor.HeaderData = headerData;
                rtnStr = RazorRender(nbRazor, razorTemplate, debugmode);
            }
            return rtnStr;
        }


        public static TemplateGetter GetTemplateEngine(string templateControlPath, string themeFolder, string lang, string versionFolder = "1.0", bool debugMode = false)
        {
            var cacheKey = templateControlPath + "*" + themeFolder + "*" + lang + "*" + versionFolder + "*" + debugMode;
            var templCtrl = CacheUtils.GetCache(cacheKey);
            if (templCtrl == null)
            {
                var controlMapPath = HttpContext.Current.Server.MapPath(templateControlPath);
                var themeFolderPath = themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder;
                var RocketThemes = DNNrocketThemesDirectory();
                templCtrl = new TemplateGetter(RocketThemes, themeFolderPath, controlMapPath, debugMode);
                CacheUtils.SetCache(cacheKey, templCtrl);
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
        public static string GetCountryName(string CountryCode, int portalId = -1)
        {
            var l = GetCountryCodeList(portalId);
            return l[CountryCode]; ;
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


        public static Dictionary<string,string> GetCultureCodeNameList(int portalId = -1)
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


        public static DotNetNuke.Entities.Users.UserInfo GetValidUser(int PortalId, string username, string password)
        {
            var userLoginStatus = new DotNetNuke.Security.Membership.UserLoginStatus();
            return DotNetNuke.Entities.Users.UserController.ValidateUser(PortalId, username, password, "", "", "", ref userLoginStatus);
        }

        public static bool IsValidUser(int PortalId, string username, string password)
        {
            var u = GetValidUser(PortalId, username, password);
            if (u != null)
            {
                return true;
            }
            return false;
        }

        public static string GetLocalizedString(string Key, string resourceFileRoot, string lang)
        {
            return Localization.GetString(Key, resourceFileRoot, lang);
        }

        public static int GetPortalByModuleID(int moduleId)
        {
            var objMCtrl = new DotNetNuke.Entities.Modules.ModuleController();
            var objMInfo = objMCtrl.GetModule(moduleId);
            if (objMInfo == null) return -1;
            return objMInfo.PortalID;
        }

        /// <summary>
        /// GET Portals 
        /// </summary>
        /// <returns></returns>
        public static List<PortalInfo> GetAllPortals()
        {
            var pList = new List<PortalInfo>();
            var objPC = new DotNetNuke.Entities.Portals.PortalController();

            var list = objPC.GetPortals();

            if (list == null || list.Count == 0)
            {
                //Problem with DNN6 GetPortals when ran from scheduler.
                PortalInfo objPInfo;
                var flagdeleted = 0;

                for (var lp = 0; lp <= 500; lp++)
                {
                    objPInfo = objPC.GetPortal(lp);
                    if ((objPInfo != null))
                    {
                        pList.Add(objPInfo);
                    }
                    else
                    {
                        // some portals may be deleted, skip 3 to see if we've got to the end of the list.
                        // VERY weak!!! shame!! but issue with a DNN6 version only.
                        if (flagdeleted == 3) break;
                        flagdeleted += 1;
                    }
                }
            }
            else
            {
                foreach (PortalInfo p in list)
                {
                    pList.Add(p);
                }
            }


            return pList;
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

        public static ModuleInfo GetModuleinfo(int moduleId)
        {
            var objMCtrl = new DotNetNuke.Entities.Modules.ModuleController();
            var objMInfo = objMCtrl.GetModule(moduleId);
            return objMInfo;
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

        public static void CreatePortalFolder(DotNetNuke.Entities.Portals.PortalSettings PortalSettings, string FolderName)
        {
            bool blnCreated = false;

            //try normal test (doesn;t work on medium trust, but avoids waiting for GetFolder.)
            try
            {
                blnCreated = System.IO.Directory.Exists(PortalSettings.HomeDirectoryMapPath + FolderName);
            }
            catch (Exception ex)
            {
                var errmsg = ex.ToString();
                blnCreated = false;
            }

            if (!blnCreated)
            {
                FolderManager.Instance.Synchronize(PortalSettings.PortalId, PortalSettings.HomeDirectory, true, true);
                var folderInfo = FolderManager.Instance.GetFolder(PortalSettings.PortalId, FolderName);
                if (folderInfo == null & !string.IsNullOrEmpty(FolderName))
                {
                    //add folder and permissions
                    try
                    {
                        FolderManager.Instance.AddFolder(PortalSettings.PortalId, FolderName);
                    }
                    catch (Exception ex)
                    {
                        var errmsg = ex.ToString();
                    }
                    folderInfo = FolderManager.Instance.GetFolder(PortalSettings.PortalId, FolderName);
                    if ((folderInfo != null))
                    {
                        int folderid = folderInfo.FolderID;
                        DotNetNuke.Security.Permissions.PermissionController objPermissionController = new DotNetNuke.Security.Permissions.PermissionController();
                        var arr = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "");
                        foreach (DotNetNuke.Security.Permissions.PermissionInfo objpermission in arr)
                        {
                            if (objpermission.PermissionKey == "WRITE")
                            {
                                // add READ permissions to the All Users Role
                                FolderManager.Instance.SetFolderPermission(folderInfo, objpermission.PermissionID, int.Parse(DotNetNuke.Common.Globals.glbRoleAllUsers));
                            }
                        }
                    }
                }
            }
        }

        public static DotNetNuke.Entities.Portals.PortalSettings GetCurrentPortalSettings()
        {
            return (DotNetNuke.Entities.Portals.PortalSettings)System.Web.HttpContext.Current.Items["PortalSettings"];
        }

        public static ModuleInfo GetModuleInfo(int tabid, int moduleid)
        {
            return ModuleController.Instance.GetModule(moduleid,tabid,false);
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
        public static Dictionary<int, string> GetTreeTabList()
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, GetCurrentCulture(), true);
            var rtnList = new Dictionary<int, string>();
            return GetTreeTabList(rtnList, tabList, 0, 0);
        }

        private static Dictionary<int, string> GetTreeTabList(Dictionary<int, string> rtnList, List<TabInfo> tabList, int level, int parentid, string prefix = "")
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
                    if (!tInfo.IsDeleted && tInfo.TabPermissions.Count > 2)
                    {
                        rtnList.Add(tInfo.TabID, prefix + "" + tInfo.TabName);
                        GetTreeTabList(rtnList, tabList, level + 1, tInfo.TabID, prefix);
                    }
                }
            }

            return rtnList;
        }


        public static Dictionary<Guid, string> GetTreeTabListOnUniqueId()
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, GetCurrentCulture(), true);
            var rtnList = new Dictionary<Guid, string>();
            return GetTreeTabListOnUniqueId(rtnList, tabList, 0, 0);
        }

        private static Dictionary<Guid, string> GetTreeTabListOnUniqueId(Dictionary<Guid, string> rtnList, List<TabInfo> tabList, int level, int parentid, string prefix = "")
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
                    if (!tInfo.IsDeleted && tInfo.TabPermissions.Count > 2)
                    {
                        rtnList.Add(tInfo.UniqueId, prefix + "" + tInfo.TabName);
                        GetTreeTabListOnUniqueId(rtnList, tabList, level + 1, tInfo.TabID, prefix);
                    }
                }
            }

            return rtnList;
        }

        public static Dictionary<int, string> GetTreeTabListOnTabId()
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, GetCurrentCulture(), true);
            var rtnList = new Dictionary<int, string>();
            return GetTreeTabListOnTabId(rtnList, tabList, 0, 0);
        }

        private static Dictionary<int, string> GetTreeTabListOnTabId(Dictionary<int, string> rtnList, List<TabInfo> tabList, int level, int parentid, string prefix = "")
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
                    if (!tInfo.IsDeleted && tInfo.TabPermissions.Count > 2)
                    {
                        rtnList.Add(tInfo.TabID, prefix + "" + tInfo.TabName);
                        GetTreeTabListOnTabId(rtnList, tabList, level + 1, tInfo.TabID, prefix);
                    }
                }
            }

            return rtnList;
        }


        public static String GetTreeViewTabJSData(String selectTabIdCVS = "")
        {
            var tabList = DotNetNuke.Entities.Tabs.TabController.GetTabsBySortOrder(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, GetCurrentCulture(), true);
            var rtnDataString = "";
            var selecttabidlist = selectTabIdCVS.Split(',');
            rtnDataString = GetTreeViewTabJSData(rtnDataString, tabList, 0, 0, selecttabidlist);
            rtnDataString = rtnDataString.Replace(", children: []", "");
            rtnDataString = "var treeData = [" + rtnDataString + "];";
            return rtnDataString;
        }

        private static String GetTreeViewTabJSData(String rtnDataString, List<TabInfo> tabList, int level, int parentid, String[] selecttabidlist)
        {

            if (level > 20) // stop infinate loop
            {
                return rtnDataString;
            }
            foreach (TabInfo tInfo in tabList)
            {
                var parenttestid = tInfo.ParentId;
                if (parenttestid < 0) parenttestid = 0;
                if (parentid == parenttestid)
                {
                    if (!tInfo.IsDeleted && tInfo.TabPermissions.Count > 2)
                    {
                        var selectedvalue = "false";
                        if (selecttabidlist.Contains(tInfo.TabID.ToString(""))) selectedvalue = "true";
                        rtnDataString += "{title: '" + tInfo.TabName.Replace("'", "&apos;") + "', key:'" + tInfo.TabID + "', selected: " + selectedvalue + ", children: [";
                        rtnDataString = GetTreeViewTabJSData(rtnDataString, tabList, level + 1, tInfo.TabID, selecttabidlist);
                        rtnDataString += "]},";
                    }
                }
            }
            rtnDataString = rtnDataString.TrimEnd(',');
            return rtnDataString;
        }


        public static Dictionary<string, string> GetUserProfileProperties(UserInfo userInfo)
        {
            var prop = new Dictionary<string, string>();
            foreach (DotNetNuke.Entities.Profile.ProfilePropertyDefinition p in userInfo.Profile.ProfileProperties)
            {
                prop.Add(p.PropertyName, p.PropertyValue);
            }
            return prop;
        }

        public static int GetCurrentUserId()
        {
            try
            {
                return UserController.Instance.GetCurrentUserInfo().UserID;
            }
            catch (Exception ex)
            {
                var ms = ex.ToString();
                return 0; // use zero;
            }
        }

        public static string GetCurrentUsername()
        {
            try
            {
                return UserController.Instance.GetCurrentUserInfo().Username;
            }
            catch (Exception ex)
            {
                var ms = ex.ToString();
                return ""; // use zero;
            }
        }


        public static Dictionary<string, string> GetUserProfileProperties(String userId)
        {
            if (!GeneralUtils.IsNumeric(userId)) return null;
            var userInfo = UserController.GetUserById(PortalSettings.Current.PortalId, Convert.ToInt32(userId));
            return GetUserProfileProperties(userInfo);
        }

        public static void SetUserProfileProperties(UserInfo userInfo, Dictionary<string, string> properties)
        {
            foreach (var p in properties)
            {
                userInfo.Profile.SetProfileProperty(p.Key, p.Value);
                UserController.UpdateUser(PortalSettings.Current.PortalId, userInfo);
            }
        }
        public static void SetUserProfileProperties(String userId, Dictionary<string, string> properties)
        {
            if (GeneralUtils.IsNumeric(userId))
            {
                var userInfo = UserController.GetUserById(PortalSettings.Current.PortalId, Convert.ToInt32(userId));
                SetUserProfileProperties(userInfo, properties);
            }
        }

        public static List<int> GetPortals()
        {
            var rtnList = new List<int>();
            var controller = new PortalController();
            foreach (PortalInfo portal in PortalController.Instance.GetPortals())
            {
                rtnList.Add(portal.PortalID);
            }
            return rtnList;
        }

        public static int GetPortalId()
        {
            return PortalSettings.Current.PortalId;
        }

        public static PortalSettings GetPortalSettings(int portalId)
        {
            var controller = new PortalController();
            var portal = controller.GetPortal(portalId);
            return new PortalSettings(portal);
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
            var obj = CacheUtils.GetCache(ckey);
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

                CacheUtils.SetCache(ckey, rtnList);
            }
            return rtnList;
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

        public static bool IsInRole(string role)
        {
            return UserController.Instance.GetCurrentUserInfo().IsInRole(role);
        }

        public static bool IsSuperUser()
        {
            return UserController.Instance.GetCurrentUserInfo().IsSuperUser;
        }

        public static Boolean IsClientOnly()
        {
            if (UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.ClientEditor) && (!UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Editor) && !UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Manager) && !UserController.Instance.GetCurrentUserInfo().IsInRole(DNNrocketRoles.Administrators)))
            {
                return true;
            }
            return false;
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
            var cachekey = "editlang*" + DNNrocketUtils.GetCurrentUserId();
            if (String.IsNullOrEmpty(editlang)) editlang = GetCurrentCulture();
            CacheUtils.SetCache(cachekey, editlang);
            return editlang;
        }

        public static string GetEditCulture()
        {
            var cachekey = "editlang*" + DNNrocketUtils.GetCurrentUserId();
            var rtnLang = HttpContext.Current.Request.QueryString["editlang"];
            if (String.IsNullOrEmpty(rtnLang))
            {
                var obj = CacheUtils.GetCache(cachekey);
                if (obj != null) rtnLang = obj.ToString();
                if (String.IsNullOrEmpty(rtnLang)) rtnLang = GetCurrentCulture();
            }
            SetEditCulture(rtnLang);
            return rtnLang;
        }


        public static string GetCurrentCulture()
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

        [Obsolete("Please use TempDirectoryMapPath() instead.")]
        public static string TempDirectory()
        {
            return PortalSettings.Current.HomeDirectoryMapPath + "DNNrocketTemp";
        }
        public static string TempDirectoryMapPath()
        {
            return PortalSettings.Current.HomeDirectoryMapPath.TrimEnd('\\') + "\\DNNrocketTemp";
        }
        public static string TempDirectoryRel()
        {
            return PortalSettings.Current.HomeDirectory.TrimEnd('/') + "/DNNrocketTemp";
        }

        public static string HomeDirectoryMapPath()
        {
            return PortalSettings.Current.HomeDirectoryMapPath;
        }
        public static string HomeDirectoryRel()
        {
            return PortalSettings.Current.HomeDirectory;
        }
        [Obsolete("Please use HomeDirectoryMapPath() instead.")]
        public static string HomeDirectory()
        {
            return PortalSettings.Current.HomeDirectoryMapPath;
        }
        [Obsolete("Please use HomeDirectoryRel() instead.")]
        public static string HomeRelDirectory()
        {
            return PortalSettings.Current.HomeDirectory;
        }

        public static string DNNrocketThemesDirectory()
        {
            return PortalSettings.Current.HomeDirectoryMapPath + "DNNrocketThemes";
        }
        public static string DNNrocketThemesRelDirectory()
        {
            return PortalSettings.Current.HomeDirectory + "DNNrocketThemes";
        }

        public static string HomeDNNrocketDirectory()
        {
            return PortalSettings.Current.HomeDirectoryMapPath + "DNNrocket";
        }
        public static string HomeDNNrocketRelDirectory()
        {
            return PortalSettings.Current.HomeDirectory + "DNNrocket";
        }

        public static string MapPath(string relpath)
        {
            if (String.IsNullOrWhiteSpace(relpath)) return "";
            return System.Web.Hosting.HostingEnvironment.MapPath(relpath);
        }

        public static string Email()
        {
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

        public static string FileUpload(HttpContext context)
        {
            try
            {

                var strOut = "";
                switch (context.Request.HttpMethod)
                {
                    case "HEAD":
                    case "GET":
                        break;
                    case "POST":
                    case "PUT":
                        strOut = UploadFile(context);
                        break;
                    case "DELETE":
                        break;
                    case "OPTIONS":
                        break;

                    default:
                        context.Response.ClearHeaders();
                        context.Response.StatusCode = 405;
                        break;
                }

                return strOut;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        // Upload file to the server
        public static string UploadFile(HttpContext context)
        {
            // use the userid to try an stop any duplicate files being assiged to incorrect sessions.
            // the userid is prefixed to the filename on upload to the temp folder and then used to get the file back from the temp folder.
            var userid = GetCurrentUserId();

            if (!Directory.Exists(TempDirectory())) {
                Directory.CreateDirectory(TempDirectory());
            }
            if (!Directory.Exists(HomeDNNrocketDirectory()))
            {
                Directory.CreateDirectory(HomeDNNrocketDirectory());
            }

            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            var flist = "";
            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                flist = UploadWholeFile(context, statuses, "", userid);
            }
            else
            {
                flist = UploadPartialFile(headers["X-File-Name"], context, statuses, "", userid);
            }

            return flist;

        }

        // Upload partial file
        public static string UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses, string fileregexpr, int userid)
        {
            Regex fexpr = new Regex(fileregexpr);
            if (fexpr.Match(fileName.ToLower()).Success)
            {
                if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
                var inputStream = context.Request.Files[0].InputStream;

                var systemData = new SystemData();
                var systemprovider = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "systemprovider"));
                var sInfoSystem = systemData.GetSystemByKey(systemprovider);
                var encryptkey = sInfoSystem.GetXmlProperty("genxml/textbox/encryptkey");

                //var fn = EncryptFileName(encryptkey, fileName);
                //var fn = GetUniqueFileName(fileName);
                var fn = fileName;
                fn = userid + "_" + fn;

                var fullName = TempDirectory() + "\\" + fn;

                using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
                {
                    var buffer = new byte[1024];

                    var l = inputStream.Read(buffer, 0, 1024);
                    while (l > 0)
                    {
                        fs.Write(buffer, 0, l);
                        l = inputStream.Read(buffer, 0, 1024);
                    }
                    fs.Flush();
                    fs.Close();
                }
                statuses.Add(new FilesStatus(new System.IO.FileInfo(fullName)));
                return fn;
            }
            return "";
        }

        // Upload entire file
        public static string UploadWholeFile(HttpContext context, List<FilesStatus> statuses, string fileregexpr, int userid)
        {
            var systemData = new SystemData();
            var systemprovider = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "systemprovider"));
            var sInfoSystem = systemData.GetSystemByKey(systemprovider);
            var encryptkey = sInfoSystem.GetXmlProperty("genxml/textbox/encryptkey");
            var flist = "";

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                Regex fexpr = new Regex(fileregexpr);
                if (fexpr.Match(file.FileName.ToLower()).Success)
                {
                    //var fn = EncryptFileName(encryptkey, file.FileName);
                    //var fn = GetUniqueFileName(file.FileName);
                    var fn = file.FileName;
                    fn = userid + "_" + fn;
                    file.SaveAs(TempDirectory() + "\\" + fn);
                    statuses.Add(new FilesStatus(Path.GetFileName(fn), file.ContentLength));
                    flist = flist + ";" + fn;
                }
            }
            return flist.TrimStart(';');
        }

        public static string GetUniqueFileName(string fileName,string folderMapPath, int idx = 1)
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

        public static SimplisityInfo GetSystemByName(string systemprovider)
        {
            var objCtrl = new DNNrocketController();
            return objCtrl.GetByGuidKey(-1, -1, "SYSTEM", systemprovider);
        }

        public static void IncludePageHeaders(ModuleParams moduleParams, Page page, int tabId)
        {
            var appThemeFolder = moduleParams.AppThemeFolder;
            var appThemeVersion = moduleParams.AppThemeVersion;
            var templateRelPath = moduleParams.AppThemeFolderRel;

            if (!String.IsNullOrEmpty(templateRelPath))
            {

            if (!page.Items.Contains("dnnrocketinject")) page.Items.Add("dnnrocketinject", "");
            var fullTemplName = appThemeFolder + "." + appThemeVersion + ".pageheader.cshtml";

                if (!page.Items["dnnrocketinject"].ToString().Contains(fullTemplName + ","))
                {
                    var activePageHeaderTemplate = DNNrocketUtils.GetRazorTemplateData("pageheader.cshtml", moduleParams.AppProjectFolderRel, "SystemThemes/dnnrocketmodule/" + moduleParams.AppThemeFolder, DNNrocketUtils.GetCurrentCulture(), moduleParams.AppThemeVersion, moduleParams.CacheDisbaled);
                    if (activePageHeaderTemplate == null) activePageHeaderTemplate = "";

                    if (activePageHeaderTemplate != null && activePageHeaderTemplate != "")
                    {
                        var settings = new Dictionary<string, string>();
                        var l = new List<object>();
                        l.Add(new SimplisityInfo());
                        var nbRazor = new SimplisityRazor(l, settings, HttpContext.Current.Request.QueryString);
                        nbRazor.PageId = tabId;
                        var strOut = RazorRender(nbRazor, activePageHeaderTemplate, false);
                        PageIncludes.IncludeTextInHeader(page, strOut);
                        page.Items["dnnrocketinject"] = page.Items["dnnrocketinject"] + fullTemplName + ",";


                        // Inject pageheader css link, use Razor Token "AddCssLinkHeader" so we do not duplicate links. [AFTER pageheader.cshtml has rendered]
                        var injectCss = "";
                        var csslist = (List<String>)CacheUtils.GetCache("csslinkdata" + tabId);
                        if (csslist != null)
                        {
                            foreach (var cssRelPath in csslist)
                            {
                                injectCss += "<link rel='stylesheet' href='" + cssRelPath + "' />";
                            }
                            PageIncludes.IncludeTextInHeader(page, injectCss);
                        }

                        // Inject pageheader JS link, use Razor Token "AddJsScriptHeader" so we do not duplicate links. [AFTER pageheader.cshtml has rendered]
                        var injectJS = "";
                        var jslist = (List<String>)CacheUtils.GetCache("jsscriptdata" + tabId);
                        if (jslist != null)
                        {
                            foreach (var jsRelPath in jslist)
                            {
                                injectJS += "<script type='text/javascript' src='" + jsRelPath + "'></script>";
                            }
                            PageIncludes.IncludeTextInHeader(page, injectJS);
                        }
                    }
                }
            }
        }

        public static Dictionary<string,string> GetProviderReturn(string paramCmd,SimplisityInfo systemInfo, DNNrocketInterface rocketInterface, SimplisityInfo postInfo, SimplisityInfo paramInfo, string templateRelPath, string editlang)
        {
            var rtnDic = new Dictionary<string, string>();
            var systemprovider = "";
            if (systemInfo != null)
            {
                systemprovider = systemInfo.GUIDKey;
            }
            if (systemprovider == "" || systemprovider == "systemapi" || systemprovider == "login")
            {
                var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.SystemData.StartConnect");
                rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, postInfo, paramInfo, editlang);
            }
            else
            {
                if (systemprovider != "")
                {
                    // Run API Provider.
                    rtnDic.Add("outputhtml", "API not found: " + systemprovider);
                    if (systemInfo != null)
                    {
                        if (rocketInterface.Exists)
                        {
                            var controlRelPath = rocketInterface.TemplateRelPath;
                            if (controlRelPath == "") controlRelPath = templateRelPath;
                            if (rocketInterface.Assembly == "" || rocketInterface.NameSpaceClass == "")
                            {
                                rtnDic.Remove("outputhtml");
                                rtnDic.Add("outputhtml", "No assembly or namespaceclass defined: " + systemprovider + " : " + rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass);
                            }
                            else
                            {
                                try
                                {
                                    if (postInfo != null)
                                    {
                                        postInfo.SystemId = systemInfo.ItemID;  // systemid is required for index, always add to postdata so it gets passed to the razor templates.
                                    }
                                    var ajaxprov = APInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                                    rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, rocketInterface.Info, postInfo, paramInfo, editlang);
                                }
                                catch (Exception ex)
                                {
                                    rtnDic.Remove("outputhtml");
                                    rtnDic.Add("outputhtml", "ERROR: " + systemprovider + " : " + rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass + " cmd:" + paramCmd + "<br/>" + ex.ToString());
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
                        rtnDic.Add("outputhtml", "No valid system found: " + systemprovider);
                    }
                }
            }
            return rtnDic;
        }

        public static Dictionary<string, SimplisityInfo> EventProviderBefore(string paramCmd, SystemInfoData systemInfoData, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang)
        {
            var rtnDic = new Dictionary<string, SimplisityInfo>();
            if (!systemInfoData.Exists) return rtnDic;  // for systemadmin this may be null
            foreach (var rocketInterface in systemInfoData.EventList)
            {
                if (rocketInterface.IsProvider("eventprovider") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    try
                    {
                        var cacheKey = rocketInterface.Assembly + "," +  rocketInterface.NameSpaceClass;
                        var ajaxprov = (EventInterface)CacheUtils.GetCache(cacheKey);
                        if (ajaxprov == null)
                        {
                            ajaxprov = EventInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                            CacheUtils.SetCache(cacheKey, ajaxprov);
                        }
                        rtnDic = ajaxprov.BeforeEvent(paramCmd, systemInfoData, rocketInterface.Info, postInfo, paramInfo, editlang);
                    }
                    catch (Exception ex)
                    {
                        var err = ex.ToString();  // ignore event provider errors.  The error trap should be in the provider.
                    }
                }
            }
            return rtnDic;
        }

        public static Dictionary<string, string> EventProviderAfter(string paramCmd, SystemInfoData systemInfoData, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang)
        {
            var rtnDic = new Dictionary<string, string>();
            if (!systemInfoData.Exists) return rtnDic;  // for systemadmin this may be null
            foreach (var rocketInterface in systemInfoData.EventList)
            {
                if (rocketInterface.IsProvider("eventprovider") && rocketInterface.Assembly != "" && rocketInterface.NameSpaceClass != "")
                {
                    try
                    {
                        var cacheKey = rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass;
                        var ajaxprov = (EventInterface)CacheUtils.GetCache(cacheKey);
                        if (ajaxprov == null)
                        {
                            ajaxprov = EventInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass);
                            CacheUtils.SetCache(cacheKey, ajaxprov);
                        }
                        rtnDic = ajaxprov.AfterEvent(paramCmd, systemInfoData, rocketInterface.Info, postInfo, paramInfo, editlang);
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
        public static string RenderImageSelect(int imagesize, bool selectsingle = true, bool autoreturn = false, string uploadRelFolder = "images", string razorTemplateName = "ImageSelect.cshtml", string templateControlRelPath = "/DesktopModules/DNNrocket/images/", string themeFolder = "config-w3")
        {
            if (!uploadRelFolder.Contains("/")) uploadRelFolder = DNNrocketUtils.HomeDNNrocketRelDirectory() + "/" + uploadRelFolder;
            var uploadFolderPath = DNNrocketUtils.MapPath(uploadRelFolder);

            var imageModel = new SimplisityRazor();

            imageModel.HeaderData.SetXmlProperty("genxml/hidden/imageselectfolder", uploadRelFolder);
            imageModel.HeaderData.SetXmlProperty("genxml/hidden/imageselectsingle", selectsingle.ToString());
            imageModel.HeaderData.SetXmlProperty("genxml/hidden/imageselectautoreturn", autoreturn.ToString());
            imageModel.HeaderData.SetXmlProperty("genxml/hidden/imageselectsize", imagesize.ToString());

            imageModel.HeaderData.SetXmlProperty("genxml/hidden/uploadfolderpath", uploadFolderPath);
            imageModel.HeaderData.SetXmlProperty("genxml/hidden/uploadrelfolder", uploadRelFolder);

            var imgList = new List<object>();
            foreach (var i in DNNrocketUtils.GetFiles(uploadFolderPath))
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

        public static string RenderDocumentSelect(bool selectsingle = true, bool autoreturn = false, string uploadFolder = "docs", string razorTemplateName = "DocSelect.cshtml", string templateControlRelPath = "/DesktopModules/DNNrocket/documents/", string themeFolder = "config-w3")
        {
            var docModel = new SimplisityRazor();

            docModel.HeaderData.SetXmlProperty("genxml/hidden/documentselectfolder", uploadFolder);
            docModel.HeaderData.SetXmlProperty("genxml/hidden/documentselectsingle", selectsingle.ToString());
            docModel.HeaderData.SetXmlProperty("genxml/hidden/documentselectautoreturn", autoreturn.ToString());

            var uploadFolderPath = DNNrocketUtils.HomeDNNrocketDirectory() + "\\" + uploadFolder;
            if (uploadFolder.Contains("//")) uploadFolderPath = uploadFolder;

            var uploadRelFolderPath = DNNrocketUtils.HomeDNNrocketRelDirectory() + "/" + uploadFolder;
            if (uploadFolder.Contains("//")) uploadRelFolderPath = DNNrocketUtils.MapPath(uploadFolder);

            var docList = new List<object>();
            foreach (var i in DNNrocketUtils.GetFiles(uploadFolderPath))
            {
                var sInfo = new SimplisityInfo();
                sInfo.SetXmlProperty("genxml/name",i.Name);
                sInfo.SetXmlProperty("genxml/fullname", i.FullName);
                sInfo.SetXmlProperty("genxml/extension", i.Extension );
                sInfo.SetXmlProperty("genxml/directoryname", i.DirectoryName);
                sInfo.SetXmlProperty("genxml/lastwritetime", i.LastWriteTime.ToShortDateString());
                sInfo.SetXmlProperty("genxml/relfolder", uploadRelFolderPath);
                sInfo.SetXmlProperty("genxml/relname", uploadRelFolderPath + "/" + i.Name);
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

        public static string GetCurrentWebsiteDomainUrl()
        {
            return PortalSettings.Current.DefaultPortalAlias;
        }


        public static string GetPortalAlias(string lang)
        {
            var padic = CBO.FillDictionary<string, PortalAliasInfo>("HTTPAlias", DotNetNuke.Data.DataProvider.Instance().GetPortalAliases());

            var portalalias = PortalSettings.Current.DefaultPortalAlias;
            foreach (var pa in padic)
            {
                if (pa.Value.PortalID == PortalSettings.Current.PortalId)
                {
                    if (lang == pa.Value.CultureCode)
                    {
                        portalalias = pa.Key;
                    }
                }
            }
            return portalalias;
        }

    }
}
