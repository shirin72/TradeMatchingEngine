using TradeMatchingEngine.UnitOfWork;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public UnitOfWork(TradeMatchingEngineContext tradeMatchingEngineContext)
        {
            this.tradeMatchingEngineContext = tradeMatchingEngineContext;
        }

        public async ValueTask DisposeAsync()
        {
            await tradeMatchingEngineContext.DisposeAsync();
        }

        public Task<int> SaveChange()
        {
            var result = tradeMatchingEngineContext.SaveChangesAsync();
            return result;
        }
    }
}
