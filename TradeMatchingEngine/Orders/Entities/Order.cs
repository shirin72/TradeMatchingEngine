﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMatchingEngine
{

    public class Order
    {
        private OrderState _state;

        public Order(long id, Side side, int price, int amount, DateTime expireTime, bool? isFillAndKill = null, long? orderParentId = null)
        {
            this.Id = id;
            this.Side = side;
            this.Price = price;
            this.Amount = amount;
            this.OriginalAmount = amount;
            this.IsFillAndKill = isFillAndKill;
            this.ExpireTime = expireTime;
            this._state = OrderState.Register;
            this.OrderParentId = orderParentId;
        }

        public OrderState OrderState { get { return _state; } private set { value = _state; } }
        public long Id { get; }

        public Side Side { get; private set; }

        public int Price { get; private set; }
        public int OriginalAmount { get; private set; }

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
        public OrderState GetOrderState() => _state;

        public void SetStateCancelled()
        {
            _state = OrderState.Cancell;
        }

        public long? OrderParentId { get; private set; }

        public void UpdateBy(Order order)
        {
            Price = order.Price;
            OriginalAmount = order.OriginalAmount;
            Amount = order.Amount;
            ExpireTime = order.ExpireTime;
            IsFillAndKill = order.IsFillAndKill;
            Side = order.Side;
        }

    }
}
