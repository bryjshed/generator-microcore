

namespace <%= namespace %>
{
    /// <summary>
    /// The configurations used for validating tokens and internal authentication.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// The url of the authentication application.
        /// </summary>
        public string OrganizationUrl { get; set; }

        /// <summary>
        /// The api client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The scopes of the authentication application.
        /// </summary>
        public string Scopes { get; set; }

        /// <summary>
        /// The response type of the authentication application.
        /// </summary>
        public string ResponseType { get; set; }

        /// <summary>
        /// The GracePeriod period used by the token validation.
        /// </summary>
        public int GracePeriod { get; set; }
    }
}
