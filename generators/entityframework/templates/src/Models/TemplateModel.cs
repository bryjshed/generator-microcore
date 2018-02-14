using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <%= modelName.toLowerCase() %> information.
    /// </summary>
    [Table("<%= modelName.toLowerCase() %>")]
    public class <%= modelName %>
    {
        /// <summary>
        /// <%= modelName %> constructor.
        /// </summary>
        public <%= modelName %>()
        {
            var date = DateTime.UtcNow;
            CreationDate = date;
            LastUpdatedDate = date;
        }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the CreationDate.
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the LastUpdatedDate.
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the CreatedById.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CreatedById { get; set; }

         /// <summary>
        /// Gets or sets the CreatedByDisplayName.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CreatedByDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedById.
        /// </summary>
        [MaxLength(100)]
        public string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByDisplayName.
        /// </summary>
        [MaxLength(100)]
        public string UpdatedByDisplayName { get; set; }
    }

}
