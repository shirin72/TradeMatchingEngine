using Domain;
using Domain.Orders.Commands;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface IAddOrderCommandHandlers:ICommandHandler<AddOrderCommand>
    {
    }
}