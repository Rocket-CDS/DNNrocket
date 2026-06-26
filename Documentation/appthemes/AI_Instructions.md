# AI Instructions — Build a Rocket AppTheme

## How to Use This Document

This document is an instruction set for an AI assistant. When a user says something like:

> *"Build me a RocketContent AppTheme that shows a hero image with a title and two columns of text"*
> *"Create a RocketDirectory product catalogue AppTheme with a card grid list and a detail page"*
> *"Make a RocketBlog AppTheme that shows posts with a category sidebar"*

The AI should read this document and use it to produce a complete, working set of AppTheme files.

---

## Step 0 — Determine the Target System

Ask the user (or infer from context) which Rocket module system the AppTheme is for:

| System | Folder prefix | Use case |
|---|---|---|
| **RocketContent** | `rocketcontentapi.` | Simple single-article content with rows: text, images, rich-text, links, docs |
| **RocketDirectory** | `rocketdirectoryapi.` | Product/service catalogue — list + detail + categories + properties |
| **RocketBlog** | `rocketblogapi.` | Blog posts — list + detail + categories + tags + months archive |
| **RocketNews** | `rocketnewsapi.` | News articles — same structure as RocketBlog |
| **RocketEvents** | `rocketeventsapi.` | Date-based events — list + detail + calendar |

If the user says "RocketBlog", "RocketNews", or "RocketEvents", follow the **RocketDirectory** template rules below (they share the same base class) but use the system-specific values shown in the per-system tables.

---

## Step 1 — Determine the AppTheme Name and Output Path

The AppTheme folder name follows the convention:

```
<systemprefix>.<ThemeName>
```

**Rules:**
- MUST NOT start with a number or underscore.
- ThemeName MUST be PascalCase, no spaces.
- The folder goes inside an AppTheme project folder, e.g. `AppThemes-W3-CSS\`.

Ask the user for a name if not provided. Default to something descriptive of the design (e.g. `rocketcontentapi.HeroCard`).

Output the full set of files in this structure:

```
<systemprefix>.<ThemeName>\
    versioncontrol.xml
    1.0\
        default\
            (all .cshtml templates)
        css\
            <ThemeName>.css
        dep\
            <themenamelowercase>.dep
```

---

## Step 2 — Understand the User's Design Request

Before generating any code, extract the following from the user's request:

1. **Fields** — what data fields does the design need? (title, image, summary, date, price, rich-text body, etc.)
2. **List layout** — how should articles/rows appear in a list? (card grid, table rows, single column, etc.)
3. **Detail layout** — is there a separate detail/single-article view needed?
4. **Sidebar** — does the design include categories, tags, filters, or months?
5. **Admin fields** — what does the editor need to fill in? (maps directly to the fields above)
6. **Module settings** — are there per-module display options? (image size, show/hide elements, colour, etc.)

---

## Step 3 — Generate the Files

Generate ALL files listed for the target system. Do not skip any file, even if it is a minimal placeholder.

---

# SYSTEM: RocketContent

## When to Use

Use when the user wants simple page content: one article per module, with optional multiple rows per article. Best for hero sections, about pages, service blocks, icon grids, and similar fixed-layout content.

## Template Header (all templates)

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
```

`@AddProcessDataResx(appTheme, true)` is optional on view templates (adds resource key lookup for the view theme). Add it after `@AssignDataModel(Model)` if the template uses `@ResourceKey(...)`.

## Available Model Objects

| Object | How to use |
|---|---|
| `articleData` | The article being displayed or edited |
| `articleRowData` | The current row in a foreach loop |
| `info` | Raw XML node for the current row (use for `GetXmlProperty`) |
| `rowData` | Alias for `info` |
| `headerData` | Article-level info node (not a row) |
| `moduleData` | Module settings — `moduleData.GetSetting("key")`, `moduleData.GetSettingInt("key")`, `moduleData.GetSettingBool("key")` |
| `sessionParams` | Session data, culture, paging |
| `portalData` | Portal name, paths |
| `appTheme` | Admin AppTheme object (same as `appThemeAdmin`) |
| `appThemeView` | View AppTheme object |
| `appThemeAdmin` | Admin AppTheme object |
| `appThemeSystem` | System shared templates |

