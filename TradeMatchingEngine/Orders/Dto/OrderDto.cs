using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMatchingEngine.Orders.Dto
{
    public class OrderDto
    {
        private OrderStates _state;
        public OrderDto()
        {
            _state = OrderStates.Register;
        }

        public int Id { get; set; }

        public string Side { get; set; }

        public int Price { get; set; }

        public int Amount { get;  set; }

        public bool? IsFillAndKill { get; set; } = false;

        public DateTime ExpireTime { get; set; }

        public bool IsExpired { get; set; }

        public int? OrderParentId { get; set; }

        public void SetStateCancelled()
        {
            _state = OrderStates.Cancell;
        }

    }
}
