using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;
using Xunit;

namespace Application.Tests
{

    public abstract class CommandHandelerTest<THandler, TCommand>
        where THandler : CommandHandler<TCommand>
    {
        protected IUnitOfWork unitOfWorkMock;
        protected IOrderCommandRepository orderCommandRepositoryMock;
        protected IOrderQueryRepository orderQueryRepositoryMock;
        protected ITradeQueryRespository tradeQueryRepositoryMock;
        protected ITradeCommandRepository tradeCommandRepositoryMock;
        protected IStockMarketFactory stockMarketFactoryMock;
        protected IStockMarketMatchEngineWithState stockMarket;
        protected IStockMarketMatchingEngineProcessContext processContext;
        protected THandler sut;

        public CommandHandelerTest()
        {
            unitOfWorkMock = Substitute.For<IUnitOfWork>();
            orderCommandRepositoryMock = Substitute.For<IOrderCommandRepository>();
            orderQueryRepositoryMock = Substitute.For<IOrderQueryRepository>();
            tradeQueryRepositoryMock = Substitute.For<ITradeQueryRespository>();
            tradeCommandRepositoryMock = Substitute.For<ITradeCommandRepository>();
            stockMarketFactoryMock = Substitute.For<IStockMarketFactory>();
            stockMarket = Substitute.For<IStockMarketMatchEngineWithState>();
            processContext = Substitute.For<IStockMarketMatchingEngineProcessContext>();

            sut = (THandler)Activator.CreateInstance(typeof(THandler), unitOfWorkMock,
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

        protected abstract TCommand MakeSomeTCommand();

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
            if (!typeof(ICallCounter).IsAssignableFrom(typeof(THandler))) return;

            //Act
            var actual = await sut.Handle(MakeSomeTCommand());

            //Assert
            var calcounter = (ICallCounter)sut;
            Assert.Equal(1, calcounter.CallCount);
        }

        [Fact]
        public async Task test1()
        {
            //Arrange
            var sut = new ConcurrentDictionary<string, int>();
            sut.AddOrUpdate("http://wwww.google.com", key => 10, (key, ov) => ov);
            sut.AddOrUpdate("http://wwww.linkedin.com", key => 20, (key, ov) => ov);
            var url = "http://wwww.google.com/trades";
            //Act

            var actual = sut.First(i => url.StartsWith(i.Key)).Value;

            //Assert
            Assert.Equal(10, actual);
        }
    }
}
