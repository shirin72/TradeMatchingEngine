namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngineWithState:IStockMarketMatchEngine
    {
        void Open();
        void PreOpen();
        void Close();
        MarcketState State { get; }
    }
}