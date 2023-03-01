using Domain;

namespace Domain
{
    public class ProcessedOrder
    {
        public long OrderId { get; set; }

        public  IEnumerable<ITrade>? Trades { get; set; }

        public IEnumerable<long> CancelledOrders { get; set; }
    }
}
