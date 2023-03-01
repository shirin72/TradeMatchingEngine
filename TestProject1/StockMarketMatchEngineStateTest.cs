using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Xunit;

namespace Test
{
    public class StockMarketMatchEngineStateTest
    {
        private StockMarketMatchEngineStateProxy sut;

        public StockMarketMatchEngineStateTest()
        {
            sut = new StockMarketMatchEngineStateProxy();
        }

        [Fact]
        public void StockMarketMatchEngine_Try_To_Open_Market_In_ClosedState_Must_Throw_NotImplemented_Exception()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<NotImplementedException>(() => sut.Open());
        }
        [Fact]
        public void StockMarketMatchEngine_Try_To_Close_Market_In_OpenState_Must_Throw_NotImplemented_Exception()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            //Act

            //Assert
            Assert.Throws<NotImplementedException>(() => sut.Close());
        }
        [Fact]
        public void OpenMethod_Should_Change_State_From_PreOpen_To_OpenState()
        {
            //Arrange
            sut.PreOpen();


            //Act
            sut.Open();

            //Assert
            Assert.Equal(MarcketState.Open, sut.State);
        }

        [Fact]
        public void CloseMethod_Try_To_Change_MarketState_ToCloseState()
        {
            //Arrange
            sut.PreOpen();

            //Act
            sut.Close();

            //Assert
            Assert.Equal(MarcketState.Close, sut.State);
        }


        [Fact]
        public void PreOpen_Try_ToChange_MarketStateOpen_ToPreOpenState()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();

            //Act
            sut.PreOpen();

            //Assert
            Assert.Equal(MarcketState.PreOpen, sut.State);
        }

        [Fact]
        public async Task ProcessOrderAsync_Several_SellOrder_Should_Enqueue_When_State_Is_PreOpen()
        {
            //Arrange
            sut.PreOpen();
            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(120, 10, Side.Sell);

            //Act
            await sut.ProcessOrderAsync(110, 10, Side.Sell);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Empty(sut.Trade);
            Assert.Equal(5, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(5, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(0, sut.AllTradeCount());
            var sumSellOrderWithPrice100 = sut.AllOrders.Where(x => x.Side == Side.Sell && x.Price == 100).Sum(x => x.Amount);
            Assert.Equal(30, sumSellOrderWithPrice100);

        }
        [Fact]
        public async void ProcessOrderAsync_BuyOrder_Should_Enqueue_When_State_Is_PreOpen()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            sut.PreOpen();

            //Act
            var buyOrder = await sut.ProcessOrderAsync(10, 5, Side.Buy);

            //Assert
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(1, sut.GetPreOrderQueue().Count);

            sut.AllOrders.Where(o => o.Id == buyOrder.Order.Id)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(
                new
                {
                    Id = buyOrder.Order.Id,
                    Price = 10,
                    Amount = 5,
                    Side = Side.Buy,

                });
        }
        [Fact]
        public async void ProcessOrderAsync_SellOrder_Should_Enqueue_When_State_Is_PreOpen()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            sut.PreOpen();

            //Act
            var sellOrder = await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //Assert
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Empty(sut.GetPreOrderQueue());

            sut.AllOrders.Where(o => o.Id == sellOrder.Order.Id)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(
                new
                {
                    Id = sellOrder.Order.Id,
                    Price = 10,
                    Amount = 5,
                    Side = Side.Sell,

                });
        }

        [Fact]
        public async Task ProcessOrderAsync_Call_ModifieOrder_When_State_Is_PreOpen()
        {
            //Arrange
            sut.PreOpen();
            var sellOrderId = await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Act
           var modifieOrderId=  await sut.ModifieOrder(sellOrderId.Order.Id,110, 10,DateTime.Now.AddDays(1));

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Empty(sut.Trade);
            Assert.Equal(2, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(2, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(0, sut.AllTradeCount());

             sut.AllOrders.Where(x => x.Id== modifieOrderId.Order.Id)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(
                new
                {
                    Id = modifieOrderId.Order.Id,
                    Price = 110,
                    Amount = 10,
                    Side = Side.Sell,

                });
        }
    }
}
