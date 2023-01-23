namespace TradeMatchingEngine.Trades.Dto
{
    public class Trade
    {
        public long Id { get; set; }
        public int OwnerId { get; set; }
        public int BuyOrderId { get; set; }
        public int SellOrderId { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
    }
}
