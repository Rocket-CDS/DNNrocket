
# SafeSubstring
**Description**: Safely gets a substring of a given length from a string, without causing an out-of-bounds exception.
**Signature**
```csharp
public static string SafeSubstring(string input, int maxLength)
```
***
# IsAbsoluteUrl
**Description**: Checks if a given URL is an absolute URL.
**Signature**
```csharp
public static bool IsAbsoluteUrl(string url)
```
***
# RemoveDiacritics
**Description**: Removes diacritics (accents) from a string.
**Signature**
```csharp
public static string RemoveDiacritics(string text)
```
***
# DeCode
**Description**: Decodes a string that was encoded by converting each character to its integer value, separated by dots.
**Signature**
```csharp
public static string DeCode(string codedval)
```
***
# EnCode
**Description**: Encodes a string by converting each character to its integer value, separated by dots.
**Signature**
```csharp
public static string EnCode(string value)
```
***
# DecodeCSV
**Description**: Decodes a comma-separated string where each value was encoded using the EnCode method.
**Signature**
```csharp
public static string DecodeCSV(string inputData)
```
***
# CreateFolder
**Description**: Creates a folder if it does not already exist.
**Signature**
```csharp
public static void CreateFolder(string folderMapPath)
```
***
# DeleteFolder
**Description**: Deletes a folder.
**Signature**
```csharp
public static void DeleteFolder(string folderMapPath, bool recursive = false)
```
***
# FormatToSave
**Description**: Formats a string for saving, applying scripting protection.
**Signature**
```csharp
public static string FormatToSave(string inpData)
```
***
# FormatToSave
**Description**: Formats a string for saving based on its data type, applying scripting protection.
**Signature**
```csharp
public static string FormatToSave(string inpData, TypeCode dataTyp)
```
***
# FormatDisableScripting
**Description**: Removes potentially malicious HTML tags and scripts from a string to prevent XSS attacks.
**Signature**
```csharp
public static string FormatDisableScripting(string strInput, bool filterlinks = true)
```
***
# FormatToSave
**Description**: Formats data for saving to XML, converting it to a culture-invariant format.
**Signature**
```csharp
public static string FormatToSave(string inpData, TypeCode dataTyp, string editlang)
```
***
# IsCultureInfo
**Description**: Checks if a given string is a valid culture code.
**Signature**
```csharp
public static bool IsCultureInfo(string cultureCode)
```
***
# FormatToDisplay
**Description**: Formats data for display, converting it from a culture-invariant format to a culture-specific format.
**Signature**
```csharp
public static string FormatToDisplay(string inpData, string cultureCode, TypeCode dataTyp, string formatCode = "")
```
***
# FormatDateToString
**Description**: Formats a DateTime object into a string based on a specific culture.
**Signature**
```csharp
public static string FormatDateToString(DateTime dateTime, string cultureCode)
```
***
# IsEmail
**Description**: Checks if a string is a valid email address.
**Signature**
```csharp
public static bool IsEmail(string emailaddress)
```
***
# IsNumeric
**Description**: Checks if an object can be converted to a numeric value, optionally using a specific culture.
**Signature**
```csharp
public static bool IsNumeric(object expression, string cultureCode = "")
```
***
# IsUriValid
**Description**: Checks if a string is a well-formed URI and can optionally check if the host exists.
**Signature**
```csharp
public static bool IsUriValid(string uri, UriKind uriKind  = UriKind.RelativeOrAbsolute, bool checkexists = false)
```
***
# IsDate
**Description**: Checks if an object can be converted to a DateTime value using a specific culture.
**Signature**
```csharp
public static bool IsDate(object expression, string cultureCode)
```
***
# IsDateInvariantCulture
**Description**: Checks if an object can be converted to a DateTime value using a set of common invariant culture formats.
**Signature**
```csharp
public static bool IsDateInvariantCulture(object expression)
```
***
# FormatAsMailTo
**Description**: Creates a 'mailto' link that is obfuscated to protect the email address from scrapers.
**Signature**
```csharp
public static string FormatAsMailTo(string email, string subject = "", string visibleText = "")
```
***
# DeleteSysFile
**Description**: Deletes a file, ignoring any exceptions if the file is locked.
**Signature**
```csharp
public static void DeleteSysFile(string filePathName)
```
***
# CleanInput
**Description**: Strips out characters from a string that do not match a given regular expression.
**Signature**
```csharp
public static string CleanInput(string strIn, string regexpr = "")
```
***
# AlphaNumeric
**Description**: Strips out all non-alphanumeric characters from a string.
**Signature**
```csharp
public static string AlphaNumeric(string strIn)
```
***
# Numeric
**Description**: Strips out all non-numeric characters from a string.
**Signature**
```csharp
public static string Numeric(string strIn)
```
***
# UrlFriendly
**Description**: Converts a string into a URL-friendly format (e.g., 'like-this-one').
**Signature**
```csharp
public static string UrlFriendly(string title)
```
***
# SanitizeFileName
**Description**: Sanitizes a file name by removing diacritics and invalid file name characters.
**Signature**
```csharp
public static string SanitizeFileName(string fileName)
```
***
# RemapInternationalCharToAscii
**Description**: Remaps common international characters to their ASCII equivalents.
**Signature**
```csharp
public static string RemapInternationalCharToAscii(char c)
```
***
# StripAccents
**Description**: Strips accents from a string.
**Signature**
```csharp
public static string StripAccents(string s)
```
***
# GetUniqueString
**Description**: Generates a highly unique, URL-safe string based on the current time and a random number.
**Signature**
```csharp
public static string GetUniqueString(int randomsize = 8)
```
***
# GetRandomKey
**Description**: Generates a random string of a specified size. Not guaranteed to be unique.
**Signature**
```csharp
public static string GetRandomKey(int maxSize = 0, bool numericOnly = false)
```
***
# GetGuidKey
**Description**: Generates a URL-safe key from a new GUID.
**Signature**
```csharp
public static string GetGuidKey()
```
***
# ReplaceFirstOccurrence
**Description**: Replaces the first occurrence of a substring within a string.
**Signature**
```csharp
public static string ReplaceFirstOccurrence(string source, string find, string replace)
```
***
# ReplaceLastOccurrence
**Description**: Replaces the last occurrence of a substring within a string.
**Signature**
```csharp
public static string ReplaceLastOccurrence(string source, string find, string replace)
```
***
# Decrypt
**Description**: Decrypts a string using DES encryption with a given key.
**Signature**
```csharp
public static string Decrypt(string strKey, string strData)
```
***
# Encrypt
**Description**: Encrypts a string using DES encryption with a given key.
**Signature**
```csharp
public static string Encrypt(string strKey, string strData)
```
***
# ReplaceFileExt
**Description**: Replaces the extension of a file path with a new extension.
**Signature**
```csharp
public static string ReplaceFileExt(string fileName, string newExt)
```
***
# StrToByteArray
**Description**: Converts a string to a byte array using UTF8 encoding.
**Signature**
```csharp
public static byte[] StrToByteArray(string str)
```
***
# ObjectToByteArray
**Description**: Serializes an object into a byte array.
**Signature**
```csharp
public static byte[] ObjectToByteArray(Object obj)
```
***
# CopyAll
**Description**: Recursively copies all files and subdirectories from a source to a target directory.
**Signature**
```csharp
public static void CopyAll(string source, string target)
```
***
# CopyAll
**Description**: Recursively copies all files and subdirectories from a source to a target directory.
**Signature**
```csharp
public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
```
***
# GetMd5Hash
**Description**: Computes the MD5 hash of a string.
**Signature**
```csharp
public static string GetMd5Hash(string input)
```
***
# GetMd5Hash
**Description**: Computes the MD5 hash of a string, with an option for uppercase or lowercase output.
**Signature**
```csharp
public static string GetMd5Hash(string input, bool uppercase)
```
***
# Base64Encode
**Description**: Encodes a string into Base64 format.
**Signature**
```csharp
public static string Base64Encode(string plainText)
```
***
# Base64Decode
**Description**: Decodes a Base64-encoded string.
**Signature**
```csharp
public static string Base64Decode(string base64EncodedData)
```
***
# AddJsonNetRootAttribute
**Description**: Adds the Json.NET namespace attribute to the root element of a SimplisityInfo's XML document.
**Signature**
```csharp
public static void AddJsonNetRootAttribute(ref SimplisityInfo sInfo)
```
***
# AddJsonArrayAttributesForXPath
**Description**: Adds the Json.NET 'Array=true' attribute to all XML elements matching an XPath expression.
**Signature**
```csharp
public static void AddJsonArrayAttributesForXPath(string xpath, ref SimplisityInfo sInfo)
```
***
# CopyDirectory
**Description**: Copies a directory and its contents to a new location.
**Signature**
```csharp
public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
```
***
# HtmlToPlainText
**Description**: Converts an HTML string to plain text by stripping out tags.
**Signature**
```csharp
public static string HtmlToPlainText(string html)
```
***
# EscapeJsonString
**Description**: Escapes special characters in a string to make it safe for inclusion in a JSON string.
**Signature**
```csharp
public static string EscapeJsonString(string value)
```
