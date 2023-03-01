using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using System.Threading.Tasks;
using Domain;
using Domain.Orders.Repositories.Command;
using Domain.Orders.Repositories.Query;
using Domain.Trades.Repositories.Command;
using Domain.Trades.Repositories.Query;
using Domain.UnitOfWork;

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