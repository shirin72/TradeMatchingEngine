using System.Collections.Generic;
using System.Linq;
using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{
    public class StocketMarketEngine
    {
        private StockMarketMatchEngine sut =new StockMarketMatchEngine();
        public StocketMarketEngine()
        {
            sut = new StockMarketMatchEngine();
        }

        [Fact]
        public void StockMarketMatchEngine_EnqueueBuyWithoughtAnySellOrder_ShouldEnqueueBuy()
        {
            //Arrange
            var order = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orders = sut.Orders;

            //Action
            sut.Trade(order);

            //Assert
            Assert.NotNull(orders.Where(x => x.Side == Side.Buy).SingleOrDefault());
            Assert.NotNull(sut.GetBuyOrderQueue());
        }

        [Fact]
        public void StockMarketMatchEngine_EnqueueBuyWithoughtAnyBuyOrder_ShouldEnqueuSell()
        {
            //Arrange
            var order = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Sell
            };

            //Action
            sut.Trade(order);

            //Assert
            Assert.NotNull(sut.GetSellOrderQueue());
            Assert.NotNull(sut.Orders.Where(x => x.Side == Side.Sell).SingleOrDefault());

        }


        [Fact]
        public void StockMarketMatchEngine_TradeWithBuyOrder_ShouldTradeCountOne()
        {
            //Arrange
            var orderSell = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Sell
            };


            var orderBuy = new Order()
            {
                Amount = 5,
                Id = 2,
                Price = 100,
                Side = Side.Buy
            };


            sut.Trade(orderSell);

            //Action
            sut.Trade(orderBuy);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(0,sut.GetSellOrderQueue().Count);
            Assert.Equal(0,sut.GetBuyOrderQueue().Count);
        }

        [Fact]
        public void StockMarketMatchEngine_TradeWithSellOrder_ShouldTradeCountOne()
        {
            //Arrange
            var orderBuy = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell = new Order()
            {
                Amount = 5,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };

            sut.Trade(orderBuy);

            //Action
            sut.Trade(orderSell);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(0,sut.GetSellOrderQueue().Count);
            Assert.Equal(0,sut.GetBuyOrderQueue().Count);

        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEntersWithHigherAmount_SellOrderMostRemainsInQueue()
        {
            //Arrange
            var orderBuy = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };


            var orderSell = new Order()
            {
                Amount = 6,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };

            sut.Trade(orderBuy);

            //Action
            sut.Trade(orderSell);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(1,sut.GetSellOrderQueue().Count);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(0,sut.GetBuyOrderQueue().Count);
        }

        [Fact]
        public void StockMarketMatchEngine_BuyOrderEntersWithHigherAmount_BuyOrderMostRemainsInQueue()
        {
            //Arrange
            var orderSell = new Order()
            {
                Amount = 5,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };

            var orderBuy = new Order()
            {
                Amount = 6,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            sut.Trade(orderSell);

            //Action
            sut.Trade(orderBuy);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(0, sut.GetSellOrderQueue().Count);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(1,sut.GetBuyOrderQueue().Count);
        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEntersWithLowerAmount_BuyOrderMostRemainsInQueue()
        {
            //Arrange
            var orderBuy = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell = new Order()
            {
                Amount = 4,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };

            sut.Trade(orderBuy);

            //Action
            sut.Trade(orderSell);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(0,sut.GetSellOrderQueue().Count);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(1,sut.GetBuyOrderQueue().Count);
        }


        [Fact]
        public void StockMarketMatchEngine_BuyOrderEntersWithLowerAmount_SellOrderMostRemainsInQueue()
        {
            //Arrange
            var orderSell = new Order()
            {
                Amount = 5,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };

            var orderBuy = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            sut.Trade(orderSell);

            //Action
            sut.Trade(orderBuy);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(1,sut.GetSellOrderQueue().Count);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(0,sut.GetBuyOrderQueue().Count);
        }

        [Fact]
        public void StockMarketMatchEngine_BuyOrderEntersWithLowerPrice_TheOrderMustEnqueu()
        {
            //Arrange
            var orderSell = new Order()
            {
                Amount = 5,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };

            var orderBuy = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 90,
                Side = Side.Buy
            };

            sut.Trade(orderSell);

            //Action
            sut.Trade(orderBuy);

            //Assert
            Assert.Equal(0,sut.TradeCount);
            Assert.Equal(1,sut.GetSellOrderQueue().Count);
            Assert.Equal(2,sut.Orders.Count);
            Assert.Equal(1,sut.GetBuyOrderQueue().Count);
        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEntersWithHigherPrice_TheOrderMustEnqueu()
        {
            //Arrange

            var orderBuy = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell = new Order()
            {
                Amount = 5,
                Id = 2,
                Price = 120,
                Side = Side.Sell
            };

            sut.Trade(orderBuy);

            //Action
            sut.Trade(orderSell);

            //Assert
            Assert.Equal(0,sut.TradeCount);
            Assert.Equal(1,sut.GetSellOrderQueue().Count);
            Assert.Equal(2,sut.Orders.Count);
            Assert.Equal(1,sut.GetBuyOrderQueue().Count);
        }

    }
}