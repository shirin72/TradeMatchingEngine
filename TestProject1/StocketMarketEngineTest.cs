using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
            Assert.Equal(0, sut.GetPreOrderQueueCount());
            Assert.Equal(MarcketState.PreOpen, sut.State);
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(1, sut.AllTradeCount());
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
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(0, sut.AllTradeCount());
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
            Assert.Equal(1, sut.AllTradeCount());
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
            Assert.Equal(1, sut.AllTradeCount());
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
            Assert.Equal(1, sut.AllTradeCount());
            Assert.Equal(3, sut.AllOrders.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(2, sut.Trade.First().Amount);
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
            Assert.Equal(5, sut.Trade.First().Amount);
            Assert.Equal(1, sut.Trade.Last().Amount);
            Assert.Equal(6, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(1, sut.AllOrders.Where(x => x.Side == Side.Buy).Select(x => x.Amount).First());
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
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
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
            Assert.Equal(8, receivedEvents.Count);
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(5, sut.Trade.First().Amount);
            Assert.Equal(1, sut.Trade.Last().Amount);
            Assert.Equal(6, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(3, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_SixEventShouldBeRaised()
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
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(5, sut.Trade.First().Amount);
            Assert.Equal(1, sut.Trade.Last().Amount);
            Assert.Equal(6, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(10, sut.Trade.First().Price);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOneTradeCommitedAndFiveEventBeRaised()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 5, Side.Sell);

            //action
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(5, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.First().Amount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.First().Price);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_OneTradeShouldBeExecutedWithTenAmount()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 5, Side.Sell);

            //action
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(5, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.First().Amount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.First().Price);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_OneTradeShouldBeExecutedWithSalePrice()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //action
            await sut.ProcessOrderAsync(90, 10, Side.Sell);

            //assert
            Assert.Equal(4, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.First().Amount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(90, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_TradeShouldNotBeExecuted()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //action
            await sut.ProcessOrderAsync(90, 10, Side.Buy);

            //assert
            Assert.Equal(4, receivedEvents.Count);
            Assert.Contains("Has been Enqueued", receivedEvents.Last().Description);
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(0, sut.Trade.Sum(x => x.Price));
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_OneTradeShouldBeExecutedWIthSellPrice()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell);

            //action
            await sut.ProcessOrderAsync(110, 10, Side.Buy);

            //assert
            Assert.Equal(4, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_TwoTradeShouldBeExecutedAndRmainAmountShouldBeFifteen()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Sell);
            await sut.ProcessOrderAsync(100, 15, Side.Sell);
            await sut.ProcessOrderAsync(100, 5, Side.Sell);

            //action
            await sut.ProcessOrderAsync(110, 10, Side.Buy);
            await sut.ProcessOrderAsync(110, 5, Side.Buy);

            //assert
            Assert.Equal(7, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(15, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(200, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(2, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_TwoTradeShouldBeExecutedAndToalAmountOfTradeShouldBeTwenty()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Buy);
            await sut.ProcessOrderAsync(100, 15, Side.Buy);
            await sut.ProcessOrderAsync(100, 5, Side.Buy);
            await sut.ProcessOrderAsync(100, 20, Side.Buy);

            //action
            await sut.ProcessOrderAsync(90, 10, Side.Sell);
            await sut.ProcessOrderAsync(90, 10, Side.Sell);

            //assert
            Assert.Equal(8, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(20, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(180, sut.Trade.Sum(x => x.Price));
            Assert.Equal(30, sut.AllOrders.Sum(x => x.Amount));
            Assert.Equal(3, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldOneTradeBeCommitedAndRemainOrderShouldBeRemoved()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //action
            await sut.ProcessOrderAsync(90, 15, Side.Sell, fillAndKill: true);

            //assert
            Assert.Equal(4, receivedEvents.Count);
            Assert.Contains("Trade Has Been Executed", receivedEvents.Last().Description);
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(10, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(90, sut.Trade.Sum(x => x.Price));
            Assert.Equal(0, sut.AllOrdersCount());
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldNotTrade()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            await sut.ProcessOrderAsync(90, 15, Side.Sell, fillAndKill: true);

            //action
            await sut.ProcessOrderAsync(100, 10, Side.Buy);

            //assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(0, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(0, sut.Trade.Sum(x => x.Price));
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldNotTrade2()
        {
            var bCollection = new BlockingCollection<int>();
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {

                    bCollection.Add(i);

                }));
            }
            var completeTask = Task.WhenAll(tasks).ContinueWith(t => bCollection.CompleteAdding());

            int k = 0;
            Task consumer = Task.Run(() =>
            {

                while (!bCollection.IsCompleted)
                {
                    if (bCollection.TryTake(out int i))
                    {

                        k++;
                    }

                }
            });

            var allTasks = new List<Task>(tasks);
            allTasks.Add(consumer);
            allTasks.Add(completeTask);
            await Task.WhenAll(allTasks);
            Assert.Equal(100, k);

            //var queue = new ConcurrentQueue<int>();
            //var i = 0;
            //var t1=Task.Run(() => queue.Enqueue(Interlocked.Increment(3)));
            //var t2 = Task.Run(() => queue.Enqueue(Interlocked.Increment(ref i)));
            //var t3 = Task.Run(() => queue.Enqueue(Interlocked.Increment(ref i)));
            //var t4 = Task.Run(async () => {
            //    var j = 0;
            //    while(j<3)
            //    {
            //        int x=0;
            //        queue.TryDequeue(out x);
            //        if (x == 0)
            //        {
            //            await Task.Delay(100);
            //            continue;
            //        }
            //        j++;
            //    }
            //});

            //await Task.WhenAll(t1, t2, t3,t4);
        }

        [Fact]
        public async void ProcessOrderAsync_ShouldNotTrade4()
        {
            var bCollection = new BlockingCollection<int>();
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(async () =>
                {

                    bCollection.Add(i);

                }));
            }
            Task.WhenAll(tasks);

            int k = 0;
            Task consumer = Task.Run(() =>
            {

                while (!bCollection.IsCompleted)
                {
                    if (bCollection.TryTake(out int i)) { k++; }
                    bCollection.Add(i + 2);
                }
            });

            var allTasks = new List<Task>(tasks);
            allTasks.Add(consumer);
            //allTasks.Add(completeTask);
            Task.WhenAll(allTasks.ToArray());
            System.Threading.Thread.Sleep(10000);
        }

        [Fact]
        public async void ProcessOrderAsync_1SellOrderEntersOrderSetToCancel1BuyOrderEnter_NoTradeShouldBeExecute()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var orderId = await sut.ProcessOrderAsync(90, 15, Side.Sell);
            await sut.CancelOrder(orderId);

            //action
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
        public async void ProcessOrderAsync_CancellNoneValidOrder_ThrowException()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            //assert
            Assert.ThrowsAsync<NotImplementedException>(async () =>await  sut.CancelOrder(0));
        }

        [Fact]
        public async void ProcessOrderAsync_1SellOrderEntersOrderGetModified1BuyOrderEnters_OneTradeWithNewAmountShoudExecute()
        {
            //arrenge
            sut.PreOpen();
            sut.Open();

            var orderId = await sut.ProcessOrderAsync(90, 15, Side.Sell);
            await sut.ModifieOrder(orderId,100,5,DateTime.MaxValue);

            //action
            await sut.ProcessOrderAsync(100, 15, Side.Buy);

            //assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(5, sut.Trade.Sum(x => x.Amount));
            Assert.Equal(100, sut.Trade.Sum(x => x.Price));
            Assert.Equal(1, sut.AllOrdersCount());
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

    }
}