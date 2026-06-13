# Rocket Intelli VS Code Extension

This document provides instructions on how to install and use the "Rocket Intelli" Visual Studio Code extension for enhanced IntelliSense in Razor (`.cshtml`) and Blazor (`.razor`) files, with special support for DNNrocket framework tokens.

## Features

- **Custom IntelliSense**: Provides auto-completion for custom Rocket framework tokens within Razor and CSHTML files.
- **Trigger-based Completion**: IntelliSense is automatically triggered by typing the `@` character, suggesting relevant tokens.
- **Enhanced Development Workflow**: Speeds up development by providing quick access to the available token syntax and descriptions.

## Requirements

- **Visual Studio Code**: Version 1.80.0 or higher.
- **Node.js and npm**: Required if you plan to develop or build the extension from the source.

## Installation

The extension is provided as a `.vsix` file. You can install it directly within Visual Studio Code.

1.  **Download the VSIX file**: Obtain the `rocket-intelli-0.0.1.vsix` file from the [GitHub repository](https://github.com/Rocket-CDS/rocket-Intelli).
2.  **Open VS Code**: Launch Visual Studio Code.
3.  **Open the Extensions View**: Click on the Extensions icon in the Activity Bar on the side of the window or press `Ctrl+Shift+X`.
4.  **Install from VSIX**:
    *   Click the **...** (More Actions) menu in the top-right corner of the Extensions view.
    *   Select **Install from VSIX...** from the dropdown menu.
    *   Navigate to and select the `rocket-intelli-0.0.1.vsix` file you downloaded.
5.  **Reload VS Code**: If prompted, reload Visual Studio Code to activate the extension.

## Usage

Once installed, the extension will automatically activate when you open a `.cshtml` or `.razor` file.

To trigger IntelliSense, simply type the `@` character inside your Razor file. A list of available DNNrocket tokens and methods will appear. You can navigate the list using the arrow keys and press `Enter` or `Tab` to insert the selected token.

**Example:**

```razor
<div class="my-class">
    @DNNrocket.GetSetting("mykey")
</div>
```

When you type `@`, the extension will suggest tokens like `DNNrocket.GetSetting`.

