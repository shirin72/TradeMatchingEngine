using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Dto;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Dto;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class AddOrderCommandHandlers : IAddOrderCommandHandlers
    {
        private readonly IOrderCommand _orderCommand;
        private readonly ITradeCommand _tradeCommand;
        private readonly IOrderQuery _orderQuery;

        public AddOrderCommandHandlers(IOrderCommand orderCommand, ITradeCommand tradeCommand, IOrderQuery orderQuery)
        {
            _orderCommand = orderCommand;
            _tradeCommand = tradeCommand;
            _orderQuery = orderQuery;
        }

        public async Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill)
        {
            var getOrders = await _orderQuery.GetAllOrders();

            var orderList = new List<Order>();

            foreach (var item in getOrders.ToList())
            {
                orderList.Add(new Order(item.Id, Enum.Parse<Side>(item.Side), item.Price, item.Amount, item.ExpireTime, item.IsFillAndKill, item.OrderParentId));
            }

            var stockMarketMatchEngine = new StockMarketMatchEngine(orderList);

            SubscribeToEvent(stockMarketMatchEngine);
            stockMarketMatchEngine.PreOpen();
            stockMarketMatchEngine.Open();

            var result = await stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);

            return result;
        }

        private void SubscribeToEvent(StockMarketMatchEngine stockMarketMatchEngine)
        {
            stockMarketMatchEngine.OrderCreated += OnOrderCreated;
            stockMarketMatchEngine.TradeCompleted += OnTradeCompleted;
        }

        public void OnOrderCreated(object sender, EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;

            var castEventObject = result.EventObject as Order;

            var orderDto = new OrderDto()
            {
                Amount = castEventObject.Amount,
                Side = castEventObject.Side.ToString(),
                ExpireTime = castEventObject.ExpireTime,
                Id = castEventObject.Id,
                OrderParentId = castEventObject.OrderParentId,
                Price = castEventObject.Price,

            };
            _orderCommand.CreateOrder(orderDto);
        }

        public void OnTradeCompleted(object sender, EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;

            // var castEventObject = result.EventObject as ITrade;
            var castEventObject =(Trade) result.EventObject;

            var tradeDto = new TradeDto()
            {
                Amount = castEventObject.Amount,
                BuyOrderId = castEventObject.BuyOrderId,
                Id = castEventObject.TradeId,
                OwnerId = castEventObject.OwnerId,
                Price = castEventObject.Price,
                SellOrderId = castEventObject.SellOrderId,
            };

            _tradeCommand.CreateTrade(tradeDto);
        }

    }
}
