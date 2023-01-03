namespace TradeMatchingEngine
{
    internal class StateController
    {
        private StockMarketMatchEngine matchEngine;
        private List<Order> PreOrdersList;
        private MarketStateEnum MarketState;
        public StateController(StockMarketMatchEngine matchEngine)
        {
            PreOrdersList = new List<Order>();
            this.matchEngine = matchEngine;
        }

        public void Execute(Order order)
        {
            switch (MarketState)
            {
                case MarketStateEnum.Close:
                    throw new Exception("Out Of Currect Time");

                case MarketStateEnum.PreOpen:
                    PreOrdersList.Add(order);
                    break;
                case MarketStateEnum.Open:
                    if (PreOrdersList.Count>0)
                    {
                        foreach (var item in PreOrdersList)
                        {
                            matchEngine.Trade(item);
                        }
                    }

                    matchEngine.Trade(order);
                    break;
                default:
                    break;
            }
        }

        public void MarketState_HasChanged(object sender, EventArgs e)
        {
            Console.WriteLine("MarketStateEnum Has Changed!");
        }

    }


}
