# RocketDirectory AppTheme

## Overview

RocketDirectory is the full-featured directory/catalogue module system in Rocket. It is the basis for **RocketBlog**, **RocketNews**, **RocketEvents**, and **RocketProducts** — all of which share the same AppTheme structure and token inheritance.

A RocketDirectory AppTheme controls: list and detail views, the admin article/category/property editors, category menus, tag/filter panels, satellite templates, and module theme settings.

---

## Template Header

All RocketDirectory templates (and derived systems) use:

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
<!--inject-->
```

The `@AssignDataModel(Model)` call populates all pre-defined model objects automatically. The two `@using` statements are required for the types used in template code blocks.

---

## Pre-Defined Model Objects

After `@AssignDataModel(Model)`, the following objects are directly available in the template:

| Object | Type | Description |
|---|---|---|
| `articleData` | `ArticleLimpet` | A single article (populated when viewing a detail page or editing an article). `null` in list context. |
| `articleDataList` | `ArticleLimpetList` | The paged article result set. Call `.GetArticleList()` to get the enumerable list of `ArticleLimpet`. |
| `categoryData` | `CategoryLimpet` | The currently active category filter. |
| `categoryDataList` | `CategoryLimpetList` | The full category tree for the system. |
| `propertyData` | `PropertyLimpet` | A single property/tag record. |
| `propertyDataList` | `PropertyLimpetList` | The full property/tag list. |
| `moduleData` | `ModuleContentLimpet` | Module settings and configuration. Use `.GetSetting("key")` or `.GetSettingBool("key")`. |
| `moduleSettings` | `ModuleContentLimpet` | Alias for `moduleData` (legacy compatibility). |
| `portalData` | `PortalLimpet` | Portal-level data (name, culture, paths). |
| `catalogSettings` | `CatalogSettingsLimpet` | System-wide catalogue configuration (page sizes, default sorting, etc.). |
| `sessionParams` | `SessionParams` | Current request session data, culture, page, paging. |
| `userParams` | `UserParams` | Current logged-in user details. |
| `appTheme` | `AppThemeLimpet` | The current view AppTheme. |
| `appThemeDefault` | `AppThemeLimpet` | The fallback/default AppTheme. |
| `appThemeSystem` | `AppThemeSystemLimpet` | The system-level shared template object. |
| `appThemeDirectory` | `AppThemeSystemLimpet` | The directory-system shared templates (used for `[INJECT:appthemedirectory,...]`). |
| `appThemeDirectoryDefault` | `AppThemeLimpet` | The default directory AppTheme. |
| `info` | `SimplisityInfo` | Raw XML data node for the current `articleData` record. |
| `infoempty` | `SimplisityInfo` | Empty `SimplisityInfo` node used in add/review forms. |
| `moduleDataInfo` | `SimplisityInfo` | Raw XML info for the module record. |

### AppTheme Object Keys (for `[INJECT]`)

| Key | Object |
|---|---|
| `apptheme` | `appTheme` (current view theme) |
| `appthemedefault` | `appThemeDefault` |
| `appthemesystem` | `appThemeSystem` (system-level shared templates) |
| `appthemedirectory` | `appThemeDirectory` (directory shared templates) |
| `appthemedirectorydefault` | `appThemeDirectoryDefault` |
| `appthemeview` | Resolved view theme (used in some shared templates) |
| `appthemeshared` | Portal-level shared templates |

---

## Standard Template Set

A typical RocketDirectory AppTheme contains:

| Template | Purpose |
|---|---|
| `View.cshtml` | Main public view: switches between list and detail based on URL params |
| `ListView.cshtml` | Alternative list-only view |
| `ArticleList.cshtml` | Renders the paged article list (injected by `View.cshtml`) |
| `ArticleDetail.cshtml` | Renders a single article detail page (injected by `View.cshtml`) |
| `ArticleSat.cshtml` | Satellite template — displays data without loading it (see cmd `satellite`) |
| `Categories.cshtml` | Category sidebar/menu template |
| `Tags.cshtml` | Tag filter panel |
| `Filters.cshtml` | Property filter panel |
| `AdminDetail.cshtml` | Admin single-article edit form |
| `AdminList.cshtml` | Admin paged article list |
| `AdminHeader.cshtml` | Admin panel header template (placeholder; usually empty) |
| `AdminExtra.cshtml` | Extra admin fields template (optional extension) |
| `ThemeSettings.cshtml` | Per-module display settings panel |
| `LayoutText.cshtml` | Text/content block used within article list/detail cards |
| `Rss.cshtml` | RSS feed template |

---

## View Template Pattern

`View.cshtml` is the entry point. It displays either a detail article or a list depending on whether `articleData` is populated (i.e. whether the URL contains an article `id` param):

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
    var mainCol = hasSidebar ? "w3-col l9 m8 s12" : "w3-col l12";
}

<div class="w3-content" style="max-width:1400px;">
    <div class="w3-row-padding">
        <div class="@mainCol">
            @if (articleData != null)
            {
                <text>[INJECT: apptheme, ArticleDetail.cshtml]</text>
            }
            else
            {
                <text>[INJECT: apptheme, ArticleList.cshtml]</text>
            }
        </div>
        @if (hasSidebar)
        {
            <div class="w3-col l3 m4 s12">
                @if (moduleData.GetSettingBool("showcategories"))
                {
                    [INJECT: apptheme, Categories.cshtml]
                }
                @if (moduleData.GetSettingBool("showtags"))
                {
                    [INJECT: apptheme, Tags.cshtml]
                }
                @if (moduleData.GetSettingBool("showfilters"))
                {
                    [INJECT: apptheme, Filters.cshtml]
                }
            </div>
        }
    </div>
</div>
```

