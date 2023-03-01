using System;
using Domain;

namespace Application.Tests
{

    public class TestOrder : IOrder
    {
        public int Amount { get; set; }

        public DateTime ExpireTime { get; set; }

        public bool HasCompleted { get; set; }

        public long Id { get; set; }

        public bool IsExpired { get; set; }

        public bool? IsFillAndKill { get; set; }

        public long? OrderParentId { get; set; }

        public OrderStates? OrderState { get; set; }

        public int? OriginalAmount { get; set; }

        public int Price { get; set; }

        public Side Side { get; set; }

        public int DecreaseAmount(int amount)
        {
            throw new NotImplementedException();
        }

        public OrderStates GetOrderState()
        {
            throw new NotImplementedException();
        }

        public void SetStateCancelled()
        {
            throw new NotImplementedException();
        }

        public void SetStateModified()
        {
            throw new NotImplementedException();
        }

        public void SetStateRegistered()
        {
            throw new NotImplementedException();
        }

        public void UpdateBy(IOrder order)
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