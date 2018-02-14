using System.Collections.Generic;
using System.Threading.Tasks;

namespace <%= namespace %>
{
    /// <summary>
    /// Defines the functionality for a I<%= modelName %>Repository.
    /// </summary>
    public interface I<%= modelName %>Repository
    {
        /// <summary>
        /// Saves a <%= modelName.toLowerCase() %>, asyncronously.
        /// </summary>
        /// <param name="model">The <%= modelName.toLowerCase() %> model to save.</param>
        Task<int> SaveAsync(<%= modelName %> model);

        /// <summary>
        /// Updates the <%= modelName.toLowerCase() %> model, asyncronously.
        /// </summary>
        /// <param name="model">The <%= modelName.toLowerCase() %> model to update.</param>
        Task<int> UpdateAsync(<%= modelName %> model);

        /// <summary>
        /// Gets a <%= modelName.toLowerCase() %> based on the id, asyncronously.
        /// </summary>
        /// <param name="id">The id to search by.</param>
        Task<<%= modelName %>> GetByIdAsync(long id);

        /// <summary>
        /// Get a list of all <%= modelName.toLowerCase() %>.
        /// </summary>
        Task<List<<%= modelName %>>> ListAsync();

        /// <summary>
        /// Search results.
        /// </summary>
        /// <param name="query">The term to query on.</param>
        /// <param name="pageNumber">The number of the page to return.</param>
        /// <param name="pageSize">The size of the page to return.</param>
        Task<PaginatedList<<%= modelName %>>> FindAsync(
            string query,
            int pageNumber,
            int pageSize);
    }
}
