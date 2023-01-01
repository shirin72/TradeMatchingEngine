using System.Collections.Generic;
using System.Linq;
using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{

    public class StocketMarketEngine
    {


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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            var orders = stockMarketMatchEngine.Orders;

            //Action
            stockMarketMatchEngine.Trade(order);

            var res = orders.Where(x => x.Side == Side.Buy).SingleOrDefault();

            var getBuyQueue = stockMarketMatchEngine.GetBuyOrderQueue();

            //Assert
            Assert.NotNull(res);
            Assert.NotNull(getBuyQueue);
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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            var orders = stockMarketMatchEngine.Orders;

            //Action
            stockMarketMatchEngine.Trade(order);

            var res = orders.Where(x => x.Side == Side.Sell).SingleOrDefault();

            var getSellQueue = stockMarketMatchEngine.GetSellOrderQueue();

            //Assert
            Assert.NotNull(getSellQueue);
            Assert.NotNull(res);

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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderSell);

            //Action
            stockMarketMatchEngine.Trade(orderBuy);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 0);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 0);
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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderBuy);

            //Action
            stockMarketMatchEngine.Trade(orderSell);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 0);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 0);


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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderBuy);

            //Action
            stockMarketMatchEngine.Trade(orderSell);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 1);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 1);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 0);
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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderSell);

            //Action
            stockMarketMatchEngine.Trade(orderBuy);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 0);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 1);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 1);
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

            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderBuy);

            //Action
            stockMarketMatchEngine.Trade(orderSell);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 0);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 1);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 1);
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



            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderSell);

            //Action
            stockMarketMatchEngine.Trade(orderBuy);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 1);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 1);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 0);
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


            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderSell);

            //Action
            stockMarketMatchEngine.Trade(orderBuy);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 0);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 1);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 2);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 1);
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



            var stockMarketMatchEngine = new StockMarketMatchEngine();

            stockMarketMatchEngine.Trade(orderBuy);

            //Action
            stockMarketMatchEngine.Trade(orderSell);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 0);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 1);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 2);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 1);
        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEntersWithDefferPricesAndAmounts_TheOrdersMustEnqueu()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 100,
                Side = Side.Sell
            };

            var orderSell2 = new Order()
            {
                Amount = 4,
                Id = 2,
                Price = 120,
                Side = Side.Sell
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 100,
                Side = Side.Sell
            };
            var orderSell4 = new Order()
            {
                Amount = 5,
                Id = 4,
                Price = 90,
                Side = Side.Sell
            };


            var stockMarketMatchEngine = new StockMarketMatchEngine();

           

            //Action
            stockMarketMatchEngine.Trade(orderSell1);
            stockMarketMatchEngine.Trade(orderSell2);
            stockMarketMatchEngine.Trade(orderSell3);
            stockMarketMatchEngine.Trade(orderSell4);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 0);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 3);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 4);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 0);
        }

        [Fact]
        public void StockMarketMatchEngine_BuyOrderEntersWithDefferPricesAndAmounts_TheOrdersMustEnqueu()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell2 = new Order()
            {
                Amount = 4,
                Id = 2,
                Price = 120,
                Side = Side.Buy
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 100,
                Side = Side.Buy
            };
            var orderSell4 = new Order()
            {
                Amount = 5,
                Id = 4,
                Price = 90,
                Side = Side.Buy
            };


            var stockMarketMatchEngine = new StockMarketMatchEngine();



            //Action
            stockMarketMatchEngine.Trade(orderSell1);
            stockMarketMatchEngine.Trade(orderSell2);
            stockMarketMatchEngine.Trade(orderSell3);
            stockMarketMatchEngine.Trade(orderSell4);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 0);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 0);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 4);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 3);
        }


        [Fact]
        public void StockMarketMatchEngine_BuyOrderEnters3WithDefferSellPriceInQueue_OneTradeMustBeExecuted()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell2 = new Order()
            {
                Amount = 4,
                Id = 2,
                Price = 120,
                Side = Side.Buy
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell4 = new Order()
            {
                Amount = 5,
                Id = 4,
                Price = 90,
                Side = Side.Buy
            };

            var sell = new Order()
            {
                Amount = 5,
                Id = 4,
                Price = 90,
                Side = Side.Sell
            };




            var stockMarketMatchEngine = new StockMarketMatchEngine();



            //Action
            stockMarketMatchEngine.Trade(orderSell1);
            stockMarketMatchEngine.Trade(orderSell2);
            stockMarketMatchEngine.Trade(orderSell3);
            stockMarketMatchEngine.Trade(orderSell4);
            stockMarketMatchEngine.Trade(sell);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 0);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 3);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 2);
        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEnters3WithDefferBuyPriceInQueue_OneTradeMustBeExecuted()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 4,
                Id = 1,
                Price = 100,
                Side = Side.Sell
            };

            var orderSell2 = new Order()
            {
                Amount = 4,
                Id = 2,
                Price = 120,
                Side = Side.Sell
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 100,
                Side = Side.Sell
            };

            var orderSell4 = new Order()
            {
                Amount = 5,
                Id = 4,
                Price = 90,
                Side = Side.Sell
            };

            var buyOrder = new Order()
            {
                Amount = 5,
                Id = 4,
                Price = 90,
                Side = Side.Buy
            };




            var stockMarketMatchEngine = new StockMarketMatchEngine();



            //Action
            stockMarketMatchEngine.Trade(orderSell1);
            stockMarketMatchEngine.Trade(orderSell2);
            stockMarketMatchEngine.Trade(orderSell3);
            stockMarketMatchEngine.Trade(orderSell4);
            stockMarketMatchEngine.Trade(buyOrder);

            //Assert
            Assert.Equal(stockMarketMatchEngine.TradeCount, 1);
            Assert.Equal(stockMarketMatchEngine.GetSellOrderQueue().Count, 2);
            Assert.Equal(stockMarketMatchEngine.Orders.Count, 3);
            Assert.Equal(stockMarketMatchEngine.GetBuyOrderQueue().Count, 0);
        }


        //#region Private
        //private PriorityQueue<pt, Order> MakeSellOrderEnqueu(Order order)
        //{
        //    var stockMarketMatchEngine = new StockMarketMatchEngine();

        //    stockMarketMatchEngine.Trade(order);

        //    return stockMarketMatchEngine.GetSellOrderQueue();

        //}

        //private PriorityQueue<Order, Order> MakeBuyOrderEnqueu(Order order)
        //{
        //    var stockMarketMatchEngine = new StockMarketMatchEngine();

        //    stockMarketMatchEngine.Trade(order);

        //    return stockMarketMatchEngine.GetBuyOrderQueue();
        //}
        //#endregion


    }
}