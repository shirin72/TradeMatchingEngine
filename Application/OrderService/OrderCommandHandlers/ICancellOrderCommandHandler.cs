using Domain;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface ICancellOrderCommandHandler:ICommandHandler<long>
    {
    }
}