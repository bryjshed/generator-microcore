using Serilog.Events;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the logging level.
        /// </summary>
        public LogEventLevel LoggingLevel { get; set; } = LogEventLevel.Debug;

        /// <summary>
        /// Gets or sets GlobalExceptionFilterEnabled for the MVC configuration.
        /// </summary>
        public bool GlobalExceptionFilterEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the acceptable number of exceptions.
        /// </summary>
        public int BreakOnNumberOfExceptions { get; set; } = 3;
    }
}
