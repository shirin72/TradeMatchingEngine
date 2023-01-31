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

        public async Task<int> SaveChange()
        {
            try
            {
                var result = await tradeMatchingEngineContext.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}
