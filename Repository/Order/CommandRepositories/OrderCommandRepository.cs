using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderCommandRepository : IOrderCommand
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;
        private readonly IServiceScopeFactory serviceScopeFactory;
        public OrderCommandRepository(TradeMatchingEngineContext tradeMatchingEngineContext, IServiceScopeFactory serviceScopeFactory)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<int> CreateOrder(TradeMatchingEngine.Order order)
        {
            try
            {
                //using (var scope = serviceScopeFactory.CreateScope())
                //{
                //    // You can ask for any service here and DI will resolve it and give you back service instance
                //    var contextService = scope.ServiceProvider.GetRequiredService <TradeMatchingEngineContext>();
                //    var s = await contextService.Orders.AddAsync(order);
                //    var state = contextService.ContextId;
                //    var added = await contextService.SaveChangesAsync();
                //    if (added > 0)
                //    {
                //        return order.Id;
                //    }
                //}

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
        public IEnumerable<TradeMatchingEngine.Order> GetAllOrders()
        {
            try
            {
                var res = tradeMatchingEngineContext.Orders.AsNoTracking().ToList();
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<TradeMatchingEngine.Order> GetOrderById(int id)
        {
            return await tradeMatchingEngineContext.Orders.FindAsync(id);
        }

        public async Task<int> GetLastOrder()
        {
            return await tradeMatchingEngineContext.Orders.MaxAsync(x => x.Id);
        }
    }
}
