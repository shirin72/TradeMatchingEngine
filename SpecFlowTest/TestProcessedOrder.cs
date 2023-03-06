using Application.Tests;
using EndPoints.Model;

namespace SpecFlowTest
{
    public class TestProcessedOrder
    {
        public RegisteredOrderVM RegisteredOrder { get; set; }

        public IEnumerable<TestTrade>? Trades { get; set; }

        public IEnumerable<long> CancelledOrders { get; set; }
    }
}
