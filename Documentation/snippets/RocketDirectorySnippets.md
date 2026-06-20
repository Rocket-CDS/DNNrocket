# RocketDirectoryAPI Razor Snippets

Snippets for building AppTheme templates for **RocketDirectoryAPI**.  
Copy and paste into your own `AppThemes` folder to get started.

---

## Folder Structure

An AppTheme for RocketDirectoryAPI lives in a folder named:

```
rocketdirectoryapi.{YourThemeName}/
  1.0/
    default/
      ListView.cshtml       ← article list (front-end)
      DetailView.cshtml     ← single article detail (front-end)
      ThemeSettings.cshtml  ← module-level settings UI
      AdminDetail.cshtml    ← admin edit form for an article
    css/
    js/
    dep/
      {YourThemeName}.dep   ← dependency/install file
    resx/
      {YourThemeName}.resx  ← resource strings
  img/                      ← thumbnail shown in theme picker
```

> RocketDirectoryAPI is different from RocketContentAPI: articles are full database records (not rows in a module). The module has separate list and detail pages.

---

## 1. Example Empty ListView.cshtml

The article list, typically rendered into a container via AJAX.

```
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
<div class="w3-section">

@foreach (var articleData in articleDataList.GetArticleList())
{
    if (!articleData.IsHidden)
    {
        <div class="w3-card w3-margin-bottom">
            <a href="@DetailUrl(moduleData.DetailPageTabId(), articleData)">
                <h3>@articleData.Name</h3>
            </a>
            <p>@articleData.Summary</p>
        </div>
    }
}

</div>

@* Paging *@
@if (articleDataList.RecordCount > sessionParams.PageSize)
{
    @RenderTemplate("ArticlePaging.cshtml", moduleData.ModuleRef, appThemeDefault, Model, true)
}
```

---

## 2. Example Empty DetailView.cshtml

The single article detail page.

```
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
@if (articleData != null && articleData.Exists)
{
    <div class="w3-section">

        <h1>@articleData.Name</h1>
        <p>@articleData.Summary</p>

        @if (articleData.GetImage(0).RelPath != "")
        {
            <img src="@articleData.GetImage(0).RelPath" alt="@articleData.GetImage(0).Alt" class="w3-image" />
        }

        <div>@Raw(articleData.RichText)</div>

    </div>
}
```

---

## 3. Reading Article Data

`articleData` is typed — use its properties directly or fall back to `info.GetXmlProperty`.

```
@{
    var info = articleData.Info;
}

@* typed shortcut properties *@
<h1>@articleData.Name</h1>
<p>@articleData.Summary</p>
<div>@Raw(articleData.RichText)</div>
<span>@articleData.PublishedDate.ToString("d MMM yyyy")</span>
<span>@articleData.Ref</span>

@* raw XML for custom fields *@
<p>@info.GetXmlProperty("genxml/textbox/customfield")</p>
<p>@info.GetXmlProperty("genxml/lang/genxml/textbox/customlangfield")</p>
@{ var isActive = info.GetXmlPropertyBool("genxml/checkbox/active"); }
@{ var qty      = info.GetXmlPropertyInt("genxml/textbox/qty"); }
```

---

## 4. Article Images

```
@* first image only *@
@{
    var img = articleData.GetImage(0);
}
@if (img.RelPath != "")
{
    <img src="@img.RelPath" alt="@img.Alt" class="w3-image" />
}

@* all images *@
@foreach (var img in articleData.GetImages())
{
    if (img.RelPath != "")
    {
        <img src="@img.RelPath" alt="@img.Alt" class="w3-image" />
    }
}
```

---

## 5. Article Documents and Links

```
@* documents *@
@if (articleData.GetDocList().Count > 0)
{
    <ul>
    @foreach (var doc in articleData.GetDocs())
    {
        <li><a href="@doc.RelPath" target="_blank">@doc.FileName</a></li>
    }
    </ul>
}

@* links *@
@if (articleData.GetLinkList().Count > 0)
{
    @foreach (var lnk in articleData.Getlinks())
    {
        <a href="@lnk.Url" class="w3-button w3-theme">@lnk.Name</a>
    }
}
```

---

## 6. Models (Price Variants)

```
@foreach (var model in articleData.GetModels())
{
    <div>
        <span>@model.Name</span>
        <span>@model.Ref</span>
        <span>@model.BestPriceDisplay</span>
    </div>
}

@* or just show the article best price range *@
<span>@articleData.BestPriceDisplay</span>
<span>@articleData.BestMinimumDisplay – @articleData.BestMaximumDisplay</span>
```

