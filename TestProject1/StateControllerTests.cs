using TradeMatchingEngine;
using Xunit;

namespace TestProject1
{
    public class StateControllerTests
    {
        private readonly StateController sut;
        private StockMarketMatchEngine stockMarketMatchEngine;
        public StateControllerTests()
        {
            stockMarketMatchEngine = new StockMarketMatchEngine();
            sut = new StateController(stockMarketMatchEngine);
        }

        [Trait("StateController","MarketState")]
        [Theory]
        [InlineData(ChangeStateNotify.NormalChange, MarketStateEnum.Close, MarketStateEnum.PreOpen)]
        [InlineData(ChangeStateNotify.NormalChange, MarketStateEnum.PreOpen, MarketStateEnum.Open)]
        [InlineData(ChangeStateNotify.NormalChange, MarketStateEnum.Open, MarketStateEnum.PreOpen)]
        [InlineData(ChangeStateNotify.ForcedChange, MarketStateEnum.Open, MarketStateEnum.Close)]
        [InlineData(ChangeStateNotify.ForcedChange, MarketStateEnum.PreOpen, MarketStateEnum.Close)]
        public void StateController_StateControllerHasBeenNotified_ChangeStateMustReturnPreOpenIfPreStateWasClosed(ChangeStateNotify value1, MarketStateEnum value2, MarketStateEnum expected)
        {
            //Arrange
            stockMarketMatchEngine.SetState(value2);
            //Action
            sut.ChangeState(value1);
            //Assert
            Assert.Equal(expected, stockMarketMatchEngine.GetCurrentMarketState());

        }

        [Trait("StateController","MarketState")]
        [Fact]
        public void StateController_StateControllerHasBeenNotified_ChangeStateMustOpenMustDoesTrade()
        {
            //Arrange
            stockMarketMatchEngine.SetState(MarketStateEnum.PreOpen);

            var sellOrder = new Order() {Id=1, Amount=10,Price=100,Side=Side.Sell};
            var buyOrder = new Order() {Id=2, Amount=10,Price=100,Side=Side.Buy};
            stockMarketMatchEngine.Trade(buyOrder); 
            stockMarketMatchEngine.Trade(sellOrder);

            stockMarketMatchEngine.SetState(MarketStateEnum.Open);

            //Action
            sut.ChangeState(ChangeStateNotify.NormalChange);

            //Assert
            Assert.Equal(MarketStateEnum.PreOpen, stockMarketMatchEngine.GetCurrentMarketState());
            Assert.Equal(1,stockMarketMatchEngine.TradeCount);
        }

        [Trait("StateController", "MarketState")]
        [Fact]
        public void StateController_StateControllerHasBeenClosed_ChangeStateOpenMustBeClosed()
        {
            //Arrange
            stockMarketMatchEngine.SetState(MarketStateEnum.Open);

            var sellOrder = new Order() { Id = 1, Amount = 10, Price = 100, Side = Side.Sell };
            var buyOrder = new Order() { Id = 2, Amount = 10, Price = 100, Side = Side.Buy };
            stockMarketMatchEngine.Trade(buyOrder);
            stockMarketMatchEngine.Trade(sellOrder);

            stockMarketMatchEngine.SetState(MarketStateEnum.Close);

            //Action
            sut.ChangeState(ChangeStateNotify.NormalChange);

            //Assert
            Assert.Equal(MarketStateEnum.PreOpen, stockMarketMatchEngine.GetCurrentMarketState());
            Assert.Equal(1, stockMarketMatchEngine.TradeCount);
        }

