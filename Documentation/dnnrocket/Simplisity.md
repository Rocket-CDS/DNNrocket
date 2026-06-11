# Simplisity Engine

## Overview

Simplisity is a versatile utility library that acts as the backbone for data handling, persistence, and UI templating across the entire Rocket-CDS ecosystem. It provides a standardized way for both client-side and server-side code to manage data and for Razor templates to generate interactive controls.

Its core responsibilities can be broken down into two main areas:
1.  **Data Abstraction and Persistence**: Providing a universal format for data transfer and storage.
2.  **Razor Templating and Tokenization**: Offering a base engine for rendering dynamic, server-aware UI components from Razor templates.

---

## 1. Data Abstraction & Persistence

At its heart, Simplisity provides a flexible data structure that decouples modules from rigid, predefined models.

### SimplisityInfo & SimplisityRecord

-   **`SimplisityRecord`**: A generic, property-bag-style object that can hold any piece of data as a key-value pair. It is roughly equivalent to a single row in a database table or a single data entity. It can be serialized to and from XML, making it an ideal format for persistence.

-   **`SimplisityInfo`**: A container object that holds metadata and a list of `SimplisityRecord` objects. It represents a complete data payload, including information about the data type, version, and other system-level properties. This is the primary object used for transferring data between the `DNNrocketAPI` and the modules.

### How It's Used

-   **Data Transfer**: When a module requests data, the `DNNrocketAPI` populates a `SimplisityInfo` object and sends it to the module. The module can then easily iterate through the records and access data by key name, without needing to know the underlying database schema.
-   **Data Persistence**: `SimplisityRecord` objects are often saved directly to the database as XML. This "schema-less" approach provides immense flexibility, allowing data structures to evolve without requiring database migrations.
-   **Client-Server Communication**: When data is sent from the client to the server via AJAX, it is often packaged in a format that can be easily deserialized into a `SimplisityInfo` or `SimplisityRecord` object on the server.

---

## 2. Razor Templating and Tokens

Simplisity includes a powerful Razor templating engine that is used to render the front-end user interface for all Rocket Modules.

### RazorModuleControlBase

This is the base class that all Rocket Module Razor views (`.cshtml`) inherit from. It is the cornerstone of the templating system, providing the view with access to essential data and helper functions, often referred to as "tokens."

### Tokens: The Building Blocks of the UI

"Tokens" are not a specific syntax but rather a concept referring to the helper methods and properties available within a Razor view that inherits from `RazorModuleControlBase`. These tokens are used to generate HTML controls that are pre-wired to communicate with the server.

They act as shortcuts for building a consistent and interactive UI.

**Common Token Examples:**

-   **`@Model.Get("fieldname")`**: A token to retrieve a value from the current data model.
-   **`@Model.TextBoxFor("fieldname")`**: A token that generates a complete HTML `<input type="text">` element. This generated control automatically includes the necessary `id`, `name`, and `value` attributes, and is often wired with data attributes for client-side validation and AJAX communication.
-   **`@Model.AjaxButton("Save", "MyController.SaveData")`**: A token that generates an HTML `<button>` that, when clicked, automatically triggers an AJAX call to the specified server-side action (`MyController.SaveData`) within the `DNNrocketAPI`.

### How It Works

1.  The `DNNrocketAPI` initiates the rendering of a Razor view for a module.
2.  It passes a data model (typically a `SimplisityInfo` object) to the view.
3.  The view, inheriting from `RazorModuleControlBase`, has access to this model and all the built-in token helpers.
4.  The developer uses these tokens (`@Model.TextBoxFor()`, `@Model.AjaxButton()`, etc.) to quickly build the UI.
5.  Simplisity's Razor engine processes the view, and the tokens generate the final HTML, CSS, and JavaScript hooks.
6.  The resulting HTML is a fully functional control panel that knows how to communicate back to the `DNNrocketAPI` server-side endpoints, creating a seamless loop between the front-end and back-end.

By combining a flexible data layer with a tokenized Razor engine, Simplisity provides a rapid and robust development experience for building complex, data-driven modules in DNN.

