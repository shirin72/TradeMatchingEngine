using EndPoints.Controller;
using Newtonsoft.Json;
using System.Text;
using TechTalk.SpecFlow.Assist;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineModifyOrderStepDefinitions
    {
        private readonly FeatureContext context;
        private readonly HttpClient httpClient;

        public StockMarketMatchingEngineModifyOrderStepDefinitions(FeatureContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();

        }

        [When(@"I Will Try To Modify The Order '([^']*)' with Another Orde '([^']*)'")]
        public async Task WhenIWillTryToModifyTheOrderWithAnotherOrde(string sellOrder, string modifiedOrder, Table table)
        {
            context.Add(modifiedOrder, table);

            var orderId = context.Get<long>($"{sellOrder}Response");

            var modifiedOrderVM = table.CreateInstance<ModifiedOrderVM>();
            modifiedOrderVM.OrderId = orderId;

            var requestBody = new StringContent(JsonConvert.SerializeObject(modifiedOrderVM), Encoding.UTF8, "application/json");

            var result = await httpClient.PutAsync("https://localhost:7092/api/Order/ModifieOrder", requestBody);

            context.Add($"{modifiedOrder}Response", Convert.ToInt64(result.Content.ReadAsStringAsync().Result));
        }


        [Then(@"The order '([^']*)' Should Be Found")]
        public async Task ThenTheOrderShouldBeFound(string order)
        {
            var result = context.Get<long>($"{order}Response");
            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());

            orderDeserialize.Id.Should().Be(result);
            orderDeserialize.Amount.Should().Be(context.Get<Table>($"{order}").CreateInstance<ModifiedOrderVM>().Amount);
        }
    }
}
