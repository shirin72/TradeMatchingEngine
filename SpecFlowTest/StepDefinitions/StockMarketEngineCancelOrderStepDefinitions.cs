using Application.Tests;
using EndPoints.Model;
using Newtonsoft.Json;
using System.Text;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketEngineCancelOrderStepDefinitions
    {
        private readonly ScenarioContext context;
        public StockMarketEngineCancelOrderStepDefinitions(ScenarioContext context)
        {
            this.context = context;
            //this.context.TryAdd("smc", new StockMarketClient("https://localhost:7092/api"));
        }

        [When(@"I cancel '([^']*)'")]
        public async Task WhenICancel(string order)
        {
            var stockMarketClient = this.context.Get<StockMarketClient>("smc");
            await stockMarketClient.CancelOrder(context.Get<TestProcessedOrder>($"{order}Response").OrderId);
        }

        [Then(@"The order '([^']*)'  Should Be Cancelled")]
        public async Task ThenTheOrderShouldBeCancelled(string order)
        {
            var stockMarketClient = this.context.Get<StockMarketClient>("smc");
            var addedOrder = await stockMarketClient.GetOrderById(context.Get<TestProcessedOrder>($"{order}Response").OrderId);

            addedOrder.OrderState.Should().Be(OrderStates.Cancell);
        }
    }
}
