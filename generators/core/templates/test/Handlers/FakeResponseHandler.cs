using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace <%= namespace %>.Tests
{
    public class FakeResponseHandler : DelegatingHandler
    {
        public void AddFakeResponse()
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(string.Empty);

            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Get)
            {
                return Task.FromResult(response);
            }

            return Task.FromResult<HttpResponseMessage>(null);
        }
    }
}
