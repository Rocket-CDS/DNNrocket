# RocketBlog Module

## Overview

RocketBlog is a specialized content module for creating and managing a blog. While it appears to the user as a standalone "Blog" module, it is architecturally a sophisticated **wrapper** around the core `RocketDirectory` engine.

This wrapper approach allows for multiple, independent directories instances to exist on a single DNN portal, each with its own data and theming, while reusing the powerful filtering and data management capabilities of RocketDirectory.

---

## Architecture: The Wrapper Model

The most important concept to understand is that RocketBlog **delegates** its core functionality to RocketDirectory.

1.  **`RocketBlogAPI` (.NET Standard):**
    *   This project is often very lightweight. Its primary purpose is to provide a unique identity for the "Blog" module type.
    *   It can also contain any specialized business logic required for a blog that `RocketDirectoryAPI` doesn't provide, such as handling author metadata, categories, or pingbacks.

2.  **`RocketBlogMod` (.NET Framework):**
    *   This project contains the module definition that makes "RocketBlog" appear as an installable module in DNN.
    *   Crucially, it **does not contain its own administrative controls** like `Edit.ascx` or `Settings.ascx`. Instead, the module definition is configured to point to the controls within `RocketDirectoryMod`.

This means when an administrator clicks "Edit" on a RocketBlog module, they are actually using the `Edit.ascx` control from the RocketDirectory module. The system knows to save the data in a way that is unique to the RocketBlog module instance, even though the UI is shared.

---

## How It Works

1.  **Separate Data Storage:** Even though it uses RocketDirectory's controls, the data for RocketBlog is stored separately. The `DNNrocketAPI` uses the module's type (`RocketBlog`) to ensure data from a blog does not mix with data from a news module or a standard directory.

2.  **Dedicated AppThemes:** RocketBlog has its own AppThemes. A theme designed for RocketBlog will have templates (`Edit.cshtml`, `View.cshtml`) with fields and layouts specific to a blog post, such as "Title", "Summary", "PostBody", "PublishDate", and "Author".

3.  **Specialized Logic:** The `RocketBlogAPI` can add extra functionality on top of the base directory features. For example, after a blog post is saved, the `RocketBlogAPI` could have custom code that automatically generates an RSS feed entry or sends a notification, features that a generic directory would not need.

## Example Use Case

-   You add the **RocketBlog** module to a page to show a list of posts.
-   You add the **RocketNews** module to another page to show company news.
-   Both modules use the same underlying `RocketDirectory` engine and administrative controls.
-   However, the RocketBlog module uses a "Blog" AppTheme, so its edit form asks for "Author" and "Tags".
-   The RocketNews module uses a "News" AppTheme, and its edit form asks for "Press Contact" and "Source".
-   The data for each is stored independently, and they can be themed and managed as completely separate entities, providing maximum code reuse and flexibility.
