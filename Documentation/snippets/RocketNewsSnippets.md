# RocketNewsAPI Razor Snippets

Snippets for building AppTheme templates for **RocketNewsAPI**.  
Copy and paste into your own `AppThemes` folder to get started.

> **RocketNewsAPI is built on RocketDirectoryAPI.** Its `StartConnect` proxies all commands directly to `RocketDirectoryAPI.API.StartConnect` with `systemKey = "rocketnewsapi"`. The data model is identical. However, `RocketNewsAPITokens` does **not** extend `RocketDirectoryAPITokens`, so typed shortcut properties (`articleData`, `articleDataList`, `moduleData`, etc.) are not available by default — they must be obtained via `Model.GetDataObject()`.

---

## Folder Structure

```
rocketnewsapi.{YourThemeName}/
  1.0/
    default/
      ListView.cshtml       ← news article list (front-end)
      DetailView.cshtml     ← single article detail (front-end)
      ThemeSettings.cshtml  ← module-level settings UI
      AdminDetail.cshtml    ← admin edit form for an article
    css/
    js/
    dep/
      {YourThemeName}.dep
    resx/
      {YourThemeName}.resx
  img/
```

---

## 1. Accessing Data in News Templates

Because `RocketNewsAPITokens` does not expose typed properties, you cast data objects from the model manually at the top of each template.

```
@inherits RocketNewsAPI.Components.RocketNewsAPITokens<Simplisity.SimplisityRazor>
@using RocketDirectoryAPI.Components
@{
    var articleDataList = (ArticleLimpetList)Model.GetDataObject("articlelist");
    var articleData     = (ArticleLimpet)Model.GetDataObject("articledata");
    var moduleData      = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var categoryDataList = (CategoryLimpetList)Model.GetDataObject("categorylist");
    var categoryData    = (CategoryLimpet)Model.GetDataObject("categorydata");
    var sessionParams   = Model.SessionParamsData;
    if (sessionParams == null) sessionParams = new Simplisity.SessionParams(new Simplisity.SimplisityInfo());
}
```

> Put this block at the top of every template. From here, use the same patterns as RocketDirectoryAPI.

---

## 2. Example Empty ListView.cshtml

```
@inherits RocketNewsAPI.Components.RocketNewsAPITokens<Simplisity.SimplisityRazor>
@using RocketDirectoryAPI.Components
@{
    var articleDataList = (ArticleLimpetList)Model.GetDataObject("articlelist");
    var moduleData      = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var appThemeDefault = (DNNrocketAPI.Components.AppThemeLimpet)Model.GetDataObject("appthemedefault");
    var sessionParams   = Model.SessionParamsData;
    if (sessionParams == null) sessionParams = new Simplisity.SessionParams(new Simplisity.SimplisityInfo());
}
<!--inject-->
<div class="w3-section">

@foreach (var article in articleDataList.GetArticleList())
{
    if (!article.IsHidden)
    {
        <div class="w3-card w3-margin-bottom">
            <a href="@DetailUrl(moduleData.DetailPageTabId(), article)">
                <h3>@article.Name</h3>
            </a>
            <p class="w3-opacity">@article.PublishedDate.ToString("d MMM yyyy")</p>
            <p>@article.Summary</p>
        </div>
    }
}

</div>

@if (articleDataList.RecordCount > sessionParams.PageSize)
{
    @RenderTemplate("ArticlePaging.cshtml", moduleData.ModuleRef, appThemeDefault, Model, true)
}
```

---

## 3. Example Empty DetailView.cshtml

```
@inherits RocketNewsAPI.Components.RocketNewsAPITokens<Simplisity.SimplisityRazor>
@using RocketDirectoryAPI.Components
@{
    var articleData = (ArticleLimpet)Model.GetDataObject("articledata");
    var moduleData  = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
}
<!--inject-->
@if (articleData != null && articleData.Exists)
{
    <article class="w3-section">

        <h1>@articleData.Name</h1>
        <p class="w3-opacity">@articleData.PublishedDate.ToString("d MMM yyyy")</p>

        @if (articleData.GetImage(0).RelPath != "")
        {
            <img src="@articleData.GetImage(0).RelPath" alt="@articleData.GetImage(0).Alt" class="w3-image" />
        }

        <div>@Raw(articleData.RichText)</div>

        @foreach (var catData in articleData.GetCategories())
        {
            <a href="@ListUrl(moduleData.ListPageTabId(), catData)" class="w3-tag w3-theme">@catData.Name</a>
        }

    </article>
}
```

