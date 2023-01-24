using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Application.OrderService.OrderCommandHandlers
{
    public class AddOrderCommandHandlers : IAddOrderCommandHandlers
    {
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;
        private readonly ITradeQuery _tradeQuery;
        private static SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        private static StockMarketMatchEngine _stockMarketMatchEngine;

        public AddOrderCommandHandlers(
            IOrderCommand orderCommand,
            IOrderQuery orderQuery,
            ITradeQuery tradeQuery
            )
        {
            _orderCommand = orderCommand;
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
        }

        public async Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill)
        {
            await getStockMarket();
            await SubscribeToEvent();
            _stockMarketMatchEngine.PreOpen();
            _stockMarketMatchEngine.Open();

            var result = await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);

            return result;
        }

        public async Task OnOrderCreated(object sender, StockMarketMatchEngineEvents eventArgs)
        {
            try
            {
                var castEventObject = eventArgs.EventObject as Order;

                await _orderCommand.CreateOrder(castEventObject);

                _stockMarketMatchEngine.OrderCreated -= OnOrderCreated;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SubscribeToEvent()
        {
            await _locker.WaitAsync();
            _stockMarketMatchEngine.OrderCreated += async (sender, args) => await OnOrderCreated(this, args).ConfigureAwait(false);
            _locker.Release();
        }

        private async Task<StockMarketMatchEngine> getStockMarket()
        {
            if (_stockMarketMatchEngine != null)
            {
                return _stockMarketMatchEngine;
            }

            await _locker.WaitAsync();

            if (_stockMarketMatchEngine != null)
            {
                return _stockMarketMatchEngine;
            }

            var getOrders = await _orderQuery.GetAllOrders();
            var getLastTrade = await _tradeQuery.GetLastTrade();

            int lastOrderId = 0;
            if (getOrders.Any())
            {
                lastOrderId = getOrders.Max(x => x.Id);
            }

            _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.ToList(), lastOrderId, getLastTrade);

            _locker.Release();

            return _stockMarketMatchEngine;
        }
    }
}
