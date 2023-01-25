using Application.OrderService.OrderCommandHandlers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Query;
using Xunit;

namespace Test
{
    public class AddOrderCommandHandlerTest
    {
        private readonly IOrderCommand orderCommand;
        private readonly IOrderQuery orderQuery;
        private readonly ITradeQuery tradeQuery;

        public AddOrderCommandHandlerTest(IOrderCommand orderCommand,IOrderQuery orderQuery,ITradeQuery tradeQuery)
        {
            this.orderCommand = orderCommand;
            this.orderQuery = orderQuery;
            this.tradeQuery = tradeQuery;
        }


        [Fact]
        public async Task Handle_ConcurrentThreadInPublishEvent()
        {
            //Arrage
            var mockOrderCommand = new Mock<IOrderCommand>();

            var buyOrder = new Order(1, Side.Buy, 100, 1, DateTime.Now.AddDays(1));

            mockOrderCommand.Setup(x => x.CreateOrder(buyOrder)).ReturnsAsync(() => 1);

            var mockOrderQuery = new Mock<IOrderQuery>();
            mockOrderQuery.Setup(x => x.GetLastOrder()).ReturnsAsync(() => 0);
            var orderList = new List<Order>();
            mockOrderQuery.Setup(x => x.GetAllOrders()).ReturnsAsync(() => orderList);

            var mockTradeQuery = new Mock<ITradeQuery>();
            mockTradeQuery.Setup(x => x.GetLastTrade()).ReturnsAsync(() => 0);

            var sut1 = new AddOrderCommandHandlers(mockOrderCommand.Object, mockOrderQuery.Object, mockTradeQuery.Object);
            var task1 = Task.Run(() => sut1.Handle(100, 1, Side.Buy, DateTime.Now.AddDays(1), false));



            var mockOrder1Command = new Mock<IOrderCommand>();

            var sellOrder = new Order(2, Side.Sell, 101, 2, DateTime.Now.AddDays(1));

            mockOrder1Command.Setup(x => x.CreateOrder(sellOrder)).ReturnsAsync(() => 1);

            var mockOrder1Query = new Mock<IOrderQuery>();
            mockOrderQuery.Setup(x => x.GetLastOrder()).ReturnsAsync(() => 1);
            var orderList1 = new List<Order>();
            orderList1.Add(buyOrder);
            mockOrderQuery.Setup(x => x.GetAllOrders()).ReturnsAsync(() => orderList1);

            var mockTrade1Query = new Mock<ITradeQuery>();
            mockTrade1Query.Setup(x => x.GetLastTrade()).ReturnsAsync(() => 0);

            var sut2 = new AddOrderCommandHandlers(mockOrderCommand.Object, mockOrderQuery.Object, mockTradeQuery.Object);
            var task2 = Task.Run(() => sut2.Handle(101, 2, Side.Sell, DateTime.Now.AddDays(1), false));



            var mockOrder2Command = new Mock<IOrderCommand>();

            var buy1Order = new Order(3, Side.Buy, 99, 1, DateTime.Now.AddDays(1));

            mockOrderCommand.Setup(x => x.CreateOrder(buy1Order)).ReturnsAsync(() => 1);

            var sut3 = new AddOrderCommandHandlers(mockOrderCommand.Object, mockOrderQuery.Object, mockTradeQuery.Object);
            var task3 = Task.Run(() => sut1.Handle(99, 1, Side.Buy, DateTime.Now.AddDays(1), false));



            //Action
            await Task.WhenAll(task1, task2, task3);

            //Assert


        }


        [Fact]
        public async Task Handle_ConcurrentThreadInPublishEvent1()
        {
            //Arrage

            var buyOrder = new Order(1, Side.Buy, 100, 1, DateTime.Now.AddDays(1));

          

            var sut1 = new AddOrderCommandHandlers(orderCommand,orderQuery,tradeQuery);
            var task1 = Task.Run(() => sut1.Handle(100, 1, Side.Buy, DateTime.Now.AddDays(1), false));




            var sellOrder = new Order(2, Side.Sell, 101, 2, DateTime.Now.AddDays(1));
    

            var sut2 = new AddOrderCommandHandlers(orderCommand,orderQuery, tradeQuery);
            var task2 = Task.Run(() => sut2.Handle(101, 2, Side.Sell, DateTime.Now.AddDays(1), false));



            

            var buy1Order = new Order(3, Side.Buy, 99, 1, DateTime.Now.AddDays(1));

            

            var sut3 = new AddOrderCommandHandlers(orderCommand, orderQuery, tradeQuery);
            var task3 = Task.Run(() => sut1.Handle(99, 1, Side.Buy, DateTime.Now.AddDays(1), false));



            //Action
            await Task.WhenAll(task1, task2, task3);

            //Assert

            Console.WriteLine("This should hold");
        }

    }
}
