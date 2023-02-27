using Application.Tests;
using EndPoints.Controller;
using EndPoints.Model;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineModifyOrderStepDefinitions
    {
        private readonly ScenarioContext context;
        private static readonly string ROOT_URL = "https://localhost:7092/api/Orders/";
        public StockMarketMatchingEngineModifyOrderStepDefinitions(ScenarioContext context)
        {
            this.context = context;
        }

        [When(@"I Modify The Order '([^']*)' to '([^']*)'")]
        public async Task WhenIModifyTheOrderTo(string sellOrder, string modifiedOrder)
        {
            var orderId = context.Get<TestProcessedOrder>($"{sellOrder}Response").OrderId;

            var _modifiedorderVM = context.Get<OrderVM>($"{modifiedOrder}");

            var modifiedOrderVM = new ModifiedOrderVM()
            {
                OrderId = orderId,
                Amount = _modifiedorderVM.Amount,
                ExpDate = _modifiedorderVM.ExpireTime,
                Price = _modifiedorderVM.Price,
            };

            var response = await HttpClientWorker.Execute<ModifiedOrderVM, long>(ROOT_URL, HttpMethod.Put, modifiedOrderVM);

            context.Add($"{modifiedOrder}Response", Convert.ToInt64(response));
        }

        [Then(@"The order '([^']*)'  Should Be Found like '([^']*)'")]
        public async Task ThenTheOrderShouldBeFoundLike(string order, string modifiedOrder)
        {
            var result = context.Get<long>($"{modifiedOrder}Response");
            var _modifiedorderVM = context.Get<OrderVM>($"{modifiedOrder}");
            string url = $"{ROOT_URL}{result}";
            var response = await HttpClientWorker.Execute<object, TestOrder>(url, HttpMethod.Get);

            response.Id.Should().Be(result);
            response.Amount.Should().Be(_modifiedorderVM.Amount);
        }

    }
}
