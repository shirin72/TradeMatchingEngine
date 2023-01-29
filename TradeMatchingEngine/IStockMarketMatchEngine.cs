namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine:IAsyncDisposable
    {
        Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null);

        Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null);

        Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null);
    }
}
