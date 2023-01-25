using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderCommandRepository : IOrderCommandRepository
    {
        private readonly TradeMatchingEngineContext _tradeMatchingEngineContext;
        public OrderCommandRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            _tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task AddOrder(TradeMatchingEngine.Order order)
        {
            await _tradeMatchingEngineContext.Orders.AddAsync(order).ConfigureAwait(false);
        }

        public async Task DeleteOrder(long id)
        {
            var findOrder = await _tradeMatchingEngineContext.Orders.FindAsync(id);
            _tradeMatchingEngineContext.Orders.Remove(findOrder);
        }

        public async Task<TradeMatchingEngine.Order> Find(long id)
        {
            return await _tradeMatchingEngineContext.Orders.FindAsync(id);
        }
    }
}
