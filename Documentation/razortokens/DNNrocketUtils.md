
# Handle404Exception
**Description**: Handles a 404 Not Found error by redirecting to the portal's defined 404 page or by returning a standard 404 response.
**Signature**
```csharp
public static void Handle404Exception(HttpResponse response, PortalSettings portalSetting)
```
***
# AppendFileModifiedDate
**Description**: Appends a version number based on the file's last modified date to a relative path. Useful for cache-busting CSS and JS files.
**Signature**
```csharp
public static string AppendFileModifiedDate(string relPath, string paramname = "v", bool useCache = true)
```
***
# ReturnString
**Description**: Creates a dictionary for a standard API response, containing an HTML output string and an optional JSON object.
**Signature**
```csharp
public static Dictionary<string, object> ReturnString(string strOut, object jsonOut = null)
```
***
# HtmlOf
**Description**: Decodes an HTML-encoded string.
**Signature**
```csharp
public static string HtmlOf(String htmlString)
```
***
# HtmlDecode
**Description**: Decodes an HTML-encoded string.
**Signature**
```csharp
public static string HtmlDecode(String htmlString)
```
***
# HtmlEncode
**Description**: Encodes a plain text string into an HTML-safe string.
**Signature**
```csharp
public static string HtmlEncode(String planStr)
```
***
# RequestParam
**Description**: Gets a request parameter from the form or query string.
**Signature**
```csharp
public static string RequestParam(HttpContextBase context, string paramName)
```
***
# RequestQueryStringParam
**Description**: Gets a request parameter from the query string.
**Signature**
```csharp
public static string RequestQueryStringParam(HttpRequestBase Request, string paramName)
```
***
# ZipFolder
**Description**: Creates a zip archive from a folder.
**Signature**
```csharp
public static void ZipFolder(string folderMapPath, string zipFileMapPath)
```
***
# ExtractZipFolder
**Description**: Extracts a zip archive to a specified folder.
**Signature**
```csharp
public static void ExtractZipFolder(string zipFileMapPath, string outFolderMapPath, bool overwrite)
```
***
# GetFiles
**Description**: Gets a list of FileInfo objects for all files in a specified folder.
**Signature**
```csharp
public static List<System.IO.FileInfo> GetFiles(string FolderMapPath)
```
***
# GetRegionList
**Description**: Gets a dictionary of regions for a given country code from a DNN list.
**Signature**
```csharp
public static Dictionary<string, string> GetRegionList(string countrycode, string dnnlistname = "Region")
```
***
# GetCountryCodeList
**Description**: Gets a dictionary of all country codes and their names for a given portal.
**Signature**
```csharp
public static Dictionary<String, String> GetCountryCodeList(int portalId = -1)
```
***
# GetCountryName
**Description**: Gets the name of a country from its country code.
**Signature**
```csharp
public static string GetCountryName(string countryCode, int portalId = -1)
```
***
# GetRegionName
**Description**: Gets the name of a region from a combined country and region code key.
**Signature**
```csharp
public static string GetRegionName(string countryRegionCodeKey, string dnnlistname = "Region")
```
***
# GetCultureCodeNameList
**Description**: Gets a dictionary of enabled culture codes and their native names for a portal.
**Signature**
```csharp
public static Dictionary<string, string> GetCultureCodeNameList(int portalId = -1)
```
***
# GetAllCultureCodeNameList
**Description**: Gets a dictionary of all culture codes and their display names available in the .NET framework.
**Signature**
```csharp
public static Dictionary<string, string> GetAllCultureCodeNameList()
```
***
# GetCultureCodeName
**Description**: Gets the display name for a given culture code.
**Signature**
```csharp
public static string GetCultureCodeName(string cultureCode)
```
***
# GetCultureCodeList
**Description**: Gets a list of enabled culture codes for a portal.
**Signature**
```csharp
public static List<string> GetCultureCodeList(int portalId = -1)
```
***
# GetPortalLanguageList
**Description**: Gets a list of unique two-letter language codes (e.g., 'en', 'fr') enabled for a portal.
**Signature**
```csharp
public static List<string> GetPortalLanguageList(int portalId = -1)
```
***
# GetCurrencyList
**Description**: Gets a dictionary of available currencies for a portal from the 'Currency' DNN list.
**Signature**
```csharp
public static Dictionary<string, string> GetCurrencyList(int portalId = -1)
```
***
# GetAllCultureCodeList
**Description**: Gets a list of SimplisityInfo objects representing all available .NET cultures, including display name, code, and a flag image URL.
**Signature**
```csharp
public static List<SimplisityInfo> GetAllCultureCodeList()
```
***
# GetDataResponseAsString
**Description**: Fetches the content of a URL as a string. Can optionally pass data in the request header.
**Signature**
```csharp
public static string GetDataResponseAsString(string dataurl, string headerFieldId = "", string headerFieldData = "")
```
***
# GetLocalizedString
**Description**: Gets a localized string from a resource file.
**Signature**
```csharp
public static string GetLocalizedString(string Key, string resourceFileRoot, string lang)
```
***
# ConvertObjectToXMLString
**Description**: Serializes an object into an XML string.
**Signature**
```csharp
public static string ConvertObjectToXMLString(object classObject)
```
***
# ConvertXmlStringtoObject
**Description**: Deserializes an XML string into an object of a specified type.
**Signature**
```csharp
public static T ConvertXmlStringtoObject<T>(string xmlString)
```
***
# GetModuleVersion
**Description**: Gets the version string of a DNN module.
**Signature**
```csharp
public static string GetModuleVersion(int moduleId)
```
***
# GetLibraryVersion
**Description**: Gets the version string of an installed DNN library package.
**Signature**
```csharp
public static string GetLibraryVersion(string packageName)
```
***
# CreateFolder
**Description**: Creates a folder using the DNN FolderManager to work around potential medium trust issues.
**Signature**
```csharp
public static void CreateFolder(string fullfolderPath)
```
***
# CreateDefaultRocketRoles
**Description**: Creates the default set of DNNrocket roles (Collaborator, Editor, Manager, etc.) for a given portal.
**Signature**
```csharp
public static void CreateDefaultRocketRoles(int portalId)
```
***
# DefaultRoleExist
**Description**: Checks if the default 'Collaborator' role exists for a given portal.
**Signature**
```csharp
public static bool DefaultRoleExist(int portalId)
```
***
# CreateRole
**Description**: Creates a new security role in a portal if it does not already exist.
**Signature**
```csharp
public static void CreateRole(int portalId, string roleName, string description = "", float serviceFee = 0, int billingPeriod = 0, string billingFrequency = "M", float trialFee = 0, int trialPeriod = 0, string trialFrequency = "N", bool isPublic = false, bool isAuto = false)
```
***
# GetRoleByName
**Description**: Gets a RoleInfo object for a role by its name.
**Signature**
```csharp
public static RoleInfo GetRoleByName(int portalId, string roleName)
```
***
# AddRoleToModule
**Description**: Grants 'EDIT' and 'DEPLOY' permissions for a specific role to a module.
**Signature**
```csharp
public static void AddRoleToModule(int portalId, int moduleid, int roleid)
```
***
# RemoveRoleToModule
**Description**: Removes permissions for a specific role from a module.
**Signature**
```csharp
public static void RemoveRoleToModule(int portalId, int moduleid, int roleid)
```
***
# GetTabModuleTitles
**Description**: Gets a dictionary of module IDs and their titles for a specific tab.
**Signature**
```csharp
public static Dictionary<int,string> GetTabModuleTitles(int tabid, bool getDeleted = false)
```
***
# UpdateModuleTitle
**Description**: Updates the title of a module.
**Signature**
```csharp
public static void UpdateModuleTitle(int tabid, int moduleid, string title)
```
***
# ModuleIsDeleted
**Description**: Checks if a module is marked as deleted.
**Signature**
```csharp
public static bool ModuleIsDeleted(int tabid, int moduleid)
```
***
# ModuleExists
**Description**: Checks if a module exists on a specific tab.
**Signature**
```csharp
public static bool ModuleExists(int tabid, int moduleid)
```
***
# GetModuleTabId
**Description**: Gets the Tab ID for a module by its unique GUID.
**Signature**
```csharp
public static int GetModuleTabId(Guid uniqueId)
```
***
# GetTabInfo
**Description**: Gets a TabInfo object for a given tab ID and portal ID.
**Signature**
```csharp
public static TabInfo GetTabInfo(int portalid, int tabid, bool ignoreCache = false)
```
***
# GetTabInfoRecord
**Description**: Gets a SimplisityRecord representation of a TabInfo object.
**Signature**
```csharp
public static SimplisityRecord GetTabInfoRecord(int portalId,int tabid, bool ignoreCache = false)
```
***
# GetTreeTabList
**Description**: Gets a dictionary of tabs in a hierarchical (tree) structure, suitable for a dropdown list.
**Signature**
```csharp
public static Dictionary<int, string> GetTreeTabList(bool showAllTabs = false)
```
***
# GetTabList
**Description**: Gets a list of all tabs in a portal as SimplisityRecord objects.
**Signature**
```csharp
public static List<SimplisityRecord> GetTabList(int portalId)
```
***
# GetTreeTabListOnTabId
**Description**: Gets a dictionary of tabs in a hierarchical structure, where the key is the TabId.
**Signature**
```csharp
public static Dictionary<int, string> GetTreeTabListOnTabId(bool showAllTabs = false)
```
***
# GetResourceString
**Description**: Gets a specific value from a resource file based on the key and extension.
**Signature**
```csharp
public static String GetResourceString(String resourcePath, String resourceKey, String resourceExt = "Text", String lang = "")
```
***
# GetResourceData
**Description**: Gets a dictionary of all resource values (Text, Help, etc.) for a given resource key.
**Signature**
```csharp
public static Dictionary<String, String> GetResourceData(String resourcePath, String resourceKey, String lang = "")
```
***
# GetResourceFileRelPath
**Description**: Constructs the relative path to a resource file based on path, filename, and culture code.
**Signature**
```csharp
public static string GetResourceFileRelPath(String resourcePath, String resourceFileName, String cultureCode = "")
```
***
# GetResourceFileMapPath
**Description**: Gets the full physical map path for a resource file.
**Signature**
```csharp
public static string GetResourceFileMapPath(String resourcePath, String resourceFileName, String cultureCode = "")
```
***
# ResxValues
**Description**: Extracts all key-value pairs from a .resx file that end with a specific key extension.
**Signature**
```csharp
public static Dictionary<string, string> ResxValues(string resourceFileName, string cultureCode, string keyExtension)
```
***
# Encrypt
**Description**: Encrypts a string using the portal's GUID as the default passkey.
**Signature**
```csharp
public static String Encrypt(String value, String passkey = "")
```
***
# Decrypt
**Description**: Decrypts a string using the portal's GUID as the default passkey.
**Signature**
```csharp
public static String Decrypt(String value, String passkey = "")
```
***
# DeleteCookieValue
**Description**: Deletes a cookie by setting its expiration date to the past.
**Signature**
```csharp
public static void DeleteCookieValue(string name)
```
***
# SetCookieValue
**Description**: Sets a cookie with a specified name and value.
**Signature**
```csharp
public static void SetCookieValue(string name, string value)
```
***
# GetCookieValue
**Description**: Gets the value of a cookie by its name.
**Signature**
```csharp
public static string GetCookieValue(string name)
```
***
# ValidCulture
**Description**: Checks if a given culture code is enabled for the current portal.
**Signature**
```csharp
public static bool ValidCulture(string cultureCode)
```
***
# SetEditCulture
**Description**: Sets the editing culture by storing it in a cookie.
**Signature**
```csharp
public static string SetEditCulture(string editlang)
```
***
# GetEditCulture
**Description**: Gets the current editing culture from the URL parameter 'editlang' or the 'simplisity_editlanguage' cookie.
**Signature**
```csharp
public static string GetEditCulture()
```
***
# SetCurrentCulture
**Description**: Sets the current thread's culture and updates the 'language' and 'simplisity_language' cookies.
**Signature**
```csharp
public static void SetCurrentCulture(string cultureCode)
```
***
# GetCurrentCulture
**Description**: Gets the name of the current thread's culture.
**Signature**
```csharp
public static string GetCurrentCulture()
```
***
# GetCurrentCountryCode
**Description**: Gets the country code part (e.g., 'US') from the current culture.
**Signature**
```csharp
public static string GetCurrentCountryCode()
```
***
# GetCountryCode
**Description**: Extracts the country code part from a full culture code.
**Signature**
```csharp
public static string GetCountryCode(string cultureCode)
```
***
# GetCurrentLanguageCode
**Description**: Gets the language code part (e.g., 'en') from the current culture.
**Signature**
```csharp
public static string GetCurrentLanguageCode()
```
***
# GetLanguageCode
**Description**: Extracts the language code part from a full culture code.
**Signature**
```csharp
public static string GetLanguageCode(string cultureCode)
```
***
# SystemThemeImgDirectoryRel
**Description**: Returns the relative path to the system theme image directory.
**Signature**
```csharp
public static string SystemThemeImgDirectoryRel()
```
***
# SystemThemeImgDirectoryMapPath
**Description**: Returns the physical map path to the system theme image directory.
**Signature**
```csharp
public static string SystemThemeImgDirectoryMapPath()
```
***
# MapPath
**Description**: Converts a relative web path to a physical file system path.
**Signature**
```csharp
public static string MapPath(string relpath)
```
***
# MapPathReverse
**Description**: Converts a physical file system path back to a relative web path.
**Signature**
```csharp
public static string MapPathReverse(string fullMapPath)
```
***
# Email
**Description**: Gets the email address of the specified portal or the current portal.
**Signature**
```csharp
public static string Email(int portalId = -1)
```
***
# GetEntityTypeCode
**Description**: Gets the entity type code from an interface's info, defaulting to the uppercase interface key if not specified.
**Signature**
```csharp
public static string GetEntityTypeCode(SimplisityInfo interfaceInfo)
```
***
# GetUniqueFileName
**Description**: Generates a unique file name for a given folder by appending an index if the file already exists.
**Signature**
```csharp
public static string GetUniqueFileName(string fileName, string folderMapPath, int idx = 1, string originalFileName = "")
```
***
# EncryptFileName
**Description**: Encrypts a file name and replaces any invalid file name characters.
**Signature**
```csharp
public static string EncryptFileName(string encryptkey, string fileName)
```
***
# GetSystemByName
**Description**: Gets a system's SimplisityInfo data by its system key.
**Signature**
```csharp
public static SimplisityInfo GetSystemByName(string systemkey)
```
***
# GetModuleSystemKey
**Description**: Gets the DNNrocket system key associated with a module, derived from its module name (e.g., 'systemkey_interfacekey').
**Signature**
```csharp
public static string GetModuleSystemKey(int moduleId, int tabId)
```
***
# GetModuleInterfaceKey
**Description**: Gets the DNNrocket interface key associated with a module, derived from its module name (e.g., 'systemkey_interfacekey').
**Signature**
```csharp
public static string GetModuleInterfaceKey(int moduleId, int tabId)
```
***
# GetAllModulesInPortal
**Description**: Gets a list of all non-deleted module IDs in a portal.
**Signature**
```csharp
public static List<int> GetAllModulesInPortal(int portalId)
```
***
# GetAllModulesOnPage
**Description**: Gets a list of all non-deleted module IDs on a specific page (tab).
**Signature**
```csharp
public static List<int> GetAllModulesOnPage(int tabId)
```
***
# GetModuleSystemInfo
**Description**: Gets the system information for a module, caching the result.
**Signature**
```csharp
public static SimplisityInfo GetModuleSystemInfo(string systemkey, int moduleId, bool loadSystemXml = true)
```
***
# GetProviderReturn
**Description**: Executes a command on an API provider (APInterface) and returns the result.
**Signature**
```csharp
public static Dictionary<string, object> GetProviderReturn(string paramCmd, SimplisityInfo systemInfo, RocketInterface rocketInterface, SimplisityInfo postInfo, SimplisityInfo paramInfo, string templateRelPath, string editlang)
```
***
# EventProviderBefore
**Description**: Executes the 'BeforeEvent' method on all active event providers for a system.
**Signature**
```csharp
public static Dictionary<string, object> EventProviderBefore(string paramCmd, SystemLimpet systemData, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang)
```
***
# EventProviderAfter
**Description**: Executes the 'AfterEvent' method on all active event providers for a system.
**Signature**
```csharp
public static Dictionary<string, object> EventProviderAfter(string paramCmd, SystemLimpet systemData, SimplisityInfo postInfo, SimplisityInfo paramInfo, string editlang)
```
***
# RenderImageSelect
**Description**: Renders an image selection interface using a specified Razor template.
**Signature**
```csharp
public static string RenderImageSelect(string systemkey, string imageFolderRel, bool selectsingle = true, bool autoreturn = false)
```
***
# RenderDocumentSelect
**Description**: Renders a document selection interface using a specified Razor template.
**Signature**
```csharp
public static string RenderDocumentSelect(string systemkey, string documentFolderRel, bool selectsingle = true, bool autoreturn = false)
```
***
# ForceDocDownload
**Description**: Forces a file to be downloaded by the browser.
**Signature**
```csharp
public static void ForceDocDownload(string docFilePath, string fileName, HttpResponse response)
```
***
# ForceStringDownload
**Description**: Forces a string of data to be downloaded by the browser as a file.
**Signature**
```csharp
public static void ForceStringDownload(HttpResponse response, string fileName, string fileData)
```
***
# UpdateFieldXpath
**Description**: Updates the XPath for a list of fields in a SimplisityRecord based on their type and name.
**Signature**
```csharp
public static SimplisityRecord UpdateFieldXpath(SimplisityInfo postInfo, SimplisityRecord record, string listname)
```
***
# BackUpNewFileName
**Description**: Generates a new backup file name in a structured backup directory, based on the current date and time.
**Signature**
```csharp
public static string BackUpNewFileName(string backupRootFolder, string moduleName, string fileAppendix = "BackUp.xml")
```
***
# RecycleApplicationPool
**Description**: Recycles the application pool by 'touching' the web.config file.
**Signature**
```csharp
public static bool RecycleApplicationPool(string siteName = null)
```
***
# CloneModule
**Description**: Clones a module from one tab to another.
**Signature**
```csharp
public static void CloneModule(int moduleid, int fromTabId, int toTabId)
```
***
# ClearThumbnailLock
**Description**: Clears file locks on thumbnail images by disposing of them from the cache.
**Signature**
```csharp
public static void ClearThumbnailLock()
```
***
# ClearTempDB
**Description**: Deletes all data from the 'DNNrocketTemp' table.
**Signature**
```csharp
public static void ClearTempDB()
```
***
# SetCache
**Description**: Sets an object in the DNN cache with a specified expiration.
**Signature**
```csharp
public static void SetCache(string cacheKey, object objObject, int keephours)
```
***
# GetCache
**Description**: Gets an object from the DNN cache.
**Signature**
```csharp
public static object GetCache(string cacheKey)
```
***
# RemoveCache
**Description**: Removes an object from the DNN cache.
**Signature**
```csharp
public static void RemoveCache(string cacheKey)
```
***
# ClearAllCache
**Description**: Clears the entire DNN cache.
**Signature**
```csharp
public static void ClearAllCache()
```
***
# ClearPortalCache
**Description**: Clears the cache for a specific portal.
**Signature**
```csharp
public static void ClearPortalCache(int portalId)
```
***
# SynchronizeModule
**Description**: Synchronizes a module's content between the cache and the database and updates its ModifiedContentDate.
**Signature**
```csharp
public static void SynchronizeModule(int moduleId)
```
***
# NavigateURL
**Description**: A wrapper for the DNN Globals.NavigateURL method to generate URLs for pages.
**Signature**
```csharp
public static string NavigateURL(int tabId, Dictionary<string,string> dictParams, string seoname)
```
***
# UrlFriendly
**Description**: Converts a string into a URL-friendly format.
**Signature**
```csharp
public static string UrlFriendly(string textstring)
```
***
# ConvertTimeToDouble
**Description**: Converts a time string (e.g., 'HH:mm:ss') into a double representation of hours.
**Signature**
```csharp
public static double ConvertTimeToDouble(string timeString)
```
***
# ProtectEmail
**Description**: Obfuscates an email address in HTML to protect it from scrapers.
**Signature**
```csharp
public static string ProtectEmail(this string email, string subject = "", string visibleText = "")
```
***
# TruncateWords
**Description**: Truncates a string to a maximum number of characters, ensuring it doesn't cut off in the middle of a word.
**Signature**
```csharp
public static string TruncateWords(this string text, int maxCharacters, string trailingText)
```
***
# Truncate
**Description**: Truncates a string to a maximum number of characters.
**Signature**
```csharp
public static string Truncate(this string text, int maxCharacters, string trailingText)
```
***
# UpdateSqlIndex
**Description**: Updates or creates a SQL index record in the DNNrocket database.
**Signature**
```csharp
public static void UpdateSqlIndex(SimplisityRecord idx)
```
***
# IsMobile
**Description**: Determines if the current request is from a mobile device.
**Signature**
```csharp
public static bool IsMobile()
```
***
# UpdateSimplsityInfoFields
**Description**: Updates fields in a SimplisityInfo object from another SimplisityInfo object based on an XPath selector.
**Signature**
```csharp
public static SimplisityInfo UpdateSimplsityInfoFields(SimplisityInfo newInfo, SimplisityInfo postInfo, string xpathListSelect)
```
***
# UpdateSimplsityRecordFields
**Description**: Updates fields in a SimplisityRecord object from a SimplisityInfo object based on an XPath selector.
**Signature**
```csharp
public static SimplisityRecord UpdateSimplsityRecordFields(SimplisityRecord newRec, SimplisityInfo postInfo, string xpathListSelect)
```
***
# ParseQueryString
**Description**: Parses a URL to get a specific query parameter value, checking both friendly URL format and standard query string.
**Signature**
```csharp
public static string ParseQueryString(string key, string requesturl)
```
***
# GetModList
**Description**: Gets a list of ModuleBase objects for all modules in a portal.
**Signature**
```csharp
public static List<ModuleBase> GetModList(int portalId, bool includeDeleted = false)
```
***
# GetPortalContentRecByRefId
**Description**: Gets a portal-level content record by its reference ID, using caching.
**Signature**
```csharp
public static SimplisityRecord GetPortalContentRecByRefId(int portalId, string systemKey, string tableName)
```
***
# DeleteSearchDocument
**Description**: Deletes a document from the DNN search index.
**Signature**
```csharp
public static void DeleteSearchDocument(int portalId, string queryString)
```
***
# GetQueryKeys
**Description**: Gets a dictionary of configured query parameter keys and their associated data for a portal.
**Signature**
```csharp
public static Dictionary<string, QueryParamsData> GetQueryKeys(int portalId)
```
***
# DeleteOldFiles
**Description**: Deletes files in a folder that are older than a specified number of days.
**Signature**
```csharp
public static void DeleteOldFiles(string folderPath, uint maximumAgeInDays, params string[] filesToExclude)
```
***
# DeleteFileIfOlderThan
**Description**: Deletes a single file if it is older than a specified date.
**Signature**
```csharp
public static void DeleteFileIfOlderThan(string path, DateTime date)
```
***
# StartBackgroundThread
**Description**: Starts a new background thread.
**Signature**
```csharp
public static void StartBackgroundThread(ThreadStart threadStart)
```
***
# DependanciesList
**Description**: Gets a list of dependency records for an app theme, processing URL tokens.
**Signature**
```csharp
public static List<SimplisityRecord> DependanciesList(string moduleRef, AppThemeBase appTheme, string domainUrl, string appThemeSystemFolder)
```
***
# InjectDependencies
**Description**: Gets a cached list of Dependency objects to be injected into a page.
**Signature**
```csharp
public static List<Dependency> InjectDependencies(string moduleRef, AppThemeBase appTheme, bool ecoMode, string skinSrc, string domainUrl, string appThemeSystemFolder)
```
***
# InjectDependacies
**Description**: Injects CSS and JS dependencies into a WebForms page.
**Signature**
```csharp
public static void InjectDependacies(string moduleRef, Page page, AppThemeBase appTheme, bool ecoMode, string skinSrc, string domainUrl, string appThemeSystemFolder, string displayTemplate)
```
***
# IsModuleDeleted
**Description**: Checks if a module is marked as deleted by its ID.
**Signature**
```csharp
public static bool IsModuleDeleted(int moduleId)
```
***
# SetTempStorage
**Description**: Saves a SimplisityInfo object to temporary database storage with an expiration.
**Signature**
```csharp
public static string SetTempStorage(SimplisityInfo value, string key = "", int keephours = 24)
```
***
# GetTempStorage
**Description**: Retrieves a SimplisityInfo object from temporary storage.
**Signature**
```csharp
public static SimplisityInfo GetTempStorage(string key, bool deleteAfterRead = false)
```
***
# SetTempRecordStorage
**Description**: Saves a SimplisityRecord object to temporary database storage.
**Signature**
```csharp
public static string SetTempRecordStorage(SimplisityRecord value, string key = "", int keephours = 24)
```
***
# GetTempRecordStorage
**Description**: Retrieves a SimplisityRecord object from temporary storage.
**Signature**
```csharp
public static SimplisityRecord GetTempRecordStorage(string key, bool deleteAfterRead = false)
```
***
# DeleteTempStorage
**Description**: Deletes an item from temporary storage by its key.
**Signature**
```csharp
public static void DeleteTempStorage(string key)
```
***
# ClearOldTempStorage
**Description**: Deletes all expired items from the temporary storage table.
**Signature**
```csharp
public static void ClearOldTempStorage()
```
***
# ImportWebsite
**Description**: Initiates a DNN website import process from a given package file.
**Signature**
```csharp
public static ExportImportJob ImportWebsite(int portalId, string importDataMapPath)
```
