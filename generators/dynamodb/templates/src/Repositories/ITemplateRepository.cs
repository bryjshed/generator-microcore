using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace <%= namespace %>
{
    /// <summary>
    ///  Controls <%= modelName.toLowerCase() %> db functionality.
    /// </summary>
    public interface I<%= modelName %>Repository
    {
         /// <summary>
        /// Saves a <%= modelName.toLowerCase() %>.
        /// </summary>
        Task SaveAsync(<%= modelName %> model);

        /// <summary>
        /// Updates a <%= modelName.toLowerCase() %>.
        /// </summary>
        Task UpdateAsync(<%= modelName %> model);

        /// <summary>
        /// Gets a <%= modelName.toLowerCase() %>.
        /// </summary>
        Task<<%= modelName %>> GetByIdAsync(string id);

        /// <summary>
        /// Scans for a <%= modelName.toLowerCase() %>.
        /// </summary>
        Task<IQueryable<<%= modelName %>>> FindAsync(IEnumerable<ScanCondition> conditions, string lastId, int? pageSize);
    }
}
