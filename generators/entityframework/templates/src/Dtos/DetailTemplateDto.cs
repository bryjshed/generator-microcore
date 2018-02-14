using System;
using System.ComponentModel.DataAnnotations;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <%= modelName.toLowerCase() %> information.
    /// </summary>
    public class <%= modelName %>Dto
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the CreationDate.
        /// </summary>
        public string CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the LastUpdatedDate.
        /// </summary>
        public string LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the CreatedById.
        /// </summary>
        public string CreatedById { get; set; }

          /// <summary>
        /// Gets or sets the CreatedByDisplayName.
        /// </summary>
        public string CreatedByDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedById.
        /// </summary>
        public string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedByDisplayName.
        /// </summary>
        public string UpdatedByDisplayName { get; set; }
    }
}
