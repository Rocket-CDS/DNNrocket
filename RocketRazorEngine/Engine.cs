using System;
using System.IO;
using RocketRazorEngine.Compilation;
using RocketRazorEngine.Templating;

namespace RocketRazorEngine
{
    /// <summary>
    /// Main entry point for RocketRazorEngine template compilation and execution.
    /// API compatible with Antaris RazorEngine.
    /// </summary>
    public static class Engine
    {
        private static readonly TemplateCache _cache = new TemplateCache();
        private static readonly TemplateCompiler _compiler = new TemplateCompiler();
  private static readonly object _compileLock = new object();

      /// <summary>
 /// Gets or sets the Razor service configuration (compatibility property).
     /// </summary>
  public static object Razor { get; set; }

   /// <summary>
      /// Compiles and runs a Razor template with a model.
 /// </summary>
/// <typeparam name="T">Model type</typeparam>
/// <param name="razorTemplate">The Razor template content</param>
/// <param name="cacheName">Cache key for the compiled template</param>
/// <param name="model">The model instance</param>
   /// <returns>Rendered template output</returns>
        public static string RunCompile<T>(string razorTemplate, string cacheName, T model)
        {
            return RunCompile(razorTemplate, cacheName, typeof(T), model);
 }

      /// <summary>
        /// Compiles and runs a Razor template with a model (type-based overload).
  /// </summary>
 public static string RunCompile(string razorTemplate, string cacheName, Type modelType, object model)
        {
    if (string.IsNullOrEmpty(razorTemplate))
    {
      throw new ArgumentNullException(nameof(razorTemplate));
            }

            if (string.IsNullOrEmpty(cacheName))
       {
          throw new ArgumentNullException(nameof(cacheName));
  }

        // Try to get from cache
   Type templateType = _cache.GetTemplate(cacheName);

      // Compile if not cached
        if (templateType == null)
        {
         lock (_compileLock)
   {
   // Double-check after acquiring lock
     templateType = _cache.GetTemplate(cacheName);
 
      if (templateType == null)
   {
          try
       {
     templateType = _compiler.CompileTemplate(razorTemplate, cacheName, modelType);
       _cache.AddTemplate(cacheName, templateType, templateType.Assembly);
       }
    catch (TemplateCompilationException ex)
 {
// Add template name to error message
          throw new TemplateCompilationException(
      $"Failed to compile template '{cacheName}': {ex.Message}",
        ex.Errors,
        ex.SourceCode);
  }
         }
     }
          }

            // Create instance and execute
     try
       {
    var instance = Activator.CreateInstance(templateType);
    
     // Set model using reflection
     var modelProperty = templateType.GetProperty("Model");
      if (modelProperty != null && model != null)
  {
     modelProperty.SetValue(instance, model);
        }

         // Create output writer
     var writer = new StringWriter();
       var outputProperty = templateType.GetProperty("Output");
       if (outputProperty != null)
 {
    outputProperty.SetValue(instance, writer);
  }

   // Execute template
          var executeMethod = templateType.GetMethod("Execute");
            if (executeMethod != null)
      {
         executeMethod.Invoke(instance, null);
     }

       return writer.ToString();
     }
 catch (Exception ex)
        {
  throw new TemplateCompilationException(
         $"Failed to execute template '{cacheName}': {ex.Message}",
   ex);
      }
    }

        /// <summary>
   /// Compiles and runs a Razor template without a model.
/// </summary>
      public static string RunCompile(string razorTemplate, string cacheName)
  {
       return RunCompile(razorTemplate, cacheName, null, null);
        }

  /// <summary>
    /// Checks if a template is already compiled and cached.
/// </summary>
  public static bool IsTemplateCached(string cacheName)
        {
 return _cache.Contains(cacheName);
        }

        /// <summary>
   /// Removes a template from the cache.
  /// </summary>
        public static void InvalidateCache(string cacheName)
        {
 _cache.Remove(cacheName);
    }

 /// <summary>
        /// Clears all cached templates.
        /// </summary>
      public static void ClearCache()
        {
_cache.Clear();
        }

        /// <summary>
        /// Adds an assembly reference for template compilation.
    /// </summary>
        public static void AddAssemblyReference(Type type)
   {
       _compiler.AddAssemblyReference(type);
 }
    }
}
