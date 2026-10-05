<style>
	details.clean-accordion {
		background-color: #fff; /* Changed to white */
		border: 1px solid #ddd;
		border-radius: 5px;
		margin-bottom: 0.5em;
		overflow: hidden;
	}
	details.clean-accordion summary {
		font-weight: 600;
		padding: 0.6em 1em; /* Reduced padding */
		cursor: pointer;
		background-color: #f5f5f5;
		border-bottom: 1px solid #ddd;
		transition: background-color 0.2s;
		list-style: none;
		display: block;
	}
	details.clean-accordion summary::-webkit-details-marker {
		display: none;
	}
	details.clean-accordion[open] > summary {
		background-color: #e9e9e9;
	}
	details.clean-accordion summary:hover {
		background-color: #e1e1e1;
	}
	details.clean-accordion .token-details {
		padding: 0.8em 1em 1em 2em; /* Reduced padding, kept indent */
		font-size: 0.9em;
	}
	details.clean-accordion .token-details p {
		margin-top: 0;
	}
	details.clean-accordion .token-details pre {
		white-space: pre-wrap;
	}
</style>
<details class="clean-accordion">
	<summary>AddJsonArrayAttributesForXPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void AddJsonArrayAttributesForXPath(string xpath, ref SimplisityInfo sInfo)</code></pre>
		<strong>Example</strong>
		<pre><code>AddJsonArrayAttributesForXPath(xpath, sInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddJsonNetRootAttribute</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void AddJsonNetRootAttribute(ref SimplisityInfo sInfo)</code></pre>
		<strong>Example</strong>
		<pre><code>AddJsonNetRootAttribute(sInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AlphaNumeric</summary>
	<div class="token-details">
		<p><strong>Description:</strong> strips out all nonalphanumeric characters.</p>
		<strong>Signature</strong>
		<pre><code>public static string AlphaNumeric(string strIn)</code></pre>
		<strong>Example</strong>
		<pre><code>AlphaNumeric(strIn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Base64Decode</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string Base64Decode(string base64EncodedData)</code></pre>
		<strong>Example</strong>
		<pre><code>Base64Decode(base64EncodedData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Base64Encode</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string Base64Encode(string plainText)</code></pre>
		<strong>Example</strong>
		<pre><code>Base64Encode(plainText)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CleanInput</summary>
	<div class="token-details">
		<p><strong>Description:</strong> CleanInput strips out all nonalphanumeric characters except periods (.), at symbols (@), and hyphens (-), and returns the remaining string. However, you can modify the regular expression pattern so that it strips out any characters that should not be included in an input string.</p>
		<strong>Signature</strong>
		<pre><code>public static string CleanInput(string strIn, string regexpr = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>CleanInput(strIn, regexpr)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CopyAll</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void CopyAll(string source, string target)</code></pre>
		<strong>Example</strong>
		<pre><code>CopyAll(source, target)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static void CopyAll(DirectoryInfo source, DirectoryInfo target)</code></pre>
		<strong>Example</strong>
		<pre><code>CopyAll(source, target)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CopyDirectory</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)</code></pre>
		<strong>Example</strong>
		<pre><code>CopyDirectory(sourceDir, destinationDir, recursive)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreateFolder</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void CreateFolder(string folderMapPath)</code></pre>
		<strong>Example</strong>
		<pre><code>CreateFolder(folderMapPath)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeCode</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string DeCode(string codedval)</code></pre>
		<strong>Example</strong>
		<pre><code>DeCode(codedval)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DecodeCSV</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string DecodeCSV(string inputData)</code></pre>
		<strong>Example</strong>
		<pre><code>DecodeCSV(inputData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Decrypt</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string Decrypt(string strKey, string strData)</code></pre>
		<strong>Example</strong>
		<pre><code>Decrypt(strKey, strData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeleteFolder</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void DeleteFolder(string folderMapPath, bool recursive = false)</code></pre>
		<strong>Example</strong>
		<pre><code>DeleteFolder(folderMapPath, recursive)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeleteSysFile</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void DeleteSysFile(string filePathName)</code></pre>
		<strong>Example</strong>
		<pre><code>DeleteSysFile(filePathName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EnCode</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string EnCode(string value)</code></pre>
		<strong>Example</strong>
		<pre><code>EnCode(value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Encrypt</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string Encrypt(string strKey, string strData)</code></pre>
		<strong>Example</strong>
		<pre><code>Encrypt(strKey, strData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EscapeJsonString</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string EscapeJsonString(string value)</code></pre>
		<strong>Example</strong>
		<pre><code>EscapeJsonString(value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatAsMailTo</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string FormatAsMailTo(string email, string subject = &quot;&quot;, string visibleText = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatAsMailTo(email, subject, visibleText)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatDateToString</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string FormatDateToString(DateTime dateTime, string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatDateToString(dateTime, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatDisableScripting</summary>
	<div class="token-details">
		<p><strong>Description:</strong> This function uses Regex search strings to remove HTML tags which are targeted in Cross-site scripting (XSS) attacks.  This function will evolve to provide more robust checking as additional holes are found.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatDisableScripting(string strInput, bool filterlinks = true)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatDisableScripting(strInput, filterlinks)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatToDisplay</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string FormatToDisplay(string inpData, string cultureCode, TypeCode dataTyp, string formatCode = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatToDisplay(inpData, cultureCode, dataTyp, formatCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatToSave</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string FormatToSave(string inpData)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatToSave(inpData)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static string FormatToSave(string inpData, TypeCode dataTyp)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatToSave(inpData, dataTyp)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static string FormatToSave(string inpData, TypeCode dataTyp, string editlang)</code></pre>
		<strong>Example</strong>
		<pre><code>FormatToSave(inpData, dataTyp, editlang)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetGuidKey</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetGuidKey()</code></pre>
		<strong>Example</strong>
		<pre><code>GetGuidKey()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetMd5Hash</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetMd5Hash(string input)</code></pre>
		<strong>Example</strong>
		<pre><code>GetMd5Hash(input)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static string GetMd5Hash(string input, bool uppercase)</code></pre>
		<strong>Example</strong>
		<pre><code>GetMd5Hash(input, uppercase)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRandomKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Get a random string. However the code MAY NOT be unique.  Do not use this method if you MUST have a unique string, try GetUniqueString()</p>
		<strong>Signature</strong>
		<pre><code>public static string GetRandomKey(int maxSize = 0, bool numericOnly = false)</code></pre>
		<strong>Example</strong>
		<pre><code>GetRandomKey(maxSize, numericOnly)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUniqueString</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Get unique string based on ticks and a random numer.  The random number is to try and stop clashes when processing on the same tick. This method has a VERY HIGH chance of being unique.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetUniqueString(int randomsize = 8)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUniqueString(randomsize)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HtmlToPlainText</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string HtmlToPlainText(string html)</code></pre>
		<strong>Example</strong>
		<pre><code>HtmlToPlainText(html)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAbsoluteUrl</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsAbsoluteUrl(string url)</code></pre>
		<strong>Example</strong>
		<pre><code>IsAbsoluteUrl(url)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsCultureInfo</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsCultureInfo(string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>IsCultureInfo(cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsDate</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsDate(object expression, string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>IsDate(expression, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsDateInvariantCulture</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Test date in these formats nby default: { &quot;yyyy-MM-dd&quot;, &quot;yyyy/MM/dd&quot;, &quot;MM-dd-yyyy&quot;, &quot;MM/dd/yyyy&quot;, &quot;dd/MM/yyyy&quot;, &quot;dd-MM-yyyy&quot; } Use IsDate function if you know the culturecode.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsDateInvariantCulture(object expression)</code></pre>
		<strong>Example</strong>
		<pre><code>IsDateInvariantCulture(expression)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> IsEmail function checks for a valid email format</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsEmail(string emailaddress)</code></pre>
		<strong>Example</strong>
		<pre><code>IsEmail(emailaddress)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsNumeric</summary>
	<div class="token-details">
		<p><strong>Description:</strong> IsNumeric function check if a given value is numeric, based on the culture code passed.  If no culture code is passed then a test on InvariantCulture is done.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsNumeric(object expression, string cultureCode = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>IsNumeric(expression, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsUriValid</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsUriValid(string uri, UriKind uriKind  = UriKind.RelativeOrAbsolute, bool checkexists = false)</code></pre>
		<strong>Example</strong>
		<pre><code>IsUriValid(uri, uriKind, checkexists)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsValidEmail</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public bool IsValidEmail(string strIn)</code></pre>
		<strong>Example</strong>
		<pre><code>@IsValidEmail(strIn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Numeric</summary>
	<div class="token-details">
		<p><strong>Description:</strong> strips out all nonnumeric characters.</p>
		<strong>Signature</strong>
		<pre><code>public static string Numeric(string strIn)</code></pre>
		<strong>Example</strong>
		<pre><code>Numeric(strIn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ObjectToByteArray</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static byte[] ObjectToByteArray(Object obj)</code></pre>
		<strong>Example</strong>
		<pre><code>ObjectToByteArray(obj)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemapInternationalCharToAscii</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string RemapInternationalCharToAscii(char c)</code></pre>
		<strong>Example</strong>
		<pre><code>RemapInternationalCharToAscii(c)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveDiacritics</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string RemoveDiacritics(string text)</code></pre>
		<strong>Example</strong>
		<pre><code>RemoveDiacritics(text)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ReplaceFileExt</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string ReplaceFileExt(string fileName, string newExt)</code></pre>
		<strong>Example</strong>
		<pre><code>ReplaceFileExt(fileName, newExt)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ReplaceFirstOccurrence</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string ReplaceFirstOccurrence(string source, string find, string replace)</code></pre>
		<strong>Example</strong>
		<pre><code>ReplaceFirstOccurrence(source, find, replace)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ReplaceLastOccurrence</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string ReplaceLastOccurrence(string source, string find, string replace)</code></pre>
		<strong>Example</strong>
		<pre><code>ReplaceLastOccurrence(source, find, replace)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SafeSubstring</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string SafeSubstring(string input, int maxLength)</code></pre>
		<strong>Example</strong>
		<pre><code>SafeSubstring(input, maxLength)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SanitizeFileName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string SanitizeFileName(string fileName)</code></pre>
		<strong>Example</strong>
		<pre><code>SanitizeFileName(fileName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>StripAccents</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Strip accents from string.</p>
		<strong>Signature</strong>
		<pre><code>public static string StripAccents(string s)</code></pre>
		<strong>Example</strong>
		<pre><code>StripAccents(s)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>StrToByteArray</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static byte[] StrToByteArray(string str)</code></pre>
		<strong>Example</strong>
		<pre><code>StrToByteArray(str)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UrlFriendly</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Produces optional, URL-friendly version of a title, &quot;like-this-one&quot;. hand-tuned for speed, reflects performance refactoring contributed by John Gietzen (user otac0n)</p>
		<strong>Signature</strong>
		<pre><code>public static string UrlFriendly(string title)</code></pre>
		<strong>Example</strong>
		<pre><code>UrlFriendly(title)</code></pre>
	</div>
</details>
