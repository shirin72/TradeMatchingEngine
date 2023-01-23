using Microsoft.EntityFrameworkCore;
using Repository;
using TradeMatchingEngine.Orders.Dto;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderCommandRepository : IOrderCommand
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public OrderCommandRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task<int> CreateOrder(TradeMatchingEngine.Order order)
        {
            try
            {
                var s = await tradeMatchingEngineContext.Orders.AddAsync(order);
                var added = await tradeMatchingEngineContext.SaveChangesAsync();
                if (added > 0)
                {
                    return order.Id;
                }

                throw new Exception(message: "Order cannot be added");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<int> DeleteOrder(int id)
        {
            var findOrder = await tradeMatchingEngineContext.Orders.Where(x => x.Id == id).FirstOrDefaultAsync();

            findOrder?.SetStateCancelled();

            tradeMatchingEngineContext.Update(findOrder);

            return findOrder.Id;
        }

        public async Task<int> UpdateOrder(TradeMatchingEngine.Order order)
        {
            var findOrder = await tradeMatchingEngineContext.Orders.Where(x => x.Id == order.Id).FirstOrDefaultAsync();
            //var createOrder = new TradeMatchingEngine.OrderDto(id: order.Id, side: findOrder.Side, price: order.Price, amount: order.Amount, expireTime: order.ExpireTime, order.IsFillAndKill, order.OrderParentId);

            tradeMatchingEngineContext.Orders.Update(order);

            return findOrder.Id;
        }
    }
}
