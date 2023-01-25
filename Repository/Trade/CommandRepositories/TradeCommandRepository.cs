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

        public async Task AddTrade(TradeMatchingEngine.Trade Trade)
        {
             await tradeMatchingEngineContext.Trades.AddAsync(Trade);
        }
    }
}