## Required Files

### `versioncontrol.xml`
```xml
<genxml>
    <rocketversion>1.0.0</rocketversion>
</genxml>
```

### `1.0\default\View.cshtml`

Main public display. Iterates rows. Use `[INJECT]` for layout sub-templates.

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
            if (!info.GetXmlPropertyBool("genxml/checkbox/hiderow"))
            {
                [INJECT:apptheme,LayoutRow.cshtml]
            }
        }

    </div>
</div>
```

> Replace `LayoutRow.cshtml` with your actual layout sub-template name(s). Add multiple `[INJECT]` blocks for multiple layout variants.

### `1.0\default\LayoutRow.cshtml`

The display sub-template for a single row. Build the HTML for the user's requested design here.

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
@{
    var articleImage = articleRowData.GetImage(0);
    var title = info.GetXmlProperty("genxml/lang/genxml/textbox/title");
    var summary = info.GetXmlProperty("genxml/lang/genxml/textbox/summary");
}
<div class="w3-row w3-padding">
    @if (articleImage.RelPath != "")
    {
        <img src="@ImageUrl(articleImage.RelPath, moduleData.GetSettingInt("width"), moduleData.GetSettingInt("height"))"
             alt="@title" class="w3-image" />
    }
    <h2 class="w3-padding-top">@title</h2>
    <p>@summary</p>
</div>
```

> Adapt the fields and HTML to match the user's request. Replace `title` and `summary` with the field names the user asked for.

### `1.0\default\AdminDetail.cshtml`

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

### `1.0\default\AdminRow.cshtml`

The admin data-entry form for a single row. Add one input block for every field the user's design needs.

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->

@RowKey(rowData)

<div class="w3-row w3-padding">
    <label>Title</label>
    @EditFlag(sessionParams)
    @TextBox(info, "genxml/lang/genxml/textbox/title",
        "class='w3-input w3-border w3-margin-bottom' autocomplete='off'", "")
</div>

<div class="w3-row w3-padding">
    <label>Summary</label>
    @EditFlag(sessionParams)
    @TextArea(info, "genxml/lang/genxml/textbox/summary",
        "class='w3-input w3-border w3-margin-bottom' autocomplete='off' rows='4'", "")
</div>

<div class="w3-row w3-padding">
    <label>Image</label>
    [INJECT:appthemesystem,ArticleImage.cshtml]
</div>

<div class="w3-row w3-padding">
    <label>Hide Row</label>
    @CheckBox(info, "genxml/checkbox/hiderow", "", "class='w3-check'", false)
</div>
```

> Add, remove, or rename fields to match the user's request. Use `genxml/lang/genxml/textbox/` for localised fields and `genxml/textbox/` for non-localised fields.

### `1.0\default\AdminFirstHeader.cshtml`

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
[INJECT:appthemesystem,CKEditor4.cshtml]
```

> Only needed if the admin form includes a rich-text `@Editor(...)` field. Leave minimal if not.

### `1.0\default\AdminLastHeader.cshtml`

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
```

### `1.0\default\ThemeSettings.cshtml`

Per-module display settings. Add controls for any display options the user's design requires.

```cshtml
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@using Simplisity;
@using RocketContentAPI.Components;
@using DNNrocketAPI.Components;
@{
    var moduleData = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var info = new SimplisityInfo(moduleData.Record);
    // NOTE: xPath for module settings MUST use "genxml/settings/*"
}

<div class="w3-row">
    <div class="w3-third w3-padding">
        <label>Image Width (px)</label>
        @TextBox(info, "genxml/settings/width", "class='w3-input w3-border'", "640")
    </div>
    <div class="w3-third w3-padding">
        <label>Image Height (px)</label>
        @TextBox(info, "genxml/settings/height", "class='w3-input w3-border'", "400")
    </div>
    <div class="w3-third w3-padding">
        <label>CSS Class</label>
        @TextBox(info, "genxml/settings/cssclass", "class='w3-input w3-border'", "")
    </div>
    <div class="w3-row w3-padding">
        <label>Top Padding</label>
        @TextBox(info, "genxml/settings/toppadding", "class='w3-input' min='0' max='320' step='8'", "8", false, 0, "", "range")
    </div>
    <div class="w3-row w3-padding">
        <label>Bottom Padding</label>
        @TextBox(info, "genxml/settings/bottompadding", "class='w3-input' min='0' max='320' step='8'", "8", false, 0, "", "range")
    </div>
