using Application.OrderService.OrderCommandHandlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using Xunit;

namespace Application.Tests
{
    public class AddOrderCommandHandlerTests : CommandHandelerTest<AddOrderCommandHandlers, AddOrderCommand>
    {
        private AddOrderCommand command;
        public AddOrderCommandHandlerTests()
        {
            //command=(AddOrderCommand)Substitute.For<ICommands>();
            //var processContext = Substitute.For<IStockMarketMatchingEngineProcessContext>();
           // processContext.Order.Returns(new Order(id: 1, side: Side.Buy, price: 10, amount: 10, expireTime: DateTime.Now.AddDays(1), orderState: OrderStates.Register));
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
            //var addOrderCommand = MakeSomeTCommand();
           
            //Act
            await sut.Handle(command);

            //Assert
            await stockMarket.Received(1).ProcessOrderAsync(command.Price,command.Amount,command.Side,command.ExpDate);

            //stockMarket.Received(1).ProcessOrderAsync(order.Price,order.Amount,order.Side,order.ExpireTime,order.IsFillAndKill,order.OrderParentId);
        }

        protected override AddOrderCommand MakeSomeTCommand()
        {
            return new AddOrderCommand
            {
                Amount = 10,
                //ExpDate = null,
                //IsFillAndKill = false,
                Price = 10,
                Side = Side.Buy,
                //orderParentId = 1
            } ;
        }
    }
}