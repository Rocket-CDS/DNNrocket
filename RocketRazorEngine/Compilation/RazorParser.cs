using System;
using System.Text;
using System.Collections.Generic;

namespace RocketRazorEngine.Compilation
{
    /// <summary>
    /// Converts Razor syntax to C# code.
    /// </summary>
    internal class RazorParser
    {
        private string _inheritsType = null;
        private string _modelType = null;
        private List<string> _usingNamespaces = new List<string>();

        /// <summary>
        /// Parses Razor template into C# class code.
        /// </summary>
        public string Parse(string razorTemplate, string className, Type modelType)
        {
            var cleanedTemplate = ExtractAndRemoveDirectives(razorTemplate);
            var sb = new StringBuilder();

            // Generate default usings
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Net;");
            sb.AppendLine("using RocketRazorEngine.Templating;");
            sb.AppendLine("using RocketRazorEngine.Text;");

            // Add Simplisity namespace if we're inheriting from it
            if (_inheritsType != null && _inheritsType.Contains("Simplisity"))
            {
                sb.AppendLine("using Simplisity;");
            }

            // Add @using directives from template
            foreach (var usingNamespace in _usingNamespaces)
            {
                sb.AppendLine($"using {usingNamespace};");
            }

            sb.AppendLine();

            // Add model type namespace if available
            if (modelType != null && !string.IsNullOrEmpty(modelType.Namespace))
            {
                sb.AppendLine($"using {modelType.Namespace};");
            }

            // Determine base class and model type
            string baseClass;
            string modelTypeName;

            if (_inheritsType != null)
            {
                baseClass = _inheritsType;
                modelTypeName = null;
            }
            else if (_modelType != null)
            {
                modelTypeName = _modelType;
                baseClass = $"TemplateBase<{modelTypeName}>";
            }
            else
            {
                modelTypeName = GetSafeTypeName(modelType);
                baseClass = $"TemplateBase<{modelTypeName}>";
            }

            // Generate class
            sb.AppendLine($"namespace RocketRazorEngine.CompiledTemplates");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className} : {baseClass}");
            sb.AppendLine("    {");
            sb.AppendLine("        public override void Execute()");
            sb.AppendLine("        {");

            ParseRazorContent(cleanedTemplate, sb);

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string ExtractAndRemoveDirectives(string razorTemplate)
        {
            // Reset directives
            _inheritsType = null;
            _modelType = null;
            _usingNamespaces.Clear();

            var lines = razorTemplate.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // @inherits directive
                if (trimmed.StartsWith("@inherits "))
                {
                    _inheritsType = trimmed.Substring("@inherits ".Length).Trim();
                    continue;
                }
                // @model directive
                else if (trimmed.StartsWith("@model "))
                {
                    _modelType = trimmed.Substring("@model ".Length).Trim();
                    continue;
                }
                // @using directive
                else if (trimmed.StartsWith("@using "))
                {
                    var usingNamespace = trimmed.Substring("@using ".Length).Trim();
                    // Remove trailing semicolon if present
                    if (usingNamespace.EndsWith(";"))
                    {
                        usingNamespace = usingNamespace.Substring(0, usingNamespace.Length - 1).Trim();
                    }
                    _usingNamespaces.Add(usingNamespace);
                    continue;
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }

        private string GetSafeTypeName(Type modelType)
        {
            if (modelType == null)
            {
                return "dynamic";
            }

            var fullName = modelType.FullName;
            if (string.IsNullOrEmpty(fullName) ||
 fullName.Contains("<") ||
       fullName.Contains(">") ||
      fullName.Contains("`") ||
   fullName.Contains("[") ||
          fullName.StartsWith("<>"))
     {
        return "dynamic";
            }

            return fullName;
        }

        /// <summary>
        /// Gets the list of using namespaces from @using directives
        /// </summary>
        public List<string> GetUsingNamespaces()
{
            return new List<string>(_usingNamespaces);
        }

        /// <summary>
        /// Gets the inherits type from @inherits directive
  /// </summary>
        public string GetInheritsType()
        {
    return _inheritsType;
        }

        /// <summary>
        /// Gets the model type from @model directive
        /// </summary>
      public string GetModelType()
      {
            return _modelType;
        }

        private void ParseRazorContent(string content, StringBuilder sb, int indent = 12)
        {
            var indentStr = new string(' ', indent);
            int position = 0;

            while (position < content.Length)
            {
                // Check for @{ } code blocks
                if (content[position] == '@' && position + 1 < content.Length && content[position + 1] == '{')
                {
                    position += 2;
                    int blockStart = position;
                    int braceCount = 1;

                    while (position < content.Length && braceCount > 0)
                    {
                        if (content[position] == '{') braceCount++;
                        else if (content[position] == '}') braceCount--;
                        position++;
                    }

                    string codeBlock = content.Substring(blockStart, position - blockStart - 1);
                    foreach (var line in codeBlock.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            sb.AppendLine($"{indentStr}{line.Trim()}");
                        }
                    }
                    continue;
                }

                // Check for @ expressions (e.g., @Model.Name, @DateTime.Now)
                if (content[position] == '@' && position + 1 < content.Length && content[position + 1] != '@' && content[position + 1] != '{')
                {
                    position++;
                    int exprStart = position;

                    while (position < content.Length &&
               (char.IsLetterOrDigit(content[position]) ||
                         content[position] == '.' ||
                      content[position] == '_' ||
                      content[position] == '(' ||
               content[position] == ')' ||
              content[position] == '[' ||
                     content[position] == ']' ||
               content[position] == '"' ||
            content[position] == '\'' ||
                      content[position] == ' ' && position > exprStart && content[position - 1] == '('))
                    {
                        position++;
                    }

                    string expression = content.Substring(exprStart, position - exprStart).Trim();
                    if (!string.IsNullOrEmpty(expression))
                    {
                        sb.AppendLine($"{indentStr}Write({expression});");
                    }
                    continue;
                }

                // Check for @@ (escaped @)
                if (content[position] == '@' && position + 1 < content.Length && content[position + 1] == '@')
                {
                    position += 2;
                    sb.AppendLine($"{indentStr}WriteLiteral(\"@\");");
                    continue;
                }

                // Regular content - collect until next @ or end
                int contentStart = position;
                while (position < content.Length && content[position] != '@')
                {
                    position++;
                }

                if (position > contentStart)
                {
                    string literalContent = content.Substring(contentStart, position - contentStart);
                    if (!string.IsNullOrEmpty(literalContent))
                    {
                        literalContent = literalContent
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
              .Replace("\r", "\\r")
                  .Replace("\n", "\\n")
                .Replace("\t", "\\t");

                        sb.AppendLine($"{indentStr}WriteLiteral(\"{literalContent}\");");
                    }
                }
            }
        }
    }
}
