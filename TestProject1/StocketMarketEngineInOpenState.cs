using System.Collections.Generic;
using System.Linq;
using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{
    public class StocketMarketEngineInOpenState
    {
        private StockMarketMatchEngine sut;
        public StocketMarketEngineInOpenState()
        {
            sut = new StockMarketMatchEngine();
            sut.SetState(MarketStateEnum.Open);
        }

        #region Commented Tests
        //    [Fact]
        //    public void StockMarketMatchEngine_EnqueueBuyWithoughtAnySellOrder_ShouldEnqueueBuy()
        //    {
        //        //Arrange
        //        var order = new Order()
        //        {
        //            Amount = 5,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        var orders = sut.Orders;

        //        //Action
        //        sut.Trade(order);

        //        //Assert
        //        Assert.NotNull(orders.Where(x => x.Side == Side.Buy).SingleOrDefault());
        //        Assert.NotNull(sut.GetBuyOrderQueue());
        //    }

        //    [Fact]
        //    public void StockMarketMatchEngine_EnqueueBuyWithoughtAnyBuyOrder_ShouldEnqueuSell()
        //    {
        //        //Arrange
        //        var order = new Order()
        //        {
        //            Amount = 5,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        //Action
        //        sut.Trade(order);

        //        //Assert
        //        Assert.NotNull(sut.GetSellOrderQueue());
        //        Assert.NotNull(sut.Orders.Where(x => x.Side == Side.Sell).SingleOrDefault());

        //    }


        //    [Fact]
        //    public void StockMarketMatchEngine_TradeWithBuyOrder_ShouldTradeCountOne()
        //    {
        //        //Arrange
        //        var orderSell = new Order()
        //        {
        //            Amount = 5,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Sell
        //        };


        //        var orderBuy = new Order()
        //        {
        //            Amount = 5,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Buy
        //        };


        //        sut.Trade(orderSell);

        //        //Action
        //        sut.Trade(orderBuy);

        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(0, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);
        //    }

        //    [Fact]
        //    public void StockMarketMatchEngine_TradeWithSellOrder_ShouldTradeCountOne()
        //    {
        //        //Arrange
        //        var orderBuy = new Order()
        //        {
        //            Amount = 5,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        var orderSell = new Order()
        //        {
        //            Amount = 5,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        sut.Trade(orderBuy);

        //        //Action
        //        sut.Trade(orderSell);

        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(0, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);

        //    }

        //    [Fact]
        //    public void StockMarketMatchEngine_SellOrderEntersWithHigherAmount_SellOrderMostRemainsInQueue()
        //    {
        //        //Arrange
        //        var orderBuy = new Order()
        //        {
        //            Amount = 5,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };


        //        var orderSell = new Order()
        //        {
        //            Amount = 6,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        sut.Trade(orderBuy);

        //        //Action
        //        sut.Trade(orderSell);

        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(1, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);
        //    }

        //    [Fact]
        //    public void StockMarketMatchEngine_BuyOrderEntersWithHigherAmount_BuyOrderMostRemainsInQueue()
        //    {
        //        //Arrange
        //        var orderSell = new Order()
        //        {
        //            Amount = 5,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        var orderBuy = new Order()
        //        {
        //            Amount = 6,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        sut.Trade(orderSell);

        //        //Action
        //        sut.Trade(orderBuy);

        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(0, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(1, sut.GetBuyOrderQueue().Count);
        //    }
        //    [Fact]
        //    public void StockMarketMatchEngine_SellOrderEntersWithHigherAmount_SellOrderMostRemainsInQueue2()
        //    {
        //        //Arrange
        //        var orderSell = new Order()
        //        {
        //            Amount = 8,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        var orderBuy = new Order()
        //        {
        //            Amount = 6,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        sut.Trade(orderBuy);

        //        //Action

        //        sut.Trade(orderSell);
        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(1, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);
        //    }
        //    [Fact]
        //    public void StockMarketMatchEngine_SellOrderEntersWithLowerAmount_BuyOrderMostRemainsInQueue()
        //    {
        //        //Arrange
        //        var orderBuy = new Order()
        //        {
        //            Amount = 5,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        var orderSell = new Order()
        //        {
        //            Amount = 4,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        sut.Trade(orderBuy);

        //        //Action
        //        sut.Trade(orderSell);

        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(0, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(1, sut.GetBuyOrderQueue().Count);
        //    }


        //    [Fact]
        //    public void StockMarketMatchEngine_BuyOrderEntersWithLowerAmount_SellOrderMostRemainsInQueue()
        //    {
        //        //Arrange
        //        var orderSell = new Order()
        //        {
        //            Amount = 5,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        var orderBuy = new Order()
        //        {
        //            Amount = 4,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        sut.Trade(orderSell);

        //        //Action
        //        sut.Trade(orderBuy);

        //        //Assert
        //        Assert.Equal(1, sut.TradeCount);
        //        Assert.Equal(1, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);
        //    }

        //    [Fact]
        //    public void StockMarketMatchEngine_BuyOrderEntersWithLowerPrice_TheOrderMustEnqueu()
        //    {
        //        //Arrange
        //        var orderSell = new Order()
        //        {
        //            Amount = 5,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        var orderBuy = new Order()
        //        {
        //            Amount = 4,
        //            Id = 1,
        //            Price = 90,
        //            Side = Side.Buy
        //        };

        //        sut.Trade(orderSell);

        //        //Action
        //        sut.Trade(orderBuy);

        //        //Assert
        //        Assert.Equal(0, sut.TradeCount);
        //        Assert.Equal(1, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(2, sut.Orders.Count);
        //        Assert.Equal(1, sut.GetBuyOrderQueue().Count);
        //    }

        //    [Fact]
        //    public void StockMarketMatchEngine_SellOrderEntersWithHigherPrice_TheOrderMustEnqueu()
        //    {
        //        //Arrange

        //        var orderBuy = new Order()
        //        {
        //            Amount = 4,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        var orderSell = new Order()
        //        {
        //            Amount = 5,
        //            Id = 2,
        //            Price = 120,
        //            Side = Side.Sell
        //        };

        //        sut.Trade(orderBuy);

        //        //Action
        //        sut.Trade(orderSell);

        //        //Assert
        //        Assert.Equal(0, sut.TradeCount);
        //        Assert.Equal(1, sut.GetSellOrderQueue().Count);
        //        Assert.Equal(2, sut.Orders.Count);
        //        Assert.Equal(1, sut.GetBuyOrderQueue().Count);
        //    }
        //    [Fact]
        //    public void StockMarketMatchEngine_severalSellOrdersEnters1BuywithHighAmount_The1SellOrderMustEnqueu()
        //    {
        //        //Arrange
        //        var sellOrder1 = new Order()
        //        {
        //            Amount = 4,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder2 = new Order()
        //        {
        //            Amount = 2,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder3 = new Order()
        //        {
        //            Amount = 6,
        //            Id = 3,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var buyOrder = new Order()
        //        {
        //            Amount = 8,
        //            Id = 4,
        //            Price = 100,
        //            Side = Side.Buy
        //        };
        //        sut.Trade(sellOrder1);
        //        sut.Trade(sellOrder2);
        //        sut.Trade(sellOrder3);
        //        //Action
        //        sut.Trade(buyOrder);

        //        //Assert

        //        Assert.Equal(3, sut.TradeCount);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);
        //        Assert.Equal(1, sut.GetSellOrderQueue().Count);
        //    }
        //    [Fact]
        //    public void StockMarketMatchEngine_severalSellOrdersEnters1BuywithHighAmount_TheBuyOrderMustEnqueu()
        //    {
        //        //Arrange
        //        var sellOrder1 = new Order()
        //        {
        //            Amount = 4,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder2 = new Order()
        //        {
        //            Amount = 2,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder3 = new Order()
        //        {
        //            Amount = 6,
        //            Id = 3,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var buyOrder = new Order()
        //        {
        //            Amount = 13,
        //            Id = 4,
        //            Price = 100,
        //            Side = Side.Buy
        //        };
        //        sut.Trade(sellOrder1);
        //        sut.Trade(sellOrder2);
        //        sut.Trade(sellOrder3);
        //        //Action
        //        sut.Trade(buyOrder);

        //        //Assert

        //        Assert.Equal(3, sut.TradeCount);
        //        Assert.Equal(1, sut.Orders.Count);
        //        Assert.Equal(1, sut.GetBuyOrderQueue().Count);
        //        Assert.Equal(0, sut.GetSellOrderQueue().Count);
        //    }
        //    [Fact]
        //    public void StockMarketMatchEngine_CheckForSellsByIdSortingInSamePrice_TheQueueMostBeSortedByIdToo()
        //    {
        //        //Arrange
        //        var sellOrder1 = new Order()
        //        {
        //            Amount = 4,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder2 = new Order()
        //        {
        //            Amount = 2,
        //            Id = 3,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder3 = new Order()
        //        {
        //            Amount = 6,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Sell
        //        };
        //        var sellOrder4 = new Order()
        //        {
        //            Amount = 8,
        //            Id = 4,
        //            Price = 100,
        //            Side = Side.Sell
        //        };

        //        //Action
        //        sut.Trade(sellOrder4);
        //        sut.Trade(sellOrder1);
        //        sut.Trade(sellOrder2);
        //        sut.Trade(sellOrder3);


        //        //Assert

        //        Assert.Equal(4, sut.Orders.Count);
        //        Assert.Equal(0, sut.GetBuyOrderQueue().Count);
        //        Assert.Equal(4, sut.GetSellOrderQueue().Count);
        //    }
        //    [Fact]
        //    public void StockMarketMatchEngine_CheckForBuysByIdSortingInSamePrice_TheQueueMostBeSortedByIdToo()
        //    {
        //        //Arrange
        //        var buyOrder1 = new Order()
        //        {
        //            Amount = 4,
        //            Id = 2,
        //            Price = 100,
        //            Side = Side.Buy
        //        };
        //        var buyOrder2 = new Order()
        //        {
        //            Amount = 2,
        //            Id = 3,
        //            Price = 100,
        //            Side = Side.Buy
        //        };
        //        var buyOrder3 = new Order()
        //        {
        //            Amount = 6,
        //            Id = 1,
        //            Price = 100,
        //            Side = Side.Buy
        //        };
        //        var buyOrder4 = new Order()
        //        {
        //            Amount = 8,
        //            Id = 4,
        //            Price = 100,
        //            Side = Side.Buy
        //        };

        //        //Action
        //        sut.Trade(buyOrder1);
        //        sut.Trade(buyOrder2);
        //        sut.Trade(buyOrder3);
        //        sut.Trade(buyOrder4);

        //        //Assert

        //        Assert.Equal(4, sut.Orders.Count);
        //        Assert.Equal(4, sut.GetBuyOrderQueue().Count);
        //        Assert.Equal(0, sut.GetSellOrderQueue().Count);
        //    }
        //}
        #endregion

        [Fact]
        [Trait("StockMarketMatchEngine", "Open")]
        public void StockMarketMatchEngine_FirstSellOrderEnters_MustEnqueue1SellOrder()
        {
            //Arrange

            var sellOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };

            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(0,sut.TradeCount);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(0,sut.GetBuyOrderCount());
            Assert.Equal(1,sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_FirstBuyOrderEnters_MustEnqueue1BuyOrder()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };

            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralSellOrderEnters_MustEnqueueAllOrder()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };

            var sellOrder2 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };

            var sellOrder3 = new Order()
            {
                Id = 2,
                Price = 110,
                Amount = 10,
                Side = Side.Sell
            };
            var sellOrder4 = new Order()
            {
                Id = 5,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };
            var sellOrder5 = new Order()
            {
                Id = 4,
                Price = 120,
                Amount = 10,
                Side = Side.Sell
            };
            //Action
            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);
            sut.Trade(sellOrder4);
            sut.Trade(sellOrder5);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(5, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(5, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralBuyOrderEnters_MustEnqueueAllOrder()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };

            var buyOrder2 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };

            var buyOrder3 = new Order()
            {
                Id = 2,
                Price = 110,
                Amount = 10,
                Side = Side.Buy
            };
            var buyOrder4 = new Order()
            {
                Id = 5,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };
            var buyOrder5 = new Order()
            {
                Id = 4,
                Price = 120,
                Amount = 10,
                Side = Side.Buy
            };
            //Action
            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);
            sut.Trade(buyOrder4);
            sut.Trade(buyOrder5);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(5, sut.Orders.Count);
            Assert.Equal(5, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralBuyAndSellOrderEnters_MustEnqueueAllOrder()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };

            var buyOrder2 = new Order()
            {
                Id = 3,
                Price = 110,
                Amount = 5,
                Side = Side.Buy
            };

            var sellOrder1 = new Order()
            {
                Id = 2,
                Price = 120,
                Amount = 10,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 5,
                Price = 125,
                Amount = 10,
                Side = Side.Sell
            };
            var sellOrder3 = new Order()
            {
                Id = 4,
                Price = 120,
                Amount = 10,
                Side = Side.Sell
            };
            //Action
            sut.Trade(buyOrder1);
            sut.Trade(sellOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);

            //Assert
            Assert.Equal(0, sut.TradeCount);
            Assert.Equal(5, sut.Orders.Count);
            Assert.Equal(2, sut.GetBuyOrderCount());
            Assert.Equal(3, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SellEntersFirstBuyOrderEntersWithSamePriceAndAmount_MustExecute1Trade()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };

            sut.Trade(buyOrder);

            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_BuyEntersFirstSellOrderEntersWithSamePriceAndAmount_MustExecute1Trade()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };

            sut.Trade(sellOrder);


            //Action
            sut.Trade(buyOrder);
            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(0, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_BuyEntersFirstSellOrderEntersWithHigherAmount_MustExecute1TradeAndEnqueue1Sell()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };


            sut.Trade(buyOrder);

            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SellEntersFirstBuyOrderEntersWithHigherAmount_MustExecute1TradeAndEnqueue1Buy()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 15,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };


            sut.Trade(sellOrder);


            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_BuyEntersFirstSellOrderEntersWithLowerAmount_MustExecute1TradeAndEnqueue1Buy()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 15,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };


            sut.Trade(buyOrder);


            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SellEntersFirstSBuyOrderEntersWithLowerAmount_MustExecute1TradeAndEnqueue1Sell()
        {
            //Arrange
            var buyOrder = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 15,
                Side = Side.Sell
            };


            sut.Trade(sellOrder);


            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralSellsEntersFirstSBuyOrderEntersWithCoverOfAllAmount_MustExecute2TradeAndClearAllQueues()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 12,
                Side = Side.Sell
            };

            var buyOrder = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 20,
                Side = Side.Buy
            };


            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);


            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralBuysEntersFirstSSellOrderEntersWithCoverOfAllAmount_MustExecute2TradeAndClearAllQueues()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 12,
                Side = Side.Buy
            };

            var sellOrder = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 20,
                Side = Side.Sell
            };


            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);


            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralSellsEntersFirstSBuyOrderEntersWithCoverOf1AndHalfOfSecondSell_MustExecute2TradeAndEnQueueSecondSell()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };

            var sellOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };
            var buyOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 13,
                Side = Side.Buy
            };

            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);

            


            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(2, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(2, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralBuysEntersFirstSellOrderEntersWithCoverOf1AndHalfOfSecondBuy_MustExecute2TradeAndEnQueueSecondBuy()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 10,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };

            var buyOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 13,
                Side = Side.Sell
            };

            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);




            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(2, sut.Orders.Count);
            Assert.Equal(2, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralSellsEntersFirstBuyOrderEntersWithHigherAmount_MustExecute3TradeAndEnQueue1Buy()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 2,
                Side = Side.Sell
            };

            var sellOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };
            var buyOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 17,
                Side = Side.Buy
            };

            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);




            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(3, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralBuysEntersFirstSellOrderEntersWithHigherAmount_MustExecute3TradeAndEnQueue1Sell()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 2,
                Side = Side.Buy
            };

            var buyOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 17,
                Side = Side.Sell
            };

            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);




            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(3, sut.TradeCount);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        [Trait("StockMarketMatchEngine", "Open")]
        public void StockMarketMatchEngine_SeveralSellsEntersFirstBuyOrderEntersWithLowerAmount_MustExecute1TradeAndEnQueueFirstSell()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 2,
                Side = Side.Sell
            };

            var sellOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };
            var buyOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 4,
                Side = Side.Buy
            };

            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);




            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(3, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(3, sut.GetSellOrderCount());
        }

        [Fact]
        [Trait("StockMarketMatchEngine", "Open")]
        public void StockMarketMatchEngine_SeveralBuysEntersFirstSellOrderEntersWithLowerAmount_MustExecute1TradeAndEnQueueFirstBuy()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 2,
                Side = Side.Buy
            };

            var buyOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };
            var sellOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 4,
                Side = Side.Sell
            };

            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);




            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(3, sut.Orders.Count);
            Assert.Equal(3, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Trait("StockMarketMatchEngine", "Open")]
        [Fact]
        public void StockMarketMatchEngine_SeveralSellsEntersWithDefferPriceFirstBuyOrderEntersWithLowerAmount_MustExecute1TradeAndEnQueueFirstSell()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 2,
                Side = Side.Sell
            };

            var sellOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };
            var sellOrder4 = new Order()
            {
                Id = 5,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };
            var sellOrder5 = new Order()
            {
                Id = 6,
                Price = 100,
                Amount = 8,
                Side = Side.Sell
            };

            var buyOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 4,
                Side = Side.Buy
            };

            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);
            sut.Trade(sellOrder3);
            sut.Trade(sellOrder4);
            sut.Trade(sellOrder5);




            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(5, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderCount());
            Assert.Equal(5, sut.GetSellOrderCount());
        }

        [Fact]
        [Trait("StockMarketMatchEngine", "Open")]
        public void StockMarketMatchEngine_SeveralBuysEntersWithDefferPriceFirstSellOrderEntersWithLowerAmount_MustExecute1TradeAndEnQueueFirstBuy()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 2,
                Price = 100,
                Amount = 2,
                Side = Side.Buy
            };

            var buyOrder3 = new Order()
            {
                Id = 3,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };
            var buyOrder4 = new Order()
            {
                Id = 5,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };
            var buyOrder5 = new Order()
            {
                Id = 6,
                Price = 100,
                Amount = 8,
                Side = Side.Buy
            };

            var sellOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 4,
                Side = Side.Sell
            };

            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            sut.Trade(buyOrder3);
            sut.Trade(buyOrder4);
            sut.Trade(buyOrder5);




            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(5, sut.Orders.Count);
            Assert.Equal(5, sut.GetBuyOrderCount());
            Assert.Equal(0, sut.GetSellOrderCount());
        }

        [Fact]
        [Trait("StockMarketMatchEngine", "Open")]
        public void StockMarketMatchEngine_SeveralBuysWithDefferPriceEntersFirstSellOrderEntersWithHigherAmount_MustExecute1TradeAndEnQueueFirstSell()
        {
            //Arrange
            var buyOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Buy
            };
            var buyOrder2 = new Order()
            {
                Id = 2,
                Price = 90,
                Amount = 2,
                Side = Side.Buy
            };


            var sellOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 6,
                Side = Side.Sell
            };

            sut.Trade(buyOrder1);
            sut.Trade(buyOrder2);
            
            //Action
            sut.Trade(sellOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(2, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }

        [Fact]
        [Trait("StockMarketMatchEngine", "Open")]
        public void StockMarketMatchEngine_SeveralSellsWithDefferPriceEntersFirstBuyOrderEntersWithHigherAmount_MustExecute1TradeAndEnQueueFirstBuy()
        {
            //Arrange
            var sellOrder1 = new Order()
            {
                Id = 1,
                Price = 100,
                Amount = 5,
                Side = Side.Sell
            };
            var sellOrder2 = new Order()
            {
                Id = 2,
                Price = 110,
                Amount = 2,
                Side = Side.Sell
            };


            var buyOrder = new Order()
            {
                Id = 4,
                Price = 100,
                Amount = 6,
                Side = Side.Buy
            };

            sut.Trade(sellOrder1);
            sut.Trade(sellOrder2);

            //Action
            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(1, sut.TradeCount);
            Assert.Equal(2, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderCount());
            Assert.Equal(1, sut.GetSellOrderCount());
        }
    }
}