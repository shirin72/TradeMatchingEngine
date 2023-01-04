namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
        void Close();

        void PreOpen();

        void Open();

        void Enqueue(int price,int amount, Side side);
    }
}
