using Domain.Orders.Repositories.Command;

namespace Domain.Trades.Repositories.Command
{
    public interface ITradeCommandRepository:ICommandRepository<Trade,ITrade>
    {
    }
}
