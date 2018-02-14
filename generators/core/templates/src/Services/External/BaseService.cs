using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Polly;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents an abstract BaseService.
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// The retry policy of the service.
        /// </summary>
        protected readonly Policy<HttpResponseMessage> _retryPolicy;

        /// <summary>
        /// The logger for the service.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// The HttpClient for the service.
        /// </summary>
        public HttpClient HttpClient;

        /// <summary>
        /// Initializes a BaseService object.
        /// </summary>
        /// <param name="logger">The logger object for the service.</param>
        /// <param name="appSettings">The settings for the application.</param>
        /// <param name="httpClient">The httpclient used.</param>
        public BaseService(ILogger logger,
           AppSettings appSettings,
           HttpClient httpClient)
        {
            _logger = logger;
            HttpStatusCode[] httpStatusCodesWorthRetrying =
            {
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.BadGateway,
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.GatewayTimeout
                };
            _retryPolicy = Polly.Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .RetryAsync(appSettings.BreakOnNumberOfExceptions);
            HttpClient = httpClient;
        }


        /// <summary>
        /// Handles the async retry for the service.
        /// </summary>
        /// <param name="action">The action to handle.</param>
        /// <returns>The http response for the service.</returns>
        protected Task<HttpResponseMessage> RetryAsync(Func<Task<HttpResponseMessage>> action)
        {
            try
            {
                return _retryPolicy.ExecuteAsync(action);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Policy retry service connection issue: {Exception}", ex);
                throw new ServiceUnavailableException("Unable to connect to service");
            }
        }

        /// <summary>
        /// Process the http message response for external service calls.
        /// </summary>
        /// <param name="request">The http response message.</param>
        /// <param name="entityName">The name of the entity.</param>
        /// <returns>The http response for the service.</returns>
        public async Task<ServiceResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage request, string entityName)
        {
            var response = new ServiceResponse<T>();
            response.StatusCode = request.StatusCode;
            var message = await request.Content.ReadAsStringAsync();
            if (request.IsSuccessStatusCode)
            {
                response.Result = JsonConvert.DeserializeObject<T>(
                    message
                );
                response.IsSuccess = true;

            }
            else switch (request.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    response.AddErrors(JsonConvert.DeserializeObject<Dictionary<string, IList<string>>>(
                        message
                    ));
                    break;
                case HttpStatusCode.NotFound:
                    response.AddError(entityName, $"The {entityName} was not found.");
                    break;
                case HttpStatusCode.Forbidden:
                    throw new NotAuthorizedException($"The {entityName} was Forbidden.");
                default:
                    throw new HttpRequestException("There was an error processing this request");
            }
            return response;
        }
    }
}