---

## Article List Pattern

```cshtml
@{
    var articleList = articleDataList.GetArticleList();
    var width = moduleData.GetSettingInt("width");
}

@foreach (var article in articleList)
{
    var info = article.Info;
    var articleImage = article.GetImage(0);
    var detailUrl = DetailUrl(moduleData.DetailPageTabId(), article, categoryData,
                              new string[] { "page", sessionParams.Page.ToString() });

    <div class="w3-col l4 m6 s12 w3-margin-bottom">
        <div class="w3-card w3-white w3-round-large">
            <a href="@detailUrl">
                <img src="@ImageUrl(articleImage.RelPath, width, 200)" alt="@article.Name" />
            </a>
            [INJECT:appthemeview, LayoutText.cshtml]
        </div>
    </div>
}

<div class="w3-center">
    [INJECT:appthemedirectorydefault, articlePaging.cshtml]
</div>
```

Key points:
- `articleDataList.GetArticleList()` returns `IEnumerable<ArticleLimpet>`.
- `DetailUrl(tabId, article, categoryData, extras)` builds the SEO-friendly detail URL.
- `[INJECT:appthemedirectorydefault,articlePaging.cshtml]` renders the shared pagination control.
- `[INJECT:appthemedirectorydefault,SearchBanner.cshtml]` adds the search/sort bar above the list.

---

## Admin Templates

### AdminList.cshtml

Renders the paged admin list of articles:

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

[INJECT:appthemedirectory,AdminSearch.cshtml]

<table class="w3-table w3-bordered w3-hoverable">
    @foreach (ArticleLimpet article in articleDataList.GetArticleList())
    {
        <tr class="simplisity_click"
            s-cmd="articleadmin_editarticle"
            s-fields='{"articleid":"@article.ArticleId","track":"true"}'
            style="cursor:pointer;">
            <td><img src="@ImageUrl(article.GetImage(0).RelPath, 48, 48)" class="w3-round" /></td>
            <td><b>@article.Name</b></td>
        </tr>
    }
</table>

@RenderTemplate("AdminPaging.cshtml", appThemeDirectory, Model, true)
```

### AdminDetail.cshtml

The article edit form. Uses `[INJECT:appthemedirectory,...]` calls to compose the form from shared directory-level blocks:

```cshtml
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketDirectoryAPI.Components;
@AssignDataModel(Model)
@AddProcessDataResx(appTheme, true)
<!--inject-->

<div id="articleeditcontent" class="w3-card w3-padding w3-white">

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

> Unlike RocketContent, the RocketDirectory admin detail form is mostly composed from shared `appthemedirectory` blocks. Custom fields specific to your theme go in `AdminExtra.cshtml`, which is injected by `ArticleModelsBlock.cshtml`.

---

## ThemeSettings.cshtml

Module display settings. Settings are read from `moduleData.GetSetting("key")` and saved under the `genxml/settings/` xPath:

```cshtml
@{
    var moduleData = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var info = new SimplisityInfo(moduleData.Record);
}

<div class="w3-third w3-padding">
    <label>Show Images</label>
    @CheckBox(info, "genxml/settings/showimage", "", "class='w3-check'", true)
</div>
<div class="w3-third w3-padding">
    <label>Show Categories</label>
    @CheckBox(info, "genxml/settings/showcategories", "", "class='w3-check'", true)
</div>
```

---

## Dependency File

