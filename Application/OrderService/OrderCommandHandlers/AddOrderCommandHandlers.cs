using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class AddOrderCommandHandlers
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderCommand _orderCommand;

        public AddOrderCommandHandlers(IUnitOfWork unitOfWork, IOrderCommand orderCommand)
        {
            _unitOfWork = unitOfWork;
            _orderCommand = orderCommand;
        }
        public async Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill)
        {
            StockMarketMatchEngine stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.ProcessCompleted += bl_ProcessCompleted; // register with an event

            var result = await stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expDate, isFillAndKill);
            

            //bl.StartProcess();
            return 0;
        }

        public void bl_ProcessCompleted(object sender, EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;
            
            switch (result.EventType)
            {
                case EventType.OrderCreated:

                    break;
                case EventType.OrderUpdated:
                    break;
                case EventType.TradeExecuted:
                    break;
                case EventType.OrderCancelled:
                    break;
                case EventType.MarketOpened:
                    break;
                case EventType.MarketClosed:
                    break;
                case EventType.MarketPreOpened:
                    break;
                case EventType.OrderEnqued:
                    break;
                case EventType.OrderExpired:
                    break;
                default:
                    break;
            }
        }

    }
}
