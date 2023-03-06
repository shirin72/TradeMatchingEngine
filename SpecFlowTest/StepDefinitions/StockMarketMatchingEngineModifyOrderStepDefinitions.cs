using Application.Tests;
using EndPoints.Controller;
using EndPoints.Model;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineModifyOrderStepDefinitions
    {
        private readonly ScenarioContext context;
        public StockMarketMatchingEngineModifyOrderStepDefinitions(ScenarioContext context)
        {
            this.context = context;
        }

        [When(@"I Modify The Order '([^']*)' to '([^']*)'")]
        public async Task WhenIModifyTheOrderTo(string sellOrder, string modifiedOrder)
        {
            var orderId = context.Get<ProcessedOrderVM>($"{sellOrder}Response").RegisteredOrder.OrderId;

            var _modifiedorderVM = context.Get<RegisterOrderVM>($"{modifiedOrder}");

            var modifiedOrderVM = new ModifiedOrderVM()
            {
                OrderId = orderId,
                Amount = _modifiedorderVM.Amount,
                ExpDate = _modifiedorderVM.ExpireTime,
                Price = _modifiedorderVM.Price,
            };

            var stockMarketClient = context.Get<StockMarketClient>("smc");
            var response = await stockMarketClient.ModifyOrder(modifiedOrderVM);

            context.Add($"{modifiedOrder}Response", Convert.ToInt64(response.RegisteredOrder.OrderId));
        }

        [Then(@"The order '([^']*)'  Should Be Found like '([^']*)'")]
        public async Task ThenTheOrderShouldBeFoundLike(string order, string modifiedOrder)
        {
            var result = context.Get<long>($"{modifiedOrder}Response");
            var _modifiedorderVM = context.Get<RegisterOrderVM>($"{modifiedOrder}");

            var stockMarketClient = context.Get<StockMarketClient>("smc");
            var response = await stockMarketClient.GetOrderById(result);

            response.Id.Should().Be(result);
            response.Amount.Should().Be(_modifiedorderVM.Amount);
        }

    }
}
