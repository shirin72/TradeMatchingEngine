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
            //var resultOfGetStockMarket = await getStockMarket();



            //return 
            if (_stockMarketMatchEngine != null)
            {
                //return _stockMarketMatchEngine;
                var getOrders = _orderQuery.GetAllOrders();
                var getLastTrade = _tradeQuery.GetLastTrade();
                //_stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.Result.Count() != 0 ? getOrders.Result.ToList() : new List<Order>(), getOrders.Result.Max(x => x.Id), getLastTrade.Result);
                SubscribeToEvent(_stockMarketMatchEngine);
                return await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);
            }

            lock (_locker)
            {
                if (_stockMarketMatchEngine != null)
                {
                    // return _stockMarketMatchEngine;
                    //var getOrders1 = _orderQuery.GetAllOrders();
                    //var getLastTrade1 = _tradeQuery.GetLastTrade();
                    //_stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.Result.Count() != 0 ? getOrders.Result.ToList() : new List<Order>(), getOrders.Result.Max(x => x.Id), getLastTrade.Result);
                    SubscribeToEvent(_stockMarketMatchEngine);
                    //return await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);
                }

                var getOrders = _orderQuery.GetAllOrders();
                var getLastTrade = _tradeQuery.GetLastTrade();
                _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.Result.Count() != 0 ? getOrders.Result.ToList() : new List<Order>(), getOrders.Result.Max(x => x.Id), getLastTrade.Result);
                SubscribeToEvent(_stockMarketMatchEngine);
                _stockMarketMatchEngine.PreOpen();
                _stockMarketMatchEngine.Open();
                   var result= _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);
                return result.Result;
               // return 

            }

        }

        private void SubscribeToEvent(StockMarketMatchEngine stockMarketMatchEngine)
        {
            stockMarketMatchEngine.OrderCreated += OnOrderCreated;
            //stockMarketMatchEngine.TradeCompleted += async (sender, args) => await OnTradeCompleted(this, args);
        }

        public void OnOrderCreated(object sender, EventArgs eventArgs)
        {
            try
            {
                var result = eventArgs as StockMarketMatchEngineEvents;

                Task.Run(() =>
              {
                  _orderCommand.CreateOrder(result.EventObject as Order);
              });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task OnTradeCompleted(object sender, EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;

            await _tradeCommand.CreateTrade(result.EventObject as Trade);
        }

        //private async Task<StockMarketMatchEngine> getStockMarket()
        //{
            //if (_stockMarketMatchEngine != null)
            //{
            //    return _stockMarketMatchEngine;
            //}

            //lock (_locker)
            //{
            //    if (_stockMarketMatchEngine != null)
            //    {
            //        return _stockMarketMatchEngine;
            //    }

            //    var getOrders = _orderQuery.GetAllOrders();
            //    var getLastTrade = _tradeQuery.GetLastTrade();
            //    _stockMarketMatchEngine = new StockMarketMatchEngine(getOrders.Result.Count() != 0 ? getOrders.Result.ToList() : new List<Order>(), getOrders.Result.Max(x => x.Id), getLastTrade.Result);
            //    SubscribeToEvent(_stockMarketMatchEngine);
            //    _stockMarketMatchEngine.PreOpen();
            //    _stockMarketMatchEngine.Open();
            //   var result= await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);
            //    return _stockMarketMatchEngine;

            //}
        //}
    }
}
