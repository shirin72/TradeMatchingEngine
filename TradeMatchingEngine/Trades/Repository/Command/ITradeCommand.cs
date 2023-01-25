using TradeMatchingEngine.Trades.Dto;

namespace TradeMatchingEngine.Trades.Repositories.Command
{
    public interface ITradeCommand
    {
        Task AddTrade(Trade trade);
    }
}
