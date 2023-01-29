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
        private readonly IOrderQuery _orderQuery;
        private readonly ITradeQuery _tradeQuery;
        private readonly ITradeCommandRepository _tradeCommand;
        private readonly IUnitOfWork _unitOfWork;
        private static SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        private static StockMarketMatchEngine _stockMarketMatchEngine;

        public AddOrderCommandHandlers(
            IOrderCommandRepository orderCommandRepository,
            IOrderQuery orderQuery,
            ITradeQuery tradeQuery,
            ITradeCommandRepository tradeCommand,
            IUnitOfWork unitOfWork
            )
        {
            _orderCommandRepository = orderCommandRepository;
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
            _tradeCommand = tradeCommand;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill)
        {
            var sm = await getStockMarket();

            var events = new StockMarketEvents()
            {
                OnOrderCreated = onOrderCreated,
                OnTradeCreated = onTradeCreated,
                OnOrderModified = onOrderModified,
            };


            var result = await sm.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill, events: events);

            await _unitOfWork.SaveChange();

            return result;
        }


        #region Private
        private async Task onOrderModified(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        {
            var founndOrder = await _orderCommandRepository.Find(order.Id);

            founndOrder.UpdateBy(order);
        }

        private async Task onOrderCreated(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        {
            await _orderCommandRepository.Add(order);
        }

        private async Task onTradeCreated(StockMarketMatchEngine stockMarketMatchEngine, Trade trade)
        {
            await _tradeCommand.Add(trade);
        }

        private async Task<StockMarketMatchEngine> getStockMarket()
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

                var getOrders = await _orderQuery.GetAllOrders();
                var lastOrderId = await _orderQuery.GetLastOrder();
                var getLastTrade = await _tradeQuery.GetLastTrade();
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
        #endregion
    }
}
