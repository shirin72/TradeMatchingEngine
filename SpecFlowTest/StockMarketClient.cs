using Application.Tests;
using EndPoints.Controller;
using EndPoints.Model;

namespace SpecFlowTest
{
    public class StockMarketClient
    {
        public StockMarketClient(string baseAddress)
        {
            BaseAddress = baseAddress;
            HttpClientWorker.AddConnection(baseAddress);
        }

        public string BaseAddress { get; }

        public Task<IEnumerable<TestTrade>> GetTrades()
        {
            return HttpClientWorker.Execute<IEnumerable<TestTrade>>($"{BaseAddress}/trades", HttpMethod.Get);
        }
        public Task<TestTrade> GetTradeById(long id)
        {
            return HttpClientWorker.Execute<long, TestTrade>($"{BaseAddress}/trades", HttpMethod.Get, id);
        }
        public Task CancelAllOrders()
        {
            return HttpClientWorker.Execute($"{BaseAddress}/orders", HttpMethod.Delete);
        }
        public Task CancelOrder(long id)
        {
            return HttpClientWorker.Execute($"{BaseAddress}/orders/{id}", HttpMethod.Delete);
        }
        public Task<TestOrder> GetOrderById(long id)
        {
            return HttpClientWorker.Execute<TestOrder>($"{BaseAddress}/orders/{id}", HttpMethod.Get);
        }
        public Task<TestProcessedOrder> ProcessOrder(OrderVM order)
        {
            return HttpClientWorker.Execute<OrderVM, TestProcessedOrder>($"{BaseAddress}/orders", HttpMethod.Post, order);
        }

        public Task<long> ModifyOrder(ModifiedOrderVM order)
        {
            return HttpClientWorker.Execute<ModifiedOrderVM, long>($"{BaseAddress}/orders", HttpMethod.Put, order);
        }
    }
}
