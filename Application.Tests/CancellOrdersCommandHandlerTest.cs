using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Xunit;
using static Application.Tests.AddOrderCommandHandlerTests;

namespace Application.Tests
{
    public class CancellOrdersCommandHandlerTest : CommandHandelerTest<CancellOrderCommandHandler, long>
    {
        private static readonly int SOME_AMOUNT = 10;
        private static readonly DateTime SOME_EXPIRATION_DATE = new DateTime(2050, 1, 1);
        private static readonly bool SOME_IS_FILL_AND_KILL = false;
        private static readonly int SOME_PRICE = 10;
        private static readonly Side SOME_SIDE = Side.Buy;
        private static readonly int SOME_ORDER_PARENT_ID = 1;
        private static readonly int SOME_ORDER_ID = 1;

        public CancellOrdersCommandHandlerTest()
        {
            stockMarketFactoryMock.GetStockMarket(orderQueryRepositoryMock, tradeQueryRepositoryMock).Returns(stockMarket);
        }

        [Fact]
        public async Task Handle_Should_Call_CancelOrderAsync()
        {
            //Arrange
            var addOrderCommand = MakeSomeTCommand();

            //Act
            await sut.Handle(addOrderCommand);

            //Assert
            await stockMarket.Received(1).CancelOrderAsync(addOrderCommand);
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

            stockMarket.CancelOrderAsync(Arg.Is(addOrderCommand)).Returns(processContext);

            orderCommandRepositoryMock.Find(Arg.Any<long>()).Returns(order);

            //Act
            await sut.Handle(addOrderCommand);


            //Assert
            await orderCommandRepositoryMock.Received(1).Find(processContext.ModifiedOrders.ToList().FirstOrDefault().Id);
        }

        protected override long MakeSomeTCommand()
        {
            return 1;
        }
    }
}
