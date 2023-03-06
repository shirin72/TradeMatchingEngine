using Application.Tests;
using EndPoints.Model;
using Newtonsoft.Json;
using System.Text;
using Domain;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketEngineCancelOrderStepDefinitions
    {
        private readonly ScenarioContext context;
        public StockMarketEngineCancelOrderStepDefinitions(ScenarioContext context)
        {
            this.context = context;
        }

        [When(@"I cancel '([^']*)'")]
        public async Task WhenICancel(string order)
        {
            var stockMarketClient = this.context.Get<StockMarketClient>("smc");
            await stockMarketClient.CancelOrder(context.Get<ProcessedOrderVM>($"{order}Response").RegisteredOrder.OrderId);
        }

        [Then(@"The order '([^']*)'  Should Be Cancelled")]
        public async Task ThenTheOrderShouldBeCancelled(string order)
        {
            var stockMarketClient = this.context.Get<StockMarketClient>("smc");
            var addedOrder = await stockMarketClient.GetOrderById(context.Get<ProcessedOrderVM>($"{order}Response").RegisteredOrder.OrderId);

            addedOrder.OrderState.Should().Be(OrderStates.Cancell);
        }
    }
}
