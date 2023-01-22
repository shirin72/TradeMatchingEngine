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
        public Task<int> SaveChange()
        {
            return tradeMatchingEngineContext.SaveChangesAsync();
        }
    }
}
