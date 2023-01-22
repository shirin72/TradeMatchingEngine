using TradeMatchingEngine.Orders.Dto;

namespace TradeMatchingEngine.Orders.Repositories.Command
{
    public interface IOrderCommand
    {
        Task<int> CreateOrder(OrderDto order);
        Task<int> DeleteOrder(int id);
        Task<int> UpdateOrder(OrderDto order);
    }
}
