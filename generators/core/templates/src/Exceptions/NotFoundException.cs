namespace <%= namespace %>
{
    /// <summary>
    ///  Represents a ServiceUnavailableException.
    /// </summary>
    public class NotFoundException: System.Exception
    {
        /// <summary>
        /// Initializes a ServiceUnavailableException.
        /// </summary>
        public NotFoundException()
        {
        }

        /// <summary>
        /// Initializes a ServiceUnavailableException with a message.
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message) : base(message)
        {
        }
    }
}