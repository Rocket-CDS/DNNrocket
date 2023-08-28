using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;
using RazorEngine.Templating;
using Simplisity;
using Simplisity.TemplateEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// 
    /// ****NOT USED****
    /// 
    /// This class is a caching provider for RazorEngine.
    /// It works to AppPool memory, but that is no different to the default caching.
    /// Currently this caching makes a copy of the compiled template folder, but the CompiledTemplate is protected, so we cannot rebuild and add to cache. 
    /// </summary>
    /// <seealso cref="RazorEngine.Templating.ICachingProvider" />
    public class RazorCacheProvider : ICachingProvider
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, ICompiledTemplate>> _cache =
            new ConcurrentDictionary<string, ConcurrentDictionary<Type, ICompiledTemplate>>();

        private readonly TypeLoader _loader;
        private readonly ConcurrentBag<Assembly> _assemblies = new ConcurrentBag<Assembly>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCachingProvider"/> class.
        /// </summary>
        public RazorCacheProvider()
        {
            _loader = new TypeLoader(AppDomain.CurrentDomain, _assemblies);
        }

        /// <summary>
        /// The manages <see cref="TypeLoader"/>. See <see cref="ICachingProvider.TypeLoader"/>
        /// </summary>
        public TypeLoader TypeLoader
        {
            get
            {
                return _loader;
            }
        }

        /// <summary>
        /// Get the key used within a dictionary for a modelType.
        /// </summary>
        public static Type GetModelTypeKey(Type modelType)
        {
            if (modelType == null ||
                typeof(System.Dynamic.IDynamicMetaObjectProvider).IsAssignableFrom(modelType))
            {
                return typeof(System.Dynamic.DynamicObject);
            }
            return modelType;
        }

        private void CacheTemplateHelper(ICompiledTemplate template, ITemplateKey templateKey, Type modelTypeKey)
        {
            var uniqueKey = templateKey.GetUniqueKeyString();
            _cache.AddOrUpdate(uniqueKey, key =>
            {
                // new item added
                _assemblies.Add(template.TemplateAssembly);
                var dict = new ConcurrentDictionary<Type, ICompiledTemplate>();
                dict.AddOrUpdate(modelTypeKey, template, (t, old) => template);
                return dict;
            }, (key, dict) =>
            {
                dict.AddOrUpdate(modelTypeKey, t =>
                {
                    // new item added (template was not compiled with the given type).
                    _assemblies.Add(template.TemplateAssembly);
                    return template;
                }, (t, old) =>
                {
                    // item was already added before
                    return template;
                });
                return dict;
            });
        }

        /// <summary>
        /// Caches a template. See <see cref="ICachingProvider.CacheTemplate"/>.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templateKey"></param>
        public void CacheTemplate(ICompiledTemplate template, ITemplateKey templateKey)
        {

            // save compiled template to persistant folder
            var sourceFolder = Path.GetDirectoryName(template.TemplateAssembly.Location);
            var destFolder = PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\razor" + templateKey.Name + "\\" +  Path.GetFileName(Path.GetDirectoryName(template.TemplateAssembly.Location));
            GeneralUtils.CopyDirectory(sourceFolder, destFolder, true);
            // --------------------------------------


            var modelTypeKey = GetModelTypeKey(template.ModelType);
            CacheTemplateHelper(template, templateKey, modelTypeKey);
            var typeArgs = template.TemplateType.BaseType.GetGenericArguments();
            if (typeArgs.Length > 0)
            {
                var alternativeKey = GetModelTypeKey(typeArgs[0]);
                if (alternativeKey != modelTypeKey)
                {
                    // could be a template with an @model directive.
                    CacheTemplateHelper(template, templateKey, typeArgs[0]);
                }
            }
        }

        /// <summary>
        /// Try to retrieve a template from the cache. See <see cref="ICachingProvider.TryRetrieveTemplate"/>.
        /// </summary>
        /// <param name="templateKey"></param>
        /// <param name="modelType"></param>
        /// <param name="compiledTemplate"></param>
        /// <returns></returns>
        public bool TryRetrieveTemplate(ITemplateKey templateKey, Type modelType, out ICompiledTemplate compiledTemplate)
        {
            compiledTemplate = null;
            var uniqueKey = templateKey.GetUniqueKeyString();
            var modelTypeKey = GetModelTypeKey(modelType);
            ConcurrentDictionary<Type, ICompiledTemplate> dict;
            if (!_cache.TryGetValue(uniqueKey, out dict))
            {
                // add cache. (CompilationData does not serialize) 
                var folderPath = PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache\\razor" + templateKey.Name;
                if (Directory.Exists(folderPath))
                {

                    //reset cache
                    if (!_cache.TryGetValue(uniqueKey, out dict))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;

                }
            }
            return dict.TryGetValue(modelTypeKey, out compiledTemplate);
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            _loader.Dispose();
        }
    }
}
