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

namespace DNNrocketAPI.Components
{

    public static class RenderRazorUtils
    {
        #region "render razor"

        /// <summary>
        /// render a razor template and return status.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="razorTempl"></param>
        /// <param name="debugMode"></param>
        /// <returns></returns>
        public static RazorProcessResult RazorProcess(SimplisityRazor model, string razorTempl, Boolean debugMode = false)
        {
            var processResult = new RazorProcessResult();
            processResult.StatusCode = "00";
            processResult.RenderedText = "";
            processResult.ErrorMsg = "";
            try
            {
                if (String.IsNullOrEmpty(razorTempl)) return processResult;
                var hashCacheKey = GeneralUtils.GetMd5Hash(razorTempl);

                if (HttpContext.Current == null) // can be null if ran from scheduler.
                {
                    try
                    {
                        processResult.RenderedText = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
                    }
                    catch (Exception ex)
                    {
                        processResult.RenderedText = "";
                        processResult.StatusCode = "01";
                        processResult.ErrorMsg = ex.ToString();
                    }
                    return processResult;
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

                var israzorCached = CacheUtilsDNN.GetCache(razorTempl); // get a cache flag for razor compile.
                if (israzorCached == null || (string)israzorCached != razorTempl || debugMode)
                {
                    try
                    {
                        processResult.RenderedText = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
                        CacheUtilsDNN.SetCache(razorTempl, razorTempl);
                    }
                    catch (Exception ex)
                    {
                        processResult.RenderedText = "";
                        processResult.StatusCode = "02";
                        processResult.ErrorMsg = ex.ToString();
                    }
                }
                else
                {
                    try
                    {
                        processResult.RenderedText = Engine.Razor.Run(hashCacheKey, null, model);
                    }
                    catch (Exception)
                    {
                        // try again with compile, the template cache may have got removed.
                        try
                        {
                            processResult.RenderedText = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, model);
                            CacheUtilsDNN.SetCache(razorTempl, razorTempl);
                        }
                        catch (Exception ex1)
                        {
                            processResult.StatusCode = "03";
                            processResult.ErrorMsg = ex1.ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CacheUtilsDNN.ClearAllCache();
                processResult.RenderedText = "";
                processResult.StatusCode = "03";
                processResult.ErrorMsg = ex.ToString();
            }

            return processResult;
        }


        [Obsolete("Use RazorProcess(SimplisityRazor model, string razorTempl, Boolean debugMode = false) instead")]
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
        /// <summary>
        /// No Cache, use when HttpContext.Current.Application is null
        /// </summary>
        /// <param name="model"></param>
        /// <param name="razorTempl"></param>
        /// <returns></returns>
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

        #region "Obsolete Razor method"
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
                return RenderRazorUtils.RazorProcess(nbRazor, razorTemplate, debugmode);
            }
            return new RazorProcessResult();
        }

        #endregion


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
            var templCtrl = CacheUtilsDNN.GetCache(cacheKey);
            if (templCtrl == null)
            {
                var controlMapPath = DNNrocketUtils.MapPath(templateControlPath);
                var themeFolderPath = themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder + "\\" + versionFolder;
                if (!Directory.Exists(controlMapPath.TrimEnd('\\') + "\\" + themeFolderPath)) themeFolderPath = "Themes\\" + themeFolder;
                var RocketThemes = PortalUtils.DNNrocketThemesDirectoryMapPath(-1);
                templCtrl = new TemplateGetter(RocketThemes, themeFolderPath, controlMapPath, debugMode);
                CacheUtilsDNN.SetCache(cacheKey, templCtrl);
            }
            return (TemplateGetter)templCtrl;
        }

        #endregion


    }
}
