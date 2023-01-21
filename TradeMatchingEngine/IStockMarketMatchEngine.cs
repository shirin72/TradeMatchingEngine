namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
        Task<int> ProcessOrderAsync(int price, int amount, Side side, bool isForModifie, DateTime? expireTime = null, bool? fillAndKill = null);
    }
}
