using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Application.OrderService.OrderEventHandler
{
    public class OrderEventHandler
    {
        private readonly IOrderCommand _orderCommand;
        public OrderEventHandler(
            IOrderCommand orderCommand
            )
        {
            _orderCommand = orderCommand;
        }

        public async Task OnOrderCreated(object sender, EventArgs eventArgs)
        {
            try
            {
                var result = eventArgs as StockMarketMatchEngineEvents;

                var castEventObject = result.EventObject as Order;

                //await _orderCommand.CreateOrder(castEventObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
