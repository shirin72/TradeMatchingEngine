using TradeMatchingEngine;

namespace TradeMatchingEngine
{
    public class ProcessedOrder
    {
        public long OrderId { get; set; }

        public IEnumerable<Trade>? Trades { get; set; }
    }
}
