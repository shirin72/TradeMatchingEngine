namespace TradeMatchingEngine.Trades.Repositories.Command
{
    public interface ITradeCommand
    {
        Task<long> CreateTrade(ITrade trade);
    }
}
