using Domain;
using Domain.Orders.Repositories.Query;
using Domain.Trades.Repositories.Query;

namespace Application.Factories
{
    public class StockMarketFactory : IStockMarketFactory
    {
        private SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        private StockMarketMatchEngineStateProxy _stockMarketMatchEngine;


        public virtual async Task<IStockMarketMatchEngineWithState> GetStockMarket(IOrderQueryRepository orderQueryRep, ITradeQueryRespository tradeQueryRep)
        {
            if (_stockMarketMatchEngine != null)
            {
                return _stockMarketMatchEngine;
            }

            await _locker.WaitAsync();
            try
            {
                if (_stockMarketMatchEngine != null)
                {
                    return _stockMarketMatchEngine;
                }

                var getOrders = await orderQueryRep.GetAll(x => x.Amount != 0 && x.OrderState != OrderStates.Cancell);
                var lastOrderId = await orderQueryRep.GetMax(o => o.Id);
                var getLastTrade = await tradeQueryRep.GetMax(t => t.Id);
                _stockMarketMatchEngine = new StockMarketMatchEngineStateProxy(getOrders.Select(x => (Order)x).ToList(), lastOrderId, getLastTrade);
                _stockMarketMatchEngine.PreOpen();
                _stockMarketMatchEngine.Open();
            }
            finally
            {
                _locker.Release();
            }


            return _stockMarketMatchEngine;
        }
    }
}
