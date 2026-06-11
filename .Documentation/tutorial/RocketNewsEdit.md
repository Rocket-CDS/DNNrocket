# Using the RocketNews Module

This guide explains how to set up and manage the RocketNews module, a system designed for publishing news articles, press releases, or announcements. It is built upon the flexible RocketDirectory system but is streamlined for journalistic content.

---

## Step 1: Initial Configuration

When a new RocketNews module is added to a page, it must be configured. The module requires an **AppTheme** to define its data fields (e.g., headline, article body), layout, and editing interface. Click the **Configure** button to start.

---

## Step 2: Select an AppTheme

On the AppTheme selection screen, you will choose a theme appropriate for news content.

1.  Select the **Project**: `AppThemes-W3-CSS`.
2.  Select the **AppTheme**: `rocketnewsapi.News`. This theme provides the structure for a classic news listing and detail page.
3.  Click **Save**.

---

## Step 3: Module Settings

The settings page controls the behavior of your news feed. It combines standard RocketDirectory options with settings specific to the News AppTheme.

### Standard Settings

*   **Disable Cache, ECO Mode, Template, Default Sort, List/Detail Page, API Module Reference:** These function exactly as they do in the standard RocketDirectory module, allowing you to control performance, select different view templates (e.g., list vs. grid), and reuse a news feed across multiple pages.

### News AppTheme Settings

The `rocketnewsapi.News` theme may add its own settings, such as:

*   **Articles Per Page:** The number of articles to display on the main news listing.
*   **Show "Read More" Link:** A toggle to show or hide a "Read More" link if the summary is used.
*   **Date Format:** Allows you to choose how the publication date is displayed (e.g., "MM/DD/YYYY" vs. "Month Day, Year").

---

## Step 4: Editing Data (Creating a News Article)

In "Edit" mode, you can write and manage your news articles. The fields are tailored for this purpose:

*   **Headline:** The title of the news article.
*   **Sub-headline/Summary:** A brief summary of the article.
*   **Article Body:** The full content of the news story.
*   **Source:** The source of the news, if it's from an external feed or newswire.
*   **Publication Date:** The date the article is published.
*   **Article Image:** A primary image to accompany the story.

---

## Step 5: The Administration Settings

The core administration for RocketNews is handled by the central RocketDirectory panel. All the system-wide settings described in the RocketDirectory tutorial—covering **General, Config, Order By, Providers, and Tools**—apply here as well. For instance, you could use the "SQL Filter" in the Config tab to create a module instance that only shows news from a specific category or time frame.
