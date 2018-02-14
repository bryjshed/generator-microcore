using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace <%= namespace %>
{
    /// <summary>
    /// Provides global exception catching functionality.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        ILogger _logger;

        /// <summary>
        /// Creates an instance of the <see cref="GlobalExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public GlobalExceptionFilter(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("Global Exception Filter");
        }

        /// <summary>
        /// Processes caught exception and sets ObjectResult response with error message, stack trace and status code (500).
        /// </summary>
        /// <param name="context">An instance of <see cref="ExceptionContext"/> class.</param>
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(1013), context.Exception, "Unhandlding exception caught by GlobalExceptionFilter");

            var response = new ErrorResponse()
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
                                .Replace("\n", "").Replace("  ", "")
                                .Split(new[] { " at " }, StringSplitOptions.RemoveEmptyEntries)
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                DeclaredType = typeof(ErrorResponse)
            };
        }
    }

    /// <summary>
    /// This represents the response entity for error.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        public string[] StackTrace { get; set; }
    }
}
