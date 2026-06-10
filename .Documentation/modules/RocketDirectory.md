# RocketDirectory Module

## Overview

RocketDirectory is a specialized module designed for creating and managing structured, filterable, and searchable directories of information. While similar to RocketContent in its use of AppThemes to define data structures, RocketDirectory is purpose-built for directory-style listings and includes powerful, out-of-the-box features for filtering and searching that data.

It is the ideal solution for building:
-   Staff or Member Directories
-   Business or Store Locators
-   Product Catalogs
-   Resource Libraries

The key differentiator is its built-in support for query-based filtering directly from URL parameters, making it easy to create dynamic, user-driven directory experiences.

---

## Architecture

RocketDirectory follows the standard two-project architecture of all Rocket Modules, ensuring a clean separation between the core logic and the DNN integration layer.

1.  **`RocketDirectoryAPI` (.NET Standard):**
    *   This project contains the core business logic for the directory module.
    *   It manages the creation, retrieval, and storage of directory entries.
    *   Crucially, it includes the logic for parsing URL query parameters and applying them as filters to the data retrieval process.
    *   As a .NET Standard library, it is completely decoupled from DNN and relies on the `DNNrocketAPI` for all CMS interactions.

2.  **`RocketDirectoryMod` (.NET Framework):**
    *   This is the lightweight DNN integration project.
    *   It contains the standard set of `.ascx` user controls (`View.ascx`, `Edit.ascx`, `Settings.ascx`, etc.) that serve as entry points for the module's administrative and public-facing functions.
    *   These controls initialize the DNNrocket engine, which then renders the appropriate AppTheme templates.

---

## Core Concepts

### Built-in Filtering and Searching

The most important feature of RocketDirectory is its native ability to filter data based on URL query strings. This is handled automatically by the `RocketDirectoryAPI`.

-   **URL-Driven Filters:** When the `View.cshtml` template is rendered, the API automatically inspects the page's URL for any query parameters.
-   **Automatic Data Filtering:** If it finds parameters (e.g., `?category=restaurants&city=london`), it dynamically builds a query to filter the directory's data records. Only records that match the criteria (e.g., where the "category" field is "restaurants" and the "city" field is "london") will be passed to the template.
-   **No Extra Code Required:** This filtering logic is built-in. Developers do not need to write any custom C# code to handle the filtering; they only need to create the front-end controls (like dropdowns or search boxes) that generate the correct URL parameters.

### AppTheme-Defined Data Structure

As with RocketContent, the data structure for a directory is not hard-coded. It is defined entirely by the form fields you place in the AppTheme's `Edit.cshtml` template. This allows RocketDirectory to be used for any type of directory.

-   For a staff directory, `Edit.cshtml` might have fields for "Name", "Department", "Email", and "Photo".
-   For a product catalog, it might have fields for "ProductName", "SKU", "Price", and "Category".

---

## Administrative Interface

RocketDirectory uses the same set of administrative controls as other Rocket Modules:

-   **`View`:** Renders the public directory listing, automatically applying any filters from the URL.
-   **`Edit` & `AdminPanel`:** The data entry interface, defined by the `AdminPanel.cshtml` AppTheme template.
-   **`Settings`:** Manages module-level settings, such as pagination ("Items per Page") or default sort order.
-   **`AppTheme`:** The in-line editor for customizing the directory's templates.
