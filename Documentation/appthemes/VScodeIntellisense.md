# Rocket Intelli VS Code Extension

This document provides instructions on how to install and use the "Rocket Intelli" Visual Studio Code extension for enhanced IntelliSense in Razor (`.cshtml`) and Blazor (`.razor`) files, with special support for DNNrocket framework tokens.

## Features

- **Custom IntelliSense**: Provides auto-completion for custom Rocket framework tokens within Razor and CSHTML files.
- **Trigger-based Completion**: IntelliSense is automatically triggered by typing the `@` character, suggesting relevant tokens.
- **Signature Help**: Parameter hints are shown when typing inside function call parentheses.
- **Auto-updating Token Definitions**: Token JSON files are downloaded automatically from `docs.rocket-cds.org` and kept separate from the extension install, so they can be refreshed without reinstalling the extension.

## Requirements

- **Visual Studio Code**: Version 1.80.0 or higher.

## Installation

The extension is distributed as a `.vsix` file via the [rocket-Intelli GitHub repository](https://github.com/Rocket-CDS/rocket-Intelli).

1.  **Download the VSIX file**: Go to the [Releases page](https://github.com/Rocket-CDS/rocket-Intelli/releases) and download the latest `rocket-intelli-x.x.x.vsix` file.
2.  **Open VS Code**: Launch Visual Studio Code.
3.  **Open the Extensions View**: Press `Ctrl+Shift+X`.
4.  **Install from VSIX**:
    *   Click the **...** (More Actions) menu in the top-right corner of the Extensions panel.
    *   Select **Install from VSIX...**.
    *   Navigate to and select the `.vsix` file you downloaded.
5.  **Reload VS Code**: If prompted, reload Visual Studio Code to activate the extension.

## Token JSON Files

The extension's IntelliSense is driven by a set of JSON files that define all available Rocket tokens, their signatures, parameters, and descriptions. These files are **not bundled inside the VSIX** in a way that is meant to be edited — instead they are downloaded automatically from:

```
https://docs.rocket-cds.org/razortokens/
```

The following JSON files are downloaded:

| File | Description |
|---|---|
| `DNNrocketTokens.json` | Core DNNrocket token helpers |
| `DNNrocketUtils.json` | DNNrocket utility methods |
| `RazorEngineTokens.json` | Razor engine tokens |
| `GeneralUtils.json` | General utility methods |
| `UserUtils.json` | User-related utility methods |
| `RocketContentTokens.json` | RocketContent module tokens |
| `RocketDirectoryTokens.json` | RocketDirectory module tokens |
| `RocketEventsTokens.json` | RocketEvents module tokens |
| `RocketFormsTokens.json` | RocketForms module tokens |

### Where the JSON files are stored

Downloaded files are stored in VS Code's **global storage** folder for the extension, not inside the extension install directory. This means they persist across extension updates and are never overwritten by a reinstall.

### Automatic download on first run

When the extension activates for the first time and no token JSON files exist in global storage, it automatically downloads all files in the background without any prompts. Bundled copies of the JSON files (included in the `src` folder of the VSIX) are used as a fallback if the download has not yet completed.

### Manually updating the token files

A **Rocket Tokens** button is always visible in the VS Code status bar (bottom-right area):

```
☁ Rocket Tokens
```

Click this button at any time to download the latest token JSON files from `docs.rocket-cds.org`. This is useful after a new version of DNNrocket or a Rocket module is released, as new tokens will appear in the JSON files without requiring a new version of the extension to be installed.

A notification will confirm when the download is complete and IntelliSense is refreshed.

## Usage

Once installed, the extension activates automatically when you open a `.cshtml` or `.razor` file.

To trigger IntelliSense, type the `@` character inside your Razor file. A list of available Rocket tokens will appear. You can filter by class name using the dot notation:

- Type `@RocketContent` to see all RocketContent tokens.
- Type `@RocketContent.` to complete method names within that class.

Press `Enter` or `Tab` to insert the selected token. Parameter hints appear automatically when you open a function's parentheses.

**Example:**

```
<div class="my-class">
    @DNNrocket.GetSetting("mykey")
</div>
```

