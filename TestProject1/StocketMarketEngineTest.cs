using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Command;
using Xunit;


namespace Test
{
    public class StocketMarketEngineTest : IAsyncDisposable
    {
        private StockMarketMatchEngine sut;

        public StocketMarketEngineTest()
        {
            sut = new StockMarketMatchEngine();
        }

       

        [Fact]
        public async Task ProcessOrderAsync_Should_Enqueue_One_SellOrder_And_NoTrades_Should_be_Created_When_Is_In_Preopen_State()
        {
            //Arrange
            sut.PreOpen();

            //Act
            var sellOrderId = await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.AllTradeCount());

            sut.AllOrders
                .Where(o => o.Id == sellOrderId).
                FirstOrDefault().
                Should().
                BeEquivalentTo(new
                {
                    Id = 1,
                    Price = 100,
                    Amount = 10,
                    Side = Side.Sell
                });
        }

        [Fact]
        public async Task ProcessOrderAsync_Should_Enqueue_One_BuyOrder_NoTrade_Should_Created_When_Is_In_Preopen_State()
        {
            //Arrange
            sut.PreOpen();

            //Act
            var buyOrderId = await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(1, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.AllTradeCount());

            sut.AllOrders
                .Where(o => o.Id == buyOrderId)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(new
                {
                    Id = 1,
                    Price = 100,
                    Amount = 10,
                    Side = Side.Buy
                });
        }


