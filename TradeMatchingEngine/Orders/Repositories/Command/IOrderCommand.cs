namespace TradeMatchingEngine.Orders.Repositories.Command
{
    public interface IOrderCommandRepository
    {
        Task<Order> Find(long id);
        Task AddOrder(Order order);
        Task DeleteOrder(long id);
    }
}
