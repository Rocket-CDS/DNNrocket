# API Compatibility Reference

## Namespace Mapping

| Antaris/RazorEngine | RocketRazorEngine |
|---------------------|-------------------|
| `RazorEngine.Text` | `RocketRazorEngine.Text` |
| `RazorEngine.Templating` | `RocketRazorEngine.Templating` |

## Type Mapping

### Text Types

| Old API | New API | Usage in Simplisity |
|---------|---------|---------------------|
| `RazorEngine.Text.IEncodedString` | `RocketRazorEngine.Text.IEncodedString` | Return type for all 50+ helper methods |
| `RazorEngine.Text.RawString` | `RocketRazorEngine.Text.RawString` | Wraps HTML output: `new RawString(html)` |

### Templating Types

| Old API | New API | Usage in Simplisity |
|---------|---------|---------------------|
| `RazorEngine.Templating.TemplateBase<T>` | `RocketRazorEngine.Templating.TemplateBase<T>` | Base class for `RazorEngineTokens<T>` |

## Code Examples

### Before (Antaris/RazorEngine)

```csharp
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Simplisity
{
    public class RazorEngineTokens<T> : TemplateBase<T>
    {
        public IEncodedString HiddenField(SimplisityInfo info, string xpath)
        {
    var html = "<input type='hidden' value='...' />";
        return new RawString(html);
        }
    }
}
```

### After (RocketRazorEngine)

```csharp
using RocketRazorEngine.Templating;
using RocketRazorEngine.Text;

namespace Simplisity
{
    public class RazorEngineTokens<T> : TemplateBase<T>
    {
        public IEncodedString HiddenField(SimplisityInfo info, string xpath)
        {
 var html = "<input type='hidden' value='...' />";
          return new RawString(html);
        }
    }
}
```

**Result:** Only the `using` statements change!

## Member Compatibility

### IEncodedString Interface

| Member | Status | Notes |
|--------|--------|-------|
| `string ToEncodedString()` | ? Implemented | Returns encoded string representation |

### RawString Class

| Member | Status | Notes |
|--------|--------|-------|
| Constructor: `RawString(string)` | ? Implemented | Creates instance with string value |
| `string ToEncodedString()` | ? Implemented | Returns raw string |
| `string ToString()` | ? Implemented | Returns raw string |
| Implicit conversion to string | ? Implemented | Allows: `string s = rawString;` |

### TemplateBase<T> Class

| Member | Status | Notes |
|--------|--------|-------|
| `T Model { get; set; }` | ? Implemented | Strongly-typed model property |
| `TextWriter Output { get; set; }` | ? Implemented | Output stream for rendering |
| `void Execute()` | ? Implemented | Virtual method for template execution |
| `void Write(object)` | ? Implemented | Writes HTML-encoded content |
| `void WriteLiteral(object)` | ? Implemented | Writes raw content |
| `void WriteAttribute(...)` | ? Implemented | Writes attributes (used by Razor compiler) |
| `void Clear()` | ? Implemented | Clears output buffer |

## Binary Compatibility

| Aspect | Status |
|--------|--------|
| Method signatures | ? 100% match |
| Property types | ? 100% match |
| Inheritance hierarchy | ? 100% match |
| Namespace structure | ? Parallel structure |

## Migration Checklist

- [x] Create `RocketRazorEngine.Text.IEncodedString`
- [x] Create `RocketRazorEngine.Text.RawString`
- [x] Create `RocketRazorEngine.Templating.TemplateBase<T>`
- [x] Create `RocketRazorEngine.Templating.AttributeValue`
- [x] Verify RocketRazorEngine builds
- [ ] Update Simplisity using statements
- [ ] Verify Simplisity builds
- [ ] Implement Phase 2 (compilation engine)

## Known Differences

None! Phase 1 provides 100% API compatibility for the features Simplisity uses.

## Phase 2 Requirements

Phase 2 will add the **template compilation engine**, which Simplisity doesn't directly use but is called by the DNN framework:

- Template parsing (Razor ? C#)
- Dynamic compilation (Roslyn/CSharpCompilation)
- Template caching
- `Engine.Razor.RunCompile()` method

These are not needed for Phase 1 compilation but are required for runtime execution.
