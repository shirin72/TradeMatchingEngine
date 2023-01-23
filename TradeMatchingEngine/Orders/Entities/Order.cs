using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMatchingEngine
{

    public class Order
    {
        private OrderState _state;

        public Order(int Id, Side Side, int Price, int Amount, DateTime ExpireTime, bool? IsFillAndKill = null, int? OrderParentId = null)
        {
            this.Id = Id;
            this.Side = Side;
            this.Price = Price;
            this.Amount = Amount;
            this.IsFillAndKill = IsFillAndKill;
            this.ExpireTime = ExpireTime;
            this._state = OrderState.Register;
            this.OrderParentId = OrderParentId;
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

        public int? OrderParentId { get; }

    }
}
