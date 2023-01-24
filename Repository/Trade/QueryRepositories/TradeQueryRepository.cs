using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine;
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

        public async Task<long> GetLastTrade()
        {
            if (tradeMatchingEngineContext.Trades.Any())
            {
                return await tradeMatchingEngineContext.Trades.MaxAsync(x => x.Id);
            }

            return 0;
        }

        public Task<ITrade> GetTradeById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
