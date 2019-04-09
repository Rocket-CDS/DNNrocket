using Simplisity;

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
