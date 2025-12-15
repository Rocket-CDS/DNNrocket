using System;

namespace RocketRazorEngine.Text
{
    /// <summary>
    /// Represents a raw (unencoded) string that implements IEncodedString.
    /// Compatible with RazorEngine.Text.RawString API.
    /// </summary>
    public class RawString : IEncodedString
    {
        private readonly string _value;

/// <summary>
  /// Initializes a new instance of the RawString class.
        /// </summary>
        /// <param name="value">The raw string value.</param>
    public RawString(string value)
     {
            _value = value ?? string.Empty;
   }

      /// <summary>
        /// Returns the raw string without encoding.
        /// </summary>
        public string ToEncodedString()
   {
   return _value;
  }

    /// <summary>
  /// Returns the raw string value.
   /// </summary>
        public override string ToString()
        {
            return _value;
        }

   /// <summary>
        /// Implicit conversion from string to RawString.
  /// </summary>
        public static implicit operator string(RawString rawString)
 {
       return rawString?._value;
        }
    }
}
