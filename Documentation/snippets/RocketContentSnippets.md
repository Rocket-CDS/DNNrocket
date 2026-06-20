# RocketContentAPI Razor Snippets

Snippets for building AppTheme templates for **RocketContentAPI**.  
Copy and paste into your own `AppThemes` folder to get started.

---

## Folder Structure

An AppTheme for RocketContentAPI lives in a folder named:

```
rocketcontentapi.{YourThemeName}/
  1.0/
    default/
      View.cshtml          ← front-end display
      ThemeSettings.cshtml ← module-level settings UI
      AdminDetail.cshtml   ← admin edit form for each content row
      AdminRow.cshtml      ← (optional) admin list row summary
    css/
    js/
    dep/
      {YourThemeName}.dep  ← dependency/install file
    resx/
      {YourThemeName}.resx ← resource strings
  img/                     ← thumbnail shown in theme picker
```

---

## 1. Example Empty View.cshtml

The bare-bones starting point for any front-end template.

```razor
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
<div class="w3-section">
@foreach (var articleRowData in articleData.GetRows())
{
    Model.SetDataObject("articlerow", articleRowData);
    var info = articleRowData.Info;

    if (!articleRowData.IsHidden)
    {
        <div class="w3-row">
            @* render your row content here *@
        </div>
    }
}
</div>
```

> `<!--inject-->` marks the point where that the code above is ignored when using hte [INJECT] token and the code beloew is added to the template.  
> `AssignDataModel(Model)` wires up all the typed properties (`articleData`, `moduleData`, etc.).

---

## 2. Reading Row Text Fields

Inside the row loop, `info` is the `SimplisityInfo` for that row.

```razor
@* plain text field *@
<p>@info.GetXmlProperty("genxml/textbox/title")</p>

@* multi-language text field *@
<p>@info.GetXmlProperty("genxml/lang/genxml/textbox/summary")</p>

@* integer field *@
@{ var count = info.GetXmlPropertyInt("genxml/textbox/count"); }

@* boolean (checkbox) field *@
@{ var isFeatured = info.GetXmlPropertyBool("genxml/checkbox/featured"); }

@* shorthand helpers on ArticleRowLimpet *@
<p>@articleRowData.Get("genxml/lang/genxml/textbox/title")</p>
@{ var qty = articleRowData.GetInt("genxml/textbox/qty"); }
@{ var active = articleRowData.GetBool("genxml/checkbox/active"); }
```

---

## 3. Reading Module Settings

Module-level settings (set in `ThemeSettings.cshtml`) are read via `moduleData`.

```razor
@{
    var cssClass  = moduleData.GetSetting("cssclass");
    var imageSize = moduleData.GetSettingInt("imageresize");
    var showTitle = moduleData.GetSettingBool("showtitle");
    var padding   = StylePadding();   @* builds inline padding CSS from top/bottom/left/right settings *@
}
<div class="w3-section @cssClass" style="@padding">
    @* content *@
</div>
```

---

## 4. Row Images

```razor
@foreach (var img in articleRowData.GetImages())
{
    if (img.RelPath != "")
    {
        <img src="@img.RelPath" alt="@img.Alt" class="w3-image" />
    }
}

@* or just the first image *@
@{
    var firstImg = articleRowData.GetImage(0);
}
@if (firstImg != null && firstImg.RelPath != "")
{
    <img src="@firstImg.RelPath" alt="@firstImg.Alt" class="w3-image" />
}
```

---

## 5. Row Documents

```razor
@if (articleRowData.GetDocList().Count > 0)
{
    <ul>
    @foreach (var doc in articleRowData.GetDocs())
    {
        <li><a href="@doc.RelPath" target="_blank">@doc.FileName</a></li>
    }
    </ul>
}
```

---

## 6. Row Links

```razor
@if (articleRowData.Getlinks().Count > 0)
{
    @foreach (var lnk in articleRowData.Getlinks())
    {
        <a href="@lnk.Url" class="w3-button w3-theme">@lnk.Name</a>
    }
}
```

---

## 7. Injecting a Sub-Layout File

Use `[INJECT: apptheme, filename.cshtml]` to pull in a separate layout partial from the same AppTheme folder.  
The injected file shares the same model and all typed properties.

```razor
@foreach (var articleRowData in articleData.GetRows())
{
    Model.SetDataObject("articlerow", articleRowData);
    var info = articleRowData.Info;
    var layout = info.GetXmlPropertyInt("genxml/select/layout");

    if (!articleRowData.IsHidden)
    {
        @if (layout == 0)
        {
            <text>[INJECT: apptheme, LayoutA.cshtml]</text>
        }
        @if (layout == 1)
        {
            <text>[INJECT: apptheme, LayoutB.cshtml]</text>
        }
    }
}
```

