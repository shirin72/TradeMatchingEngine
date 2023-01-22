using Microsoft.EntityFrameworkCore;
using Repository;
using TradeMatchingEngine.Orders.Dto;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Infrastructure.Order.CommandRepositories
{
    public class OrderQueryRepository : IOrderQuery
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;
        public OrderQueryRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            return await tradeMatchingEngineContext.Orders.AsNoTracking().ToListAsync();
        }

        public async Task<OrderDto> GetOrderById(int id)
        {
            return await tradeMatchingEngineContext.Orders.FindAsync(id);
        }

    }
}
