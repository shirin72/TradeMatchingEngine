using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Application.Tests
{
    public class TestCommandHandler : CommandHandler<TestCommand>,ICallCounter
    {
        public int CallCount { get; set; }
        public TestCommandHandler(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
        }

        protected override Task<ProcessedOrder> SpecificHandle(TestCommand? command)
        {
            CallCount++; 
            return Task.FromResult(new ProcessedOrder());
        }
    }
}