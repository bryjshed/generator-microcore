using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace <%= namespace %>.Tests
{
    public class BaseServiceStub : BaseService
    {
        public BaseServiceStub(ILogger logger, AppSettings appSettings, HttpClient httpClient) : base(logger, appSettings, httpClient)
        {
        }
    }
}