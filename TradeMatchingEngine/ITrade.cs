namespace Domain
{
    public interface ITrade
    {
        int Amount { get; }
        long BuyOrderId { get; }
        int Price { get; }
        long SellOrderId { get; }
        long Id { get; }
    }
}