using Infrastructure.GenericServices;
using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine;
using TradeMatchingEngine.Trades.Repositories.Query;
namespace Infrastructure.Order.CommandRepositories
{
    public class TradeQueryRepository : QueryRepository<TradeMatchingEngine.Trade, TradeMatchingEngine.ITrade>,ITradeQueryRespository
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public TradeQueryRepository(TradeMatchingEngineContext dbcontext) : base(dbcontext)
        {

        }

       
    }
}
