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
    public class StockMarketMatchingEngineFeature3StepDefinitions : Steps
    {
        private readonly FeatureContext context;
        private readonly HttpClient httpClient;

        public StockMarketMatchingEngineFeature3StepDefinitions(FeatureContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();

        }

        [Given(@"Order '([^']*)' Has Been Registerd")]
        public void GivenOrderHasBeenRegisterd(string order, Table table)
        {
            var table1 = new Table(new string[] { "Side", "Price", "Amount", "IsFillAndKill", "ExpireTime" });
            table1.AddRow(new string[] { "0", "100", "5", "false", "2024-02-05 09:30:26.2080000" });
            Given("Order 'SellOrder' Has Been Defined", table1);
            When("I Register The Order 'SellOrder'");
            Then("Order 'SellOrder' Should Be Enqueued");

            context.Add(order, table);

        }

        [Then(@"The following trade will be created by ([^']*)")]
        public async Task ThenTheFollowingTradeWillBeCreated(string order,Table table)
        {
            var orderVm = context.Get<Table>(order).CreateInstance<OrderVM>();

            var requestBody = new StringContent(JsonConvert.SerializeObject(orderVm), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync("https://localhost:7092/api/Order/ProcessOrder", requestBody).GetAwaiter().GetResult();
            context.Add($"{order}Response", Convert.ToInt64(result.Content.ReadAsStringAsync().Result));
        }

        [Then(@"Order '([^']*)' Should Be Modified like this")]
        public async Task ThenOrderShouldBeModifiedLikeThis(string order, Table table)
        {
            var result = context.Get<long>("response");
            var expected = context.Get<Table>(order).CreateInstance<Order>();

            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());

            orderDeserialize.Id.Should().Be(result);

            orderDeserialize.Amount.Should().Be(expected.Amount);
            orderDeserialize.Price.Should().Be(expected.Price);
        }


    }
}
