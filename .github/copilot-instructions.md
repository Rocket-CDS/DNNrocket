# Copilot Instructions

## General Guidelines
- User authorizes editing any file in projects in this solution without asking per-file permission.
- User wants autonomous template edits.

## Project Guidelines
- User prefers old-style Razor formatting with all if statements using multi-line braces (no single-line if statements).
- Fixes for MVC pipeline code should only use members actually available in `RazorModuleControlBase`; avoid suggesting invalid members like `Response` directly.
- In this codebase, w3.css should always be loaded and Font Awesome should not be loaded.
- When troubleshooting, ask targeted verification questions before proposing full fixes.

## HTML Standards
- User prefers strict semantic HTML for Jodit pasted content, converting `<b>`/`<i>` to `<strong>`/`<em>`.
- For Jodit pasted content, headings (h1-h6) must not contain span wrappers; keep only heading content.
- Remove Microsoft Word Office tags like `<o:p>` from Jodit content; they should not be present in saved HTML.
- For Jodit pasted content, remove Word formatting globally across all elements: no class/style Word attributes and no span/font wrappers in saved HTML.

## Template Improvements
- For the AppThemes-W3-CSS workspace, display template improvements must use W3 CSS.
- Apply blog-style design to other ListView.cshtml files.