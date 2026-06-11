
# AddProcessData
**Description**: Adds metadata to the current rendering process. This data can be used by other tokens or within the same template. Returns an empty string.
**Signature**
```csharp
public IEncodedString AddProcessData(String metaType, String metaValue)
```
**Example**
```csharp
@AddProcessData("mykey", "myvalue")
```
***
# AddPreProcessData
**Description**: Adds metadata to a specific cache list before the Razor template is rendered. This allows module code to use this data (e.g., for database queries) before rendering. It requires a unique template name and module ID to create a specific cache key.
**Signature**
```csharp
public IEncodedString AddPreProcessData(String metaKey, String metaValue, String templateFullName, String moduleId)
```
**Example**
```csharp
@AddPreProcessData("sortfield", "name", "MyTheme.list.cshtml", "123")
```
***
# AddCssLinkHeader
**Description**: Generates a <link> tag to include a CSS file in the HTML header.
**Signature**
```csharp
public IEncodedString AddCssLinkHeader(string cssRelPath)
```
**Example**
```csharp
@AddCssLinkHeader("/DesktopModules/MyModule/style.css")
```
***
# AddJsScriptHeader
**Description**: Generates a <script> tag to include a JavaScript file in the HTML header.
**Signature**
```csharp
public IEncodedString AddJsScriptHeader(string jsRelPath)
```
**Example**
```csharp
@AddJsScriptHeader("/DesktopModules/MyModule/script.js")
```
***
# HiddenField
**Description**: Renders a hidden input field bound to a SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString HiddenField(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@HiddenField(Model.Info, "genxml/hidden/mydata")
```
***
# TextBox
**Description**: Renders a text input field bound to a SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString TextBox(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "", string type = "text")
```
**Example**
```csharp
@TextBox(Model.Info, "genxml/textbox/firstname")
```
***
# TextBoxDate
**Description**: Renders a date input field (type='date') bound to a SimplisityInfo data model. The value is formatted as 'yyyy-MM-dd'.
**Signature**
```csharp
public IEncodedString TextBoxDate(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@TextBoxDate(Model.Info, "genxml/date/startdate")
```
***
# TextArea
**Description**: Renders a textarea field bound to a SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString TextArea(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@TextArea(Model.Info, "genxml/textbox/description")
```
***
# CheckBox
**Description**: Renders a single checkbox with a label, bound to a boolean value in a SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString CheckBox(SimplisityInfo info, String xpath, String text, String attributes = "", Boolean defaultValue = false, bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@CheckBox(Model.Info, "genxml/checkbox/isactive", "Is Active")
```
***
# CheckBoxList
**Description**: Renders a list of checkboxes from a dictionary or comma-separated strings. Each checkbox corresponds to a sub-node in the SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString CheckBoxList(SimplisityInfo info, string xpath, Dictionary<string, string> dataDictionary, string attributes = "", bool defaultValue = false, bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@CheckBoxList(Model.Info, "genxml/checkboxlist/options", myDictionary)
```
***
# RadioButtonList
**Description**: Renders a list of radio buttons from a dictionary or comma-separated strings, bound to a single field in the SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString RadioButtonList(SimplisityInfo info, string xpath, Dictionary<string, string> dataDictionary, string attributes = "", string defaultValue = "", string labelattributes = "", bool localized = false, int row = 0, string listname = "", string inputclass = "")
```
**Example**
```csharp
@RadioButtonList(Model.Info, "genxml/radio/selection", myDictionary)
```
***
# DropDownList
**Description**: Renders a dropdown list (select) from a dictionary or comma-separated strings, bound to a single field in the SimplisityInfo data model.
**Signature**
```csharp
public IEncodedString DropDownList(SimplisityInfo info, String xpath, Dictionary<string, string> dataDictionary, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```
**Example**
```csharp
@DropDownList(Model.Info, "genxml/dropdown/category", myDictionary)
```
***
# SortableListIndex
**Description**: Outputs hidden fields required for a sortable list to correctly process the sort order. This includes a unique item reference and the current row index.
**Signature**
```csharp
public IEncodedString SortableListIndex(SimplisityInfo info, int row)
```
**Example**
```csharp
@SortableListIndex(Model.Info, i)
```
***
# EmailOf
**Description**: Formats an email address from the data model as a 'mailto:' link.
**Signature**
```csharp
public IEncodedString EmailOf(SimplisityInfo info, String xpath, string subject = "", string visibleText = "")
```
**Example**
```csharp
@EmailOf(Model.Info, "genxml/textbox/email", "Inquiry")
```
***
# HtmlOf
**Description**: HTML-decodes a string from the data model or a direct string, rendering it as raw HTML.
**Signature**
```csharp
public IEncodedString HtmlOf(SimplisityInfo info, String xpath)
```
**Example**
```csharp
@HtmlOf(Model.Info, "genxml/richtext/content")
```
***
# DateOf
**Description**: Formats a date from the data model or a DateTime object into a string using a specific culture and format.
**Signature**
```csharp
public IEncodedString DateOf(SimplisityInfo info, String xpath, String cultureCode, String format = "d")
```
**Example**
```csharp
@DateOf(Model.Info, "genxml/date/publishdate", "en-GB", "D")
```
***
# Succinct
**Description**: Shortens a string to a specified length and appends '...' if it was truncated.
**Signature**
```csharp
public IEncodedString Succinct(string value, int size, bool showdots = true)
```
**Example**
```csharp
@Succinct(Model.Get("summary"), 100)
```
***
# BreakOf
**Description**: Converts newline characters in a string to <br/> tags and HTML-encodes the content.
**Signature**
```csharp
public IEncodedString BreakOf(SimplisityInfo info, String xpath)
```
**Example**
```csharp
@BreakOf(Model.Info, "genxml/textbox/multilinetext")
```
***
# CheckBoxListOf
**Description**: Displays a formatted list (ul/li) of the selected items from a checkbox list.
**Signature**
```csharp
public IEncodedString CheckBoxListOf(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "")
```
**Example**
```csharp
@CheckBoxListOf(Model.Info, "genxml/checkboxlist/options", "1,2,3", "Option 1,Option 2,Option 3")
```
***
# FileSelectList
**Description**: Renders a dropdown list of files from a specified directory.
**Signature**
```csharp
public IEncodedString FileSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
```
**Example**
```csharp
@FileSelectList(Model.Info, "genxml/dropdown/templatefile", Server.MapPath("~/MyTemplates"))
```
***
# FolderSelectList
**Description**: Renders a dropdown list of subdirectories from a specified directory.
**Signature**
```csharp
public IEncodedString FolderSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
```
**Example**
```csharp
@FolderSelectList(Model.Info, "genxml/dropdown/themefolder", Server.MapPath("~/MyThemes"))
```
***
# SFields
**Description**: Generates a 's-fields' HTML attribute containing a JSON object from a series of key-value pairs. This is used for client-side scripting with Simplisity.
**Signature**
```csharp
public IEncodedString SFields(params string[] sFields)
```
**Example**
```csharp
@SFields("key1", "value1", "key2", "value2")
```
***
# SecuritySiteKey
**Description**: Renders a hidden element containing the current session's site key. This can be used for security validation in client-side calls.
**Signature**
```csharp
public IEncodedString SecuritySiteKey(SessionParams sessionParams)
```
**Example**
```csharp
@SecuritySiteKey(Model.SessionParams)
```
