using Microsoft.EntityFrameworkCore;
using Repository;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderQueryRepository : IOrderCommand
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public OrderQueryRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task<int> CreateOrder(TradeMatchingEngine.Order order)
        {
            await tradeMatchingEngineContext.AddAsync(order);

            return order.Id;
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
            var createOrder = new TradeMatchingEngine.Order(id: order.Id, side: findOrder.Side, price: order.Price, amount: order.Amount, expireTime: order.ExpireTime, order.IsFillAndKill, order.OrderParentId);

            tradeMatchingEngineContext.Update(createOrder);

            return findOrder.Id;
        }
    }
}
