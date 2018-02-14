using System;
using System.Collections.Generic;
using System.Linq;

namespace <%= namespace %>
{
    /// <summary>
    /// The application user mapped from the token claims.
    /// </summary>
    public class ApplicationUser
    {
        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The openId token.
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Gets or sets the DisplayName.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Groups.
        /// </summary>
        public List<string> Groups { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the ExpiresAt.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Validates if a user belongs to a certain group.
        /// </summary>
        public bool HasGroup(string group) => Groups.Any(x => string.Equals(x, group, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Checks if the ExpiresAt has passed. Also adds a grace period which will consider the token as expired.
        /// </summary>
        /// <param name="gracePeriod">The allowed grace period in minutes before the token is considered expired.</param>
        public bool IsExpired(int gracePeriod) => (ExpiresAt - DateTime.UtcNow) <= TimeSpan.FromMinutes(gracePeriod);
    }
}

