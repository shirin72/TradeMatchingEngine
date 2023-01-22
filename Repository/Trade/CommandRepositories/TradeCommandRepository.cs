using Repository;
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

        public async Task<long> CreateTrade(TradeMatchingEngine.ITrade Trade)
        {
            await tradeMatchingEngineContext.AddAsync(Trade);

            return Trade.TradeId;
        }
    }
}