</div>
```

### `1.0\dep\<themenamelowercase>.dep`

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
            <url>{appthemefolder}/css/<ThemeName>.css</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
    </deps>
    <searchindex list="true">
        <genxml>
            <searchtitle>genxml/lang/genxml/textbox/title</searchtitle>
            <searchdescription>genxml/lang/genxml/textbox/summary</searchdescription>
            <searchbody>genxml/lang/genxml/textbox/summary</searchbody>
        </genxml>
    </searchindex>
</genxml>
```

### `1.0\css\<ThemeName>.css`

Generate CSS rules that match the user's design request. Use W3.CSS utility classes in the templates where possible; only add custom CSS for anything W3.CSS does not provide.

---

# SYSTEM: RocketDirectory (also RocketBlog, RocketNews, RocketEvents)

## When to Use

Use when the user wants a full content directory, catalogue, blog, or news feed with:
- A **list** of articles (paged)
- A **detail** view for single articles
- **Category** navigation
- **Tag** or **property/filter** panels
- Admin CRUD for articles, categories, and properties

## Per-System Values

Replace the placeholder values in every file according to the target system:

| System | `@inherits` class | `systemkey` | Article queryparam | Category queryparam |
|---|---|---|---|---|
| RocketDirectory | `RocketDirectoryAPI.Components.RocketDirectoryAPITokens` | `rocketdirectoryapi` | `articleid` | `catid` |
| RocketBlog | `RocketDirectoryAPI.Components.RocketDirectoryAPITokens` | `rocketblogapi` | `blogid` | `blogcatid` |
| RocketNews | `RocketDirectoryAPI.Components.RocketDirectoryAPITokens` | `rocketnewsapi` | `newsid` | `newscatid` |
| RocketEvents | `RocketEventsAPI.Components.RocketEventsAPITokens` | `rocketeventsapi` | `eventid` | *(none)* |

For **RocketEvents**, `@using RocketEventsAPI.Components;` must be added. For all others use `@using RocketDirectoryAPI.Components;`.

## Template Header (all templates)

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->
```

> For RocketEvents replace both `RocketDirectoryAPI.Components.RocketDirectoryAPITokens` and `@using RocketDirectoryAPI.Components;` with `RocketEventsAPI.Components.RocketEventsAPITokens` and `@using RocketEventsAPI.Components;`.

## Available Model Objects

| Object | How to use |
|---|---|
| `articleData` | Single article — populated only when an article id is in the URL. `null` on list pages. |
| `articleDataList` | The paged article result set. Call `articleDataList.GetArticleList()` to iterate. |
| `categoryData` | Currently active category. Check `categoryData.Exists` before using. |
| `categoryDataList` | Full category tree. |
| `propertyData` | A single property/tag. |
| `propertyDataList` | All properties/tags. |
| `moduleData` | Module settings — `.GetSetting("key")`, `.GetSettingInt("key")`, `.GetSettingBool("key")` |
| `moduleSettings` | Alias for `moduleData` |
| `catalogSettings` | Portal-wide catalogue config (page size, sort, etc.) |
| `sessionParams` | Session, culture, paging |
| `portalData` | Portal info |
| `info` | Raw XML node for `articleData` |
| `infoempty` | Empty `SimplisityInfo` — use in add/review forms |
| `appTheme` | Current view AppTheme |
| `appThemeDirectory` | Directory shared templates (`[INJECT:appthemedirectory,...]`) |
| `appThemeDirectoryDefault` | Default directory templates (`[INJECT:appthemedirectorydefault,...]`) |
| `appThemeSystem` | System shared templates (`[INJECT:appthemesystem,...]`) |

## Required Files

### `versioncontrol.xml`
```xml
<genxml>
    <rocketversion>1.0.0</rocketversion>