---

## 7. Categories

```
@* categories the current article belongs to *@
@foreach (var catData in articleData.GetCategories())
{
    <a href="@ListUrl(moduleData.ListPageTabId(), catData)">@catData.Name</a>
}

@* full category list (navigation/filter use) *@
@foreach (var catData in categoryDataList.GetCategoryList())
{
    if (!catData.IsHidden)
    {
        <a href="@ListUrl(moduleData.ListPageTabId(), catData)">@catData.Name</a>
    }
}

@* check if article is in a specific category *@
@if (articleData.IsInCategory(categoryData.CategoryId))
{
    <span>@categoryData.Name</span>
}
```

---

## 8. Properties and Tags

Properties are global taxonomy items (tags, attributes, filters).

```
@* tags the article has assigned *@
@foreach (var prop in articleData.GetProperties())
{
    <span class="w3-tag">@prop.Name</span>
}

@* properties by group *@
@foreach (var prop in articleData.GetProperties("colour"))
{
    <span>@prop.Name</span>
}

@* test if article has a specific property by ref *@
@if (articleData.HasProperty("featured"))
{
    <span class="w3-badge w3-theme">Featured</span>
}

@* clickable tag buttons with AJAX reload *@
@TagJsApiCall(moduleData, "#articlelistcontainer", sessionParams)
@foreach (var prop in propertyDataList.GetPropertyList())
{
    @TagButton(prop.PropertyId, prop.Name, sessionParams)
}
@TagButtonClear("Clear", sessionParams)
```

---

## 9. Reviews

```
@{ var reviews = articleData.GetReviews(); }
@if (reviews.Count > 0)
{
    <div>
        <span>@articleData.ReviewScore / 5 (@articleData.ReviewCount reviews)</span>
        @foreach (var review in reviews)
        {
            <blockquote>
                <p>@review.Comment</p>
                <footer>@review.Name – @review.ReviewDate.ToString("d MMM yyyy")</footer>
            </blockquote>
        }
    </div>
}
```

---

## 10. URL Helpers

```
@* link to detail page for an article *@
<a href="@DetailUrl(moduleData.DetailPageTabId(), articleData)">@articleData.Name</a>

@* link to detail page scoped to a category *@
<a href="@DetailUrl(moduleData.DetailPageTabId(), articleData, categoryData)">@articleData.Name</a>

@* link to list page *@
<a href="@ListUrl(moduleData.ListPageTabId())">All items</a>

@* link to list page filtered by category *@
<a href="@ListUrl(moduleData.ListPageTabId(), categoryData)">@categoryData.Name</a>
```

---

## 11. Property Filters (AJAX)

Checkboxes that refresh the list without a full page reload.

```
@* put JS call helper near the list container *@
@FilterJsApiCall(moduleData, sessionParams)

@* render filter checkboxes per property group *@
@foreach (var group in moduleData.GetPropertyModuleGroups(catalogSettings))
{
    <div class="w3-margin-bottom">
        <strong>@group.Value</strong>
        @foreach (var prop in propertyDataList.GetPropertyList(group.Key))
        {
            <label>
                @FilterCheckBox(prop.PropertyId.ToString(), prop.Name, "#articlelistcontainer", articleData.HasProperty(prop.PropertyId))
            </label>
        }
    </div>
}
@FilterClearButton("Clear filters", "#articlelistcontainer")
```

---

## 12. Reading Module Settings

```
@{
    var cssClass  = moduleData.GetSetting("cssclass");
    var imageSize = moduleData.GetSettingInt("imageresize");
    var itemLimit = moduleData.GetSettingInt("itemlimit");
    var defaultCatId = moduleData.DefaultCategoryId;
    var listTabId    = moduleData.ListPageTabId();
    var detailTabId  = moduleData.DetailPageTabId();
}
<div class="w3-section @cssClass">
    @* content *@
</div>
```

---

## 13. Example ThemeSettings.cshtml

All field xPaths **must** use the `genxml/settings/` prefix.

