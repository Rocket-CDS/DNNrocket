using DotNetNuke.Web.DDRMenu;
using RocketRazorEngine;
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
using System.Web.UI;
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
                try
                {
                    processResult.RenderedText = Engine.RunCompile(razorTempl, templateKey);
                }
                catch (TemplateCompilationException ex)
                {
                    // Get full error details
                    var fullError = Engine.GetCompilationErrors(ex);

                    // Write to a file you can check
                    File.WriteAllText(PortalUtils.TempDirectoryMapPath() + "\\RazorCompileError.txt", fullError);

                    processResult.RenderedText = "";
                    processResult.StatusCode = "01";
                    processResult.ErrorMsg = "<div style='background:red;color:white;padding:20px;'>" +
                            "<h3>Template Compilation Failed</h3>" +
                            "<pre>" + System.Web.HttpUtility.HtmlEncode(fullError) + "</pre>" +
                            "</div>";
                    LogUtils.LogSystem(ex.ToString());
                    LogUtils.LogException(ex);
                }
                return processResult;
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
                        Engine.RunCompile(razorTempl, hashCacheKey);
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
