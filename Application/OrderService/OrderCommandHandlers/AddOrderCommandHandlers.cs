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
        private readonly ITradeCommand _tradeCommand;
        private readonly IOrderQuery _orderQuery;
        private readonly ITradeQuery _tradeQuery;
        private static SemaphoreSlim _locker = new SemaphoreSlim(int.MaxValue);
        private static StockMarketMatchEngine _stockMarketMatchEngine;


        public AddOrderCommandHandlers(
            IOrderCommand orderCommand,
            ITradeCommand tradeCommand,
            IOrderQuery orderQuery,
            ITradeQuery tradeQuery
            )
        {
            _orderCommand = orderCommand;
            _tradeCommand = tradeCommand;
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
        }

        public async Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill)
        {
            await getStockMarket();

            _stockMarketMatchEngine.PreOpen();
            _stockMarketMatchEngine.Open();

            var result = await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill, orderCreate: onOrderCreated,tradeCreat: onTradeCompleted);

            return result;
        }

        private async Task onOrderCreated(object sender, EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;

            var castEventObject = result.EventObject as Order;

            await _orderCommand.CreateOrder(castEventObject);
        }

        private async Task onTradeCompleted(object sender, EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;

            var castEventObject = result.EventObject as Trade;

            await _tradeCommand.CreateTrade(castEventObject);
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
            var lastOrderId = await _orderQuery.GetLastOrder();

            _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.ToList(), lastOrderId, getLastTrade);

            _locker.Release();

            return _stockMarketMatchEngine;
        }
    }
}
