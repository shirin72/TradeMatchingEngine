using Application.Tests;
using TechTalk.SpecFlow.Assist;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature3StepDefinitions : Steps
    {
        private readonly ScenarioContext context;
        private static readonly string ROOT_URL = "https://localhost:7092/api/";
        public StockMarketMatchingEngineFeature3StepDefinitions(ScenarioContext context)
        {
            this.context = context;
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
            string url = $"{ROOT_URL}Trades/GetAllTrades";

            var response = await HttpClientWorker.Execute<object, IEnumerable<TestTrade>>(url, HttpMethod.Get);

            var findTrade = response.Where(t => t.Id == context.Get<TestProcessedOrder>($"BuyOrderResponse").Trades.First().Id).FirstOrDefault();

            findTrade.Amount.Should().Be(table.CreateInstance<TestTrade>().Amount);

            findTrade.Price.Should().Be(table.CreateInstance<TestTrade>().Price);
        }


        [Then(@"Order '([^']*)' Should Be Modified  like this")]
        public async Task ThenOrderShouldBeModifiedLikeThis(string order, Table table)
        {
            var buyOrderId = context.Get<TestProcessedOrder>($"{order}Response").OrderId;

            string url = $"{ROOT_URL}Order/GetOrder?orderId={buyOrderId}";
            var buyOrderDeserialize = await HttpClientWorker.Execute<object, TestOrder>(url, HttpMethod.Get);

            buyOrderDeserialize.Id.Should().Be(buyOrderId);
            buyOrderDeserialize.Amount.Should().Be(table.CreateInstance<TestOrder>().Amount);
            buyOrderDeserialize.Price.Should().Be(table.CreateInstance<TestOrder>().Price);
        }
    }
}
