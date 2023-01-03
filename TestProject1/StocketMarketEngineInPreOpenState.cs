using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{
    public class StocketMarketEngineInPreOpenState
    {
        private StockMarketMatchEngine sut;
        public StocketMarketEngineInPreOpenState()
        {
            sut = new StockMarketMatchEngine();
            sut.SetState(MarketStateEnum.PreOpen);
        }

        [Fact]
        public void StockMarketMatchEngine_1BuyOrderEnters_MustEnQueueToPreOrderQueue()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };

            //Action
            sut.Trade(buyOrder);    

            //Assert

            Assert.Equal(0,sut.TradeCount);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(1,sut.GetPreOrderQueue().Count);
            Assert.Equal(0,sut.GetBuyOrderCount());
            Assert.Equal(0,sut.GetSellOrderCount());


        }

        [Fact]
        public void StockMarketMatchEngine_1SellOrderEnters_MustEnQueueToPreOrderQueue()
        {
            //Arrange
            var sellOrder = new Order()
            {
                Id = 1,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };

            //Action
            sut.Trade(sellOrder);

            //Assert

            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetPreOrderQueue().Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());


        }

        [Fact]
        public void StockMarketMatchEngine_MultipleSellOrderEnters_MustEnQueueToPreOrderQueue()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 3,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            var sellOrder3 = new Order()
            {
                Id = 2,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            var sellOrder4 = new Order()
            {
                Id = 4,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            //Action
            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);
            sut.Trade(sellOrder4);

            //Assert

            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(4, sut.Orders.Count);
            Assert.Equal(4, sut.GetPreOrderQueue().Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());


        }

        [Fact]
        public void StockMarketMatchEngine_MultipleBuyOrderEnters_MustEnQueueToPreOrderQueue()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 3,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var buyOrder3 = new Order()
            {
                Id = 2,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var buyOrder4 = new Order()
            {
                Id = 4,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            //Action
            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);
            sut.Trade(buyOrder4);

            //Assert

            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(4, sut.Orders.Count);
            Assert.Equal(4, sut.GetPreOrderQueue().Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());


        }

        [Fact]
        public void StockMarketMatchEngine_MultipleBuyAndSellOrderEnters_MustEnQueueToPreOrderQueue()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 3,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var buyOrder3 = new Order()
            {
                Id = 2,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var buyOrder4 = new Order()
            {
                Id = 4,
                Amount = 5,
                Price = 100,
                Side = Side.Buy
            };
            var sellOrder1 = new Order()
            {
                Id = 5,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 6,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            var sellOrder3 = new Order()
            {
                Id = 7,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            var sellOrder4 = new Order()
            {
                Id = 4,
                Amount = 5,
                Price = 100,
                Side = Side.Sell
            };
            //Action
            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);
            sut.Trade(buyOrder4);
            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);
            sut.Trade(sellOrder4);
            //Assert

            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(8, sut.Orders.Count);
            Assert.Equal(8, sut.GetPreOrderQueue().Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());


        }
    }
}
