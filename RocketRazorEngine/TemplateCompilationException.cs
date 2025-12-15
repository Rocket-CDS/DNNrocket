using System;

namespace RocketRazorEngine
{
    /// <summary>
    /// Exception thrown when template compilation fails.
    /// </summary>
    public class TemplateCompilationException : Exception
    {
        public string[] Errors { get; }
 public string SourceCode { get; }

public TemplateCompilationException(string message) : base(message)
   {
        }

        public TemplateCompilationException(string message, string[] errors, string sourceCode = null) 
          : base(message)
        {
      Errors = errors;
            SourceCode = sourceCode;
        }

        public TemplateCompilationException(string message, Exception innerException) 
          : base(message, innerException)
        {
        }
    }
}
