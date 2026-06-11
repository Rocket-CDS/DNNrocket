# AddProcessDataResx
**Description**: Adds resource paths to the process data for later use by resource key tokens. Can include portal-specific, app-theme-specific, and optionally the core API resx paths.
**Signature**
```csharp
public IEncodedString AddProcessDataResx(AppThemeLimpet appTheme, bool includeAPIresx = false)
```
**Example**
```csharp
@DnnRocket.AddProcessDataResx(appTheme, true)
```
***
# DropDownLanguageList
**Description**: Renders a dropdown list of enabled languages for the portal, with flags.
**Signature**
```csharp
public IEncodedString DropDownLanguageList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@DnnRocket.DropDownLanguageList(Model.Info, "genxml/dropdown/language")
```
***
# DropDownCurrencyList
**Description**: Renders a dropdown list of available currencies.
**Signature**
```csharp
public IEncodedString DropDownCurrencyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@DnnRocket.DropDownCurrencyList(Model.Info, "genxml/dropdown/currency")
```
***
# DropDownCultureCodeList
**Description**: Renders a dropdown list of culture codes for the portal.
**Signature**
```csharp
public IEncodedString DropDownCultureCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@DnnRocket.DropDownCultureCodeList(Model.Info, "genxml/dropdown/culturecode")
```
***
# DropDownCountryCodeList
**Description**: Renders a dropdown list of country codes.
**Signature**
```csharp
public IEncodedString DropDownCountryCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@DnnRocket.DropDownCountryCodeList(Model.Info, "genxml/dropdown/countrycode")
```
***
# DropDownSystemKeyList
**Description**: Renders a dropdown list of active system keys.
**Signature**
```csharp
public IEncodedString DropDownSystemKeyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@DnnRocket.DropDownSystemKeyList(Model.Info, "genxml/dropdown/systemkey")
```
***
# ResourceCSV
**Description**: Output CSV list of resx values. Example: @ResourceCSV("RocketIntra", "test1,test2,test3")
**Signature**
```csharp
public IEncodedString ResourceCSV(String resourceFileKey, string keyListCSV, string lang = "", string resourceExtension = "Text")
```
**Example**
```csharp
@DnnRocket.ResourceCSV("MyResources", "key1,key2,key3")
```
***
# ButtonTextIcon
**Description**: Renders a button with text followed by an icon, based on a button type.
**Signature**
```csharp
public IEncodedString ButtonTextIcon(ButtonTypes buttontype, String lang = "")
```
**Example**
```csharp
@DnnRocket.ButtonTextIcon(ButtonTypes.Save)
```
***
# ButtonIconText
**Description**: Renders a button with an icon followed by text, based on a button type.
**Signature**
```csharp
public IEncodedString ButtonIconText(ButtonTypes buttontype, String lang = "")
```
**Example**
```csharp
@DnnRocket.ButtonIconText(ButtonTypes.Save)
```
***
# ButtonText
**Description**: Renders a button with an icon followed by text.
**Signature**
```csharp
public IEncodedString ButtonText(ButtonTypes buttontype, String lang = "")
```
**Example**
```csharp
@DnnRocket.ButtonText(ButtonTypes.Delete)
```
***
# ButtonIcon
**Description**: Renders a button with only an icon, using the button text as the title attribute for accessibility.
**Signature**
```csharp
public IEncodedString ButtonIcon(ButtonTypes buttontype, String lang = "")
```
**Example**
```csharp
@DnnRocket.ButtonIcon(ButtonTypes.Edit)
```
***
# ResourceKeyMod
**Description**: Gets a resource string, automatically prepending the key with a module reference and an underscore.
**Signature**
```csharp
public IEncodedString ResourceKeyMod(String moduleRef, String resourceFileKey, String lang = "", String resourceExtension = "Text")
```
**Example**
```csharp
@DnnRocket.ResourceKeyMod("MyModRef", "MyKey")
```
***
# ResourceKey
**Description**: Gets a resource string from the resource paths previously added via AddProcessDataResx.
**Signature**
```csharp
public IEncodedString ResourceKey(String resourceFileKey, String lang = "", String resourceExtension = "Text")
```
**Example**
```csharp
@DnnRocket.ResourceKey("WelcomeMessage")
```
***
# ResourceKeyJS
**Description**: Gets a resource string and escapes single quotes for safe use within JavaScript code.
**Signature**
```csharp
public IEncodedString ResourceKeyJS(String resourceFileKey, String lang = "", String resourceExtension = "Text")
```
**Example**
```csharp
var message = '@DnnRocket.ResourceKeyJS("AlertMessage")';
```
***
# RenderLanguageSelector
**Description**: Renders a language selector component with a dictionary for selector fields.
**Signature**
```csharp
public IEncodedString RenderLanguageSelector(string scmd, Dictionary<string, string> sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)
```
**Example**
```csharp
@DnnRocket.RenderLanguageSelector("changelang", new Dictionary<string, string>(), appThemeSystem, Model)
```
***
# RenderRemoteLanguageSelector
**Description**: Renders a remote language selector component.
**Signature**
```csharp
public IEncodedString RenderRemoteLanguageSelector(string scmd, string sfields, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)
```
**Example**
```csharp
@DnnRocket.RenderRemoteLanguageSelector("changelang", "{}", appThemeSystem, Model)
```
***
# RenderTemplate
**Description**: Renders a Razor template string with the given model.
**Signature**
```csharp
public IEncodedString RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)
```
**Example**
```csharp
@DnnRocket.RenderTemplate("<div>Hello @Model.Get('name')</div>", myModel)
```
***
# RenderPlugin
**Description**: Renders a plugin based on its registered interface key. The 'systemdata' object must be available in the model.
**Signature**
```csharp
public IEncodedString RenderPlugin(string interfaceKey, string cmd, SimplisityRazor model)
```
**Example**
```csharp
@DnnRocket.RenderPlugin("myplugin", "showdetails", Model)
```
***
# RenderXml
**Description**: Renders a display of the XML model from a SimplisityInfo object for debugging purposes.
**Signature**
```csharp
public IEncodedString RenderXml(SimplisityInfo info, string xmlidx = "")
```
**Example**
```csharp
@DnnRocket.RenderXml(Model.Info)
```
***
# RenderImageSelect
**Description**: Renders an image selection interface.
**Signature**
```csharp
public IEncodedString RenderImageSelect(string systemKey, string imageFolderRel, bool singleselect = true, bool autoreturn = false)
```
**Example**
```csharp
@DnnRocket.RenderImageSelect("mysystem", "/Portals/0/Images")
```
***
# RenderDocumentSelect
**Description**: Renders a document selection interface.
**Signature**
```csharp
public IEncodedString RenderDocumentSelect(string systemKey, string docFolderRel, bool singleselect = true, bool autoreturn = false)
```
**Example**
```csharp
@DnnRocket.RenderDocumentSelect("mysystem", "/Portals/0/Documents")
```
***
# TranslationLock
**Description**: Renders a lock/unlock icon for managing the translation state of a field. It includes a hidden checkbox to store the state.
**Signature**
```csharp
public IEncodedString TranslationLock(SimplisityInfo info, string xpath, bool active = true, int row = 0)
```
**Example**
```csharp
@DnnRocket.TranslationLock(Model.Info, "genxml/textbox/title")
```
***
# Translate
**Description**: Renders a translation icon that can be clicked to trigger a translation action for a specific field.
**Signature**
```csharp
public IEncodedString Translate(SimplisityInfo info, string xpath, bool active = true, int row = 0)
```
**Example**
```csharp
@DnnRocket.Translate(Model.Info, "genxml/textbox/summary")
```
***
# TranslationKeyUp
**Description**: Generates an 'onkeyup' HTML attribute. When the user types in a field, this script will automatically set the corresponding translation lock to 'locked'.
**Signature**
```csharp
public IEncodedString TranslationKeyUp(string fieldId, bool active = true, int row = 0)
```
**Example**
```csharp
<input type='text' @DnnRocket.TranslationKeyUp("title") />
```
***
# EditFlag
**Description**: Displays the flag image for the current editing culture code from session parameters.
**Signature**
```csharp
public IEncodedString EditFlag(SessionParams sessionParams, string classvalues = "")
```
**Example**
```csharp
@DnnRocket.EditFlag(Model.SessionParams, "my-flag-class")
```
***
# DisplayFlag
**Description**: Displays a flag image for a given culture code, if the image file exists.
**Signature**
```csharp
public IEncodedString DisplayFlag(string cultureCode, string classvalues = "")
```
**Example**
```csharp
@DnnRocket.DisplayFlag("fr-FR")
```
***
# DisplayEngineFlag
**Description**: Displays a flag image from a remote engine URL for a given culture code.
**Signature**
```csharp
public IEncodedString DisplayEngineFlag(string engineUrl, string cultureCode, string classvalues = "")
```
**Example**
```csharp
@DnnRocket.DisplayEngineFlag("https://myothersite.com", "de-DE")
```
***
# ImageUrl
**Description**: Display Thumbnail Image. Creates and returns a URL for a resized version of an image. Supports various output formats and cropping. By default, PNGs remain PNGs, and other formats are converted to WEBP. The cache holds a lock on the image file, so use DNNrocketUtils.ClearThumbnailLock() before deleting the original image.
**Signature**
```csharp
public IEncodedString ImageUrl(string engineUrl, string imgRelPath, int width, int height, string imgType, bool cropCenter)
```
**Example**
```csharp
@DnnRocket.ImageUrl("", "/Portals/0/my-image.jpg", 200, 200, "webp", true)
```
***
# InjectHiddenFieldData
**Description**: Renders all nodes under 'genxml/hidden/*' as hidden input fields in the HTML. This is useful for passing data from the model to client-side scripts.
**Signature**
```csharp
public IEncodedString InjectHiddenFieldData(SimplisityInfo sInfo)
```
**Example**
```csharp
@DnnRocket.InjectHiddenFieldData(Model.Info)
```
***
# CKEditor4legacy
**Description**: Legacy CKEditor 4 implementation. Consider using @Editor() instead.
**Signature**
```csharp
public IEncodedString CKEditor4legacy(SimplisityInfo info, string xpath, bool localized = false, int row = 0, string listname = "", string langauge = "", bool coded = false, string filename = "ckeditor4startup1.js")
```
**Example**
```csharp
@DnnRocket.CKEditor4legacy(Model.Info, "genxml/richtext/content")
```
***
# Editor
**Description**: Renders a rich text editor (defaulting to Jodit). The specific editor template can be configured in the portal settings or specified directly.
**Signature**
```csharp
public IEncodedString Editor(SimplisityInfo info, string xpath, SimplisityRazor model, int row = 0, string listname = "", string editorRazorTemplate = "EditorJoditDefault.cshtml")
```
**Example**
```csharp
@DnnRocket.Editor(Model.Info, "genxml/richtext/content", Model)
```
***
# LinkInternalUrl
**Description**: Generates a URL for an internal DNN page (tab) with a specific culture code and optional extra parameters.
**Signature**
```csharp
public IEncodedString LinkInternalUrl(int portalid, int tabid, string cultureCode, PortalSettings portalSettings = null, string[] extraparams = null)
```
**Example**
```csharp
@DnnRocket.LinkInternalUrl(0, 55, "en-US")
```
***
# TabSelectListOnTabId
**Description**: Renders a dropdown list of portal tabs (pages), structured as a tree. The value of each option is the TabId.
**Signature**
```csharp
public IEncodedString TabSelectListOnTabId(SimplisityInfo info, String xpath, String attributes = "", Boolean allowEmpty = true, bool localized = false, int row = 0, string listname = "", bool showAllTabs = false)
```
**Example**
```csharp
@DnnRocket.TabSelectListOnTabId(Model.Info, "genxml/dropdown/pagelink")
```
***
# GetTabUrlByGuid
**Description**: Gets the URL for a tab by its unique GUID.
**Signature**
```csharp
public IEncodedString GetTabUrlByGuid(String tabguid)
```
**Example**
```csharp
@DnnRocket.GetTabUrlByGuid("a1b2c3d4-e5f6-7890-1234-567890abcdef")
```
***
# LinkPageURL
**Description**: Creates an anchor tag linking to an internal DNN page. The tab ID is read from a SimplisityInfo field.
**Signature**
```csharp
public IEncodedString LinkPageURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = "", string attributes = "")
```
**Example**
```csharp
@DnnRocket.LinkPageURL(Model.Info, "genxml/data/linkedpageid", text: "Read More")
```
***
# LinkURL
**Description**: Creates an anchor tag for a URL stored in a SimplisityInfo field. Automatically handles adding 'https://' if missing.
**Signature**
```csharp
public IEncodedString LinkURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = "", string attributes = "")
```
**Example**
```csharp
@DnnRocket.LinkURL(Model.Info, "genxml/textbox/websiteurl", true, "Visit Website")
```
***
# DataSourceList
**Description**: Renders a dropdown list of data sources (MODULEPARAMS) for a given system key.
**Signature**
```csharp
public IEncodedString DataSourceList(SimplisityInfo info, int systemkey, string xpath, string attributes = "", bool allowEmpty = true, bool localized = false)
```
**Example**
```csharp
@DnnRocket.DataSourceList(Model.Info, 1, "genxml/dropdown/datasource")
```
***
# GetTreeTabList
**Description**: Generates a tree-structured HTML list of portal tabs with checkboxes for selection.
**Signature**
```csharp
public IEncodedString GetTreeTabList(int portalId, List<int> selectedTabIdList, string treeviewId, string lang = "", string attributes = "", bool showAllTabs = false)
```
**Example**
```csharp
@DnnRocket.GetTreeTabList(0, new List<int>(), "mytree")
```
***
# ModSelectList
**Description**: Renders a dropdown list of modules for a given portal, showing module references.
**Signature**
```csharp
public IEncodedString ModSelectList(SimplisityInfo info, String xpath, int portalId, String attributes = "", bool addEmpty = true)
```
**Example**
```csharp
@DnnRocket.ModSelectList(Model.Info, "genxml/dropdown/moduleid", 0)
```
***
# CheckBoxRowECOMode
**Description**: Creates a checkbox for ECOMode in the settings of a module.
**Signature**
```csharp
public IEncodedString CheckBoxRowECOMode(SimplisityInfo rowData, bool defaultValue = true)
```
**Example**
```csharp
@DnnRocket.CheckBoxRowECOMode(Model.Info)
```
***
