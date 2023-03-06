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

        public async Task<IEnumerable<TestTrade>> GetTrades()
        {
            return await HttpClientWorker.Execute<IEnumerable<TestTrade>>($"{BaseAddress}/trades", HttpMethod.Get);
        }
        public async Task<TestTrade> GetTradeById(long id)
        {
            return await HttpClientWorker.Execute<long, TestTrade>($"{BaseAddress}/trades", HttpMethod.Get, id);
        }
        public Task CancelAllOrders()
        {
            return HttpClientWorker.Execute($"{BaseAddress}/orders", HttpMethod.Delete);
        }
        public Task CancelOrder(long id)
        {
            return HttpClientWorker.Execute($"{BaseAddress}/orders/{id}", HttpMethod.Delete);
        }
        public async Task<TestOrder> GetOrderById(long id)
        {
            return await HttpClientWorker.Execute<TestOrder>($"{BaseAddress}/orders/{id}", HttpMethod.Get);
        }
        public async Task<ProcessedOrderVM> ProcessOrder(RegisterOrderVM order)
        {
            return await HttpClientWorker.Execute<RegisterOrderVM, ProcessedOrderVM>($"{BaseAddress}/orders", HttpMethod.Post, order);
        }

        public async Task<ProcessedOrderVM> ModifyOrder(ModifiedOrderVM order)
        {
            return await HttpClientWorker.Execute<ModifiedOrderVM, ProcessedOrderVM>($"{BaseAddress}/orders", HttpMethod.Put, order);
        }
    }
}
