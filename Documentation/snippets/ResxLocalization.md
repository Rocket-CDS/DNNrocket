
## LOCALIZATION SYSTEM

### ResourceKey() Method Overview
The Razor templates use a `ResourceKey()` method to display localized text content from .resx resource files.

### Resource File Rules
1. **Default Project Resources**: `<ProjectName>.resx`
   - Use unless specifically told to create template-specific files
   - Located in `App_Resources` folder

2. **Template-Specific Resources**: `<TemplateName>.resx`
   - Only create when explicitly requested
   - Template name without file extension
   - Example: For `ProductForm.cshtml` → `ProductForm.resx`

3. **French Resource Files**: `<FileName>.fr-FR.resx`
 - Must contain all resource keys from English file
   - Leave values empty (no translation required)
 - User will handle translations

### Shared Rsource File
The file called DNNrocket.resx is a shared file from another project, that is always applied.  You should never alter the DNNrocket.resx file and any Resource Keys that use it should be unaltered by you. 

### Shared Resource File (DNNrocket.resx)
**IMPORTANT: DO NOT MODIFY**

- `DNNrocket.resx` is a shared resource file from an external project dependency
- This file is automatically included and applied to all projects
- **Never create, modify, or delete** the `DNNrocket.resx` file
- **Never alter any `ResourceKey()` calls** that reference `DNNrocket.resx`
  - Example: `@ResourceKey("DNNrocket.save")` - leave unchanged
  - Do not move these keys to project-specific resource files
- Only create/modify resource keys in your project's own `.resx` files

**When you see existing ResourceKey calls using "DNNrocket.*":**
- Leave them exactly as they are
- Do not suggest changes or refactoring
- These are managed by the external dependency

### ResourceKey() Method Syntax
```csharp
ResourceKey(String resourceFileKey, String lang = "", String resourceExtension = "Text")
```

#### Parameters:
- **resourceFileKey**: Two-level dot notation `"<FileName>.<ResourceKey>"`
  - Format: `<FileName(without extension)>.<resx key (lowercase)>`
  - Example: `"ProjectName.welcomemessage"`
  - Split on '.' produces exactly 2 parts

- **lang**: Language code (optional)
  - Leave blank unless specifically instructed
  - Default behavior handles language selection

- **resourceExtension**: Resource file extension
  - Always use default value `"Text"`
  - Full resource path becomes: `<FileName>.Text`

#### Usage Examples:
```razor
@ResourceKey("ProjectName.pagetitle")
@ResourceKey("ContactForm.submitbutton")
@ResourceKey("TransportEstimate.calculationerror")
```

### Implementation Protocol
1. **Create English .resx**: Add keys with English text values
2. **Create French .resx**: Add same keys with empty values
3. **No Translation**: Do not provide translated text - user handles translation
4. **Key Naming**: Use descriptive, hierarchical key names
5. **Default Selection**: Use project-level .resx unless told otherwise

### Resource File Content Format
**CRITICAL: Do NOT include XSD schema in .resx files - it breaks DNN compatibility**

**Example: English File (ProjectName.resx):**
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
</resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name="welcomemessage.Text" xml:space="preserve">
 <value>Welcome to Transport Estimate</value>
  </data>
  <data name="calculatebutton.Text" xml:space="preserve">
    <value>Calculate</value>
  </data>
</root>
```

**Example: French File (ProjectName.fr-FR.resx):**
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype">
<value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
  <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
  <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name="welcomemessage.Text" xml:space="preserve">
    <value></value>
  </data>
  <data name="calculatebutton.Text" xml:space="preserve">
    <value></value>
  </data>
</root>
```

### Resource File Requirements
- **NO XSD Schema**: Never include `<xsd:schema>` elements in .resx files as they break DNN
- **Simple Structure**: Only include `<resheader>` and `<data>` elements
- **Standard Headers**: Use the standard ResX headers shown above
- **XML Declaration**: Always include `<?xml version="1.0" encoding="utf-8"?>` at the top
- **Do NOT create a SqlReports.Designer.cs file**: And prompt/ask if I want it deleted if one exists.









 