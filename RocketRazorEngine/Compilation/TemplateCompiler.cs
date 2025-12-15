using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace RocketRazorEngine.Compilation
{
    /// <summary>
    /// Compiles C# code to assemblies using Roslyn.
    /// </summary>
    internal class TemplateCompiler
    {
        private readonly RazorParser _parser;
        private readonly List<MetadataReference> _references;

        public TemplateCompiler()
        {
            _parser = new RazorParser();
            _references = new List<MetadataReference>();

            // Add essential references
            AddDefaultReferences();
        }

        private void AddDefaultReferences()
        {
            // Core .NET Standard references
            _references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            _references.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
            _references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
            _references.Add(MetadataReference.CreateFromFile(typeof(System.Text.StringBuilder).Assembly.Location));

            // .NET Standard 2.0
            try
            {
                _references.Add(MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location));
            }
            catch { /* netstandard might not be available in all contexts */ }

            // System.Runtime
            try
            {
                _references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location));
            }
            catch { /* Might not be available */ }

            // System.Collections
            try
            {
                _references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location));
            }
            catch { /* Might not be available */ }

            // System.Linq
            try
            {
                _references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location));
            }
            catch { /* Might not be available */ }

            // System.Core (for dynamic support)
            try
            {
                _references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Core").Location));
            }
            catch { /* Might not be available in .NET Core */ }

            // Microsoft.CSharp (for dynamic support)
            try
            {
                _references.Add(MetadataReference.CreateFromFile(Assembly.Load("Microsoft.CSharp").Location));
            }
            catch { /* Might not be available */ }

            // Add DynamicAttribute explicitly for dynamic support
            try
            {
                var dynamicType = typeof(System.Runtime.CompilerServices.DynamicAttribute);
                _references.Add(MetadataReference.CreateFromFile(dynamicType.Assembly.Location));
            }
            catch { /* DynamicAttribute might not be available */ }

            // RocketRazorEngine itself
            _references.Add(MetadataReference.CreateFromFile(typeof(Templating.TemplateBase<>).Assembly.Location));
        }

        /// <summary>
        /// Adds a reference to an assembly.
        /// </summary>
        public void AddAssemblyReference(Assembly assembly)
        {
            if (assembly != null)
            {
                _references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        /// <summary>
        /// Adds a reference to an assembly by type.
        /// </summary>
        public void AddAssemblyReference(Type type)
        {
            if (type != null)
            {
                AddAssemblyReference(type.Assembly);
            }
        }

        /// <summary>
        /// Compiles a Razor template to a Type.
        /// </summary>
        public Type CompileTemplate(string razorTemplate, string templateName, Type modelType)
        {
            // Generate class name from template name
            var className = SanitizeClassName(templateName);

            // Parse Razor to C#
            var csharpCode = _parser.Parse(razorTemplate, className, modelType);

            // Add model type assembly reference if needed
            if (modelType != null)
            {
                AddAssemblyReference(modelType);
            }

            // Auto-load assemblies from @using and @inherits directives
            AutoLoadAssembliesFromDirectives();

            // Check if code contains Simplisity references and add assembly
            if (csharpCode.Contains("Simplisity"))
            {
                try
                {
                    // Try to load Simplisity assembly and add reference
                    var simplisityAssembly = Assembly.Load("Simplisity");
                    AddAssemblyReference(simplisityAssembly);
                }
                catch
                {
                    // Simplisity not available - template will fail to compile if it really needs it
                }
            }

            // Compile
            var syntaxTree = CSharpSyntaxTree.ParseText(csharpCode);

            var compilation = CSharpCompilation.Create(
           $"RocketRazorEngine.Dynamic.{className}",
      new[] { syntaxTree },
     _references,
       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    .WithOptimizationLevel(OptimizationLevel.Release)
   .WithPlatform(Platform.AnyCpu));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    // Compilation failed
                    var errors = result.Diagnostics
                         .Where(d => d.Severity == DiagnosticSeverity.Error)
                 .Select(d => $"{d.Id}: {d.GetMessage()}")
                    .ToArray();

                    throw new TemplateCompilationException(
                     "Template compilation failed",
                                errors,
                              csharpCode);
                }

                // Load assembly
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                // Get the template type
                var typeName = $"RocketRazorEngine.CompiledTemplates.{className}";
                var templateType = assembly.GetType(typeName);

                if (templateType == null)
                {
                    throw new TemplateCompilationException(
              $"Could not find type '{typeName}' in compiled assembly");
                }

                return templateType;
            }
        }
        private string SanitizeClassName(string templateName)
        {
            // Remove invalid characters and ensure valid C# identifier
            var sb = new StringBuilder();
            bool firstChar = true;

            foreach (char c in templateName)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    firstChar = false;
                }
                else if (!firstChar && c == '_')
                {
                    sb.Append(c);
                }
            }

            if (sb.Length == 0 || char.IsDigit(sb[0]))
            {
                sb.Insert(0, "Template_");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Automatically loads assemblies based on @using and @inherits directives
        /// </summary>
        private void AutoLoadAssembliesFromDirectives()
        {
            var usingNamespaces = _parser.GetUsingNamespaces();
            var inheritsType = _parser.GetInheritsType();

            foreach (var ns in usingNamespaces)
            {
                TryLoadAssemblyFromNamespace(ns);
            }

            if (!string.IsNullOrEmpty(inheritsType))
            {
                TryLoadAssemblyFromTypeName(inheritsType);
            }
        }

        /// <summary>
        /// Attempts to load an assembly based on a namespace
        /// </summary>
        private void TryLoadAssemblyFromNamespace(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) return;

            try
            {
                var assemblyNames = new[] { namespaceName, namespaceName.Split('.')[0] };

                foreach (var assemblyName in assemblyNames)
                {
                    try
                    {
                        var assembly = Assembly.Load(assemblyName);
                        if (assembly != null && !string.IsNullOrEmpty(assembly.Location))
                        {
                            if (!_references.Any(r => r.Display != null && r.Display.Contains(assembly.GetName().Name)))
                            {
                                AddAssemblyReference(assembly);
                                return;
                            }
                        }
                    }
                    catch { continue; }
                }
            }
            catch { }
        }

        /// <summary>
        /// Attempts to load assemblies based on a type name from @inherits
        /// </summary>
        private void TryLoadAssemblyFromTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return;

            try
            {
                var cleanTypeName = typeName;

                if (cleanTypeName.Contains("<"))
                {
                    var genericStart = cleanTypeName.IndexOf('<');
                    var genericEnd = cleanTypeName.LastIndexOf('>');
                    if (genericEnd > genericStart)
                    {
                        var genericArgs = cleanTypeName.Substring(genericStart + 1, genericEnd - genericStart - 1);
                        foreach (var arg in genericArgs.Split(','))
                        {
                            TryLoadAssemblyFromTypeName(arg.Trim());
                        }
                    }
                    cleanTypeName = cleanTypeName.Substring(0, genericStart);
                }

                var lastDot = cleanTypeName.LastIndexOf('.');
                if (lastDot > 0)
                {
                    var namespaceName = cleanTypeName.Substring(0, lastDot);
                    TryLoadAssemblyFromNamespace(namespaceName);
                }
            }
            catch { }
        }
    }
}
