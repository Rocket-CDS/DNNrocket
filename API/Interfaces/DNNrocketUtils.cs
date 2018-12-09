using NBrightCore.render;
using NBrightDNN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Simplisity;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using RazorEngine;
using System.Security.Cryptography;
using NBrightCore.common;
using NBrightBuy.render;

namespace DNNrocketAPI
{
    public static class DNNrocketUtils
    {
        public static NBrightInfo GetAjaxFields(HttpContext context)
        {
            var strIn = HttpUtility.UrlDecode(RequestParam(context, "inputxml"));
            var xmlData = "";
            xmlData = GenXmlFunctions.GetGenXmlByAjax(strIn, "", "genxml", true);
            var objInfo = new NBrightInfo();
            objInfo.ItemID = -1;
            objInfo.TypeCode = "AJAXDATA";
            objInfo.XMLData = xmlData;
            return objInfo;
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


        public static string RazorRender(Object info, string razorTempl, string templateKey, Boolean debugMode = false)
        {
            var result = "";
            try
            {
                var service = (IRazorEngineService)HttpContext.Current.Application.Get("NBrightBuyIRazorEngineService");
                if (service == null)
                {
                    // do razor test
                    var config = new TemplateServiceConfiguration();
                    config.Debug = debugMode;
                    config.BaseTemplateType = typeof(NBrightBuyRazorTokens<>);
                    service = RazorEngineService.Create(config);
                    HttpContext.Current.Application.Set("NBrightBuyIRazorEngineService", service);
                }
                Engine.Razor = service;
                var hashCacheKey = GetMd5Hash(razorTempl);
                var israzorCached = Utils.GetCache("nbrightbuyrzcache_" + hashCacheKey); // get a cache flag for razor compile.
                if (israzorCached == null || (string)israzorCached != razorTempl || debugMode)
                {
                    result = Engine.Razor.RunCompile(razorTempl, hashCacheKey, null, info);
                    Utils.SetCache("nbrightbuyrzcache_" + hashCacheKey, razorTempl);
                }
                else
                {
                    result = Engine.Razor.Run(hashCacheKey, null, info);
                }

            }
            catch (Exception ex)
            {
                result = ex.ToString();
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


    }
}
