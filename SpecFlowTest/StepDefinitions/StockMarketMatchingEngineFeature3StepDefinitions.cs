using Application.Tests;
using TechTalk.SpecFlow.Assist;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature3StepDefinitions : Steps
    {
        private readonly ScenarioContext context;
        public StockMarketMatchingEngineFeature3StepDefinitions(ScenarioContext context)
        {
            this.context = context;
            //this.context.TryAdd("smc", new StockMarketClient("https://localhost:7092/api"));
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
            var stockMarketClient =context.Get<StockMarketClient>("smc");
            var response = await stockMarketClient.GetTrades();

            var findTrade = response.Where(t => t.Id == context.Get<TestProcessedOrder>($"BuyOrderResponse").Trades.First().Id).FirstOrDefault();

            findTrade.Amount.Should().Be(table.CreateInstance<TestTrade>().Amount);

            findTrade.Price.Should().Be(table.CreateInstance<TestTrade>().Price);
        }


        [Then(@"Order '([^']*)' Should Be Modified  like this")]
        public async Task ThenOrderShouldBeModifiedLikeThis(string order, Table table)
        {
            var buyOrderId = context.Get<TestProcessedOrder>($"{order}Response").OrderId;

            var stockMarketClient = context.Get<StockMarketClient>("smc");
            var response = await stockMarketClient.GetOrderById(buyOrderId);

            response.Id.Should().Be(buyOrderId);
            response.Amount.Should().Be(table.CreateInstance<TestOrder>().Amount);
            response.Price.Should().Be(table.CreateInstance<TestOrder>().Price);
        }
    }
}
