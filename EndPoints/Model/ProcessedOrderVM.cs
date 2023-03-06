namespace EndPoints.Model
{
    public class ProcessedOrderVM
    {
        public RegisteredOrderVM RegisteredOrder { get; set; }

        public IEnumerable<TradeVM>? Trades { get; set; }

        public IEnumerable<long> CancelledOrders { get; set; }

    }
}
