﻿namespace TradeMatchingEngine.Orders.Repositories.Command
{
    public interface IOrderCommand
    {
        Task<int> CreateOrder(Order order);
        Task<int> DeleteOrder(int id);
        Task<int> UpdateOrder(Order order);
    }
}
