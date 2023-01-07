namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
        void Enqueue(int price, int amount, Side side);

        void ClearQueue();
    }
}
