# RocketEventsAPI Razor Snippets

Snippets for building AppTheme templates for **RocketEventsAPI**.  
Copy and paste into your own `AppThemes` folder to get started.

> **RocketEventsAPI is built on RocketDirectoryAPI.** It uses the same article/category/property structure and URL helpers, plus event-specific date fields and pre-built event lists. Refer to `RocketDirectorySnippets.md` for the full reference — these snippets cover the events-specific patterns.

---

## Folder Structure

```
rocketeventsapi.{YourThemeName}/
  1.0/
    default/
      ListView.cshtml       ← event list (front-end)
      DetailView.cshtml     ← single event detail (front-end)
      ThemeSettings.cshtml  ← module-level settings UI
      AdminDetail.cshtml    ← admin edit form for an event
    css/
    js/
    dep/
      {YourThemeName}.dep
    resx/
      {YourThemeName}.resx
  img/
```

---

## Typed Properties Available in Event Templates

`AssignDataModel` populates these extra properties on top of the standard RocketDirectory ones:

| Property | Type | Description |
|---|---|---|
| `articleEventStartDate` | `DateTime` | Start date of the current detail article |
| `articleEventEndDate` | `DateTime` | End date of the current detail article |
| `monthStartDate` | `DateTime` | First day of the currently viewed month |
| `monthEndDate` | `DateTime` | Last day of the currently viewed month |
| `calMonthStartDate` | `DateTime` | First day of the calendar month (from session) |
| `listUrlParams` | `string[]` | URL params for month/year navigation |

Pre-built event lists are available in the model via `Model.GetDataObject()`:

| Key | Contents |
|---|---|
| `eventnextlist` | `EventListData` — next upcoming events |
| `eventpassedlist` | `EventListData` — past events |
| `eventmonthlist` | `EventListData` — events in the selected month |
| `eventlistbymonth` | `Dictionary<DateTime, List<ArticleLimpet>>` — events grouped by month |
| `eventlistbyday` | `Dictionary<int, List<ArticleLimpet>>` — events grouped by day of the selected month |

---

## Example Empty ListView.cshtml

```
@inherits RocketEventsAPI.Components.RocketEventsAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
@{
    var nextEvents = (RocketEventsAPI.Components.EventListData)Model.GetDataObject("eventnextlist");
}
<div class="w3-section">

    <h2>Upcoming Events</h2>
    @foreach (var ev in nextEvents.EventList)
    {
        if (!ev.IsHidden)
        {
            var startDate = ev.Info.GetXmlPropertyDate("genxml/textbox/eventstartdate");
            var endDate   = ev.Info.GetXmlPropertyDate("genxml/textbox/eventenddate");

            <div class="w3-card w3-margin-bottom">
                <a href="@DetailUrl(moduleData.DetailPageTabId(), ev)">
                    <h3>@ev.Name</h3>
                </a>
                <p>@startDate.ToString("d MMM yyyy") – @endDate.ToString("d MMM yyyy")</p>
                <p>@ev.Summary</p>
            </div>
        }
    }

</div>
```

---

## Example Empty DetailView.cshtml

```
@inherits RocketEventsAPI.Components.RocketEventsAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
<!--inject-->
@if (articleData != null && articleData.Exists)
{
    <article class="w3-section">

        <h1>@articleData.Name</h1>

        <p>
            <strong>Start:</strong> @articleEventStartDate.ToString("d MMM yyyy HH:mm")<br />
            <strong>End:</strong>   @articleEventEndDate.ToString("d MMM yyyy HH:mm")
        </p>

        @if (articleData.GetImage(0).RelPath != "")
        {
            <img src="@articleData.GetImage(0).RelPath" alt="@articleData.GetImage(0).Alt" class="w3-image" />
        }

        <div>@Raw(articleData.RichText)</div>

    </article>
}
```

---

## Event Start / End Dates

Dates are stored in custom xPath fields on the article.

```
@{
    var info      = articleData.Info;
    var startDate = info.GetXmlPropertyDate("genxml/textbox/eventstartdate");
    var endDate   = info.GetXmlPropertyDate("genxml/textbox/eventenddate");
}

@* or use the pre-assigned helpers in AssignDataModel *@
<p>@articleEventStartDate.ToString("d MMM yyyy")</p>
<p>@articleEventEndDate.ToString("d MMM yyyy")</p>

@* format as time too *@
<p>@articleEventStartDate.ToString("d MMM yyyy HH:mm")</p>
```

---

## Next and Past Event Lists

```
@{
    var nextEvents   = (RocketEventsAPI.Components.EventListData)Model.GetDataObject("eventnextlist");
    var pastEvents   = (RocketEventsAPI.Components.EventListData)Model.GetDataObject("eventpassedlist");
    var monthEvents  = (RocketEventsAPI.Components.EventListData)Model.GetDataObject("eventmonthlist");
}

@* upcoming *@
@foreach (var ev in nextEvents.EventList)
{
    <div>@ev.Name — @ev.Info.GetXmlPropertyDate("genxml/textbox/eventstartdate").ToString("d MMM")</div>
}

@* past *@
@foreach (var ev in pastEvents.EventList)
{
    <div>@ev.Name</div>
}

@* paged from a list *@
@foreach (var ev in nextEvents.GetEvents(sessionParams.Page, sessionParams.PageSize))
{
    <div>@ev.Name</div>
}
<p>Total: @nextEvents.RowCount</p>
```

