using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface ICommandHandler<T1>
    {
        Task<long?> Handle(T1 modifieOrcommandderCommand);
    }
    public interface IModifieOrderCommandHandler:ICommandHandler<ModifieOrderCommand>
    {
    }
}
