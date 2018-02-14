using System;
using System.Text.RegularExpressions;

namespace <%= namespace %>
{
    /// <summary>
    /// Provides extra functionality for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convert the string to camel case.
        /// </summary>
        public static string ToCamelCase(this string value)
        {
            string[] spacedWords = Regex.Split(value, @"(?<!^)(?=[A-Z])");
            spacedWords[0] = spacedWords[0].ToLower();
            return (String.Join("", spacedWords)).Trim();
        }
    }
}
