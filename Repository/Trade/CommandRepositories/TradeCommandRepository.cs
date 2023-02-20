using Infrastructure.Order.QueryRepositories;
using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine.Trades.Repositories.Command;

namespace Infrastructure.Trade.QueryRepositories
{
    public class TradeCommandRepository : CommandRepository<TradeMatchingEngine.Trade, TradeMatchingEngine.ITrade>,ITradeCommandRepository
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public TradeCommandRepository(TradeMatchingEngineContext dbcontext) : base(dbcontext)
        {
        }
    }
}
