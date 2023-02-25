using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;

namespace SpecFlowTest
{
    public class HttpClientWorker
    {
        private static readonly IHttpClientFactory factory;
        static HttpClientWorker()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            factory = container.Resolve<IHttpClientFactory>();
        }

        public async static Task<TOutput> Execute<TInput, TOutput>(string url, HttpMethod httpMethod, TInput input = default(TInput))
        {
            var httpClient = factory.CreateClient();
            var serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var request = new HttpRequestMessage(httpMethod, url);

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            using var textWriter = new JsonTextWriter(writer);

            serializer.Serialize(textWriter, input);
            textWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);
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
    }
}
