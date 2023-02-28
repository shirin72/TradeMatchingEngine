using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace SpecFlowTest
{

    public static class HttpClientWorker
    {
        static ConcurrentDictionary<string, ICircuitBreaker> connections =
            new ConcurrentDictionary<string, ICircuitBreaker>();

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

        public async static Task Execute(string url, HttpMethod httpMethod)
        {
            await Execute<object>(url, httpMethod);
        }
        public async static Task<TOutput> Execute<TOutput>(string url, HttpMethod httpMethod)
        {
            return await Execute<object, TOutput>(url, httpMethod, null);
        }
        public async static Task<TOutput> Execute<TInput, TOutput>(string url, HttpMethod httpMethod, TInput input = default(TInput))
        {
            var cb = connections.First(i => url.StartsWith(i.Key)).Value;
            return await cb.ExecuteService(input, i => execute<TInput, TOutput>(url, httpMethod, i));
        }

        private async static Task<TOutput> execute<TInput, TOutput>(string url, HttpMethod httpMethod, TInput input = default(TInput))
        {
            var httpClient = factory.CreateClient();
            var serializer = new JsonSerializer() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            MemoryStream? ms = null;
            StreamWriter? writer = null;
            JsonTextWriter? textWriter = null;
            StreamContent? requestContent = null;
            try
            {
                if (httpMethod != HttpMethod.Get)
                {
                    ms = new MemoryStream();
                    writer = new StreamWriter(ms);
                    textWriter = new JsonTextWriter(writer);

                    serializer.Serialize(textWriter, input);
                    textWriter.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    requestContent = new StreamContent(ms);

                    request.Content = requestContent;

                    requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(content);
                using var textReader = new JsonTextReader(reader);

                return serializer.Deserialize<TOutput>(textReader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ms?.Dispose();
                writer?.Dispose();
                ((IDisposable)textWriter)?.Dispose();
                requestContent?.Dispose();
            }
        }

        internal static void AddConnection(string baseAddress)
        {
            connections.AddOrUpdate(baseAddress, ba => new CircuitBreaker(), (ba, ov) => ov);
        }
    }
}
