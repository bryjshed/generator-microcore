using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
using ThirdParty.Json.LitJson;

namespace <%= namespace %>
{
    /// <summary>
    ///  Controls <%= modelName.toLowerCase() %> db functionality.
    /// </summary>
    public class <%= modelName %>Repository : I<%= modelName %>Repository
    {
        private readonly IDynamoDBContext _context;

        private readonly ILogger _logger;

        private readonly Table _targetTable;

        /// <summary>
        /// Initializes a new <see cref="<%= modelName %>Repository"/>.
        /// </summary>
        /// <param name="logger">The logger handler to use.</param>
        /// <param name="context">The DynamoDbContext to use.</param>
        public <%= modelName %>Repository(
            ILogger<<%= modelName %>Repository> logger,
            IDynamoDBContext context)
        {
            _logger = logger;
            _context = context;
            _targetTable = _context.GetTargetTable<<%= modelName %>>();
        }

        /// <summary>
        /// Saves a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="model">The <%= modelName.toLowerCase() %> to save.</param>
        /// <returns>The saved <%= modelName.toLowerCase() %>.</returns>
        public Task SaveAsync(<%= modelName %> model)
        {
            _logger.LogInformation("Save <%= modelName %>: {@<%= modelName %>}", model);
            model.Id = Guid.NewGuid().ToString();
            return _context.SaveAsync(model);
        }

        /// <summary>
        /// Updates an existing <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="model">The <%= modelName.toLowerCase() %> to update.</param>
        public Task UpdateAsync(<%= modelName %> model)
        {
            _logger.LogInformation("Update <%= modelName %>: {@<%= modelName %>}", model);
            return _context.SaveAsync(model);
        }

        /// <summary>
        /// Gets a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The id of the <%= modelName.toLowerCase() %> to retrieve.</param>
        /// <returns>The requested <%= modelName.toLowerCase() %>.</returns>
        public Task<<%= modelName %>> GetByIdAsync(string id)
        {
            _logger.LogInformation("Get <%= modelName %> By Id: {Id}", id);
            return _context.LoadAsync<<%= modelName %>>(id);
        }

        /// <summary>
        /// Scan for <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="conditions">The conditions to scan by.</param>
        /// <param name="lastId">The last id to be retrieved.</param>
        /// <param name="pageSize">The max number of records to be retrieved.</param>
        public async Task<IQueryable<<%= modelName %>>> FindAsync(IEnumerable<ScanCondition> conditions, string lastId, int? pageSize)
        {
            _logger.LogInformation("Get <%= modelName %>s: {LastId} {PageSize}", lastId, pageSize);
            try
            {
                ScanOperationConfig scanOpConfig = new ScanOperationConfig()
                {
                    PaginationToken = CreatePaginationToken(lastId)
                };

                var scan = _targetTable.Scan(scanOpConfig);

                if (pageSize.HasValue)
                {
                    scanOpConfig.Limit = pageSize.Value;
                }

                List<Document> reqs = await scan.GetNextSetAsync();

                return _context.FromDocuments<<%= modelName %>>(reqs).AsQueryable();
            }
            catch (JsonException e)
            {
                throw e;
            }
        }

        private string CreatePaginationToken(string key)
        {
            return key != null ? "{\"Id\":{\"S\":\"" + key + "\"}}" : null;
        }

        private string CreateIndexPaginationToken(string key, string index, string search)
        {
            return key != null && index != null && search != null ? "{\"Id\":{\"S\":\"" + key + "\"},\"" + index + "\":{\"N\":\"" + search + "\"}}" : null;
        }
    }
}
