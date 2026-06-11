# Using the RocketBlog Module

This guide explains how to set up and manage the RocketBlog module, a complete system for creating and managing a blog on your DNN site. As a specialized version of the RocketDirectory system, it provides a familiar interface tailored specifically for blog posts.

---

## Step 1: Initial Configuration

After adding a new RocketBlog module to a page, it will prompt you to configure it. The module needs an **AppTheme** to define its data structure (e.g., title, summary, post body), layout, and administrative interface. Click the **Configure** button to begin.

---

## Step 2: Select an AppTheme

You will be taken to the AppTheme selection screen. For a blog, you will use a theme designed for that purpose.

1.  Select the **Project**: `AppThemes-W3-CSS` (the default open-source theme project).
2.  Select the **AppTheme**: `rocketblogapi.Blog`. This theme provides all the necessary templates for a standard blog, including a post list, detail view, and data entry forms.
3.  Click **Save**.

---

## Step 3: Module Settings

The settings page allows you to control the behavior of your blog. It includes standard RocketDirectory settings plus options specific to the Blog AppTheme.

### Standard Settings

*   **Disable Cache, ECO Mode, Template, Default Sort, List/Detail Page, API Module Reference:** These function exactly as they do in the standard RocketDirectory module, allowing you to control performance, select view templates, and reuse data from other modules.

### Blog AppTheme Settings

The `rocketblogapi.Blog` theme will add its own settings, which may include:

*   **Posts Per Page:** The number of blog post summaries to show on the main listing page.
*   **Enable Comments:** An option to integrate with a commenting system like Disqus.
*   **Show Author/Date:** Toggles for displaying the author's name and the publication date on posts.
*   **Social Sharing Buttons:** Options to enable sharing links for platforms like Twitter, Facebook, and LinkedIn.

---

## Step 4: Editing Data (Creating a Blog Post)

In the module's "Edit" mode, you can create and manage your blog posts. The fields are specifically designed for blogging:

*   **Title:** The headline of your blog post.
*   **Summary:** A short teaser or summary that appears on the main blog listing page.
*   **Post Body:** The main content of your article, usually edited with a rich text editor.
*   **Author:** The name of the person who wrote the post.
*   **Publication Date:** The date the post should be published. You can often set this to a future date to schedule posts.
*   **Tags & Categories:** Taxonomies to organize your posts and help readers find related content.
*   **Featured Image:** An image that represents the post, often shown at the top of the article and on the listing page.

---

## Step 5: The Administration Settings

The underlying RocketDirectory administration panel governs all directory-based systems, including RocketBlog. These settings for **General, Config, Order By, Providers, and Tools** apply globally and function as described in the RocketDirectory tutorial. For example, you can use the "Order By" tab to create a sort option for `PublicationDate DESC` and set it as the default for your blog.
