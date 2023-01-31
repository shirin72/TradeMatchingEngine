using Application.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class ModifieOrderCommandHandler : CommandHandler<ModifieOrderCommand>, IModifieOrderCommandHandler
    {
        public ModifieOrderCommandHandler(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
        }

        protected async override Task<long> SpecificHandle(ModifieOrderCommand? command, StockMarketEvents? events = null)
        {
            return await this._stockMarketMatchEngine.ModifieOrder(command.OrderId, command.Price, command.Amount, command.ExpDate, events) ?? 0;
        }
    }
}
