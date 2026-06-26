# Editing an AppTheme

## Overview

AppTheme templates are standard Razor `.cshtml` files. You can edit them with any text editor or IDE that understands Razor syntax. The recommended tool is **Visual Studio Code** with the [Rocket Intellisense extension](VScodeIntellisense.md), which provides token completion specific to Rocket templates.

---

## Where Templates Live

Templates are stored inside the versioned AppTheme folder on the server file system:

```
\DesktopModules\RocketThemes\<ProjectName>\<systemkey>.<ThemeName>\<version>\default\
```

For portal-level overrides (customising a theme for a specific site without touching the base files):

```
\Portals\<PortalId>\DNNrocket\AppThemes\<ThemeName>\<version>\default\
```

Place a file with the same name as the original in the portal path and it will be used in place of the base file for that portal only. See [AppTheme Engine](AppThemeEngine.md) for the full cascading resolution order.

---

## Workflow: Editing Existing Templates

1. Open the template file in your editor.
2. Make your changes and save.
3. In the DNN Persona Bar, open the **Rocket** menu and click **Clear Cache**.

> The Rocket cache stores compiled Razor templates. Without clearing it, the old compiled version continues to run even after the file is saved.

You do **not** need to restart the Application Pool when editing files that already exist.

---

## Workflow: Adding a New File

Whenever a **new file** is added to an AppTheme folder (a new template, CSS file, JS file, etc.) the system must be made aware of it:

1. Add the file to the correct folder.
2. **Restart the IIS Application Pool.**
3. Clear the Rocket cache if needed.

The Application Pool restart is required because the file system is scanned at startup and new files are not detected at runtime.

---

## Template Header

Every template must begin with the correct `@inherits` line for the system it belongs to. This determines which model objects and token methods are available.

**RocketContent:**
```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
```

**RocketDirectory (and derived systems — RocketBlog, RocketNews, RocketEvents):**
```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
<!--inject-->
```

The `@AssignDataModel(Model)` call populates all the pre-defined model objects (see the system-specific pages for the full list).

The `<!--inject-->` comment marks the split point used by `[INJECT]` token merging — everything before it is the header (discarded when the template is used as a sub-template), and everything after is the content.

---

## The Inject Split Point

When a template is used as an injected sub-template via `[INJECT:key,template.cshtml]`, only the code **after** `<!--inject-->` is merged into the parent template. This means:

- Sub-templates still compile cleanly in isolation with full IntelliSense.
- The `@inherits`, `@using`, and `@AssignDataModel` lines are written once in the parent, not repeated in every sub-template.

If no `<!--inject-->` marker is present, the system falls back to splitting at the first `<div` tag (lower case).

---

## Resource Keys

Localisation strings are loaded by `@AddProcessDataResx(appTheme, true)` (or the `@AddProcessData("resourcepath", "...")` call). Use `@ResourceKey("MyKey")` anywhere in the template to render a localised string from the `.resx` files in the `resx` subfolder.

```cshtml
<label>@ResourceKey("DNNrocket.title")</label>
```

---

## Using the Editor Token (`@Editor`)

The rich-text editor (CKEditor or Jodit) is rendered with the `@Editor` token:

```cshtml
@Editor(info, "genxml/lang/genxml/textbox/richtext", Model, 0, "")
```

The editor requires CKEditor to be loaded. In the admin header template (`AdminFirstHeader.cshtml`), inject it via:

```cshtml
[INJECT:appthemesystem,CKEditor4.cshtml]
```

---

## Common Form Tokens

These are the most frequently used form-building tokens available in all templates:

| Token | Purpose |
|---|---|
| `@TextBox(info, xpath, attrs, default)` | Single-line text input |
| `@TextArea(info, xpath, attrs, default)` | Multi-line text area |
| `@Editor(info, xpath, Model, row, "")` | Rich-text editor |
| `@CheckBox(info, xpath, label, attrs, checked)` | Checkbox |
| `@DropDownList(info, xpath, list, attrs, default)` | Drop-down select |
| `@RadioButtonList(info, xpath, values, labels, attrs, default)` | Radio buttons |
| `@HiddenField(info, xpath)` | Hidden input |
| `@ResourceKey("key")` | Localised string from `.resx` |
| `@EditFlag(sessionParams)` | Multi-language edit-flag icon |
| `@DeepL("field", "xpath")` | DeepL translation button |
| `@ChatGPT("field")` | ChatGPT content-assist button |

---

## Saving Data: XPath Convention

All form field data is stored in an XML structure. The xPath determines where in the XML the value is stored.

- **Non-localised fields:** `genxml/textbox/myfieldname`
- **Localised fields:** `genxml/lang/genxml/textbox/myfieldname`
- **Module settings:** `genxml/settings/myfieldname` *(settings use a different record)*
- **Row data** (RocketContent rows): same xPaths, but on the `info` object which points to the current row

---

## Clearing the Cache

The Rocket cache must be cleared whenever template content has been edited:

1. Open the DNN Persona Bar.
2. Navigate to **Rocket** in the left menu.
3. Click **Clear Cache**.

Alternatively, any DNN host-level cache clear (e.g., the standard DNN "Clear Cache" host action) will also clear compiled Razor templates.

---

## IntelliSense in Visual Studio Code

Install the **Rocket Intellisense** VS Code extension for full token completion when editing `.cshtml` AppTheme files. See [VS Code Intellisense](VScodeIntellisense.md) for installation and usage.

---

## Tips

- Always put new templates through an Application Pool restart before testing — the file must be registered by the system first.
- Use `[INJECT]` to break large templates into smaller, manageable parts. Sub-templates can still be opened and edited independently with full IntelliSense.
- Use `@RenderTemplate("partial.cshtml", appThemeView, Model)` when you need an independently compiled sub-template (e.g. a shared component that must work standalone). Be aware this compiles separately on first use.
- Keep the `<!--inject-->` marker on its own line for clarity and to avoid accidental content trimming.
