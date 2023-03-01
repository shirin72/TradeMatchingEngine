namespace Domain
{
    public interface ITrade
    {
        int Amount { get; }
        long BuyOrderId { get; }
       // int OwnerId { get; }
        int Price { get; }
        long SellOrderId { get; }
        long Id { get; }
    }
}