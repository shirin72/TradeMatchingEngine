using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;
using Xunit;

namespace Application.Tests
{
    public class AddOrderCommandHandlerTest : AddOrderCommandHandlers
    {

        public AddOrderCommandHandlerTest(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
            this._stockMarketMatchEngine = this._stockMarketFactory.GetStockMarket(this._orderQuery, this._tradeQuery).GetAwaiter().GetResult();
         
        }

        public async Task<ProcessedOrder> SpecificHandle(AddOrderCommand? command)
        {
            return await base.SpecificHandle(command);
        }

    }
}
