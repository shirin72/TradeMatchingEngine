namespace TradeMatchingEngine
{
    public interface ITrade
    {
        int Amount { get; }
        int BuyOrderId { get; }
        int OwnerId { get; }
        int Price { get; }
        int SellOrderId { get; }
        long TradeId { get; }
    }
}