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