---

## 4. Reading Article Data

Once you have `articleData` from `Model.GetDataObject`, use it identically to RocketDirectoryAPI.

```
@{
    var info = articleData.Info;
}

<h1>@articleData.Name</h1>
<p>@articleData.Summary</p>
<div>@Raw(articleData.RichText)</div>
<span>@articleData.PublishedDate.ToString("d MMM yyyy")</span>

@* images *@
@foreach (var img in articleData.GetImages())
{
    if (img.RelPath != "")
    {
        <img src="@img.RelPath" alt="@img.Alt" class="w3-image" />
    }
}

@* custom fields *@
<p>@info.GetXmlProperty("genxml/textbox/customfield")</p>
<p>@info.GetXmlProperty("genxml/lang/genxml/textbox/customlangfield")</p>
```

---

## 5. Category Navigation

```
@{
    var categoryDataList = (RocketDirectoryAPI.Components.CategoryLimpetList)Model.GetDataObject("categorylist");
    var moduleData       = (RocketDirectoryAPI.Components.ModuleContentLimpet)Model.GetDataObject("modulesettings");
}
@foreach (var catData in categoryDataList.GetCategoryList())
{
    if (!catData.IsHidden)
    {
        <a href="@ListUrl(moduleData.ListPageTabId(), catData)" class="w3-bar-item w3-button">
            @catData.Name
        </a>
    }
}
```

---

## 6. Example ThemeSettings.cshtml

```
@inherits RocketNewsAPI.Components.RocketNewsAPITokens<Simplisity.SimplisityRazor>
@using RocketDirectoryAPI.Components
@{
    var moduleData = (ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var appTheme   = (DNNrocketAPI.Components.AppThemeLimpet)Model.GetDataObject("apptheme");
    var info       = new Simplisity.SimplisityInfo(moduleData.Record);
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
        <label>Articles per page</label>
        @TextBox(info, "genxml/settings/pagesize", "class='w3-input w3-border'", "10")
    </div>

    <div class="w3-third w3-padding">
        <label>Background Color</label>
        @DropDownList(info, "genxml/settings/backgroundcolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
    </div>

</div>
```

---

## 7. Example AdminDetail.cshtml

```
@inherits RocketNewsAPI.Components.RocketNewsAPITokens<Simplisity.SimplisityRazor>
@using RocketDirectoryAPI.Components
@{
    var articleData = (ArticleLimpet)Model.GetDataObject("articledata");
    var info        = articleData.Info;
}

<div class="w3-row">

    <div class="w3-col w3-padding" style="width:100%">
        <label>Title</label>
        @TextBox(info, "genxml/lang/genxml/textbox/articlename", "id='articlename' class='w3-input w3-border' autocomplete='off'", "", true, 0)
    </div>

    <div class="w3-col w3-padding" style="width:100%">
        <label>Summary</label>
        @TextArea(info, "genxml/lang/genxml/textbox/articlesummary", "class='w3-input w3-border' rows='4'", "", true, 0)
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Published Date</label>
        @TextBox(info, "genxml/textbox/publisheddate", "class='w3-input w3-border' type='date'", "")
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Hidden</label>
        @CheckBox(info, "genxml/checkbox/hidden", "Hide this article", "class='w3-check'")
    </div>

</div>
```

---

## Quick Reference

| What | Code |
|---|---|
| Inherits line | `@inherits RocketNewsAPI.Components.RocketNewsAPITokens<Simplisity.SimplisityRazor>` |
| Get article list | `(ArticleLimpetList)Model.GetDataObject("articlelist")` |
| Get single article | `(ArticleLimpet)Model.GetDataObject("articledata")` |
| Get module data | `(ModuleContentLimpet)Model.GetDataObject("modulesettings")` |
| Get category list | `(CategoryLimpetList)Model.GetDataObject("categorylist")` |
| Get session params | `Model.SessionParamsData` |
| Article name | `article.Name` |
| Article summary | `article.Summary` |
| Article rich text | `@Raw(article.RichText)` |
| Article published date | `article.PublishedDate` |
| Article is hidden | `article.IsHidden` |
| Article first image | `article.GetImage(0).RelPath` |
| Article list loop | `articleDataList.GetArticleList()` |
| Record count | `articleDataList.RecordCount` |
| Detail URL | `@DetailUrl(moduleData.DetailPageTabId(), article)` |
| List URL by category | `@ListUrl(moduleData.ListPageTabId(), catData)` |
