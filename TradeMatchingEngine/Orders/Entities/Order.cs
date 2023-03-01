using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{

    public class Order : IOrder
    {
        private OrderStates _state;

        internal Order(long id, Side side, int price, int amount, DateTime expireTime, OrderStates? orderState, int? originalAmount = null, bool? isFillAndKill = null, long? orderParentId = null)
        {
            this.Id = id;
            this.Side = side;
            this.Price = price;
            this.Amount = amount;
            this.OriginalAmount = originalAmount ?? amount;
            this.IsFillAndKill = isFillAndKill;
            this.ExpireTime = expireTime;
            _state = orderState == null ? OrderStates.Register : (OrderStates)orderState;
            this.OrderParentId = orderParentId;
        }

        public OrderStates? OrderState { get { return _state; } private set { value = _state; } }
        public long Id { get; }

        public Side Side { get; private set; }

        public int Price { get; private set; }
        public int? OriginalAmount { get; private set; }

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

        public bool? IsFillAndKill { get; private set; } = false;

        public bool HasCompleted
        {
            get
            {
                if (Amount <= 0) return true;

                return false;
            }
        }

        public DateTime ExpireTime { get; private set; }

        public bool IsExpired => ExpireTime < DateTime.Now;
        public OrderStates GetOrderState() => _state;

        public void SetStateCancelled()
        {
            _state = OrderStates.Cancell;
        }
        public void SetStateRegistered()
        {
            _state = OrderStates.Register;
        }
        public void SetStateModified()
        {
            _state = OrderStates.Modifie;
        }
        public long? OrderParentId { get; private set; }

        public void UpdateBy(IOrder order)
        {
            Price = order.Price;
            OriginalAmount = order.OriginalAmount;
            Amount = order.Amount;
            ExpireTime = order.ExpireTime;
            IsFillAndKill = order.IsFillAndKill;
            Side = order.Side;
            _state = (OrderStates)order.OrderState;
        }
        internal Order Clone(int originalAmount)
        {
            return new Order(Id, Side, Price, Amount, ExpireTime, OrderState, originalAmount, IsFillAndKill, OrderParentId);
        }
    }
}
