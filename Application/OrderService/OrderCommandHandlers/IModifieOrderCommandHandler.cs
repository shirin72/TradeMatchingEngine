using TradeMatchingEngine.Orders.Commands;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface ICommandHandler<T>
    {
        Task<long?> Handle(T modifieOrcommandderCommand);
    }
    public interface IModifieOrderCommandHandler:ICommandHandler<ModifieOrderCommand>
    {
    }
}
