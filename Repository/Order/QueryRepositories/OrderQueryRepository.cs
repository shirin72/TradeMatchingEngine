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
