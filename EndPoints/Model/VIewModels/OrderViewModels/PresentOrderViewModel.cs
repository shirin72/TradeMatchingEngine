using Domain;

namespace EndPoints.Model.VIewModels.OrderViewModels
{
    public class PresentOrderViewModel : IRestModelBase, IOrder
    {
       

        public int Amount { get; private set; }

        public DateTime ExpireTime { get; private set; }

        public bool HasCompleted { get; private set; }

        public long Id { get; private set; }

        public bool IsExpired { get; private set; }

        public bool? IsFillAndKill { get; private set; }

        public long? OrderParentId { get; private set; }

        public OrderStates? OrderState { get; private set; }

        public int? OriginalAmount { get; private set; }

        public int Price { get; private set; }

        public Side Side { get; private set; }
        public List<Link> Links { get; set; }

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
            throw new NotImplementedException();
        }
    }
}
