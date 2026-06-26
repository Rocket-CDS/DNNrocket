# RocketContent AppTheme

## Overview

RocketContent is the simplest Rocket module system. It stores data as a single article containing one or more **rows**, where each row can hold text, images, documents, links, and rich-text content.

A RocketContent AppTheme controls how that data is displayed on the front end and how it is edited in the admin panel.

---

## Template Header

All RocketContent templates use:

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
```

The `@AssignDataModel(Model)` call is required. It populates all pre-defined model objects and sets up resource paths automatically.

The `<!--inject-->` marker is required if the template will be used as an injected sub-template. Place it after the `@inherits` / `@AssignDataModel` lines and before any HTML output.

---

## Pre-Defined Model Objects

After `@AssignDataModel(Model)` has been called, the following objects are available directly in the template without any additional `GetDataObject` calls:

| Object | Type | Description |
|---|---|---|
| `articleData` | `ArticleLimpet` | The article record being edited or displayed. `null` in list/row contexts. |
| `articleRowData` | `ArticleRowLimpet` | The current row when iterating article rows. |
| `moduleData` | `ModuleContentLimpet` | Module settings and configuration. Use `.GetSetting("key")` to read values. |
| `portalData` | `PortalLimpet` | Portal-level data (name, culture, paths, etc.). |
| `sessionParams` | `SessionParams` | Current request session data, culture, page, paging, etc. |
| `userParams` | `UserParams` | Current logged-in user details. |
| `appTheme` | `AppThemeLimpet` | The admin AppTheme (same as `appThemeAdmin`). |
| `appThemeView` | `AppThemeLimpet` | The view/front-end AppTheme object. |
| `appThemeAdmin` | `AppThemeLimpet` | The admin AppTheme object. |
| `appThemeSystem` | `AppThemeSystemLimpet` | The system-level shared AppTheme object. |
| `info` | `SimplisityInfo` | Raw XML data node for the current row (or article header if no row). Use for direct `GetXmlProperty` calls. |
| `rowData` | `SimplisityInfo` | Same as `info` — alias for row context. |
| `headerData` | `SimplisityInfo` | Article-level info node (the article header, not a row). |
| `moduleDataInfo` | `SimplisityInfo` | Raw XML info node for the module record. |

### AppTheme Object Keys (for `[INJECT]`)

| Key | Object |
|---|---|
| `apptheme` | `appThemeAdmin` (admin theme) |
| `appthemeview` | `appThemeView` (view/front-end theme) |
| `appthemeadmin` | `appThemeAdmin` |
| `appthemesystem` | `appThemeSystem` (shared system templates) |

---

## Standard Template Set

A typical RocketContent AppTheme contains these templates:

| Template | Purpose |
|---|---|
| `View.cshtml` | Main front-end display. Iterates article rows. |
| `ViewHeader.cshtml` | Injected into the page header area for the view. |
| `AdminDetail.cshtml` | Admin data-entry form for the article. |
| `AdminRow.cshtml` | Fields for a single article row in the admin panel. |
| `AdminFirstHeader.cshtml` | CSS/JS loaded in the top of the admin panel `<head>`. |
| `AdminLastHeader.cshtml` | CSS/JS loaded at the bottom of the admin panel `<head>`. |
| `ThemeSettings.cshtml` | Per-module display settings panel. |

Optional partial sub-templates (injected via `[INJECT]`):

| Template | Purpose |
|---|---|
| `ArticleHeader.cshtml` | Injected once per article iteration in the view. |
| `Layout0.cshtml` … `LayoutN.cshtml` | Separate layout variations selected per row. |
| `HtmlText.cshtml` | Rich-text block sub-template. |
| `Documents.cshtml` | Document list sub-template. |
| `Links.cshtml` | Link list sub-template. |

---

## View Template Pattern

The view template iterates the rows of the article and injects sub-templates based on per-row settings:

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->
@{
    var stylepadding = StylePadding();
    var backgroundcolor = moduleData.GetSetting("backgroundcolor");
}
<div class="containerouter @backgroundcolor @(moduleData.GetSetting("cssclass"))">
    <div class="w3-section containerinner" style="@stylepadding">

        @foreach (var articleRowData in articleData.GetRows())
        {
            Model.SetDataObject("articlerow", articleRowData);
            var info = articleRowData.Info;
            var layout = info.GetXmlPropertyInt("genxml/select/layout");

            if (!info.GetXmlPropertyBool("genxml/checkbox/hiderow"))
            {
                [INJECT: apptheme, ArticleHeader.cshtml]

                @if (layout == 0)
                {
                    <text>[INJECT: apptheme, Layout0.cshtml]</text>
                }
                @if (layout == 1)
                {
                    <text>[INJECT: apptheme, Layout1.cshtml]</text>
                }
            }
        }

    </div>
</div>
```

