namespace TradeMatchingEngine
{
    public interface IStockMarketMatchEngine
    {
         Task<int> Enqueue(int price, int amount, Side side);

       // void ClearQueue();
    }
}
