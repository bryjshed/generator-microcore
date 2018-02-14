using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents a <%= modelName.toLowerCase() %>.
    /// </summary>
    [DynamoDBTable("<%= modelName.toLowerCase() %>")]
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
        /// Gets or sets the Id of the <%= modelName.toLowerCase() %>.
        /// </summary>
        [DynamoDBHashKey]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the CreationDate for the <%= modelName.toLowerCase() %>.
        /// </summary>
        [DynamoDBProperty]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the LastUpdatedDate.
        /// </summary>
        [DynamoDBProperty]
        public DateTime LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the CreatedById.
        /// </summary>
        [DynamoDBProperty]
        public string CreatedById { get; set; }

         /// <summary>
        /// Gets or sets the CreatedByDisplayName.
        /// </summary>
        [DynamoDBProperty]
        public string CreatedByDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedById.
        /// </summary>
        [DynamoDBProperty]
        public string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByDisplayName.
        /// </summary>
        [DynamoDBProperty]
        public string UpdatedByDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the VersionNumber for the <%= modelName.toLowerCase() %>.
        /// </summary>
        [DynamoDBVersion]
        public long? VersionNumber { get; set; }
    }
}
