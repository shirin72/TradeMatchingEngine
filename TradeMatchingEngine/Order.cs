namespace TradeMatchingEngine
{
    public class Order
    {
        private  OrderState _state;

        internal Order(int id, Side side, int price, int amount, DateTime expireTime, bool? isFillAndKill = null)
        {
            Id = id;
            Side = side;
            Price = price;
            Amount = amount;
            IsFillAndKill = isFillAndKill;
            ExpireTime = expireTime;
            _state = OrderState.Register;
        }

        public int Id { get; }

        public Side Side { get; }

        public int Price { get; }

        public int Amount { get; private set; }

        public int DecreaseAmount(int amount)
        {
            Amount = Amount - amount;

            if (Amount <= 0)
            {
                Amount = 0;
            }

            return Amount;
        }

        public bool? IsFillAndKill { get; } = false;

        public bool HasCompleted
        {
            get
            {
                if (Amount <= 0) return true;

                return false;
            }
        }

        public DateTime ExpireTime { get; }

        public bool IsExpired => ExpireTime < DateTime.Now;
        public OrderState GetOrderState() => _state;

        public void SetStateCancelled()
        {
            _state = OrderState.Cancell;
        }
    }

    public enum OrderState
    {
        Cancell,
        Modifie,
        Register
    }
}
