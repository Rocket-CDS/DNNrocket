# RocketContent Module

## Overview

RocketContent is a powerful and highly flexible general-purpose content management module. It is designed to handle any type of structured content that doesn't fit into more specialized modules like RocketBlog or RocketEvents.

Think of it as a custom content type builder. It is the ideal tool for managing single-section content or lists of items such as:
-   Testimonials
-   Team Members
-   Portfolio Items
-   FAQ Sections
-   Client Logos
-   Product Features

The module allows you to define a data structure via an AppTheme and then render that data in any way you can imagine on the front-end.

---

## Architecture

Like all Rocket Modules, RocketContent is split into two distinct projects to maintain a clean separation of concerns.

1.  **`RocketContentAPI` (.NET Standard):**
    *   This is the "brain" of the module. It contains all the core business logic, services, and data models for creating, retrieving, and versioning content.
    *   Because it's a .NET Standard library, it has no knowledge of DNN and communicates exclusively with the `DNNrocketAPI` bridge.

2.  **`RocketContentMod` (.NET Framework):**
    *   This is the lightweight "control" component that lives inside DNN. It contains several specialized `.ascx` user controls that provide the administrative interface for the module.
    *   These controls act as entry points for different administrative functions, with the actual UI being rendered by dedicated AppThemes.

---

## Administrative Interface

The administrative functions of RocketContent are handled by a set of distinct `.ascx` controls, each with a specific purpose.

### `View` - The Public View
-   **Purpose:** This is the default control that renders the public-facing output of the module.
-   **Function:** Its only job is to load the currently selected AppTheme and render its `View.cshtml` template, passing in the content data.

### `Edit` - Data Entry & Content Management
-   **Purpose:** This is the primary interface for content editors to add, edit, and manage the module's data.
-   **Function:** It renders the AppTheme's `Edit.cshtml` template. This template contains the form fields (textboxes, image uploaders, etc.) that define the data structure for the content. All data creation and modification happens here.

### `Settings` - Module Configuration
-   **Purpose:** This control manages the configuration settings for a specific module instance, which are separate from the content itself.
-   **Function:** It renders the AppTheme's `Settings.cshtml` template. The settings defined here are options that can be used to alter the behavior or display of the `View.cshtml` template.
-   **Example:** A setting might be "Number of Items to Display" or "Sort Order". The `View.cshtml` can then read this setting (`@Model.GetSetting("itemcount")`) and adjust its output accordingly.

### `AppTheme` - In-line Template Editing
-   **Purpose:** Provides a powerful, in-line code editor for developers and administrators to modify the AppTheme files directly from the browser.
-   **Function:** This control allows for direct editing of the AppTheme's `.cshtml`, `.css`, `.js`, and `.dep` files. When a file is edited, it is saved at the appropriate override level (Portal or Module), leaving the base System theme untouched. This is the primary tool for bespoke customizations.

### `RecycleBin` - Data Versioning and Restore
-   **Purpose:** Provides a safety net by giving access to the historical versions of the module's data.
-   **Function:** Every time content is saved via the `Edit.ascx` interface, the previous version is stored. The `RecycleBin.ascx` control displays a list of these past versions. An administrator can view the old data and choose to restore any previous version, overwriting the current data if a mistake was made.

