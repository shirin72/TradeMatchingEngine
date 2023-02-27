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
        private static readonly string ROOT_URL = "https://localhost:7092/api/Orders/";
        public StockMarketEngineCancelOrderStepDefinitions(ScenarioContext context)
        {
            this.context = context;
        }

        [When(@"I cancel '([^']*)'")]
        public async Task WhenICancel(string order)
        {
            var orderId = context.Get<TestProcessedOrder>($"{order}Response").OrderId;
            await HttpClientWorker.Execute($"{ROOT_URL}{orderId}", HttpMethod.Patch);
        }

        [Then(@"The order '([^']*)'  Should Be Cancelled")]
        public async Task ThenTheOrderShouldBeCancelled(string order)
        {
            var result = context.Get<TestProcessedOrder>($"{order}Response").OrderId;

            var addedOrderId = await HttpClientWorker.Execute<TestOrder>($"{ROOT_URL}{result}", HttpMethod.Get);

            addedOrderId.OrderState.Should().Be(OrderStates.Cancell);
        }
    }
}
