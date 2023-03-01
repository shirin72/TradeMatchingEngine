using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Orders.Commands;
using Xunit;

namespace Application.Tests
{
    public partial class AddOrderCommandHandlerTests : CommandHandelerTest<AddOrderCommandHandlers, AddOrderCommand>
    {
        private static readonly int SOME_AMOUNT = 10;
        private static readonly DateTime SOME_EXPIRATION_DATE = new DateTime(2050, 1, 1);
        private static readonly bool SOME_IS_FILL_AND_KILL = false;
        private static readonly int SOME_PRICE = 10;
        private static readonly Side SOME_SIDE = Side.Buy;
        private static readonly int SOME_ORDER_PARENT_ID = 1;
        private static readonly int SOME_BUY_ORDER_ID = 1;
        private static readonly int SOME_SELL_ORDER_ID = 1;
        private static readonly int SOME_ORDER_ID = 1;
        private static readonly OrderStates SOME_ORDER_STATE = OrderStates.Register;
        public AddOrderCommandHandlerTests()
        {
            stockMarketFactoryMock.GetStockMarket(orderQueryRepositoryMock, tradeQueryRepositoryMock).Returns(stockMarket);
        }


        [Fact]
        public async Task Handle_Should_Call_OrderCommandRepository_Add()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            processContext.Order.Returns(new TestOrder
            {
                Id = SOME_ORDER_ID,
                Amount = SOME_AMOUNT,
                ExpireTime = SOME_EXPIRATION_DATE,
                IsFillAndKill = SOME_IS_FILL_AND_KILL,
                OrderParentId = SOME_ORDER_PARENT_ID,
                OrderState = SOME_ORDER_STATE,
                Price = SOME_PRICE,
                Side = SOME_SIDE
            });

            stockMarket.ProcessOrderAsync(
               SOME_PRICE,
               SOME_AMOUNT,
               SOME_SIDE,
               SOME_EXPIRATION_DATE,
               SOME_IS_FILL_AND_KILL,
               SOME_ORDER_PARENT_ID).Returns(processContext);

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
              SOME_PRICE,
               SOME_AMOUNT,
               SOME_SIDE,
              SOME_EXPIRATION_DATE,
               SOME_IS_FILL_AND_KILL,
              SOME_ORDER_PARENT_ID
               );
        }

        [Fact]
        public async Task Handle_Should_Call_TradeCommandRepository_Add()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            processContext.CreatedTrades.Returns(
            new List<TestTrade> {
                new TestTrade {
                Amount = SOME_AMOUNT,
                BuyOrderId =SOME_BUY_ORDER_ID,
                Id=SOME_ORDER_ID,
                Price = SOME_PRICE,
                SellOrderId=SOME_SELL_ORDER_ID } });

            stockMarket.ProcessOrderAsync(
            SOME_PRICE,
            SOME_AMOUNT,
            SOME_SIDE,
            SOME_EXPIRATION_DATE,
            SOME_IS_FILL_AND_KILL,
            SOME_ORDER_PARENT_ID).Returns(processContext);

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

            var order = new TestOrder
            {
                Id = SOME_ORDER_ID,
                Amount = SOME_AMOUNT,
                ExpireTime = SOME_EXPIRATION_DATE,
                IsFillAndKill = SOME_IS_FILL_AND_KILL,
                OrderParentId = SOME_ORDER_PARENT_ID,
                OrderState = SOME_ORDER_STATE,
                Price = SOME_PRICE,
                Side = SOME_SIDE
            };

            processContext.ModifiedOrders.Returns(new List<TestOrder> { order });

            stockMarket.ProcessOrderAsync(
            SOME_PRICE,
            SOME_AMOUNT,
            SOME_SIDE,
            SOME_EXPIRATION_DATE,
            SOME_IS_FILL_AND_KILL,
            SOME_ORDER_PARENT_ID).Returns(processContext);

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
                Amount = SOME_AMOUNT,
                ExpDate = SOME_EXPIRATION_DATE,
                IsFillAndKill = SOME_IS_FILL_AND_KILL,
                Price = SOME_PRICE,
                Side = SOME_SIDE,
                orderParentId = SOME_ORDER_PARENT_ID,
            };
        }
    }
}