</genxml>
```

### `1.0\default\View.cshtml`

Switches between list and detail based on whether `articleData` is populated.

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->
@{
    var hasSidebar = moduleData.GetSettingBool("showcategories")
                  || moduleData.GetSettingBool("showtags")
                  || moduleData.GetSettingBool("showfilters");
    var mainCol = hasSidebar ? "w3-col l9 m8 s12 w3-margin-bottom"
                             : "w3-col l12 m12 s12 w3-margin-bottom";
}

<div class="w3-content" style="max-width:1400px;">
    <div class="w3-row-padding">
        <div class="@mainCol">
            @if (articleData != null)
            {
                <text>[INJECT:apptheme,ArticleDetail.cshtml]</text>
            }
            else
            {
                <text>[INJECT:apptheme,ArticleList.cshtml]</text>
            }
        </div>
        @if (hasSidebar)
        {
            <div class="w3-col l3 m4 s12 w3-margin-bottom">
                @if (moduleData.GetSettingBool("showcategories"))
                {
                    [INJECT:apptheme,Categories.cshtml]
                }
                @if (moduleData.GetSettingBool("showtags"))
                {
                    [INJECT:apptheme,Tags.cshtml]
                }
                @if (moduleData.GetSettingBool("showfilters"))
                {
                    [INJECT:apptheme,Filters.cshtml]
                }
            </div>
        }
    </div>
</div>

<div class="simplisity_loader" style="display:none;"><span class="simplisity_loader_inner"></span></div>
<script>
    $(document).ready(function () { w3shared_pageLoad('w3-theme-l3'); });
</script>
```

### `1.0\default\ArticleList.cshtml`

Renders the paged article list. Build the card/row/grid HTML to match the user's request.

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->
@{
    var articleList = articleDataList.GetArticleList();
    var width = moduleData.GetSettingInt("width");
    var imageHeight = moduleData.GetSettingInt("height");
}

[INJECT:appthemedirectorydefault,SearchBanner.cshtml]

@if (articleList.Count > 0)
{
    <div class="w3-row" style="display:flex;flex-wrap:wrap;">
        @foreach (var article in articleList)
        {
            var info = article.Info;
            var articleImage = article.GetImage(0);
            var detailUrl = DetailUrl(moduleData.DetailPageTabId(), article, categoryData,
                                     new string[] { "page", sessionParams.Page.ToString() });
            <div class="w3-col l4 m6 s12 w3-margin-bottom" style="display:flex;padding:8px;box-sizing:border-box;">
                <div class="w3-card w3-white w3-round-large w3-border w3-hover-shadow" style="width:100%;overflow:hidden;">
                    <a href="@detailUrl" onclick="$('.simplisity_loader').show();">
                        @if (articleImage.RelPath != "" && moduleData.GetSettingBool("showimage"))
                        {
                            <img src="@ImageUrl(articleImage.RelPath, width, imageHeight)"
                                 alt="@article.Name"
                                 style="width:100%;height:@(imageHeight)px;object-fit:cover;" />
                        }
                    </a>
                    <div class="w3-container w3-padding">
                        [INJECT:apptheme,LayoutText.cshtml]
                    </div>
                </div>
            </div>
        }
    </div>

    <div class="w3-center w3-padding-16">
        [INJECT:appthemedirectorydefault,articlePaging.cshtml]
    </div>
}
else
{
    <div class="w3-card w3-white w3-round-large w3-border w3-padding w3-center">
        <div class="w3-large w3-margin-top"><b>@ResourceKey("DNNrocket.notfound")</b></div>
    </div>
}
```

### `1.0\default\LayoutText.cshtml`

Text content inside each list card. Adapt fields to match the user's request.

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
<!--inject-->
@{
    var localInfo = articleData.Info;
    var name = articleData.Name;
    var summary = localInfo.GetXmlProperty("genxml/lang/genxml/textbox/summary");
}
<h3 class="w3-margin-bottom">@name</h3>
<p class="w3-text-grey w3-small">@summary</p>
```

### `1.0\default\ArticleDetail.cshtml`

Single article detail view. Build the full-page detail HTML to match the user's request.

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->
@{
    var articleImage = articleData.GetImage(0);
    var name = articleData.Name;
    var body = info.GetXmlProperty("genxml/lang/genxml/textbox/body");
    var listRtnUrl = ReturnUrl(moduleData.DetailPageTabId(), categoryData);
}

