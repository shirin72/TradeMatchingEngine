namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine:IAsyncDisposable
    {
        Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null);
        void Close();
        Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null);
        void Open();
        void PreOpen();
        Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null);
    }
}
