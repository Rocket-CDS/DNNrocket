using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
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
                if (EmailData.SystemKey == null) EmailData.SystemKey = EmailData.AppTheme.SystemKey;
                var systemData = new SystemLimpet(EmailData.SystemKey);
                var razorTempl = EmailData.AppTheme.GetTemplate(EmailData.RazorTemplateName);
                
                // if we have no theme template, look in the system folder.
                if (razorTempl == "")
                {
                    var appthemeSystem = new AppThemeSystemLimpet(EmailData.SystemKey);
                    razorTempl = appthemeSystem.GetTemplate(EmailData.RazorTemplateName);
                }

                var pr = RenderRazorUtils.RazorProcess(EmailData.Model, razorTempl, false);
                if (pr.IsValid) EmailData.EmailBody = pr.RenderedText;

                return EmailData.EmailBody;
            }
            return "";
        }

        public bool SendEmail()
        {
            Error = "";
            EmailData.ToEmail = EmailData.ToEmail.Trim();
            EmailData.FromEmail = EmailData.FromEmail.Trim();
            if (EmailData.ReplyToEmail == "") EmailData.ReplyToEmail = EmailData.FromEmail;
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
                            string[] stringarray = new string[0];
                            DotNetNuke.Services.Mail.Mail.SendMail(EmailData.FromEmail.Trim(), email.Trim(), "", "", EmailData.ReplyToEmail, DotNetNuke.Services.Mail.MailPriority.Normal, EmailData.EmailSubject, DotNetNuke.Services.Mail.MailFormat.Html, Encoding.UTF8, EmailData.EmailBody,  stringarray, "", "", "", "", false);
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
            DebugMode = false;
            CultureCode = cultureCode;
        }
        public EmailSenderData(string systemKey, string cultureCode = "")
        {
            DebugMode = false;
            CultureCode = cultureCode;
        }

        public SimplisityRazor Model { get; set; }
        public string EmailBody { get; set; }
        public string RazorTemplateName { get; set; }
        /// <summary>
        /// Semi-colon list of email addresses.
        /// </summary>
        public string ToEmail { get; set; }
        public string EmailSubject { get; set; }
        public string FromEmail { get; set; }
        public string ReplyToEmail { get; set; }
        public string CultureCode { get; set; }
        /// <summary>
        /// Multiple attachments as csv with "|" seperator
        /// </summary>
        public string Attchments { get; set; }
        public bool DebugMode { get; set; }
        public string SystemKey { get; set; }

        public AppThemeLimpet AppTheme { get; set; }

    }

}
