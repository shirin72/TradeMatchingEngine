using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using Xunit;

namespace Application.Tests
{
    public class AddOrderCommandHandlerTests : CommandHandelerTest<AddOrderCommandHandlers, AddOrderCommand>
    {
        public AddOrderCommandHandlerTests()
        {
            stockMarketFactoryMock.GetStockMarket(orderQueryRepositoryMock, tradeQueryRepositoryMock).Returns(stockMarket);
        }


        [Fact]
        public async Task Handle_Should_Call_OrderCommandRepository_Add()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            //Act
            await sut.Handle(addOrderCommand);

            //Assert
            await orderCommandRepositoryMock.Received(1).Add(processContext.Order);
        }

        [Fact]
        public async Task Handle_Should_Call_ProcessOrderAsync()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            //Act
            await sut.Handle(addOrderCommand);

            //Assert
            await stockMarket.Received(1).ProcessOrderAsync(
               Arg.Is(addOrderCommand.Price),
               Arg.Is(addOrderCommand.Amount),
               Arg.Is(addOrderCommand.Side),
               Arg.Is(addOrderCommand.ExpDate),
               Arg.Is(addOrderCommand.IsFillAndKill),
               Arg.Is(addOrderCommand.orderParentId)
               );
        }

        [Fact]
        public async Task Handle_Should_Call_TradeCommandRepository_Add()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            processContext.CreatedTrades.Returns(new List<Trade> { new Trade(1, 1, 2, 10, 10) });
            stockMarket.ProcessOrderAsync(Arg.Is(addOrderCommand.Price),
               Arg.Is(addOrderCommand.Amount),
               Arg.Is(addOrderCommand.Side),
               Arg.Is(addOrderCommand.ExpDate),
               Arg.Is(addOrderCommand.IsFillAndKill),
               Arg.Is(addOrderCommand.orderParentId)).Returns(processContext);

            //Act
            await sut.Handle(addOrderCommand);
            var trade = processContext.CreatedTrades.ToList().FirstOrDefault();

            //Assert
            await tradeCommandRepositoryMock.Received(1).Add(trade);
        }


        [Fact]
        public async Task Handle_Should_Call_OrderCommandRepository_Find()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            var order = new Order(1, addOrderCommand.Side, addOrderCommand.Price, addOrderCommand.Amount, (DateTime)addOrderCommand.ExpDate, OrderStates.Register, orderParentId: addOrderCommand.orderParentId);

            processContext.ModifiedOrders.Returns(new List<Order> { order });

            stockMarket.ProcessOrderAsync(Arg.Is(addOrderCommand.Price),
               Arg.Is(addOrderCommand.Amount),
               Arg.Is(addOrderCommand.Side),
               Arg.Is(addOrderCommand.ExpDate),
               Arg.Is(addOrderCommand.IsFillAndKill),
               Arg.Is(addOrderCommand.orderParentId)).Returns(processContext);

            orderCommandRepositoryMock.Find(Arg.Any<long>()).Returns(order);

            //Act
            await sut.Handle(addOrderCommand);


            //Assert
            await orderCommandRepositoryMock.Received(1).Find(processContext.ModifiedOrders.ToList().FirstOrDefault().Id);
        }

        protected override AddOrderCommand MakeSomeTCommand()
        {
            return new AddOrderCommand
            {
                Amount = 10,
                ExpDate = new DateTime(2050, 1, 1),
                IsFillAndKill = false,
                Price = 10,
                Side = Side.Buy,
                orderParentId = 1,
                events = new StockMarketEvents()
            };
        }
    }
}