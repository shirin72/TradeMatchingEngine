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
            try
            {
                if (tradeMatchingEngineContext.Trades.Count()!=0)
                {
                    var result = await tradeMatchingEngineContext.Trades.MaxAsync(x => x.Id);

                    return result;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public Task<ITrade> GetTradeById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
