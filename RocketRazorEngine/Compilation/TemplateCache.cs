using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace RocketRazorEngine.Compilation
{
  /// <summary>
 /// Manages caching of compiled templates.
    /// </summary>
    internal class TemplateCache
    {
   private readonly ConcurrentDictionary<string, Type> _compiledTemplates;
        private readonly ConcurrentDictionary<string, Assembly> _compiledAssemblies;

        public TemplateCache()
        {
_compiledTemplates = new ConcurrentDictionary<string, Type>();
   _compiledAssemblies = new ConcurrentDictionary<string, Assembly>();
   }

  /// <summary>
        /// Gets a cached template type by key.
        /// </summary>
     public Type GetTemplate(string cacheKey)
        {
  _compiledTemplates.TryGetValue(cacheKey, out var templateType);
            return templateType;
    }

 /// <summary>
        /// Adds a compiled template to the cache.
      /// </summary>
      public void AddTemplate(string cacheKey, Type templateType, Assembly assembly)
        {
      _compiledTemplates[cacheKey] = templateType;
    _compiledAssemblies[cacheKey] = assembly;
 }

      /// <summary>
   /// Checks if a template exists in the cache.
        /// </summary>
        public bool Contains(string cacheKey)
        {
return _compiledTemplates.ContainsKey(cacheKey);
 }

      /// <summary>
        /// Removes a template from the cache.
        /// </summary>
        public void Remove(string cacheKey)
        {
            _compiledTemplates.TryRemove(cacheKey, out _);
        _compiledAssemblies.TryRemove(cacheKey, out _);
      }

        /// <summary>
        /// Clears all cached templates.
     /// </summary>
        public void Clear()
        {
    _compiledTemplates.Clear();
            _compiledAssemblies.Clear();
 }
    }
}
