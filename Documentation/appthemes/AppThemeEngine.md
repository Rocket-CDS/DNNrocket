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
    *   `Partial/`: A folder for reusable Razor components that are specific to this AppTheme.
    *   `Shared/`: A folder for globally reusable Razor components that can be accessed from *any* AppTheme.

*   **Stylesheet Files (`.css`):** Standard CSS files for styling the templates.

*   **JavaScript Files (`.js`):** Used for client-side interactivity, validation, and making AJAX calls to the `DNNrocketAPI`.

*   **Dependency Files (`.dep`):** A unique and powerful feature for managing dependencies.
    *   A `.dep` file is a simple text file where each line contains a path to a CSS or JS file that the theme depends on (e.g., a jQuery plugin, a third-party library, or a shared stylesheet).
    *   When a template is rendered, the system reads its corresponding `.dep` file (e.g., `View.cshtml.dep`) and automatically injects the necessary `<link>` and `<script>` tags into the page's header.
    *   The system is smart and prevents the same dependency from being loaded more than once, even if multiple modules on the same page request it.

---

## The `[INJECT]` Token — Pre-Compilation Template Merging

The `[INJECT]` token is a powerful mechanism for composing templates. Unlike a runtime include, it works as a **pre-compilation merge**: the content of one template is physically spliced into the parent template's source code *before* the Razor engine compiles and executes anything. The result is a single, unified template that the Razor engine sees and compiles as one unit.

### Syntax

```
[INJECT:<dataObjectKey>,<templateName>]
```

| Part | Description |
|---|---|
| `<dataObjectKey>` | The key name of an `AppThemeBase` data object registered in the template's data context (e.g., `appthemesystem`). |
| `<templateName>` | The filename of the template to merge in (e.g., `Popups.cshtml`). |

**Example:**

```cshtml
<div class="w3-container">
    @* ... main template content ... *@
</div>

[INJECT:appthemesystem,Popups.cshtml]
```

### How It Works

Before the Razor engine compiles a template, the system scans the raw template string for every `[INJECT:...]` token and processes them in order:

1.  The `<dataObjectKey>` is used to look up an `AppThemeBase` object from the template's data context.
2.  The target template file is retrieved using `appTheme.GetTemplate(templateName)`. This call **fully respects the cascading file system**, so portal-level or module-level overrides of the injected template are honoured automatically.
3.  The Razor header (the `@inherits`, `@using`, `@model`, and any other directives at the top of the file) is stripped from the injected template, because those declarations already belong to the parent.
4.  The stripped content replaces the `[INJECT:...]` token in the parent template source.
5.  The combined source is then compiled and executed as a single Razor template.

Because the merge happens before compilation, the injected content shares the exact same model, data objects, and helper methods as the parent template. There is no secondary render pass and no additional compilation cost.

### The `<!--inject-->` Marker

To give precise control over the strip point, a template that is designed to be injected should contain the HTML comment `<!--inject-->`. Everything above this marker (the Razor header directives) is discarded; everything from the marker onward is included.

```cshtml
@inherits RocketPortal.Components.RocketPortalTokens<Simplisity.SimplisityRazor>
@using Simplisity;
@using DNNrocketAPI.Components;

@AddProcessData("resourcepath", "/DesktopModules/DNNrocket/...")

<!--inject-->

<div id="messagepopup" class="w3-modal">
    ...
</div>
```

If the `<!--inject-->` marker is absent, the system falls back to starting from the first `<div` in the file.

### Nested and Recursive Injection

Injected templates can themselves contain `[INJECT:...]` tokens. The system processes them recursively, so deeply composed templates are fully supported. To prevent infinite loops, the system tracks which templates have already been merged in the current chain. If a circular reference is detected, it stops and renders a red error message in place of the token.

### Use in Email Templates

The `[INJECT]` token works in email templates as well. The system registers a `appthemesystem` data object in the email rendering context for exactly this purpose, so email templates can pull in shared popup or layout fragments from the system AppTheme in the same way as page templates.

---

## `@RenderTemplate` — Runtime Template Rendering

`@RenderTemplate` is the alternative approach to template composition. Instead of merging source code before compilation, it renders a separate template at **runtime** as an independent Razor compilation. The partial template is a fully self-contained `.cshtml` file with its own `@inherits` and `@using` declarations.

### Usage

The most common form passes an AppTheme object so the cascading file system is respected when locating the template:

```cshtml
@RenderTemplate("LanguageEdit.cshtml", appThemeTools, Model)
```

A path-based overload is available when the template lives in a specific system folder rather than a module's AppTheme:

```cshtml
@RenderTemplate("Admin_SystemDetailData.cshtml", "\\DesktopModules\\DNNrocket\\SystemData", "config-w3", Model, "1.0", true)
```

`Model` (a `SimplisityRazor` instance) is passed explicitly to the partial, so it has access to exactly the same data objects and settings as the parent.

### Independent Compilation

Because each partial is compiled as a separate unit by the Razor engine, the partial template must be a valid, standalone `.cshtml` file. It does **not** need the `<!--inject-->` marker. On first load, each distinct partial incurs its own Razor JIT compile step. Subsequent requests use the cached compiled assembly, so the overhead only occurs once per application lifetime.

### When to Use `@RenderTemplate`

*   The partial is a natural standalone template with its own `@inherits` line and helper calls.
*   The partial is large or complex enough that having it as a separate file significantly improves readability.
*   The composition point is inside a Razor `@{ }` block or conditional branch where a method call reads more clearly than an injected block.
*   The slight first-load compile cost is acceptable in exchange for simpler authoring.

---

## Choosing Between `[INJECT]` and `@RenderTemplate`

| | `[INJECT]` | `@RenderTemplate` |
|---|---|---|
| **When it runs** | Before Razor compilation (string merge) | At render time (runtime call) |
| **Compilation** | One compile for the merged template | Separate compile per partial |
| **First-load cost** | Lower — one compilation unit | Higher — one extra compile per partial |
| **Partial needs `@inherits`** | No — header is stripped | Yes — must be a valid standalone file |
| **`<!--inject-->` marker needed** | Yes (recommended) | No |
| **Accesses parent variables** | Yes — same compiled scope | Via `Model` only |
| **Circular reference detection** | Built-in | Not applicable |
| **Cascading file system** | Yes | Yes (when AppTheme object is passed) |
| **Best for** | Shared popups, dialogs, large reusable blocks | Clean standalone partials, conditional includes |

In practice, `[INJECT]` is preferred for large shared fragments such as popup modals that every admin template needs, while `@RenderTemplate` is preferred when composing a view from logically distinct, independently maintained pieces.

---

## AppThemes as Reusable Functionality

Because an AppTheme bundles all the necessary display logic and client-side code, it represents a complete, portable piece of functionality.

For example, the RocketBlog module might use different AppThemes to provide:
-   A "List/Detail" theme for the main blog.
-   An "Admin Dashboard" theme for managing posts.
-   An "Archive View" theme for displaying posts by month.

A developer can switch between these functionalities simply by changing the selected AppTheme in the module's settings, without ever touching the module's core .NET Standard code. This makes AppThemes the primary mechanism for linking the system's back-end capabilities to a tangible front-end user interface.

---
