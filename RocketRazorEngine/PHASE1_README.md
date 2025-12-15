# RocketRazorEngine - Phase 1 Implementation

## Overview
Phase 1 implements the minimal API surface required for Simplisity compatibility with Antaris/RazorEngine.

## Implemented Components

### 1. Text Namespace (`RocketRazorEngine.Text`)

#### `IEncodedString`
- Interface for encoded string representations
- Compatible with `RazorEngine.Text.IEncodedString`

#### `RawString`
- Represents unencoded HTML/text output
- Compatible with `RazorEngine.Text.RawString`
- Used extensively in Simplisity's `RazorEngineTokens<T>` class for returning raw HTML

### 2. Templating Namespace (`RocketRazorEngine.Templating`)

#### `TemplateBase<T>`
- Abstract base class for Razor templates with strongly-typed models
- Compatible with `RazorEngine.Templating.TemplateBase<T>`
- Key members:
  - `Model` property - holds the template's data model
  - `Output` property - `TextWriter` for template output
  - `Execute()` method - overridden by compiled templates
- `Write()` method - writes HTML-encoded content
  - `WriteLiteral()` method - writes raw content without encoding
  - `WriteAttribute()` method - handles attribute rendering
  - `Clear()` method - clears output buffer

#### `AttributeValue`
- Helper class for attribute rendering
- Supports Razor's attribute syntax compilation

## API Compatibility

All Phase 1 classes maintain API compatibility with Antaris/RazorEngine:

| RazorEngine (Old) | RocketRazorEngine (New) |
|-------------------|-------------------------|
| `RazorEngine.Text.IEncodedString` | `RocketRazorEngine.Text.IEncodedString` |
| `RazorEngine.Text.RawString` | `RocketRazorEngine.Text.RawString` |
| `RazorEngine.Templating.TemplateBase<T>` | `RocketRazorEngine.Templating.TemplateBase<T>` |

## Simplisity Integration

The `RazorEngineTokens<T>` class in Simplisity only needs namespace changes:

```csharp
// OLD:
using RazorEngine.Templating;
using RazorEngine.Text;

// NEW:
using RocketRazorEngine.Templating;
using RocketRazorEngine.Text;
```

No other code changes are required in Simplisity.

## Next Steps (Phase 2)

Phase 2 will implement:
- Razor template parsing
- Dynamic C# code generation from Razor syntax
- Runtime compilation using Roslyn (Microsoft.CodeAnalysis.CSharp)
- Template caching mechanism
- Main Engine API for compiling and running templates

## Technical Notes

- Target Framework: **.NET Standard 2.0**
- HTML Encoding: Uses `System.Net.WebUtility.HtmlEncode` (available in .NET Standard 2.0)
- No external dependencies required for Phase 1
- Fully compatible with DNN runtime environment

## Testing

Phase 1 can be validated by:
1. Building RocketRazorEngine project (should compile without errors)
2. Updating Simplisity namespace references
3. Building Simplisity project (should compile without errors)
4. Runtime testing will require Phase 2 implementation
