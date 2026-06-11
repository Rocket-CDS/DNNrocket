# AppThemes Engine

## Overview

AppThemes are the complete theming and templating engine for all Rocket Modules. They control the entire client-side experience, from the HTML structure and styling to the interactive functionality. An AppTheme is more than just a "skin"; it is a self-contained package of Razor templates, CSS, JavaScript, and dependency definitions that dictates how a module's data is presented and interacted with.

This architecture allows a single module (like RocketBlog) to have multiple, distinct functionalities—such as a blog post list, a detail view, an admin dashboard, or an RSS feed—simply by switching the AppTheme or the specific template being rendered.

---

## The Cascading File System

The power of AppThemes lies in its hierarchical, cascading file system. This allows for a base theme to be customized at different levels without modifying the original files, ensuring that updates to the base theme don't overwrite bespoke work.

The system resolves template files in the following order of priority:

1.  **Module Level (Highest Priority):**
    *   **Location:** A specific subfolder within the Portal Level, typically named after the module's ID (e.g., `/Portals/[PortalID]/DNNrocket/AppThemes/[ThemeName]/[ModuleID]/`).
    *   **Purpose:** This provides the highest level of granularity. Templates placed here will **only** apply to the single module instance on the page that is configured to use this path. This is useful for one-off customizations that should not affect any other module on the portal.

2.  **Portal Level (Mid-level Priority):**
    *   **Location:** `/Portals/[PortalID]/DNNrocket/AppThemes/`
    *   **Purpose:** This is where you place customized templates for a specific portal (website). If a file exists here, it will be used for all modules on that portal, unless a Module Level override exists. This is the standard place for bespoke project work.

3.  **System Level (Base/Default - Lowest Priority):**
    *   **Location:** `/DesktopModules/DNNrocket/AppThemes/`
    *   **Purpose:** This folder contains the default AppThemes downloaded from GitHub. These files should **never be edited directly**, as they serve as the fallback for all portals and will be overwritten during updates.

This cascading logic provides incredible flexibility. You can have a base theme, override it for an entire site at the Portal Level, and then make a specific, targeted change for a single module on a single page at the Module Level.

---

## Anatomy of an AppTheme

An AppTheme is a collection of files and folders that work together to create the user experience.

*   **Razor Templates (`.cshtml`):** The core files that generate HTML.
    *   `View.cshtml`: The primary template used for the module's main public view.
    *   `Settings.cshtml`: The template used for the module's settings interface.
    *   `Partial/`: A folder for reusable Razor components that are specific to this AppTheme. They can be rendered inside other templates using `@Model.RenderTemplate("mypartial.cshtml")`.
    *   `Shared/`: A folder for globally reusable Razor components that can be accessed from *any* AppTheme.

*   **Stylesheet Files (`.css`):** Standard CSS files for styling the templates.

*   **JavaScript Files (`.js`):** Used for client-side interactivity, validation, and making AJAX calls to the `DNNrocketAPI`.

*   **Dependency Files (`.dep`):** A unique and powerful feature for managing dependencies.
    *   A `.dep` file is a simple text file where each line contains a path to a CSS or JS file that the theme depends on (e.g., a jQuery plugin, a third-party library, or a shared stylesheet).
    *   When a template is rendered, the system reads its corresponding `.dep` file (e.g., `View.cshtml.dep`) and automatically injects the necessary `<link>` and `<script>` tags into the page's header.
    *   The system is smart and prevents the same dependency from being loaded more than once, even if multiple modules on the same page request it.

---

## AppThemes as Reusable Functionality

Because an AppTheme bundles all the necessary display logic and client-side code, it represents a complete, portable piece of functionality.

For example, the RocketBlog module might use different AppThemes to provide:
-   A "List/Detail" theme for the main blog.
-   An "Admin Dashboard" theme for managing posts.
-   An "Archive View" theme for displaying posts by month.

A developer can switch between these functionalities simply by changing the selected AppTheme in the module's settings, without ever touching the module's core .NET Standard code. This makes AppThemes the primary mechanism for linking the system's back-end capabilities to a tangible front-end user interface.

---
