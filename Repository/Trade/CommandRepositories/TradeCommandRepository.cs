using Repository;
using TradeMatchingEngine.Trades.Dto;
using TradeMatchingEngine.Trades.Repositories.Command;

namespace Infrastructure.Trade.QueryRepositories
{
    public class TradeCommandRepository : ITradeCommand
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public TradeCommandRepository(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async Task<long> CreateTrade(TradeDto Trade)
        {
            try
            {
                var res1 = await tradeMatchingEngineContext.AddAsync(Trade);
                var res2 = await tradeMatchingEngineContext.SaveChangesAsync();

                return Trade.Id;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
