# RocketRazorEngine - Quick Start Guide

## Installation

### Option 1: Project Reference (Recommended for DNN)
```xml
<ItemGroup>
  <ProjectReference Include="path\to\RocketRazorEngine\RocketRazorEngine.csproj" />
</ItemGroup>
```

### Option 2: DLL Reference
Copy `RocketRazorEngine.dll` to your project and reference it.

## Basic Usage

### 1. Simple Template
```csharp
using RocketRazorEngine;

var template = "<h1>Hello @Model.Name!</h1>";
var model = new { Name = "World" };
var output = Engine.RunCompile(template, "hello-template", model);

Console.WriteLine(output);
// Output: <h1>Hello World!</h1>
```

### 2. Template with Code Block
```csharp
var template = @"
@{
    var greeting = ""Hello"";
    var name = Model.Name;
}
<p>@greeting @name!</p>";

var model = new { Name = "RocketRazorEngine" };
var output = Engine.RunCompile(template, "greeting", model);
```

### 3. Template with Loop
```csharp
var template = @"
<ul>
@{
    foreach(var item in Model.Items)
    {
        WriteLiteral(""<li>"");
        Write(item);
    WriteLiteral(""</li>"");
    }
}
</ul>";

var model = new { Items = new[] { "Apple", "Banana", "Cherry" } };
var output = Engine.RunCompile(template, "list", model);
```

### 4. Strongly-Typed Template
```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var template = @"
<div>
    <h2>@Model.Name</h2>
    <p>Age: @Model.Age</p>
</div>";

var person = new Person { Name = "John", Age = 30 };
var output = Engine.RunCompile<Person>(template, "person-card", person);
```

## Razor Syntax Supported

### Output Expressions
```razor
@Model.Property          ? Supported
@variable           ? Supported
@(complex + expression)  ? Supported (use parentheses for complex expressions)
```

### Code Blocks
```razor
@{
    var x = 5;
    var y = 10;
    var z = x + y;
}
<p>Result: @z</p>        ? Supported
```

### Loops
```razor
@{
    foreach(var item in Model.Items)
    {
        WriteLiteral("<li>");
     Write(item);
        WriteLiteral("</li>");
    }
}                 ? Supported
```

### Conditionals
```razor
@{
    if (Model.IsActive)
    {
        WriteLiteral("<p>Active</p>");
    }
    else
    {
        WriteLiteral("<p>Inactive</p>");
    }
}     ? Supported
```

### Escaped @ Symbol
```razor
@@username        ? Outputs: @username
```

## Performance Tips

### 1. Always Use Cache Keys
```csharp
// ? Good - template is compiled once
Engine.RunCompile(template, "my-template", model);

// ? Bad - template is recompiled every time
Engine.RunCompile(template, Guid.NewGuid().ToString(), model);
```

### 2. Check If Cached
```csharp
if (!Engine.IsTemplateCached("my-template"))
{
    // First time - will compile
}
// Subsequent calls use cached version
```

### 3. Precompile Templates
```csharp
// At application startup
Engine.RunCompile(template1, "template1");
Engine.RunCompile(template2, "template2");
// Later usage will be instant
```

### 4. Clear Cache When Needed
```csharp
// Clear specific template
Engine.InvalidateCache("my-template");

// Clear all templates
Engine.ClearCache();
```

## Advanced Features

### Add Custom Assembly References
```csharp
// Before compiling templates using custom types
Engine.AddAssemblyReference(typeof(MyCustomClass));

var template = "@Model.MyCustomProperty";
var model = new MyCustomClass();
var output = Engine.RunCompile(template, "custom", model);
```

### Error Handling
```csharp
try
{
    var output = Engine.RunCompile(template, "test", model);
}
catch (TemplateCompilationException ex)
{
    Console.WriteLine($"Compilation failed: {ex.Message}");
    
    if (ex.Errors != null)
{
  foreach (var error in ex.Errors)
 {
            Console.WriteLine($"  - {error}");
        }
    }
    
    if (ex.SourceCode != null)
    {
        Console.WriteLine("Generated C# code:");
        Console.WriteLine(ex.SourceCode);
    }
}
```

## Common Patterns

