using System.Collections.Generic;
using System.Linq;
using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{

    public class StocketMarketEngine
    {
        private readonly StockMarketMatchEngine sut;   
        public StocketMarketEngine( )
        {

            sut =new StockMarketMatchEngine();

            
        }
        [Fact]
        public void EnqueueBuyWithoughtAnySellOrder_ShouldEnqueueBuy()
        {
            //Arrange
            var order = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };
            
            //Action
            sut.Trade(order);


            //Assert
            var res = sut.Orders.Where(x => x.Side == Side.Buy).SingleOrDefault();
            var getBuyQueue = sut.GetBuyOrderQueue();
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

           

            var orders = sut.Orders;

            //Action
            sut.Trade(order);

            var res = orders.Where(x => x.Side == Side.Sell).SingleOrDefault();

            var getSellQueue = sut.GetSellOrderQueue();

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

            

            sut.Trade(orderSell);

            //Action
            sut.Trade(orderBuy);

            //Assert
            Assert.Equal( 1,sut.TradeCount);
            Assert.Equal( 0,sut.GetSellOrderQueue().Count);
            Assert.Equal( 0,sut.GetBuyOrderQueue().Count);
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
            Assert.Equal( 1,sut.TradeCount);
            Assert.Equal( 0,sut.GetSellOrderQueue().Count);
            Assert.Equal( 0,sut.GetBuyOrderQueue().Count);
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
            Assert.Equal( 1,sut.TradeCount);
            Assert.Equal( 1,sut.GetSellOrderQueue().Count);
            Assert.Equal( 1,sut.Orders.Count);
            Assert.Equal( 0,sut.GetBuyOrderQueue().Count);
            Assert.Equal(1, sut.Orders.Where(x => x.Side == Side.Sell).Sum(x => x.Amount));

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
            Assert.Equal(0,sut.GetSellOrderQueue().Count);
            Assert.Equal(1,sut.Orders.Count);
            Assert.Equal(1,sut.GetBuyOrderQueue().Count);
            Assert.Equal(1, sut.Orders.Where(x => x.Side == Side.Buy).Sum(x => x.Amount));

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
            Assert.Equal(1, sut.Orders.Where(x => x.Side == Side.Buy).Sum(x => x.Amount));

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
            Assert.Equal(1, sut.Orders.Where(x=>x.Side==Side.Sell).Sum(x=>x.Amount));
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
            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);

            //Action

            sut.Trade(orderSell4);

            //Assert
            Assert.Equal(0,sut.TradeCount);
            Assert.Equal(3,sut.GetSellOrderQueue().Count);
            Assert.Equal(4,sut.Orders.Count);
            Assert.Equal(0,sut.GetBuyOrderQueue().Count);
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


            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);


            //Action
            sut.Trade(orderSell4);

            //Assert

            Assert.Equal(0,sut.TradeCount);
            Assert.Equal(0,sut.GetSellOrderQueue().Count);
            Assert.Equal(4,sut.Orders.Count);
            Assert.Equal(3,sut.GetBuyOrderQueue().Count);
            Assert.Equal(1, sut.Orders.FirstOrDefault().Id);
            Assert.Equal(4, sut.Orders.LastOrDefault().Id);
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
                Id = 5,
                Price = 90,
                Side = Side.Sell
            };


            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);
            sut.Trade(orderSell4);

            //Action
            sut.Trade(sell);

            //Assert
            Assert.Equal(2,sut.TradeCount );
            Assert.Equal(0,sut.GetSellOrderQueue().Count);
            Assert.Equal(3,sut.Orders.Count);
            Assert.Equal(2,sut.GetBuyOrderQueue().Count);
            Assert.Equal(10, sut.Orders.Where(x=>x.Side==Side.Buy).Sum(x=>x.Amount));
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

            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);
            sut.Trade(orderSell4);


            //Action

            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(1,sut.TradeCount);
            Assert.Equal(2,sut.GetSellOrderQueue().Count);
            Assert.Equal(3,sut.Orders.Count);
            Assert.Equal(0,sut.GetBuyOrderQueue().Count);
            Assert.Equal(10, sut.Orders.Where(x=>x.Side==Side.Sell).Sum(x=>x.Amount));
        }

        [Fact]
        public void StockMarketMatchEngine_BuyOrderEntersAndPerforms2Trades_TowTradeMustBeExecutedAndOneSaleMustBeInQueue()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Sell
            };

            var orderSell2 = new Order()
            {
                Amount = 3,
                Id = 2,
                Price = 100,
                Side = Side.Sell
            };
            var orderSell3 = new Order()
            {
                Amount =2,
                Id = 3,
                Price = 110,
                Side = Side.Sell
            };

            var buyOrder = new Order()
            {
                Amount = 8,
                Id = 4,
                Price = 100,
                Side = Side.Buy
            };

            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);

            //Action

            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(1, sut.GetSellOrderQueue().Count);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(0, sut.GetBuyOrderQueue().Count);
            Assert.Equal(2, sut.Orders.Where(x => x.Side == Side.Sell).Sum(x => x.Amount));
        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEntersAndPerforms3Trades_TowTradeMustBeExecutedAndOneBuyMustBeInQueue()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 100,
                Side = Side.Buy
            };

            var orderSell2 = new Order()
            {
                Amount = 3,
                Id = 2,
                Price = 100,
                Side = Side.Buy
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 110,
                Side = Side.Buy
            };

            var buyOrder = new Order()
            {
                Amount = 8,
                Id = 4,
                Price = 100,
                Side = Side.Sell
            };

            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);

            //Action

            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(3, sut.TradeCount);
            Assert.Equal(0, sut.GetSellOrderQueue().Count);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderQueue().Count);
            Assert.Equal(2, sut.Orders.Where(x => x.Side == Side.Buy).Sum(x => x.Amount));
        }

        [Fact]
        public void StockMarketMatchEngine_SellOrderEntersAndPerforms2Trades_TowTradeMustBeExecutedAndOneBuyMustBeInQueue()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 5,
                Id = 1,
                Price = 110,
                Side = Side.Buy
            };

            var orderSell2 = new Order()
            {
                Amount = 3,
                Id = 2,
                Price = 110,
                Side = Side.Buy
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 100,
                Side = Side.Buy
            };

            var buyOrder = new Order()
            {
                Amount = 8,
                Id = 4,
                Price = 110,
                Side = Side.Sell
            };

            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);

            //Action

            sut.Trade(buyOrder);

            //Assert
            Assert.Equal(2, sut.TradeCount);
            Assert.Equal(0, sut.GetSellOrderQueue().Count);
            Assert.Equal(1, sut.Orders.Count);
            Assert.Equal(1, sut.GetBuyOrderQueue().Count);
            Assert.Equal(2, sut.Orders.Where(x => x.Side == Side.Buy).Sum(x => x.Amount));
        }




        [Fact]
        public void StockMarketMatchEngine_()
        {
            //Arrange

            var orderSell1 = new Order()
            {
                Amount = 10,
                Id = 1,
                Price = 110,
                Side = Side.Sell
            };

            var orderSell2 = new Order()
            {
                Amount = 3,
                Id = 2,
                Price = 110,
                Side = Side.Sell
            };
            var orderSell3 = new Order()
            {
                Amount = 2,
                Id = 3,
                Price = 100,
                Side = Side.Sell
            };

            var buyOrder = new Order()
            {
                Amount = 8,
                Id = 4,
                Price = 110,
                Side = Side.Buy
            };

            var buyOrder1 = new Order()
            {
                Amount = 8,
                Id = 5,
                Price = 50,
                Side = Side.Buy
            };

            sut.Trade(orderSell1);
            sut.Trade(orderSell2);
            sut.Trade(orderSell3);
            sut.Trade(orderSell3);
            sut.Trade(buyOrder1);

            //Action
            sut.Trade(buyOrder);

            //Assert
            //Assert.Equal(2, sut.TradeCount);
            //Assert.Equal(0, sut.GetSellOrderQueue().Count);
            //Assert.Equal(1, sut.Orders.Count);
            //Assert.Equal(1, sut.GetBuyOrderQueue().Count);
            //Assert.Equal(2, sut.Orders.Where(x => x.Side == Side.Buy).Sum(x => x.Amount));
        }
    }
}