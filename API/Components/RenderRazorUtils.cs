using DotNetNuke.Web.DDRMenu;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Simplisity;
using Simplisity.TemplateEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using static System.Net.Mime.MediaTypeNames;

namespace DNNrocketAPI.Components
{
    public static class RenderRazorUtils
    {
        #region "render razor"

        private static RazorProcessResult RazorProcessRunCompile(SimplisityRazor model, string razorTempl, Boolean debugMode = false)
        {
            var processResult = new RazorProcessResult();
            processResult.StatusCode = "00";
            processResult.RenderedText = "";
            processResult.ErrorMsg = "";

            if (String.IsNullOrEmpty(razorTempl)) return processResult;
            var templateKey = GeneralUtils.GetMd5Hash(razorTempl);
            try
            {
                if (HttpContext.Current == null) // can be null if ran from scheduler.
                {
                    try
                    {
                        var config = new TemplateServiceConfiguration();
                        //config.Debug = debugMode;
                        config.BaseTemplateType = typeof(RazorEngineTokens<>);
                        config.DisableTempFileLocking = true;
                        IRazorEngineService service = RazorEngineService.Create(config);
                        Engine.Razor = service;

                        LogUtils.LogSystem("START - (HttpContext.Current == null) RunCompile: " + templateKey);
                        processResult.RenderedText = Engine.Razor.RunCompile(razorTempl, templateKey, null, model);
                        LogUtils.LogSystem("END - (HttpContext.Current == null) RunCompile: " + templateKey);
                    }
                    catch (Exception ex)
                    {
                        processResult.RenderedText = "";
                        processResult.StatusCode = "01";
                        processResult.ErrorMsg = ex.ToString();
                        LogUtils.LogSystem(ex.ToString());
                        LogUtils.LogException(ex, true); // clear cache so we see the error.
                        LogUtils.LogSystem("WARNING!!! - Cache cleared for the razor template error.  THIS MAY CAUSE THE SYSTEM TO GO SLOW IF IT KEEPS HAPPENING.");
                    }
                    return processResult;
                }
                else
                {
                    var service = (IRazorEngineService)HttpContext.Current.Application.Get("DNNrocketIRazorEngineService");
                    if (service == null)
                    {
                        var config = new TemplateServiceConfiguration();
                        //config.Debug = debugMode;
                        config.BaseTemplateType = typeof(RazorEngineTokens<>);
                        config.DisableTempFileLocking = true;
                        service = RazorEngineService.Create(config);
                        HttpContext.Current.Application.Set("DNNrocketIRazorEngineService", service);
                    }
                    Engine.Razor = service;
                    var israzorCached = CacheUtils.GetCache("rzcache_" + templateKey); // get a cache flag for razor compile.
                    if (israzorCached == null || (string)israzorCached != razorTempl)
                    {
                        var t = DateTime.Now;

                        //LogUtils.LogSystem("START - RunCompile: " + templateKey);
                        processResult.RenderedText = Engine.Razor.RunCompile(razorTempl, templateKey, null, model);
                        CacheUtils.SetCache("rzcache_" + templateKey, razorTempl);
                        //if (t.AddSeconds(10) < DateTime.Now)
                        //{
                        //    LogUtils.LogSystem("------------------------- Compile over 10s ---------------------------------------- ");
                        //}
                        //LogUtils.LogSystem("END - RunCompile Time: " + DateTime.Now.Subtract(t).TotalSeconds + "s" + templateKey);

                    }
                    else
                    {
                        processResult.RenderedText = Engine.Razor.Run(templateKey, null, model);
                    }
                }
            }
            catch (Exception ex)
            {
                processResult.RenderedText = "<div>" + ex.Message + " templateKey='" + templateKey + "'</div>";
            }

            return processResult;
        }
        public static void PreCompileRazorFolder(string folderMapPath)
        {
            foreach (var f in Directory.GetFiles(folderMapPath,"*.cshtml", SearchOption.AllDirectories))
            {
                var c = CacheUtils.GetCache(f);
                if (c == null )
                {
                    var razorTempl = FileUtils.ReadFile(f);
                    var hashCacheKey = GeneralUtils.GetMd5Hash(razorTempl);
                    try
                    {
                        Engine.Razor.Compile(razorTempl, hashCacheKey, null);
                        LogUtils.LogSystem("PreCompiled Razor: " + f);
                    }
                    catch (Exception)
                    {
                        // ignore, expect to fail on some templates
                    }
                    CacheUtils.SetCache(f, hashCacheKey);
                }
            }
        }


        #region "Obsolete Razor method"
        [Obsolete("Use RazorProcessData(...) instead")]
        public static RazorProcessResult RazorProcess(SimplisityRazor model, string razorTempl, Boolean debugMode = false)
        {
            return RazorProcessData(razorTempl, model.DataObjects, model.Settings, model.SessionParamsData, debugMode);
        }

