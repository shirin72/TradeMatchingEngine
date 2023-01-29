using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Application.Utilities
{
    public class StockMarketUtilities
    {
        private readonly IOrderQueryRepository _orderQuery;
        private readonly ITradeQueryRespository _tradeQuery;
        private readonly IOrderCommandRepository orderCommandRepository;
        private readonly ITradeCommandRepository tradeCommandRepository;
        private static SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        private static StockMarketMatchEngine _stockMarketMatchEngine;

        public StockMarketUtilities(
            IOrderQueryRepository orderQuery,
            ITradeQueryRespository tradeQuery,
            IOrderCommandRepository orderCommandRepository,
            ITradeCommandRepository tradeCommandRepository
            )
        {
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
            this.orderCommandRepository = orderCommandRepository;
            this.tradeCommandRepository = tradeCommandRepository;
        }

        public async Task onOrderModified(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        {
            var founndOrder = await orderCommandRepository.Find(order.Id);

            founndOrder.UpdateBy(order);
        }

        public async Task onOrderCreated(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        {
            await orderCommandRepository.Add(order);
        }

        public async Task onTradeCreated(StockMarketMatchEngine stockMarketMatchEngine, Trade trade)
        {
            await tradeCommandRepository.Add(trade);
        }


        public async Task<StockMarketMatchEngine> GetStockMarket()
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

                var getOrders = await _orderQuery.GetAll();
                var lastOrderId = await _orderQuery.GetLastId();
                var getLastTrade = await _tradeQuery.GetLastId();
                _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.ToList(), lastOrderId, getLastTrade);
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