        [Fact]
        public async Task ProcessOrderAsync_Should_One_Trade_Create_With_Amount10_And_Price100_When_Is_In_Open_State()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            var sellOrderId = await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Act
            var buyOrderId = await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.Open, sut.State);
            Assert.Equal(1, sut.AllTradeCount());

            sut.Trade.
                Where(o => o.Id == 1)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(
                new
                {
                    Id = 1,
                    BuyOrderId = buyOrderId
                    ,
                    SellOrderId = sellOrderId,
                    Amount = 10,
                    Price = 100
                });
        }


        [Fact]
        public async void ProcessOrderAsync_Should_Not_Create_Trade_By_ExpiredBuyOrder_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Buy, DateTime.Now.AddDays(-1));

            //Act
            var sellOrderId = await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.AllTradeCount());

            sut.AllOrders
                .Where(o => o.Id == sellOrderId)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(new
                {
                    Price = 100,
                    Amount = 10,
                    Side = Side.Sell
                });
        }

        [Fact]
        public async void ProcessOrderAsync_Should_Not_Create_Trade_By_ExpiredSellOrder_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell, DateTime.Now.AddDays(-1));

            //Act
            var buyOrderId = await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.AllTradeCount());

            sut.AllOrders
                .Where(o => o.Id == buyOrderId)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(new
                {
                    Price = 100,
                    Amount = 10,
                    Side = Side.Buy
                });
        }

        [Fact]
        public async void ProcessOrderAsync_Should_One_Trade_Created_And_One_SellOrder_Enqueu_With_Amount_1_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 1, Side.Buy);

            //Act
            var sellOrderId = await sut.ProcessOrderAsync(10, 2, Side.Sell);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(1, sut.AllTradeCount());
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(1, sut.Trade.First().Amount);

            sut.AllOrders
                 .Where(o => o.Id == sellOrderId)
                 .FirstOrDefault()
                 .Should()
                 .BeEquivalentTo(new
                 {
                     Price = 10,
                     Amount = 1,
                     Side = Side.Sell
                 });
        }

        [Fact]
        public async void ProcessOrderAsync_Should_One_Trade_Created_And_One_BuyOrder_Enqueu_With_Amount_1_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 1, Side.Sell);

            //Act
            var buyOrderId = await sut.ProcessOrderAsync(10, 2, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(1, sut.AllTradeCount());
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(1, sut.Trade.First().Amount);

            sut.AllOrders
               .Where(o => o.Id == buyOrderId)
               .FirstOrDefault()
               .Should()
               .BeEquivalentTo(new
               {
                   Price = 10,
                   Amount = 1,
                   Side = Side.Buy
               });
        }

        [Fact]
        public async void ProcessOrderAsync_Should_One_Trade_Created_With_2_Amount_And_One_SellOrder_Enqueu_With_Amount3_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrderId = await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //Act
            var buyOrderId = await sut.ProcessOrderAsync(10, 2, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(1, sut.AllTradeCount());
            Assert.Equal(3, sut.AllOrders.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(2, sut.Trade.First().Amount);

            var buyOrder = sut.AllOrders
                .Where(o => o.Id == sellOrderId)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(new
                {
                    Price = 10,
                    Amount = 3,
                    Side = Side.Sell
                });

            sut.Trade
                .Where(o => o.BuyOrderId == buyOrderId && o.SellOrderId == sellOrderId)
                .FirstOrDefault().Should().BeEquivalentTo(
                new
                {
                    Price = 10,
                    Amount = 2,
                    SellOrderId = sellOrderId,
                    BuyOrderId = buyOrderId
                });
        }

        [Fact]
        public async void ProcessOrderAsync_Should_Create_Two_Trades_And_Left_No_Order_In_Queues_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrder = await sut.ProcessOrderAsync(10, 100, Side.Sell);
            var buyOrder1 = await sut.ProcessOrderAsync(10, 50, Side.Buy);

            //Act
            var buyOrder2 = await sut.ProcessOrderAsync(10, 50, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());


            sut.Trade.Where(o => o.BuyOrderId == buyOrder1 && o.SellOrderId == sellOrder)
                .FirstOrDefault().Should().BeEquivalentTo(
                new
                {
                    Price = 10,
                    Amount = 50,
                    SellOrderId = sellOrder,
                    BuyOrderId = buyOrder1
                });

            sut.Trade.Where(o => o.BuyOrderId == buyOrder2 && o.SellOrderId == sellOrder)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 10,
                Amount = 50,
                SellOrderId = sellOrder,
                BuyOrderId = buyOrder2
            });
        }

        [Fact]
        public async void ProcessOrderAsync_Two_Trade_Should_Create_And_One_SellOrder_Should_Be_Enqueued_With_New_Amount_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrder1 = await sut.ProcessOrderAsync(10, 5, Side.Sell);
            var sellOrder2 = await sut.ProcessOrderAsync(10, 2, Side.Sell);

            //Act
            var buyOrder = await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder1)
           .FirstOrDefault().Should().BeEquivalentTo(
           new
           {
               Price = 10,
               Amount = 5,
               SellOrderId = sellOrder1,
               BuyOrderId = buyOrder
           });

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder2)
         .FirstOrDefault().Should().BeEquivalentTo(
         new
         {
             Price = 10,
             Amount = 1,
             SellOrderId = sellOrder2,
             BuyOrderId = buyOrder
         });
        }

        [Fact]
        public async void ProcessOrderAsync_One_Trade_Should_Create_And_One_BuyOrder_Should_Be_Enqueued_With_New_Amount_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var buyOrder1 = await sut.ProcessOrderAsync(10, 5, Side.Buy);
            var buyOrder2 = await sut.ProcessOrderAsync(10, 2, Side.Buy);

            //Act
            var sellOrder = await sut.ProcessOrderAsync(10, 6, Side.Sell);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(5, sut.Trade.First().Amount);
            Assert.Equal(1, sut.Trade.Last().Amount);
            Assert.Equal(6, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(1, sut.AllOrders.Where(x => x.Side == Side.Buy).Select(x => x.Amount).First());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder1 && o.SellOrderId == sellOrder)
         .FirstOrDefault().Should().BeEquivalentTo(
         new
         {
             Price = 10,
             Amount = 5,
             SellOrderId = sellOrder,
             BuyOrderId = buyOrder1
         });

            sut.Trade.Where(o => o.BuyOrderId == buyOrder2 && o.SellOrderId == sellOrder)
         .FirstOrDefault().Should().BeEquivalentTo(
         new
         {
             Price = 10,
             Amount = 1,
             SellOrderId = sellOrder,
             BuyOrderId = buyOrder2
         });
        }

        [Fact]
        public async void ProcessOrderAsync_TwoTrade_Should_Create_And_Three_BuyOrder_Should_Be_Enqueued_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var buyOrder1 = await sut.ProcessOrderAsync(10, 5, Side.Buy);
            var buyOrder2 = await sut.ProcessOrderAsync(10, 2, Side.Buy);
            var buyOrder3 = await sut.ProcessOrderAsync(10, 1, Side.Buy);
            var buyOrder4 = await sut.ProcessOrderAsync(10, 5, Side.Buy);

            //Act
            var sellOrder = await sut.ProcessOrderAsync(10, 6, Side.Sell);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(3, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder1 && o.SellOrderId == sellOrder)
          .FirstOrDefault().Should().BeEquivalentTo(
          new
          {
              Price = 10,
              Amount = 5,
              SellOrderId = sellOrder,
              BuyOrderId = buyOrder1
          });
            sut.Trade.Where(o => o.BuyOrderId == buyOrder2 && o.SellOrderId == sellOrder)
         .FirstOrDefault().Should().BeEquivalentTo(
         new
         {
             Price = 10,
             Amount = 1,
             SellOrderId = sellOrder,
             BuyOrderId = buyOrder2
         });
        }

        [Fact]
        public async void ProcessOrderAsync_Two_Trades_Should_Create_And_Two_SellOrder_Should_Be_Enqueued_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrder1 = await sut.ProcessOrderAsync(10, 1, Side.Sell);
            var sellOrder2 = await sut.ProcessOrderAsync(10, 5, Side.Sell);
            var sellOrder3 = await sut.ProcessOrderAsync(10, 5, Side.Sell);
            var sellOrder4 = await sut.ProcessOrderAsync(10, 2, Side.Sell);

            //Act
            var buyOrder = await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(2, sut.GetSellOrderCount());
            Assert.Equal(6, sut.Trade.Sum(t => t.Amount));

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder1)
         .FirstOrDefault().Should().BeEquivalentTo(
         new
         {
             Price = 10,
             Amount = 1,
             SellOrderId = sellOrder1,
             BuyOrderId = buyOrder
         });
            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder2)
         .FirstOrDefault().Should().BeEquivalentTo(
         new
         {
             Price = 10,
             Amount = 5,
             SellOrderId = sellOrder2,
             BuyOrderId = buyOrder
         });
        }





        [Fact]
        public async void ProcessOrderAsync_Four_SellOrder_Exsist_And_One_BuyOrder_Enters_Two_Trades_Should_Create_And_Three_SellOrder_Should_Be_Enqueued_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrder1 = await sut.ProcessOrderAsync(10, 5, Side.Sell);
            var sellOrder2 = await sut.ProcessOrderAsync(10, 2, Side.Sell);
            var sellOrder3 = await sut.ProcessOrderAsync(10, 1, Side.Sell);
            var sellOrder4 = await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //Act
            var buyOrder = await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(5, sut.Trade.First().Amount);
            Assert.Equal(1, sut.Trade.Last().Amount);
            Assert.Equal(6, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(3, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder1)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 10,
                Amount = 5,
                SellOrderId = sellOrder1,
                BuyOrderId = buyOrder
            });

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder2)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 10,
                Amount = 1,
                SellOrderId = sellOrder2,
                BuyOrderId = buyOrder
            });
        }

        //[Fact]
        //public async void ProcessOrderAsync_SixEventShouldBeRaised()
        //{
        //    //arrenge
        //    sut.PreOpen();
        //    sut.Open();

        //    await sut.ProcessOrderAsync(10, 5, Side.Sell);
        //    await sut.ProcessOrderAsync(10, 1, Side.Sell);

        //    //Act
        //    await sut.ProcessOrderAsync(10, 6, Side.Buy);

        //    //assert
        //    Assert.Equal(2, sut.TradeCount);
        //    Assert.Equal(5, sut.Trade.First().Amount);
        //    Assert.Equal(1, sut.Trade.Last().Amount);
        //    Assert.Equal(6, sut.Trade.Sum(x => x.Amount));
        //    Assert.Equal(10, sut.Trade.First().Price);
        //    Assert.Equal(0, sut.GetBuyOrderCount());
        //    Assert.Equal(0, sut.GetSellOrderCount());

        //}

        [Fact]
        public async void ProcessOrderAsync_ShouldOneTradeCreatedAndFiveEventBeRaised()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var events = new StockMarketEvents()
            {
                OnOrderCreated = onOrderCreated,
                OnTradeCreated = onTradeCreated
            };

            await sut.ProcessOrderAsync(100, 10, Side.Sell, events: events);
            await sut.ProcessOrderAsync(100, 5, Side.Sell);

            //Act
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.First().Amount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.First().Price);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        private async Task onOrderCreated(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        {

            var _orderCommandRepository = new Mock<IOrderCommandRepository>();

            _orderCommandRepository.Setup(x => x.Add(It.IsAny<Order>()));

            await _orderCommandRepository.Object.Add(order);
        }

        private async Task onTradeCreated(StockMarketMatchEngine stockMarketMatchEngine, Trade trade)
        {

            var _tradeCommandRepository = new Mock<ITradeCommandRepository>();

            _tradeCommandRepository.Setup(x => x.Add(It.IsAny<Trade>()));

            await _tradeCommandRepository.Object.Add(trade);
        }

        private async Task onOrderModified(StockMarketMatchEngine stockMarketMatchEngine, Order order)
        {
            try
            {
                var _orderCommandRepository = new Mock<IOrderCommandRepository>();

                _orderCommandRepository.Setup(x => x.Find(order.Id)).ReturnsAsync(() => order);

                var founndOrder = await _orderCommandRepository.Object.Find(order.Id);

                founndOrder.UpdateBy(order);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [Fact]
        public async void ProcessOrderAsync_One_Trade_Should_Be_Created_With_Seller_Price_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var buyOrder = await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //Act
            var sellOrder = await sut.ProcessOrderAsync(90, 10, Side.Sell);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.First().Amount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(90, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 90,
                Amount = 10,
                SellOrderId = sellOrder,
                BuyOrderId = buyOrder
            });
        }

        [Fact]
        public async void ProcessOrderAsync_Trade_Should_Not_Be_Created_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Act
            await sut.ProcessOrderAsync(90, 10, Side.Buy);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(0, sut.Trade.Sum(x => x.Price));
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_One_Trade_Should_Be_Created_With_SellPrice_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrder = await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Act
            var buyOrder = await sut.ProcessOrderAsync(110, 10, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder)
           .FirstOrDefault().Should().BeEquivalentTo(
           new
           {
               Price = 100,
               Amount = 10,
               SellOrderId = sellOrder,
               BuyOrderId = buyOrder
           });
        }

        [Fact]
        public async void ProcessOrderAsync_Two_Trades_Should_Be_Created_And_Rmain_Amount_Should_Be_Fifteen_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var sellOrder1 = await sut.ProcessOrderAsync(100, 10, Side.Sell);

            var sellOrder2 = await sut.ProcessOrderAsync(100, 15, Side.Sell);

            var sellOrder3 = await sut.ProcessOrderAsync(100, 5, Side.Sell);


            //Act
            var buyOrder1 = await sut.ProcessOrderAsync(110, 10, Side.Buy);
            var buyOrder2 = await sut.ProcessOrderAsync(110, 5, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(15, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(200, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(2, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder1 && o.SellOrderId == sellOrder1)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 100,
                Amount = 10,
                SellOrderId = sellOrder1,
                BuyOrderId = buyOrder1
            });

            sut.Trade.Where(o => o.BuyOrderId == buyOrder2 && o.SellOrderId == sellOrder2)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 100,
                Amount = 5,
                SellOrderId = sellOrder2,
                BuyOrderId = buyOrder2
            });
        }

        [Fact]
        public async void ProcessOrderAsync_Two_Trades_Should_Be_Created_And_Toal_Amount_Of_Trade_Should_Be_Twenty_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var buyOrder1 = await sut.ProcessOrderAsync(100, 10, Side.Buy);
            var buyOrder2 = await sut.ProcessOrderAsync(100, 15, Side.Buy);
            var buyOrder3 = await sut.ProcessOrderAsync(100, 5, Side.Buy);
            var buyOrder4 = await sut.ProcessOrderAsync(100, 20, Side.Buy);

            //Act
            var sellOrder1 = await sut.ProcessOrderAsync(90, 10, Side.Sell);
            var sellOrder2 = await sut.ProcessOrderAsync(90, 10, Side.Sell);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(20, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(180, sut.Trade.Sum(x => x.Price));
            Assert.Equal(30, sut.AllOrders.Sum(x => x.Amount));
            Assert.Equal(3, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder1 && o.SellOrderId == sellOrder1)
           .FirstOrDefault().Should().BeEquivalentTo(
           new
           {
               Price = 90,
               Amount = 10,
               SellOrderId = sellOrder1,
               BuyOrderId = buyOrder1
           });


            sut.Trade.Where(o => o.BuyOrderId == buyOrder2 && o.SellOrderId == sellOrder2)
           .FirstOrDefault().Should().BeEquivalentTo(
           new
           {
               Price = 90,
               Amount = 10,
               SellOrderId = sellOrder2,
               BuyOrderId = buyOrder2
           });
        }

        [Fact]
        public async void ProcessOrderAsync_Should_One_Trade_Be_Created_And_Remain_Order_Should_Be_Removed_FillAndKill_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var buyOrder = await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //Act
            var sellOrder = await sut.ProcessOrderAsync(90, 15, Side.Sell, fillAndKill: true);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(90, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrder && o.SellOrderId == sellOrder)
            .FirstOrDefault().Should().BeEquivalentTo(
            new
            {
                Price = 90,
                Amount = 10,
                SellOrderId = sellOrder,
                BuyOrderId = buyOrder
            });
        }

        [Fact]
        public async void ProcessOrderAsync_No_Trade_Should_Be_Created_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(90, 15, Side.Sell, fillAndKill: true);

            //Act
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(0, sut.Trade.Sum(x => x.Price));
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        //[Fact]
        //public async void ProcessOrderAsync_ShouldNotTrade2()
        //{
        //    var bCollection = new BlockingCollection<int>();
        //    var tasks = new List<Task>();
        //    for (int i = 0; i < 100; i++)
        //    {
        //        tasks.Add(Task.Run(async () =>
        //        {

        //            bCollection.Add(i);

        //        }));
        //    }
        //    var completeTask = Task.WhenAll(tasks).ContinueWith(t => bCollection.CompleteAdding());

        //    int k = 0;
        //    Task consumer = Task.Run(() =>
        //    {

        //        while (!bCollection.IsCompleted)
        //        {
        //            if (bCollection.TryTake(out int i))
        //            {

        //                k++;
        //            }

        //        }
        //    });

        //    var allTasks = new List<Task>(tasks);
        //    allTasks.Add(consumer);
        //    allTasks.Add(completeTask);
        //    await Task.WhenAll(allTasks);
        //    Assert.Equal(100, k);

        //    var queue = new ConcurrentQueue<int>();
        //    var i = 0;
        //    var t1 = Task.Run(() => queue.Enqueue(Interlocked.Increment(3)));
        //    var t2 = Task.Run(() => queue.Enqueue(Interlocked.Increment(ref i)));
        //    var t3 = Task.Run(() => queue.Enqueue(Interlocked.Increment(ref i)));
        //    var t4 = Task.Run(async () =>
        //    {
        //        var j = 0;
        //        while (j < 3)
        //        {
        //            int x = 0;
        //            queue.TryDequeue(out x);
        //            if (x == 0)
        //            {
        //                await Task.Delay(100);
        //                continue;
        //            }
        //            j++;
        //        }
        //    });

        //    await Task.WhenAll(t1, t2, t3, t4);
        //}

        //[Fact]
        //public async void ProcessOrderAsync_ShouldNotTrade4()
        //{
        //    var bCollection = new BlockingCollection<int>();
        //    var tasks = new List<Task>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        tasks.Add(Task.Run(async () =>
        //        {

        //            bCollection.Add(i);

        //        }));
        //    }
        //    Task.WhenAll(tasks);

        //    int k = 0;
        //    Task consumer = Task.Run(() =>
        //    {

        //        while (!bCollection.IsCompleted)
        //        {
        //            if (bCollection.TryTake(out int i)) { k++; }
        //            bCollection.Add(i + 2);
        //        }
        //    });

        //    var allTasks = new List<Task>(tasks);
        //    allTasks.Add(consumer);
        //    //allTasks.Add(completeTask);
        //    Task.WhenAll(allTasks.ToArray());
        //    System.Threading.Thread.Sleep(10000);
        //}

        [Fact]
        public async void ProcessOrderAsync_OrderEnters_Then_Is_Cancelled_Then_BuyOrder_Enters_And_No_Trade_Should_Be_Created_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var orderId = await sut.ProcessOrderAsync(90, 15, Side.Sell);
            await sut.CancelOrderAsync(orderId);

            //Act
            await sut.ProcessOrderAsync(90, 15, Side.Buy);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(0, sut.Trade.Sum(x => x.Price));
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_Cancell_Not_Exist_Order_Should_Throw_Execption_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            //assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await sut.CancelOrderAsync(0));
        }

        [Fact]
        public async void ProcessOrderAsync_Order_Enters_Then_Modified_Trade_Should_Be_Created_With_New_Price_And_Amount_When_Is_In_Open_State()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var events = new StockMarketEvents()
            {
                OnOrderCreated = onOrderCreated,
                OnOrderModified = onOrderModified,
                OnTradeCreated = onTradeCreated
            };

            var orderId = await sut.ProcessOrderAsync(90, 15, Side.Sell);
            var sellOrderId = await sut.ProcessOrderAsync(110, 15, Side.Sell);
            var modifiedOrderId = await sut.ModifieOrder(orderId, 100, 5, DateTime.MaxValue, events);

            //Act
            var buyOrderId = await sut.ProcessOrderAsync(100, 15, Side.Buy, events: events);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(5, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.Sum(x => x.Price));
            Assert.Equal(2, sut.AllOrdersCount());
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());

            sut.Trade.Where(o => o.BuyOrderId == buyOrderId && o.SellOrderId == modifiedOrderId)
        .FirstOrDefault().Should().BeEquivalentTo(
        new
        {
            Price = 100,
            Amount = 5,
            SellOrderId = modifiedOrderId,
            BuyOrderId = buyOrderId
        });
        }

        [Fact]
        public async void ProcessOrderAsync_Cancell_Order()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var events = new StockMarketEvents()
            {
                OnOrderCreated = onOrderCreated,
                OnOrderModified = onOrderModified,
                OnTradeCreated = onTradeCreated
            };

            var orderId = await sut.ProcessOrderAsync(90, 15, Side.Sell);

            //Act
            var modifiedOrderId = await sut.CancelOrderAsync(orderId, events);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_Initialize_Orders_And_Enqueue_SellOrder_When_State_Is_Open()
        {
            //arrenge
            var orders = new List<Order>()
            {
                new Order(1,Side.Sell,price: 100,amount:2,expireTime: DateTime.Now.AddDays(1))
            };
            var sut1 = new StockMarketMatchEngine(orders);
            sut1.PreOpen();
            sut1.Open();

            //Act
          await sut1.ProcessOrderAsync(90, 15, Side.Sell);

            //assert
            Assert.Equal(0, sut1.TradeCount);
            Assert.Equal(2, sut1.AllOrdersCount());
            Assert.Equal(0, sut1.GetBuyOrderCount());
            Assert.Equal(2, sut1.GetSellOrderCount());
        }


        public async ValueTask DisposeAsync()
        {
            await sut.DisposeAsync();
        }
    }
}