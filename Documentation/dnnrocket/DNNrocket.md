# DNNrocket Core API

## Overview

The DNNrocket Core API (`DNNrocketAPI`) is the central nervous system of the Rocket-CDS ecosystem within the DNN (DotNetNuke) Platform. It functions as a critical abstraction layer and bridge, enabling modern .NET Standard libraries and modules to seamlessly operate within the .NET Framework-based DNN environment.

All Rocket Modules (like RocketBlog, RocketContent, etc.) are built on .NET Standard for maximum portability and modern development practices. They do not—and cannot—interact directly with the DNN Core APIs. Instead, they rely exclusively on the `DNNrocketAPI` to perform all CMS-related operations, such as database access, user management, file system operations, and page rendering.

This architecture decouples the modules from the underlying CMS, making them more robust, maintainable, and easier to develop.

---

## Core Concepts

### The Bridge Architecture
The primary purpose of `DNNrocketAPI` is to be a "bridge." It exposes a stable, well-defined API to the .NET Standard modules while handling all the complexities of interacting with the DNN Core in the background.

- **Modules (.NET Standard):** Contain business logic, data models, and UI templates. They are completely unaware of DNN.
- **DNNrocketAPI (.NET Framework):** Receives requests from modules, translates them into DNN-specific actions, executes them, and returns the results.
- **DNN Core (.NET Framework):** The underlying CMS platform that manages content, users, and site structure.

```
graph TD
    A["Rocket Modules (.NET Standard)"] -- "Feature Requests (e.g., Get Data, Save Content)" --> B["DNNrocketAPI (.NET Framework)"];
    B -- "Translates to DNN API Calls" --> C["DNN Core (Users, Pages, DB)"];
    C -- "Returns DNN Objects" --> B;
    B -- "Returns Standardized Objects" --> A;
```

### Simplisity - The Data Engine
A key component within `DNNrocketAPI` is `Simplisity`, a universal data access and modeling library. It provides a flexible, property-based data structure (`SimplisityInfo` and `SimplisityRecord`) that is used to pass information between the modules and the API. This avoids dependencies on `System.Data.DataTable` or other .NET Framework-specific data types.

---

## Key Components & Features

The `DNNrocketAPI` project is organized into several key functional areas:

### 1. System & Initialization (`SystemActions`, `StartConnect`)
- **`StartConnect`**: This is the entry point that hooks into the DNN application startup process. It initializes all necessary services, caches, and configurations required for the Rocket ecosystem to function.
- **System Actions**: Handles core system-level tasks, such as cache management, event logging, and providing system-wide information to modules.

### 2. DNN Integration (`DNNrocketUtils`)
This is a massive utility component that provides hundreds of helper functions to abstract away common DNN tasks. It is the heart of the bridge, offering simplified methods for:
- **Portal/Tab/Module Management**: Getting information about the current portal, page, and module context.
- **User & Role Management**: Fetching user data, checking permissions, and managing security.
- **Database Interaction**: A simplified wrapper around the DNN database provider, allowing modules to execute queries without knowing the underlying connection details.
- **File System**: Safe and abstracted access to the DNN file system, including secure folders and portal-specific directories.

### 3. Content Rendering (`DNNrocketController`, `Razor*`)
- **`DNNrocketController`**: The primary MVC controller that handles AJAX requests from the modules. It acts as the main server-side endpoint for client-side operations.
- **Razor Templating**: Provides the engine for rendering Razor templates (`.cshtml`) used by the modules. It injects the necessary data and context, allowing templates to be written with simple, clean models.

### 4. Module Communication (`ModuleParams`)
- **`ModuleParams`**: A dedicated class for managing the lifecycle of a module instance. When a module is rendered or an action is performed, `ModuleParams` gathers all the necessary context (module ID, settings, parameters) and makes it available to the rendering engine and action handlers. This ensures each module instance operates independently and correctly.

---

## How It Works: A Typical Request Flow

1.  A user interacts with a Rocket Module on a DNN page (e.g., submitting a form in RocketForms).
2.  The module's client-side script sends an AJAX request to the `DNNrocketController`. The request includes the module ID and action-specific parameters.
3.  `DNNrocketController` receives the request and uses `ModuleParams` to load the context for that specific module instance.
4.  It calls the appropriate function, often leveraging `DNNrocketUtils` to interact with the DNN database or file system.
5.  Data is retrieved or saved, typically using the `Simplisity` data format.
6.  The controller returns a standardized JSON response or a rendered Razor template back to the client.
7.  The module's client-side script updates the UI with the response.

This entire process happens without the module's core logic ever referencing a `DotNetNuke.*` namespace, fulfilling its role as a true bridge to the DNN platform.

