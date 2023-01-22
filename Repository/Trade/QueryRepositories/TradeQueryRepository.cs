using Microsoft.EntityFrameworkCore;
using Repository;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Infrastructure.Order.CommandRepositories
{
    public class TradeQueryRepository : ITradeQuery
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public TradeQueryRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }


        public Task<ITrade> GetAllTrades()
        {
            throw new NotImplementedException();
        }

        public Task<ITrade> GetTradeById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
