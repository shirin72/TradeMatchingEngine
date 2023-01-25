using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine.Orders.Repositories.Query;

namespace Infrastructure.Order.CommandRepositories
{
    public class OrderQueryRepository : IOrderQuery
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;
        public OrderQueryRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task<IEnumerable<TradeMatchingEngine.Order>> GetAllOrders()
        {
            return await tradeMatchingEngineContext.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<TradeMatchingEngine.Order> GetOrderById(long id)
        {
            return await tradeMatchingEngineContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long> GetLastOrder()
        {
            if (tradeMatchingEngineContext.Orders.Any())
            {
                return await tradeMatchingEngineContext.Orders.MaxAsync(x => x.Id);
            }

            return 0;
        }
    }
}
