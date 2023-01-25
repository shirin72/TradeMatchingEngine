using TradeMatchingEngine.Orders.Dto;

namespace TradeMatchingEngine.Orders.Repositories.Query
{
    public interface IOrderQuery
    {
        Task<Order> GetOrderById(long id);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<long> GetLastOrder();
    }
}
