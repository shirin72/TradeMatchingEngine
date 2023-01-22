namespace TradeMatchingEngine.Trades.Repositories.Query
{
    public interface ITradeQuery
    {
        Task<ITrade> GetTradeById(int id);
        Task<ITrade> GetAllTrades();
    }
}
