using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace <%= namespace %>
{
    /// <summary>
    /// The service config used for <see cref="SearchService{T}"/>.
    /// </summary>
    public class SearchServiceConfig
    {
        /// <summary>
        /// Gets or sets the IndexName field.
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Gets or sets the EndPoint field.
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// The Username for Elastic Search BasicAuthentication.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The Password for Elastic Search BasicAuthentication.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// This class implements the functionality needed to communicate with ElasticSearch.
    /// </summary>    
    public abstract class SearchService<T> : ISearchService<T> where T : ElasticDoc
    {
        /// <summary>
        /// The log handler for the controller.
        /// </summary>
        protected readonly ILogger _logger;

        private readonly SearchServiceConfig _serviceConfig;

        private IElasticClient _client;

        private IElasticLowLevelClient _lowLevelClient;

        /// <summary>
        /// Gets or sets the Claims
        /// </summary>
        public IApplicationUser User { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService{T}"/> class.
        /// </summary>
        /// <param name="logger">The log handler for the controller.</param>
        /// <param name="serviceConfig">The service config settings <see cref="SearchServiceConfig"/></param>
        /// <param name="client">Interface to high level ElasticSearch client.</param>
        /// <param name="lowLevelClient">Interface to low level ElasticSearch client.</param>
        public SearchService(
            ILogger logger,
            SearchServiceConfig serviceConfig,
            IElasticClient client,
            IElasticLowLevelClient lowLevelClient)
        {
            _logger = logger;
            _serviceConfig = serviceConfig;
            _client = client;
            _lowLevelClient = lowLevelClient;
        }

        /// <summary>
        /// Search ElasticSearch for containting the given search term.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="sort">The sort selector.</param>
        /// <param name="pageNumber">The starting page number of the search results from which to take.</param>
        /// <param name="pageSize">The number of items to take.</param>
        protected virtual async Task<SearchResultsDto<T>> Search(
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort,
            int? pageNumber,
            int? pageSize)
        {
            var searchResults = await _client.SearchAsync<T>(s => s
                .Query(query)
                .Sort(sort)
                .From((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value)
            );

            if (!searchResults.IsValid)
            {
                throw new Exception($"Unable to execute search due to errors in query: {JsonConvert.SerializeObject(searchResults.ServerError)}");
            }

            var resultsDto = new SearchResultsDto<T>
            {
                Docs = searchResults.Documents.ToList(),
                TotalCount = await GetTotalCount(practiceId),
                FilteredCount = searchResults.HitsMetaData.Total
            };

            return resultsDto;
        }

        /// <summary>
        /// Gets counts of ElasticSearch document type grouping field, for a given query.
        /// </summary>
        /// <param name="query">The raw count query string.</param>
        /// <returns>An enumerable list of key/value pairs.</returns>
        public async Task<IEnumerable<DocumentCount>> GetCounts(string query)
        {
            var response = await _lowLevelClient.SearchAsync<string>(query);

            if (!response.Success)
            {
                throw new Exception($"Unable to execute search due to errors in query: {JsonConvert.SerializeObject(response.ServerError)}");
            }

            var oResponse = JObject.Parse(response.Body);
            var sBuckets = JsonConvert.SerializeObject(oResponse.Last.First.First.First.Last.First);
            var buckets = JsonConvert.DeserializeObject<List<DocumentCount>>(sBuckets);
            return buckets;
        }

        /// <summary>
        /// Creates the default ElasticSearch index.
        /// </summary>
        public async Task<ICreateIndexResponse> CreateIndex()
        {
            if (!(await _client.IndexExistsAsync(_serviceConfig.IndexName)).Exists)
            {
                return await _client.CreateIndexAsync(_serviceConfig.IndexName, cid => cid
                    .Mappings(md => SearchConstants.Mappings(md))
                );
            }

            return null;
        }

        /// <summary>
        /// Delete ElasticSearch index.
        /// </summary>
        public async Task<IDeleteIndexResponse> DeleteIndex()
        {
            if ((await _client.IndexExistsAsync(_serviceConfig.IndexName)).Exists)
            {
                return await _client.DeleteIndexAsync(_serviceConfig.IndexName);
            }

            return null;
        }

        /// <summary>
        /// Inserts documents in bulk into Elastic Search.
        /// </summary>
        /// <param name="docs">The documents to insert.</param>
        public virtual async Task<IBulkResponse> Index(IEnumerable<T> docs)
        {
            var request = new BulkDescriptor();

            foreach (var req in docs)
            {
                var reqES = PrepareDoc(req);
                request.Index<T>(op => op
                    .Id(reqES.Id)
                    .Index(_serviceConfig.IndexName)
                    .Document(reqES)
                );
            }

            return await _client.BulkAsync(request);
        }

        /// <summary>
        /// Indexes a single document within Elastic Search.
        /// </summary>
        /// <param name="doc">The document to insert.</param>
        public async Task<IIndexResponse> Index(T doc)
        {
            var documentEs = PrepareDoc(doc);

            return await _client.IndexAsync<T>(documentEs, op => op
                .Id(documentEs.Id)
                .Index(_serviceConfig.IndexName)
            );
        }


        /// <summary>
        /// Updates an Elastic Search document.
        /// </summary>
        /// <param name="updateDoc">The elastic document to update.</param>
        public async Task<IUpdateResponse<T>> UpdateDoc(dynamic updateDoc)
        {
            var response = await _client.UpdateAsync<T, dynamic>(new DocumentPath<T>(updateDoc.Id), u => u
                .Index(_serviceConfig.IndexName)
                .Doc(updateDoc)
             );

            return response;
        }

        /// <summary>
        /// Prepares the ElasticDoc prior to index.
        /// </summary>
        /// <param name="doc">The ElasticDoc document to be indexed.</param>
        /// <returns>A prepared ElasticDoc.</returns>
        protected virtual T PrepareDoc(T doc)
        {
            return doc;
        }

        /// <summary>
        /// Fetches total count base on practiceId
        /// </summary>
        /// <param name="practiceId">PracticeId which can be null.</param>
        /// <returns>Total count for either all records or based on practiceId</returns>
        protected virtual async Task<long> GetTotalCount(long? practiceId)
        {
            var countResult = await _client.CountAsync<T>(GetTotalCountQuery(practiceId));
            if (!countResult.IsValid)
            {
                throw new Exception($"Unable to execute count search due to errors in query: {JsonConvert.SerializeObject(countResult.ServerError)}");
            }
            return countResult.Count;
        }
    }
}