<div class="w3-card w3-white w3-round-large w3-border w3-padding w3-margin-bottom">
    @if (articleImage.RelPath != "")
    {
        <img src="@ImageUrl(articleImage.RelPath, moduleData.GetSettingInt("width"), 400)"
             alt="@name" class="w3-image w3-margin-bottom" style="width:100%;" />
    }
    <h1>@name</h1>
    <div class="w3-padding">@Raw(body)</div>
    <div class="w3-margin-top">
        <a href="@listRtnUrl" class="w3-button w3-theme-l5 w3-round-xlarge">
            @ButtonIconText(ButtonTypes.back)
        </a>
    </div>
</div>
```

### `1.0\default\AdminList.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

<div class="w3-animate-opacity w3-card w3-padding w3-margin w3-white">

    [INJECT:appthemedirectory,AdminSearch.cshtml]

    <div id="datalistsection" class="w3-padding">
        <table class="w3-table w3-bordered w3-hoverable">
            <thead>
                <tr>
                    <th style="width:54px;"></th>
                    <th>@ResourceKey("DNNrocket.name")</th>
                </tr>
            </thead>
            @foreach (ArticleLimpet article in articleDataList.GetArticleList())
            {
                <tr class="simplisity_click"
                    s-cmd="articleadmin_editarticle"
                    s-fields='{"articleid":"@article.ArticleId","track":"true"}'
                    style="cursor:pointer;">
                    <td>
                        <img src="@ImageUrl(article.GetImage(0).RelPath, 48, 48)"
                             style="height:48px;width:48px;" class="w3-round" />
                    </td>
                    <td><b>@article.Name</b></td>
                </tr>
            }
        </table>
        @RenderTemplate("AdminPaging.cshtml", appThemeDirectory, Model, true)
    </div>
</div>
```

### `1.0\default\AdminDetail.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

<div id="articleeditcontent" class="w3-animate-opacity w3-card w3-padding w3-margin w3-white">

    [INJECT:appthemedirectory,editbuttonbar.cshtml]

    <div class="w3-row">
        <div class="w3-twothird w3-padding-small">
            [INJECT:appthemesystem,AdminGeneralBlock.cshtml]
            [INJECT:appthemedirectory,ArticleModelsBlock.cshtml]
        </div>
        <div class="w3-third w3-padding-small">
            [INJECT:appthemedirectory,ArticleCategoryListBlock.cshtml]
            [INJECT:appthemedirectory,ArticlePropertyListBlock.cshtml]
            [INJECT:appthemedirectory,ArticleimagesBlock.cshtml]
            [INJECT:appthemedirectory,ArticleDocumentsBlock.cshtml]
            [INJECT:appthemedirectory,ArticleLinksBlock.cshtml]
        </div>
    </div>

</div>
```

### `1.0\default\AdminExtra.cshtml`

Custom admin fields specific to this AppTheme. Called by `ArticleModelsBlock.cshtml`.

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
<!--inject-->

<div class="w3-row w3-padding">
    <label>Summary</label>
    @EditFlag(sessionParams)
    @TextArea(info, "genxml/lang/genxml/textbox/summary",
        "class='w3-input w3-border w3-margin-bottom' autocomplete='off' rows='3'", "")
</div>

<div class="w3-row w3-padding">
    <label>Body</label>
    @EditFlag(sessionParams)
    @Editor(info, "genxml/lang/genxml/textbox/body", Model, 0, "")
</div>
```

> Add, remove, or rename fields to match the user's request. Only put fields here that are NOT already in `AdminGeneralBlock.cshtml` (which provides name, ref, published date, hide/show).

### `1.0\default\AdminHeader.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
<!-- Placeholder template so the system does not show an error on admin. -->
<!--inject-->
```

### `1.0\default\Categories.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

[INJECT:appthemeshared,categories.cshtml]
```

### `1.0\default\Tags.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

[INJECT:appthemeshared,tags.cshtml]
```

### `1.0\default\Filters.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

[INJECT:appthemeshared,filters.cshtml]
```

### `1.0\default\ArticleSat.cshtml`

Satellite template — displays article data without loading it (loaded via AJAX by another module).

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

@if (articleData != null)
{
    <div class="w3-padding">
        <h3>@articleData.Name</h3>
    </div>
}
```

