using Newtonsoft.Json;
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
        }

        [Then(@"The following '([^']*)' will be created")]
        public async Task ThenTheFollowingWillBeCreated(string trade, Table table)
        {
            var getTrades = httpClient.GetAsync("https://localhost:7092/api/Trades/GetAllTrades").GetAwaiter().GetResult();

            var response = JsonConvert.DeserializeObject<IEnumerable<Trade>>(await getTrades.Content.ReadAsStringAsync());
            var findTrade = response.Where(t => t.Id == context.Get<ProcessedOrder>($"BuyOrderResponse").Trades.First().Id).FirstOrDefault();

            findTrade.Amount.Should().Be(table.CreateInstance<Trade>().Amount);
            findTrade.Price.Should().Be(table.CreateInstance<Trade>().Price);
        }


        [Then(@"Order '([^']*)' Should Be Modified  like this")]
        public async Task ThenOrderShouldBeModifiedLikeThis(string order, Table table)
        {
            var result = context.Get<ProcessedOrder>($"{order}Response").OrderId;

            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());

            orderDeserialize.Id.Should().Be(result);

            orderDeserialize.Amount.Should().Be(table.CreateInstance<Order>().Amount);
            orderDeserialize.Price.Should().Be(table.CreateInstance<Order>().Price);
        }
    }
}
