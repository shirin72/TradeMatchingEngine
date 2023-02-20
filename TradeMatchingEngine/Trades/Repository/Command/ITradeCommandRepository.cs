using TradeMatchingEngine.Orders.Repositories.Command;

namespace TradeMatchingEngine.Trades.Repositories.Command
{
    public interface ITradeCommandRepository:ICommandRepository<Trade,ITrade>
    {
    }
}
