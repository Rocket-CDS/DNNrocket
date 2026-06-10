# RocketTools

## Overview

`RocketTools` is a special administrative module that serves as the central control panel for the entire DNNrocket ecosystem. Unlike other Rocket Modules that provide front-end functionality for website visitors, `RocketTools` is a pure back-end utility designed for developers and system administrators.

It is typically accessed from the DNN Persona Bar or as a standalone admin page. Its primary purpose is to provide a user interface for managing system settings, installing and updating components, and debugging the DNNrocket environment.

---

## Core Features

The `RocketTools` interface provides a suite of powerful utilities for managing a DNNrocket installation. The main tools available from its menu are:

### 1. System Configuration & Health
-   **System Info:** The main dashboard displays critical information about the environment, such as the current Portal ID, version numbers, file permissions, and cache status. A refresh button allows for reloading the environment.
-   **Global Settings:** Configure system-wide parameters that affect all Rocket Modules.
-   **Validate & Index:**
    *   **Index Portal:** Triggers a re-indexing of the portal's content for search purposes.
    *   **Validate System:** Performs a system health check.

### 2. Content & Language Tools
-   **Page Localization:** Provides tools for managing multi-lingual content on pages.
-   **Copy Language:** A utility to duplicate content from one language to another across the portal, which is useful when setting up a new language.
-   **Menu & URL Settings:** A suite of tools to manage URL generation, menu structures, and apply global styling helpers.

### 3. Import & Export of Pre-builds
This is a powerful feature for developers to package and distribute entire site configurations.
-   **Export Pre-build:** Allows a developer to package up all the custom portal-level configurations (AppThemes, data, settings) into a single distributable ZIP file. This is ideal for creating a "starter kit" or a snapshot of a completed project.
-   **Import Pre-build:** An administrator can upload a pre-build ZIP file, and `RocketTools` will automatically deploy all the AppThemes and data to the current portal. This is used to quickly set up a new site with a pre-configured structure and content.

### 4. Security & Data Management
-   **Role Manager:** A utility to manage security roles within the context of DNNrocket.
-   **Cloning Tool:** Provides functionality to duplicate or "clone" module content or settings, speeding up site builds.
-   **Cache Management:** Includes buttons to clear the DNNrocket system cache, which is often necessary after making configuration or template changes.

---

## How It Is Used

A typical workflow involving `RocketTools` might be:

1.  A developer finishes building a client's site.
2.  They navigate to `RocketTools` and use the **Export Pre-build** feature to create a `ClientName.zip` file containing all their portal-level AppTheme customizations and content.
3.  To deploy this to a live server, they install a fresh DNNrocket instance, navigate to `RocketTools`, and use the **Import Pre-build** feature to upload the `ClientName.zip`.
4.  The entire site configuration is instantly deployed and ready to go.

In summary, `RocketTools` is the essential administrative backend that makes the management of the sophisticated, multi-layered DNNrocket architecture possible through a user-friendly interface.

