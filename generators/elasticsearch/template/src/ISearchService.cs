using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace <%= namespace %>
{
    /// <summary>
    /// The interface that defines the functionality for communicating with ElasticSearch.
    /// </summary>
    public interface ISearchService<T> where T : ElasticDoc
    {
        /// <summary>
        /// Gets counts of ElasticSearch document type grouping field, for a given query.
        /// </summary>
        /// <param name="query">The raw count query string.</param>
        /// <returns>An enumerable list of key/value pairs.</returns>
        Task<IEnumerable<DocumentCount>> GetCounts(string query);

        /// <summary>
        /// Creates the default ElasticSearch index.
        /// </summary>
        Task<ICreateIndexResponse> CreateIndex();

        /// <summary>
        /// Indexes elastic documents in bulk into Elastic Search.
        /// </summary>
        /// <param name="docs">The elastic documents to index.</param>
        Task<IBulkResponse> Index(IEnumerable<T> docs);

        /// <summary>
        /// Indexes an elastic document into Elastic Search.
        /// </summary>
        /// <param name="doc">The elastic document to index.</param>
        Task<IIndexResponse> Index(T doc);

        /// <summary>
        /// Delete ElasticSearch index.
        /// </summary>
        Task<IDeleteIndexResponse> DeleteIndex();

        /// <summary>
        /// Gets or sets the Claims
        /// </summary>
        IApplicationUser User { get; set; }
    }
}