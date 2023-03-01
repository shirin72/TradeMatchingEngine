using Microsoft.Extensions.DependencyInjection;
using Domain.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class OrderCommandRepository :CommandRepository<Domain.Order, Domain.IOrder>,  IOrderCommandRepository
    {
        private readonly TradeMatchingEngineContext _tradeMatchingEngineContext;

        public OrderCommandRepository(TradeMatchingEngineContext dbcontext) : base(dbcontext)
        {
            _tradeMatchingEngineContext = dbcontext;
        }
    }
}
