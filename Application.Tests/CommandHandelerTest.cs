using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using TradeMatchingEngine.Orders.Commands;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;
using Xunit;

namespace Application.Tests
{
    public class TestCommandHandlerTests : CommandHandelerTest<TestCommandHandler, TestCommand>
    {

    }
    public abstract class CommandHandelerTest<THandler,TCommand> 
        where THandler : CommandHandler<TCommand> 
    {
        private IUnitOfWork unitOfWorkMock;
        private IOrderCommandRepository orderCommandRepositoryMock;
        private IOrderQueryRepository orderQueryRepositoryMock;
        private ITradeQueryRespository tradeQueryRepositoryMock;
        private ITradeCommandRepository tradeCommandRepositoryMock;
        private IStockMarketFactory stockMarketFactoryMock;
        protected THandler sut;
        public CommandHandelerTest()
        {
             unitOfWorkMock = Substitute.For<IUnitOfWork>();
             orderCommandRepositoryMock = Substitute.For<IOrderCommandRepository>();
             orderQueryRepositoryMock = Substitute.For<IOrderQueryRepository>();
             tradeQueryRepositoryMock = Substitute.For<ITradeQueryRespository>();
             tradeCommandRepositoryMock = Substitute.For<ITradeCommandRepository>();
             stockMarketFactoryMock = Substitute.For<IStockMarketFactory>();
             sut =(THandler)Activator.CreateInstance(typeof(THandler), unitOfWorkMock,
                stockMarketFactoryMock,
                orderCommandRepositoryMock,
                orderQueryRepositoryMock,
                 tradeCommandRepositoryMock,
                tradeQueryRepositoryMock
                );
        }
        [Fact]
        public async Task Handle_Should_Call_GetStockMarket_On_StockMarketFactory_TestAsync()
        {
            //Arrange

            //Act
            var actual = await sut.Handle(MakeSomeTCommand());
            //Assert
            stockMarketFactoryMock.Received(1).GetStockMarket(orderQueryRepositoryMock, tradeQueryRepositoryMock);
        }

        private static TCommand MakeSomeTCommand() => Activator.CreateInstance<TCommand>();

        [Fact]
        public async Task Handle_Should_Call_SaveChange_On_UnitOfWork_TestAsync()
        {
            //Arrange
          
            //Act
            var actual = await sut.Handle(MakeSomeTCommand());
            //Assert
            unitOfWorkMock.Received(1).SaveChange();
        }

        [Fact]
        public async Task Handle_Should_Call_SpecificHandle_On_Sut_TestAsync()
        {
            //Arrange

            //Act
            var actual = await sut.Handle(MakeSomeTCommand());
            //Assert

            Assert.Equal(1,sut.CallCount);
        }
    }
}