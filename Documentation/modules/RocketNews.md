# RocketNews Module

## Overview

RocketNews is a specialized content module for creating and managing a news feed or press releases. While it appears to the user as a standalone "News" module, it is architecturally a sophisticated **wrapper** around the core `RocketDirectory` engine.

This wrapper approach allows for multiple, independent directories to exist on a single DNN portal, each with its own data and theming, while reusing the powerful filtering and data management capabilities of RocketDirectory.

---

## Architecture: The Wrapper Model

The most important concept to understand is that RocketNews **delegates** its core functionality to RocketDirectory.

1.  **`RocketNewsAPI` (.NET Standard):**
    *   This project is often very lightweight. Its primary purpose is to provide a unique identity for the "News" module type.
    *   It can also contain any specialized business logic required for a news article that `RocketDirectoryAPI` doesn't provide, such as handling press contacts or source attributions.

2.  **`RocketNewsMod` (.NET Framework):**
    *   This project contains the module definition that makes "RocketNews" appear as an installable module in DNN.
    *   Crucially, it **does not contain its own administrative controls** like `Edit.ascx` or `Settings.ascx`. Instead, the module definition is configured to point to the controls within `RocketDirectoryMod`.

This means when an administrator clicks "Edit" on a RocketNews module, they are actually using the `Edit.ascx` control from the RocketDirectory module. The system knows to save the data in a way that is unique to the RocketNews module instance, even though the UI is shared.

---

## How It Works

1.  **Separate Data Storage:** Even though it uses RocketDirectory's controls, the data for RocketNews is stored separately. The `DNNrocketAPI` uses the module's type (`RocketNews`) to ensure data from a news article does not mix with data from a blog or a standard directory.

2.  **Dedicated AppThemes:** RocketNews has its own AppThemes. A theme designed for RocketNews will have templates (`Edit.cshtml`, `View.cshtml`) with fields and layouts specific to a news item, such as "Headline", "Summary", "FullStory", "PublicationDate", and "Source".

3.  **Specialized Logic:** The `RocketNewsAPI` can add extra functionality on top of the base directory features. For example, it could have logic to syndicate content to other services or to feature "breaking news" articles at the top of a list.

## Example Use Case

-   You add the **RocketNews** module to a page to show a list of official press releases.
-   You add the **RocketBlog** module to another page to show informal developer blog posts.
-   Both modules use the same underlying `RocketDirectory` engine and administrative controls.
-   However, the RocketNews module uses a "News" AppTheme, so its edit form asks for "Headline" and "Source".
-   The RocketBlog module uses a "Blog" AppTheme, and its edit form asks for "Title" and "Author".
-   The data for each is stored independently, and they can be themed and managed as completely separate entities, providing maximum code reuse and flexibility.