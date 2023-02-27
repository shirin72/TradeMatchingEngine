using Application.Tests;
using EndPoints.Model;
using TechTalk.SpecFlow.Assist;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature2StepDefinitions : Steps
    {
        private ScenarioContext context;
        private static readonly string ROOT_URL = "https://localhost:7092/api/Orders/";

        public StockMarketMatchingEngineFeature2StepDefinitions(ScenarioContext context)
        {
            this.context = context;
            HttpClientWorker.AddConnection(ROOT_URL);
        }

        public string BaseAddress { get; }

        [BeforeScenario]
        public async Task CancellAllOrders()
        {
            await HttpClientWorker.Execute<object, object>($"{ROOT_URL}", HttpMethod.Patch);
        }

        [Given(@"Order '([^']*)' Has Been Defined")]
        public void GivenSellOrderHasBeenDefined(string order, Table table)
        {
            context.Add(order, table.CreateInstance<OrderVM>());
        }

        [When(@"I Register The Order '([^']*)'")]
        public async Task WhenIRegisterTheSellOrder(string order)
        {
            var orderVm = context.Get<OrderVM>(order);
            string url = $"{ROOT_URL}";
            var result = await HttpClientWorker.Execute<OrderVM, TestProcessedOrder>(url, HttpMethod.Post, orderVm);
            context.Add($"{order}Response", result);
        }


        [Then(@"Order '([^']*)' Should Be Enqueued")]
        public async Task ThenOrderShouldBeEnqueuedAsync(string order)
        {
            var result = context.Get<TestProcessedOrder>($"{order}Response").OrderId;
            string url = $"{ROOT_URL}{result}";
            var addedOrderId = await HttpClientWorker.Execute<object, TestOrder>(url, HttpMethod.Get);

            addedOrderId.Id.Should().Be(result);

        }
    }
}
