using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace <%= namespace %>
{
    /// <summary>
    /// The configuration for the <%= appname %>Service.
    /// </summary>
    public class <%= serviceName %>ServiceConfig
    {
        /// <summary>
        /// Gets or sets the EndPoint for the service.
        /// </summary>
        public string EndPoint { get; set; }
    }

    /// <summary>
    /// Represents the <%= serviceName %>Service.
    /// </summary>
    public class <%= serviceName %>Service : BaseService, I<%= serviceName %>Service
    {
        private readonly <%= serviceName %>ServiceConfig _serviceConfig;

        /// <summary>
        /// Initializes the <%= serviceName %>Service.
        /// </summary>
        /// <param name="logger">The log handler for the service.</param>
        /// <param name="serviceConfig">The service config settings <see cref="<%= serviceName %>ServiceConfig"/></param>
        /// <param name="appSettings">The app settings <see cref="AppSettings"/></param>
        /// <param name="httpClient">The httpclient used.</param>
        public <%= serviceName %>Service(ILogger<<%= serviceName %>Service> logger,
                                 <%= serviceName %>ServiceConfig serviceConfig,
                                 AppSettings appSettings,
                                 HttpClient httpClient)
                                 : base(logger, appSettings, httpClient)
        {
            _serviceConfig = serviceConfig;
        }

        /// <summary>
        /// Gets a <%= serviceName.toLowerCase() %> by id.
        /// </summary>
        /// <param name="id">The id to search by.</param>
        /// <param name="idToken">The bearer authentication token.</param>
        public async Task<ServiceResponse<<%= serviceDtoName %>Dto>> Get(long id, string idToken)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            string url = $"{_serviceConfig.EndPoint}<%= serviceName.toLowerCase() %>/{id}";
            _logger.LogDebug($"Making request to {url}");
            var request = await RetryAsync(async () => await HttpClient.GetAsync(url));
            return await ProcessResponseAsync<<%= serviceDtoName %>Dto>(request, "<%= serviceName.toLowerCase() %>");
        }

        /// <summary>
        /// Save a <%= serviceName.toLowerCase() %>.
        /// </summary>
        /// <param name="dto">The <%= serviceName.toLowerCase() %> to save.</param>
        /// <param name="idToken">The bearer authentication token.</param>
        public async Task<ServiceResponse<<%= serviceDtoName %>Dto>> Save(<%= serviceDtoName %>Dto dto, string idToken)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            string url = $"{_serviceConfig.EndPoint}<%= serviceName.toLowerCase() %>/";
            _logger.LogDebug($"Making request to {url}");
            var request = await RetryAsync(async () =>
            {
                var jsonString = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return await HttpClient.PostAsync(url, content);
            });

            return await ProcessResponseAsync<<%= serviceDtoName %>Dto>(request, "<%= serviceDtoName.toLowerCase() %>");
        }

        /// <summary>
        /// Update a <%= serviceName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The id to update.</param>
        /// <param name="dto">The <%= serviceName.toLowerCase() %> to update.</param>
        /// <param name="idToken">The bearer authentication token.</param>
        public async Task<ServiceResponse<<%= serviceDtoName %>Dto>> Update(long id, <%= serviceDtoName %>Dto dto, string idToken)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            string url = $"{_serviceConfig.EndPoint}<%= serviceName.toLowerCase() %>/{id}";
            _logger.LogDebug($"Making request to {url}");
            var request = await RetryAsync(async () =>
            {
                var jsonString = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return await HttpClient.PutAsync(url, content);
            });

            return await ProcessResponseAsync<<%= serviceDtoName %>Dto>(request, "<%= serviceDtoName.toLowerCase() %>");
        }
    }


}
