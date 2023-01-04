namespace TradeMatchingEngine
{
    public class PreOpened : IStockMarketMatchEngine
    {
        public void Close(StockMarketMatchEngine stockMarketMatchEngine)
        {
            stockMarketMatchEngine.StockMarketMatchEngineState = new Closed();
        }

        public void Open(StockMarketMatchEngine stockMarketMatchEngine)
        {
            stockMarketMatchEngine.Trade(null);
            stockMarketMatchEngine.StockMarketMatchEngineState = new Opened();
        }

        public void PreOpen(StockMarketMatchEngine stockMarketMatchEngine)
        {

        }
    }
}
