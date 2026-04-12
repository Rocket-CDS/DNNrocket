# Copilot Instructions

## Project Guidelines
- User prefers old-style Razor formatting with all if statements using multi-line braces (no single-line if statements).
- Fixes for MVC pipeline code should only use members actually available in `RazorModuleControlBase`; avoid suggesting invalid members like `Response` directly.
- When troubleshooting, ask targeted verification questions before proposing full fixes.