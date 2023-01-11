using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{
    public class StocketMarketEngineTest
    {
        private StockMarketMatchEngine sut;
        private List<StockMarketMatchEngineEvents> receivedEvents = new();

        public StocketMarketEngineTest()
        {
            sut = new StockMarketMatchEngine();
            sut.ProcessCompleted += delegate (object sender, EventArgs e)
            {
                var stockMarketMatchEngineEvents = e as StockMarketMatchEngineEvents;
                receivedEvents.Add(stockMarketMatchEngineEvents);
            };
        }

        [Fact]
        public async Task StockMarketMatchEngine_FirstPreOrderSellEnters_MustEnqueue1PreOrderSell()
        {
            //Arrange
            sut.PreOpen();

            //Action
            sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0,sut.AllTradeCount ());
        }

        [Fact]
        public async Task StockMarketMatchEngine_FirstPreOrderBuyEnters_MustEnqueue1PreOrderBuy()
        {
            //Arrange
            sut.PreOpen();

            //Action
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(1, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.AllTradeCount ());
        }

        [Fact]
        public void StockMarketMatchEngine_TryToOpenMarketInClosedState_MustThrowNotImplementedException()
        {
            //Arrange

            //Action

            //Assert
            Assert.Throws<NotImplementedException>(() => sut.Open());
        }

        [Fact]
        public async Task StockMarketMatchEngine_TryToChangeMarketStateToOpenAndCommitOneTrade_MustOneTradeGetCommit()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Action
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.Open, sut.State);
            Assert.Equal(1, sut.AllTradeCount ());
            Assert.Equal(4, receivedEvents.Count);
        }

        [Fact]
        public void StockMarketMatchEngine_TryToChangeMarketStateToClose_MustQueueGetClear()
        {
            //Arrange
            sut.PreOpen();

            //Action
            sut.Close();

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.Close, sut.State);
            Assert.Equal(0, sut.AllTradeCount ());
            Assert.Equal(2, receivedEvents.Count);
        }

        [Fact]
        public async Task StockMarketMatchEngine_TryToChangeMarketStateOpenToPreOpen_MustEnqueuPreOrder()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            sut.PreOpen();

            //Action
            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.AllTradeCount ());
            Assert.Equal(3, receivedEvents.Count);
        }

        [Fact]
        public void StockMarketMatchEngine_TryToChangeMarketOpenToClose_MustThrowNotImplementedException()
        {
            //Arrange
            sut.PreOpen();

            //Action
            sut.Open();

            //Assert
            Assert.Throws<NotImplementedException>(() => sut.Close());
        }

        [Fact]
        public async Task StockMarketMatchEngine_SeveralSellOrderEnters_MustEnqueueAllOrder()
        {
            //Arrange
            sut.PreOpen();
            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(120, 10, Side.Sell);

            //Action
            await sut.ProcessOrderAsync(110, 10, Side.Sell);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Empty(sut.Trade);
            Assert.Equal(5, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(5, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(0, sut.AllTradeCount ());
            Assert.Equal(1, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync_Should_Not_Execute_Expired_Order()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();
            await sut.ProcessOrderAsync(100, 10, Side.Buy, DateTime.Now.AddDays(-1));

            //action
            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.AllTradeCount ());
            Assert.Equal(5, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync_Should_Not_Execute_Expired_SellOrder()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();
            await sut.ProcessOrderAsync(100, 10, Side.Sell, DateTime.Now.AddDays(-1));

            //action
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.AllTradeCount ());
            Assert.Equal(5, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOnTradeCommitedAndOneSellEnqueuWithNewAmount()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();
            await sut.ProcessOrderAsync(10, 1, Side.Buy);

            //action
            await sut.ProcessOrderAsync(10, 2, Side.Sell);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(1, sut.AllTradeCount ());
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(1, sut.Trade.First().Amount);
            Assert.Equal(5, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOnTradeCommitedAndOneBuyOrderEnqueuWithNewAmount()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 1, Side.Sell);

            //action
            await sut.ProcessOrderAsync(10, 2, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(1, sut.AllTradeCount ());
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(1, sut.Trade.First().Amount);
            Assert.Equal(5, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOnTradeCommitedAndOneSellOrderEnqueuWithNewAmount()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //action
            await sut.ProcessOrderAsync(10, 2, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(1, sut.AllTradeCount ());
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(3, sut.Trade.First().Amount);
            Assert.Equal(4, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldTwoTradeCommited()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 100, Side.Sell);
            await sut.ProcessOrderAsync(10, 50, Side.Buy);

            //action
            await sut.ProcessOrderAsync(10, 50, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());

        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOneTradeCommitedAndOneSellOrderShouldBeEnteredWithNewAmount()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Sell);
            await sut.ProcessOrderAsync(10, 2, Side.Sell);

            //action
            await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOneTradeCommitedAndOneBuyOrderShouldBeEnteredWithNewAmount()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Buy);
            await sut.ProcessOrderAsync(10, 2, Side.Buy);

            //action
            await sut.ProcessOrderAsync(10, 6, Side.Sell);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldTwoTradeCommitedAndThreeBuyOrderShouldBeRemained()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Buy);
            await sut.ProcessOrderAsync(10, 2, Side.Buy);
            await sut.ProcessOrderAsync(10, 1, Side.Buy);
            await sut.ProcessOrderAsync(10, 5, Side.Buy);

            //action
            await sut.ProcessOrderAsync(10, 6, Side.Sell);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(3, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldTwoTradeCommitedAndThreeSellOrderShouldBeRemained()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Sell);
            await sut.ProcessOrderAsync(10, 2, Side.Sell);
            await sut.ProcessOrderAsync(10, 1, Side.Sell);
            await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //action
            await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(3, sut.GetSellOrderCount());
            Assert.Equal(6, sut.Trade.Sum(t => t.Amount));
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldBuyOrderEnqueInPreQueue()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            sut.PreOpen();

            //Action
            await sut.ProcessOrderAsync(10, 5, Side.Buy);

            //Assert
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
            Assert.Equal(1, sut.GetPreOrderQueue().Count);
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldSellOrderEnqueInPreQueue()
        {
            //Arrange
            sut.PreOpen();
            sut.Open();
            sut.PreOpen();

            //Action
            await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //Assert
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueue().Count);
        }

        [Fact]
        public async void ProcessOrderAsync_FourSellOrderExsistAndOneBuyOrderEnters_TwoSellOrderMustExecutedAndBuyOrderMustBeDone()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Sell);
            await sut.ProcessOrderAsync(10, 2, Side.Sell);
            await sut.ProcessOrderAsync(10, 1, Side.Sell);
            await sut.ProcessOrderAsync(10, 5, Side.Sell);

            //action
            await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(9, receivedEvents.Count);
        }

        [Fact]
        public async void ProcessOrderAsync()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(10, 5, Side.Sell);
            await sut.ProcessOrderAsync(10, 1, Side.Sell);

            //action
            await sut.ProcessOrderAsync(10, 6, Side.Buy);

            //assert
            Assert.Equal(6, receivedEvents.Count);
        }
    }
}