RocketDirectory AppThemes have richer dependency files than RocketContent because they require the `simplisity.js` AJAX library, shared JS/CSS, paging/filter functionality, and `queryparams` for SEO and detail-URL activation.

Example from `rocketdirectoryapi.Products`:

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
            <file>listview.cshtml</file>
            <name>List View</name>
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
    </adminpanelinterfacekeys>
    <queryparams list="true">
        <genxml>
            <queryparam>productscatid</queryparam>
            <tablename>rocketdirectoryapi</tablename>
            <systemkey>rocketdirectoryapi</systemkey>
            <datatype>category</datatype>
        </genxml>
        <genxml>
            <queryparam>productsid</queryparam>
            <tablename>rocketdirectoryapi</tablename>
            <systemkey>rocketdirectoryapi</systemkey>
            <datatype>article</datatype>
        </genxml>
    </queryparams>
    <menuprovider>
        <genxml>
            <assembly>RocketDirectoryAPI</assembly>
            <namespaceclass>RocketDirectoryAPI.Components.MenuDirectory</namespaceclass>
            <systemkey>rocketdirectoryapi</systemkey>
        </genxml>
    </menuprovider>
</genxml>
```

### queryparams

The `queryparams` section is essential for RocketDirectory AppThemes. It tells the system:

1. Which URL parameter holds a category or article ID.
2. Which system and database table to query when that parameter is present.
3. What type of data it represents (`article` or `category`).

This drives both **SEO metadata injection** (the detail page gets the article's title/description in the page `<head>`) and **detail display activation** (the module switches from list to detail view when the param is in the URL).

Each system derived from RocketDirectory uses its own param names (e.g. `productsid`, `newsid`, `blogid`) to avoid conflicts when multiple directory modules exist on the same page.

### menuprovider

The `menuprovider` section connects the AppTheme's category data to the DNN DDR menu system, allowing categories to appear as DNN navigation items.

---

## XPath Data Convention

| Data type | xPath |
|---|---|
| Plain text field | `genxml/textbox/myfieldname` |
| Localised text field | `genxml/lang/genxml/textbox/myfieldname` |
| Checkbox | `genxml/checkbox/mycheckbox` |
| Select / DropDown | `genxml/select/myselect` |
| Image (first) | `article.GetImage(0)` |
| Published date | `genxml/textbox/publisheddate` |
| Module setting | `genxml/settings/mysetting` |

> **SEO fields:** For SEO to work automatically, use standard field names: `genxml/lang/genxml/textbox/articlename` or `genxml/lang/genxml/textbox/seotitle` for the meta title, and `genxml/lang/genxml/textbox/articlesummary` or `genxml/lang/genxml/textbox/seodescription` for the meta description.

---

## cmd Values

The `cmd` value in `moduletemplates` determines what data is loaded for the module:

| cmd | Behaviour |
|---|---|
| `listdetail` | Loads list data; switches to detail data if an article URL param is present (default) |
| `listonly` | Loads list data only; never switches to detail |
| `detailonly` | Loads a single article only; no list |
| `catmenu` | Loads category data (no article data) |
| `satellite` | No data loaded; the template is responsible for its own AJAX data call |

---

## Directory-Level Shared Templates

The `appthemedirectory` key accesses shared templates provided by the RocketDirectory system. These cover admin CRUD operations and standard UI blocks shared across all directory-based AppThemes:

| Shared Template | Purpose |
|---|---|
| `[INJECT:appthemedirectory,editbuttonbar.cshtml]` | Save/Cancel/Delete button bar for the admin detail form |
| `[INJECT:appthemedirectory,AdminSearch.cshtml]` | Search/filter bar for the admin list |
| `[INJECT:appthemedirectory,ArticleModelsBlock.cshtml]` | Custom field block; includes `AdminExtra.cshtml` from the AppTheme |
| `[INJECT:appthemedirectory,ArticleCategoryListBlock.cshtml]` | Category assignment panel |
| `[INJECT:appthemedirectory,ArticlePropertyListBlock.cshtml]` | Property/tag assignment panel |
| `[INJECT:appthemedirectory,ArticleimagesBlock.cshtml]` | Image management panel |
| `[INJECT:appthemedirectory,ArticleDocumentsBlock.cshtml]` | Document management panel |
| `[INJECT:appthemedirectory,ArticleLinksBlock.cshtml]` | Link management panel |
| `[INJECT:appthemedirectorydefault,articlePaging.cshtml]` | Front-end pagination control |
| `[INJECT:appthemedirectorydefault,SearchBanner.cshtml]` | Front-end search/sort/filter bar |

