using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
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

            //var modifiedOrder = new List<Order>();
            //modifiedOrder.Add(
            //    new Order(Arg.Any<long>(),
            //    Arg.Any<Side>(),
            //    Arg.Any<int>(),
            //    Arg.Any<int>(),
            //    Arg.Any<DateTime>(),
            //    Arg.Any<OrderStates>()));

            //stockMarketFactoryMock.GetStockMarket(Arg.Is<IOrderQueryRepository>(orderQueryRepositoryMock), Arg.Is<ITradeQueryRespository>(tradeQueryRepositoryMock)).Returns(stockMarket);

            sut = (THandler)Activator.CreateInstance(typeof(THandler), unitOfWorkMock,
               stockMarketFactoryMock,
               orderCommandRepositoryMock,
               orderQueryRepositoryMock,
                tradeCommandRepositoryMock,
               tradeQueryRepositoryMock
               );

            //stockMarket.ProcessOrderAsync(10, 10, Side.Buy, null, false, 1).Returns(processContext);
            //processContext.Order.Returns(new Order(Arg.Is<long>(1), Arg.Is<Side>(Side.Buy), Arg.Is<int>(10), Arg.Is<int>(10), Arg.Is<DateTime>(DateTime.Now.AddDays(1)), Arg.Is<OrderStates>(OrderStates.Register)));
            //processContext.ModifiedOrders.Returns(modifiedOrder);

            //stockMarket.ProcessOrderAsync(Arg.Is<int>(10), Arg.Is<int>(10), Arg.Is<Side>(Side.Buy), Arg.Is<DateTime>(DateTime.Now.AddDays(1)), Arg.Is<bool>(false), Arg.Is<long>(null)).Returns(processContext);
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
    }
}
