namespace TradeMatchingEngine
{
    public class StateController
    {
        #region Private
        private StockMarketMatchEngine stockMarketMatchEngine;
        #endregion

        #region Constructor
        public StateController(StockMarketMatchEngine stockMarketMatchEngine)
        {
            this.stockMarketMatchEngine = stockMarketMatchEngine;
        }
        #endregion


        #region PublicMethod
        public void ChangeState(ChangeStateNotify changeStateNotify)
        {
            if (changeStateNotify == ChangeStateNotify.ForcedChange)
            {
                stockMarketMatchEngine.SetState(MarketStateEnum.Close);
            }
            else
            {
                var priviousState = MarketStateEnum.Close;

                var currentState = stockMarketMatchEngine.GetCurrentMarketState();

                if (currentState == MarketStateEnum.Close)
                {
                    stockMarketMatchEngine.SetState(MarketStateEnum.PreOpen);
                    priviousState = (MarketStateEnum.Close);
                }
                else if (currentState == MarketStateEnum.PreOpen)
                {
                    if (priviousState == MarketStateEnum.Close)
                    {
                        stockMarketMatchEngine.SetState(MarketStateEnum.Open);
                        priviousState = (MarketStateEnum.PreOpen);
                    }
                    else if (priviousState == MarketStateEnum.Open)
                    {
                        stockMarketMatchEngine.SetState(MarketStateEnum.Close);
                        priviousState = (MarketStateEnum.Open);
                    }
                }
                else if (currentState == MarketStateEnum.Open)
                {
                    stockMarketMatchEngine.SetState(MarketStateEnum.PreOpen);
                    priviousState = (MarketStateEnum.Open);
                }
            }
        }

        public void MarketState_HasChanged(object sender, EventArgs e)
        {
            var castObj = e as MarketStateEngine;

            Console.WriteLine("MarketStateEnum Has Changed! {0}", castObj.ChangeStateNotify);

            ChangeState(castObj.ChangeStateNotify);
        }
        #endregion

    }
}
