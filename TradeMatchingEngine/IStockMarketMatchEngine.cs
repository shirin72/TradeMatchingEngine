namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
        void Close();

        void PreOpen();

        void Open();
    }
}
