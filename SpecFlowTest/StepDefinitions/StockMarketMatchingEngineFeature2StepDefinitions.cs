using EndPoints.Model;
using TechTalk.SpecFlow.Assist;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature2StepDefinitions : Steps
    {
        private ScenarioContext context;

        public StockMarketMatchingEngineFeature2StepDefinitions(ScenarioContext context)
        {
            this.context = context;

        }

        [BeforeScenario]
        public async Task CancellAllOrders()
        {
            this.context.Add("smc", new StockMarketClient("https://localhost:7092/api"));

            var client = this.context.Get<StockMarketClient>("smc");
            await client.CancelAllOrders();
        }

        [Given(@"Order '([^']*)' Has Been Defined")]
        public void GivenSellOrderHasBeenDefined(string order, Table table)
        {
            context.Add(order, table.CreateInstance<RegisterOrderVM>());
        }

        [When(@"I Register The Order '([^']*)'")]
        public async Task WhenIRegisterTheSellOrder(string order)
        {
            var client = this.context.Get<StockMarketClient>("smc");
            var result = await client.ProcessOrder(context.Get<RegisterOrderVM>(order));
            context.Add($"{order}Response", result);
        }


        [Then(@"Order '([^']*)' Should Be Enqueued")]
        public async Task ThenOrderShouldBeEnqueuedAsync(string order)
        {
            var orderId = context.Get<ProcessedOrderVM>($"{order}Response").RegisteredOrder.OrderId;
            var client = this.context.Get<StockMarketClient>("smc");
            var addedOrderId = await client.GetOrderById(orderId);

            addedOrderId.Id.Should().Be(orderId);

        }
    }
}