        [Trait("StateController", "MarketState")]
        [Fact]
        public void StateController_SeveralOrdersEntersInPreOrderStateThenStateChangedToOpenStateThenMultiOrdersEnter_SomeTradesMustBeDone()
        {
            //Arrange
            stockMarketMatchEngine.SetState(MarketStateEnum.PreOpen);

            var sellPreOrder = new Order() { Id = 1, Amount = 10, Price = 100, Side = Side.Sell };
            var sellPreOrder1 = new Order() { Id = 2, Amount = 10, Price = 100, Side = Side.Sell };
            var sellPreOrder2 = new Order() { Id = 3, Amount = 10, Price = 100, Side = Side.Sell };
            var sellPreOrder3 = new Order() { Id = 4, Amount = 10, Price = 100, Side = Side.Sell };

            var buyPreOrder = new Order() { Id = 5, Amount = 10, Price = 100, Side = Side.Buy };
            var buyPreOrder1 = new Order() { Id = 6, Amount = 10, Price = 100, Side = Side.Buy };

            var sellOrder = new Order() { Id = 7, Amount = 10, Price = 100, Side = Side.Sell };
            var sellOrder1 = new Order() { Id = 8, Amount = 10, Price = 100, Side = Side.Sell };

            var buyOrder = new Order() { Id = 11, Amount = 10, Price = 100, Side = Side.Buy };
            var buyOrder1 = new Order() { Id = 12, Amount = 10, Price = 100, Side = Side.Buy };
            var buyOrder2 = new Order() { Id = 13, Amount = 10, Price = 100, Side = Side.Buy };
            var buyOrder3 = new Order() { Id = 14, Amount = 10, Price = 100, Side = Side.Buy };

            stockMarketMatchEngine.Trade(sellPreOrder);
            stockMarketMatchEngine.Trade(sellPreOrder1);
            stockMarketMatchEngine.Trade(sellPreOrder2);
            stockMarketMatchEngine.Trade(sellPreOrder3);
            stockMarketMatchEngine.Trade(buyPreOrder);
            stockMarketMatchEngine.Trade(buyPreOrder1);

            sut.ChangeState(ChangeStateNotify.NormalChange);

            stockMarketMatchEngine.Trade(sellOrder);

            stockMarketMatchEngine.Trade(sellOrder1);

            stockMarketMatchEngine.Trade(buyOrder);

            stockMarketMatchEngine.Trade(buyOrder1);

            stockMarketMatchEngine.Trade(buyOrder2);

            stockMarketMatchEngine.Trade(buyOrder3);

            //Action
            sut.ChangeState(ChangeStateNotify.NormalChange);

            //Assert
            Assert.Equal(MarketStateEnum.PreOpen, stockMarketMatchEngine.GetCurrentMarketState());
            Assert.Equal(6, stockMarketMatchEngine.TradeCount);

        }

        [Trait("StateController", "MarketState")]
        [Fact]
        public void StateController_SeveralOrdersEntersInOpenStateThenStateChangedToPreStateThenTradeBecomeStop_SomeTradesMustBeDone()
        {
            //Arrange
            stockMarketMatchEngine.SetState(MarketStateEnum.Open);

            var sellPreOrder = new Order() { Id = 1, Amount = 10, Price = 100, Side = Side.Sell };
            var buyPreOrder = new Order() { Id = 5, Amount = 10, Price = 100, Side = Side.Buy };
            stockMarketMatchEngine.Trade(sellPreOrder);
            stockMarketMatchEngine.Trade(buyPreOrder);


            var sellOrder = new Order() { Id = 7, Amount = 10, Price = 100, Side = Side.Sell };
            var buyOrder = new Order() { Id = 11, Amount = 10, Price = 100, Side = Side.Buy };

            //Action
            sut.ChangeState(ChangeStateNotify.NormalChange);
            stockMarketMatchEngine.Trade(sellOrder);
            stockMarketMatchEngine.Trade(buyOrder);

            //Assert
            Assert.Equal(MarketStateEnum.PreOpen, stockMarketMatchEngine.GetCurrentMarketState());
            Assert.Equal(1, stockMarketMatchEngine.TradeCount);
            Assert.Equal(2, stockMarketMatchEngine.GetPreOrderQueue().Count);
        }
    }
}
