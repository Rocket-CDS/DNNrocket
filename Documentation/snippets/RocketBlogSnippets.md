# RocketBlogAPI Razor Snippets

Snippets for building AppTheme templates for **RocketBlogAPI**.  
Copy and paste into your own `AppThemes` folder to get started.

> **RocketBlogAPI is built on RocketDirectoryAPI.** It uses the same data model, the same article/category/property structure, and the same URL helpers. The only differences are the `inherits` class, the system key (`rocketblogapi`), and the folder prefix. Refer to `RocketDirectorySnippets.md` for the full reference — these snippets cover the blog-specific patterns.

---

## Folder Structure

```
rocketblogapi.{YourThemeName}/
  1.0/
    default/
      ListView.cshtml       ← blog post list (front-end)
      DetailView.cshtml     ← single post detail (front-end)
      ThemeSettings.cshtml  ← module-level settings UI
      AdminDetail.cshtml    ← admin edit form for a post
    css/
    js/
    dep/
      {YourThemeName}.dep
    resx/
      {YourThemeName}.resx
  img/
```

---

## Example Empty ListView.cshtml

```
@inherits RocketBlogAPI.Components.RocketBlogAPITokens<Simplisity.SimplisityRazor>
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
            <p class="w3-opacity">@articleData.PublishedDate.ToString("d MMM yyyy")</p>
            <p>@articleData.Summary</p>
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

## Example Empty DetailView.cshtml

```
@inherits RocketBlogAPI.Components.RocketBlogAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
@if (articleData != null && articleData.Exists)
{
    <article class="w3-section">

        <header>
            <h1>@articleData.Name</h1>
            <p class="w3-opacity">
                @articleData.PublishedDate.ToString("d MMM yyyy")
            </p>
        </header>

        @if (articleData.GetImage(0).RelPath != "")
        {
            <img src="@articleData.GetImage(0).RelPath" alt="@articleData.GetImage(0).Alt" class="w3-image" />
        }

        <div>@Raw(articleData.RichText)</div>

        @* categories *@
        @foreach (var catData in articleData.GetCategories())
        {
            <a href="@ListUrl(moduleData.ListPageTabId(), catData)" class="w3-tag w3-theme">@catData.Name</a>
        }

    </article>
}
```

---

## Blog-Specific Article Fields

Blog posts use the same xPath patterns as RocketDirectory articles. Use `articleData.PublishedDate` for the post date.

```
@{
    var info = articleData.Info;
}

<h1>@articleData.Name</h1>
<p>@articleData.Summary</p>
<div>@Raw(articleData.RichText)</div>
<span>@articleData.PublishedDate.ToString("d MMM yyyy")</span>
<span>@articleData.Ref</span>

@* custom fields added via AdminDetail *@
<p>@info.GetXmlProperty("genxml/textbox/customfield")</p>
<p>@info.GetXmlProperty("genxml/lang/genxml/textbox/customlangfield")</p>
```

---

## Category Navigation

```
@* category list for sidebar or menu *@
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

## Example ThemeSettings.cshtml

All field xPaths **must** use the `genxml/settings/` prefix.

```
@inherits RocketBlogAPI.Components.RocketBlogAPITokens<Simplisity.SimplisityRazor>
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
        <label>Posts per page</label>
        @TextBox(info, "genxml/settings/pagesize", "class='w3-input w3-border'", "10")
    </div>

    <div class="w3-third w3-padding">
        <label>Background Color</label>
        @DropDownList(info, "genxml/settings/backgroundcolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
    </div>

</div>
```

---

## Example AdminDetail.cshtml

```
@inherits RocketBlogAPI.Components.RocketBlogAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@{
    var info = articleData.Info;
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
        @CheckBox(info, "genxml/checkbox/hidden", "Hide this post", "class='w3-check'")
    </div>

</div>
```

---

## Quick Reference

| What | Code |
|---|---|
| Inherits line | `@inherits RocketBlogAPI.Components.RocketBlogAPITokens<Simplisity.SimplisityRazor>` |
| Assign data model | `@AssignDataModel(Model)` |
| Post name | `articleData.Name` |
| Post summary | `articleData.Summary` |
| Post rich text | `@Raw(articleData.RichText)` |
| Post published date | `articleData.PublishedDate` |
| Post is hidden | `articleData.IsHidden` |
| Post first image | `articleData.GetImage(0).RelPath` |
| Post all images | `articleData.GetImages()` |
| Post categories | `articleData.GetCategories()` |
| Post properties / tags | `articleData.GetProperties()` |
| Article list | `articleDataList.GetArticleList()` |
| Record count | `articleDataList.RecordCount` |
| Detail URL | `@DetailUrl(moduleData.DetailPageTabId(), articleData)` |
| List URL | `@ListUrl(moduleData.ListPageTabId(), categoryData)` |
| Render paging | `@RenderTemplate("ArticlePaging.cshtml", moduleData.ModuleRef, appThemeDefault, Model, true)` |
