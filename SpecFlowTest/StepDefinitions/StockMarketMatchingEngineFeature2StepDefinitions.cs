using EndPoints.Model;
using Newtonsoft.Json;
using System;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TradeMatchingEngine;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeature2StepDefinitions:Steps
    {
        private FeatureContext context;
        private readonly HttpClient httpClient;


        public StockMarketMatchingEngineFeature2StepDefinitions(FeatureContext context)
        {
            this.context = context;
            this.httpClient = new HttpClient();
        }

        [Given(@"Order '([^']*)' Has Been Defined")]
        public void GivenSellOrderHasBeenDefined(string order, Table table)
        {
            context.Add(order, table);
        }

        [When(@"I Register The Order '([^']*)'")]
        public void WhenIRegisterTheSellOrder(string order)
        {

            var orderVm = context.Get<Table>(order).CreateInstance<OrderVM>();

            var requestBody = new StringContent(JsonConvert.SerializeObject(orderVm), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync("https://localhost:7092/api/Order/ProcessOrder", requestBody).GetAwaiter().GetResult();
            context.Add($"{order}Response", Convert.ToInt64(result.Content.ReadAsStringAsync().Result));
        }


        //[Then(@"The following trade will be created")]
        //public async Task ThenTheFollowingTradeWillBeCreated(Table table)
        //{
        //    var expected = table.CreateInstance<Trade>();

        //    var allCreatedTrade = httpClient.GetAsync($"https://localhost:7092/api/Trades/GetAllTrades").GetAwaiter().GetResult();

        //    var allCreatedTradeDeserialize = JsonConvert.DeserializeObject<IEnumerable<Trade>>(await allCreatedTrade.Content.ReadAsStringAsync());

        //    var trade = allCreatedTradeDeserialize.Where(t => t.SellOrderId == context.Get<long>($"SellOrderResponse") && t.BuyOrderId == context.Get<long>("BuyOrderResponse")).FirstOrDefault();

        //    trade.Amount.Should().Be(expected.Amount);
        //    trade.Price.Should().Be(expected.Price);

        //}

        
        //[Then(@"Order '([^']*)' Should Be Modified  With Order '([^']*)'")]
        //public async Task ThenOrderShouldBeModifiedWithOrder(string modifiedOrder, string order, Table table)
        //{
        //    context.Add(modifiedOrder, table);

        //    var result = context.Get<long>($"{order}Response");
        //    var expected = table.CreateInstance<Order>();

        //    var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
        //    var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());

        //    orderDeserialize.Id.Should().Be(result);

        //    orderDeserialize.Amount.Should().Be(expected.Amount);
        //    orderDeserialize.Price.Should().Be(expected.Price);
        //}

        [Then(@"Order '([^']*)' Should Be Enqueued")]
        public async Task ThenOrderShouldBeEnqueuedAsync(string order)
        {
            var result = context.Get<long>($"{order}Response");
            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());
            orderDeserialize.Id.Should().Be(result);
        }

    }
}
