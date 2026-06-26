# Create an AppTheme

## Introduction

There are two ways to create a new AppTheme:

- **Copy an existing AppTheme** – the easiest and preferred method
- **Build from scratch** – full control, but requires manually constructing every file and folder

In both cases you must restart the IIS Application Pool whenever a new file is added, and clear the Rocket cache when editing template content.

---

## Method 1: Copy an Existing AppTheme (Recommended)

The fastest way to create a new AppTheme is to copy an existing one that is close to what you want and then customise it.

### Steps

1. Open the AppThemes folder for your project, for example:
   ```
   \DesktopModules\RocketThemes\AppThemes-W3-CSS\
   ```

2. Find a theme whose layout is closest to your needs.  
   Good starting points included in `AppThemes-W3-CSS` are:
   - `rocketcontentapi.About` – a simple single-content layout
   - `rocketcontentapi.HtmlContent` – HTML/rich-text layout with CSS injection

3. Copy the entire folder.

4. Paste it back into the same directory and rename it using the convention:
   ```
   <systemkey>.<ThemeName>
   ```
   For example:
   ```
   rocketcontentapi.MyNewTheme
   ```
   > **Important:** AppTheme folder names **must not start with a number**. Themes with a numeric prefix will not appear in the module selector.

5. Restart the IIS Application Pool.

6. The new AppTheme `MyNewTheme` will now appear in the dropdown when configuring a RocketContent module.

You can now edit the admin templates and view templates inside the new folder.

---

## Method 2: Create from Scratch

If you need a completely custom structure, you can build the folder and files manually.

### Minimum Required Structure

```
\DesktopModules\RocketThemes\<ProjectName>\<systemkey>.<ThemeName>\
    versioncontrol.xml
    1.0\
        default\
            View.cshtml
            AdminDetail.cshtml
            AdminRow.cshtml
        dep\
            <themename>.dep
```

The `versioncontrol.xml` contains the minimum version metadata:

```xml
<genxml>
    <rocketversion>1.0.0</rocketversion>
</genxml>
```

Add files and subfolders as needed. **Restart the Application Pool every time a new file is added** so the system registers it.

---

## AppTheme Folder Structure

A typical versioned AppTheme folder looks like this:

```
rocketcontentapi.HtmlContent\
    versioncontrol.xml
    img\                         ← Preview thumbnails shown in the AppTheme selector
        Newspaper0.png
        Newspaper1.png
        ...
    1.0\
        css\                     ← Theme-specific CSS files
            HtmlContent.css
        js\                      ← Theme-specific JavaScript files
        default\                 ← Razor templates (.cshtml)
            AdminDetail.cshtml
            AdminFirstHeader.cshtml
            AdminLastHeader.cshtml
            AdminRow.cshtml
            View.cshtml
            ViewHeader.cshtml
            ThemeSettings.cshtml
            ...
        dep\                     ← Dependency file (CSS/JS injection)
            htmlcontent.dep
        resx\                    ← Resource/localisation files
            HtmlContent.resx
            HtmlContent.fr-FR.resx
```

### Versioning

The `1.0` folder holds the current version of the theme. The version number in `versioncontrol.xml` is matched against the system version. If a newer version folder exists it will be used automatically when the system upgrades.

---

## Templates

### Admin Templates

Admin templates control the data-entry interface shown in the Rocket admin panel.

| Template | Purpose |
|---|---|
| `AdminDetail.cshtml` | Data-entry form for a single item |
| `AdminRow.cshtml` | Row rendering in the admin list |
| `AdminFirstHeader.cshtml` | CSS/JS to inject in the admin panel header (top) |
| `AdminLastHeader.cshtml` | CSS/JS to inject at the bottom of the admin panel header |
| `ThemeSettings.cshtml` | Optional per-module display settings |

### View Templates

View templates control the public-facing output.

| Template | Purpose |
|---|---|
| `View.cshtml` | Main front-end display template |
| `ViewHeader.cshtml` | Optional header-area content for the view |

> You can add as many custom templates as your theme needs. Additional templates will be listed in the module settings dropdown when registered in the `.dep` file (see `moduletemplates` below).

---

## The Dependency File (.dep)

The `.dep` file is an XML file that controls what CSS and JavaScript files are injected onto the page when the AppTheme is active, and defines other metadata used by the system.

