using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using Xunit;
using static Application.Tests.AddOrderCommandHandlerTests;

namespace Application.Tests
{
    public class ModifieOrderCommandHandlerTest : CommandHandelerTest<ModifieOrderCommandHandler, ModifieOrderCommand>
    {
        private static readonly int SOME_AMOUNT = 10;
        private static readonly DateTime SOME_EXPIRATION_DATE = new DateTime(2050, 1, 1);
        private static readonly bool SOME_IS_FILL_AND_KILL = false;
        private static readonly int SOME_PRICE = 10;
        private static readonly Side SOME_SIDE = Side.Buy;
        private static readonly int SOME_ORDER_PARENT_ID = 1;
        private static readonly int SOME_ORDER_ID = 1;
        private static readonly int SOME_BUY_ORDER_ID = 1;
        private static readonly int SOME_SELL_ORDER_ID = 1;
        public ModifieOrderCommandHandlerTest()
        {
            stockMarketFactoryMock.GetStockMarket(orderQueryRepositoryMock, tradeQueryRepositoryMock).Returns(stockMarket);
        }

        [Fact]
        public async Task Handle_Should_Call_ModifieOrderAsync()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();


            //Act
            await sut.Handle(addOrderCommand);

            //Assert
            await stockMarket.Received(1).ModifieOrder(
                                    SOME_ORDER_ID,
                                    SOME_PRICE,
                                    SOME_AMOUNT,
                                    SOME_EXPIRATION_DATE);
        }

        [Fact]
        public async Task Handle_Should_Call_OrderCommandRepository_Add()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();
            processContext.Order.Returns(
                new TestOrder
                {
                    Id = 10,
                    Amount = SOME_AMOUNT,
                    ExpireTime = SOME_EXPIRATION_DATE,
                    IsFillAndKill = SOME_IS_FILL_AND_KILL,
                    OrderParentId = SOME_ORDER_PARENT_ID,
                    OrderState = OrderStates.Register,
                    Price = SOME_PRICE,
                    Side = SOME_SIDE
                });

            stockMarket.ModifieOrder(
               addOrderCommand.OrderId,
               addOrderCommand.Price,
               addOrderCommand.Amount,
               addOrderCommand.ExpDate).Returns(processContext);

            //Act
            await sut.Handle(addOrderCommand);

            //Assert
            await orderCommandRepositoryMock.Received(1).Add(processContext.Order);
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
                OrderState = OrderStates.Register,
                Price = SOME_PRICE,
                Side = SOME_SIDE
            };

            processContext.ModifiedOrders.Returns(new List<TestOrder> { order });

            stockMarket.ModifieOrder(
               addOrderCommand.OrderId,
               addOrderCommand.Price,
               addOrderCommand.Amount,
               addOrderCommand.ExpDate).Returns(processContext);

            orderCommandRepositoryMock.Find(Arg.Any<long>()).Returns(order);

            //Act
            await sut.Handle(addOrderCommand);


            //Assert
            await orderCommandRepositoryMock.Received(1).Find(processContext.ModifiedOrders.ToList().FirstOrDefault().Id);
        }

        [Fact]
        public async Task Handle_Should_Call_TradeCommandRepository_Add()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            processContext.CreatedTrades.Returns(new List<ITrade>
            {
                new TestTrade() {
                Amount = SOME_AMOUNT,
                BuyOrderId =SOME_BUY_ORDER_ID,
                Id=SOME_ORDER_ID,
                Price = SOME_PRICE,
                SellOrderId=SOME_SELL_ORDER_ID
                }
            });

            stockMarket.ModifieOrder(
               addOrderCommand.OrderId,
               addOrderCommand.Price,
               addOrderCommand.Amount,
               addOrderCommand.ExpDate).Returns(processContext);

            //Act
            await sut.Handle(addOrderCommand);
            var trade = processContext.CreatedTrades.ToList().FirstOrDefault();

            //Assert
            await tradeCommandRepositoryMock.Received(1).Add(trade);
        }


        protected override ModifieOrderCommand MakeSomeTCommand()
        {
            return new ModifieOrderCommand()
            {
                Amount = SOME_AMOUNT,
                ExpDate = SOME_EXPIRATION_DATE,
                OrderId = SOME_ORDER_ID,
                Price = SOME_PRICE
            };
        }
    }
}
