using System;
using System.IO;
using System.Net;

namespace RocketRazorEngine.Templating
{
    /// <summary>
    /// Base class for Razor templates with strongly-typed model support.
    /// Compatible with RazorEngine.Templating.TemplateBase<T> API.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public abstract class TemplateBase<T>
    {
      private TextWriter _output;

   /// <summary>
        /// Gets or sets the model for the template.
   /// </summary>
   public T Model { get; set; }

   /// <summary>
        /// Gets or sets the output writer for the template.
        /// </summary>
        public TextWriter Output
 {
            get => _output ?? (_output = new StringWriter());
     set => _output = value;
    }

    /// <summary>
  /// Executes the template. Override this method in generated template classes.
        /// </summary>
        public virtual void Execute()
     {
   // This will be overridden by the dynamically compiled template
   }

      /// <summary>
        /// Writes an object value to the output (HTML-encoded by default).
        /// </summary>
      /// <param name="value">The value to write.</param>
        public virtual void Write(object value)
        {
      if (value == null)
  return;

       WriteLiteral(WebUtility.HtmlEncode(value.ToString()));
        }

        /// <summary>
        /// Writes a literal string to the output without encoding.
   /// </summary>
     /// <param name="value">The value to write.</param>
        public virtual void WriteLiteral(object value)
        {
   if (value == null)
           return;

            Output.Write(value.ToString());
        }

        /// <summary>
        /// Writes an attribute to the output.
   /// </summary>
        public virtual void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
 {
        WriteLiteral(prefix.Item1);

          foreach (var value in values)
            {
     WriteLiteral(value.Prefix.Item1);

      if (value.Literal)
           {
      WriteLiteral(value.Value.Item1);
         }
            else
       {
       Write(value.Value.Item1);
    }
          }

            WriteLiteral(suffix.Item1);
        }

        /// <summary>
        /// Clears the current output buffer.
        /// </summary>
        public void Clear()
      {
   if (_output is StringWriter sw)
         {
    var sb = sw.GetStringBuilder();
       sb.Clear();
            }
}
    }

    /// <summary>
    /// Represents an attribute value for WriteAttribute method.
    /// </summary>
    public class AttributeValue
    {
        public Tuple<string, int> Prefix { get; set; }
        public Tuple<object, int> Value { get; set; }
  public bool Literal { get; set; }

        public AttributeValue(Tuple<string, int> prefix, Tuple<object, int> value, bool literal)
        {
            Prefix = prefix;
            Value = value;
       Literal = literal;
        }
    }
}
