using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Simplisity
{
    public class UtilsEmail
    {
        bool _invalid = false;

        public bool IsValidEmail(string strIn)
        {
            _invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
            if (_invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            var idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }

    public class GeneralUtils
    {
        public static string DeCode(string codedval)
        {
            var strOut = "";
            var s = codedval.Split('.');
            foreach (var c in s)
            {
                if (c != "")
                {
                    strOut += (char)Convert.ToInt32(c);
                }
            }
            return strOut;
        }

        public static string EnCode(string value)
        {
            var strOut = "";
            var l = value.ToCharArray();
            foreach (var c in l)
            {
                strOut += ((int)c).ToString() + '.';
            }
            return strOut.TrimEnd('.');
        }
        public static string DecodeCSV(string inputData)
        {
            var datatext = "";
            var arraytext = inputData.Split(',');
            foreach (var a in arraytext)
            {
                datatext += GeneralUtils.DeCode(a).Replace(",", ".") + ",";
            }
            return datatext.TrimEnd(',');
        }


        public static void CreateFolder(string folderMapPath)
        {
            if (!Directory.Exists(folderMapPath))
            {
                Directory.CreateDirectory(folderMapPath);
            }
        }

        public static void DeleteFolder(string folderMapPath, bool recursive = false)
        {
            if (Directory.Exists(folderMapPath))
            {
                Directory.Delete(folderMapPath, recursive);
            }
        }



        public static string FormatToSave(string inpData)
        {
            return FormatToSave(inpData, TypeCode.String, "");
        }

        public static string FormatToSave(string inpData, TypeCode dataTyp)
        {
            return FormatToSave(inpData, dataTyp, "");
        }

        /// -----------------------------------------------------------------------------
        ///  <summary>
        ///  This function uses Regex search strings to remove HTML tags which are
        ///  targeted in Cross-site scripting (XSS) attacks.  This function will evolve
        ///  to provide more robust checking as additional holes are found.
        ///  </summary>
        ///  <param name="strInput">This is the string to be filtered</param>
        /// <param name="filterlinks">remove href elements</param>
        /// <returns>Filtered UserInput</returns>
        ///  <remarks>
        ///  This is a private function that is used internally by the FormatDisableScripting function
        ///  </remarks>
        ///  <history>
        ///      [cathal]        3/06/2007   Created
        ///  </history>
        /// -----------------------------------------------------------------------------
        private static string FilterStrings(string strInput, bool filterlinks)
        {
            //setup up list of search terms as items may be used twice
            var tempInput = strInput;
            var listStrings = new List<string>
            {
                "<script[^>]*>.*?</script[^><]*>",
                "<script",
                "<input[^>]*>.*?</input[^><]*>",
                "<object[^>]*>.*?</object[^><]*>",
                "<embed[^>]*>.*?</embed[^><]*>",
                "<applet[^>]*>.*?</applet[^><]*>",
                "<form[^>]*>.*?</form[^><]*>",
                "<option[^>]*>.*?</option[^><]*>",
                "<select[^>]*>.*?</select[^><]*>",
                "<iframe[^>]*>.*?</iframe[^><]*>",
                "<iframe.*?<",
                "<iframe.*?",
                "<ilayer[^>]*>.*?</ilayer[^><]*>",
                "<form[^>]*>",
                "</form[^><]*>",
                "onerror",
                "onmouseover",
                "javascript:",
                "vbscript:",
                "unescape",
                "alert[\\s(&nbsp;)]*\\([\\s(&nbsp;)]*'?[\\s(&nbsp;)]*[\"(&quot;)]?",
                @"eval*.\(",
                "onload"
            };

            if (filterlinks)
            {
                listStrings.Add("<a[^>]*>.*?</a[^><]*>");
                listStrings.Add("<a");
            }

            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            const string replacement = " ";

            //check if text contains encoded angle brackets, if it does it we decode it to check the plain text
            if (tempInput.Contains("&gt;") && tempInput.Contains("&lt;"))
            {
                //text is encoded, so decode and try again
                tempInput = HttpUtility.HtmlDecode(tempInput);
                tempInput = listStrings.Aggregate(tempInput,
                    (current, s) => Regex.Replace(current, s, replacement, options));

                //Re-encode
                tempInput = HttpUtility.HtmlEncode(tempInput);
            }
            else
            {
                tempInput = listStrings.Aggregate(tempInput,
                    (current, s) => Regex.Replace(current, s, replacement, options));
            }
            return tempInput;
        }

        /// -----------------------------------------------------------------------------
        ///  <summary>
        ///  This function uses Regex search strings to remove HTML tags which are
        ///  targeted in Cross-site scripting (XSS) attacks.  This function will evolve
        ///  to provide more robust checking as additional holes are found.
        ///  </summary>
        ///  <param name="strInput">This is the string to be filtered</param>
        /// <param name="filterlinks">Remove href link elements</param>
        /// <returns>Filtered UserInput</returns>
        ///  <remarks>
        ///  This is a private function that is used internally by the InputFilter function
        ///  </remarks>
        /// -----------------------------------------------------------------------------
        public static string FormatDisableScripting(string strInput, bool filterlinks = true)
        {
            var tempInput = strInput;
            if (strInput == " " || String.IsNullOrEmpty(strInput))
            {
                return tempInput;
            }
            tempInput = FilterStrings(tempInput, filterlinks);
            return tempInput;
        }


        public static string FormatToSave(string inpData, TypeCode dataTyp, string editlang)
        {
            if (String.IsNullOrEmpty(inpData))
                return inpData;
            switch (dataTyp)
            {
                case TypeCode.Double:
                    //always save CultureInfo.InvariantCulture format to the XML
                    if (IsNumeric(inpData, editlang))
                    {
                        var cultureInfo = new CultureInfo(editlang, true);
                        var num = Convert.ToDouble(inpData, cultureInfo);
                        return num.ToString(CultureInfo.InvariantCulture);
                    }
                    if (IsNumeric(inpData)) // just check if we have a Invariant double
                    {
                        var num = Convert.ToDouble(inpData, CultureInfo.InvariantCulture);
                        return num.ToString(CultureInfo.InvariantCulture);
                    }
                    return "0";
                case TypeCode.DateTime:
                    if (IsDate(inpData, editlang))
                    {
                        var cultureInfo = new CultureInfo(editlang, true);
                        var dte = Convert.ToDateTime(inpData, cultureInfo);
                        return dte.ToString("s");
                    }
                    return "";
                default:
                    return FormatDisableScripting(inpData);
            }
        }

        public static string FormatToDisplay(string inpData, string cultureCode, TypeCode dataTyp, string formatCode = "")
        {
            if (String.IsNullOrEmpty(inpData))
            {
                if (dataTyp == TypeCode.Double)
                {
                    return "0";
                }
                return inpData;
            }
            var outCulture = new CultureInfo(cultureCode, false);
            switch (dataTyp)
            {
                case TypeCode.Double:
                    if (IsNumeric(inpData))
                    {
                        return Double.Parse(inpData, CultureInfo.InvariantCulture).ToString(formatCode, outCulture);
                    }
                    return "0";
                case TypeCode.DateTime:
                    if (IsDate(inpData, cultureCode))
                    {
                        if (formatCode == "") formatCode = "d";
                        return DateTime.Parse(inpData).ToString(formatCode, outCulture);
                    }
                    return inpData;
                default:
                    return inpData;
            }
        }


        /// <summary>
        ///  IsEmail function checks for a valid email format         
        /// </summary>
        public static bool IsEmail(string emailaddress)
        {
            var e = new UtilsEmail();
            return e.IsValidEmail(emailaddress);
        }

        /// <summary>
        ///  IsNumeric function check if a given value is numeric, based on the culture code passed.  If no culture code is passed then a test on InvariantCulture is done.
        /// </summary>
        public static bool IsNumeric(object expression, string cultureCode = "")
        {
            if (expression == null) return false;

            double retNum;
            bool isNum = false;
            if (!String.IsNullOrEmpty(cultureCode))
            {
                var cultureInfo = new CultureInfo(cultureCode, true);
                isNum = Double.TryParse(Convert.ToString(expression), NumberStyles.Number, cultureInfo.NumberFormat, out retNum);
            }
            else
            {
                isNum = Double.TryParse(Convert.ToString(expression), NumberStyles.Number, CultureInfo.InvariantCulture, out retNum);
            }

            return isNum;
        }

        // IsDate culture Function
        public static bool IsDate(object expression, string cultureCode)
        {
            DateTime rtnD;
            return DateTime.TryParse(Convert.ToString(expression), CultureInfo.CreateSpecificCulture(cultureCode),
                DateTimeStyles.None, out rtnD);
        }


        public static string FormatAsMailTo(string email)
        {
            var functionReturnValue = "";

            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(email.Trim(Convert.ToChar(" "))))
            {
                if (email.IndexOf(Convert.ToChar("@")) != -1)
                {
                    functionReturnValue = "<a href=\"mailto:" + email + "\">" + email + "</a>";
                }
                else
                {
                    functionReturnValue = email;
                }
            }

            return CloakText(functionReturnValue);

        }


        // obfuscate sensitive data to prevent collection by robots and spiders and crawlers
        public static string CloakText(string personalInfo)
        {
            return CloakText(personalInfo, true);
        }

        public static string CloakText(string personalInfo, bool addScriptTag)
        {
            if (personalInfo != null)
            {
                var sb = new StringBuilder();
                var chars = personalInfo.ToCharArray();
                foreach (char chr in chars)
                {
                    sb.Append(((int)chr).ToString(CultureInfo.InvariantCulture));
                    sb.Append(',');
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                if (addScriptTag)
                {
                    var sbScript = new StringBuilder();
                    sbScript.Append("<script type=\"text/javascript\">");
                    sbScript.Append("document.write(String.fromCharCode(" + sb + "))");
                    sbScript.Append("</script>");
                    return sbScript.ToString();
                }
                return String.Format("document.write(String.fromCharCode({0}))", sb);
            }
            return "";
        }

        public static void DeleteSysFile(string filePathName)
        {
            try
            {
                File.Delete(filePathName);
            }
            catch (Exception)
            {
                //ignore file could be locked.
                // should only be called if not important to remove file.                
            }
        }

        /// <summary>
        /// CleanInput strips out all nonalphanumeric characters except periods (.), at symbols (@), and hyphens (-), and returns the remaining string. However, you can modify the regular expression pattern so that it strips out any characters that should not be included in an input string.
        /// </summary>
        /// <param name="strIn">Dirty String</param>
        /// <param name="regexpr"></param>
        /// <returns>Clean String</returns>
        public static string CleanInput(string strIn, string regexpr = "")
        {
            if (regexpr == "") regexpr = @"[^\w\.@-]";
            // Replace invalid characters with empty strings. 
            return Regex.Replace(strIn, regexpr, "", RegexOptions.None);
        }
        /// <summary>
        /// strips out all nonalphanumeric characters.
        /// </summary>
        /// <param name="strIn">Dirty String</param>
        /// <param name="regexpr"></param>
        /// <returns>Clean String</returns>
        public static string AlphaNumeric(string strIn)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var str = rgx.Replace(strIn, "");
            return str.Replace(" ", "");
        }


        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one". 
        /// hand-tuned for speed, reflects performance refactoring contributed
        /// by John Gietzen (user otac0n) 
        /// </summary>
        public static string UrlFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 255;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= 'а' && c <= 'я') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if ((c >= 0x4E00 && c <= 0x9FFF) ||
                (c >= 0x3400 && c <= 0x4DBF) ||
                (c >= 0x3400 && c <= 0x4DBF) ||
                (c >= 0x20000 && c <= 0x2CEAF) ||
                (c >= 0x2E80 && c <= 0x31EF) ||
                (c >= 0xF900 && c <= 0xFAFF) ||
                (c >= 0xFE30 && c <= 0xFE4F) ||
                (c >= 0xF2800 && c <= 0x2FA1F))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z' || (c >= 'А' && c <= 'Я'))
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }



        /// <summary>
        /// Strip accents from string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>String without accents</returns>
        public static string StripAccents(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s.Normalize(NormalizationForm.FormKD))
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.EnclosingMark:
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            return sb.ToString();
        }


        public static string GetUniqueKey(int maxSize = 0)
        {
            if (maxSize == 0)
            {
                return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            }
            else
            {
                var a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                var chars = a.ToCharArray();
                var data = new byte[1];
                var crypto = new RNGCryptoServiceProvider();
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
                var result = new StringBuilder(maxSize);
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length - 1)]);
                }
                return result.ToString();
            }
        }

        public static string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            int place = source.IndexOf(find, StringComparison.Ordinal);
            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);
            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public static string Decrypt(string strKey, string strData)
        {
            if (String.IsNullOrEmpty(strData))
            {
                return "";
            }
            string strValue = "";
            if (!String.IsNullOrEmpty(strKey))
            {
                //convert key to 16 characters for simplicity
                if (strKey.Length < 16)
                {
                    strKey = strKey + "XXXXXXXXXXXXXXXX".Substring(0, 16 - strKey.Length);
                }
                else
                {
                    strKey = strKey.Substring(0, 16);
                }

                //create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));

                //convert data to byte array and Base64 decode
                var byteData = new byte[strData.Length];
                try
                {
                    byteData = Convert.FromBase64String(strData);
                }
                catch //invalid length
                {
                    strValue = strData;
                }
                if (String.IsNullOrEmpty(strValue))
                {
                    try
                    {
                        //decrypt
                        var objDes = new DESCryptoServiceProvider();
                        var objMemoryStream = new MemoryStream();
                        var objCryptoStream = new CryptoStream(objMemoryStream,
                            objDes.CreateDecryptor(byteKey, byteVector), CryptoStreamMode.Write);
                        objCryptoStream.Write(byteData, 0, byteData.Length);
                        objCryptoStream.FlushFinalBlock();

                        //convert to string
                        Encoding objEncoding = Encoding.UTF8;
                        strValue = objEncoding.GetString(objMemoryStream.ToArray());
                    }
                    catch //decryption error
                    {
                        strValue = "";
                    }
                }
            }
            else
            {
                strValue = strData;
            }
            return strValue;
        }

        public static string Encrypt(string strKey, string strData)
        {
            string strValue;
            if (!String.IsNullOrEmpty(strKey))
            {
                //convert key to 16 characters for simplicity
                if (strKey.Length < 16)
                {
                    strKey = strKey + "XXXXXXXXXXXXXXXX".Substring(0, 16 - strKey.Length);
                }
                else
                {
                    strKey = strKey.Substring(0, 16);
                }

                //create encryption keys
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));

                //convert data to byte array
                byte[] byteData = Encoding.UTF8.GetBytes(strData);

                //encrypt 
                var objDes = new DESCryptoServiceProvider();
                var objMemoryStream = new MemoryStream();
                var objCryptoStream = new CryptoStream(objMemoryStream, objDes.CreateEncryptor(byteKey, byteVector),
                    CryptoStreamMode.Write);
                objCryptoStream.Write(byteData, 0, byteData.Length);
                objCryptoStream.FlushFinalBlock();

                //convert to string and Base64 encode
                strValue = Convert.ToBase64String(objMemoryStream.ToArray());
            }
            else
            {
                strValue = strData;
            }
            return strValue;
        }

        public static string ReplaceFileExt(string fileName, string newExt)
        {
            var strOut = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + newExt;
            return strOut;
        }

        public static byte[] StrToByteArray(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }


    }

}
