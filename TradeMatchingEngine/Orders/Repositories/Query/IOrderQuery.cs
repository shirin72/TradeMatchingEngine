using TradeMatchingEngine.Orders.Dto;

namespace TradeMatchingEngine.Orders.Repositories.Query
{
    public interface IOrderQuery
    {
        Task<OrderDto> GetOrderById(int id);
        Task<IEnumerable<OrderDto>> GetAllOrders();
    }
}
