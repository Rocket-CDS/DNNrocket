using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Componants
{
    public class EmailSender
    {
        public EmailSender(EmailSenderData emailData)
        {
            EmailData = emailData;
            RenderEmailBody();
        }
        public string RenderEmailBody(bool debugmode = true)
        {
            if (EmailData.RazorTemplateName != "")
            {
                var razorTempl = DNNrocketUtils.GetRazorTemplateData(EmailData.RazorTemplateName, EmailData.TemplateControlRelPath, EmailData.ThemeFolder, EmailData.CultureCode, EmailData.VersionFolder, debugmode);
                EmailData.EmailBody = DNNrocketUtils.RazorRender(EmailData.Model, razorTempl, true);
                return EmailData.EmailBody;
            }
            return "";
        }

        public bool SendEmail()
        {
            Error = "";
            EmailData.ToEmail = EmailData.ToEmail.Trim();
            EmailData.FromEmail = EmailData.FromEmail.Trim();
            if (EmailData.EmailBody == "") Error = "Missing EmailBody";
            if (EmailData.ToEmail == "") Error = "Missing ToEmail";
            if (EmailData.FromEmail == "") Error = "Missing FromEmail";
            if (EmailData.Attchments == null) EmailData.Attchments = "";

            if (Error == "")
            {

                try
                {
                    var emailarray = EmailData.ToEmail.Replace(';',',').Split(',');
                    foreach (var email in emailarray)
                    {
                        if (!string.IsNullOrEmpty(email.Trim()) && GeneralUtils.IsEmail(EmailData.FromEmail) && GeneralUtils.IsEmail(email.Trim()))
                        {
                            DotNetNuke.Services.Mail.Mail.SendMail(EmailData.FromEmail, email.Trim(), "", EmailData.EmailSubject, EmailData.EmailBody, EmailData.Attchments, "HTML", "", "", "", "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public EmailSenderData EmailData { get; set; }
        public string Error { get; set; }

    }


    public class EmailSenderData
    {
        public EmailSenderData(string cultureCode = "")
        {
            ThemeFolder = "config-w3";
            VersionFolder = "1.0";
            DebugMode = false;
            CultureCode = cultureCode;
        }

        public SimplisityRazor Model { get; set; }
        public string EmailBody { get; set; }
        public string RazorTemplateName { get; set; }
        public string TemplateControlRelPath { get; set; }
        /// <summary>
        /// Semi-colon list of email addresses.
        /// </summary>
        public string ToEmail { get; set; }
        public string EmailSubject { get; set; }
        public string FromEmail { get; set; }
        public string CultureCode { get; set; }
        /// <summary>
        /// Multiple attachments as csv with "|" seperator
        /// </summary>
        public string Attchments { get; set; }
        public string ThemeFolder { get; set; }
        public string VersionFolder { get; set; }
        public bool DebugMode { get; set; }

    }

}
