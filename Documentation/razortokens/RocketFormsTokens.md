
# AssignDataModel
**Description**: Assigns the data model for Razor, making the template easier to build by populating various data properties like appTheme, moduleData, portalData, etc., from the SimplisityRazor model.
**Signature**
```csharp
public string AssignDataModel(SimplisityRazor sModel)
```
**Example**
```csharp
@{ AssignDataModel(Model); }
```
***
# DelayFormButton
**Description**: Renders a form submission button that appears after a specified delay. This is a security measure to help prevent automated bot submissions. It renders a specified Razor template for the apearance.
**Signature**
```csharp
public IEncodedString DelayFormButton(SimplisityRazor sModel, string spost, int millisec = 1200, string template = "DelayFormButton.cshtml")
```
**Example**
```csharp
@DelayFormButton(Model, "#postbuttonwrapper", 2000)
```
***
# ResourceKey
**Description**: Gets a localized resource string. It searches through a list of resource paths and file keys (including 'RocketForms' and theme-specific resx files) to find the requested key.
**Signature**
```csharp
public string ResourceKey(AppThemeLimpet appTheme, String resourceKey, String lang = "", String resourceExtension = "Text")
```
**Example**
```csharp
@ResourceKey(appTheme, "submitbutton")
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
