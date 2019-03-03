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
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace DNNrocketAPI
{
    public static class DNNrocketUtils
    {

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


        public static string RazorRender(Object info, string razorTempl, Boolean debugMode = false)
        {
            var errorPath = "";
            var result = "";
            try
            {
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
                    result = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, info);
                    CacheUtils.SetCache(razorTempl, razorTempl);
                }
                else
                {
                    try
                    {
                        errorPath += "Run>";
                        result = Engine.Razor.Run(hashCacheKey, null, info);
                    }
                    catch (Exception ex)
                    {
                        var errmsg = ex.ToString();
                        errorPath += "RunCompile2>";
                        result = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, info);
                        CacheUtils.SetCache(razorTempl, razorTempl);
                    }
                }

            }
            catch (Exception ex)
            {
                CacheUtils.ClearAllCache();
                result = "CANNOT REBUILD TEMPLATE: errorPath=" + errorPath + " - " + ex.ToString();
            }

            return result;
        }


        public static string GetMd5Hash(string input)
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


        public static string GetRazorTemplateData(string templatename, string templateControlPath, string themeFolder, string lang)
        {
            var controlMapPath = HttpContext.Current.Server.MapPath(templateControlPath);
            var templCtrl = new Simplisity.TemplateEngine.TemplateGetter(PortalSettings.Current.HomeDirectoryMapPath, "Themes\\" + themeFolder, controlMapPath);
            var templ = templCtrl.GetTemplateData(templatename, lang);
            return templ;
        }



        public static void Zip(string zipFileMapPath, List<String> fileMapPathList)
        {
            // Zip up the files - From SharpZipLib Demo Code
            using (var s = new ZipOutputStream(File.Create(zipFileMapPath)))
            {
                s.SetLevel(9); // 0-9, 9 being the highest compression

                byte[] buffer = new byte[4096];

                foreach (string file in fileMapPathList)
                {

                    ZipEntry entry = new
                    ZipEntry(Path.GetFileName(file));

                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);

                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0,
                            buffer.Length);

                            s.Write(buffer, 0, sourceBytes);

                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }


        }

        public static void ZipFolder(string folderName, String zipFileMapPath)
        {
            var zipStream = new ZipOutputStream(File.Create(zipFileMapPath));
            try
            {
                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);
                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
                CompressFolder(folderName, zipStream, folderOffset);
                zipStream.Close();
            }
            catch (Exception ex)
            {
                zipStream.Close();
                throw ex;
            }
        }

        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {

            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                System.IO.FileInfo fi = new System.IO.FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    ZipUtilCopy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
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


        public static void UnZip(string zipFileMapPath, string outputFolder)
        {
            var zipStream = new FileStream(zipFileMapPath, FileMode.Open, FileAccess.Read);
            var zStream = new ZipInputStream(zipStream);
            UnzipResources(zStream, outputFolder);
            zipStream.Close();
            zStream.Close();
        }

        public static void UnzipResources(ZipInputStream zipStream, string destPath)
        {
            try
            {
                ZipEntry objZipEntry;
                string LocalFileName;
                string RelativeDir;
                string FileNamePath;
                objZipEntry = zipStream.GetNextEntry();
                while (objZipEntry != null)
                {
                    LocalFileName = objZipEntry.Name;
                    RelativeDir = Path.GetDirectoryName(objZipEntry.Name);
                    if ((RelativeDir != string.Empty) && (!Directory.Exists(Path.Combine(destPath, RelativeDir))))
                    {
                        Directory.CreateDirectory(Path.Combine(destPath, RelativeDir));
                    }
                    if ((!objZipEntry.IsDirectory) && (!String.IsNullOrEmpty(LocalFileName)))
                    {
                        FileNamePath = Path.Combine(destPath, LocalFileName).Replace("/", "\\");
                        try
                        {
                            if (File.Exists(FileNamePath))
                            {
                                File.SetAttributes(FileNamePath, FileAttributes.Normal);
                                File.Delete(FileNamePath);
                            }
                            FileStream objFileStream = null;
                            try
                            {
                                objFileStream = File.Create(FileNamePath);
                                int intSize = 2048;
                                var arrData = new byte[2048];
                                intSize = zipStream.Read(arrData, 0, arrData.Length);
                                while (intSize > 0)
                                {
                                    objFileStream.Write(arrData, 0, intSize);
                                    intSize = zipStream.Read(arrData, 0, arrData.Length);
                                }
                            }
                            finally
                            {
                                if (objFileStream != null)
                                {
                                    objFileStream.Close();
                                    objFileStream.Dispose();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var errmsg = ex.ToString();

                            //   DnnLog.Error(ex);
                        }
                    }
                    objZipEntry = zipStream.GetNextEntry();
                }
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
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


        public static TabCollection GetPortalTabs(int portalId)
        {
            var portalTabs = (TabCollection)CacheUtils.GetCache("DNNrocket_portalTabs" + portalId.ToString(""));
            if (portalTabs == null)
            {
                var objTabCtrl = new DotNetNuke.Entities.Tabs.TabController();
                portalTabs = objTabCtrl.GetTabsByPortal(portalId);
                CacheUtils.SetCache("DNNrocket_portalTabs" + portalId.ToString(""), portalTabs);
            }
            return portalTabs;
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
                return resDic[resourceExt];
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

        public static void ClearPortalCache(int portalId)
        {
            DataCache.ClearPortalCache(portalId, true);
        }

        public static bool IsInRole(string role)
        {
            return UserController.Instance.GetCurrentUserInfo().IsInRole(role);
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


        public static void SetEditCulture(string editlang)
        {
            if (editlang != null)
            {
                HttpCookie MyCookie = new HttpCookie("editlang");
                MyCookie.Value = editlang;
                HttpContext.Current.Response.Cookies.Add(MyCookie);
            }
        }

        public static string GetEditCulture()
        {
            if (HttpContext.Current.Request.Cookies["editlang"] != null)
            {
                var l = GetCultureCodeList();
                var rtnlang = HttpContext.Current.Request.Cookies["editlang"].Value;
                if (rtnlang == null || rtnlang == "" || !l.Contains(rtnlang))
                {
                    if (l.Count >= 1)
                    {
                        rtnlang = l.First();
                    }
                }
                return rtnlang;

            }
            return GetCurrentCulture();
        }


        public static string GetCurrentCulture()
        {
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
            var l2 = GetCultureCodeList();
            if (l2.Count >= 1)
            {
                return l2.First();
            }
            return "en-US";  // should not happen.
        }

        public static string GetCurrentCountryCode()
        {
            var cc = GetCurrentCulture();
            var c = cc.Split('-');
            var rtn = "";
            if (c.Length > 0) rtn = c[c.Length - 1];
            return rtn;
        }

        public static string TempDirectory()
        {
            return PortalSettings.Current.HomeDirectoryMapPath + "DNNrocketTemp";
        }

        public static string HomeDirectory()
        {
            return PortalSettings.Current.HomeDirectoryMapPath + "DNNrocket";
        }
        public static string HomeRelDirectory()
        {
            return PortalSettings.Current.HomeDirectory + "DNNrocket";
        }

        public static string MapPath(string relpath)
        {
            return System.Web.Hosting.HostingEnvironment.MapPath(relpath);
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
            if (!Directory.Exists(TempDirectory())) {
                Directory.CreateDirectory(TempDirectory());
            }
            if (!Directory.Exists(HomeDirectory()))
            {
                Directory.CreateDirectory(HomeDirectory());
            }

            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                return UploadWholeFile(context, statuses, "");
            }
            else
            {
                return UploadPartialFile(headers["X-File-Name"], context, statuses, "");
            }
        }

        // Upload partial file
        public static string UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses, string fileregexpr)
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

                var fn = DNNrocketUtils.EncryptFileName(encryptkey, fileName);

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
            }
            return "";
        }

        // Upload entire file
        public static string UploadWholeFile(HttpContext context, List<FilesStatus> statuses, string fileregexpr)
        {
            var systemData = new SystemData();
            var systemprovider = HttpUtility.UrlDecode(DNNrocketUtils.RequestParam(context, "systemprovider"));
            var sInfoSystem = systemData.GetSystemByKey(systemprovider);
            var encryptkey = sInfoSystem.GetXmlProperty("genxml/textbox/encryptkey");

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                Regex fexpr = new Regex(fileregexpr);
                if (fexpr.Match(file.FileName.ToLower()).Success)
                {
                    var fn = EncryptFileName(encryptkey, file.FileName);
                    file.SaveAs(TempDirectory() + "\\" + fn);
                    statuses.Add(new FilesStatus(Path.GetFileName(fn), file.ContentLength));
                }
            }
            return "";
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

        public static bool SecurityCheckIsSuperUser()
        {
            return UserController.Instance.GetCurrentUserInfo().IsSuperUser;
        }

        public static bool SecurityCheckCurrentUser(DNNrocketInterface interfaceInfo)
        {
            var user = UserController.Instance.GetCurrentUserInfo();
            return SecurityCheckUser(PortalSettings.Current.PortalId, user.UserID, interfaceInfo);
        }

        public static bool SecurityCheckUser(int portalid, int userid, DNNrocketInterface interfaceInfo)
        {
            var user = UserController.Instance.GetUserById(portalid, userid);

            if (user.IsSuperUser) return true;
            if (interfaceInfo != null)
            {
                if (interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='Administrators']/@value"))
                {
                    if (user.IsInRole("Administrators")) return true;
                }
                if (interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='Manager']/@value"))
                {
                    if (user.IsInRole("Manager")) return true;
                }
                if (interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='Editor']/@value"))
                {
                    if (user.IsInRole("Editor")) return true;
                }
                if (interfaceInfo.GetXmlPropertyBool("genxml/checkboxlist/securityroles/chk[@data='ClientEditor']/@value"))
                {
                    if (user.IsInRole("ClientEditor")) return true;
                }
            }
            var ps = new PortalSecurity();
            ps.SignOut();

            return false;
        }

        public static void SignUserOut()
        {
            var ps = new PortalSecurity();
            ps.SignOut();
        }

        public static void IncludePageHeaders(int moduleid, Page page, String moduleName, string templateControlRelPath, String razortemplate = "", String theme = "")
        {
            if (!page.Items.Contains("dnnrocketinject")) page.Items.Add("dnnrocketinject", "");

            var settignInfo = DNNrocketUtils.GetModuleSettings(moduleid);

            if (theme == "") theme = settignInfo.GetXmlProperty("genxml/dropdownlist/themefolder");
            var fullTemplName = theme + ".pageheader.cshtml";

            if (!page.Items["dnnrocketinject"].ToString().Contains(fullTemplName + "." + moduleName + ","))
            {
                var debug = settignInfo.GetXmlPropertyBool("genxml/checkbox/debugmode");
                var nbi = new SimplisityInfo();
                nbi.Lang = GetCurrentCulture();

                var razorTempl = DNNrocketUtils.GetRazorTemplateData(razortemplate, templateControlRelPath, theme, DNNrocketUtils.GetCurrentCulture());


                var strOut = DNNrocketUtils.RazorDetail(razorTempl, new SimplisityInfo(), settignInfo.ToDictionary());

                if (razorTempl != "")
                {
                    PageIncludes.IncludeTextInHeader(page, strOut);
                    page.Items["dnnrocketinject"] = page.Items["dnnrocketinject"] + fullTemplName + "." + moduleName + ",";
                }
            }
        }

        public static SimplisityInfo GetModuleSettings(int moduleid, Boolean useCache = true)
        {
            var rtnCache =  CacheUtils.GetCache("dnnrocketmodsettings*" + moduleid);
            if (rtnCache != null && useCache) return (SimplisityInfo)rtnCache;
            // get template
            if (GeneralUtils.IsNumeric(moduleid) && Convert.ToInt32(moduleid) > 0)
            {
                var objCtrl = new DNNrocketController();
                var dataRecord = objCtrl.GetByType(PortalSettings.Current.PortalId, Convert.ToInt32(moduleid), "MODULESETTINGS");
                if (dataRecord == null) dataRecord = new SimplisityInfo();
                if (dataRecord.TypeCode == "MODULESETTINGS")
                {
                    // only add to cache if we have a valid settings record, LocalUtils.IncludePageHeaders may be called before the creation of the settings.
                    CacheUtils.SetCache("dnnrocketmodsettings*" + moduleid, dataRecord);
                }
                return dataRecord;
            }
            return new SimplisityInfo();
        }

        public static void UpdateModuleSettings(SimplisityInfo settings)
        {
            // get template
            if (settings.ModuleId > 0)
            {
                var objCtrl = new DNNrocketController();

                objCtrl.Update(settings);
                CacheUtils.RemoveCache("dnnrocketmodsettings*" + settings.ModuleId);
            }
        }

        public static Dictionary<string,string> GetProviderReturn(string paramCmd,SimplisityInfo systemInfo, DNNrocketInterface rocketInterface, SimplisityInfo postInfo, string templateRelPath, string editlang)
        {
            var rtnDic = new Dictionary<string, string>();
            var systemprovider = "";
            if (systemInfo != null)
            {
                systemprovider = systemInfo.GUIDKey;
            }
            if (systemprovider == "" || systemprovider == "systemapi" || systemprovider == "login")
            {
                var ajaxprov = APInterface.Instance("DNNrocketSystemData", "DNNrocket.SystemData.startconnect", templateRelPath);
                rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, null, postInfo, HttpContext.Current.Request.UserHostAddress, editlang);
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
                                    var ajaxprov = APInterface.Instance(rocketInterface.Assembly, rocketInterface.NameSpaceClass, controlRelPath);
                                    rtnDic = ajaxprov.ProcessCommand(paramCmd, systemInfo, rocketInterface.Info, postInfo, HttpContext.Current.Request.UserHostAddress, editlang);
                                }
                                catch (Exception ex)
                                {
                                    rtnDic.Remove("outputhtml");
                                    rtnDic.Add("outputhtml", "No valid assembly found: " + systemprovider + " : " + rocketInterface.Assembly + "," + rocketInterface.NameSpaceClass + "<br/>" + ex.ToString());
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


    }
}
