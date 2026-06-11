
# AssignDataModel
**Description**: Assigns the data model for Razor, making the template easier to build by populating various data properties like appTheme, moduleData, articleData, etc., from the SimplisityRazor model.
**Signature**
```csharp
public string AssignDataModel(SimplisityRazor sModel)
```
**Example**
```csharp
@{ AssignDataModel(Model); }
```
***
# TextBoxMoney
**Description**: Renders a textbox for currency input. The value is formatted according to the portal's currency settings.
**Signature**
```csharp
public IEncodedString TextBoxMoney(int portalId, string systemKey, string cultureCode, SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "", string type = "text")
```
**Example**
```csharp
@TextBoxMoney(0, "rocketdirectoryapi", "en-US", Model.Info, "genxml/textbox/price")
```
***
# InterfaceNameResourceKey
**Description**: Gets the localized name for a RocketInterface from the resource files. It searches in the system's resources first, then the interface's template resources.
**Signature**
```csharp
public IEncodedString InterfaceNameResourceKey(RocketInterface rocketInterface, SystemLimpet systemData, String lang = "", string resxFileName = "SideMenu")
```
**Example**
```csharp
@InterfaceNameResourceKey(myInterface, systemData)
```
***
# FilterGroupCheckBox
**Description**: Renders a checkbox for a property group filter, typically used in theme settings to enable or disable filter groups.
**Signature**
```csharp
public IEncodedString FilterGroupCheckBox(SimplisityInfo info, string groupId, string textName)
```
**Example**
```csharp
@FilterGroupCheckBox(Model.Info, "group1", "Group 1 Filters")
```
***
# FilterCheckBox
**Description**: Renders a filter checkbox for the public-facing view. When changed, it updates a session field and triggers a JavaScript function to refresh the article list.
**Signature**
```csharp
public IEncodedString FilterCheckBox(string checkboxId, string textName, string sreturn, bool value, string cssClass = "", string attributes = "")
```
**Example**
```csharp
@FilterCheckBox("filter-color-red", "Red", "#articlelist", false)
```
***
# FilterJsApiCall
**Description**: Renders the JavaScript function 'callFilterArticleList' which calls the remote API to refresh the list of articles based on the current filter selections.
**Signature**
```csharp
public IEncodedString FilterJsApiCall(ModuleContentLimpet moduleData, SessionParams sessionParams, string templateName = "articlelist.cshtml")
```
**Example**
```csharp
@FilterJsApiCall(moduleData, sessionParams)
```
***
# FilterClearButton
**Description**: Renders a button that clears all active filters by unchecking all filter checkboxes and refreshing the article list.
**Signature**
```csharp
public IEncodedString FilterClearButton(string textName, string sreturn)
```
**Example**
```csharp
@FilterClearButton("Clear All", "#articlelist")
```
***
# TagButtonClear
**Description**: Renders a button to clear the active tag filter. It is initially hidden and appears when a tag is selected.
**Signature**
```csharp
public IEncodedString TagButtonClear(string textName, SessionParams sessionParams, string displayClass = "rocket-tagbutton")
```
**Example**
```csharp
@TagButtonClear("Clear Tag", sessionParams)
```
***
# TagButton
**Description**: Renders a clickable tag button. When clicked, it sets the 'rocketpropertyidtag' session field and refreshes the article list to show items with that tag.
**Signature**
```csharp
public IEncodedString TagButton(int propertyid, string textName, SessionParams sessionParams, string displayClass = "rocket-tagbutton", string selectedClass = "rocket-tagbuttonOn")
```
**Example**
```csharp
@TagButton(123, "Featured", sessionParams)
```
***
# TagJsApiCall
**Description**: Renders the JavaScript function 'callTagArticleList' which calls the remote API to refresh the list of articles based on the selected tag.
**Signature**
```csharp
public IEncodedString TagJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string templateName = "articlelist.cshtml")
```
**Example**
```csharp
@TagJsApiCall(moduleData, "#articlelist", sessionParams)
```
***
# DateJsApiCall
**Description**: Renders the JavaScript function 'doDateSearchReload' which calls the remote API to refresh the list of articles based on a selected date range.
**Signature**
```csharp
public IEncodedString DateJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string templateName = "articlelist.cshtml")
```
**Example**
```csharp
@DateJsApiCall(moduleData, "#articlelist", sessionParams)
```
***
# ListUrl
**Description**: Builds a friendly URL to a list page, optionally including category information.
**Signature**
```csharp
public IEncodedString ListUrl(int listpageid, CategoryLimpet categoryData, string[] urlparams = null)
```
**Example**
```csharp
@ListUrl(100, myCategory)
```
***
# DetailUrl
**Description**: Builds a friendly URL to a detail page for a specific article, including the article title and ID for SEO and routing.
**Signature**
```csharp
public IEncodedString DetailUrl(int detailpageid, ArticleLimpet articleData, string[] urlparams = null)
```
**Example**
```csharp
@DetailUrl(101, myArticle)
```
***
# RssUrl
**Description**: Generates a URL for an RSS feed based on a command, date range, and optional SQL index.
**Signature**
```csharp
public IEncodedString RssUrl(int portalId, string cmd, int yearDate, int monthDate, int numberOfMonths = 1, string sqlidx = "")
```
**Example**
```csharp
@RssUrl(0, "getfeed", 2023, 1, 12)
```
***
# ChatGPT
**Description**: Renders a button to open a ChatGPT modal for generating text. Requires a ChatGPT API key in the global settings. The generated text will populate the field specified by 'textId'.
**Signature**
```csharp
public IEncodedString ChatGPT(string textId, string sourceTextId = "")
```
**Example**
```csharp
@ChatGPT("mytextarea", "mysourcetextbox")
```
***
# DeepL
**Description**: Renders a button to open a DeepL translation modal. Requires a DeepL API key in the global settings and more than one portal language to be enabled. The translated text will populate the field specified by 'textId'.
**Signature**
```csharp
public IEncodedString DeepL(string textId, string sourceTextId = "", string cultureCode = "")
```
**Example**
```csharp
@DeepL("translatedtext", "originaltext")
```
