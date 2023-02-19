using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using TradeMatchingEngine.Orders.Commands;
using Xunit;

namespace Application.Tests
{
    public class ModifieOrderCommandHandlerTest : CommandHandelerTest<ModifieOrderCommandHandler, ModifieOrderCommand>
    {
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
               Arg.Is(addOrderCommand.OrderId),
               Arg.Is(addOrderCommand.Price),
               Arg.Is(addOrderCommand.Amount),
               Arg.Is(addOrderCommand.ExpDate)
               );
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


        protected override ModifieOrderCommand MakeSomeTCommand()
        {
            return new ModifieOrderCommand()
            {
                Amount = 10,
                ExpDate = new DateTime(2050, 1, 1),
                OrderId = 1,
                Price = 10
            };
        }
    }
}
