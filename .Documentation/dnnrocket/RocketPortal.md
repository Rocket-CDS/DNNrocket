# RocketPortal

## Overview

`RocketPortal` is a specialized administrative module within the DNNrocket ecosystem, designed to function as a "portal-specific" control panel. While `RocketTools` manages system-wide settings for the entire DNN installation, `RocketPortal` provides a dashboard for managing settings and data that apply only to the current portal.

This is crucial in a multi-portal DNN environment, where an administrator might need to configure settings for one website (portal) without affecting others on the same DNN instance. It acts as a focused, portal-aware version of `RocketTools`.

---

## Core Features

The `RocketPortal` interface is built using the same AppTheme and Simplisity engine as other modules, but its tools are scoped to the current portal.

### 1. Portal-Specific Data Management
This is the primary function of `RocketPortal`. It provides an interface to manage data that is unique to the current portal.
-   **Data Dashboard:** Displays a list of data sources or content types relevant to the current portal.
-   **Content Editing:** Allows administrators to directly view, edit, and manage `Simplisity` data records associated with the portal. This could include things like global content blocks, portal-wide configuration settings, or lists of shared resources.

### 2. Portal Settings
Provides a UI for managing settings that override the system-level defaults for the current portal.
-   **Theme Overrides:** May provide options to set a default AppTheme for certain module types across the portal.
-   **Configuration:** Manage portal-specific API keys, settings, or other parameters that modules within the portal will use.

---