---

## 8. Rendering a Shared Sub-Template

Use `RenderTemplate` to call any `.cshtml` from the AppTheme folder passing the current model.

```razor
@RenderTemplate("htmltext.cshtml", moduleData.ModuleRef, appThemeView, Model, true)
```

> Parameters: `(templateName, moduleRef, appTheme, model, cacheEnabled)`

---

## 9. Example ThemeSettings.cshtml

Provides the settings UI shown in the module settings panel.  
All field xPaths **must** use the `genxml/settings/` prefix.

```razor
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@{
    var appThemeAdmin = (DNNrocketAPI.Components.AppThemeLimpet)Model.GetDataObject("appthemeadmin");
    var moduleData    = (RocketContentAPI.Components.ModuleContentLimpet)Model.GetDataObject("modulesettings");
    var info          = new Simplisity.SimplisityInfo(moduleData.Record);
    AddProcessDataResx(appThemeAdmin, true);
    AddProcessData("resourcepath", "/DesktopModules/DNNrocketModules/RocketContentAPI/App_LocalResources/");
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
        <label>Background Color</label>
        @DropDownList(info, "genxml/settings/backgroundcolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
    </div>

    <div class="w3-third w3-padding">
        <label>Alignment</label>
        @DropDownList(info, "genxml/settings/alignment", W3Utils.W3alignment(), "class='w3-input w3-border'", "w3-center")
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

---

## 10. Example AdminDetail.cshtml

The edit form for a single content row in the admin panel.  
`@RowKey(info)` is **required** — it generates the hidden fields that identify the row.

```razor
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@{
    var info = articleRowData.Info;
}
@RowKey(info)

<div class="w3-row">

    <div class="w3-col w3-padding" style="width:100%">
        <label>Title</label>
        @TextBoxRowTitle(info)
        @CheckBoxRowIsHidden(info)
    </div>

    <div class="w3-col w3-padding" style="width:100%">
        <label>Summary</label>
        @TextArea(info, "genxml/lang/genxml/textbox/summary", "class='w3-input w3-border' rows='4'", "")
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Custom Text</label>
        @TextBox(info, "genxml/textbox/customfield", "class='w3-input w3-border'", "")
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Pick a Layout</label>
        @DropDownList(info, "genxml/select/layout", "0:Layout A,1:Layout B,2:Layout C", "class='w3-input w3-border'", "0")
    </div>

</div>
```

---

## 11. Example AdminRow.cshtml

The compact row summary line shown in the admin content list (one line per row).

```razor
@inherits RocketContentAPI.Components.RocketContentAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@{
    var info = articleRowData.Info;
}
<span>@info.GetXmlProperty("genxml/lang/genxml/textbox/title")</span>
```

---

## Quick Reference

| What | Code |
|---|---|
| Row title (lang) | `info.GetXmlProperty("genxml/lang/genxml/textbox/title")` |
| Row textbox | `info.GetXmlProperty("genxml/textbox/fieldname")` |
| Row checkbox | `info.GetXmlPropertyBool("genxml/checkbox/fieldname")` |
| Row integer | `info.GetXmlPropertyInt("genxml/textbox/fieldname")` |
| Row select | `info.GetXmlPropertyInt("genxml/select/fieldname")` |
| Module setting (string) | `moduleData.GetSetting("fieldname")` |
| Module setting (int) | `moduleData.GetSettingInt("fieldname")` |
| Module setting (bool) | `moduleData.GetSettingBool("fieldname")` |
| Images list | `articleRowData.GetImages()` |
| Docs list | `articleRowData.GetDocs()` |
| Links list | `articleRowData.Getlinks()` |
| Is row hidden | `articleRowData.IsHidden` |
| Inline padding style | `StylePadding()` |
| W3 colour list | `W3Utils.W3colors()` |
| W3 alignment list | `W3Utils.W3alignment()` |
| Inject partial | `[INJECT: apptheme, MyPartial.cshtml]` |
| Render sub-template | `@RenderTemplate("name.cshtml", moduleData.ModuleRef, appThemeView, Model, true)` |
| Required row hidden fields | `@RowKey(info)` |
| Title textbox (admin) | `@TextBoxRowTitle(info)` |
| Hide row checkbox (admin) | `@CheckBoxRowIsHidden(info)` |