### Pattern 1: Template Library
```csharp
public static class TemplateLibrary
{
    private const string EmailTemplate = @"
 <h1>Hello @Model.Name!</h1>
        <p>@Model.Message</p>
    ";
    
    private const string ReportTemplate = @"
     <table>
  @{
            foreach(var row in Model.Rows)
            {
  WriteLiteral(""<tr><td>"");
Write(row);
        WriteLiteral(""</td></tr>"");
            }
        }
   </table>
    ";
    
    public static string RenderEmail(dynamic model)
    {
        return Engine.RunCompile(EmailTemplate, "email", model);
    }
    
    public static string RenderReport(dynamic model)
    {
        return Engine.RunCompile(ReportTemplate, "report", model);
    }
}
```

### Pattern 2: File-Based Templates
```csharp
public class TemplateRenderer
{
    private readonly string _templatePath;
    
    public TemplateRenderer(string templatePath)
    {
   _templatePath = templatePath;
    }
    
    public string Render<T>(string templateName, T model)
    {
var filePath = Path.Combine(_templatePath, templateName + ".cshtml");
        var template = File.ReadAllText(filePath);
        
        // Use file path as cache key
        var cacheKey = $"file:{filePath}";
        
        return Engine.RunCompile(template, cacheKey, model);
    }
}
```

### Pattern 3: Template Inheritance (using Simplisity)
```csharp
// Your template can inherit from RazorEngineTokens<T>
var template = @"
    @AddProcessData(""key"", ""value"")
    @HiddenField(Model, ""genxml/id"")
    @TextBox(Model, ""genxml/name"")
";

var info = new SimplisityInfo();
var output = Engine.RunCompile<SimplisityInfo>(template, "simplisity-form", info);
```

## Troubleshooting

### Problem: "Type not found" error
**Solution:** Add assembly reference before compiling:
```csharp
Engine.AddAssemblyReference(typeof(YourType));
```

### Problem: Complex expression not working
**Solution:** Use parentheses or code blocks:
```csharp
// Instead of:
@Model.Items.Where(x => x.IsActive).Count()

// Use:
@(Model.Items.Where(x => x.IsActive).Count())

// Or:
@{
    var count = Model.Items.Where(x => x.IsActive).Count();
}
<p>@count</p>
```

### Problem: Template not updating
**Solution:** Clear the cache:
```csharp
Engine.InvalidateCache("template-name");
```

### Problem: Special characters in output
**Solution:** Use `WriteLiteral()` for raw HTML:
```csharp
@{
    WriteLiteral("<div class='special'>");
    Write(Model.Content); // This is HTML-encoded
    WriteLiteral("</div>");
}
```

## Testing

### Unit Test Example
```csharp
[Test]
public void Template_Should_Render_Correctly()
{
    // Arrange
    var template = "<h1>@Model.Title</h1>";
    var model = new { Title = "Test" };
    
    // Act
  var output = Engine.RunCompile(template, "unit-test", model);
    
 // Assert
    Assert.That(output, Is.EqualTo("<h1>Test</h1>"));
}
```

## Best Practices

1. ? **Always use meaningful cache keys**
2. ? **Precompile templates at startup**
3. ? **Use strongly-typed models when possible**
4. ? **Handle TemplateCompilationException**
5. ? **Clear cache when templates change**
6. ? **Keep templates simple and focused**
7. ? **Use code blocks for complex logic**
8. ? **Test templates with unit tests**

## Migration from Antaris/RazorEngine

### Before
```csharp
using RazorEngine;
using RazorEngine.Templating;

var result = Engine.Razor.RunCompile(template, "key", typeof(MyModel), model);
```

### After
```csharp
using RocketRazorEngine;

var result = Engine.RunCompile(template, "key", typeof(MyModel), model);
// Or with generics:
var result = Engine.RunCompile<MyModel>(template, "key", model);
```

### Changes Required
1. Change namespace: `RazorEngine` ? `RocketRazorEngine`
2. Remove `.Razor` from API calls (optional - both work)
3. Update assembly references

That's it! Most code works without changes.

## Resources

- **Full Documentation:** See `PHASE2_COMPLETE.md`
- **API Reference:** See `API_COMPATIBILITY.md`
- **Examples:** See `RREconsole/Program.cs`
- **Migration Guide:** See `PROJECT_COMPLETE.md`

## Support

For issues or questions:
1. Check the documentation files
2. Review RREconsole examples
3. Check for TemplateCompilationException details
4. Review generated C# code (in exception.SourceCode)

---

**Happy templating with RocketRazorEngine! ??**
