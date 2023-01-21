namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
        Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null);
    }
}
