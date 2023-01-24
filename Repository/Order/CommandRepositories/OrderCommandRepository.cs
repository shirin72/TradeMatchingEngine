using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderCommandRepository : IOrderCommand
    {
        private  TradeMatchingEngineContext tradeMatchingEngineContext;
        public OrderCommandRepository()
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task<int> CreateOrder(TradeMatchingEngine.Order order)
        {
            try
            {
                var addAsync = await tradeMatchingEngineContext.Orders.AddAsync(order).ConfigureAwait(false);
                var saveChangesAsync = await tradeMatchingEngineContext.SaveChangesAsync().ConfigureAwait(false);
                if (saveChangesAsync > 0)
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

            tradeMatchingEngineContext.Orders.Update(order);

            return findOrder.Id;
        }
    }
}
