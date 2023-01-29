namespace Application.OrderService.OrderCommandHandlers
{
    public interface ICancellOrderCommandHandler
    {
        Task<long?> Handle(long orderId);
    }
}