
namespace <%= namespace %>
{
    /// <summary>
    ///  Represents a ServiceUnavailableException.
    /// </summary>
    public class NotAuthorizedException : System.Exception
    {
        /// <summary>
        /// Initializes a ServiceUnavailableException.
        /// </summary>
        public NotAuthorizedException()
        {
        }

        /// <summary>
        /// Initializes a ServiceUnavailableException with a message.
        /// </summary>
        /// <param name="message"></param>
        public NotAuthorizedException(string message) : base(message)
        {
        }
    }
}
