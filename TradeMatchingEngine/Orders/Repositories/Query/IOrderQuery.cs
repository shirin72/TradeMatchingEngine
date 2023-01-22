namespace TradeMatchingEngine.Orders.Repositories.Query
{
    public interface IOrderQuery
    {
        Task<Order> GetOrderById(int id);
        Task<IEnumerable<Order>> GetAllOrders();
    }
}
