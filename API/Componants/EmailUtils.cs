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

namespace DNNrocketAPI
{
    public static class EmailUtils
    {

        public static void SendEmail(string emailbody, string toEmail, string emailsubject, string fromEmail, string lang, string attchments)
        {
            if (lang == "") lang = DNNrocketUtils.GetCurrentCulture();
            var emaillist = toEmail;
            if (emaillist != "")
            {
                var emailarray = emaillist.Split(',');
                foreach (var email in emailarray)
                {
                    if (!string.IsNullOrEmpty(email.Trim()) && GeneralUtils.IsEmail(fromEmail.Trim()) && GeneralUtils.IsEmail(email.Trim()))
                    {
                        // multiple attachments as csv with "|" seperator
                        DotNetNuke.Services.Mail.Mail.SendMail(fromEmail.Trim(), email.Trim(), "", emailsubject, emailbody, attchments, "HTML", "", "", "", "");
                    }
                }
            }

        }

    }
}
