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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.Skins;
using System.Text.RegularExpressions;

namespace DNNrocketAPI.Components
{
    public class PageIncludes
    {

        public static void IncludeScripts(int portalId, string modulePath, Page page, string jsIncludeList, string cssIncludeList)
        {

            //custom js includes
            var jsList = jsIncludeList.Split(',');
            foreach (var j in jsList)
            {
                if (!string.IsNullOrEmpty(j))
                {
                    if (j.StartsWith(@"/"))
                    {
                        RegisterJs("GenCSS" + j, j, modulePath, page);
                    }
                    else
                    {
                        char[] charsToTrim = { '/', '\\', '.', ' ' };
                        RegisterJs("GenCSS" + j, j, modulePath.TrimEnd(charsToTrim), page);
                    }

                }
            }

            //custom css
            var cssList = cssIncludeList.Split(',');
            foreach (var c in cssList)
            {
                if (!string.IsNullOrEmpty(c))
                {
                    if (c.StartsWith(@"/"))
                    {
                        IncludeCssFile(page, "GenCSS" + c, c);
                    }
                    else
                    {
                        char[] charsToTrim = { '/', '\\', '.', ' ' };
                        IncludeCssFile(page, "GenCSS" + c, modulePath.TrimEnd(charsToTrim) + @"/css/" + c);
                    }
                }
            }

        }

        public static void RegisterJs(string regName, string jsFileName, string modulePath, Page page)
        {
            if (!string.IsNullOrEmpty(jsFileName))
            {
                if (!page.ClientScript.IsClientScriptIncludeRegistered(regName))
                {
                    if (jsFileName.StartsWith(@"/"))
                    {
                        IncludeJsFile(page, regName, jsFileName);
                    }
                    else
                    {
                        char[] charsToTrim = { '/', '\\', '.', ' ' };
                        IncludeJsFile(page, regName, modulePath.TrimEnd(charsToTrim) + @"/js/" + jsFileName);
                    }
                }
            }
        }

        public static void LoadJQueryCode(string jCodeKey, string jCode, Page page)
        {
            if (!string.IsNullOrEmpty(jCode))
            {
                if (!jCode.ToLower().StartsWith("<script"))
                {
                    jCode = System.Web.HttpUtility.HtmlDecode(jCode);
                    jCode = "<script language=\"javascript\" type=\"text/javascript\">" + jCode + "</script>";
                }

                page.ClientScript.RegisterStartupScript(page.GetType(), jCodeKey, jCode);
            }
        }

        public static void IncludeJsFile(Page page, string id, string href)
        {
            if (!string.IsNullOrEmpty(href))
            {
                string strId = id.Replace(@"/", "");
                var jsCtrl = page.Header.FindControl(strId);
                if (jsCtrl == null)
                {
                    jsCtrl = new LiteralControl("<script src=\"" + href + "\" type=\"text/javascript\"></script>");
                    jsCtrl.ID = strId;
                    page.Header.Controls.Add(jsCtrl); // add after jQuery
                }
            }

            // This adds script into the body, works but can cause problems.
            //if (!string.IsNullOrEmpty(href))
            //{
            //    if (!page.ClientScript.IsClientScriptIncludeRegistered(id))
            //    {
            //        page.ClientScript.RegisterClientScriptInclude(id, href);
            //    }
            //}
        }

        public static bool IgnoreOnSkin(string skinSrc, string ignoreonskin)
        {
            if (String.IsNullOrEmpty(ignoreonskin)) return false;
            var l = ignoreonskin.Split(',');
            var rtn = false;
            foreach (var i in l)
            {
                if (i != "")
                {
                    if (skinSrc.ToLower().Contains(i.ToLower())) return true;
                }
            }
            return rtn;
        }
        public static void IncludeCssFile(Page page, string id, string href)
        {
            if (!string.IsNullOrEmpty(href))
            {
                string strId = id.Replace(@"/", "");
                var cssLink = (HtmlLink)page.Header.FindControl(strId);
                if (cssLink == null)
                {
                    cssLink = new HtmlLink { ID = strId };
                    cssLink.Attributes.Add("rel", "stylesheet");
                    cssLink.Attributes.Add("type", "text/css");
                    cssLink.Href = href;
                    page.Header.Controls.AddAt(0, cssLink);
                }
            }
        }

        /// <summary>
        /// Oringal code for CSS removal from 40 fingers.  (https://www.40fingers.net/Products/DNN-Stylehelper)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sFileName"></param>
        public static void RemoveCssFile(Page page, string sFileName)
        {
            Control oCSS = page.FindControl("CSS");

            foreach (Control oControl in oCSS.Controls)
            {
                switch (oControl.GetType().ToString())
                {
                    case "System.Web.UI.HtmlControls.HtmlLink":
                        {
                            HtmlLink oLink = (HtmlLink)oControl;
                            if (CheckStringFound(oLink.Attributes["href"], sFileName))
                                oLink.Visible = false;
                            break;
                        }
                }
            }

            // For the control panel CSS
            Control oHead = page.FindControl("Head");
            foreach (Control oControl in oHead.Controls)
            {
                switch (oControl.GetType().ToString())
                {
                    case "System.Web.UI.LiteralControl":
                        {
                            LiteralControl oLink = (LiteralControl)oControl;
                            if (CheckStringFound(oLink.Text, sFileName))
                                oLink.Visible = false;
                            break;
                        }
                }
            }

            // For Dnn 6.1+
            Control oIncludes = page.FindControl("ClientResourceIncludes");
            if (oIncludes != null)
            {

                // Get list of child items client resource controls
                List<string> lstControl2Remove = new List<string>();


                int iItems = oIncludes.Controls.Count - 1;

                // Loop though Items reverse
                for (int i = iItems; i >= 0; i += -1)
                {
                    Control oCssControl = oIncludes.Controls[i];

                    // Check if it's a CssInclude
                    switch (oCssControl.GetType().ToString())
                    {
                        case "DotNetNuke.Web.Client.ClientResourceManagement.DnnCssInclude":
                            {
                                DotNetNuke.Web.Client.ClientResourceManagement.DnnCssInclude oCSSRemove = (DotNetNuke.Web.Client.ClientResourceManagement.DnnCssInclude)oCssControl;
                                // Check the path
                                if (CheckStringFound(oCSSRemove.FilePath, sFileName))
                                    // Add to the list of controls to remove
                                    oIncludes.Controls.RemoveAt(i);
                                break;
                            }
                    }
                }
            }
        }
        private static bool CheckStringFound(string sIn, string sCheck)
        {
            try
            {
                if (sCheck == "/")
                    sCheck = "/.*";
                return Regex.IsMatch(sIn, sCheck, RegexOptions.IgnoreCase);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static void IncludeCanonicalLink(Page page, string href)
        {
            if (!string.IsNullOrEmpty(href))
            {
                var cLink = new HtmlLink();
                cLink.Attributes.Add("rel", "canonical");
                cLink.Href = href;
                page.Header.Controls.Add(cLink);
            }
        }

        public static void IncludeTextInHeader(Page page, string TextToInclude)
        {
            if (TextToInclude != "") page.Header.Controls.Add(new LiteralControl(TextToInclude));
        }
        public static void IncludeTextInHeaderAt(Page page, string TextToInclude, int addAt = 0)
        {
            if (TextToInclude != "") page.Header.Controls.AddAt(addAt, new LiteralControl(TextToInclude));
        }
    }
}
