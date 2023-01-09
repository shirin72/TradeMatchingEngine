namespace TradeMatchingEngine
{
    public class Order
    {
        public Order()
        {
        }
        public int Id { get; set; }

        public Side Side { get; set; }

        public int Price { get; set; }

        public int Amount { get; set; }

        public bool HasCompleted
        {
            get
            {
                if (Amount <= 0) return true;

                return false;
            }
        }
        public DateTime ExpireTime { get; set; }

        public bool IsExpired { get; set; } = false;
    }
}
