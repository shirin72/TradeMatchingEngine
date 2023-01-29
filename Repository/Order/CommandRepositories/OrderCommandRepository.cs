using Microsoft.Extensions.DependencyInjection;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderCommandRepository :CommandRepository<TradeMatchingEngine.Order>,  IOrderCommandRepository
    {
        private readonly TradeMatchingEngineContext _tradeMatchingEngineContext;

        public OrderCommandRepository(TradeMatchingEngineContext dbcontext) : base(dbcontext)
        {
            _tradeMatchingEngineContext = dbcontext;
        }
    }
}
