# RocketForms Module

## Overview

RocketForms is a versatile module for creating, managing, and deploying data capture forms within a DNN site. It provides the tools to build everything from simple contact forms to complex, multi-field data collection interfaces.

The module is designed to capture user submissions, store them, and optionally trigger actions such as sending notification emails. It uses the AppTheme system to define the form's fields, layout, and post-submission behavior, making it highly customizable.

---

## Architecture

Following the standard pattern for Rocket Modules, RocketForms separates its core logic from its presentation layer to ensure maintainability and portability.

1.  **`RocketForms` (.NET Standard):**
    *   This project serves as the core engine of the module. It contains all the business logic, data services, and validation mechanisms for handling form structures and submissions.
    *   As a .NET Standard library, it is completely decoupled from the DNN environment and relies on the `DNNrocketAPI` for any platform interaction.

2.  **`RocketFormsMod` (.NET Framework):**
    *   This is the DNN-specific presentation layer. It consists of a set of `.ascx` user controls that provide the administrative back-end for the module within the DNN framework.
    *   These controls are lightweight wrappers that load the appropriate AppTheme templates for rendering the UI, connecting the DNN interface to the `RocketForms` core logic.

---

## Administrative Interface

The administrative functions of RocketForms are managed through a series of dedicated `.ascx` controls, each providing a specific piece of functionality.

### `View.ascx` - The Public View
-   **Purpose:** This is the default control responsible for rendering the public-facing form on the website.
-   **Function:** It loads the selected AppTheme and renders its `View.cshtml` template. This template contains the HTML for the form fields, the submit button, and any client-side validation logic.

### `Edit.ascx` - Form Submissions & Data Management
-   **Purpose:** This control provides the interface for viewing and managing the data captured by the form.
-   **Function:** It renders the AppTheme's `Edit.cshtml` template, which typically displays a list or table of all the form submissions. From here, an administrator can view, search, and manage the submitted data.

### `Settings.ascx` - Module Configuration
-   **Purpose:** Manages the configuration settings for a specific instance of the RocketForms module. These settings control the behavior of the form.
-   **Function:** It renders the AppTheme's `Settings.cshtml` template. This is where you configure critical form settings, such as the recipient email address for notifications, subject lines, and other options that alter the form's processing logic.
-   **Example:** A setting might be "NotificationEmail" or "ThankYouPageURL". The system uses these settings when a form is submitted.

### `AppTheme.ascx` - In-line Template Editing
-   **Purpose:** Provides a built-in code editor for developers to customize the AppTheme files directly within the browser.
-   **Function:** This control allows for direct editing of the AppTheme's `.cshtml`, `.css`, and `.js` files. It is the primary tool for customizing the form's appearance, defining its data fields in the `Settings.cshtml`, and controlling the layout of the submissions view in `Edit.cshtml`. When a file is saved, an override is created at the Portal or Module level, preserving the original system theme.

