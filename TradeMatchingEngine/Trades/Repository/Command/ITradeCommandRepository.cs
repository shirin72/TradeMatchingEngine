using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Trades.Dto;

namespace TradeMatchingEngine.Trades.Repositories.Command
{
    public interface ITradeCommandRepository:ICommandRepository<Trade>
    {
    }
}