### `1.0\default\ThemeSettings.cshtml`

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using Simplisity;
@using RocketDirectoryAPI.Components;
@using DNNrocketAPI.Components;
@{
    var moduleData = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var info = new SimplisityInfo(moduleData.Record);
}

<div class="w3-row">
    <div class="w3-third w3-padding">
        <label>Image Width (px)</label>
        @TextBox(info, "genxml/settings/width", "class='w3-input w3-border'", "640")
    </div>
    <div class="w3-third w3-padding">
        <label>Image Height (px)</label>
        @TextBox(info, "genxml/settings/height", "class='w3-input w3-border'", "200")
    </div>
    <div class="w3-third w3-padding">
        <label>Show Image</label>
        @CheckBox(info, "genxml/settings/showimage", "", "class='w3-check'", true)
    </div>
</div>

@if (moduleData.DisplayTemplate == "view.cshtml")
{
    <div class="w3-row">
        <div class="w3-third w3-padding">
            @CheckBox(info, "genxml/settings/showcategories", "Show Categories", "class='w3-check'")
        </div>
        <div class="w3-third w3-padding">
            @CheckBox(info, "genxml/settings/showtags", "Show Tags", "class='w3-check'")
        </div>
        <div class="w3-third w3-padding">
            @CheckBox(info, "genxml/settings/showfilters", "Show Filters", "class='w3-check'")
        </div>
    </div>
}

<div class="w3-row w3-padding">
    <label>Top Padding</label>
    @TextBox(info, "genxml/settings/toppadding", "class='w3-input' min='0' max='320' step='8'", "8", false, 0, "", "range")
</div>
<div class="w3-row w3-padding">
    <label>Bottom Padding</label>
    @TextBox(info, "genxml/settings/bottompadding", "class='w3-input' min='0' max='320' step='8'", "8", false, 0, "", "range")
</div>
```

### `1.0\dep\<themenamelowercase>.dep`

Replace `SYSTEMKEY`, `ARTICLEIDPARAM`, and `CATIDPARAM` with the values from the per-system table above.

```xml
<genxml>
    <deps list="true">
        <genxml>
            <ctrltype>css</ctrltype>
            <url>/DesktopModules/DNNrocket/css/w3.css</url>
            <ecofriendly>true</ecofriendly>
            <ignoreonskin>rocketw3</ignoreonskin>
        </genxml>
        <genxml>
            <ctrltype>css</ctrltype>
            <url>/DesktopModules/DNNrocket/Simplisity/css/simplisity.css</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
        <genxml>
            <ctrltype>js</ctrltype>
            <url>/DesktopModules/DNNrocket/Simplisity/js/simplisity.js</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
        <genxml>
            <ctrltype>js</ctrltype>
            <url>{appthemeshareddirectory}/js/w3shared.js</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
        <genxml>
            <ctrltype>css</ctrltype>
            <url>{appthemeshareddirectory}/css/w3shared.css</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
        <genxml>
            <ctrltype>css</ctrltype>
            <url>{appthemefolder}/css/<ThemeName>.css</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
        <genxml>
            <ctrltype>js</ctrltype>
            <url>{jquery}</url>
            <ecofriendly>true</ecofriendly>
        </genxml>
    </deps>
    <moduletemplates list="true">
        <genxml>
            <file>view.cshtml</file>
            <name>Full View</name>
            <cmd>listdetail</cmd>
        </genxml>
        <genxml>
            <file>Categories.cshtml</file>
            <name>Category Menu</name>
            <cmd>catmenu</cmd>
        </genxml>
        <genxml>
            <file>ArticleSat.cshtml</file>
            <name>Satellite</name>
            <cmd>satellite</cmd>
        </genxml>
    </moduletemplates>
    <adminpanelinterfacekeys list="true">
        <genxml>
            <interfacekey>articleadmin</interfacekey>
            <show>true</show>
        </genxml>
        <genxml>
            <interfacekey>categoryadmin</interfacekey>
            <show>true</show>
        </genxml>
        <genxml>
            <interfacekey>propertyadmin</interfacekey>
            <show>true</show>
        </genxml>
        <genxml>
            <interfacekey>settingsadmin</interfacekey>
            <show>true</show>
        </genxml>
        <genxml>
            <interfacekey>rocketdirectoryadmin</interfacekey>
            <show>true</show>
        </genxml>
    </adminpanelinterfacekeys>
    <queryparams list="true">
        <genxml>
            <queryparam>CATIDPARAM</queryparam>
            <tablename>rocketdirectoryapi</tablename>
            <systemkey>SYSTEMKEY</systemkey>
            <datatype>category</datatype>
        </genxml>
        <genxml>
            <queryparam>ARTICLEIDPARAM</queryparam>
            <tablename>rocketdirectoryapi</tablename>
            <systemkey>SYSTEMKEY</systemkey>
            <datatype>article</datatype>
        </genxml>
    </queryparams>
    <menuprovider>
        <genxml>
            <assembly>RocketDirectoryAPI</assembly>
            <namespaceclass>RocketDirectoryAPI.Components.MenuDirectory</namespaceclass>
            <systemkey>SYSTEMKEY</systemkey>
        </genxml>
    </menuprovider>
