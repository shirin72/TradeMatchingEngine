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
        private static readonly object _locker = new object();
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
            var resultOfGetStockMarket = await getStockMarket();

            await SubscribeToEvent(resultOfGetStockMarket);
            resultOfGetStockMarket.PreOpen();
            resultOfGetStockMarket.Open();

            return await resultOfGetStockMarket.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);
        }

        private async Task SubscribeToEvent(StockMarketMatchEngine stockMarketMatchEngine)
        {
            stockMarketMatchEngine.OrderCreated += async (sender, args) => await this.OnOrderCreated(sender, args);
            stockMarketMatchEngine.TradeCompleted += async (sender, args) => await this.OnTradeCompleted(sender, args);
        }

        public async Task OnOrderCreated(object sender, EventArgs eventArgs)
        {
            try
            {
                var result = eventArgs as StockMarketMatchEngineEvents;

                var castEventObject = result.EventObject as Order;

                await _orderCommand.CreateOrder(castEventObject);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        public async Task OnTradeCompleted(object sender, EventArgs eventArgs)
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

            lock (_locker)
            {
                if (_stockMarketMatchEngine != null)
                {
                    return _stockMarketMatchEngine;
                }

                var getOrders = _orderQuery.GetAllOrders();
                var getLastTrade = _tradeQuery.GetLastTrade();
                return _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders != null ? getOrders.ToList() : new List<Order>(), getOrders.Max(x => x.Id), getLastTrade.Result);
            }
        }
    }
}
