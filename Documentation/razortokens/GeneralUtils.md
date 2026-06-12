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
	<summary>SafeSubstring</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Safely gets a substring of a given length from a string, without causing an out-of-bounds exception.</p>
		<strong>Signature</strong>
		<pre><code>public static string SafeSubstring(string input, int maxLength)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAbsoluteUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a given URL is an absolute URL.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsAbsoluteUrl(string url)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveDiacritics</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Removes diacritics (accents) from a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string RemoveDiacritics(string text)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeCode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Decodes a string that was encoded by converting each character to its integer value, separated by dots.</p>
		<strong>Signature</strong>
		<pre><code>public static string DeCode(string codedval)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EnCode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Encodes a string by converting each character to its integer value, separated by dots.</p>
		<strong>Signature</strong>
		<pre><code>public static string EnCode(string value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DecodeCSV</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Decodes a comma-separated string where each value was encoded using the EnCode method.</p>
		<strong>Signature</strong>
		<pre><code>public static string DecodeCSV(string inputData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreateFolder</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a folder if it does not already exist.</p>
		<strong>Signature</strong>
		<pre><code>public static void CreateFolder(string folderMapPath)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeleteFolder</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Deletes a folder.</p>
		<strong>Signature</strong>
		<pre><code>public static void DeleteFolder(string folderMapPath, bool recursive = false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatToSave</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats a string for saving, applying scripting protection.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatToSave(string inpData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatToSave</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats a string for saving based on its data type, applying scripting protection.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatToSave(string inpData, TypeCode dataTyp)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatDisableScripting</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Removes potentially malicious HTML tags and scripts from a string to prevent XSS attacks.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatDisableScripting(string strInput, bool filterlinks = true)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatToSave</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats data for saving to XML, converting it to a culture-invariant format.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatToSave(string inpData, TypeCode dataTyp, string editlang)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsCultureInfo</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a given string is a valid culture code.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsCultureInfo(string cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatToDisplay</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats data for display, converting it from a culture-invariant format to a culture-specific format.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatToDisplay(string inpData, string cultureCode, TypeCode dataTyp, string formatCode = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatDateToString</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats a DateTime object into a string based on a specific culture.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatDateToString(DateTime dateTime, string cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a string is a valid email address.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsEmail(string emailaddress)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsNumeric</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if an object can be converted to a numeric value, optionally using a specific culture.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsNumeric(object expression, string cultureCode = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsUriValid</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a string is a well-formed URI and can optionally check if the host exists.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsUriValid(string uri, UriKind uriKind  = UriKind.RelativeOrAbsolute, bool checkexists = false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsDate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if an object can be converted to a DateTime value using a specific culture.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsDate(object expression, string cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsDateInvariantCulture</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if an object can be converted to a DateTime value using a set of common invariant culture formats.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsDateInvariantCulture(object expression)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FormatAsMailTo</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a 'mailto' link that is obfuscated to protect the email address from scrapers.</p>
		<strong>Signature</strong>
		<pre><code>public static string FormatAsMailTo(string email, string subject = "", string visibleText = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeleteSysFile</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Deletes a file, ignoring any exceptions if the file is locked.</p>
		<strong>Signature</strong>
		<pre><code>public static void DeleteSysFile(string filePathName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CleanInput</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Strips out characters from a string that do not match a given regular expression.</p>
		<strong>Signature</strong>
		<pre><code>public static string CleanInput(string strIn, string regexpr = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AlphaNumeric</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Strips out all non-alphanumeric characters from a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string AlphaNumeric(string strIn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Numeric</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Strips out all non-numeric characters from a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string Numeric(string strIn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UrlFriendly</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Converts a string into a URL-friendly format (e.g., 'like-this-one').</p>
		<strong>Signature</strong>
		<pre><code>public static string UrlFriendly(string title)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SanitizeFileName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sanitizes a file name by removing diacritics and invalid file name characters.</p>
		<strong>Signature</strong>
		<pre><code>public static string SanitizeFileName(string fileName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemapInternationalCharToAscii</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Remaps common international characters to their ASCII equivalents.</p>
		<strong>Signature</strong>
		<pre><code>public static string RemapInternationalCharToAscii(char c)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>StripAccents</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Strips accents from a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string StripAccents(string s)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUniqueString</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a highly unique, URL-safe string based on the current time and a random number.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetUniqueString(int randomsize = 8)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRandomKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a random string of a specified size. Not guaranteed to be unique.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetRandomKey(int maxSize = 0, bool numericOnly = false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetGuidKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a URL-safe key from a new GUID.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetGuidKey()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ReplaceFirstOccurrence</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Replaces the first occurrence of a substring within a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string ReplaceFirstOccurrence(string source, string find, string replace)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ReplaceLastOccurrence</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Replaces the last occurrence of a substring within a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string ReplaceLastOccurrence(string source, string find, string replace)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Decrypt</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Decrypts a string using DES encryption with a given key.</p>
		<strong>Signature</strong>
		<pre><code>public static string Decrypt(string strKey, string strData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Encrypt</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Encrypts a string using DES encryption with a given key.</p>
		<strong>Signature</strong>
		<pre><code>public static string Encrypt(string strKey, string strData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ReplaceFileExt</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Replaces the extension of a file path with a new extension.</p>
		<strong>Signature</strong>
		<pre><code>public static string ReplaceFileExt(string fileName, string newExt)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>StrToByteArray</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Converts a string to a byte array using UTF8 encoding.</p>
		<strong>Signature</strong>
		<pre><code>public static byte[] StrToByteArray(string str)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ObjectToByteArray</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Serializes an object into a byte array.</p>
		<strong>Signature</strong>
		<pre><code>public static byte[] ObjectToByteArray(Object obj)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CopyAll</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Recursively copies all files and subdirectories from a source to a target directory.</p>
		<strong>Signature</strong>
		<pre><code>public static void CopyAll(string source, string target)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CopyAll</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Recursively copies all files and subdirectories from a source to a target directory.</p>
		<strong>Signature</strong>
		<pre><code>public static void CopyAll(DirectoryInfo source, DirectoryInfo target)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetMd5Hash</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Computes the MD5 hash of a string.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetMd5Hash(string input)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetMd5Hash</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Computes the MD5 hash of a string, with an option for uppercase or lowercase output.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetMd5Hash(string input, bool uppercase)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Base64Encode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Encodes a string into Base64 format.</p>
		<strong>Signature</strong>
		<pre><code>public static string Base64Encode(string plainText)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Base64Decode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Decodes a Base64-encoded string.</p>
		<strong>Signature</strong>
		<pre><code>public static string Base64Decode(string base64EncodedData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddJsonNetRootAttribute</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds the Json.NET namespace attribute to the root element of a SimplisityInfo's XML document.</p>
		<strong>Signature</strong>
		<pre><code>public static void AddJsonNetRootAttribute(ref SimplisityInfo sInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddJsonArrayAttributesForXPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds the Json.NET 'Array=true' attribute to all XML elements matching an XPath expression.</p>
		<strong>Signature</strong>
		<pre><code>public static void AddJsonArrayAttributesForXPath(string xpath, ref SimplisityInfo sInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CopyDirectory</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Copies a directory and its contents to a new location.</p>
		<strong>Signature</strong>
		<pre><code>public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HtmlToPlainText</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Converts an HTML string to plain text by stripping out tags.</p>
		<strong>Signature</strong>
		<pre><code>public static string HtmlToPlainText(string html)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EscapeJsonString</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Escapes special characters in a string to make it safe for inclusion in a JSON string.</p>
		<strong>Signature</strong>
		<pre><code>public static string EscapeJsonString(string value)</code></pre>
	</div>
</details>
