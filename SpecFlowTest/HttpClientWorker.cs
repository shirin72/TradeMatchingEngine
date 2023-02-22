using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;

namespace SpecFlowTest
{
    public class HttpClientWorker : IDisposable
    {
        private static readonly HttpClient httpClient;
        static HttpClientWorker()
        {
            httpClient = new HttpClient();
        }

        public async static Task<TOutput> Execute<TInput, TOutput>(string url, TInput input, HttpMethod httpMethod)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            using var textWriter = new JsonTextWriter(writer);
            var serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            serializer.Serialize(textWriter, input);
            textWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            var request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var requestContent = new StreamContent(ms);
            request.Content = requestContent;
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(content);
            using var textReader = new JsonTextReader(reader);

            return serializer.Deserialize<TOutput>(textReader);
        }


        public async static Task Execute(string url, HttpMethod httpMethod)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            using var textWriter = new JsonTextWriter(writer);

            var serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            serializer.Serialize(textWriter, string.Empty);
            textWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var requestContent = new StreamContent(ms);
            request.Content = requestContent;

            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(content);
            using var textReader = new JsonTextReader(reader);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
