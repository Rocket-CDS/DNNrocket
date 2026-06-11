# Using the RocketDirectory Module: Product Catalog Example

This guide explains how to set up and manage the RocketDirectory module, a powerful tool for creating and displaying lists of structured data. For this tutorial, we will walk through creating a **product catalog**.

The process begins after the RocketDirectory module has been added to a DNN page.

---

## Step 1: Initial Configuration

When you add a new RocketDirectory module to a page, it will first prompt you to configure it. The module needs to be assigned an **AppTheme** to define its data structure, appearance, and administrative interface. Click the **Configure** button to start.

---

## Step 2: Select an AppTheme

You will be taken to the AppTheme selection screen. The AppTheme you choose determines what kind of directory you will be building.

For this tutorial, we will use the open-source **"AppThemes-W3-CSS"** project, which is downloaded by default during the RocketCDS installation.

1.  Select the **Project**: `AppThemes-W3-CSS`. *Automatically selected if you only have 1 AppTheme project*
2.  Select the **AppTheme**: `Products`. This theme is specifically designed to manage a product catalog.
3.  Click **Save**.  *You will have a set of setting appear that is linked to the `Template` you have select.*

---

## Step 3: Module Settings

The settings page allows you to control the behavior of your product catalog. It includes standard settings common to RocketDirectory and custom settings defined by the "Products" AppTheme.

### Standard Settings

Based on the default settings template, here are the key options you can configure:

*   **Disable Cache:** Check this to turn off caching for the module. This is useful during development but should be disabled in production for optimal performance.
*   **ECO Mode:** An efficiency mode that can improve performance by optimizing how the module renders.
*   **Template:** Selects the specific Razor template (`.cshtml` file) from the AppTheme to use for the public display. The "Products" theme may have different templates for a product list, a featured product grid, etc.
*   **Default Sort:** Sets the default order in which the products will be displayed (e.g., by product name, price).
*   **List Page:** Specifies which DNN page the main product catalog should be displayed on.
*   **Detail Page:** Specifies the DNN page where the detailed view of a single product will be shown.
*   **API Module Reference:** A powerful feature that allows this module to use another RocketDirectory module as its data source. This is perfect for reusing the same product data in different places with different display settings.

### AppTheme Settings

The "Products" AppTheme will add its own set of custom settings, which might include options like currency symbol, image sizes, or filtering options.

After configuring the settings, click **Update** to apply them.

---

## Step 4: Editing Data

In the module's "Edit" mode will show an administration panel, you can add, manage, and organize the individual products, categories, properties, settings and administration settings in your catalog. The fields available for each product (e.g., product name, SKU, price, image, description) are entirely defined by the "Products" AppTheme.

Add the information for each product and click **Save**. The content will then be displayed on the front end according to the layout in your AppTheme's view template.

---

---

## Step 5: The Administration Settings

Beyond the settings for an individual module, RocketDirectory has a powerful system-level administration area. This is typically accessed from the module admin panel. These settings control the fundamental behavior of the entire directory system for the portal, not just a single module instance.

The administration area is organized into several tabs:

### General Tab

This tab contains the core operational settings for the directory system.

*   **AppTheme Project / AppTheme / Version:** This selects the default AppTheme that will be used for the administration interface itself.
*   **Max Articles:** Sets the maximum number of directory items (articles) that can be created for this portal. Defaults to 1000.
*   **Articles Image Limit:** The maximum number of images that can be uploaded for a single directory item. Defaults to 12.
*   **Articles Document Limit:** The maximum number of document attachments for a single directory item. Defaults to 12.
*   **Image Resize:** Sets the maximum width or height (in pixels) to which uploaded images will be resized. This helps to conserve server space and improve performance. Defaults to 1024px.
*   **Email On:** Enables or disables email notifications for certain system events.
*   **Debug Mode:** Activates debug mode, which may display additional diagnostic information. This should be disabled on a live site.
*   **Secure Upload:** Enforces stricter security checks on file uploads.
*   **Security Key:** A unique key used for securing API endpoints and other communications.
*   **Count Downloads:** Enables a counter for any downloadable files linked within directory items.
*   **Checkbox Filter AND:** Changes the logic for filtering. When checked, if a user selects multiple checkbox filters (e.g., "Feature A" and "Feature B"), it will only show items that have **both** features (AND logic). If unchecked, it will show items that have **either** feature (OR logic).

### Config Tab

This tab is for SuperUsers only and controls more advanced, technical aspects of the directory.

*   **Currency Culture Code:** Sets the culture code (e.g., `en-US`, `de-DE`) to be used for formatting currency values.
*   **Canonical Pages (List/Detail/Search):** Defines the primary, canonical URL for the list, detail, and search pages. This is important for SEO to avoid duplicate content penalties.
*   **Search Module ID:** Specifies which module on the search page is responsible for handling search queries.
*   **SQL Filter:** Allows a SuperUser to write a custom SQL `WHERE` clause to apply a permanent filter to the items displayed in the directory, both for public and admin views. This is a powerful tool for creating specialized sub-directories from a master data set.
*   **DNN Search Extra:** Lets you append extra text to the content that is passed to the standard DNN search indexer, allowing you to include keywords or data that might not be in the main display templates.
*   **Generate Link Images:** An option related to how images are handled in specific linking scenarios.

### Order By Tab

This tab allows you to define the custom sorting options that will be available to both administrators and, potentially, end-users.

*   You can add, remove, and reorder various sorting fields. Each entry consists of a **Key** (a short name, often the database field name) and a **Value** (the full SQL `ORDER BY` clause).
*   For example, you could create a sort option with the Key `Price` and the Value `order by articlename.GUIDKey` to allow sorting by price in ascending order.
*   **Admin Order By:** Selects the default sort order used in the administrative back-end.

### Providers (Plugins) Tab

This area manages the active plugins for the RocketDirectory system. Plugins are extensions that can add new functionality, data types, or integration points. Here you can:

*   Activate or deactivate available plugins.
*   Set the sort order to control the display order of plugin-related UI elements.

### Tools Tab

This tab provides access to various maintenance and administrative tools.

*   **Validate and Index:** This crucial tool performs two actions: it validates the integrity of the directory data and then rebuilds the search index. This should be run if you are having issues with search results not being up-to-date.
*   **Scheduler Run Hours:** Sets the interval, in hours, for how often the system's background scheduler task should run. This task might perform maintenance like re-indexing or data cleanup.
*   **Missing Documents:** A utility to scan the directory and find any records that are referencing document files that no longer exist on the server's file system.

---

### A Note on Changing AppThemes

While you can change the AppTheme at any time from the module's settings, exercise caution. Different AppThemes use different data structures. Switching from the "Products" theme to another theme on a module that already contains content could make that data inaccessible. It's always best to be certain before switching themes on a live module.
