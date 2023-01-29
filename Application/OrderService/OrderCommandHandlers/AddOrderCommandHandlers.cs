using Application.Utilities;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class AddOrderCommandHandlers : IAddOrderCommandHandlers
    {
        private readonly IOrderCommandRepository _orderCommandRepository;
        private readonly IOrderQueryRepository _orderQuery;
        private readonly ITradeQueryRespository _tradeQuery;
        private readonly ITradeCommandRepository _tradeCommand;
        private readonly IUnitOfWork _unitOfWork;
        private static SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        //private static StockMarketMatchEngine _stockMarketMatchEngine;
        private readonly StockMarketUtilities _stockMarketUtilities;

        public AddOrderCommandHandlers(
            IOrderCommandRepository orderCommandRepository,
            IOrderQueryRepository orderQuery,
            ITradeQueryRespository tradeQuery,
            ITradeCommandRepository tradeCommand,
            IUnitOfWork unitOfWork,
            StockMarketUtilities stockMarketUtilities
            )
        {
            _orderCommandRepository = orderCommandRepository;
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
            _tradeCommand = tradeCommand;
            _unitOfWork = unitOfWork;
            _stockMarketUtilities = stockMarketUtilities;
        }

        public async Task<long> Handle(int price, int amount, Side side, DateTime? expDate, bool isFillAndKill)
        {
            var sm = await _stockMarketUtilities.GetStockMarket();

            var events = new StockMarketEvents()
            {
                OnOrderCreated =_stockMarketUtilities.onOrderCreated,
                OnTradeCreated = _stockMarketUtilities.onTradeCreated,
                OnOrderModified = _stockMarketUtilities.onOrderModified,
            };


            var result = await sm.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill, events: events);

            await _unitOfWork.SaveChange();

            return result;
        }

        #region Private
        //public async Task onOrderModified(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        //{
        //    var founndOrder = await _orderCommandRepository.Find(order.Id);

        //    founndOrder.UpdateBy(order);
        //}

        //public async Task onOrderCreated(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        //{
        //    await _orderCommandRepository.Add(order);
        //}

        //public async Task onTradeCreated(StockMarketMatchEngine stockMarketMatchEngine, Trade trade)
        //{
        //    await _tradeCommand.Add(trade);
        //}

        //public async Task<StockMarketMatchEngine> getStockMarket()
        //{
        //    if (_stockMarketMatchEngine != null)
        //    {
        //        return _stockMarketMatchEngine;
        //    }

        //    await _locker.WaitAsync();
        //    try
        //    {
        //        if (_stockMarketMatchEngine != null)
        //        {
        //            return _stockMarketMatchEngine;
        //        }

        //        var getOrders = await _orderQuery.GetAll();
        //        var lastOrderId = await _orderQuery.GetLastId();
        //        var getLastTrade = await _tradeQuery.GetLastId();
        //        _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.ToList(), lastOrderId, getLastTrade);
        //        _stockMarketMatchEngine.PreOpen();
        //        _stockMarketMatchEngine.Open();
        //    }
        //    finally
        //    {
        //        _locker.Release();
        //    }


        //    return _stockMarketMatchEngine;
        //}
        #endregion
    }
}
