
# AssignDataModel
**Description**: Assigns and prepares the data model for Razor event templates. It calls the base class's AssignDataModel, sets up date parameters for calendar views, and populates several event lists (next events, past events, monthly events, etc.) into the SimplisityRazor model.
**Signature**
```csharp
public new string AssignDataModel(SimplisityRazor sModel)
```
**Example**
```csharp
@{ AssignDataModel(Model); }
```
***
# RssEventUrl
**Description**: Generates a URL for an RSS feed of events for a specific month and year. The URL includes parameters for the command, month, year, and a SQL index for sorting by event start date.
**Signature**
```csharp
public IEncodedString RssEventUrl(int portalId, string cmd, int monthDate, int yearDate)
```
**Example**
```csharp
@RssEventUrl(0, "eventsfeed", 12, 2023)
```
***
# monthStartDate
**Description**: The start date of the current month being viewed.
**Signature**
```csharp
public DateTime monthStartDate;
```
***
# monthEndDate
**Description**: The end date of the current month being viewed.
**Signature**
```csharp
public DateTime monthEndDate;
```
***
# calMonthStartDate
**Description**: The start date of the calendar month being displayed.
**Signature**
```csharp
public DateTime calMonthStartDate;
```
***
# articleEventStartDate
**Description**: The start date of the event for the current article.
**Signature**
```csharp
public DateTime articleEventStartDate;
```
***
# articleEventEndDate
**Description**: The end date of the event for the current article.
**Signature**
```csharp
public DateTime articleEventEndDate;
```
***
# listUrlParams
**Description**: An array of URL parameters for the event list.
**Signature**
```csharp
public string[] listUrlParams;
```
