using EndPoints.Model;
using Newtonsoft.Json;
using System.Text;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketEngineCancelOrderStepDefinitions
    {
        private readonly FeatureContext context;
        private readonly HttpClient httpClient;

        public StockMarketEngineCancelOrderStepDefinitions(FeatureContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();

        }

        [When(@"I Send '([^']*)' orderId And Ask For Cancel")]
        public async Task WhenISendOrderIdAndAskForCancelAsync(string order)
        {
            var orderId = context.Get<ProcessedOrder>($"{order}Response").OrderId;
            var body = new StringContent(JsonConvert.SerializeObject(new CancellOrderVM() { OrderId = orderId }), Encoding.UTF8, "application/json");
            await httpClient.PatchAsync($"https://localhost:7092/api/Order/CancellOrder", body);
        }

        [Then(@"The '([^']*)' OrderState Should Be Equal to Cancel")]
        public async Task ThenTheOrderStateShouldBeEqualToCancelAsync(string order)
        {
            var result = context.Get<ProcessedOrder>($"{order}Response").OrderId;
            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());

            orderDeserialize.OrderState.Should().Be(OrderStates.Cancell);
        }

    }
}
