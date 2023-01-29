namespace TradeMatchingEngine
{
    internal interface IStockMarketMatchEngineWithState:IStockMarketMatchEngine
    {
        void Open();
        void PreOpen();
        void Close();
        MarcketState State { get; }
    }
}