using Domain;
using Domain.Orders.Commands;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface ICommandHandler<T1>
    {
        Task<ProcessedOrder?> Handle(T1 modifieOrcommandderCommand);
    }
    public interface IModifieOrderCommandHandler:ICommandHandler<ModifieOrderCommand>
    {
    }
}
