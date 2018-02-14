using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace <%= namespace %>
{
    /// <summary>
    ///  Controls <%= modelName.toLowerCase() %> db functionality.
    /// </summary>
    public class <%= modelName %>Repository : I<%= modelName %>Repository
    {
        private readonly ILogger _logger;

        private readonly <%= appname %>Context _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="<%= modelName %>Repository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="<%= modelName %>"/> database context.</param>
        /// <param name="logger">The log handler for the controller.</param>
        public <%= modelName %>Repository(<%= appname %>Context context, ILogger<<%= modelName %>Repository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Save a new <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="model"></param>
        public Task<int> SaveAsync(<%= modelName %> model)
        {
            _logger.LogInformation("Save <%= modelName %>: {@<%= modelName %>}", model);
            _context.<%= modelName %>s.Add(model);
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="model"><%= modelName %> to be saved.</param>
        public Task<int> UpdateAsync(<%= modelName %> model)
        {
            _logger.LogInformation("Update <%= modelName %>: {@<%= modelName %>}", model);
            _context.Entry(model).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get a <%= modelName.toLowerCase() %> from dbcontext.
        /// </summary>
        /// <param name="id">The id key for <%= modelName.toLowerCase() %>.</param>
        public Task<<%= modelName %>> GetByIdAsync(long id)
        {
            _logger.LogInformation("Get <%= modelName %>: {Id}", id);
            return _context.<%= modelName %>s
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Get a list of all <%= modelName.toLowerCase() %>.
        /// </summary>
        public Task<List<<%= modelName %>>> ListAsync()
        {
            _logger.LogInformation("Get List of <%= modelName %>");
            return _context.<%= modelName %>s.ToListAsync();
        }

        /// <summary>
        /// A paginated list of search results.
        /// </summary>
        /// <param name="query">The term to query on.</param>
        /// <param name="pageNumber">The number of the page to return.</param>
        /// <param name="pageSize">The size of the page to return.</param>
        public async Task<PaginatedList<<%= modelName %>>> FindAsync(
            string query,
            int pageNumber,
            int pageSize)
        {
            _logger.LogInformation("List <%= modelName %>s");

            IQueryable<<%= modelName %>> queryable = _context.<%= modelName %>s;

            var totalRecords = await queryable.CountAsync();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(x =>
                    x.CreatedById.Equals(query, StringComparison.OrdinalIgnoreCase) ||
                    x.CreatedByDisplayName.Equals(query, StringComparison.OrdinalIgnoreCase) ||
                    x.UpdatedById.Equals(query, StringComparison.OrdinalIgnoreCase) ||
                    x.UpdatedByDisplayName.Equals(query, StringComparison.OrdinalIgnoreCase));
            }

            queryable = queryable
                .OrderByDescending(p => p.LastUpdatedDate);

            return await PaginatedList<<%= modelName %>>.CreateAsync(queryable, pageNumber, pageSize, totalRecords);
        }
    }
}
