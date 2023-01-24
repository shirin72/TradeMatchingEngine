namespace TradeMatchingEngine
{
    public class Trade : ITrade
    {
        public Trade(long id, int ownerId, int buyOrderId, int sellOrderId, int amount, int price)
        {
            Id = id;
            OwnerId = ownerId;
            BuyOrderId = buyOrderId;
            SellOrderId = sellOrderId;
            Amount = amount;
            Price = price;
        }
        public long Id { get;  }
        public int OwnerId { get;  }
        public int BuyOrderId { get;  }
        public int SellOrderId { get;  }
        public int Amount { get;  }
        public int Price { get;  }
    }
}
