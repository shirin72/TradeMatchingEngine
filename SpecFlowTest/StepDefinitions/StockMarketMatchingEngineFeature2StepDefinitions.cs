using EndPoints.Model;
using Newtonsoft.Json;
using System;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature2StepDefinitions:Steps
    {
        private FeatureContext context;
        private readonly HttpClient httpClient;


        public StockMarketMatchingEngineFeature2StepDefinitions(FeatureContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();
        }

        [Given(@"Order '([^']*)' Has Been Defined")]
        public void GivenSellOrderHasBeenDefined(string order, Table table)
        {
            context.Add(order, table);
        }

        [When(@"I Register The Order '([^']*)'")]
        public async Task WhenIRegisterTheSellOrder(string order)
        {
            var orderVm = context.Get<Table>(order).CreateInstance<OrderVM>();

            var requestBody = new StringContent(JsonConvert.SerializeObject(orderVm), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync("https://localhost:7092/api/Order/ProcessOrder", requestBody).GetAwaiter().GetResult();
            context.Add($"{order}Response", JsonConvert.DeserializeObject<ProcessedOrder>(await result.Content.ReadAsStringAsync()));
        }


        [Then(@"Order '([^']*)' Should Be Enqueued")]
        public async Task ThenOrderShouldBeEnqueuedAsync(string order)
        {
            var result = context.Get<ProcessedOrder>($"{order}Response").OrderId;
            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());
            orderDeserialize.Id.Should().Be(result);
        }

    }
}
