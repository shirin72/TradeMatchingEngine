using Infrastructure.GenericServices;
using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine.Orders.Repositories.Query;

namespace Infrastructure.Order.CommandRepositories
{
    public class OrderQueryRepository :QueryRepository<TradeMatchingEngine.Order, TradeMatchingEngine.IOrder>, IOrderQueryRepository
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public OrderQueryRepository(TradeMatchingEngineContext tradeMatchingEngineContext):base(tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }
    }

    
}
