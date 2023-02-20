using Application.Tests;

namespace SpecFlowTest
{
    public class TestProcessedOrder
    {
        public long OrderId { get; set; }
        public IEnumerable<long> CancelledOrders { get; set; }
        public  IEnumerable<TestTrade>? Trades { get; set; }
    }
}
