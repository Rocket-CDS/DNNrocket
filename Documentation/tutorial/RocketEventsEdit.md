# Using the RocketEvents Module

This guide explains how to set up and manage the RocketEvents module, a complete solution for creating and displaying a calendar of events. As a specialized wrapper for the RocketDirectory system, it provides a familiar interface tailored for time-based entries.

---

## Step 1: Initial Configuration

After adding a new RocketEvents module to a page, it must be configured. The module needs an **AppTheme** to define its data structure (e.g., event date, time, location), layout (list vs. calendar), and editing interface. Click the **Configure** button to begin.

---

## Step 2: Select an AppTheme

On the AppTheme selection screen, you will choose a theme designed for managing events.

1.  Select the **Project**: `AppThemes-W3-CSS`.
2.  Select the **AppTheme**: `rocketeventsapi.Events`. This theme includes the logic for handling event dates, times, and displaying them in a user-friendly format.
3.  Click **Save**.

---

## Step 3: Module Settings

The settings page allows you to control how your events are displayed. It includes standard RocketDirectory settings plus options specific to the Events AppTheme.

### Standard Settings

*   **Disable Cache, ECO Mode, Template, Default Sort, List/Detail Page, API Module Reference:** These function as they do in the standard RocketDirectory module. The "Template" setting is particularly useful here, as an AppTheme might offer both a `ListView.cshtml` and a `CalendarView.cshtml`.

### Events AppTheme Settings

The `rocketeventsapi.Events` theme will add its own crucial settings:

*   **Default View:** Choose whether the module should initially display as a list or a full calendar.
*   **Date & Time Format:** Customize how dates and times are displayed to match your region.
*   **Show Past Events:** A toggle to control whether events that have already occurred should be hidden from the main view.
*   **Calendar Integration:** Options to provide "Add to Calendar" links for iCal, Google Calendar, etc.

---

## Step 4: Editing Data (Creating an Event)

In "Edit" mode, you can create and manage your events. The fields are specifically designed for event management:

*   **Event Title:** The name of the event.
*   **Description:** Full details about the event.
*   **Start Date & Time:** The date and time the event begins.
*   **End Date & Time:** The date and time the event ends. This is optional for single-day events.
*   **Location:** The physical address or venue of the event. This can be linked to a map.
*   **Registration Link:** A URL to an external registration page or a RocketForms module.
*   **Event Image:** A promotional image or banner for the event.

---

## Step 5: The Administration Settings

The underlying RocketDirectory administration panel governs all directory-based systems, including RocketEvents. These settings for **General, Config, Order By, Providers, and Tools** apply globally and function as described in the RocketDirectory tutorial. For example, the default sort order is critical for events, and you would typically set it to `EventDate ASC` in the "Order By" tab to ensure upcoming events appear first.