It lives in the `dep` subfolder of the versioned theme:

```
1.0\dep\<themename>.dep
```

### Basic Structure

```xml
<genxml>
    <deps list="true">
        <genxml>
            <ctrltype>css</ctrltype>
            <url>/DesktopModules/DNNrocket/css/w3.css</url>
            <ecofriendly>true</ecofriendly>
            <condition check="notexist">/Portals/_default/skins/rocketw3/w3.css</condition>
        </genxml>
        <genxml>
            <ctrltype>css</ctrltype>
            <url>{appthemefolder}/css/HtmlContent.css</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
    </deps>
</genxml>
```

### Key Elements

| Element | Values | Description |
|---|---|---|
| `<ctrltype>` | `css` or `js` | The type of file to load |
| `<url>` | path or token | The file to load (see URL tokens below) |
| `<ecofriendly>` | `true` / `false` | When ECO Mode is ON, only `true` items load |
| `<condition check="notexist">` | path | Skip if this file already exists on the file system |
| `<ignoreonskin>` | comma-separated skin names | Skip if the active DNN skin name contains any of these values |
| `<ignoreontemplate>` | comma-separated template filenames | Skip if the current display template matches |

### URL Tokens

| Token | Replaced With |
|---|---|
| `{appthemefolder}` | Path to the versioned theme folder, e.g. `/DesktopModules/RocketThemes/AppThemes-W3-CSS/rocketcontentapi.HtmlContent/1.0` |
| `{appthemesystemfolder}` | Path to the system-level theme folder |
| `{domainurl}` | Full domain URL including scheme, e.g. `https://yoursite.com` |
| `{jquery}` | Loads the jQuery library |

### ECO Mode

Each module has an **ECO Mode** setting:

- **ECO Mode ON** – only dependencies with `<ecofriendly>true</ecofriendly>` are loaded. Faster page load.
- **ECO Mode OFF** – all dependencies load regardless of their eco-friendly setting.

Mark your essential framework and theme CSS as `ecofriendly="true"`. Mark optional enhancements (animations, sliders) as `ecofriendly="false"`.

### Module Templates (moduletemplates)

Register additional display templates so they appear in the module settings dropdown:

```xml
<moduletemplates list="true">
    <genxml>
        <file>view.cshtml</file>
        <name>List View</name>
        <cmd>list</cmd>
    </genxml>
</moduletemplates>
```

| Element | Description |
|---|---|
| `<file>` | The template filename |
| `<name>` | Friendly label shown in module settings |
| `<cmd>` | The data command sent with the render call |

### Search Index (RocketContent)

For RocketContent AppThemes, you can define which fields are used for DNN search indexing:

```xml
<searchindex list="true">
    <genxml>
        <searchtitle>genxml/lang/genxml/textbox/title</searchtitle>
        <searchdescription>genxml/lang/genxml/textbox/richtext</searchdescription>
        <searchbody>genxml/lang/genxml/textbox/richtext</searchbody>
    </genxml>
</searchindex>
```

> **Note:** RocketDirectory-based systems (RocketBlog, RocketNews, RocketEvents) have their own fixed search fields and do not use this section.

---

## Editing an AppTheme

### Adding a New File

Whenever you add a **new file** to an AppTheme folder (template, CSS, JS, etc.), you must **restart the Application Pool** so the system picks it up.

### Editing Existing Templates

When editing existing `.cshtml` templates, no AppPool restart is needed. However, Rocket caches compiled templates. After making changes you must clear the cache using the **Rocket Personabar > Clear Cache** option, otherwise you will not see your changes.

### IntelliSense Support

For Razor IntelliSense, open your AppTheme files in **VScode**.  
See the [VScode IntelliSense](VScodeIntellisense.md).

---

## Naming Rules Summary

| Rule | Detail |
|---|---|
| Folder format | `<systemkey>.<ThemeName>` |
| No numeric prefix | Names starting with a digit will not appear in the module selector |
| System key examples | `rocketcontentapi`, `rocketdirectoryapi`, `rocketblogapi`, `rocketnewsapi` |
| Version folder | Numeric, e.g. `1.0` – matched against `versioncontrol.xml` |
| Dep file name | Any name, must have `.dep` extension, placed in `1.0\dep\` |