```
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@{
    var info = moduleDataInfo;
    AddProcessDataResx(appTheme, true);
}
<div class="w3-row">

    <div class="w3-third w3-padding">
        <label>CSS Class</label>
        @TextBox(info, "genxml/settings/cssclass", "class='w3-input w3-border'", "")
    </div>

    <div class="w3-third w3-padding">
        <label>Image Resize (px)</label>
        @TextBox(info, "genxml/settings/imageresize", "class='w3-input w3-border'", "1024")
    </div>

    <div class="w3-third w3-padding">
        <label>Items per page</label>
        @TextBox(info, "genxml/settings/pagesize", "class='w3-input w3-border'", "12")
    </div>

    <div class="w3-third w3-padding">
        <label>Columns</label>
        @DropDownList(info, "genxml/settings/columns", "1:1 Column,2:2 Columns,3:3 Columns,4:4 Columns", "class='w3-input w3-border'", "3")
    </div>

    <div class="w3-third w3-padding">
        <label>Background Color</label>
        @DropDownList(info, "genxml/settings/backgroundcolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
    </div>

    <div class="w3-third w3-padding">
        <label>Show Filters</label>
        @CheckBox(info, "genxml/settings/showfilters", "Enable property filters", "class='w3-check'")
    </div>

</div>
```

---

## 14. Example AdminDetail.cshtml

The edit form for a single article in the admin panel.

```
@inherits RocketDirectoryAPI.Components.RocketDirectoryAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@{
    var info = articleData.Info;
}

<div class="w3-row">

    <div class="w3-col w3-padding" style="width:100%">
        <label>Name</label>
        @TextBox(info, "genxml/lang/genxml/textbox/articlename", "id='articlename' class='w3-input w3-border' autocomplete='off'", "", true, 0)
    </div>

    <div class="w3-col w3-padding" style="width:100%">
        <label>Summary</label>
        @TextArea(info, "genxml/lang/genxml/textbox/articlesummary", "class='w3-input w3-border' rows='4'", "", true, 0)
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Ref</label>
        @TextBox(info, "genxml/textbox/articleref", "class='w3-input w3-border'", "")
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Hidden</label>
        @CheckBox(info, "genxml/checkbox/hidden", "Hide this item", "class='w3-check'")
    </div>

</div>
```

---

## Quick Reference

| What | Code |
|---|---|
| Article name (lang) | `articleData.Name` or `info.GetXmlProperty("genxml/lang/genxml/textbox/articlename")` |
| Article summary (lang) | `articleData.Summary` |
| Article rich text (lang) | `@Raw(articleData.RichText)` |
| Article ref | `articleData.Ref` |
| Article published date | `articleData.PublishedDate` |
| Article is hidden | `articleData.IsHidden` |
| Article first image | `articleData.GetImage(0).RelPath` |
| Article all images | `articleData.GetImages()` |
| Article docs | `articleData.GetDocs()` |
| Article links | `articleData.Getlinks()` |
| Article categories | `articleData.GetCategories()` |
| Article properties | `articleData.GetProperties()` |
| Article properties by group | `articleData.GetProperties("groupref")` |
| Test property on article | `articleData.HasProperty("ref")` |
| Article models | `articleData.GetModels()` |
| Best price display | `articleData.BestPriceDisplay` |
| Article reviews | `articleData.GetReviews()` |
| Review count / score | `articleData.ReviewCount` / `articleData.ReviewScore` |
| Article list | `articleDataList.GetArticleList()` |
| Article list (rows) | `articleDataList.GetArticleRows(3)` |
| Record count | `articleDataList.RecordCount` |
| Category name | `categoryData.Name` |
| Category image | `categoryData.GetImage(0).RelPath` |
| Category children | `categoryData.GetDirectChildren()` |
| Full category list | `categoryDataList.GetCategoryList()` |
| Detail URL | `@DetailUrl(moduleData.DetailPageTabId(), articleData)` |
| List URL | `@ListUrl(moduleData.ListPageTabId(), categoryData)` |
| Module setting (string) | `moduleData.GetSetting("fieldname")` |
| Module setting (int) | `moduleData.GetSettingInt("fieldname")` |
| Module default category | `moduleData.DefaultCategoryId` |
| Filter checkboxes | `@FilterCheckBox(id, name, "#container", selected)` |
| Filter JS call | `@FilterJsApiCall(moduleData, sessionParams)` |
| Tag button | `@TagButton(propertyId, name, sessionParams)` |
| Tag JS call | `@TagJsApiCall(moduleData, "#container", sessionParams)` |
| Render paging | `@RenderTemplate("ArticlePaging.cshtml", moduleData.ModuleRef, appThemeDefault, Model, true)` |