        [Obsolete("Use RazorProcessData(...) instead")]
        public static string RazorRender(SimplisityRazor model, string razorTempl, Boolean debugMode = false)
        {
            var errorPath = "";
            var result = "";
            var errmsg = "";
            try
            {
                if (razorTempl == null || razorTempl == "") return "";
                if (HttpContext.Current == null) // can be null if ran from scheduler.
                {
                    return RazorRunCompile(model, razorTempl);
                }
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
                var hashCacheKey = GeneralUtils.GetMd5Hash(razorTempl);

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
        /// <summary>
        /// No Cache, use when HttpContext.Current.Application is null
        /// </summary>
        /// <param name="model"></param>
        /// <param name="razorTempl"></param>
        /// <returns></returns>
        [Obsolete("Use RazorProcessData(...) instead")]
        private static string RazorRunCompile(SimplisityRazor model, string razorTempl)
        {
            try
            {
                if (razorTempl == null || razorTempl == "") return "";
                var hashCacheKey = GeneralUtils.GetMd5Hash(razorTempl);
                return Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
            }
            catch (Exception ex)
            {
                return "ERROR in RazorRunCompile : " + ex.ToString();
            }
        }
        [Obsolete("Use RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false) instead")]
        public static string RazorDetail(string razorTemplate, object obj, Dictionary<string, object> dataObjects, SessionParams sessionParams = null, Dictionary<string, string> settings = null, bool cacheOff = false)
        {
            return RazorObjectRender(razorTemplate, obj, dataObjects, settings, sessionParams, cacheOff);
        }
        [Obsolete("Use RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false) instead")]
        public static string RazorDetail(string razorTemplate, object obj, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            return RazorObjectRender(razorTemplate, obj, null, settings, sessionParams, debugmode);
        }
        [Obsolete("Use RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false) instead")]
        public static string RazorDetail(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)
        {
            return RazorDetail(appTheme.GetTemplate(razorTemplateName), model, cacheOff);
        }
        [Obsolete("Use RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false) instead")]
        public static string RazorDetail(string razorTemplate, SimplisityRazor model, bool debugMode = false)
        {
            var pr = RenderRazorUtils.RazorProcess(model, razorTemplate, debugMode);
            return pr.RenderedText;
        }
        [Obsolete("Use RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false) instead")]
        public static string RazorObjectRender(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            var rtnStr = "";
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>();

                if (obj == null) obj = new SimplisityInfo();
                var l = new List<object>();
                l.Add(obj);

                var nbRazor = new SimplisityRazor(l, settings);
                nbRazor.SessionParamsData = sessionParams;
                nbRazor.DataObjects = dataObjects;
                var pr = RenderRazorUtils.RazorProcess(nbRazor, razorTemplate, debugmode);
                rtnStr = pr.RenderedText;
            }

            return rtnStr;
        }
        [Obsolete("Use RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false) instead")]
        public static string RazorList(string razorTemplate, List<object> objList, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            var strOut = "";
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>();
                var nbRazor = new SimplisityRazor(objList, settings);
                nbRazor.SessionParamsData = sessionParams;
                var pr = RenderRazorUtils.RazorProcess(nbRazor, razorTemplate, debugmode);
                strOut = pr.RenderedText;
            }
            return strOut;
        }
        #endregion

        #region "Unified RazorProcess"

