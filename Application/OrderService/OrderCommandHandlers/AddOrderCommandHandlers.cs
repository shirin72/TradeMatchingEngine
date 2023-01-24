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
        private readonly OrderEventHandler.OrderEventHandler _orderEventHandler;


        public AddOrderCommandHandlers(
            IOrderCommand orderCommand,
            ITradeCommand tradeCommand,
            IOrderQuery orderQuery,
            ITradeQuery tradeQuery,
            OrderEventHandler.OrderEventHandler orderEventHandler

            )
        {
            _orderCommand = orderCommand;
            _tradeCommand = tradeCommand;
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
            _orderEventHandler = orderEventHandler;
        }

        public async Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill)
        {
            await getStockMarket();

            _stockMarketMatchEngine.PreOpen();
            _stockMarketMatchEngine.Open();

            await SubscribeToEvent();

            var result= await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill));
        
            return result;
        }

        private async Task SubscribeToEvent()
        {
           _stockMarketMatchEngine.OrderCreated += async (sender, args) => await _orderEventHandler.OnOrderCreated(this, args);

            //_stockMarketMatchEngine.TradeCompleted += async (sender, args) => await OnTradeCompleted(this, args);
        }

      

        //public async Task OnTradeCompleted(object sender, EventArgs eventArgs)
        //{
        //    var result = eventArgs as StockMarketMatchEngineEvents;

        //    var castEventObject = result.EventObject as Trade;

        //    await _tradeCommand.CreateTrade(castEventObject);
        //}

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
