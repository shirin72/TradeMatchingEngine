using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using TradeMatchingEngine;
using Xunit;

namespace Test
{
    public class StockMarketMatchEngineStateTest
    {
        private StockMarketMatchEngine sut;

        public StockMarketMatchEngineStateTest()
        {
            sut = new StockMarketMatchEngine();
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
        public async Task ProcessOrderAsync_Several_SellOrder_Should_Enqueue_In_PreOrderQueue_With_No_Trade()
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
        public async void ProcessOrderAsync_BuyOrder_Should_Enque_In_PreQueue()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            sut.PreOpen();

            //Act
            var buyOrder=await sut.ProcessOrderAsync(10, 5, Side.Buy);

            //Assert
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(1, sut.GetPreOrderQueue().Count);

            sut.AllOrders.Where(o => o.Id == buyOrder )
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(
                new
                {
                    Id=buyOrder,
                    Price = 10,
                    Amount = 5,
                    Side= Side.Buy,
                  
                });
        }
        [Fact]
        public async void ProcessOrderAsync_SellOrder_Should_Enque_In_SellOrderQueue()
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

            sut.AllOrders.Where(o => o.Id == sellOrder)
                .FirstOrDefault()
                .Should()
                .BeEquivalentTo(
                new
                {
                    Id = sellOrder,
                    Price = 10,
                    Amount = 5,
                    Side = Side.Sell,

                });
        }
    }
}
