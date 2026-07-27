# DNNrocket Documentation

This folder contains the DocFx documentation for the DNNrocket project and all associated modules.

## Prerequisites

Before building the documentation, you need to install DocFx:

### Option 1: Install via Chocolatey
```powershell
choco install docfx
```

### Option 2: Install via .NET Tool
```powershell
dotnet tool install -g docfx
```

### Option 3: Download Manually
Download from [DocFx Releases](https://github.com/dotnet/docfx/releases) and add to your PATH.

## Populating razortoken .md Documentation Files

> **For humans and AI:** This section explains how the files in `Documentation\razortokens\` are created and kept up to date.

Each `.md` file in `Documentation\razortokens\` (e.g. `UserUtils.md`, `PortalUtils.md`) documents all the public methods in a corresponding C# utility class. The process is two steps:

### Step 1 — Generate the `.json` file from C# source

The `.dnnpack` config file has a `<json>` section that maps a C# source file to an output `.json` file. Running the package build process parses the source and writes a structured JSON summary of every public, non-obsolete method.

Example `.dnnpack` config entry:
```xml
<json>
  <file>
    <input>D:\NEVOWEB\Project\DesktopModules\DNNrocket\API\Components\PortalUtils.cs</input>
    <output>D:\NEVOWEB\Project\DesktopModules\DNNrocket\Documentation\razortokens\PortalUtils.json</output>
  </file>
</json>
```

Each JSON entry has: `name`, `signature`, `description`, `parameters` (name + type + description), and `returns`.

### Step 2 — Generate the `.md` file from the `.json` file (using AI)

Once the `.json` file exists, open it and ask AI (GitHub Copilot or similar) to:

> "Generate a `PortalUtils.md` documentation file for the `razortokens` folder using the same HTML accordion format as `UserUtils.md`, based on the entries in `PortalUtils.json`. Infer a plain-English description for each method from its name and parameters where the `description` field in the JSON is empty."

The resulting `.md` uses `<details class="clean-accordion">` HTML blocks (the CSS style block is included at the top of the file — copy it from any existing `.md` in the same folder).

### Rules for the `.md` format

- The CSS `<style>` block at the top of the file must be included exactly as in `UserUtils.md`.
- Each method is a `<details class="clean-accordion">` block with a `<summary>` showing the method name.
- The `<div class="token-details">` contains: **Description**, **Signature** (in a `<pre><code>` block), and optionally **Parameters** (as a `<ul>`).
- Overloaded methods should have a disambiguating suffix in the `<summary>`, e.g. `LoginTabId (by portal ID)` vs `LoginTabId (current portal)`.
- Descriptions with empty `""` values in the JSON should be inferred by AI from the method name and context.

## Build JSON for intellisense

JSON files for intellisense can be automatically created.  

In the *.dnnpack config file add a "json" node that defines each file with what output file we want.

```
<json>
	<file>
		<input>D:\NEVOWEB\Project\DesktopModules\DNNrocket\API\Components\DNNrocketUtils.cs</input>
		<output>D:\NEVOWEB\Project\DesktopModules\DNNrocket\Documentation\razortokens\DNNrocketUtils.json</output>
	</file>
	<file>
		<input>D:\NEVOWEB\Project\DesktopModules\DNNrocket\API\render\DNNrocketTokens.cs</input>
		<output>D:\NEVOWEB\Project\DesktopModules\DNNrocket\Documentation\razortokens\DNNrocketTokens.json</output>
	</file>
</json>
```

Each file listed should be a code file, the program will process the code files to make a json summary of each 

Any methods as marked as [Obsolete] will be ignored.
Any methods private methods will be ignored.

## Building the Documentation

### Build and Serve (Recommended for Development)
This will build the documentation and start a local web server:

```powershell
cd D:\NEVOWEB\Project\DesktopModules\DNNrocket\Documentation
docfx docfx.json --serve
```

The documentation will be available at: **http://localhost:8080**

The server will watch for file changes and automatically rebuild.

### Build Only
To just build the documentation without serving:

```powershell
cd D:\NEVOWEB\Project\DesktopModules\DNNrocket\Documentation
docfx docfx.json
```

The generated website will be in the `_site` folder.

### Specify Custom Port
To use a different port:

```powershell
docfx docfx.json --serve --port 8081
```

## Folder Structure

```
.Documentation/
├── docfx.json              # DocFx configuration
├── toc.yml                 # Root table of contents
├── articles/               # Documentation articles
│   ├── index.md           # Home page
│   ├── toc.yml            # Articles navigation
│   ├── DNNrocket.md       # DNNrocket Core documentation
│   ├── RocketBlog.md      # RocketBlog module documentation
│   ├── RocketContent.md   # RocketContent module documentation
│   ├── RocketDirectory.md # RocketDirectory module documentation
│   ├── RocketEvents.md    # RocketEvents module documentation
│   ├── RocketForms.md     # RocketForms module documentation
│   ├── RocketNews.md      # RocketNews module documentation
│   ├── RocketTools.md     # RocketTools documentation
│   ├── RocketUtils.md     # RocketUtils documentation
│   ├── RocketEndInstall.md # RocketEndInstall documentation
│   ├── RocketPortal.md    # RocketPortal documentation
│   ├── AppThemes.md       # AppThemes documentation
│   └── Simplisity.md      # Simplisity documentation
└── _site/                  # Generated website (gitignored)
```

## Writing Documentation

All documentation is written in Markdown format. Each module has its own .md file in the `articles/` folder.

### Markdown Syntax
DocFx supports standard Markdown plus some extensions. See [DocFx Markdown](https://dotnet.github.io/docfx/docs/markdown.html) for details.

### Adding Images
1. Create an `images` folder in the .Documentation directory
2. Add your images there
3. Reference in markdown: `![Alt text](../images/your-image.png)`

### Cross-References
Link to other articles: `[Link Text](RocketBlog.md)`

## Project Mapping

The documentation covers these project pairs (linked projects share one documentation file):

- **DNNrocket** - Core API (DNNrocketAPI)
- **RocketBlog** - Blog module (RocketBlogAPI + RocketBlogMod)
- **RocketContent** - Content module (RocketContentAPI + RocketContentMod)
- **RocketDirectory** - Directory module (RocketDirectoryAPI + RocketDirectoryMod)
- **RocketEvents** - Events module (RocketEventsAPI + RocketEventsMod)
- **RocketForms** - Forms module (RocketForms + RocketFormsMod)
- **RocketNews** - News module (RocketNewsAPI + RocketNewsMod)
- **RocketTools** - Tools utility
- **RocketUtils** - Utilities library
- **RocketEndInstall** - Installation helper
- **RocketPortal** - Portal functionality
- **AppThemes** - Theme components
- **Simplisity** - Simplisity library

*Note: Razor projects are excluded from documentation as they are templates only.*

## Troubleshooting

### DocFx Command Not Found
Ensure DocFx is installed and in your PATH. Try restarting your terminal after installation.

### Port Already in Use
If port 8080 is in use, specify a different port with `--port` option.

### Build Errors
Check the console output for specific errors. Common issues:
- Invalid markdown syntax
- Broken links to other files
- Missing images or resources

## Additional Resources

- [DocFx Documentation](https://dotnet.github.io/docfx/)
- [DocFx Markdown Reference](https://dotnet.github.io/docfx/docs/markdown.html)
- [DocFx Configuration](https://dotnet.github.io/docfx/docs/config.html)
