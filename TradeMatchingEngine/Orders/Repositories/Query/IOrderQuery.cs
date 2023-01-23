using TradeMatchingEngine.Orders.Dto;

namespace TradeMatchingEngine.Orders.Repositories.Query
{
    public interface IOrderQuery
    {
        Task<Order> GetOrderById(int id);
        IEnumerable<Order> GetAllOrders();
        Task<int> GetLastOrder();
    }
}