> **Note:** `articleRowData` is re-assigned to `Model` via `Model.SetDataObject("articlerow", articleRowData)` before injecting row sub-templates. This ensures the `info` and `rowData` objects in injected sub-templates refer to the current row.

---

## Admin Templates

### AdminDetail.cshtml

The admin detail template is the entry point for the article edit form. It typically injects the row editor:

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->

<div class="w3-row">
    <div id="articledetailpanel" class="w3-threequarter">
        [INJECT:appthemeadmin,AdminRow.cshtml]
    </div>
    <div class="w3-quarter">
        [INJECT:appthemesystem,AdminRowSelect.cshtml]
    </div>
</div>
```

### AdminRow.cshtml

`AdminRow.cshtml` contains the actual input fields. It uses `info` (which points to the current row's XML) and `rowData` for field data:

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->

@RowKey(rowData)

<div class="w3-row w3-padding">
    <label>@ResourceKey("DNNrocket.title")</label>
    @EditFlag(sessionParams)
    @TextBox(info, "genxml/lang/genxml/textbox/title",
        "class='w3-input w3-border w3-margin-bottom' autocomplete='off'", "")
</div>

<div class="w3-row w3-padding">
    <label>@ResourceKey("DNNrocket.image")</label>
    [INJECT:appthemesystem,ArticleImage.cshtml]
</div>
```

> `@RowKey(rowData)` is **required** in every row admin template. It generates hidden fields that allow the row to be saved back to the database.

### AdminFirstHeader.cshtml

Inject the rich-text editor and any other admin panel CSS/JS here:

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
[INJECT:appthemesystem,CKEditor4.cshtml]
```

### AdminLastHeader.cshtml

Used for any scripts that must load at the end of the admin head:

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
```

---

## ThemeSettings.cshtml

This template provides the per-module display settings panel. It reads and saves values under the `genxml/settings/` xPath on the module record:

```cshtml
@{
    // NOTE: xPath for module settings must use "genxml/settings/*"
    var moduleData = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var info = new SimplisityInfo(moduleData.Record);
}

<div class="w3-third w3-padding">
    <label>Width (px)</label>
    @TextBox(info, "genxml/settings/width", "class='w3-input w3-border'", "320")
</div>
<div class="w3-third w3-padding">
    <label>Height (px)</label>
    @TextBox(info, "genxml/settings/height", "class='w3-input w3-border'", "320")
</div>
```

Use `moduleData.GetSetting("width")` and `moduleData.GetSettingInt("width")` to read these values back in the view template.

---

## XPath Data Convention

RocketContent data is stored in XML. The standard xPath layout:

| Data type | xPath |
|---|---|
| Plain text field | `genxml/textbox/myfieldname` |
| Localised text field | `genxml/lang/genxml/textbox/myfieldname` |
| Checkbox | `genxml/checkbox/mycheckbox` |
| Select / DropDown | `genxml/select/myselect` |
| Image (first) | accessed via `articleRowData.GetImage(0)` |
| Module setting | `genxml/settings/mysetting` |

---

## Dependency File

RocketContent AppThemes typically need a minimal `.dep` file to load the theme CSS:

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
            <url>{appthemefolder}/css/MyTheme.css</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
    </deps>
    <searchindex list="true">
        <genxml>
            <searchtitle>genxml/lang/genxml/textbox/title</searchtitle>
            <searchdescription>genxml/lang/genxml/textbox/text</searchdescription>
            <searchbody>genxml/lang/genxml/textbox/richtext</searchbody>
        </genxml>
    </searchindex>
</genxml>
```

> The `<searchindex>` section defines which fields are sent to the DNN search index.  
> See [CreateAppTheme](CreateAppTheme.md) for the full dependency file reference.

---

## System-Level Shared Templates

The `appthemesystem` key gives access to shared admin sub-templates provided by the RocketContent system. These cover image management, document lists, link lists, and the row-selector panel. You do not need to recreate these in your AppTheme:

| Shared Template | Purpose |
|---|---|
| `[INJECT:appthemesystem,ArticleImage.cshtml]` | Single image upload/select for a row |
| `[INJECT:appthemesystem,ArticleImages.cshtml]` | Multiple images for a row |
| `[INJECT:appthemesystem,ArticleDocuments.cshtml]` | Document list for a row |
| `[INJECT:appthemesystem,ArticleLinks.cshtml]` | Link list for a row |
| `[INJECT:appthemesystem,AdminRowSelect.cshtml]` | Row selector panel (add/remove/reorder rows) |
| `[INJECT:appthemesystem,CKEditor4.cshtml]` | CKEditor rich-text editor loader |
