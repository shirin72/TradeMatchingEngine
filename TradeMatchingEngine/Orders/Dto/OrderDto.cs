using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMatchingEngine.Orders.Dto
{
    [Table("Order")]
    public class OrderDto
    {
        private OrderState _state;
        public OrderDto()
        {
            _state = OrderState.Register;
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
            _state = OrderState.Cancell;
        }

    }
}
