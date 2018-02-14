using System;
using System.Globalization;

namespace <%= namespace %>
{
    /// <summary>
    /// Extension methods for Datetime.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Format a nullable datetime.
        /// </summary>
        /// <param name="dateTime">The date.</param>
        /// <param name="format">The format.</param>
        public static string FormatDate(this DateTime? dateTime, string format)
        {
            return dateTime?.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Format a datetime.
        /// </summary>
        /// <param name="dateTime">The date.</param>
        /// <param name="format">The format.</param>
        public static string FormatDate(this DateTime dateTime, string format)
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}