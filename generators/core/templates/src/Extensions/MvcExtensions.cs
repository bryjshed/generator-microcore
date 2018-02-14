using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace <%= namespace %>
{
    /// <summary>
    /// Extension methods for mvc
    /// </summary>
    public static class MvcExtensions
    {
        /// <summary>
        /// Extension method for adding model errros.
        /// </summary>
        /// <param name="value">The model state dictionary.</param>
        /// <param name="errors">The errors to be added.</param>
        public static void AddModelErrors(this ModelStateDictionary value, IDictionary<string, IList<string>> errors)
        {
            AddModelErrors(value, errors, null);
        }

        /// <summary>
        /// Extension method for adding model errros.
        /// </summary>
        /// <param name="value">The model state dictionary.</param>
        /// <param name="errors">The errors to be added.</param>
        /// <param name="prefix">If a prfix is required. Such as patient.patiendId</param>
        public static void AddModelErrors(this ModelStateDictionary value, IDictionary<string, IList<string>> errors, string prefix = "")
        {
            if (errors?.Count > 0)
            {
                foreach (KeyValuePair<string, IList<string>> pair in errors)
                {
                    string key = prefix + pair.Key;
                    foreach (string val in pair.Value)
                    {
                        value.AddModelError(key, val);
                    }
                }
            }
        }
    }
}
