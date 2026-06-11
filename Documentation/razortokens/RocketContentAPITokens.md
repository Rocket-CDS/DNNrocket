
# AssignDataModel
**Description**: Assigns the data model for Razor, making the template easier to build by populating various data properties like articleData, appTheme, moduleData, etc., from the SimplisityRazor model.
**Signature**
```csharp
public string AssignDataModel(SimplisityRazor sModel)
```
**Example**
```csharp
@{ AssignDataModel(Model); }
```
***
# RowKey
**Description**: Generates the necessary hidden fields for a row's unique key ('rowkey' and 'rowkeylang') and a unique entity ID ('eid'). A row MUST have a rowkey to be saved to the database.
**Signature**
```csharp
public IEncodedString RowKey(SimplisityInfo info)
```
**Example**
```csharp
@RowKey(Model.Info)
```
***
# CheckBoxRowIsHidden
**Description**: Creates a checkbox for the 'IsHidden' property of a row, allowing a row to be marked as hidden.
**Signature**
```csharp
public IEncodedString CheckBoxRowIsHidden(SimplisityInfo rowData)
```
**Example**
```csharp
@CheckBoxRowIsHidden(Model.Info)
```
***
# TextBoxRowTitle
**Description**: Creates a standard textbox for a row's title using the XPath 'genxml/lang/genxml/textbox/title'.
**Signature**
```csharp
public IEncodedString TextBoxRowTitle(SimplisityInfo rowData)
```
**Example**
```csharp
@TextBoxRowTitle(Model.Info)
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
***
# StylePadding
**Description**: Generates an inline CSS padding style string based on module settings. It reads 'leftpadding', 'rightpadding', 'toppadding', and 'bottompadding' settings and creates corresponding CSS properties.
**Signature**
```csharp
public string StylePadding()
```
**Example**
```csharp
<div style="@StylePadding()">...</div>
```
