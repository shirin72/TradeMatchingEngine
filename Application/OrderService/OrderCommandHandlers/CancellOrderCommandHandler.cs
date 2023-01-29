using Application.Utilities;
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
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class CancellOrderCommandHandler : ICancellOrderCommandHandler
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly StockMarketUtilities _stockMarketUtilities;

        public CancellOrderCommandHandler(IOrderCommandRepository orderCommandRepository,
            IUnitOfWork unitOfWork,
            StockMarketUtilities stockMarketUtilities)
        {
            _unitOfWork = unitOfWork;
            _stockMarketUtilities = stockMarketUtilities;
        }

        public async Task<long?> Handle(long orderId)
        {
            var stockMarketEngine = await _stockMarketUtilities.GetStockMarket();


            var events = new StockMarketEvents()
            {
                OnOrderCreated = _stockMarketUtilities.onOrderCreated,
                OnTradeCreated = _stockMarketUtilities.onTradeCreated,
                OnOrderModified = _stockMarketUtilities.onOrderModified,
            };

            var result = await stockMarketEngine.CancelOrderAsync(orderId, events);

            await _unitOfWork.SaveChange();

            return result;
        }
    }
}
