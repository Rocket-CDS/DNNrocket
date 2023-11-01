# Snippets - RocketContent
These snippets and examples give help for building AppThemes.    

## Templates
### AdminDetail.cshtml - Single Row
```
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssigDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->

[INJECT:appthemeadmin,AdminRow.cshtml]

```
### AdminRow.cshtml - Single Row
Injected by token: **[INJECT:appthemeadmin,AdminRow.cshtml]**
```
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssigDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->
@RowKey(rowData)

<div id="heading" class='w3-row w3-margin-top sectionname'>
    <div class='w3-col m12 w3-padding' style='min-width:200px;'>
        <label>@ResourceKey("DNNrocket.heading")</label> @EditFlag(sessionParams)
    @TextBox(rowData, "genxml/lang/genxml/textbox/title", " id='title' class='w3-input w3-border' autocomplete='off' ", "", true, 0)
    </div>
</div>
```

### AdminDetail.cshtml - Multiple Rows
```
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssigDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->

<div class="w3-row">
    [INJECT:appthemesystem,ArticleHeader.cshtml]
    <div id="articledetailpanel" class="w3-threequarter">
        [INJECT:appthemeadmin,AdminRow.cshtml]
    </div>
    <div class="w3-quarter">
        [INJECT:appthemesystem,AdminRowSelect.cshtml]
    </div>
</div>
```
### AdminRow.cshtml - Multiple Rows 
Injected by token: **[INJECT:appthemeadmin,AdminRow.cshtml]**
```
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssigDataModel(Model)
@AddProcessDataResx(appThemeView, true)
<!--inject-->
@RowKey(rowData)

<div id="heading" class='w3-row w3-margin-top sectionname'>
    <div class='w3-col m12 w3-padding' style='min-width:200px;'>
        <label>@ResourceKey("DNNrocket.heading")</label> @EditFlag(sessionParams)
    @TextBox(rowData, "genxml/lang/genxml/textbox/title", " id='title' class='w3-input w3-border' autocomplete='off' ", "", true, 0)
    </div>
</div>
```
## Images
### Admin
**Single Image**  *INJECT or Render method.*
```
[INJECT:appthemesystem,ArticleImage.cshtml]
```
```
@RenderTemplate("ArticleImage.cshtml",appThemeSystem, Model, false)
```
**Multiple Images**  *INJECT or Render method.*
```
[INJECT:appthemesystem,ArticleImages.cshtml]
```
```
@RenderTemplate("ArticleImages.cshtml",appThemeSystem, Model, false)
```
**Multiple Images**  *With size*
```
[INJECT:appthemesystem,ArticleImagesSize.cshtml]
```

### View
**Single Image**
```
@{
    var articleImage = articleRowData.GetImage(0);
}
<img src="@ImageUrl(articleImage.RelPath,200,200)" />
```
**Single Image - webp**
```
@{
    var articleImage = articleRowData.GetImage(0);
}
<img src="@ImageUrl(articleImage.RelPathWebp,200,200,"","webp")" />
```
**Multiple Images**
```
@foreach (ArticleImage articleImage in articleRowData.GetImages())
{
    if (articleImage.RelPath != "")
    {
        <img src="@ImageUrl(articleImage.RelPath, articleImage.Width, articleImage.Height)" alt="@(articleImage.Alt)" />
    }
}

```

## Documents
### Admin
**Single Document**
```
[INJECT:appthemesystem,ArticleDocument.cshtml]
```
**Multiple Documents**
```
[INJECT:appthemesystem,ArticleDocuments.cshtml]
```
### View
```
@foreach (ArticleDoc documentData in rowDetail.GetDocs())
{
    if (!documentData.Hidden)
    {
        <div>
            <a href="@(documentData.RelPath)" target="_blank" >@documentData.Name</a>
        </div>
    }
}
```
## Links
### Admin
```
[INJECT:appthemesystem,ArticleLink.cshtml]
```
```
[INJECT:appthemesystem,ArticleLinks.cshtml]
```
### View
```
```

