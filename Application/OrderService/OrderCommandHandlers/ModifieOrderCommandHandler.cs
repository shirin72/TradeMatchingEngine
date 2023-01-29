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
    public class ModifieOrderCommandHandler : IModifieOrderCommandHandler
    {
        private readonly IOrderCommandRepository _orderCommandRepository;
        private readonly IOrderQueryRepository _orderQuery;
        private readonly ITradeQueryRespository _tradeQuery;
        private readonly ITradeCommandRepository _tradeCommand;
        private readonly IUnitOfWork _unitOfWork;
        private readonly StockMarketUtilities _stockMarketUtilities;

        public ModifieOrderCommandHandler(IOrderCommandRepository orderCommandRepository,
            IOrderQueryRepository orderQuery,
            ITradeQueryRespository tradeQuery,
            ITradeCommandRepository tradeCommand,
            IUnitOfWork unitOfWork,
            StockMarketUtilities stockMarketUtilities)
        {

            _orderCommandRepository = orderCommandRepository;
            _orderQuery = orderQuery;
            _tradeQuery = tradeQuery;
            _tradeCommand = tradeCommand;
            _unitOfWork = unitOfWork;
            _stockMarketUtilities = stockMarketUtilities;
        }
        public async Task<long?> Handle(long orderId, int price, int amount, DateTime? expDate)
        {
            var stockMarketEngine = await _stockMarketUtilities.GetStockMarket();


            var events = new StockMarketEvents()
            {
                OnOrderCreated = _stockMarketUtilities.onOrderCreated,
                OnTradeCreated = _stockMarketUtilities.onTradeCreated,
                OnOrderModified = _stockMarketUtilities.onOrderModified,
            };

            var result = await stockMarketEngine.ModifieOrder(orderId, price, amount, expDate, events);

            await _unitOfWork.SaveChange();

            return result;
        }

    }
}