        public static RazorProcessResult RazorProcessData(SimplisityRazor model, string razorTempl, Boolean debugMode = false)
        {
            return RazorProcessData(razorTempl, model.List, model.DataObjects, model.Settings, model.SessionParamsData, debugMode);
        }
        public static RazorProcessResult RazorProcessData(string razorTemplate, Dictionary<string, object> dataObjects, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            return RazorProcessData(razorTemplate, null, dataObjects, settings, sessionParams, debugmode);
        }
        public static RazorProcessResult RazorProcessData(string razorTemplate, List<object> modelList, Dictionary<string, object> dataObjects, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            if (!String.IsNullOrWhiteSpace(razorTemplate))
            {
                if (settings == null) settings = new Dictionary<string, string>();
                var sRazor = new SimplisityRazor();
                sRazor.SessionParamsData = sessionParams;
                sRazor.DataObjects = dataObjects;
                sRazor.Settings = settings;
                sRazor.List = modelList;

                razorTemplate = ReplaceInjectTokens(razorTemplate, sRazor);

                return RazorProcessRunCompile(sRazor, razorTemplate, debugmode);
            }
            return new RazorProcessResult();
        }
        private static string ReplaceInjectTokens(string templateData, SimplisityRazor sRazor, List<string> templateList = null)
        {
            if (templateData.Contains("[INJECT:"))
            {
                if (templateList == null) templateList = new List<string>();
                FastReplacer fr = new FastReplacer("[INJECT:", "]", false);
                fr.Append(templateData);
                var tokenList = fr.GetTokenStrings();
                foreach (var token in tokenList)
                {
                    if (templateList.Contains(token))
                    {
                        return "<div style='color:red;background-color:white'>ERROR: Circular reference to a template. [" + token + "]</div>";
                    }
                    else
                    {
                        templateList.Add(token);
                        var tokenSplit = token.Split(',');
                        var datObjectName = tokenSplit[0].Trim();
                        var templateName = tokenSplit[1].Trim();
                        var appTheme = (AppThemeBase)sRazor.GetDataObject(datObjectName);
                        if (appTheme != null)
                        {
                            var moduleRef = "";
                            if (sRazor.SessionParamsData != null) moduleRef = sRazor.SessionParamsData.ModuleRef;
                            var injectTemplate = appTheme.GetTemplate(templateName, moduleRef);
                            if (injectTemplate != "")
                            {
                                injectTemplate = ReplaceInjectTokens(injectTemplate, sRazor, templateList);

                                //remove top razor section.  search for '<!--inject-->' or for the  first '<div' and use that as the starting point for the inject template.
                                var searchStr = "<!--inject-->";
                                int startIndex = injectTemplate.IndexOf(searchStr);
                                if (startIndex == -1)
                                    startIndex = injectTemplate.IndexOf("<div");
                                else
                                    startIndex = startIndex + searchStr.Length;

                                var rtn = "";
                                if (startIndex > -1) rtn = injectTemplate.Substring(startIndex);
                                fr.Replace("[INJECT:" + token + "]", rtn);
                            }
                        }
                    }
                }
                templateData  = fr.ToString();
            }
            return templateData;
        }
        public static RazorProcessResult RazorProcessData(string razorTemplate, object obj, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            if (razorTemplate != "")
            {
                var objList = new List<object>();
                objList.Add(obj);
                return RazorProcessList(razorTemplate, objList, dataObjects, settings, sessionParams, debugmode);
            }
            return new RazorProcessResult();
        }
        public static RazorProcessResult RazorProcessList(string razorTemplate, List<object> objList, Dictionary<string, object> dataObjects = null, Dictionary<string, string> settings = null, SessionParams sessionParams = null, bool debugmode = false)
        {
            if (razorTemplate != "")
            {
                if (settings == null) settings = new Dictionary<string, string>();
                var nbRazor = new SimplisityRazor(objList, settings);
                nbRazor.SessionParamsData = sessionParams;
                nbRazor.DataObjects = dataObjects;
                return RenderRazorUtils.RazorProcessData(nbRazor, razorTemplate, debugmode);
            }
            return new RazorProcessResult();
        }

        #endregion

        public static SimplisityInfo GetResxPaths(Dictionary<string, List<string>> processdata)
        {
            var s = new SimplisityInfo();
            if (processdata.ContainsKey("resourcepath"))
            {
                var l = processdata["resourcepath"];
                foreach (var r in l)
                {
                    var sr = new SimplisityRecord();
                    sr.SetXmlProperty("genxml/path", r);
                    s.AddRecordListItem("resxpath", sr);
                }
            }
            return s;
        }


        #endregion

        #region "templates"

        [Obsolete("GetRazorTemplateData() is deprecated, please use AppThemeLimpet.GetTemplate() instead.")]
        public static string GetRazorTemplateData(string templatename, string templateControlPath, string themeFolder, string lang, string versionFolder = "1.0", bool debugMode = false)
        {
            var templCtrl = GetTemplateEngine("", templateControlPath, themeFolder, lang, versionFolder, debugMode);
            var templ = templCtrl.GetTemplateData(templatename, lang);
            return templ;
        }
        [Obsolete("GetSystemRazorTemplate() is deprecated, please use AppThemeLimpet.GetTemplate() instead.")]
        public static string GetSystemRazorTemplate(string systemKey, string templatename, string templateControlPath, string themeFolder, string lang, string versionFolder = "1.0", bool debugMode = false)
        {
            var templCtrl = GetTemplateEngine(systemKey, templateControlPath, themeFolder, lang, versionFolder, debugMode);
            var templ = templCtrl.GetTemplateData(templatename, lang);
            return templ;
        }

        #endregion

        #region "private methods"

        private static TemplateGetter GetTemplateEngine(string systemKey, string templateControlPath, string themeFolder, string lang, string versionFolder = "1.0", bool debugMode = false)
        {
            var cacheKey = templateControlPath + "*" + themeFolder + "*" + lang + "*" + versionFolder + "*" + debugMode + "*" + PortalUtils.GetPortalId();
            var templCtrl = CacheUtils.GetCache(cacheKey);
            if (templCtrl == null)
            {
                var controlMapPath = DNNrocketUtils.MapPath(templateControlPath);
                var themeFolderPath = themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder;
                var RocketThemes = PortalUtils.DNNrocketThemesDirectoryMapPath(-1);
                templCtrl = new TemplateGetter(RocketThemes, themeFolderPath, controlMapPath, debugMode);
                CacheUtils.SetCache(cacheKey, templCtrl);
            }
            return (TemplateGetter)templCtrl;
        }

        #endregion


    }
}
