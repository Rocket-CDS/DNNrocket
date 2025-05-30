﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

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
        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static string DeCode(string codedval)
        {
            if (codedval == null) return "";
            var strOut = "";
            try
            {
                var s = codedval.Split('.');
                foreach (var c in s)
                {
                    if (c != "")
                    {
                        strOut += (char)Convert.ToInt32(c);
                    }
                }
            }
            catch (Exception)
            {
                // this may not be incoded, and that will throw and error.  So return the value
                strOut = codedval;
            }
            return strOut;
        }

        public static string EnCode(string value)
        {
            if (value == null) return "";
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
                case TypeCode.Int32:
                    inpData = Regex.Replace(inpData, @"[^\d]+", "");
                    if (inpData.Length == 0) return "0";
                    return inpData;
                case TypeCode.DateTime:
                    if (IsDate(inpData, editlang))
                    {
                        if (!IsCultureInfo(editlang)) editlang = "en-US";
                        var cultureInfo = new CultureInfo(editlang, true);
                        var dte = Convert.ToDateTime(inpData, cultureInfo);
                        return dte.ToString("s");
                    }
                    return "";
                default:
                    return FormatDisableScripting(inpData);
            }
        }
        public static bool IsCultureInfo(string cultureCode)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureCode);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
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
        public static string FormatDateToString(DateTime dateTime, string cultureCode)
        {
            return dateTime.ToString(new CultureInfo(cultureCode));
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
        public static bool IsUriValid(string uri, UriKind uriKind  = UriKind.RelativeOrAbsolute, bool checkexists = false)
        {
            if (uri == "") return false;
            var isformatted = Uri.IsWellFormedUriString(uri, uriKind);            
            if (isformatted && checkexists)
            {
                bool br = false;
                try
                {
                    IPHostEntry ipHost = Dns.GetHostEntry(uri);
                    br = true;
                }
                catch (SocketException se)
                {
                    br = false;
                }
                return br;
            }
            return isformatted;
        }

        // IsDate culture Function
        public static bool IsDate(object expression, string cultureCode)
        {
            if (!IsCultureInfo(cultureCode)) cultureCode = "en-US";
            DateTime rtnD;
            return DateTime.TryParse(Convert.ToString(expression), System.Globalization.CultureInfo.CreateSpecificCulture(cultureCode),
                DateTimeStyles.None, out rtnD);
        }
        /// <summary>
        /// Test date in these formats nby default: { "yyyy-MM-dd", "yyyy/MM/dd", "MM-dd-yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "dd-MM-yyyy" }
        /// Use IsDate function if you know the culturecode.
        /// </summary>
        /// <param name="datevalue">Date value (converted to string)</param>
        /// <returns></returns>
        public static bool IsDateInvariantCulture(object expression)
        {
            DateTime dt;
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "MM-dd-yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-d", "yyyy/MM/d", "MM-d-yyyy", "MM/d/yyyy", "d/MM/yyyy", "d-MM-yyyy" };
            return DateTime.TryParseExact(Convert.ToString(expression), formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }
        public static string FormatAsMailTo(string email, string subject = "", string visibleText = "")
        {
            if (string.IsNullOrEmpty(email)) return "";

            var onload = "";

            var dataAttribs = "data-contact='" + Convert.ToBase64String(Encoding.UTF8.GetBytes(email)) + "'";
            if (string.IsNullOrEmpty(visibleText))
            {
                onload = $"<img src onerror='this.outerHTML = atob(\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(email))}\")'>"; //nice hack to mimic an onload event
            }
            var onfocus = "this.href='mailto:'+atob(this.dataset.contact)";
            if (!string.IsNullOrEmpty(subject))
            {
                dataAttribs = dataAttribs + " data-subj='" + Convert.ToBase64String(Encoding.UTF8.GetBytes(subject)) + "'";
                onfocus = onfocus + "+'?subject=' + atob(this.dataset.subj || '')";
            }
            var result = $"<a href='#' {dataAttribs} onfocus=\"{onfocus}\">{visibleText}{onload}</a>";
            return result;
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
        /// strips out all nonnumeric characters.
        /// </summary>
        /// <param name="strIn">Dirty String</param>
        /// <param name="regexpr"></param>
        /// <returns>Clean String</returns>
        public static string Numeric(string strIn)
        {
            return Regex.Replace(strIn, "[^0-9]", "");
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
        /// <summary>
        /// Get unique string based on ticks and a random numer.  The random number is to try and stop clashes when processing on the same tick.
        /// This method has a VERY HIGH chance of being unique.
        /// </summary>
        /// <param name="randomsize">RandomKey size, the ramdon key is added to the tick string. #= size of random key</param>
        /// <returns></returns>
        public static string GetUniqueString(int randomsize = 8)
        {
            if (randomsize > 8) randomsize = 8;
            if (randomsize <= 0) randomsize = 8;
            int timeStamp = (int)(DateTime.Now.Ticks >> 23); // retain bits 23 to 55
            var strticks = timeStamp.ToString() + GetRandomKey(randomsize, true);
            long ticks = Convert.ToInt64(Convert.ToDecimal(strticks));
            byte[] bytes = BitConverter.GetBytes(ticks);
            return Convert.ToBase64String(bytes).Replace('+', '_').Replace('/', '-').TrimEnd('=');
        }
        /// <summary>
        /// This mehtod may NOT be unique, use GetUniqueString().
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        [Obsolete("GetUniqueKey() is deprecated, please use GetUniqueString() or GetGuidKey() instead.")]
        public static string GetUniqueKey(int maxSize = 0)
        {
            return GetRandomKey(maxSize);
        }
        /// <summary>
        /// Get a random string. However the code MAY NOT be unique.  Do not use this method if you MUST have a unique string, try GetUniqueString()
        /// </summary>
        /// <param name="maxSize">0= Guid (Unique across machines),#= size of return string, larger return strings have more chance to be unique</param>
        /// <param name="lowercaseonly">Only use lowercase and numeric in the return string</param>
        /// <returns></returns>
        public static string GetRandomKey(int maxSize = 0, bool numericOnly = false)
        {
            if (maxSize == 0)
            {
                return GetGuidKey();
            }
            else
            {
                var a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                if (numericOnly) a = "1234567890";
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
        public static string GetGuidKey()
        {
            return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
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
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }


        public static void CopyAll(string source, string target)
        {
            var diSource = new DirectoryInfo(source);
            var diTarget = new DirectoryInfo(target);
            GeneralUtils.CopyAll(diSource, diTarget);
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static string GetMd5Hash(string input)
        {
            return GetMd5Hash(input, true);
        }
        public static string GetMd5Hash(string input, bool uppercase)
        {
            if (input != "")
            {
                var md5 = MD5.Create();
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (byte t in hash)
                {
                    if (uppercase)
                        sb.Append(t.ToString("X2"));
                    else
                        sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
            return "";
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static void AddJsonNetRootAttribute(ref SimplisityInfo sInfo)
        {
            XmlAttribute jsonNS = sInfo.XMLDoc.CreateAttribute("xmlns", "json", "http://www.w3.org/2000/xmlns/");
            jsonNS.Value = "http://james.newtonking.com/projects/json";

            sInfo.XMLDoc.DocumentElement.SetAttributeNode(jsonNS);
        }
        public static void AddJsonArrayAttributesForXPath(string xpath, ref SimplisityInfo sInfo)
        {
            var elements = sInfo.XMLDoc.SelectNodes(xpath);
            foreach (var element in elements)
            {
                var el = element as XmlElement;
                if (el != null)
                {
                    var jsonArray = sInfo.XMLDoc.CreateAttribute("json", "Array", "http://james.newtonking.com/projects/json");
                    jsonArray.Value = "true";
                    el.SetAttributeNode(jsonArray);
                }
            }
        }
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
        public static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
    }

}
