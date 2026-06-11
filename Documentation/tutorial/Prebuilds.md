# Using Prebuilds for Site Export and Import

The Prebuild feature, found in the Rocket Tools section, is a powerful utility for creating and restoring complete snapshots of your DNN website. It extends the standard DNN Site Export/Import functionality by including critical Rocket-specific data, making it the recommended way to migrate or back up a Rocket-powered site.

---

## What a Prebuild Does

A Prebuild is more than just a standard site export. Here’s what makes it different:

*   **Extends DNN Export:** It uses the core DNN export engine to package all standard site assets like pages, modules, content, and files.
*   **Exports Global Rocket Data:** Crucially, it also exports all global data associated with the Rocket framework. This includes system-wide configurations for RocketDirectory, portal-level AppTheme settings, and other essential data that is not part of the standard DNN export.
*   **Excludes DNN Workflow:** The DNN Workflow feature is not supported by this process and is automatically excluded from the export to ensure a clean import.

---

## How to Export a Prebuild

Exporting your site into a prebuild package is a straightforward process.

1.  Navigate to the Persona Bar and go to **Rocket > Tools**.
2.  Find and click on the **Export Prebuild** option.
3.  The system will begin packaging the entire site, including all content and Rocket data, into a single `.zip` file.
4.  Once the process is complete, your browser will prompt you to save the prebuild `.zip` file to your local computer.

---

## How to Import a Prebuild

Importing a prebuild can sometimes be a two-step process, as it may require configuration changes to your DNN environment.

1.  Navigate to the Persona Bar and go to **Rocket > Tools**.
2.  Click on the **Import Prebuild** option.

3.  **Important First Step - Configuration Check:** The import tool will first check if your `web.config` file needs to be updated for compatibility with the prebuild.
    *   If changes are required, the tool will apply them and then **automatically restart the Application Pool**.
    *   You must wait for the site to come back online after the restart.

4.  **Second Step - The Actual Import:** After the site has restarted (or if no configuration changes were needed), you must navigate back to **Rocket > Tools** and click **Import Prebuild** again.

5.  This time, you will be prompted to select the prebuild `.zip` file you wish to import (e.g., `prebuild0_PREBUILD_W3.zip`).

6.  Select the file and the import process will begin. This will restore the site, including all pages, modules, content, and global Rocket data, to the exact state captured in the prebuild file.
