﻿using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Application.Factories
{
    public class StockMarketFactory : IStockMarketFactory
    {
        private SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        private StockMarketMatchEngineStateProxy _stockMarketMatchEngine;


        public async Task<IStockMarketMatchEngineWithState> GetStockMarket(IOrderQueryRepository orderQueryRep, ITradeQueryRespository tradeQueryRep)
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

                var getOrders = await orderQueryRep.GetAll(x => x.Amount != 0);
                var lastOrderId = await orderQueryRep.GetMax(o => o.Id);
                var getLastTrade = await tradeQueryRep.GetMax(t => t.Id);
                _stockMarketMatchEngine = new StockMarketMatchEngineStateProxy(getOrders.ToList(), lastOrderId, getLastTrade);
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