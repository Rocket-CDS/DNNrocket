# RocketEvents Module

## Overview

RocketEvents is a specialized content module for creating and managing a calendar of events. While it appears to the user as a standalone "Events" module, it is architecturally a sophisticated **wrapper** around the core `RocketDirectory` engine.

This wrapper approach allows for multiple, independent directories to exist on a single DNN portal, each with its own data and theming, while reusing the powerful filtering and data management capabilities of RocketDirectory.

---

## Architecture: The Wrapper Model

The most important concept to understand is that RocketEvents **delegates** its core functionality to RocketDirectory.

1.  **`RocketEventsAPI` (.NET Standard):**
    *   This project is often very lightweight. Its primary purpose is to provide a unique identity for the "Events" module type.
    *   It can also contain any specialized business logic required for an event that `RocketDirectoryAPI` doesn't provide, such as handling start/end dates, recurring events, or location data.

2.  **`RocketEventsMod` (.NET Framework):**
    *   This project contains the module definition that makes "RocketEvents" appear as an installable module in DNN.
    *   Crucially, it **does not contain its own administrative controls** like `Edit.ascx` or `Settings.ascx`. Instead, the module definition is configured to point to the controls within `RocketDirectoryMod`.

This means when an administrator clicks "Edit" on a RocketEvents module, they are actually using the `Edit.ascx` control from the RocketDirectory module. The system knows to save the data in a way that is unique to the RocketEvents module instance, even though the UI is shared.

---

## How It Works

1.  **Separate Data Storage:** Even though it uses RocketDirectory's controls, the data for RocketEvents is stored separately. The `DNNrocketAPI` uses the module's type (`RocketEvents`) to ensure data from an event does not mix with data from a blog or a standard directory.

2.  **Dedicated AppThemes:** RocketEvents has its own AppThemes. A theme designed for RocketEvents will have templates (`Edit.cshtml`, `View.cshtml`) with fields and layouts specific to an event, such as "EventTitle", "StartDate", "EndDate", "Location", and "Description".

3.  **Specialized Logic:** The `RocketEventsAPI` can add extra functionality on top of the base directory features. For example, it can include logic to only show upcoming events, sort events by date, or provide an iCal export. The `RocketDirectoryAPI` would not have this date-specific awareness.

## Example Use Case

-   You add the **RocketEvents** module to a page to show a calendar of upcoming company events.
-   You add the **RocketDirectory** module to another page to show a list of office locations.
-   Both modules use the same underlying `RocketDirectory` engine and administrative controls.
-   However, the RocketEvents module uses an "Events" AppTheme, so its edit form asks for "Start Date" and "End Date". Its view template might display a calendar.
-   The RocketDirectory module uses a "Locations" AppTheme, and its edit form asks for "Address" and "Phone Number". Its view template might display a map.
-   The data for each is stored independently, and they can be themed and managed as completely separate entities, providing maximum code reuse and flexibility.