# Copilot Instructions

## Project Guidelines
- User prefers old-style Razor formatting with all if statements using multi-line braces (no single-line if statements).
- Fixes for MVC pipeline code should only use members actually available in `RazorModuleControlBase`; avoid suggesting invalid members like `Response` directly.
- In this codebase, w3.css should always be loaded and Font Awesome should not be loaded.
- When troubleshooting, ask targeted verification questions before proposing full fixes.

## Template Improvements
- For the AppThemes-W3-CSS workspace, display template improvements must use W3 CSS.