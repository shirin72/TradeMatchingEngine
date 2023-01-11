namespace TradeMatchingEngine
{
    internal class Trade : ITrade
    {
        public Trade(long tradeId, int ownerId, int buyOrderId, int sellOrderId, int amount, int price)
        {
            TradeId = tradeId;
            OwnerId = ownerId;
            BuyOrderId = buyOrderId;
            SellOrderId = sellOrderId;
            Amount = amount;
            Price = price;
        }
        public long TradeId { get; }
        public int OwnerId { get; }
        public int BuyOrderId { get; }
        public int SellOrderId { get; }
        public int Amount { get; }
        public int Price { get; }
    }
}
