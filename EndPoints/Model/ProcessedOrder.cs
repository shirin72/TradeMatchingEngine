using TradeMatchingEngine;

namespace EndPoints.Model
{
    public class ProcessedOrder
    {
        public long OrderId { get; set; }

        public IEnumerable<Trade>? Trades { get; set; }
    }
}
