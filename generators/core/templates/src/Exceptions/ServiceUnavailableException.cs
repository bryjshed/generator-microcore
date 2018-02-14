namespace <%= namespace %>
{
    /// <summary>
    ///  Represents a ServiceUnavailableException.
    /// </summary>
    public class ServiceUnavailableException : System.Exception
    {
        /// <summary>
        /// Initializes a ServiceUnavailableException.
        /// </summary>
        public ServiceUnavailableException()
        {
        }

        /// <summary>
        /// Initializes a ServiceUnavailableException with a message.
        /// </summary>
        /// <param name="message"></param>
        public ServiceUnavailableException(string message) : base(message)
        {
        }
    }
}
