using Application.Tests;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Assist;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature3StepDefinitions : Steps
    {
        private readonly ScenarioContext context;
        private readonly HttpClient httpClient;

        public StockMarketMatchingEngineFeature3StepDefinitions(ScenarioContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();
        }

        [Given(@"Order '([^']*)' Has Been Registerd")]
        public void GivenOrderHasBeenRegisterd(string order, Table table)
        {
            var table1 = new Table(new string[] { "Side", "Price", "Amount", "IsFillAndKill", "ExpireTime" });
            foreach (var row in table.Rows)
            {
                table1.AddRow(new string[] { row[0], row[1], row[2], row[3], row[4] });
            }

            Given("Order 'SellOrder' Has Been Defined", table1);
            When("I Register The Order 'SellOrder'");
            Then("Order 'SellOrder' Should Be Enqueued");
        }

        [Then(@"The following '([^']*)' will be created")]
        public async Task ThenTheFollowingWillBeCreated(string trade, Table table)
        {
            var getTrades = httpClient.GetAsync("https://localhost:7092/api/Trades/GetAllTrades").GetAwaiter().GetResult();

            var response = JsonConvert.DeserializeObject<IEnumerable<TestTrade>>(await getTrades.Content.ReadAsStringAsync(),
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Objects
                });

            var findTrade = response.Where(t => t.Id == context.Get<TestProcessedOrder>($"BuyOrderResponse").Trades.First().Id).FirstOrDefault();

            findTrade.Amount.Should().Be(table.CreateInstance<TestTrade>().Amount);

            findTrade.Price.Should().Be(table.CreateInstance<TestTrade>().Price);
        }


        [Then(@"Order '([^']*)' Should Be Modified  like this")]
        public async Task ThenOrderShouldBeModifiedLikeThis(string order, Table table)
        {
            var buyOrderId = context.Get<TestProcessedOrder>($"{order}Response").OrderId;

            var getBuyOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={buyOrderId}").GetAwaiter().GetResult();
            var buyOrderDeserialize = JsonConvert.DeserializeObject<TestOrder>(await getBuyOrderId.Content.ReadAsStringAsync());

            buyOrderDeserialize.Id.Should().Be(buyOrderId);
            buyOrderDeserialize.Amount.Should().Be(table.CreateInstance<TestOrder>().Amount);
            buyOrderDeserialize.Price.Should().Be(table.CreateInstance<TestOrder>().Price);
        }
    }
}
