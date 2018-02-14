using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace <%= namespace %>.Tests
{
    public static class HttpContentExtensions
    {
        public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, object model)
        {
            var jsonString = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            return await client.PostAsync(requestUri, content);
        }

        public static async Task<HttpResponseMessage> PutAsync(this HttpClient client, string requestUri, object model)
        {
            var jsonString = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            return await client.PutAsync(requestUri, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var response = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(
                response
            );
        }
    }
}
