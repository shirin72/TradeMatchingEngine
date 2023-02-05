using BoDi;
using EndPoints.Model;
using Infrastructure.Order.CommandRepositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using TechTalk.SpecFlow.Assist;
using TradeMatchingEngine;
using TradeMatchingEngine.GenericRepositories;
using TradeMatchingEngine.Orders.Repositories.Query;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineFeatureStepDefinitions : Steps
    {
        private FeatureContext context;
        private readonly HttpClient httpClient;
        private IOrderQueryRepository orderQueryRepository;
        private readonly IQueryRepository<Order> queryRepository;
        private readonly IObjectContainer _container;

        public StockMarketMatchingEngineFeatureStepDefinitions(FeatureContext context, IObjectContainer container)
        {
            this.context = context;
            this.httpClient = new HttpClient();
            _container = container;

        }

        [BeforeScenario]
        public void CreateOrderQueryRepository()
        {
            var connectionstring = "Server=.;Initial Catalog=TradeMatchingEngine;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;";

            var optionsBuilder = new DbContextOptionsBuilder<Infrastructure.TradeMatchingEngineContext>();
            optionsBuilder.UseSqlServer(connectionstring);

            Infrastructure.TradeMatchingEngineContext dbContext = new Infrastructure.TradeMatchingEngineContext(optionsBuilder.Options);

            _container.RegisterInstanceAs(new OrderQueryRepository(dbContext));

            this.orderQueryRepository = _container.Resolve<OrderQueryRepository>();
        }


        [Given(@"'([^']*)' Has Been Defined")]
        public void GivenHasBeenDefined(string sellOrder, Table table)
        {
            context.Add(sellOrder, table);
        }

        [When(@"I Register The '([^']*)'")]
        public async Task WhenIRegisterThe(string sellOrder)
        {
            var orderVm = context.Get<Table>(sellOrder).CreateInstance<OrderVM>();

            var requestBody = new StringContent(JsonConvert.SerializeObject(orderVm), Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync("https://localhost:7092/api/Order/ProcessOrder", requestBody).GetAwaiter().GetResult();
            context.Add("response", Convert.ToInt64(result.Content.ReadAsStringAsync().Result));
        }

        [Then(@"Should The '([^']*)' Be Enqueued")]
        public async Task ThenShouldTheBeEnqueued(string sellOrder)
        {
            var result = context.Get<long>("response");
            var expected = context.Get<Table>(sellOrder).CreateInstance<Order>();

            var addedOrderId = httpClient.GetAsync($"https://localhost:7092/api/Order/GetOrder?orderId={result}").GetAwaiter().GetResult();
            var orderDeserialize = JsonConvert.DeserializeObject<Order>(await addedOrderId.Content.ReadAsStringAsync());

            orderDeserialize.Id.Should().Be(result);

            orderDeserialize.Amount.Should().Be(expected.Amount);
            orderDeserialize.Price.Should().Be(expected.Price);
        }

    }
}
