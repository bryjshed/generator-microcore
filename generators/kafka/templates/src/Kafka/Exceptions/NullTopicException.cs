namespace <%= namespace %>
{
    /// <summary>
    /// Represents a NullTopicException.
    /// </summary>
    public class NullTopicException : System.Exception
    {
        /// <summary>
        /// Initializes a ServiceUnavailableException.
        /// </summary>
        public NullTopicException()
        {
        }

        /// <summary>
        /// Initializes a ServiceUnavailableException with the specified message.
        /// </summary>
        /// <param name="message">The message to be included in the exception.</param>
        public NullTopicException(string message) : base(message)
        {
        }
    }
}