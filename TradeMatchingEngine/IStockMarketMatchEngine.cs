namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
        Task<int> ProcessOrderAsync(
            int price, int amount, Side side,
            DateTime? expireTime = null, bool? fillAndKill = null,
            int? orderParentId = null,
            Func<object, EventArgs, Task> orderCreate = null,
            Func<object, EventArgs, Task> orderModifie = null,
            Func<object, EventArgs, Task> tradeCreat = null);
    }
}