---

## Events Grouped by Month

```
@{
    var byMonth = (System.Collections.Generic.Dictionary<System.DateTime, System.Collections.Generic.List<RocketDirectoryAPI.Components.ArticleLimpet>>)Model.GetDataObject("eventlistbymonth");
}
@foreach (var month in byMonth)
{
    if (month.Value.Count > 0)
    {
        <h3>@month.Key.ToString("MMMM yyyy")</h3>
        @foreach (var ev in month.Value)
        {
            <div>@ev.Name</div>
        }
    }
}
```

---

## Calendar Grid (Events by Day)

```
@{
    var byDay = (System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<RocketDirectoryAPI.Components.ArticleLimpet>>)Model.GetDataObject("eventlistbyday");
}
@for (int d = 1; d <= DateTime.DaysInMonth(calMonthStartDate.Year, calMonthStartDate.Month); d++)
{
    <div class="w3-col" style="width:14.28%">
        <span>@d</span>
        @if (byDay.ContainsKey(d))
        {
            @foreach (var ev in byDay[d])
            {
                <a href="@DetailUrl(moduleData.DetailPageTabId(), ev)" class="w3-tiny">@ev.Name</a>
            }
        }
    </div>
}
```

---

## Month Navigation

```
@{
    var prevMonth = calMonthStartDate.AddMonths(-1);
    var nextMonth = calMonthStartDate.AddMonths(1);
}
<a href="@ListUrl(moduleData.ListPageTabId(), new string[] { "calmonth", prevMonth.Month.ToString(), "calyear", prevMonth.Year.ToString() })">
    &laquo; @prevMonth.ToString("MMMM yyyy")
</a>
<strong>@calMonthStartDate.ToString("MMMM yyyy")</strong>
<a href="@ListUrl(moduleData.ListPageTabId(), new string[] { "calmonth", nextMonth.Month.ToString(), "calyear", nextMonth.Year.ToString() })">
    @nextMonth.ToString("MMMM yyyy") &raquo;
</a>
```

---

## Example ThemeSettings.cshtml

```
@inherits RocketEventsAPI.Components.RocketEventsAPITokens<Simplisity.SimplisityRazor>
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
        <label>Background Color</label>
        @DropDownList(info, "genxml/settings/backgroundcolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
    </div>

</div>
```

---

## Example AdminDetail.cshtml

```
@inherits RocketEventsAPI.Components.RocketEventsAPITokens<Simplisity.SimplisityRazor>
@AssignDataModel(Model)
@{
    var info = articleData.Info;
}

<div class="w3-row">

    <div class="w3-col w3-padding" style="width:100%">
        <label>Event Name</label>
        @TextBox(info, "genxml/lang/genxml/textbox/articlename", "id='articlename' class='w3-input w3-border' autocomplete='off'", "", true, 0)
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Start Date</label>
        @TextBox(info, "genxml/textbox/eventstartdate", "class='w3-input w3-border' type='datetime-local'", "")
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>End Date</label>
        @TextBox(info, "genxml/textbox/eventenddate", "class='w3-input w3-border' type='datetime-local'", "")
    </div>

    <div class="w3-col w3-padding" style="width:100%">
        <label>Summary</label>
        @TextArea(info, "genxml/lang/genxml/textbox/articlesummary", "class='w3-input w3-border' rows='4'", "", true, 0)
    </div>

    <div class="w3-col w3-padding" style="width:50%">
        <label>Hidden</label>
        @CheckBox(info, "genxml/checkbox/hidden", "Hide this event", "class='w3-check'")
    </div>

</div>
```

---

## Quick Reference

| What | Code |
|---|---|
| Inherits line | `@inherits RocketEventsAPI.Components.RocketEventsAPITokens<Simplisity.SimplisityRazor>` |
| Event start date (detail) | `articleEventStartDate` |
| Event end date (detail) | `articleEventEndDate` |
| Event start date (raw) | `info.GetXmlPropertyDate("genxml/textbox/eventstartdate")` |
| Event end date (raw) | `info.GetXmlPropertyDate("genxml/textbox/eventenddate")` |
| Calendar month start | `calMonthStartDate` |
| Next events list | `(EventListData)Model.GetDataObject("eventnextlist")` |
| Past events list | `(EventListData)Model.GetDataObject("eventpassedlist")` |
| Month events list | `(EventListData)Model.GetDataObject("eventmonthlist")` |
| Events by month dict | `Model.GetDataObject("eventlistbymonth")` |
| Events by day dict | `Model.GetDataObject("eventlistbyday")` |
| Paged events | `nextEvents.GetEvents(page, pageSize)` |
| Event count | `nextEvents.RowCount` |
| RSS event URL | `@RssEventUrl(portalData.PortalId, "cmd", month, year)` |
| Detail URL | `@DetailUrl(moduleData.DetailPageTabId(), articleData)` |
| List URL | `@ListUrl(moduleData.ListPageTabId())` |
