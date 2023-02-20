using Application.Tests;
using EndPoints.Controller;
using EndPoints.Model;
using Newtonsoft.Json;
using System.Text;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineModifyOrderStepDefinitions
    {
        private readonly ScenarioContext context;
        private readonly HttpClient httpClient;

        public StockMarketMatchingEngineModifyOrderStepDefinitions(ScenarioContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();

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

            var requestBody = new StringContent(JsonConvert.SerializeObject(modifiedOrderVM), Encoding.UTF8, "application/json");

            var result = await httpClient.PutAsync("https://localhost:7092/api/Order/ModifieOrder", requestBody);

            context.Add($"{modifiedOrder}Response", Convert.ToInt64(result.Content.ReadAsStringAsync().Result));
        }

        [Then(@"The order '([^']*)'  Should Be Found like '([^']*)'")]
        public async Task ThenTheOrderShouldBeFoundLike(string order, string modifiedOrder)
        {
            var result = context.Get<long>($"{modifiedOrder}Response");
            var _modifiedorderVM = context.Get<OrderVM>($"{modifiedOrder}");

            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<TestOrder>(await addedOrderId.Content.ReadAsStringAsync());

            orderDeserialize.Id.Should().Be(result);
            orderDeserialize.Amount.Should().Be(_modifiedorderVM.Amount);
        }

    }
}
