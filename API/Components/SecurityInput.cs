using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace DNNrocketAPI.Components
{
    public static class SecurityInput
    {
        private const RegexOptions RxOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled;

        private static readonly Regex[] RxListStrings = new[]
        {
            new Regex("<script[^>]*>.*?</script[^><]*>", RxOptions),
            new Regex("<script", RxOptions),
            new Regex("<input[^>]*>.*?</input[^><]*>", RxOptions),
            new Regex("<object[^>]*>.*?</object[^><]*>", RxOptions),
            new Regex("<embed[^>]*>.*?</embed[^><]*>", RxOptions),
            new Regex("<applet[^>]*>.*?</applet[^><]*>", RxOptions),
            new Regex("<form[^>]*>.*?</form[^><]*>", RxOptions),
            new Regex("<option[^>]*>.*?</option[^><]*>", RxOptions),
            new Regex("<select[^>]*>.*?</select[^><]*>", RxOptions),
            new Regex("<source[^>]*>.*?</source[^><]*>", RxOptions),
            new Regex("<iframe[^>]*>.*?</iframe[^><]*>", RxOptions),
            new Regex("<iframe.*?<", RxOptions),
            new Regex("<iframe.*?", RxOptions),
            new Regex("<ilayer[^>]*>.*?</ilayer[^><]*>", RxOptions),
            new Regex("<form[^>]*>", RxOptions),
            new Regex("</form[^><]*>", RxOptions),
            new Regex("\bonerror\b", RxOptions),
            new Regex("\bonload\b", RxOptions),
            new Regex("\bonfocus\b", RxOptions),
            new Regex("\bonblur\b", RxOptions),
            new Regex("\bonclick\b", RxOptions),
            new Regex("\bondblclick\b", RxOptions),
            new Regex("\bonchange\b", RxOptions),
            new Regex("\bonselect\b", RxOptions),
            new Regex("\bonsubmit\b", RxOptions),
            new Regex("\bonreset\b", RxOptions),
            new Regex("\bonkeydown\b", RxOptions),
            new Regex("\bonkeyup\b", RxOptions),
            new Regex("\bonkeypress\b", RxOptions),
            new Regex("\bonmousedown\b", RxOptions),
            new Regex("\bonmousemove\b", RxOptions),
            new Regex("\bonmouseout\b", RxOptions),
            new Regex("\bonmouseover\b", RxOptions),
            new Regex("\bonmouseup\b", RxOptions),
            new Regex("\bonreadystatechange\b", RxOptions),
            new Regex("\bonfinish\b", RxOptions),
            new Regex("javascript:", RxOptions),
            new Regex("vbscript:", RxOptions),
            new Regex("unescape", RxOptions),
            new Regex("alert[\\s(&nbsp;)]*\\([\\s(&nbsp;)]*'?[\\s(&nbsp;)]*[\"(&quot;)]?", RxOptions),
            new Regex(@"eval*.\(", RxOptions),
        };
        private static readonly Regex DangerElementsRegex = new Regex(@"(<[^>]*?) on.*?\=(['""]*)[\s\S]*?(\2)( *)([^>]*?>)", RxOptions);
        private static readonly Regex DangerElementContentRegex = new Regex(@"on.*?\=(['""]*)[\s\S]*?(\1)( *)", RxOptions);


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This function uses Regex search strings to remove HTML tags which are
        /// targeted in Cross-site scripting (XSS) attacks.  This function will evolve
        /// to provide more robust checking as additional holes are found.
        /// </summary>
        /// <param name="strInput">This is the string to be filtered.</param>
        /// <returns>Filtered UserInput.</returns>
        /// <remarks>
        /// This is a private function that is used internally by the FormatDisableScripting function.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public static string RemoveScripts(string strInput)
        {
            // setup up list of search terms as items may be used twice
            var tempInput = strInput;
            if (string.IsNullOrWhiteSpace(tempInput))
            {
                return tempInput;
            }

            const string replacement = " ";

            // remove the js event from html tags
            var tagMatches = DangerElementsRegex.Matches(tempInput);
            foreach (Match match in tagMatches)
            {
                var tagContent = match.Value;
                var cleanTagContent = DangerElementContentRegex.Replace(tagContent, string.Empty);
                tempInput = tempInput.Replace(tagContent, cleanTagContent);
            }

            // check if text contains encoded angle brackets, if it does it we decode it to check the plain text
            if (tempInput.Contains("&gt;") || tempInput.Contains("&lt;"))
            {
                // text is encoded, so decode and try again
                tempInput = DNNrocketUtils.HtmlDecode(tempInput);
                tempInput = RxListStrings.Aggregate(tempInput, (current, s) => s.Replace(current, replacement));

                // Re-encode
                tempInput = DNNrocketUtils.HtmlEncode(tempInput);
            }
            else
            {
                tempInput = RxListStrings.Aggregate(tempInput, (current, s) => s.Replace(current, replacement));
            }

            return tempInput;
        }
        public static Boolean CheckForSQLInjection(string userInput)
        {
            bool isSQLInjection = false;
            string[] sqlCheckList =
                {
                "--",
                ";--",
                ";",
                "/*",
                "*/",
                "@@",
                "@",
                "alter",
                "begin",
                "cast",
                "create",
                "cursor",
                "declare",
                "delete",
                "drop",
                "end",
                "exec",
                "execute",
                "fetch",
                "insert",
                "kill",
                "select",
                "sys",
                "sysobjects",
                "syscolumns",
                "table",
                "update"
            };
            string CheckString = userInput.Replace("'", "''");
            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((CheckString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) >= 0)) isSQLInjection = true;
            }
            return isSQLInjection;
        }



    }
}
