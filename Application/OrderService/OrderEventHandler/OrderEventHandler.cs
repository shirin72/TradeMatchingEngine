using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Application.OrderService.OrderEventHandler
{
    public class OrderEventHandler
    {
    //    private readonly IOrderCommand _orderCommand;
    //    private StockMarketMatchEngine _stockMarketMatchEngine;
    //    public OrderEventHandler(
    //        IOrderCommand orderCommand,
    //        StockMarketMatchEngine stockMarketMatchEngine
    //        )
    //    {
    //        _orderCommand = orderCommand;
    //        _stockMarketMatchEngine= stockMarketMatchEngine;
    //        _stockMarketMatchEngine.OrderCreated += OnOrderCreated;
    //    }

    //    public async Task OnOrderCreated(object sender, StockMarketMatchEngineEvents eventArgs)
    //    {
    //        try
    //        {
    //            var castEventObject = eventArgs.EventObject as Order;

    //            await _orderCommand.CreateOrder(castEventObject);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }
    }
}