</genxml>
```

### `1.0\css\<ThemeName>.css`

Generate CSS to match the user's design. Use W3.CSS utility classes in templates first; only add custom CSS here for anything W3.CSS does not cover.

---

## Step 4 — Field Naming Conventions

Use these xPath conventions consistently:

| Field type | xPath |
|---|---|
| Localised text (title, name, summary, body) | `genxml/lang/genxml/textbox/fieldname` |
| Non-localised text (ref, price, date) | `genxml/textbox/fieldname` |
| Checkbox | `genxml/checkbox/fieldname` |
| Select / dropdown | `genxml/select/fieldname` |
| Module setting | `genxml/settings/fieldname` |

Read values in view templates:

```cshtml
var title = info.GetXmlProperty("genxml/lang/genxml/textbox/title");
var price = info.GetXmlPropertyDecimal("genxml/textbox/price");
var isHidden = info.GetXmlPropertyBool("genxml/checkbox/hidden");
```

Read module settings:

```cshtml
var showImage = moduleData.GetSettingBool("showimage");
var width = moduleData.GetSettingInt("width");
var cssClass = moduleData.GetSetting("cssclass");
```

---

## Step 5 — Common Helper Tokens

These tokens are available in all templates and should be used where appropriate:

| Token | Purpose |
|---|---|
| `@ImageUrl(relPath, width, height)` | Returns a resized image URL |
| `@Raw(htmlString)` | Outputs HTML without encoding (for rich-text fields) |
| `@ResourceKey("key")` | Localised string from .resx |
| `@EditFlag(sessionParams)` | Multi-language flag icon (admin only) |
| `@DeepL("field", "xpath")` | DeepL translation button (admin only) |
| `@ChatGPT("field")` | ChatGPT assist button (admin only) |
| `@DetailUrl(tabId, article, category, extras)` | SEO-friendly article detail URL |
| `@ReturnUrl(tabId, category)` | Back-to-list URL from detail page |
| `@ButtonIconText(ButtonTypes.back)` | Back button with icon |
| `@DateOf(info, xpath, culture, format)` | Formatted date from XML field |
| `@StylePadding()` | CSS `padding` string from `toppadding`/`bottompadding` module settings |

---

## Step 6 — Mandatory Rules

The AI MUST follow these rules for every generated AppTheme:

1. **Every template file MUST start with the correct `@inherits` line** for the target system.
2. **`@AssignDataModel(Model)` MUST be called** in every template that accesses model objects.
3. **`<!--inject-->` MUST be present** in every template that will be used as an injected sub-template.
4. **`@RowKey(rowData)` MUST be the first output line in `AdminRow.cshtml`** for RocketContent themes.
5. **Folder names MUST NOT start with a number.**
6. **Module settings xPaths MUST use `genxml/settings/`** in `ThemeSettings.cshtml`.
7. **The `.dep` file MUST include the correct `systemkey` and `queryparam` values** for the target system.
8. **`versioncontrol.xml` MUST be present** in the root of the AppTheme folder.
9. **Do not load w3.css if the skin already provides it** — use the `<condition>` or `<ignoreonskin>` guard in the dep file.
10. **Do not hardcode portal IDs or module IDs** in templates — always use tokens and model objects.



