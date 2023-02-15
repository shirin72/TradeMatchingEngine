namespace TradeMatchingEngine
{
    public interface IStockMarketMatchingEngineProcessContext
    {
        IEnumerable<Trade> CreatedTrades { get; }
        IEnumerable<Order> ModifiedOrders { get; }
        Order? Order { get; }
    }
}