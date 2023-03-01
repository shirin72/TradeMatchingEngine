namespace Domain
{
    public interface IStockMarketMatchEngine:IAsyncDisposable
    {
        Task<IStockMarketMatchingEngineProcessContext> CancelOrderAsync(long orderId);

        Task<IStockMarketMatchingEngineProcessContext> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate);

        Task<IStockMarketMatchingEngineProcessContext> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null);
    }
}
